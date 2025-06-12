using LorePdks.BAL.Services.LogParsing.Interfaces;
using LorePdks.BAL.Managers.FirmaCihaz.Interfaces;
using LorePdks.COMMON.Enums;
using LorePdks.COMMON.Models;
using LorePdks.COMMON.DTO.LogParser;
using LorePdks.COMMON.DTO.FirmaCihaz;
using LorePdks.COMMON.DTO.Firma;
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
        private readonly IFirmaCihazManager _firmaCihazManager;

        public LogParserService(IFirmaCihazManager firmaCihazManager)
        {
            _firmaCihazManager = firmaCihazManager;
        }
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
                    return DateTime.TryParse(value, out DateTime parsedDate) ? parsedDate : DateTime.MinValue;                case "string":
                default:
                    return value;
            }
        }        // CRUD Operations for LogParser Configuration
        /// <summary>
        /// Log parser konfigürasyonu kaydet
        /// </summary>
        public LogParserDTO saveLogParser(LogParserDTO logParserDto)
        {
            try
            {
                // LogParserDTO'yu FirmaCihazDTO'ya çevir
                var firmaCihazDto = ConvertLogParserToFirmaCihaz(logParserDto);
                
                // FirmaCihazManager ile kaydet
                var savedFirmaCihaz = _firmaCihazManager.saveFirmaCihaz(firmaCihazDto);
                
                // Kaydedilen FirmaCihazDTO'yu LogParserDTO'ya çevir
                return ConvertFirmaCihazToLogParser(savedFirmaCihaz);
            }
            catch (Exception ex)
            {
                throw new AppException(MessageCode.ERROR_500_BIR_HATA_OLUSTU, $"Log parser kayıt hatası: {ex.Message}");
            }
        }

        /// <summary>
        /// ID'ye göre log parser konfigürasyonu getir
        /// </summary>
        public LogParserDTO getLogParserById(int id)
        {
            try
            {
                var firmaCihazDto = _firmaCihazManager.getFirmaCihazDtoById(id, true);
                return ConvertFirmaCihazToLogParser(firmaCihazDto);
            }
            catch (Exception ex)
            {
                throw new AppException(MessageCode.ERROR_500_BIR_HATA_OLUSTU, $"Log parser getirme hatası: {ex.Message}");
            }
        }

        /// <summary>
        /// Firmaya göre log parser konfigürasyonları getir
        /// </summary>
        public List<LogParserDTO> getLogParserListByFirmaId(int firmaId)
        {
            try
            {
                var firmaCihazList = _firmaCihazManager.getFirmaCihazDtoListByFirmaId(firmaId);
                return firmaCihazList.Select(ConvertFirmaCihazToLogParser).ToList();
            }
            catch (Exception ex)
            {
                throw new AppException(MessageCode.ERROR_500_BIR_HATA_OLUSTU, $"Firmaya göre log parser listesi getirme hatası: {ex.Message}");
            }
        }

        /// <summary>
        /// Tüm log parser konfigürasyonları getir
        /// </summary>
        public List<LogParserDTO> getAllLogParsers()
        {
            try
            {
                // Tüm firmaları getir ve her firma için cihazları al
                var allLogParsers = new List<LogParserDTO>();
                
                // Bu metod için FirmaCihazManager'da tüm cihazları getirecek bir metod eklenmeli
                // Şimdilik boş liste döndürüyoruz
                return allLogParsers;
            }
            catch (Exception ex)
            {
                throw new AppException(MessageCode.ERROR_500_BIR_HATA_OLUSTU, $"Tüm log parser listesi getirme hatası: {ex.Message}");
            }
        }

        /// <summary>
        /// Log parser konfigürasyonunu sil
        /// </summary>
        public void deleteLogParser(int id)
        {
            try
            {
                _firmaCihazManager.deleteFirmaCihazByFirmaCihazId(id);
            }
            catch (Exception ex)
            {
                throw new AppException(MessageCode.ERROR_500_BIR_HATA_OLUSTU, $"Log parser silme hatası: {ex.Message}");
            }
        }        /// <summary>
        /// FirmaCihazDTO'yu LogParserDTO'ya çevirir
        /// </summary>
        private LogParserDTO ConvertFirmaCihazToLogParser(FirmaCihazDTO firmaCihazDto)
        {
            if (firmaCihazDto == null) return null;

            return new LogParserDTO
            {
                id = firmaCihazDto.id,
                firmaDto = firmaCihazDto.firmaDto,
                ad = firmaCihazDto.ad,
                aciklama = firmaCihazDto.aciklama,
                cihazTip = firmaCihazDto.firmaCihazTipKodDto?.kisaAd ?? "",
                delimiter = firmaCihazDto.logDelimiter,
                dateFormat = firmaCihazDto.logDateFormat,
                timeFormat = firmaCihazDto.logTimeFormat,
                fieldMappingJson = firmaCihazDto.logFieldMapping,
                sampleLogData = firmaCihazDto.logSample,
                aktif = true
            };
        }

        /// <summary>
        /// LogParserDTO'yu FirmaCihazDTO'ya çevirir
        /// </summary>
        private FirmaCihazDTO ConvertLogParserToFirmaCihaz(LogParserDTO logParserDto)
        {
            if (logParserDto == null) return null;

            return new FirmaCihazDTO
            {
                id = logParserDto.id,
                firmaDto = logParserDto.firmaDto,
                ad = logParserDto.ad,
                aciklama = logParserDto.aciklama,
                cihazMakineGercekId = 0, // Bu değer controller'da set edilmeli
                logDelimiter = logParserDto.delimiter,
                logDateFormat = logParserDto.dateFormat,
                logTimeFormat = logParserDto.timeFormat,
                logFieldMapping = logParserDto.fieldMappingJson,
                logSample = logParserDto.sampleLogData,
                logParserConfig = BuildLogParserConfig(logParserDto)
            };
        }

        /// <summary>
        /// LogParserDTO'dan JSON konfigürasyonu oluşturur
        /// </summary>
        private string BuildLogParserConfig(LogParserDTO logParserDto)
        {
            try
            {
                var config = new LogParserConfig
                {
                    Delimiter = logParserDto.delimiter,
                    DateFormat = logParserDto.dateFormat,
                    TimeFormat = logParserDto.timeFormat,
                    RegexPattern = logParserDto.regexPattern
                };

                // FieldMapping JSON'ını parse et
                if (!string.IsNullOrEmpty(logParserDto.fieldMappingJson))
                {
                    config.FieldMapping = JsonConvert.DeserializeObject<List<FieldMapping>>(logParserDto.fieldMappingJson) ?? new List<FieldMapping>();
                }

                return JsonConvert.SerializeObject(config);
            }
            catch (Exception)
            {
                return "{}";
            }
        }        /// <summary>
        /// Konfigürasyon ID'si ile log verisi parse et
        /// </summary>
        public Dictionary<string, object> parseLogData(string rawLogData, int configId)
        {
            try
            {
                var config = getLogParserById(configId);
                if (config == null)
                {
                    throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"ID: {configId} ile log parser konfigürasyonu bulunamadı");
                }

                return ParseLog(rawLogData, config.fieldMappingJson ?? "{}");
            }
            catch (Exception ex)
            {
                throw new AppException(MessageCode.ERROR_500_BIR_HATA_OLUSTU, $"Log parse hatası: {ex.Message}");
            }
        }        /// <summary>
        /// Log parser konfigürasyonunu test et
        /// </summary>
        public Dictionary<string, object> testLogParserConfig(string sampleLogData, string config)
        {
            var result = TestParserConfig(sampleLogData, config);
            if (result.Success)
            {
                return result.ParsedData;
            }
            else
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, result.Message);
            }
        }
    }    /// <summary>
    /// Log parser konfigürasyon modeli
    /// </summary>
    public class LogParserConfig
    {
        public string? Delimiter { get; set; }
        public string? DateFormat { get; set; }
        public string? TimeFormat { get; set; }
        public string? RegexPattern { get; set; }
        public List<FieldMapping> FieldMapping { get; set; } = new List<FieldMapping>();
    }

    /// <summary>
    /// Alan eşleme modeli
    /// </summary>
    public class FieldMapping
    {
        public string? Name { get; set; }
        public int Index { get; set; }
        public string Type { get; set; } = "string";
        public string? Format { get; set; }
    }
}
