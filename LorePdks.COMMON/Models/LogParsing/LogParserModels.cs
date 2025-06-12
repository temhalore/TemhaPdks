using System;
using System.Collections.Generic;
using System.Text.Json;

namespace LorePdks.COMMON.Models.LogParsing
{
    /// <summary>
    /// Log parsing konfigürasyon modeli
    /// </summary>
    public class LogParserConfig
    {
        public string Type { get; set; } // "PDKS", "ALARM", "KAMERA"
        public int SkipLines { get; set; } = 0
        public string Encoding { get; set; } = "UTF-8";
        public bool HasHeader { get; set; } = false;
        public List<ValidationRule> ValidationRules { get; set; } = new();
    }

    /// <summary>
    /// Alan validation kuralları
    /// </summary>
    public class ValidationRule
    {
        public string Field { get; set; }
        public bool Required { get; set; }
        public string Type { get; set; } // "numeric", "string", "datetime"
        public string Pattern { get; set; } // Regex pattern
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
    }

    /// <summary>
    /// Alan pozisyon mapping
    /// </summary>
    public class LogFieldMapping
    {
        public int? UserId { get; set; }
        public int? Time { get; set; }
        public int? Date { get; set; }
        public int? Direction { get; set; } // 1: giriş, 0: çıkış
        public int? DeviceId { get; set; }
        public int? AlarmType { get; set; }
        public int? AlarmLevel { get; set; }
        public int? CameraId { get; set; }
        public int? EventType { get; set; }
    }

    /// <summary>
    /// Parse edilmiş log satırı
    /// </summary>
    public class ParsedLogEntry
    {
        public string OriginalLine { get; set; }
        public DateTime? ProcessedDateTime { get; set; }
        public string UserId { get; set; }
        public string DeviceId { get; set; }
        public LogEntryType EntryType { get; set; }
        public Dictionary<string, object> CustomFields { get; set; } = new();
        public bool IsValid { get; set; }
        public List<string> ValidationErrors { get; set; } = new();
    }

    /// <summary>
    /// Log giriş tipleri
    /// </summary>
    public enum LogEntryType
    {
        PdksEntry = 1,
        PdksExit = 0,
        AlarmTriggered = 10,
        AlarmCleared = 11,
        CameraMotion = 20,
        CameraRecording = 21,
        SystemEvent = 30
    }
}
