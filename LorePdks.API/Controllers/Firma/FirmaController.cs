using Microsoft.AspNetCore.Mvc;
using LorePdks.COMMON.DTO.Base;
using LorePdks.COMMON.Models.ServiceResponse;
using LorePdks.COMMON.DTO.Firma;
using LorePdks.BAL.Managers.Firma.Interfaces;

namespace LorePdks.API.Controllers.Firma
{
    [Route("Api/Firma")]
    [ApiController]
    public class FirmaController(ILogger<FirmaController> _logger, IFirmaManager _firmaManager) : ControllerBase
    {
        /// <summary>
        /// Firma kaydeder veya günceller
        /// </summary>
        [HttpPost]
        [Route("saveFirmaByFirmaDto")]
        public IActionResult saveFirmaByFirmaDto(FirmaDTO request)
        {
            var response = new ServiceResponse<FirmaDTO>();
            var dto = _firmaManager.saveFirma(request);
            response.data = dto;
            response.messageType = ServiceResponseMessageType.Success;
            response.message = "Kayıt İşlemi Başarılı";
            return Ok(response);
        }

        /// <summary>
        /// Firmayı siler
        /// </summary>
        [HttpPost]
        [Route("deleteFirmaByEIdDto")]
        public IActionResult deleteFirmaByEIdDto(EIdDTO request)
        {
            var response = new ServiceResponse<bool>();
            _firmaManager.deleteFirmaByFirmaId(request.id);
            response.messageType = ServiceResponseMessageType.Success;
            response.message = "Silme İşlemi Başarılı";
            response.data = true;
            return Ok(response);
        }

        /// <summary>
        /// ID'ye göre firma bilgisini getirir
        /// </summary>
        [HttpPost]
        [Route("getFirmaDtoByEIdDto")]
        public IActionResult getFirmaDtoByEIdDto(EIdDTO request)
        {
            var response = new ServiceResponse<FirmaDTO>();
            var dto = _firmaManager.getFirmaDtoById(request.id);
            response.data = dto;
            return Ok(response);
        }

        /// <summary>
        /// Tüm firmaları listeler
        /// </summary>
        [HttpPost]
        [Route("getAllFirmaListDto")]
        public IActionResult getAllFirmaListDto()
        {
            var response = new ServiceResponse<List<FirmaDTO>>();
            var dto = _firmaManager.getFirmaDtoListById();
            response.data = dto;
            return Ok(response);
        }
    }
}
