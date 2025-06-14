using LorePdks.COMMON.DTO.Base;

namespace LorePdks.COMMON.DTO.LogParser
{
    /// <summary>
    /// Log Parser Şablonu DTO
    /// </summary>
    public class LogParserTemplateDTO : BaseDTO
    {
        public string templateName { get; set; } = "";
        public string description { get; set; } = "";
        public string deviceType { get; set; } = ""; // PDKS, ALARM, KAMERA
        public string deviceBrand { get; set; } = ""; // ZKTeco, Hikvision vs
        public string logParserConfig { get; set; } = "";
        public string logDelimiter { get; set; } = "";
        public string logDateFormat { get; set; } = "";
        public string logTimeFormat { get; set; } = "";
        public string logFieldMapping { get; set; } = "";
        public string sampleLogData { get; set; } = "";
        public bool isSystemTemplate { get; set; } = false; // Sistem şablonu mu yoksa kullanıcı şablonu mu
        public bool isActive { get; set; } = true;
    }

    /// <summary>
    /// Alan eşleme tipi enum
    /// </summary>
    public enum FieldMappingType
    {
        Position = 1,    // Pozisyon bazlı (PDKS için: 1. alan, 2. alan...)
        Keyword = 2,     // Kelime bazlı (Alarm için: "ALARM:" sonrası...)
        Regex = 3        // Regex bazlı (Kompleks formatlar için)
    }

    /// <summary>
    /// Alan eşleme detayı
    /// </summary>
    public class FieldMappingDetail
    {
        public string fieldName { get; set; } = "";        // userId, timestamp, direction vs
        public string displayName { get; set; } = "";      // Türkçe açıklama
        public FieldMappingType mappingType { get; set; }  // Position, Keyword, Regex
        public int position { get; set; } = 0;             // Position bazlı için (1, 2, 3...)
        public string keyword { get; set; } = "";          // Keyword bazlı için ("ALARM:", "USER:" vs)
        public string pattern { get; set; } = "";          // Regex bazlı için
        public string dataType { get; set; } = "string";   // string, int, datetime, bool
        public string format { get; set; } = "";           // Datetime için format
        public bool isRequired { get; set; } = false;      // Zorunlu alan mı
    }
}
