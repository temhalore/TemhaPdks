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
                    return (false, "Delimiter alanı boş olamaz");                if (config.FieldMappings == null || config.FieldMappings.Count == 0)
                    return (false, "FieldMappings en az bir alan içermelidir");

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
                }                // Field mapping'e göre alanları eşle
                foreach (var mapping in config.FieldMappings)
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
                };                // FieldMappings JSON'ını parse et
                if (!string.IsNullOrEmpty(logParserDto.fieldMappingJson))
                {
                    config.FieldMappings = JsonConvert.DeserializeObject<List<FieldMapping>>(logParserDto.fieldMappingJson) ?? new List<FieldMapping>();
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

        /// <summary>
        /// Sistem şablonlarını getirir
        /// </summary>
        public List<LogParserTemplateDTO> GetSystemTemplates()
        {
            return new List<LogParserTemplateDTO>
            {
                // PDKS Şablonları
                new LogParserTemplateDTO
                {
                    templateName = "ZKTeco PDKS - Standart Format",
                    description = "ZKTeco cihazlar için standart PDKS log formatı (UserID,Time,Date,Direction,DeviceID)",
                    deviceType = "PDKS",
                    deviceBrand = "ZKTeco",
                    logDelimiter = ",",
                    logDateFormat = "ddMMyy",
                    logTimeFormat = "HH:mm",
                    sampleLogData = "00007,14:00,060125,1,001\n00012,14:15,060125,2,001\n00003,08:30,070125,1,001",
                    logFieldMapping = JsonConvert.SerializeObject(new List<FieldMappingDetail>
                    {
                        new FieldMappingDetail { fieldName = "userId", displayName = "Kullanıcı ID", mappingType = FieldMappingType.Position, position = 1, dataType = "string", isRequired = true },
                        new FieldMappingDetail { fieldName = "time", displayName = "Saat", mappingType = FieldMappingType.Position, position = 2, dataType = "time", format = "HH:mm", isRequired = true },
                        new FieldMappingDetail { fieldName = "date", displayName = "Tarih", mappingType = FieldMappingType.Position, position = 3, dataType = "date", format = "ddMMyy", isRequired = true },
                        new FieldMappingDetail { fieldName = "direction", displayName = "Yön (1=Giriş, 2=Çıkış)", mappingType = FieldMappingType.Position, position = 4, dataType = "int", isRequired = true },
                        new FieldMappingDetail { fieldName = "deviceId", displayName = "Cihaz ID", mappingType = FieldMappingType.Position, position = 5, dataType = "string", isRequired = false }
                    }),
                    isSystemTemplate = true
                },
                new LogParserTemplateDTO
                {
                    templateName = "Hikvision PDKS - Tab Ayrımlı",
                    description = "Hikvision cihazlar için tab ile ayrılmış PDKS log formatı",
                    deviceType = "PDKS",
                    deviceBrand = "Hikvision",
                    logDelimiter = "\t",
                    logDateFormat = "yyyy-MM-dd",
                    logTimeFormat = "HH:mm:ss",
                    sampleLogData = "123456\t2025-01-06\t14:30:15\tIN\tDoor1\n789012\t2025-01-06\t14:32:20\tOUT\tDoor1",
                    logFieldMapping = JsonConvert.SerializeObject(new List<FieldMappingDetail>
                    {
                        new FieldMappingDetail { fieldName = "userId", displayName = "Kullanıcı ID", mappingType = FieldMappingType.Position, position = 1, dataType = "string", isRequired = true },
                        new FieldMappingDetail { fieldName = "date", displayName = "Tarih", mappingType = FieldMappingType.Position, position = 2, dataType = "date", format = "yyyy-MM-dd", isRequired = true },
                        new FieldMappingDetail { fieldName = "time", displayName = "Saat", mappingType = FieldMappingType.Position, position = 3, dataType = "time", format = "HH:mm:ss", isRequired = true },
                        new FieldMappingDetail { fieldName = "direction", displayName = "Yön (IN/OUT)", mappingType = FieldMappingType.Position, position = 4, dataType = "string", isRequired = true },
                        new FieldMappingDetail { fieldName = "deviceId", displayName = "Kapı/Cihaz", mappingType = FieldMappingType.Position, position = 5, dataType = "string", isRequired = false }
                    }),
                    isSystemTemplate = true
                },

                // ALARM Şablonları  
                new LogParserTemplateDTO
                {
                    templateName = "Genel Alarm Sistemi - Kelime Bazlı",
                    description = "Genel alarm sistemleri için kelime bazlı log parsing (ALARM: sonrası bilgi yakalama)",
                    deviceType = "ALARM",
                    deviceBrand = "Genel",
                    logDelimiter = " ",
                    logDateFormat = "yyyy-MM-dd",
                    logTimeFormat = "HH:mm:ss",
                    sampleLogData = "2025-01-06 14:30:15 ALARM: Zone1 Motion Detected\n2025-01-06 14:35:20 ALARM: Door2 Opened\n2025-01-06 14:40:10 STATUS: System Armed",
                    logFieldMapping = JsonConvert.SerializeObject(new List<FieldMappingDetail>
                    {
                        new FieldMappingDetail { fieldName = "date", displayName = "Tarih", mappingType = FieldMappingType.Position, position = 1, dataType = "date", format = "yyyy-MM-dd", isRequired = true },
                        new FieldMappingDetail { fieldName = "time", displayName = "Saat", mappingType = FieldMappingType.Position, position = 2, dataType = "time", format = "HH:mm:ss", isRequired = true },
                        new FieldMappingDetail { fieldName = "alarmType", displayName = "Alarm Tipi", mappingType = FieldMappingType.Keyword, keyword = "ALARM:", dataType = "string", isRequired = true },
                        new FieldMappingDetail { fieldName = "statusType", displayName = "Durum Tipi", mappingType = FieldMappingType.Keyword, keyword = "STATUS:", dataType = "string", isRequired = false }
                    }),
                    isSystemTemplate = true
                },
                new LogParserTemplateDTO
                {
                    templateName = "DSC Alarm - Structured Format",
                    description = "DSC alarm sistemleri için yapılandırılmış log formatı",
                    deviceType = "ALARM",
                    deviceBrand = "DSC",
                    logDelimiter = "|",
                    logDateFormat = "dd/MM/yyyy",
                    logTimeFormat = "HH:mm:ss",
                    sampleLogData = "06/01/2025|14:30:15|ZONE|001|MOTION|TRIGGERED\n06/01/2025|14:35:20|USER|123|ARMED|SYSTEM",
                    logFieldMapping = JsonConvert.SerializeObject(new List<FieldMappingDetail>
                    {
                        new FieldMappingDetail { fieldName = "date", displayName = "Tarih", mappingType = FieldMappingType.Position, position = 1, dataType = "date", format = "dd/MM/yyyy", isRequired = true },
                        new FieldMappingDetail { fieldName = "time", displayName = "Saat", mappingType = FieldMappingType.Position, position = 2, dataType = "time", format = "HH:mm:ss", isRequired = true },
                        new FieldMappingDetail { fieldName = "sourceType", displayName = "Kaynak Tipi", mappingType = FieldMappingType.Position, position = 3, dataType = "string", isRequired = true },
                        new FieldMappingDetail { fieldName = "sourceId", displayName = "Kaynak ID", mappingType = FieldMappingType.Position, position = 4, dataType = "string", isRequired = true },
                        new FieldMappingDetail { fieldName = "eventType", displayName = "Olay Tipi", mappingType = FieldMappingType.Position, position = 5, dataType = "string", isRequired = true },
                        new FieldMappingDetail { fieldName = "status", displayName = "Durum", mappingType = FieldMappingType.Position, position = 6, dataType = "string", isRequired = true }
                    }),
                    isSystemTemplate = true
                },

                // KAMERA Şablonları
                new LogParserTemplateDTO
                {
                    templateName = "Hikvision Kamera - Event Log",
                    description = "Hikvision kameralar için event log formatı",
                    deviceType = "KAMERA",
                    deviceBrand = "Hikvision",
                    logDelimiter = ",",
                    logDateFormat = "yyyy-MM-dd",
                    logTimeFormat = "HH:mm:ss",
                    sampleLogData = "2025-01-06,14:30:15,CAM001,MOTION_DETECTED,Zone1\n2025-01-06,14:35:20,CAM002,FACE_DETECTED,Entrance",
                    logFieldMapping = JsonConvert.SerializeObject(new List<FieldMappingDetail>
                    {
                        new FieldMappingDetail { fieldName = "date", displayName = "Tarih", mappingType = FieldMappingType.Position, position = 1, dataType = "date", format = "yyyy-MM-dd", isRequired = true },
                        new FieldMappingDetail { fieldName = "time", displayName = "Saat", mappingType = FieldMappingType.Position, position = 2, dataType = "time", format = "HH:mm:ss", isRequired = true },
                        new FieldMappingDetail { fieldName = "cameraId", displayName = "Kamera ID", mappingType = FieldMappingType.Position, position = 3, dataType = "string", isRequired = true },
                        new FieldMappingDetail { fieldName = "eventType", displayName = "Olay Tipi", mappingType = FieldMappingType.Position, position = 4, dataType = "string", isRequired = true },
                        new FieldMappingDetail { fieldName = "location", displayName = "Konum", mappingType = FieldMappingType.Position, position = 5, dataType = "string", isRequired = false }
                    }),
                    isSystemTemplate = true
                }
            };
        }

        /// <summary>
        /// Cihaz tipine göre şablonları filtreler
        /// </summary>
        public List<LogParserTemplateDTO> GetTemplatesByDeviceType(string deviceType)
        {
            var allTemplates = GetSystemTemplates();
            return allTemplates.Where(t => t.deviceType.Equals(deviceType, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        /// <summary>
        /// Şablondan konfigürasyon oluşturur
        /// </summary>
        public string CreateConfigFromTemplate(LogParserTemplateDTO template)
        {
            var fieldMappings = JsonConvert.DeserializeObject<List<FieldMappingDetail>>(template.logFieldMapping);
            
            var config = new LogParserConfig
            {
                Type = template.deviceType.ToLower(),
                Delimiter = template.logDelimiter,
                DateFormat = template.logDateFormat,
                TimeFormat = template.logTimeFormat,
                FieldMappings = fieldMappings.Select(fm => new FieldMapping
                {
                    Field = fm.fieldName,
                    Position = fm.position,
                    Type = fm.dataType,
                    Pattern = !string.IsNullOrEmpty(fm.keyword) ? fm.keyword : fm.pattern,
                    Format = fm.format
                }).ToList()
            };

            return JsonConvert.SerializeObject(config, Formatting.Indented);
        }

        /// <summary>
        /// Akıllı örnek veri analizi yaparak alan eşlemesi önerir
        /// </summary>
        public List<FieldMappingDetail> AnalyzeSampleDataAndSuggestMappings(string sampleData, string delimiter)
        {
            var suggestions = new List<FieldMappingDetail>();
            
            if (string.IsNullOrWhiteSpace(sampleData)) return suggestions;

            var lines = sampleData.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length == 0) return suggestions;

            var firstLine = lines.First().Trim();
            var fields = firstLine.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i].Trim();
                var suggestion = AnalyzeFieldContent(field, i + 1);
                suggestions.Add(suggestion);
            }

            return suggestions;
        }

        /// <summary>
        /// Alan içeriğini analiz ederek tipini tahmin eder
        /// </summary>
        private FieldMappingDetail AnalyzeFieldContent(string fieldContent, int position)
        {
            var suggestion = new FieldMappingDetail
            {
                position = position,
                mappingType = FieldMappingType.Position
            };

            // Sayısal değer kontrolü
            if (int.TryParse(fieldContent, out int intValue))
            {
                if (intValue >= 1 && intValue <= 10) // Muhtemelen direction
                {
                    suggestion.fieldName = "direction";
                    suggestion.displayName = "Yön";
                    suggestion.dataType = "int";
                }
                else if (fieldContent.Length >= 3) // Muhtemelen user ID
                {
                    suggestion.fieldName = "userId";
                    suggestion.displayName = "Kullanıcı ID";
                    suggestion.dataType = "string";
                }
                else
                {
                    suggestion.fieldName = $"field{position}";
                    suggestion.displayName = $"Alan {position}";
                    suggestion.dataType = "int";
                }
                return suggestion;
            }

            // Tarih formatı kontrolü
            if (DateTime.TryParseExact(fieldContent, new[] { "ddMMyy", "dd/MM/yyyy", "yyyy-MM-dd", "dd.MM.yyyy" }, 
                CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                suggestion.fieldName = "date";
                suggestion.displayName = "Tarih";
                suggestion.dataType = "date";
                suggestion.format = DetectDateFormat(fieldContent);
                return suggestion;
            }

            // Saat formatı kontrolü
            if (DateTime.TryParseExact(fieldContent, new[] { "HH:mm", "HH:mm:ss", "H:mm", "H:mm:ss" }, 
                CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            {
                suggestion.fieldName = "time";
                suggestion.displayName = "Saat";
                suggestion.dataType = "time";
                suggestion.format = DetectTimeFormat(fieldContent);
                return suggestion;
            }

            // Yön kontrolü (string)
            if (fieldContent.ToUpper() == "IN" || fieldContent.ToUpper() == "OUT" || 
                fieldContent.ToUpper() == "GIRIŞ" || fieldContent.ToUpper() == "ÇIKIŞ")
            {
                suggestion.fieldName = "direction";
                suggestion.displayName = "Yön";
                suggestion.dataType = "string";
                return suggestion;
            }

            // Varsayılan string alan
            suggestion.fieldName = $"field{position}";
            suggestion.displayName = $"Alan {position}";
            suggestion.dataType = "string";
            
            return suggestion;
        }

        /// <summary>
        /// Tarih formatını tespit eder
        /// </summary>
        private string DetectDateFormat(string dateString)
        {
            var formats = new[] { "ddMMyy", "dd/MM/yyyy", "yyyy-MM-dd", "dd.MM.yyyy", "MM/dd/yyyy" };
            
            foreach (var format in formats)
            {
                if (DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                    return format;
            }
            
            return "yyyy-MM-dd"; // varsayılan
        }

        /// <summary>
        /// Saat formatını tespit eder
        /// </summary>
        private string DetectTimeFormat(string timeString)
        {
            var formats = new[] { "HH:mm:ss", "HH:mm", "H:mm:ss", "H:mm" };
            
            foreach (var format in formats)
            {
                if (DateTime.TryParseExact(timeString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                    return format;
            }
            
            return "HH:mm:ss"; // varsayılan
        }
    }    /// <summary>
    /// Log parser konfigürasyon modeli
    /// </summary>
    public class LogParserConfig
    {
        public string? Type { get; set; }
        public string? Delimiter { get; set; }
        public string? DateFormat { get; set; }
        public string? TimeFormat { get; set; }
        public string? RegexPattern { get; set; }
        public List<FieldMapping> FieldMappings { get; set; } = new List<FieldMapping>();
    }

    /// <summary>
    /// Alan eşleme modeli
    /// </summary>
    public class FieldMapping
    {
        public string? Field { get; set; }
        public int Position { get; set; }
        public string Type { get; set; } = "string";
        public string? Format { get; set; }
        public string? Pattern { get; set; }
        public string? Name { get; set; }
        public int Index { get; set; }
    }
}
