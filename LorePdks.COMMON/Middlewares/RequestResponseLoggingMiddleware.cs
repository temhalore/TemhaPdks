
using LorePdks.COMMON.Configuration;
using LorePdks.COMMON.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LorePdks.COMMON.Middlewares
{
    /// <summary>
    /// tüm responce ve requestler loglanır
    /// </summary>
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Serilog.ILogger _logger;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
        private readonly IConfiguration _configuration;

        public RequestResponseLoggingMiddleware(RequestDelegate next,  IConfiguration configuration)
        {
            _next = next;
            
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
            _configuration = configuration;
        }


        public async Task Invoke(HttpContext context)
        {
            var correlationId = context.Request.Headers["X-Correlation-ID"].ToString();
            var appToken = string.Empty;
            var isActive = bool.Parse(_configuration["LogConfig:RequestResponseLoggingIsActive"]);

            if (isActive)
            {
                var requestStartDate = DateTime.Now;
                appToken = await LogRequest(context, correlationId);
                await LogResponse(context, correlationId, appToken, requestStartDate);
            }
            else
            {
                await _next(context);
            }
        }

        private async Task<string> LogRequest(HttpContext context, string correlationId)
        {
            try
            {
                //string aa = context.Request.Path.ToString();

                //if (aa.Contains("save3dOdeme"))
                //{
                //    return "bankaçağrısı-save3dOdeme";

                //}



                context.Request.EnableBuffering();

                await using var requestStream = _recyclableMemoryStreamManager.GetStream();
                await context.Request.Body.CopyToAsync(requestStream);
                var requestBody = ReadStreamInChunks(requestStream);

                if (!string.IsNullOrEmpty(requestBody))
                {
                    dynamic requestJsonBody = JsonConvert.DeserializeObject<dynamic>(requestBody);

                    string hasPassword = requestJsonBody?.password;

                    if (!string.IsNullOrEmpty(hasPassword))
                    {
                        requestBody = requestBody.Replace(hasPassword, hasPassword.Substring(0, 4) + "********");

                    }
                }

                string log = $"Http Request Information:{Environment.NewLine}" +
                                       $"CorrelationId:{correlationId} " +
                                       $"Schema:{context.Request.Scheme} " +
                                       $"Host: {context.Request.Host} " +
                                       $"Path: {context.Request.Path} " +
                                       $"QueryString: {context.Request.QueryString} " +
                                       $"Request Body: {requestBody} ";

                string appToken = context.Request.Headers[CoreConfig.TokenKeyName].ToString();

                if (!string.IsNullOrEmpty(appToken))
                {
                    log += $"appToken: {appToken} ";
                }

                _logger.Information(log);
                context.Request.Body.Position = 0;
                return appToken;
            }
            catch (Exception e)
            {

                //TODO:burayı elden geçir ae
                return "aaa";
            }
        }

        private async Task LogResponse(HttpContext context, string correlationId, string appToken, DateTime requestStartDate)
        {
            var originalBodyStream = context.Response.Body;

            await using var responseBody = _recyclableMemoryStreamManager.GetStream();
            context.Response.Body = responseBody;

            await _next(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            string log = $"Http Response Information:{Environment.NewLine}" +
                                   $"CorrelationId:{correlationId} " +
                                   $"Schema:{context.Request.Scheme} " +
                                   $"Host: {context.Request.Host} " +
                                   $"Path: {context.Request.Path} " +
                                   $"QueryString: {context.Request.QueryString} ";
            if (!text.Contains("swagger"))
            {
                log += $"Response Body: {text} ";
            }

            if (!string.IsNullOrEmpty(appToken))
            {
                log += $"appToken: {appToken} ";
            }
            log += $"ReqResElapsedTime: {(DateTime.Now - requestStartDate).TotalMilliseconds.ToString()}";

            _logger.Information(log);

            await responseBody.CopyToAsync(originalBodyStream);
        }

        private async Task<string> LogRequest1(HttpContext context, string correlationId)
        {

            var userId = string.Empty;

            using (var requestStream = _recyclableMemoryStreamManager.GetStream())
            {
                await context.Request.Body.CopyToAsync(requestStream);

                var stringBuilder = new StringBuilder();

                stringBuilder.Append("{CorrelationId}");
                stringBuilder.Append("{Schema}");
                stringBuilder.Append("{Host}");
                stringBuilder.Append("{Path}");
                stringBuilder.Append("{QueryString}");

                var fieldValueList = new List<string>();
                fieldValueList.Add(correlationId);
                fieldValueList.Add(context.Request.Scheme);
                fieldValueList.Add(context.Request.Host.ToString());
                fieldValueList.Add(context.Request.Path);
                fieldValueList.Add(context.Request.QueryString.ToString());


                var requestBody = ReadStreamInChunks(requestStream);

                if (!string.IsNullOrEmpty(requestBody))
                {
                    dynamic requestJsonBody = JsonConvert.DeserializeObject<dynamic>(requestBody);

                    string hasPassword = requestJsonBody.password;

                    if (!string.IsNullOrEmpty(hasPassword))
                    {
                        requestBody = requestBody.Replace(hasPassword, hasPassword.Substring(0, 4) + "********");

                    }
                }
                stringBuilder.Append("{RequestBody}");
                fieldValueList.Add(requestBody);

                var userIdClaims = context.User.FindFirst(x => x.Type == "UserId");
                if (userIdClaims != null)
                {
                    userId = userIdClaims.Value;
                    stringBuilder.Append("{UserId}");
                    fieldValueList.Add(userIdClaims.Value);
                }
                _logger.Information(stringBuilder.ToString(), fieldValueList.ToArray());

                context.Request.Body.Position = 0;

                return userId;
            }
        }
        private async Task LogResponse1(HttpContext context, string correlationId, string userId, DateTime requestStartDate)
        {
            var originalBodyStream = context.Response.Body;

            using (var responseBody = _recyclableMemoryStreamManager.GetStream())
            {
                context.Response.Body = responseBody;
                await _next(context);

                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var text = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                var stringBuilder = new StringBuilder();

                stringBuilder.Append("{CorrelationId}");

                stringBuilder.Append("{Schema}");
                stringBuilder.Append("{Host}");
                stringBuilder.Append("{Path}");
                stringBuilder.Append("{QueryString}");

                var fieldValueList = new List<string>();
                fieldValueList.Add(correlationId);

                fieldValueList.Add(context.Request.Scheme);
                fieldValueList.Add(context.Request.Host.ToString());
                fieldValueList.Add(context.Request.Path);
                fieldValueList.Add(context.Request.QueryString.ToString());

                if (!text.Contains("swagger"))
                {
                    stringBuilder.Append("{ResponseBody}");
                    fieldValueList.Add(text);
                }

                if (!string.IsNullOrEmpty(userId))
                {
                    stringBuilder.Append("{UserId}");
                    fieldValueList.Add(userId);
                }

                stringBuilder.Append("{ReqResElapsedTime}");
                fieldValueList.Add((DateTime.Now - requestStartDate).TotalMilliseconds.ToString());
                _logger.Information(stringBuilder.ToString(), fieldValueList.ToArray());

                await responseBody.CopyToAsync(originalBodyStream);
            }
          ;
        }

        private static string ReadStreamInChunks(Stream stream)
        {
            using (var textWriter = new StringWriter())
            {
                using (var reader = new StreamReader(stream))
                {
                    const int readChunkBufferLength = 4096;
                    stream.Seek(0, SeekOrigin.Begin);
                    var readChunk = new char[readChunkBufferLength];
                    int readChunkLength;
                    do
                    {
                        readChunkLength = reader.ReadBlock(readChunk,
                                                           0,
                                                           readChunkBufferLength);
                        textWriter.Write(readChunk, 0, readChunkLength);
                    } while (readChunkLength > 0);
                    return textWriter.ToString();
                }
            }
        }

    }
}
