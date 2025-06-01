using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Lore.SetupAndDosyaOku.Models;
using Lore.SetupAndDosyaOku.Helpers;

namespace Lore.SetupAndDosyaOku.Services
{
    public class UpdateService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly Logger _logger;

        public UpdateService(HttpClient httpClient, IConfiguration configuration, Logger logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<VersionCheckResult?> CheckForUpdatesAsync(string firmaKod, string currentVersion)
        {
            try
            {
                var baseUrl = _configuration["ApiSettings:BaseUrl"];
                var endpoint = _configuration["ApiSettings:Endpoints:CheckVersion"];
                var url = $"{baseUrl}{endpoint}?firmaKod={firmaKod}&currentVersion={currentVersion}";

                _logger.Info($"Güncelleme kontrolü başlatıldı: {url}");

                var response = await _httpClient.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<VersionCheckResult>>(jsonContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    _logger.Info($"Güncelleme kontrolü tamamlandı. UpdateAvailable: {result?.Data?.UpdateAvailable}");
                    return result?.Data;
                }
                else
                {
                    _logger.Error($"Güncelleme kontrolü başarısız. StatusCode: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Güncelleme kontrolü sırasında hata: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DownloadUpdateAsync(string firmaKod, string downloadPath)
        {
            try
            {
                var baseUrl = _configuration["ApiSettings:BaseUrl"];
                var endpoint = _configuration["ApiSettings:Endpoints:DownloadUpdate"];
                var url = $"{baseUrl}{endpoint}?firmaKod={firmaKod}";

                _logger.Info($"Güncelleme indirme başlatıldı: {url}");

                var response = await _httpClient.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    var fileBytes = await response.Content.ReadAsByteArrayAsync();
                    await System.IO.File.WriteAllBytesAsync(downloadPath, fileBytes);
                    
                    _logger.Info($"Güncelleme başarıyla indirildi: {downloadPath}");
                    return true;
                }
                else
                {
                    _logger.Error($"Güncelleme indirme başarısız. StatusCode: {response.StatusCode}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Güncelleme indirme sırasında hata: {ex.Message}");
                return false;
            }
        }
    }

    public class VersionCheckResult
    {
        public string CurrentVersion { get; set; } = string.Empty;
        public string LatestVersion { get; set; } = string.Empty;
        public bool UpdateAvailable { get; set; }
        public string? DownloadUrl { get; set; }
        public string? ReleaseNotes { get; set; }
    }

    public class ApiResponse<T>
    {
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public int MessageType { get; set; }
    }
}
