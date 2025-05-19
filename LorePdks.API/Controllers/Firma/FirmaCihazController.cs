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
        }

        /// <summary>
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
    }
}
