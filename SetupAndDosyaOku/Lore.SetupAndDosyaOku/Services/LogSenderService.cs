using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Lore.SetupAndDosyaOku.Models;
using Lore.SetupAndDosyaOku.Helpers;

namespace Lore.SetupAndDosyaOku.Services
{
    public class LogSenderService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly Logger _logger;

        public LogSenderService(HttpClient httpClient, IConfiguration configuration, Logger logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendLogDataAsync(string firmaKod, string logType, string logData)
        {
            try
            {
                var baseUrl = _configuration["ApiSettings:BaseUrl"];
                var endpoint = _configuration["ApiSettings:Endpoints:SendLogData"];
                var url = $"{baseUrl}{endpoint}";

                var request = new LogDataRequest
                {
                    FirmaKod = firmaKod,
                    LogType = logType,
                    LogData = logData
                };

                var jsonContent = JsonSerializer.Serialize(request);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                _logger.Info($"Log gönderimi başlatıldı. FirmaKod: {firmaKod}, LogType: {logType}, DataLength: {logData.Length}");

                var response = await _httpClient.PostAsync(url, content);
                
                if (response.IsSuccessStatusCode)
                {
                    _logger.Info($"Log başarıyla gönderildi. FirmaKod: {firmaKod}, LogType: {logType}");
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.Error($"Log gönderimi başarısız. StatusCode: {response.StatusCode}, Content: {errorContent}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Log gönderimi sırasında hata: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendPdksLogAsync(string firmaKod, string logData)
        {
            return await SendLogDataAsync(firmaKod, "pdks", logData);
        }

        public async Task<bool> SendAlarmLogAsync(string firmaKod, string logData)
        {
            return await SendLogDataAsync(firmaKod, "alarm", logData);
        }

        public async Task<bool> SendKameraLogAsync(string firmaKod, string logData)
        {
            return await SendLogDataAsync(firmaKod, "kameralog", logData);
        }
    }

    public class LogDataRequest
    {
        public string FirmaKod { get; set; } = string.Empty;
        public string LogType { get; set; } = string.Empty;
        public string LogData { get; set; } = string.Empty;
    }
}
