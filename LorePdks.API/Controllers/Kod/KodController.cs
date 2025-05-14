using Microsoft.AspNetCore.Mvc;
using LorePdks.COMMON.DTO.Base;
using LorePdks.COMMON.Models.ServiceResponse;
using LorePdks.COMMON.DTO.Common;
using LorePdks.BAL.Managers.Common.Kod.Interfaces;

namespace LorePdks.API.Controllers.Kod
{
    [Route("Api/Kod")]
    [ApiController]
    public class KodController : ControllerBase
    {
        private readonly ILogger<KodController> _logger;
        private readonly IKodManager _kodManager;

        public KodController(ILogger<KodController> logger, IKodManager kodManager)
        {
            _logger = logger;
            _kodManager = kodManager;
        }

        /// <summary>
        /// Kod kaydeder veya günceller
        /// </summary>
        [HttpPost]
        [Route("saveKodByKodDto")]
        public IActionResult saveKodByKodDto(KodDTO request)
        {
            var response = new ServiceResponse<KodDTO>();
            var dto = _kodManager.saveKod(request);
            response.data = dto;
            return Ok(response);
        }

        /// <summary>
        /// Kod ID'ye göre kod bilgisini getirir
        /// </summary>
        [HttpPost]
        [Route("getKodDtoByEIdDto")]
        public IActionResult getKodDtoByEIdDto(EIdDTO request)
        {
            var response = new ServiceResponse<KodDTO>();
            var dto = _kodManager.GetKodDtoByKodId(request.id);
            response.data = dto;
            return Ok(response);
        }

        /// <summary>
        /// Tip ID'ye göre kod listesini getirir
        /// </summary>
        [HttpPost]
        [Route("getKodDtoListByKodTipId")]
        public IActionResult getKodDtoListByKodTipId(SingleValueDTO<int> request)
        {
            var response = new ServiceResponse<List<KodDTO>>();
            var dto = _kodManager.GetKodDtoListByKodTipId(request.Value);
            response.data = dto;
            return Ok(response);
        }

        /// <summary>
        /// Tip ID'ye göre kod listesini getirir
        /// </summary>
        [HttpPost]
        [Route("getKodDtoListAll")]
        public IActionResult getKodDtoListAll()
        {
            var response = new ServiceResponse<List<KodDTO>>();
            var dto = _kodManager.allKodDtoList();
            response.data = dto;
            return Ok(response);
        }

        

        /// <summary>
        /// Kod cache'ini yeniler
        /// </summary>
        [HttpPost]
        [Route("refreshKodListCache")]
        public IActionResult refreshKodListCache()
        {
            var response = new ServiceResponse<string>();
            var message = _kodManager.refreshKodListCache();
            response.data = message;
            return Ok(response);
        }
    }
}
