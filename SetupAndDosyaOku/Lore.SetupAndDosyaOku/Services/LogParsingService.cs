using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Configuration;
using Lore.SetupAndDosyaOku.Models;
using Lore.SetupAndDosyaOku.Helpers;

namespace Lore.SetupAndDosyaOku.Services
{
    /// <summary>
    /// Log verilerini parse etmek için API'yi kullanır
    /// </summary>
    public class LogParsingService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly Logger _logger;

        public LogParsingService(HttpClient httpClient, IConfiguration configuration, Logger logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Ham log verisini parse eder
        /// </summary>
        /// <param name="rawLogData">Ham log verisi</param>
        /// <param name="configId">Log parser konfigürasyon ID'si</param>
        /// <returns>Parse edilmiş log verisi</returns>
        public async Task<Dictionary<string, object>?> ParseLogDataAsync(string rawLogData, int configId)
        {
            try
            {
                var baseUrl = _configuration["ApiSettings:BaseUrl"];
                var endpoint = "/api/LogParser/parseLogData";
                var url = $"{baseUrl}{endpoint}";

                var request = new
                {
                    rawLogData = rawLogData,
                    configId = configId
                };

                var jsonContent = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                _logger.Info($"Log parsing başlatıldı. ConfigId: {configId}, DataLength: {rawLogData.Length}");

                var response = await _httpClient.PostAsync(url, content);
                  if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<LogParsingApiResponse<Dictionary<string, object>>>(responseContent);
                    
                    if (result?.data != null)
                    {
                        _logger.Info($"Log başarıyla parse edildi. ConfigId: {configId}");
                        return result.data;
                    }
                }
                else
                {
                    _logger.Error($"Log parsing API hatası: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Log parsing sırasında hata oluştu: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Log konfigürasyonunu test eder
        /// </summary>
        /// <param name="sampleLogData">Örnek log verisi</param>
        /// <param name="logParserConfig">Log parser konfigürasyonu JSON</param>
        /// <returns>Test sonucu</returns>
        public async Task<LogParsingTestResult?> TestLogParsingAsync(string sampleLogData, string logParserConfig)
        {
            try
            {
                var baseUrl = _configuration["ApiSettings:BaseUrl"];
                var endpoint = "/api/LogParser/testLogParserConfig";
                var url = $"{baseUrl}{endpoint}";

                var request = new
                {
                    sampleLogData = sampleLogData,
                    config = logParserConfig
                };

                var jsonContent = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content);
                  if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<LogParsingApiResponse<Dictionary<string, object>>>(responseContent);
                    
                    if (result?.data != null)
                    {
                        return new LogParsingTestResult
                        {
                            Success = true,
                            Message = "Test başarılı",
                            ParsedData = result.data
                        };
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new LogParsingTestResult
                    {
                        Success = false,
                        Message = $"Test hatası: {response.StatusCode}",
                        ParsedData = null
                    };
                }
            }
            catch (Exception ex)
            {
                return new LogParsingTestResult
                {
                    Success = false,
                    Message = $"Test sırasında hata oluştu: {ex.Message}",
                    ParsedData = null
                };
            }

            return null;
        }
    }    /// <summary>
    /// Log parsing test sonucu
    /// </summary>
    public class LogParsingTestResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public Dictionary<string, object>? ParsedData { get; set; }
    }

    /// <summary>
    /// API response wrapper for log parsing
    /// </summary>
    public class LogParsingApiResponse<T>
    {
        public T? data { get; set; }
        public string message { get; set; } = "";
        public string messageType { get; set; } = "";
    }
}
