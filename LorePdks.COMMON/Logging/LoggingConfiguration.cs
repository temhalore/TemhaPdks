using LorePdks.COMMON.Logging.Serilog.LogUsingModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.MSSqlServer;
using System.Security.AccessControl;

namespace LorePdks.COMMON.Logging
{
    public static class LoggingConfiguration
    {
        public static LoggerConfiguration Configuration(IConfiguration config, string patern = "logsPaternsiz", string sink = "")
        {
            var logConfig = config.Get<LogingConfigurationModel>() ??
                        throw new Exception("Null Options Message");

            if (string.IsNullOrEmpty(sink))
            {
                sink = logConfig.Serilog.ActiveSink;
            }

            var seriLogConfig = new LoggerConfiguration()
               .Enrich.FromLogContext()
               .Enrich.With(new ThreadIdEnricher())
               .Enrich.WithProperty("Application", logConfig.ProjectName)
               //.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
               //.MinimumLevel.Override("System", LogEventLevel.Warning)
               .MinimumLevel
               .Verbose();


            if (logConfig.Serilog.ActiveSink == "Elasticsearch")
            {
                if (!string.IsNullOrEmpty(logConfig.Serilog.Elasticsearch.Host))
                {
                    string indexFormat = $"{logConfig.ProjectName}-{patern}-" + "{0:yyyy-MM-dd}";
                    if (patern != "logs")
                        indexFormat = $"{logConfig.ProjectName}-{patern}-" + "{0:yyyy-MM}";
                    seriLogConfig.WriteTo.Elasticsearch(
                        new ElasticsearchSinkOptions(new Uri(logConfig.Serilog.Elasticsearch.Host))
                        {
                            AutoRegisterTemplate = true,
                            AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                            IndexFormat = indexFormat,
                            IndexAliases = new[] { patern },
                            TemplateName = $"{logConfig.ProjectName}-{patern}",
                            ModifyConnectionSettings = x => x.BasicAuthentication(logConfig.Serilog.Elasticsearch.Username, logConfig.Serilog.Elasticsearch.Password),
                            CustomFormatter = new ElasticsearchJsonFormatter(),
                            //FailureCallback = e => new FileLogger().Fatal("Unable to submit event " + e.MessageTemplate),
                            EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog |
                                       EmitEventFailureHandling.WriteToFailureSink |
                                       EmitEventFailureHandling.RaiseCallback,
                        });
                }
                else
                {
                    throw new Exception("Elastic Search Url can't read from appsetting.json");
                }
            }
            else if (sink == "File")
            {
                seriLogConfig.WriteTo.File(
                    // CreateLogFilePath(config, patern+"\\"+DateTime.Now.ToString("yyyy-MM-dd")),
                    CreateLogFilePath(config, patern),
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: null,
                        fileSizeLimitBytes: 500000, //500MB
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}");
            }
            else if (logConfig.Serilog.ActiveSink == "MSSqlServer")
            {
                var sinkOpts = new MSSqlServerSinkOptions
                {
                    TableName = "Logs",
                    AutoCreateSqlTable = true,
                    BatchPostingLimit = 1
                };

                var columnOptions = new ColumnOptions();
                columnOptions.Store.Remove(StandardColumn.Properties);
                columnOptions.Store.Remove(StandardColumn.Exception);
                columnOptions.Store.Remove(StandardColumn.MessageTemplate);

                if (!string.IsNullOrEmpty(logConfig.Serilog.MSSqlServer.ConnectionString))
                {
                    seriLogConfig.WriteTo.MSSqlServer(
                         connectionString: logConfig.Serilog.MSSqlServer.ConnectionString,
                        sinkOptions: sinkOpts,
                        columnOptions: columnOptions
                    );
                }
                else
                {
                    throw new Exception("MSSqlServer ConnectionString can't read from appsetting.json");
                }
            }

            return seriLogConfig;
        }

        public static string CreateLogFilePath(IConfiguration config, string pattern = "logs")
        {
            try
            {
                // Config kontrolü ve alınması
                var logConfig = config.Get<LogingConfigurationModel>() ??
                    throw new Exception("Logging configuration is null");

                // Ana log klasörü yolu oluşturma
                string baseLogDirectory = Path.Combine(
                    logConfig.Serilog.File.Path.TrimEnd('\\'),
                    logConfig.ProjectName,
                    pattern);

                // Klasör yapısını ve izinleri ayarla
                EnsureDirectoryExists(baseLogDirectory);

                // Log dosyası yolu
                string logFilePath = Path.Combine(baseLogDirectory, "logs.txt");

                // Dosya izinlerini ayarla
                EnsureFilePermissions(logFilePath);

                return logFilePath;
            }
            catch (Exception ex)
            {
                // Hata durumunda fallback mekanizması
                try
                {
                    string fallbackDirectory = Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "FallbackLogs",
                        pattern);

                    EnsureDirectoryExists(fallbackDirectory);

                    string fallbackPath = Path.Combine(fallbackDirectory, "error_logs.txt");

                    // Hatayı loglama
                    LogCreationError(ex, fallbackPath);

                    return fallbackPath;
                }
                catch (Exception fallbackEx)
                {
                    // Son çare: Temp klasörüne yazma
                    string tempPath = Path.Combine(
                        Path.GetTempPath(),
                        "EmergencyLogs",
                        "emergency_log.txt");

                    Directory.CreateDirectory(Path.GetDirectoryName(tempPath));

                    File.AppendAllText(tempPath,
                        $"Critical Error in Log Creation: {DateTime.Now}\n" +
                        $"Original Error: {ex.Message}\n" +
                        $"Fallback Error: {fallbackEx.Message}\n\n");

                    return tempPath;
                }
            }
        }

        private static void EnsureFilePermissions(string filePath)
        {
            try
            {
                // Dosya yoksa oluştur
                if (!File.Exists(filePath))
                {
                    using (File.Create(filePath)) { }
                }

                // FileInfo kullanarak izinleri ayarla
                FileInfo fileInfo = new FileInfo(filePath);

#if WINDOWS
        // Windows sistemlerde izinleri ayarla
        try
        {
            var fileSecurity = fileInfo.GetAccessControl();

            var everyoneRule = new FileSystemAccessRule(
                "Everyone",
                FileSystemRights.Modify,
                AccessControlType.Allow);

            var authenticatedUserRule = new FileSystemAccessRule(
                "Authenticated Users",
                FileSystemRights.Modify,
                AccessControlType.Allow);

            fileSecurity.AddAccessRule(everyoneRule);
            fileSecurity.AddAccessRule(authenticatedUserRule);

            fileInfo.SetAccessControl(fileSecurity);
        }
        catch (PlatformNotSupportedException)
        {
            // Windows dışı sistemlerde bu kısmı atla
        }
#endif

                // Dosyanın yazılabilir olduğundan emin ol
                if (!fileInfo.IsReadOnly)
                {
                    fileInfo.IsReadOnly = false;
                }
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to set file permissions: {filePath}", ex);
            }
        }

        private static void EnsureDirectoryExists(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                try
                {
                    // Klasörü oluştur
                    Directory.CreateDirectory(directoryPath);

                    // DirectoryInfo kullanarak izinleri ayarla
                    DirectoryInfo dirInfo = new DirectoryInfo(directoryPath);

#if WINDOWS
            // Windows sistemlerde izinleri ayarla
            try
            {
                var dirSecurity = dirInfo.GetAccessControl();

                var everyoneRule = new FileSystemAccessRule(
                    "Everyone",
                    FileSystemRights.Modify,
                    InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                    PropagationFlags.None,
                    AccessControlType.Allow);

                var authenticatedUserRule = new FileSystemAccessRule(
                    "Authenticated Users",
                    FileSystemRights.Modify,
                    InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                    PropagationFlags.None,
                    AccessControlType.Allow);

                dirSecurity.AddAccessRule(everyoneRule);
                dirSecurity.AddAccessRule(authenticatedUserRule);

                dirInfo.SetAccessControl(dirSecurity);
            }
            catch (PlatformNotSupportedException)
            {
                // Windows dışı sistemlerde bu kısmı atla
            }
#endif
                }
                catch (Exception ex)
                {
                    throw new DirectoryNotFoundException(
                        $"Failed to create or set permissions for directory: {directoryPath}", ex);
                }
            }
        }
        private static void LogCreationError(Exception ex, string fallbackPath)
        {
            try
            {
                string errorMessage =
                    $"=== Log Creation Error: {DateTime.Now} ===\n" +
                    $"Error Message: {ex.Message}\n" +
                    $"Stack Trace: {ex.StackTrace}\n" +
                    $"Fallback Path: {fallbackPath}\n" +
                    "=====================================\n\n";

                File.AppendAllText(fallbackPath, errorMessage);
            }
            catch
            {
                // En kötü durumda bile hata fırlatma
            }
        }


        //public static string CreateLogFilePath(IConfiguration config, string patern = "logs")
        //{
        //    var logConfig = config.Get<LogingConfigurationModel>() ??
        //                throw new Exception("Null Options Message");

        //    // Ana log klasörü yolu
        //    string baseLogDirectory = Path.Combine($"{logConfig.Serilog.File.Path}{"\\"}{logConfig.ProjectName}{"\\"}{patern}");

        //    // Tarih bazlı klasör yolu
        //    string dateBasedDirectory = Path.Combine(baseLogDirectory, patern);

        //    try
        //    {
        //        // Klasörleri oluştur (yoksa)
        //        if (!Directory.Exists(dateBasedDirectory))
        //        {
        //            Directory.CreateDirectory(dateBasedDirectory);
        //        }

        //        // Log dosyası yolu
        //        string logFilePath = Path.Combine(dateBasedDirectory, "logs.txt");
        //        return logFilePath;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Hata durumunda alternatif log yolu
        //        string fallbackPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "loglarkenHata", patern, "loglarkenHataAll.txt");
        //        return fallbackPath;
        //    }
        //}
    }

}
class ThreadIdEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        =>
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                "ThreadId", Thread.CurrentThread.ManagedThreadId));
}

