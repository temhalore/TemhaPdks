using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using static LorePdks.COMMON.Enums.AppEnums;

namespace LorePdks.COMMON.Aspects.Logging.Serilog
{
    /*
         var jsonBirModelimiz = new { Name = "Login2", Id = "2" };
         var degiskenimiz = "Login1";
         {@attr} mesajda json data tutmak için kullanılıyor bu datayı tutarken json içeriğini indexliyor
         {attr} mesaja index eklemek için kullanılıyor
         SetPage(name) fonksiyonu mesajı page adı alında indexlemek için kullanılıyor
         OYSLog.Instance.SetLogIndexTip(OysEnums.LOG_INDEX_TIP.LOGIN).Warn("Cumali {@jsonBirModelimiz} girdirği sayfa {degiskenimiz} oldu2", jsonBirModelimiz, degiskenimiz);

    */
    public sealed class AppLogService
    {
        public ILogger Logger { get; set; }

        private static AppLogService instance = null;
        public static AppLogService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AppLogService();
                }
                instance.Logger = Log.Logger;
                return instance;
            }
        }
        public void Warn(string message)
        {
            Write(LogEventLevel.Warning, message, new { });
        }
        public void Warn(string message, params object[] propertyValues)
        {
            Write(LogEventLevel.Warning, message, propertyValues);
        }

        public void Info(string message)
        {
            Write(LogEventLevel.Information, message, new { });
        }
        public void Info(string message, params object[] propertyValues)
        {
            Write(LogEventLevel.Information, message, propertyValues);
        }
        public void Debug(string message)
        {
            Write(LogEventLevel.Debug, message, new { });
        }
        public void Debug(string message, params object[] propertyValues)
        {
            Write(LogEventLevel.Debug, message, propertyValues);
        }
        public void Error(string message)
        {
            Write(LogEventLevel.Error, message, new { });
        }
        public void Error(string message, params object[] propertyValues)
        {
            Write(LogEventLevel.Error, message, propertyValues);
        }

        public AppLogService SetLogIndexTip(LOG_INDEX_TIP logIndexTipEnum)
        {
            Logger = Logger.ForContext("logIndexTipEnum", logIndexTipEnum.ToString());
            return this;
        }
        private void Write(LogEventLevel level, string message, params object[] propertyValues)
        {
            var stackTrace = new System.Diagnostics.StackTrace();
            int frameindex = 2;
            if (stackTrace.FrameCount < frameindex)
                frameindex = stackTrace.FrameCount;
            var methodInfo = stackTrace.GetFrame(frameindex).GetMethod();
            Logger = Logger.ForContext(methodInfo.DeclaringType);
            Logger.Write(level, message, propertyValues);
        }

    }
    public static class AppLogExtension
    {
        public static ILogger SetLogIndexTip(this ILogger log, LOG_INDEX_TIP logIndexTipEnum)
        {
            return log.ForContext("logIndexTipEnum", logIndexTipEnum.ToString());
        }
    }
    public class PropertyBagEnricher : ILogEventEnricher
    {
        private readonly Dictionary<string, Tuple<object, bool>> _properties;

        /// <summary>
        /// Creates a new <see cref="PropertyBagEnricher" /> instance.
        /// </summary>
        public PropertyBagEnricher()
        {
            _properties = new Dictionary<string, Tuple<object, bool>>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Enriches the <paramref name="logEvent" /> using the values from the property bag.
        /// </summary>
        /// <param name="logEvent">The log event to enrich.</param>
        /// <param name="propertyFactory">The factory used to create the property.</param>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            foreach (KeyValuePair<string, Tuple<object, bool>> prop in _properties)
            {
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(prop.Key, prop.Value.Item1, prop.Value.Item2));
            }
        }

        /// <summary>
        /// Add a property that will be added to all log events enriched by this enricher.
        /// </summary>
        /// <param name="key">The property key.</param>
        /// <param name="value">The property value.</param>
        /// <param name="destructureObject">
        /// Whether to destructure the value. See https://github.com/serilog/serilog/wiki/Structured-Data
        /// </param>
        /// <returns>The enricher instance, for chaining Add operations together.</returns>
        public PropertyBagEnricher Add(string key, object value, bool destructureObject = false)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));

            if (!_properties.ContainsKey(key)) _properties.Add(key, Tuple.Create(value, destructureObject));

            return this;
        }
    }
}
