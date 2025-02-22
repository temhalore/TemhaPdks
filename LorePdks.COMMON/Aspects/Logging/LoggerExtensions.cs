using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LorePdks.COMMON.Aspects.Logging
{
    public static class LoggerExtensions
    {
        public static void LogHttpResponse(this ILogger logger, HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                logger.LogDebug("Received a success response from {Url}", response.RequestMessage.RequestUri);
            }
            else
            {
                logger.LogWarning("{Url} {StatusCode} ",
                    response.RequestMessage.RequestUri, (int)response.StatusCode);
            }
        }
        public static void LogWithCorrelationId(this ILogger logger, string correlationId, string message, LogLevel logLevel = LogLevel.Information)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append("{CorrelationId}");

            stringBuilder.Append("{Message}");

            var fieldValueList = new List<string>();

            fieldValueList.Add(correlationId);


            fieldValueList.Add(message);


            logger.Log(logLevel, stringBuilder.ToString(), fieldValueList.ToArray());

        }
        public static void LogWithCorrelationId(this ILogger logger, string correlationId, Dictionary<string, string> Fields, LogLevel logLevel = LogLevel.Information)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append("{CorrelationId}");
            foreach (var item in Fields.Keys)
            {
                stringBuilder.Append(" {" + item + "}");
            }

            var fieldValueList = new List<string>();

            fieldValueList.Add(correlationId);

            fieldValueList.AddRange(Fields.Values.ToList());

            logger.Log(logLevel, stringBuilder.ToString(), fieldValueList.ToArray());
        }




    }
}
