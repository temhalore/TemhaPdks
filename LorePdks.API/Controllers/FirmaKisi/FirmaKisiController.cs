using Microsoft.AspNetCore.Mvc;
using LorePdks.COMMON.DTO.Base;
using LorePdks.COMMON.Models.ServiceResponse;
using LorePdks.COMMON.DTO.FirmaKisi;
using LorePdks.BAL.Managers.FirmaKisi.Interfaces;
using LorePdks.BAL.Managers.Kisi.Interfaces;

namespace LorePdks.API.Controllers.FirmaKisi
{
    [Route("Api/FirmaKisi")]
    [ApiController]
    public class FirmaKisiController : ControllerBase
    {
        private readonly ILogger<FirmaKisiController> _logger;
        private readonly IFirmaKisiManager _firmaKisiManager;
        private readonly IKisiManager _kisiManager;

        public FirmaKisiController(
            ILogger<FirmaKisiController> logger, 
            IFirmaKisiManager firmaKisiManager,
            IKisiManager kisiManager)
        {
            _logger = logger;
            _firmaKisiManager = firmaKisiManager;
            _kisiManager = kisiManager;
        }

        /// <summary>
        /// Firma kişi kaydeder veya günceller
        /// </summary>
        [HttpPost]
        [Route("saveFirmaKisiByFirmaKisiDto")]
        public IActionResult saveFirmaKisiByFirmaKisiDto(FirmaKisiDTO request)
        {
            var response = new ServiceResponse<FirmaKisiDTO>();
            
            // Eğer kişi yeni ise önce kişiyi kaydet
            if (request.kisiDto.id <= 0)
            {
                request.kisiDto = _kisiManager.saveKisi(request.kisiDto);
            }
            
            var dto = _firmaKisiManager.saveFirmaKisi(request);
            response.data = dto;
            response.messageType = ServiceResponseMessageType.Success;
            response.message = "Kayıt İşlemi Başarılı";
            return Ok(response);
        }

        /// <summary>
        /// Firma kişiyi siler
        /// </summary>
        [HttpPost]
        [Route("deleteFirmaKisiByEIdDto")]
        public IActionResult deleteFirmaKisiByEIdDto(EIdDTO request)
        {
            var response = new ServiceResponse<bool>();
            _firmaKisiManager.deleteFirmaKisiByFirmaKisiId(request.id);
            response.messageType = ServiceResponseMessageType.Success;
            response.message = "Silme İşlemi Başarılı";
            response.data = true;
            return Ok(response);
        }

        /// <summary>
        /// ID'ye göre firma kişi bilgisini getirir
        /// </summary>
        [HttpPost]
        [Route("getFirmaKisiDtoByEIdDto")]
        public IActionResult getFirmaKisiDtoByEIdDto(EIdDTO request)
        {
            var response = new ServiceResponse<FirmaKisiDTO>();
            var dto = _firmaKisiManager.getFirmaKisiDtoById(request.id);
            response.data = dto;
            return Ok(response);
        }

        /// <summary>
        /// Firma ID'ye göre firma kişi bilgilerini getirir
        /// </summary>
        [HttpPost]
        [Route("getFirmaKisiDtoListByFirmaIdDto")]
        public IActionResult getFirmaKisiDtoListByFirmaIdDto(EIdDTO request)
        {
            var response = new ServiceResponse<List<FirmaKisiDTO>>();
            var dtoList = _firmaKisiManager.getFirmaKisiDtoListByFirmaId(request.id);
            response.data = dtoList;
            return Ok(response);
        }
    }
}
