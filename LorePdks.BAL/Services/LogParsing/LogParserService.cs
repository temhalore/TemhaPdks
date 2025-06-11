using LorePdks.BAL.Services.LogParsing.Interfaces;
using LorePdks.COMMON.Enums;
using LorePdks.COMMON.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace LorePdks.BAL.Services.LogParsing
{
    /// <summary>
    /// Log parsing servisi implementasyonu - Configuration-based parsing sistemi
    /// </summary>
    public class LogParserService : ILogParserService
    {
        /// <summary>
        /// Ham log verisini parse ederek yapılandırılmış veri döndürür
        /// </summary>
        public Dictionary<string, object> ParseLog(string rawLogData, string parserConfig)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(rawLogData))
                    throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, "Ham log verisi boş olamaz");

                if (string.IsNullOrWhiteSpace(parserConfig))
                    throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, "Parser konfigürasyonu boş olamaz");

                var config = JsonConvert.DeserializeObject<LogParserConfig>(parserConfig);
                return ParseWithConfig(rawLogData, config);
            }
            catch (JsonException ex)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"Parser konfigürasyonu geçersiz JSON formatında: {ex.Message}");
            }
        }

        /// <summary>
        /// PDKS cihazından gelen log verisini parse eder
        /// </summary>
        public Dictionary<string, object> ParsePdksLog(string rawLogData, string parserConfig)
        {
            var parsedData = ParseLog(rawLogData, parserConfig);
            
            // PDKS spesifik validasyonlar
            if (!parsedData.ContainsKey("userId") || !parsedData.ContainsKey("timestamp"))
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, "PDKS log verisi userId ve timestamp alanlarını içermelidir");
            }

            return parsedData;
        }

        /// <summary>
        /// Alarm cihazından gelen log verisini parse eder
        /// </summary>
        public Dictionary<string, object> ParseAlarmLog(string rawLogData, string parserConfig)
        {
            var parsedData = ParseLog(rawLogData, parserConfig);
            
            // Alarm spesifik validasyonlar
            if (!parsedData.ContainsKey("alarmType") || !parsedData.ContainsKey("timestamp"))
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, "Alarm log verisi alarmType ve timestamp alanlarını içermelidir");
            }

            return parsedData;
        }

        /// <summary>
        /// Kamera cihazından gelen log verisini parse eder
        /// </summary>
        public Dictionary<string, object> ParseKameraLog(string rawLogData, string parserConfig)
        {
            var parsedData = ParseLog(rawLogData, parserConfig);
            
            // Kamera spesifik validasyonlar
            if (!parsedData.ContainsKey("eventType") || !parsedData.ContainsKey("timestamp"))
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, "Kamera log verisi eventType ve timestamp alanlarını içermelidir");
            }

            return parsedData;
        }

        /// <summary>
        /// Parser konfigürasyonunu test eder
        /// </summary>
        public (bool Success, string Message, Dictionary<string, object> ParsedData) TestParserConfig(string sampleLogData, string parserConfig)
        {
            try
            {
                var parsedData = ParseLog(sampleLogData, parserConfig);
                return (true, "Parser konfigürasyonu başarıyla test edildi", parsedData);
            }
            catch (Exception ex)
            {
                return (false, $"Parser test hatası: {ex.Message}", new Dictionary<string, object>());
            }
        }

        /// <summary>
        /// Parser konfigürasyonunu doğrular
        /// </summary>
        public (bool IsValid, string ErrorMessage) ValidateParserConfig(string parserConfig)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(parserConfig))
                    return (false, "Parser konfigürasyonu boş olamaz");

                var config = JsonConvert.DeserializeObject<LogParserConfig>(parserConfig);
                
                if (config == null)
                    return (false, "Parser konfigürasyonu geçersiz");

                if (string.IsNullOrWhiteSpace(config.Delimiter))
                    return (false, "Delimiter alanı boş olamaz");

                if (config.FieldMapping == null || config.FieldMapping.Count == 0)
                    return (false, "FieldMapping en az bir alan içermelidir");

                return (true, "Konfigürasyon geçerli");
            }
            catch (JsonException ex)
            {
                return (false, $"JSON formatı geçersiz: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"Doğrulama hatası: {ex.Message}");
            }
        }

        /// <summary>
        /// Konfigürasyona göre log verisini parse eder
        /// </summary>
        private Dictionary<string, object> ParseWithConfig(string rawLogData, LogParserConfig config)
        {
            var result = new Dictionary<string, object>();

            try
            {
                string[] fields;

                // Delimiter'a göre ayır
                if (config.Delimiter == "\\t")
                    fields = rawLogData.Split('\t');
                else if (config.Delimiter == "\\n")
                    fields = rawLogData.Split('\n');
                else if (config.Delimiter == "regex")
                {
                    // Regex pattern kullan
                    var regex = new Regex(config.RegexPattern ?? ".*");
                    var match = regex.Match(rawLogData);
                    if (match.Success)
                    {
                        fields = match.Groups.Cast<Group>().Skip(1).Select(g => g.Value).ToArray();
                    }
                    else
                    {
                        throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, "Regex pattern log verisi ile eşleşmiyor");
                    }
                }
                else
                {
                    fields = rawLogData.Split(config.Delimiter.ToCharArray(), StringSplitOptions.None);
                }

                // Field mapping'e göre alanları eşle
                foreach (var mapping in config.FieldMapping)
                {
                    if (mapping.Index >= 0 && mapping.Index < fields.Length)
                    {
                        var value = fields[mapping.Index].Trim();
                        
                        // Tip dönüşümü yap
                        object convertedValue = ConvertValue(value, mapping.Type, mapping.Format);
                        result[mapping.Name] = convertedValue;
                    }
                }

                // Timestamp alanını özel işle
                if (!string.IsNullOrWhiteSpace(config.DateFormat) && result.ContainsKey("timestamp"))
                {
                    if (result["timestamp"] is string timestampStr)
                    {
                        if (DateTime.TryParseExact(timestampStr, config.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                        {
                            result["timestamp"] = parsedDate;
                        }
                    }
                }

                // Ham veriyi de ekle
                result["rawData"] = rawLogData;

                return result;
            }
            catch (Exception ex)
            {
                throw new AppException(MessageCode.ERROR_500_BIR_HATA_OLUSTU, $"Log parse hatası: {ex.Message}");
            }
        }

        /// <summary>
        /// Değeri belirtilen tipe dönüştürür
        /// </summary>
        private object ConvertValue(string value, string type, string format = null)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            switch (type?.ToLowerInvariant())
            {
                case "int":
                case "integer":
                    return int.TryParse(value, out int intValue) ? intValue : 0;

                case "long":
                    return long.TryParse(value, out long longValue) ? longValue : 0L;

                case "double":
                case "decimal":
                    return double.TryParse(value, out double doubleValue) ? doubleValue : 0.0;

                case "bool":
                case "boolean":
                    return bool.TryParse(value, out bool boolValue) ? boolValue : false;

                case "datetime":
                case "date":
                    if (!string.IsNullOrWhiteSpace(format))
                    {
                        return DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateValue) 
                            ? dateValue : DateTime.MinValue;
                    }
                    return DateTime.TryParse(value, out DateTime parsedDate) ? parsedDate : DateTime.MinValue;

                case "string":
                default:
                    return value;
            }
        }
    }

    /// <summary>
    /// Log parser konfigürasyon modeli
    /// </summary>
    public class LogParserConfig
    {
        public string Delimiter { get; set; }
        public string DateFormat { get; set; }
        public string TimeFormat { get; set; }
        public string RegexPattern { get; set; }
        public List<FieldMapping> FieldMapping { get; set; } = new List<FieldMapping>();
    }

    /// <summary>
    /// Alan eşleme modeli
    /// </summary>
    public class FieldMapping
    {
        public string Name { get; set; }
        public int Index { get; set; }
        public string Type { get; set; } = "string";
        public string Format { get; set; }
    }
}
