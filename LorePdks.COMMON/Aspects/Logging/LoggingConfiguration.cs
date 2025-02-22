using LorePdks.COMMON.Aspects.Logging.Serilog;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.MSSqlServer;

namespace LorePdks.COMMON.Aspects.Logging
{
    public static class LoggingConfiguration
    {
        public static LoggerConfiguration Configuration(IConfiguration config, string patern = "logs", string sink = "")
        {
            var logConfig = config.GetSection("LogConfig").Get<LogingConfigurationModel>() ??
                        throw new Exception("Null Options Message");

            var seriLogConfig = new LoggerConfiguration()
               .Enrich.FromLogContext()
               .Enrich.With(new ThreadIdEnricher())
               .Enrich.WithProperty("Application", "SANALPOS")
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
                //var filePath = logConfig.Serilog.File.Path;
                // seriLogConfig.WriteTo.File(filePath, rollingInterval: RollingInterval.Day);
                var logFilePath = string.Format("{0}{1}", Path.Combine(logConfig.Serilog.File.Path, patern + "/"), ".txt");
                seriLogConfig.WriteTo.File(logFilePath,
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: null,
                        fileSizeLimitBytes: 5000000,
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

    }
    class ThreadIdEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
            =>
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                    "ThreadId", Thread.CurrentThread.ManagedThreadId));
    }
}
