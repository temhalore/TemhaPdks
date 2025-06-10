using Lore.SetupAndDosyaOku.Models;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Lore.SetupAndDosyaOku.Helpers
{    public class ApiHelper
    {        private readonly HttpClient _httpClient;
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
        /// API bağlantısını kontrol eder
        /// </summary>
        /// <returns>API'ye bağlanılabiliyorsa true, aksi halde false</returns>
        public async Task<bool> CheckApiConnectivityAsync()
        {
            try
            {
                string endpoint = $"{_baseUrl}/health";
                
                _logger.Debug($"API bağlantısı kontrol ediliyor: {endpoint}");
                
                // Önce basit bir GET isteği göndererek endpoint'in erişilebilir olup olmadığını kontrol et
                using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                var response = await _httpClient.GetAsync(endpoint, cancellationTokenSource.Token);
                
                if (response.IsSuccessStatusCode)
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
        /// Dosya içeriğini API'ye gönderir
        /// </summary>
        public async Task<bool> SendFileContentAsync(string fileType, string content)
        {
            try
            {                if (string.IsNullOrEmpty(content))
                {
                    _logger.Warning($"API'ye gönderilecek içerik boş: {fileType}");
                    return false;
                }
                
                string endpoint = $"{_baseUrl}/SaveData";                var payload = new
                {
                    FirmaKod = _configHelper.GetSettings().FirmaKod,
                    FileType = fileType,
                    Content = content,
                    Timestamp = DateTime.Now
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
        /// Güncel sürümü kontrol eder
        /// </summary>
        /// <param name="currentVersion">Mevcut uygulama sürümü (örn: "1.0.0.0")</param>
        /// <returns>Eğer güncelleme varsa indirme URL'si, yoksa null</returns>
        public async Task<string?> CheckForUpdatesAsync(string currentVersion)
        {
            try
            {                string endpoint = $"{_baseUrl}/CheckForUpdates?currentVersion={currentVersion}&firmaKod={_configHelper.GetSettings().FirmaKod}";
                
                _logger.Info($"Güncellemeler kontrol ediliyor: {endpoint}");
                
                var response = await _httpClient.GetAsync(endpoint);
                
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var updateInfo = JsonSerializer.Deserialize<UpdateInfo>(responseContent);
                      if (updateInfo != null && !string.IsNullOrEmpty(updateInfo.DownloadUrl) && updateInfo.NewVersion != currentVersion)
                    {
                        _logger.Info($"Yeni sürüm mevcut: {updateInfo.NewVersion} (Şu anki: {currentVersion})");
                        return updateInfo.DownloadUrl;
                    }
                    else
                    {
                        _logger.Info("Güncel sürüm kullanılıyor.");
                        return null;
                    }
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    _logger.Warning($"Güncelleme kontrolü başarısız: {response.StatusCode} - {errorContent}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Güncelleme kontrolü sırasında hata oluştu", ex);
                return null;
            }
        }
          /// <summary>
        /// Güncelleme dosyasını indirir
        /// </summary>
        /// <param name="downloadUrl">İndirilecek güncelleme dosyası URL'si</param>
        /// <param name="savePath">İndirilen dosyanın kaydedileceği yerel yol</param>
        /// <returns>İndirme başarılı ise true, değilse false</returns>
        public async Task<bool> DownloadUpdateAsync(string downloadUrl, string savePath)
        {
            try
            {                _logger.Info($"Güncelleme indiriliyor: {downloadUrl}");
                
                using (var response = await _httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string directory = Path.GetDirectoryName(savePath) ?? string.Empty;
                        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }
                        
                        using (var fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None))
                        using (var downloadStream = await response.Content.ReadAsStreamAsync())
                        {
                            // İlerleme göstergesi ile indir
                            byte[] buffer = new byte[8192];
                            long totalBytes = response.Content.Headers.ContentLength ?? -1;
                            long bytesRead = 0;
                            int read;
                            
                            while ((read = await downloadStream.ReadAsync(buffer)) > 0)
                            {
                                await fileStream.WriteAsync(buffer.AsMemory(0, read));
                                bytesRead += read;
                                
                                if (totalBytes > 0)
                                {
                                    int progressPercentage = (int)((bytesRead * 100) / totalBytes);
                                    if (progressPercentage % 10 == 0) // Her %10'luk artışta log
                                    {
                                        _logger.Info($"İndirme ilerlemesi: %{progressPercentage}");
                                    }
                                }
                            }
                        }
                        
                        _logger.Info($"Güncelleme başarıyla indirildi: {savePath}");
                        return true;
                    }
                    else
                    {
                        _logger.Error($"Güncelleme indirme başarısız: {response.StatusCode}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Güncelleme indirilirken hata oluştu", ex);
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
                string endpoint = $"{_baseUrl}/Api/DataOkuConsoleSetup/getFirmaDataOkuSetupBilgi?firmaKodu={firmaKodu}";
                
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
    
    public class UpdateInfo
    {
        public string? NewVersion { get; set; }
        public string? DownloadUrl { get; set; }
        public string? ReleaseNotes { get; set; }
    }
}
