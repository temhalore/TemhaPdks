using Microsoft.AspNetCore.Mvc;
using LorePdks.COMMON.DTO.Base;
using LorePdks.COMMON.Models.ServiceResponse;
using LorePdks.BAL.Managers.FirmaCihaz.Interfaces;
using LorePdks.BAL.Services.LogParsing.Interfaces;
using LorePdks.COMMON.DTO.FirmaCihaz;
using LorePdks.COMMON.DTO.Firma;
using LorePdks.COMMON.Models;
using LorePdks.COMMON.Enums;

namespace LorePdks.API.Controllers.Firma
{    [Route("Api/FirmaCihaz")]
    [ApiController]
    public class FirmaCihazController(ILogger<FirmaCihazController> _logger, IFirmaCihazManager _firmaCihazManager, ILogParserService _logParserService) : ControllerBase
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
        }        /// <summary>
        /// Ham log verisini parse eder (cihaz ID ile)
        /// </summary>
        [HttpPost]
        [Route("parseLogData")]
        public IActionResult parseLogData([FromBody] ParseLogDataRequest request)
        {
            var response = new ServiceResponse<Dictionary<string, object>>();
            try
            {
                // Cihazın log parser konfigürasyonunu al
                var cihaz = _firmaCihazManager.getFirmaCihazDtoById(request.firmaCihazId, true);
                if (cihaz == null)
                {
                    response.messageType = ServiceResponseMessageType.Error;
                    response.message = "Cihaz bulunamadı";
                    return BadRequest(response);
                }

                if (string.IsNullOrEmpty(cihaz.logParserConfig))
                {
                    response.messageType = ServiceResponseMessageType.Error;
                    response.message = "Cihaz için log parser konfigürasyonu tanımlanmamış";
                    return BadRequest(response);
                }

                // LogParserService ile parse et
                var parsedData = _logParserService.ParseLog(request.rawLogData, cihaz.logParserConfig);
                
                response.data = parsedData;
                response.messageType = ServiceResponseMessageType.Success;
                response.message = "Log verisi başarıyla parse edildi";
                return Ok(response);
            }
            catch (AppException ex)
            {
                response.messageType = ServiceResponseMessageType.Error;
                response.message = ex.appMessage;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.messageType = ServiceResponseMessageType.Error;
                response.message = $"Log parse edilirken hata oluştu: {ex.Message}";
                return BadRequest(response);
            }
        }        /// <summary>
        /// Cihazın log parsing konfigürasyonunu günceller
        /// </summary>
        [HttpPost]
        [Route("updateLogConfig")]
        public IActionResult updateLogConfig([FromBody] UpdateLogConfigRequest request)
        {
            var response = new ServiceResponse<FirmaCihazDTO>();
            try
            {
                // EIdDTO oluştur ve mevcut cihaz bilgilerini al
                var eidDto = new EIdDTO { eid = request.eid };
                var existingCihaz = _firmaCihazManager.getFirmaCihazDtoById(eidDto.id);
                
                // Log config alanlarını güncelle
                existingCihaz.logParserConfig = request.logParserConfig;
                existingCihaz.logDelimiter = request.logDelimiter;
                existingCihaz.logDateFormat = request.logDateFormat;
                existingCihaz.logTimeFormat = request.logTimeFormat;
                existingCihaz.logFieldMapping = request.logFieldMapping;
                existingCihaz.logSample = request.sampleLogData;
                
                // LogParserService ile konfigürasyonu doğrula
                if (!string.IsNullOrEmpty(request.logParserConfig))
                {
                    var validation = _logParserService.ValidateParserConfig(request.logParserConfig);
                    if (!validation.IsValid)
                    {
                        response.messageType = ServiceResponseMessageType.Error;
                        response.message = $"Log parser konfigürasyonu geçersiz: {validation.ErrorMessage}";
                        return BadRequest(response);
                    }
                }
                
                // Kaydet
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
        }/// <summary>
        /// Cihazın log parsing konfigürasyonunu test eder
        /// </summary>
        [HttpPost]
        [Route("testLogConfig")]
        public IActionResult testLogConfig([FromBody] TestLogConfigRequest request)
        {
            var response = new ServiceResponse<object>();
            try
            {
                // LogParserService kullanarak test et
                var testResult = _logParserService.testLogParserConfig(request.sampleLogData, request.logParserConfig);
                
                response.data = new
                {
                    success = true,
                    message = "Log konfigürasyonu başarıyla test edildi",
                    parsedData = testResult
                };
                response.messageType = ServiceResponseMessageType.Success;
                response.message = "Test başarıyla tamamlandı";
                return Ok(response);
            }
            catch (AppException ex)
            {
                response.messageType = ServiceResponseMessageType.Error;
                response.message = ex.appMessage;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.messageType = ServiceResponseMessageType.Error;
                response.message = $"Test sırasında hata oluştu: {ex.Message}";
                return BadRequest(response);
            }
        }        /// <summary>
        /// Cihazın log parsing konfigürasyonunu getirir
        /// </summary>
        [HttpPost]
        [Route("getLogConfig")]
        public IActionResult getLogConfig([FromBody] EIdDTO request)
        {
            var response = new ServiceResponse<object>();
            try
            {
                var firmaCihaz = _firmaCihazManager.getFirmaCihazDtoById(request.id);
                
                var logConfig = new
                {
                    delimiter = firmaCihaz.logDelimiter,
                    dateFormat = firmaCihaz.logDateFormat,
                    timeFormat = firmaCihaz.logTimeFormat,
                    fieldMapping = !string.IsNullOrEmpty(firmaCihaz.logFieldMapping) 
                        ? Newtonsoft.Json.JsonConvert.DeserializeObject(firmaCihaz.logFieldMapping)
                        : new object[0],
                    sampleLogData = firmaCihaz.logSample
                };
                
                response.data = logConfig;
                response.messageType = ServiceResponseMessageType.Success;
                response.message = "Log konfigürasyonu başarıyla getirildi";
                return Ok(response);
            }
            catch (AppException ex)
            {
                response.messageType = ServiceResponseMessageType.Error;
                response.message = ex.appMessage;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.messageType = ServiceResponseMessageType.Error;
                response.message = $"Log konfigürasyonu getirilirken hata oluştu: {ex.Message}";
                return BadRequest(response);
            }
        }
    }    /// <summary>
    /// Log konfigürasyon güncelleme request modeli
    /// </summary>
    public class UpdateLogConfigRequest
    {
        public string eid { get; set; } = "";
        public string logParserConfig { get; set; } = "";
        public string logDelimiter { get; set; } = "";
        public string logDateFormat { get; set; } = "";
        public string logTimeFormat { get; set; } = "";
        public string logFieldMapping { get; set; } = "";
        public string sampleLogData { get; set; } = "";
    }

    /// <summary>
    /// Log konfigürasyon test request modeli
    /// </summary>
    public class TestLogConfigRequest
    {
        public string sampleLogData { get; set; } = "";
        public string logParserConfig { get; set; } = "";
    }

    /// <summary>
    /// Log data parse request modeli
    /// </summary>
    public class ParseLogDataRequest
    {
        public int firmaCihazId { get; set; }
        public string rawLogData { get; set; } = "";
    }
}
