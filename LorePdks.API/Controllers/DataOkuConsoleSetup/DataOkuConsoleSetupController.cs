using Microsoft.AspNetCore.Mvc;
using LorePdks.COMMON.DTO.Base;
using LorePdks.COMMON.Models.ServiceResponse;
using LorePdks.COMMON.DTO.DataOkuConsole;
using LorePdks.BAL.Managers.Firma.Interfaces;
using Microsoft.Extensions.Configuration;

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
        [Route("getFirmaDataOkuVersiyon")]
        public IActionResult getFirmaDataOkuVersiyon([FromQuery] string firmaKodu)
        {
            try
            {
                _logger.LogInformation($"getFirmaDataOkuVersiyon çağrıldı. FirmaKodu: {firmaKodu}");
                
                var response = new ServiceResponse<DataOkuVersiyon>();
                
                // Firma koduna göre firmayı bul
                var firmaList = _firmaManager.getFirmaDtoListById(false);
                var firma = firmaList.FirstOrDefault(x => x.kod == firmaKodu);
                
                if (firma == null)
                {
                    response.data = new DataOkuVersiyon
                    {
                        Success = false,
                        Message = $"Firma bulunamadı: {firmaKodu}",
                        Version = ""
                    };
                    response.messageType = ServiceResponseMessageType.Error;
                    response.message = $"Firma bulunamadı: {firmaKodu}";
                    return Ok(response);
                }
                
                // Firma için DataOkuConsole versiyon bilgisini getir
                // TODO: Gerçek uygulamada firma veya genel versiyon bilgisi veritabanından alınabilir
                // Şimdilik config dosyasından okuyalım
                string version = _configuration["DataOkuConsole:CurrentVersion"] ?? "1.0.0";
                
                response.data = new DataOkuVersiyon
                {
                    Success = true,
                    Message = "Başarılı",
                    Version = version
                };
                
                _logger.LogInformation($"getFirmaDataOkuVersiyon başarılı. FirmaKodu: {firmaKodu}, Versiyon: {version}");
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"getFirmaDataOkuVersiyon hatası. FirmaKodu: {firmaKodu}");
                
                var response = new ServiceResponse<DataOkuVersiyon>
                {
                    data = new DataOkuVersiyon
                    {
                        Success = false,
                        Message = $"İşlem sırasında hata: {ex.Message}",
                        Version = ""
                    },
                    messageType = ServiceResponseMessageType.Error,
                    message = $"İşlem sırasında hata: {ex.Message}"
                };
                
                return Ok(response);
            }
        }

        /// <summary>
        /// Güncel DataOkuConsole setup versiyonu ve indirme bağlantısını getirir
        /// </summary>
        [HttpGet]
        [Route("getGuncelDataOkuSetupVersiyon")]
        public IActionResult getGuncelDataOkuSetupVersiyon([FromQuery] string firmaKodu)
        {
            try
            {
                _logger.LogInformation($"getGuncelDataOkuSetupVersiyon çağrıldı. FirmaKodu: {firmaKodu}");
                
                var response = new ServiceResponse<DataOkuSetupVersiyon>();
                
                // Firma koduna göre firmayı bul
                var firmaList = _firmaManager.getFirmaDtoListById(false);
                var firma = firmaList.FirstOrDefault(x => x.kod == firmaKodu);
                
                if (firma == null)
                {
                    response.messageType = ServiceResponseMessageType.Error;
                    response.message = $"Firma bulunamadı: {firmaKodu}";
                    return Ok(response);
                }
                
                // Güncel setup versiyonu ve indirme bağlantısını getir
                // TODO: Gerçek uygulamada firma veya genel versiyon bilgisi veritabanından alınabilir
                // Şimdilik config dosyasından okuyalım
                string version = _configuration["DataOkuConsole:CurrentVersion"] ?? "1.0.0";
                string setupUrl = _configuration["DataOkuConsole:SetupUrl"] ?? "https://example.com/dataoku-setup.exe";
                string releaseNotes = _configuration["DataOkuConsole:ReleaseNotes"] ?? "Bu sürümde yeni özellikler ve hata düzeltmeleri bulunmaktadır.";
                
                response.data = new DataOkuSetupVersiyon
                {
                    Version = version,
                    SetupUrl = setupUrl,
                    ReleaseNotes = releaseNotes
                };
                
                _logger.LogInformation($"getGuncelDataOkuSetupVersiyon başarılı. FirmaKodu: {firmaKodu}, Versiyon: {version}");
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"getGuncelDataOkuSetupVersiyon hatası. FirmaKodu: {firmaKodu}");
                
                var response = new ServiceResponse<DataOkuSetupVersiyon>
                {
                    messageType = ServiceResponseMessageType.Error,
                    message = $"İşlem sırasında hata: {ex.Message}"
                };
                
                return Ok(response);
            }
        }
        
        /// <summary>
        /// DataOkuConsole setup dosyalarını yükle
        /// </summary>
        [HttpPost]
        [Route("uploadSetupFile")]
        public IActionResult uploadSetupFile(IFormFile file)
        {
            try
            {
                _logger.LogInformation($"uploadSetupFile çağrıldı. Dosya boyutu: {file?.Length ?? 0} bytes");
                
                if (file == null || file.Length == 0)
                {
                    return BadRequest("Dosya yüklenmedi.");
                }
                
                // TODO: Dosyayı kaydetme işlemleri burada yapılacak
                // Örnek olarak, uploads klasörüne kaydet
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                
                var uniqueFileName = $"DataOkuSetup_{DateTime.Now:yyyyMMddHHmmss}.exe";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                
                // Başarılı yanıt döndür
                var response = new ServiceResponse<string>
                {
                    data = $"/uploads/{uniqueFileName}",
                    messageType = ServiceResponseMessageType.Success,
                    message = "Dosya başarıyla yüklendi."
                };
                
                _logger.LogInformation($"uploadSetupFile başarılı. Yüklenen dosya: {filePath}");
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "uploadSetupFile hatası.");
                
                var response = new ServiceResponse<string>
                {
                    messageType = ServiceResponseMessageType.Error,
                    message = $"Dosya yükleme sırasında hata: {ex.Message}"
                };
                
                return Ok(response);
            }
        }
    }
}
