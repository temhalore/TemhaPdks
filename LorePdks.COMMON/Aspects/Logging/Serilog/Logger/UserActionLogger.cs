using LorePdks.COMMON.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;

namespace LorePdks.COMMON.Aspects.Logging.Serilog.Logger
{
    public class UserActionLogger : LoggerServiceBase
    {

        public UserActionLogger()
        {
            var configuration = ServiceProviderHelper.ServiceProvider.GetService<IConfiguration>();
            var logConfig = configuration.GetSection("SeriLogConfigurations").Get<LogingConfigurationModel>() ??
                            throw new Exception("Null Options Message");

            var sinkOpts = new ElasticsearchSinkOptions(new Uri(logConfig.Serilog.Elasticsearch.Host))
            {
                CustomFormatter = new ExceptionAsObjectJsonFormatter(renderMessage: true),
                AutoRegisterTemplate = true,
                TemplateName = "serilog-events-template",
                IndexFormat = "SANALPOS-UserAction-{0:MM-yyyy}",
                ModifyConnectionSettings = x => x.BasicAuthentication(logConfig.Serilog.Elasticsearch.Username, logConfig.Serilog.Elasticsearch.Password),
            };

            var seriLogConfig = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "SANALPOS")
                .Enrich.With(new ThreadIdEnricher())
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .WriteTo.Elasticsearch(sinkOpts)
                .MinimumLevel.Verbose()
                .CreateLogger();

            Logger = seriLogConfig;

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


