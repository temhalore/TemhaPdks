namespace Lore.SetupAndDosyaOku.Models
{
    public class FirmaDataOkuSetupBilgiDto
    {
        public string FirmaKod { get; set; } = string.Empty;
        public bool isPdks { get; set; }
        public bool isAlarm { get; set; }
        public bool isKamera { get; set; }
    }

    public class ServiceResponse<T>
    {
        public T? data { get; set; }
        public string message { get; set; } = string.Empty;
        public ServiceResponseMessageType messageType { get; set; }
    }

    public enum ServiceResponseMessageType
    {
        Success,
        Warning,
        Error,
        Info
    }

    public enum WizardStep
    {
        FirmaKodu = 1,
        ModulSecimi = 2,
        DosyaYollari = 3,
        Tamamlama = 4
    }

    public class WizardData
    {
        public string FirmaKod { get; set; } = string.Empty;
        public bool IsPdks { get; set; }
        public bool IsAlarm { get; set; }
        public bool IsKamera { get; set; }
        public string PdksKayitDosyaYolu { get; set; } = string.Empty;
        public string AlarmKayitDosyaYolu { get; set; } = string.Empty;
        public string KameraLogDosyaYolu { get; set; } = string.Empty;
        public bool StartWithWindows { get; set; } = true;
        public bool CreateDesktopShortcut { get; set; } = true;
    }
}
