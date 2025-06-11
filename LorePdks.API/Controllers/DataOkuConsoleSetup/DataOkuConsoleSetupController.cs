using Microsoft.AspNetCore.Mvc;
using LorePdks.COMMON.Models.ServiceResponse;
using LorePdks.BAL.Managers.Firma.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace LorePdks.API.Controllers.DataOkuConsoleSetup
{
    /// <summary>
    /// DataOkuConsole uygulamasının setup ve versiyon bilgilerini yöneten kontrolcü
    /// </summary>
    [Route("Api/DataOkuConsoleSetup")]
    [ApiController]
    public class DataOkuConsoleSetupController : ControllerBase
    {
        private readonly ILogger<DataOkuConsoleSetupController> _logger;
        private readonly IFirmaManager _firmaManager;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// DataOkuConsoleSetupController constructor
        /// </summary>
        public DataOkuConsoleSetupController(
            ILogger<DataOkuConsoleSetupController> logger,
            IFirmaManager firmaManager,
            IConfiguration configuration)
        {
            _logger = logger;
            _firmaManager = firmaManager;
            _configuration = configuration;
        }

        /// <summary>
        /// Firma koduna göre DataOkuConsole versiyon bilgisini getirir
        /// </summary>
        [HttpGet]
        [Route("getFirmaDataOkuSetupBilgi")]
        public IActionResult getFirmaDataOkuSetupBilgi([FromQuery] string firmaKodu)
        {
            try
            {
                _logger.LogInformation($"getFirmaDataOkuSetupBilgi çağrıldı. FirmaKodu: {firmaKodu}");

                var response = new ServiceResponse<FirmaDataOkuSetupBilgiDto>();

                // Firma koduna göre firmayı bul
                var firmaList = _firmaManager.getFirmaDtoListById(false);
                var firma = firmaList.FirstOrDefault(x => x.kod == firmaKodu);

                if (firma == null)
                {

                    response.messageType = ServiceResponseMessageType.Error;
                    response.message = $"Firma bulunamadı: {firmaKodu}";
                    return Ok(response);
                }

                var firmaBilgiDto = new FirmaDataOkuSetupBilgiDto
                {
                    FirmaKod = firma.kod,
                    isPdks = firma.isPdks,
                    isAlarm = firma.isAlarm,
                    isKamera = firma.isKamera
                   
                };
                response.data = firmaBilgiDto;


                _logger.LogInformation($"getFirmaDataOkuSetupBilgi çağrıldı ve döüş sağlandı. FirmaKodu: {firmaKodu}");

                return Ok(response);            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"getFirmaDataOkuSetupBilgi hatası. FirmaKodu: {firmaKodu}");

                var response = new ServiceResponse<FirmaDataOkuSetupBilgiDto>
                {
                    messageType = ServiceResponseMessageType.Error,
                    message = $"İşlem sırasında hata: {ex.Message}"
                };

                return Ok(response);
            }
        }/// <summary>
        /// Log verilerini alır ve dosya sistemine kaydeder (PDKS, Alarm, KameraLog)
        /// </summary>
        [HttpPost]
        [Route("SendLogData")]
        public async Task<IActionResult> SendLogData([FromBody] LogDataRequest request)
        {
            try
            {
                _logger.LogInformation($"SendLogData çağrıldı. FirmaKod: {request.FirmaKod}, LogType: {request.LogType}");

                if (string.IsNullOrEmpty(request.FirmaKod) || string.IsNullOrEmpty(request.LogType) || string.IsNullOrEmpty(request.LogData))
                {
                    return BadRequest("FirmaKod, LogType ve LogData alanları gereklidir.");
                }

                // Gelen request klasörünü oluştur
                var baseFolder = Path.Combine(Directory.GetCurrentDirectory(), "GelenRequestler");
                var firmaFolder = Path.Combine(baseFolder, request.FirmaKod);
                var logTypeFolder = Path.Combine(firmaFolder, request.LogType.ToLower());

                // Klasörleri oluştur
                Directory.CreateDirectory(logTypeFolder);

                // Dosya adını oluştur (log tipine göre)
                string fileName = request.LogType.ToLower() switch
                {
                    "pdks" => "pdks_logs.txt",
                    "alarm" => "alarm_logs.txt",
                    "kameralog" => "kamera_logs.txt",
                    _ => "unknown_logs.txt"
                };

                var filePath = Path.Combine(logTypeFolder, fileName);

                // Log verisini işle - her satırın başına tarih saat ekle
                var lines = request.LogData.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                var processedLines = lines.Select(line =>
                    $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {line.Trim()}");

                // Dosyaya append et
                await System.IO.File.AppendAllLinesAsync(filePath, processedLines, Encoding.UTF8);

                _logger.LogInformation($"Log verisi başarıyla kaydedildi. Dosya: {filePath}, Satır sayısı: {lines.Length}");

                var response = new ServiceResponse<object>
                {
                    data = new { Success = true, Message = "Log verisi başarıyla kaydedildi.", ProcessedLines = lines.Length },
                    messageType = ServiceResponseMessageType.Success,
                    message = "İşlem başarılı"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"SendLogData hatası. FirmaKod: {request?.FirmaKod}");

                var response = new ServiceResponse<object>
                {
                    data = new { Success = false, Message = $"Hata: {ex.Message}" },
                    messageType = ServiceResponseMessageType.Error,
                    message = $"İşlem sırasında hata: {ex.Message}"
                };

                return Ok(response);
            }
        }

        /// <summary>
        /// Uygulama versiyon kontrolü yapar
        /// </summary>
        [HttpGet]
        [Route("CheckVersion")]
        public IActionResult CheckVersion([FromQuery] string firmaKod, [FromQuery] string currentVersion)
        {
            try
            {
                _logger.LogInformation($"CheckVersion çağrıldı. FirmaKod: {firmaKod}, CurrentVersion: {currentVersion}");

                if (string.IsNullOrEmpty(firmaKod) || string.IsNullOrEmpty(currentVersion))
                {
                    return BadRequest("FirmaKod ve CurrentVersion parametreleri gereklidir.");
                }

                // Güncel versiyonu al (config'den veya veritabanından)
                string latestVersion = _configuration["DataOkuConsole:CurrentVersion"] ?? "1.0.0";

                // Versiyon karşılaştırması
                bool updateAvailable = IsVersionNewer(latestVersion, currentVersion);

                var response = new ServiceResponse<VersionCheckResult>
                {
                    data = new VersionCheckResult
                    {
                        CurrentVersion = currentVersion,
                        LatestVersion = latestVersion,
                        UpdateAvailable = updateAvailable,
                        DownloadUrl = updateAvailable ? _configuration["DataOkuConsole:SetupUrl"] : null,
                        ReleaseNotes = updateAvailable ? _configuration["DataOkuConsole:ReleaseNotes"] : null
                    },
                    messageType = ServiceResponseMessageType.Success,
                    message = updateAvailable ? "Güncelleme mevcut" : "Güncel versiyon kullanılıyor"
                };

                _logger.LogInformation($"CheckVersion tamamlandı. UpdateAvailable: {updateAvailable}");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"CheckVersion hatası. FirmaKod: {firmaKod}");

                var response = new ServiceResponse<VersionCheckResult>
                {
                    messageType = ServiceResponseMessageType.Error,
                    message = $"Versiyon kontrolü sırasında hata: {ex.Message}"
                };

                return Ok(response);
            }
        }

        /// <summary>
        /// Güncelleme dosyasını indirir
        /// </summary>
        [HttpGet]
        [Route("DownloadUpdate")]
        public IActionResult DownloadUpdate([FromQuery] string firmaKod)
        {
            try
            {
                _logger.LogInformation($"DownloadUpdate çağrıldı. FirmaKod: {firmaKod}");

                if (string.IsNullOrEmpty(firmaKod))
                {
                    return BadRequest("FirmaKod parametresi gereklidir.");
                }

                // Setup dosyasının yolunu al
                var setupFileName = "LoreDosyaIzleyici_Setup.exe";
                var setupFilePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", setupFileName);

                if (!System.IO.File.Exists(setupFilePath))
                {
                    return NotFound("Güncelleme dosyası bulunamadı.");
                }

                // Dosyayı stream olarak dön
                var fileStream = new FileStream(setupFilePath, FileMode.Open, FileAccess.Read);

                _logger.LogInformation($"DownloadUpdate başarılı. FirmaKod: {firmaKod}, Dosya: {setupFileName}");

                return File(fileStream, "application/octet-stream", setupFileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"DownloadUpdate hatası. FirmaKod: {firmaKod}");
                return StatusCode(500, $"Dosya indirme sırasında hata: {ex.Message}");
            }
        }

        /// <summary>
        /// Versiyon karşılaştırması yapar
        /// </summary>
        private bool IsVersionNewer(string latestVersion, string currentVersion)
        {
            try
            {
                var latest = new Version(latestVersion);
                var current = new Version(currentVersion);

                return latest > current;
            }
            catch
            {
                // Versiyon formatı hatalıysa güncelleme gerekli sayalım
                return true;
            }
        }
    }

    // DTO sınıfları
    public class LogDataRequest
    {
        public string FirmaKod { get; set; } = string.Empty;
        public string LogType { get; set; } = string.Empty; // "pdks", "alarm", "kameralog"
        public string LogData { get; set; } = string.Empty;
    }

    public class FirmaDataOkuSetupBilgiDto
    {
        public string FirmaKod { get; set; } = string.Empty;
        public bool isPdks { get; set; }
        public bool isAlarm { get; set; }
        public bool isKamera { get; set; }
    }

    public class VersionCheckResult
    {
        public string CurrentVersion { get; set; } = string.Empty;
        public string LatestVersion { get; set; } = string.Empty;
        public bool UpdateAvailable { get; set; }
        public string? DownloadUrl { get; set; }
        public string? ReleaseNotes { get; set; }
    }
}
