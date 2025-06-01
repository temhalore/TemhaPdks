namespace Lore.SetupAndDosyaOku.Models
{    public class AppSettings
    {
        public string FirmaKod { get; set; } = string.Empty;
        public string PdksKayitDosyaYolu { get; set; } = string.Empty;
        public string AlarmKayitDosyaYolu { get; set; } = string.Empty;
        public string KameraLogDosyaYolu { get; set; } = string.Empty;
        public string ApiEndpoint { get; set; } = string.Empty;
        public string InstallationPath { get; set; } = string.Empty;
        public bool IsDebugMode { get; set; } = false;
    }

    public class LoggingSettings
    {
        public LogLevel LogLevel { get; set; } = new LogLevel();
    }

    public class LogLevel
    {
        public string Default { get; set; } = "Information";
        public string Microsoft { get; set; } = "Warning";
        public string MicrosoftHostingLifetime { get; set; } = "Information";
    }

    public class AppConfiguration
    {
        public AppSettings AppSettings { get; set; } = new AppSettings();
        public LoggingSettings Logging { get; set; } = new LoggingSettings();
    }
}
