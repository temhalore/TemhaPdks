using LorePdks.COMMON.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;

namespace LorePdks.COMMON.Aspects.Logging.Serilog
{
    public class ElasticsearchLogger : LoggerServiceBase
    {
        public ElasticsearchLogger()
        {
            var configuration = ServiceProviderHelper.ServiceProvider.GetService<IConfiguration>();
            var logConfig = configuration.GetSection("LogConfig").Get<LogingConfigurationModel>() ??
                            throw new Exception("Null Options Message");

            var sinkOpts = new ElasticsearchSinkOptions(new Uri(logConfig.Serilog.Elasticsearch.Host))
            {
                CustomFormatter = new ExceptionAsObjectJsonFormatter(renderMessage: true),
                AutoRegisterTemplate = true,
                TemplateName = "serilog-events-template",
                IndexFormat = logConfig.ProjectName + "-log-{0:yyyy.MM}",
                ModifyConnectionSettings = x => x.BasicAuthentication(logConfig.Serilog.Elasticsearch.Username, logConfig.Serilog.Elasticsearch.Password),
            };


            var seriLogConfig = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", logConfig.ProjectName)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)

                .WriteTo.Elasticsearch(sinkOpts)
                .MinimumLevel.Verbose()
                .CreateLogger();

            Logger = seriLogConfig;
        }
    }
}
