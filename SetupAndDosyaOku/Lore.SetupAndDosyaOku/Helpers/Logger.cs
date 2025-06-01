using Serilog;
using Serilog.Events;
using System.Diagnostics;

namespace Lore.SetupAndDosyaOku.Helpers
{
    public class Logger
    {
        private static readonly object _lockObject = new();
        private string _logDirectory = string.Empty;
        private string _currentLogFile = string.Empty;
        private DateTime _startTime;
        
        private ILogger _logger;
        
        // Static instance for static methods
        private static Logger? _instance;
        
        /// <summary>
        /// Static instance for use in static methods
        /// </summary>
        private static Logger Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Logger();
                }
                return _instance;
            }
        }
        
        public Logger()
        {
            _startTime = DateTime.Now;
            
            // Uygulama dizinindeki Logs klasörünü kullan
            string appDir = AppDomain.CurrentDomain.BaseDirectory;
            _logDirectory = Path.Combine(appDir, "Logs");
            
            // Log dizini yoksa oluştur
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
            
            InitializeLogger();
        }
          private void InitializeLogger()
        {
            UpdateCurrentLogFile();
            
            _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(Path.Combine(_logDirectory, "log-debug-.txt"),
                    restrictedToMinimumLevel: LogEventLevel.Debug,
                    rollingInterval: RollingInterval.Day)
                .WriteTo.File(Path.Combine(_logDirectory, "log-info-.txt"),
                    restrictedToMinimumLevel: LogEventLevel.Information,
                    rollingInterval: RollingInterval.Day)
                .WriteTo.File(Path.Combine(_logDirectory, "log-error-.txt"),
                    restrictedToMinimumLevel: LogEventLevel.Error,
                    rollingInterval: RollingInterval.Day)
                .CreateLogger();
                
            Info("=== Lore SetupAndDosyaOku Başlatılıyor ===");
            Info($"Çalışma dizini: {AppDomain.CurrentDomain.BaseDirectory}");
        }
        
        private void UpdateCurrentLogFile()
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            _currentLogFile = Path.Combine(_logDirectory, $"log-{today}.txt");
        }
          #region Instance Methods
        
        public void Debug(string message)
        {
            _logger.Debug(message);
            WriteToConsoleIf(message, ConsoleColor.Gray);
        }
        
        public void Info(string message)
        {
            _logger.Information(message);
            WriteToConsoleIf(message, ConsoleColor.White);
        }
        
        public void Warning(string message)
        {
            _logger.Warning(message);
            WriteToConsoleIf(message, ConsoleColor.Yellow);
        }
        
        public void Error(string message, Exception? ex = null)
        {
            if (ex != null)
            {
                _logger.Error(ex, $"{message} | Hata: {ex.Message}");
                WriteToConsoleIf($"{message} | Hata: {ex.Message}", ConsoleColor.Red);
                
                // Windows Olay Günlüğü'ne de yazalım
                try
                {
                    EventLog.WriteEntry("LoreSetupAndDosyaOku", 
                        $"{message} | Hata: {ex.Message}\nStack: {ex.StackTrace}", 
                        EventLogEntryType.Error);
                }
                catch
                {
                    // Olay günlüğüne yazma başarısız olursa sessizce devam et
                }
            }
            else
            {
                _logger.Error(message);
                WriteToConsoleIf(message, ConsoleColor.Red);
            }
        }
        
        public void Critical(string message, Exception? ex = null)
        {
            if (ex != null)
            {
                _logger.Fatal(ex, $"{message} | Kritik Hata: {ex.Message}");
                WriteToConsoleIf($"{message} | Kritik Hata: {ex.Message}", ConsoleColor.Magenta);
                
                // Windows Olay Günlüğü'ne de yazalım
                try
                {
                    EventLog.WriteEntry("LoreSetupAndDosyaOku", 
                        $"{message} | Kritik Hata: {ex.Message}\nStack: {ex.StackTrace}", 
                        EventLogEntryType.Error);
                }
                catch
                {
                    // Olay günlüğüne yazma başarısız olursa sessizce devam et
                }
            }
            else
            {
                _logger.Fatal(message);
                WriteToConsoleIf(message, ConsoleColor.Magenta);
            }
        }
        
        #endregion
        
        #region Static Methods For Backward Compatibility
        
        public static void WriteDebug(string message)
        {
            Instance.Debug(message);
        }
        
        public static void WriteInfo(string message)
        {
            Instance.Info(message);
        }
        
        public static void WriteWarning(string message)
        {
            Instance.Warning(message);
        }
        
        public static void WriteError(string message, Exception? ex = null)
        {
            Instance.Error(message, ex);
        }
        
        public static void WriteCritical(string message, Exception? ex = null)
        {
            Instance.Critical(message, ex);
        }
        
        #endregion
        
        private void WriteToConsoleIf(string message, ConsoleColor color)
        {
            // Debug modunda veya konsol uygulamasıysa konsola yaz
            if (Environment.UserInteractive)
            {
                lock (_lockObject)
                {
                    ConsoleColor originalColor = Console.ForegroundColor;
                    Console.ForegroundColor = color;
                    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}");
                    Console.ForegroundColor = originalColor;
                }
            }
        }        public TimeSpan GetUptime()
        {
            return DateTime.Now - _startTime;
        }
        
        public static TimeSpan GetUptimeStatic()
        {
            return Instance.GetUptime();
        }
        
        public void CleanUpOldLogs(int daysToKeep = 30)
        {
            try
            {
                Info($"Eski log dosyaları temizleniyor ({daysToKeep} günden eski)...");
                
                if (!Directory.Exists(_logDirectory))
                    return;
                
                var cutOffDate = DateTime.Now.AddDays(-daysToKeep);
                var logFiles = Directory.GetFiles(_logDirectory, "*.txt");
                int deletedCount = 0;
                
                foreach (var logFile in logFiles)
                {
                    var fileInfo = new FileInfo(logFile);
                    if (fileInfo.CreationTime < cutOffDate)
                    {
                        File.Delete(logFile);
                        deletedCount++;
                    }
                }
                
                Info($"{deletedCount} eski log dosyası silindi.");
            }
            catch (Exception ex)
            {
                WriteError("Eski log dosyaları silinirken hata oluştu", ex);
            }
        }
          // This is now handled by the instance method GetUptime() and static method GetUptimeStatic()
        
        public static string GetUptimeString()
        {
            var uptime = GetUptimeStatic();
            return $"{uptime.Days} gün, {uptime.Hours} saat, {uptime.Minutes} dakika, {uptime.Seconds} saniye";
        }
    }
}
