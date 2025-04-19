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

namespace LorePdks.API.Controllers.Deneme
{

    /// <summary>
    /// firmalardan toplanan tüm hareket çağrıları buraya düşecek ilk
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class HareketController : ControllerBase
    {
        private readonly IHareketManager _hareketManager;

        public HareketController(IHareketManager hareketManager)
        {
            _hareketManager = hareketManager;
        }
        [HttpPost]
        [Route("saveFirmaByFirmaDto")]
        public IActionResult SaveHareket(HareketDTO hareketDto)
        {
            var result = _hareketManager.saveHareket(hareketDto);
            return Ok(result);
        }

        [Route("GetHareketListByFirmaId")]
        public IActionResult GetHareketListByFirmaDto(FirmaDTO firmaDto)
        {
            var result = _hareketManager.getHareketListByFirmaId(firmaDto.id);
            return Ok(result);
        }
    }

}
