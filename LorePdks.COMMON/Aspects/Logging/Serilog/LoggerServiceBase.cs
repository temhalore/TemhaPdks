using Serilog;
using System;

namespace LorePdks.COMMON.Aspects.Logging.Serilog
{
    public abstract class LoggerServiceBase
    {
        public ILogger Logger;
        public void Verbose(string message) => Logger.Verbose(message);
        public void Fatal(string message) => Logger.Fatal(message);
        public void Info(string message) => Logger.Information(message);
        public void Info(string message, params object[] propertyValues) => Logger.Information(message, propertyValues);
        public void Warn(string message) => Logger.Warning(message);
        public void Debug(string message) => Logger.Debug(message);
        public void Error(Exception ex, string message) => Logger.Error(ex, message);
        public void Error(string message) => Logger.Error(message);
    }
}
