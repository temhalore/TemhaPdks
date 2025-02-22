using Serilog;

namespace LorePdks.COMMON.Aspects.Logging.Serilog.Logger
{
    public class FileLogger : LoggerServiceBase
    {
        public static ILogger _Logger;
        public FileLogger()
        {
            Logger = _Logger;
            //if (_Logger == null)
            //{
            //    var configuration = ServiceTool.ServiceProvider.GetService<IConfiguration>();

            //var logConfig = configuration.GetSection("LogConfig").Get<LogingConfigurationModel>() ?? throw new Exception("Null Options Message");

            //var logFilePath = string.Format("{0}{1}", logConfig.Serilog.File.Path, ".txt");

            //    _Logger = new LoggerConfiguration()
            //        .WriteTo.File(logFilePath,
            //            rollingInterval: RollingInterval.Day,
            //            retainedFileCountLimit: null,
            //            fileSizeLimitBytes: 5000000,
            //            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}")
            //        .CreateLogger();
            //}
        }

    }
}
