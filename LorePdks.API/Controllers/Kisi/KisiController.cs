using Microsoft.AspNetCore.Mvc;
using LorePdks.COMMON.DTO.Base;
using LorePdks.COMMON.Models.ServiceResponse;
using LorePdks.COMMON.DTO.Kisi;
using LorePdks.BAL.Managers.Kisi.Interfaces;
using LorePdks.COMMON.DTO.Auth;

namespace LorePdks.API.Controllers.Kisi
{
    [Route("api/Kisi")]
    [ApiController]
    public class KisiController(
        ILogger<KisiController> _logger,
        IKisiManager _kisiManager) : ControllerBase
    {
        /// <summary>
        /// Kişi kaydeder veya günceller
        /// </summary>
        [HttpPost]
        [Route("saveKisiByKisiDto")]
        public IActionResult saveKisiByKisiDto(KisiDTO request)
        {
            var response = new ServiceResponse<KisiDTO>();
            var dto = _kisiManager.saveKisi(request);
            response.data = dto;
            return Ok(response);
        }

        /// <summary>
        /// Kişiyi siler
        /// </summary>
        [HttpPost]
        [Route("deleteKisiByEIdDto")]
        public IActionResult deleteKisiByEIdDto(EIdDTO request)
        {
            var response = new ServiceResponse<bool>();
            _kisiManager.deleteKisiByKisiId(request.id);
            response.messageType = ServiceResponseMessageType.Success;
            response.message = "Silme İşlemi Başarılı";
            response.data = true;
            return Ok(response);
        }

        /// <summary>
        /// ID'ye göre kişi bilgisini getirir
        /// </summary>
        [HttpPost]
        [Route("getKisiDtoByEIdDto")]
        public IActionResult getKisiDtoByEIdDto(EIdDTO request)
        {
            var response = new ServiceResponse<KisiDTO>();
            var dto = _kisiManager.getKisiDtoById(request.id);
            response.data = dto;
            return Ok(response);
        }

        /// <summary>
        /// Tüm kişileri listeler
        /// </summary>
        [HttpPost]
        [Route("getAllKisiList")]
        public IActionResult getAllKisiList()
        {
            var response = new ServiceResponse<List<KisiDTO>>();
            var dto = _kisiManager.getKisiDtoListById();
            response.data = dto;
            return Ok(response);
        }

        /// <summary>
        /// Kullanıcı adı ve şifreye göre kişi bilgisini getirir
        /// </summary>
        [HttpPost]
        [Route("getKisiByLoginNameAndSifre")]
        public IActionResult getKisiByLoginNameAndSifre(LoginRequestDTO request)
        {
            var response = new ServiceResponse<KisiDTO>();
            var dto = _kisiManager.getKisiDtoByLoginNameAndSifre(request.loginName, request.sifre);
            response.data = dto;
            return Ok(response);
        }

        /// <summary>
        /// Kullanıcı adına göre kişi bilgisini getirir
        /// </summary>
        [HttpPost]
        [Route("getKisiByLoginName")]
        public IActionResult getKisiByLoginName(SingleValueDTO<string> request)
        {
            var response = new ServiceResponse<KisiDTO>();
            var dto = _kisiManager.getKisiDtoByLoginName(request.Value);
            response.data = dto;
            return Ok(response);
        }

        /// <summary>
        /// TC'ye göre kişi bilgisini getirir
        /// </summary>
        [HttpPost]
        [Route("getKisiByTc")]
        public IActionResult getKisiByTc(SingleValueDTO<string> request)
        {
            var response = new ServiceResponse<KisiDTO>();
            var dto = _kisiManager.getKisiDtoByTc(request.Value);
            response.data = dto;
            return Ok(response);
        }

        /// <summary>
        /// Arama metni ile kişileri arar
        /// </summary>
        [HttpPost]
        [Route("getKisiListByAramaText")]
        public IActionResult getKisiListByAramaText(SingleValueDTO<string> request)
        {
            var response = new ServiceResponse<List<KisiDTO>>();
            var dto = _kisiManager.getKisiDtoListByAramaText(request.Value);
            response.data = dto;
            return Ok(response);
        }
    }
}
