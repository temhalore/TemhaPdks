using Microsoft.AspNetCore.Mvc;
using LorePdks.COMMON.DTO.Base;
using LorePdks.COMMON.Models.ServiceResponse;
using LorePdks.COMMON.DTO.DataOkuConsole;
using LorePdks.BAL.Managers.Firma.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.IO;
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
                // TODO: Gerçek uygulamada firma veya genel versiyon bilgisi veritabanından alınabilir bunu indirince firma için firma tablosuna yazacağız
                // Şimdilik config dosyasından okuyalım
                string version = _configuration["DataOkuConsole:CurrentVersion"] ?? "1.0.0";
                
                response.data = new DataOkuVersiyon
                {
                    Success = true,
                    Message = "Başarılı",
                    //Version = version
                     Version = "0.0.9"
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
                string setupUrl = _configuration["DataOkuConsole:SetupUrl"] ?? "https://example.com/dataoku-setup.zip";
                string releaseNotes = _configuration["DataOkuConsole:ReleaseNotes"] ?? "Bu sürümde yeni özellikler ve hata düzeltmeleri bulunmaktadır.";
                string defaultInstallPath = _configuration["DataOkuConsole:DefaultInstallPath"] ?? "C:\\LoreSoft\\";
                string executablePath = _configuration["DataOkuConsole:ExecutablePath"] ?? "Temha.DataOkuConsole.exe";
                
                response.data = new DataOkuSetupVersiyon
                {
                    Version = version,
                    SetupUrl = setupUrl,
                    ReleaseNotes = releaseNotes,
                    DefaultInstallPath = defaultInstallPath,
                    ExecutablePath = executablePath
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
                
                // Dosya uzantısı kontrolü yap (.zip, .rar veya .exe dosyaları kabul ediliyor)
                string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (extension != ".zip" && extension != ".rar" && extension != ".exe")
                {
                    return BadRequest("Sadece .zip, .rar veya .exe dosyaları yüklenebilir.");
                }
                
                // Uploads klasörüne dosyayı kaydet
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                
                // Dosya adını uzantıyla birlikte belirle
                var uniqueFileName = $"DataOkuConsole_latest{extension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                
                // Eğer dosya mevcutsa önce sil
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                
                // Yeni dosyayı kaydet
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                
                // appSettings.json dosyasını güncelle
                UpdateAppSettings(extension);
                
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
        
        /// <summary>
        /// appSettings.json dosyasını günceller
        /// </summary>
        private void UpdateAppSettings(string extension)
        {
            try
            {
                // AppSettings yolunu ve içeriğini al
                var appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
                var json = System.IO.File.ReadAllText(appSettingsPath);
                
                // Json dosyasını objeye dönüştür
                var jsonObj = System.Text.Json.JsonDocument.Parse(json).RootElement;
                
                // JsonElement'i dynamic olarak kullanamadığımız için, yeni bir JsonObject oluşturalım
                using (JsonDocument document = JsonDocument.Parse(json))
                {
                    var root = document.RootElement;
                    
                    using (var stream = new MemoryStream())
                    {
                        using (var writer = new Utf8JsonWriter(stream))
                        {
                            writer.WriteStartObject();
                            
                            // Root seviyesindeki tüm özellikleri kopyala
                            foreach (var property in root.EnumerateObject())
                            {
                                if (property.Name != "DataOkuConsole")
                                {
                                    property.WriteTo(writer);
                                }
                                else
                                {
                                    // DataOkuConsole bölümünü güncelle
                                    writer.WritePropertyName("DataOkuConsole");
                                    writer.WriteStartObject();
                                    
                                    foreach (var setting in property.Value.EnumerateObject())
                                    {
                                        if (setting.Name == "SetupUrl")
                                        {
                                            writer.WritePropertyName("SetupUrl");
                                            writer.WriteStringValue($"https://api.yourserver.com/uploads/DataOkuConsole_latest{extension}");
                                        }
                                        else
                                        {
                                            setting.WriteTo(writer);
                                        }
                                    }
                                    
                                    writer.WriteEndObject();
                                }
                            }
                            
                            writer.WriteEndObject();
                        }
                        
                        // Güncellenmiş JSON'u dosyaya yaz
                        var updatedJson = Encoding.UTF8.GetString(stream.ToArray());
                        System.IO.File.WriteAllText(appSettingsPath, updatedJson);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "appSettings.json dosyası güncellenirken hata oluştu.");
                throw;
            }
        }
    }
}
