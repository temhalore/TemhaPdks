using Microsoft.AspNetCore.Mvc;
using LorePdks.COMMON.Configuration;
using LorePdks.COMMON.DTO.Base;
using LorePdks.COMMON.Models;
using LorePdks.COMMON.Models.ServiceResponse;
using LorePdks.COMMON.Enums;
using LorePdks.BAL.Managers.Deneme;
using LorePdks.BAL.Managers.Deneme.Interfaces;
using LorePdks.COMMON.Logging;
using LorePdks.COMMON.DTO.Common;

namespace LorePdks.API.Controllers.Firma
{
    [Route("Api/Firma")]
    [ApiController]
    public class FirmaController(ILogger<FirmaController> _logger, IFirmaManager _firmaManager) : ControllerBase
    {

        [HttpPost]
        [Route("saveFirmaByFirmaDto")]
        public IActionResult saveFirmaByFirmaDto(FirmaDTO request)
        {
            var response = new ServiceResponse<FirmaDTO>();
            var dto = _firmaManager.saveFirma(request);
            response.data = dto;
            return Ok(response);
        }

        [HttpPost]
        [Route("deleteFirmaByEIdDto")]
        public IActionResult deleteFirmaByEIdDto(EIdDTO request)
        {
            var response = new ServiceResponse<FirmaDTO>();
            _firmaManager.deleteFirmaByFirmaId(request.id);
            return Ok(response);
        }

        [HttpPost]
        [Route("getFirmaDtoByEIdDto")]
        public IActionResult getFirmaDtoByEIdDto(EIdDTO request)
        {
            var response = new ServiceResponse<FirmaDTO>();
            var dto = _firmaManager.getFirmaDtoById(request.id);
            response.data = dto;
            return Ok(response);
        }

        // bir user a tanımlı firma listler
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
