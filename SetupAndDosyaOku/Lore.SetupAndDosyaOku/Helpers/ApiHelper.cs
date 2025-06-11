using Lore.SetupAndDosyaOku.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Lore.SetupAndDosyaOku.Helpers
{
    public class ApiHelper
    {
        private readonly HttpClient _httpClient;
        private readonly ConfigHelper _configHelper;
        private readonly Logger _logger;
        private string _baseUrl;

        public ApiHelper(ConfigHelper configHelper, Logger logger)
        {
            _httpClient = new HttpClient();
            _configHelper = configHelper;
            _logger = logger;
            
            // API ayarlarını almak için GetConfig kullan
            var config = _configHelper.GetConfig();
            _baseUrl = config.ApiSettings.BaseUrl.TrimEnd('/');
            
            // Bağlantı zaman aşımını 30 saniye olarak ayarla
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }
        
        /// <summary>
        /// API bağlantısını kontrol eder - getFirmaDataOkuSetupBilgi endpoint'ini kullanarak
        /// </summary>
        /// <returns>API'ye bağlanılabiliyorsa true, aksi halde false</returns>
        public async Task<bool> CheckApiConnectivityAsync()
        {
            try
            {
                var config = _configHelper.GetConfig();
                string endpoint = $"{_baseUrl}/{config.ApiSettings.Endpoints.GetFirmaSetupBilgi}?firmaKodu=TEST";
                
                _logger.Debug($"API bağlantısı kontrol ediliyor: {endpoint}");
                
                // Önce basit bir GET isteği göndererek endpoint'in erişilebilir olup olmadığını kontrol et
                using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                var response = await _httpClient.GetAsync(endpoint, cancellationTokenSource.Token);
                
                // 200 OK veya 400 Bad Request (geçersiz firma kodu) da kabul edilebilir bağlantı kanıtı
                if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    _logger.Debug("API bağlantısı başarılı");
                    return true;
                }
                else
                {
                    _logger.Warning($"API bağlantısı başarısız: {response.StatusCode}");
                    return false;
                }
            }
            catch (OperationCanceledException)
            {
                _logger.Warning("API bağlantısı zaman aşımına uğradı");
                return false;
            }
            catch (HttpRequestException ex)
            {
                _logger.Warning($"API'ye bağlanılamadı: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error($"API bağlantısı kontrol edilirken hata oluştu: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Log verilerini API'ye gönderir - SendLogData endpoint'ini kullanır
        /// </summary>
        public async Task<bool> SendFileContentAsync(string fileType, string content)
        {
            try
            {
                if (string.IsNullOrEmpty(content))
                {
                    _logger.Warning($"API'ye gönderilecek içerik boş: {fileType}");
                    return false;
                }
                
                var config = _configHelper.GetConfig();
                string endpoint = $"{_baseUrl}/{config.ApiSettings.Endpoints.SendLogData}";
                
                var payload = new
                {
                    FirmaKod = _configHelper.GetSettings().FirmaKod,
                    LogType = fileType,
                    LogData = content
                };
                
                _logger.Info($"API isteği gönderiliyor: {endpoint}");
                
                // Retry mekanizması
                int maxRetries = 3;
                int currentRetry = 0;
                
                while (currentRetry < maxRetries)
                {
                    try
                    {
                        var jsonContent = JsonContent.Create(payload);
                        var response = await _httpClient.PostAsync(endpoint, jsonContent);
                        
                        if (response.IsSuccessStatusCode)
                        {
                            string responseContent = await response.Content.ReadAsStringAsync();
                            _logger.Info($"API yanıtı başarılı: {response.StatusCode} - {responseContent}");
                            return true;
                        }
                        else
                        {
                            string errorContent = await response.Content.ReadAsStringAsync();
                            _logger.Warning($"API yanıtı başarısız: {response.StatusCode} - {errorContent}");
                            
                            // 500 hataları için yeniden dene
                            if ((int)response.StatusCode >= 500)
                            {
                                currentRetry++;
                                if (currentRetry < maxRetries)
                                {
                                    int waitTime = 1000 * currentRetry;
                                    _logger.Info($"API isteği yeniden denenecek ({currentRetry}/{maxRetries}), {waitTime}ms bekleniyor...");
                                    await Task.Delay(waitTime);
                                    continue;
                                }
                            }
                            
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"API isteği sırasında hata (Deneme {currentRetry+1}/{maxRetries})", ex);
                        
                        currentRetry++;
                        if (currentRetry < maxRetries)
                        {
                            int waitTime = 1000 * currentRetry;
                            await Task.Delay(waitTime);
                            continue;
                        }
                        
                        return false;
                    }
                }
                
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error($"API isteği hazırlanırken hata oluştu: {fileType}", ex);
                return false;
            }
        }
        
        /// <summary>
        /// Firma koduna göre setup bilgilerini API'den alır
        /// </summary>
        /// <param name="firmaKodu">Firma kodu</param>
        /// <returns>Firma setup bilgileri</returns>
        public async Task<FirmaDataOkuSetupBilgiDto?> GetFirmaDataOkuSetupBilgiAsync(string firmaKodu)
        {
            try
            {
                var config = _configHelper.GetConfig();
                string endpoint = $"{_baseUrl}/{config.ApiSettings.Endpoints.GetFirmaSetupBilgi}?firmaKodu={firmaKodu}";
                
                _logger.Debug($"Firma setup bilgileri isteniyor: {endpoint}");
                
                using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                var response = await _httpClient.GetAsync(endpoint, cancellationTokenSource.Token);
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var serviceResponse = JsonSerializer.Deserialize<ServiceResponse<FirmaDataOkuSetupBilgiDto>>(jsonContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    
                    if (serviceResponse?.data != null)
                    {
                        _logger.Info($"Firma setup bilgileri başarıyla alındı: {firmaKodu}");
                        return serviceResponse.data;
                    }
                    else
                    {
                        _logger.Warning($"Firma setup bilgileri alınamadı: {serviceResponse?.message}");
                        return null;
                    }
                }
                else
                {
                    _logger.Warning($"Firma setup bilgileri API çağrısı başarısız: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Firma setup bilgileri alınırken hata oluştu: {firmaKodu}", ex);
                return null;
            }
        }
    }
}
