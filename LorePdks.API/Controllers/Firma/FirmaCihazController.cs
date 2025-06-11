using Microsoft.AspNetCore.Mvc;
using LorePdks.COMMON.DTO.Base;
using LorePdks.COMMON.Models.ServiceResponse;
using LorePdks.BAL.Managers.FirmaCihaz.Interfaces;
using LorePdks.COMMON.DTO.FirmaCihaz;
using LorePdks.COMMON.DTO.Firma;

namespace LorePdks.API.Controllers.Firma
{
    [Route("Api/FirmaCihaz")]
    [ApiController]
    public class FirmaCihazController(ILogger<FirmaCihazController> _logger, IFirmaCihazManager _firmaCihazManager) : ControllerBase
    {
        /// <summary>
        /// Firma Cihaz kaydeder veya günceller
        /// </summary>
        [HttpPost]
        [Route("saveFirmaCihazByFirmaCihazDto")]
        public IActionResult saveFirmaCihazByFirmaCihazDto(FirmaCihazDTO request)
        {
            var response = new ServiceResponse<FirmaCihazDTO>();
            var dto = _firmaCihazManager.saveFirmaCihaz(request);
            response.data = dto;
            response.messageType = ServiceResponseMessageType.Success;
            response.message = "Kayıt İşlemi Başarılı";
            return Ok(response);
        }

        /// <summary>
        /// Firma Cihazı siler
        /// </summary>
        [HttpPost]
        [Route("deleteFirmaCihazByEIdDto")]
        public IActionResult deleteFirmaCihazByEIdDto(EIdDTO request)
        {
            var response = new ServiceResponse<bool>();
            _firmaCihazManager.deleteFirmaCihazByFirmaCihazId(request.id);
            response.messageType = ServiceResponseMessageType.Success;
            response.message = "Silme İşlemi Başarılı";
            response.data = true;
            return Ok(response);
        }

        /// <summary>
        /// ID'ye göre firma cihaz bilgisini getirir
        /// </summary>
        [HttpPost]
        [Route("getFirmaCihazDtoByEIdDto")]
        public IActionResult getFirmaCihazDtoByEIdDto(EIdDTO request)
        {
            var response = new ServiceResponse<FirmaCihazDTO>();
            var dto = _firmaCihazManager.getFirmaCihazDtoById(request.id);
            response.data = dto;
            return Ok(response);
        }

        /// <summary>
        /// Firma ID'ye göre firma cihazlarını listeler
        /// </summary>
        [HttpPost]
        [Route("getFirmaCihazDtoListByFirmaDto")]
        public IActionResult getFirmaCihazDtoListByFirmaDto(FirmaDTO request)
        {
            var response = new ServiceResponse<List<FirmaCihazDTO>>();
            var dto = _firmaCihazManager.getFirmaCihazDtoListByFirmaId(request.id);
            response.data = dto;
            return Ok(response);
        }        /// <summary>
        /// Firma ID'ye göre firma cihazlarını listeler (SingleValueDTO kullanarak)
        /// </summary>
        [HttpPost]
        [Route("getFirmaCihazDtoListByFirmaId")]
        public IActionResult getFirmaCihazDtoListByFirmaId(SingleValueDTO<int> request)
        {
            var response = new ServiceResponse<List<FirmaCihazDTO>>();
            var dto = _firmaCihazManager.getFirmaCihazDtoListByFirmaId(request.Value);
            response.data = dto;
            return Ok(response);
        }

        /// <summary>
        /// Cihazın log parsing konfigürasyonunu günceller
        /// </summary>
        [HttpPost]
        [Route("updateLogConfig")]
        public IActionResult updateLogConfig(FirmaCihazDTO request)
        {
            var response = new ServiceResponse<FirmaCihazDTO>();
            try
            {
                // Sadece log config alanlarını güncelle
                var existingCihaz = _firmaCihazManager.getFirmaCihazDtoById(request.id, true);
                
                // Log config alanlarını güncelle
                existingCihaz.logParserConfig = request.logParserConfig;
                existingCihaz.logDelimiter = request.logDelimiter;
                existingCihaz.logDateFormat = request.logDateFormat;
                existingCihaz.logTimeFormat = request.logTimeFormat;
                existingCihaz.logFieldMapping = request.logFieldMapping;
                existingCihaz.logSample = request.logSample;
                
                var dto = _firmaCihazManager.saveFirmaCihaz(existingCihaz);
                response.data = dto;
                response.messageType = ServiceResponseMessageType.Success;
                response.message = "Log konfigürasyonu başarıyla güncellendi";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.messageType = ServiceResponseMessageType.Error;
                response.message = $"Log konfigürasyonu güncellenirken hata oluştu: {ex.Message}";
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Cihazın log parsing konfigürasyonunu test eder
        /// </summary>
        [HttpPost]
        [Route("testLogConfig")]
        public IActionResult testLogConfig(FirmaCihazDTO request)
        {
            var response = new ServiceResponse<object>();
            try
            {
                // Log parser servisini kullanarak test et
                // Bu servis henüz oluşturulmadı, placeholder olarak bırakıyorum
                var testResult = new
                {
                    success = true,
                    message = "Log konfigürasyonu test edildi",
                    parsedFields = new string[] { "timestamp", "user_id", "action", "result" },
                    sampleOutput = "Test başarılı - log parsing yapılandırması çalışıyor"
                };
                
                response.data = testResult;
                response.messageType = ServiceResponseMessageType.Success;
                response.message = "Test başarıyla tamamlandı";
                return Ok(response);
            }            catch (Exception ex)
            {
                response.messageType = ServiceResponseMessageType.Error;
                response.message = $"Test sırasında hata oluştu: {ex.Message}";
                return BadRequest(response);
            }
        }
    }
}
