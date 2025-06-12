using System.Collections.Generic;

namespace LorePdks.COMMON.DTO.LogParser
{
    /// <summary>
    /// Log parsing isteği için DTO
    /// </summary>
    public class LogParseRequestDTO
    {
        public string rawLogData { get; set; }
        public int configId { get; set; }
    }    /// <summary>
    /// Log parser test isteği için DTO
    /// </summary>
    public class LogParserTestRequestDTO
    {
        public string sampleLogData { get; set; }
        public string config { get; set; }
    }
}
