using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LorePdks.COMMON.Aspects.Logging.Serilog
{
    public class LogingConfigurationModel
    {
        public string ProjectName { get; set; }
        public bool RequestResponseLoggingIsActive { get; set; }
        public ConfigurationModelSerilog Serilog { get; set; }
    }
    public class ConfigurationModelSerilog
    {
        public string ActiveSink { get; set; }
        public ConfigurationModelElastic Elasticsearch { get; set; }
        public ConfigurationModelFile File { get; set; }
        public ConfigurationModelDatabase MSSqlServer { get; set; }
    }

    public class ConfigurationModelElastic
    {
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class ConfigurationModelFile
    {
        public string Path { get; set; }
    }
    public class ConfigurationModelDatabase
    {
        public string ConnectionString { get; set; }
    }
}
