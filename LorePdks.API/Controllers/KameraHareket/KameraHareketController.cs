using LorePdks.BAL.Managers.Kamera.Interfaces;
using LorePdks.COMMON.DTO.KameraHareket;
using LorePdks.COMMON.DTO.Base;
using LorePdks.COMMON.Models.ServiceResponse;
using LorePdks.COMMON.Enums;
using LorePdks.COMMON.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace LorePdks.API.Controllers.KameraHareket
{
    [Route("api/KameraHareket")]
    [ApiController]
    public class KameraHareketController : ControllerBase
    {
        private readonly IKameraHareketManager _kameraHareketManager;

        public KameraHareketController(IKameraHareketManager kameraHareketManager)
        {
            _kameraHareketManager = kameraHareketManager;
        }        /// <summary>
        /// Kamera hareket kaydı oluştur veya güncelle
        /// </summary>
        [HttpPost]
        [Route("saveKameraHareketByKameraHareketDto")]
        public IActionResult saveKameraHareketByKameraHareketDto(KameraHareketDTO request)
        {
            var response = new ServiceResponse<KameraHareketDTO>();
            try
            {
                var result = _kameraHareketManager.saveKameraHareket(request);
                response.data = result;
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
                response.message = "Bir hata oluştu";
                return StatusCode(500, response);
            }
        }        /// <summary>
        /// ID'ye göre Kamera hareket kaydı getir
        /// </summary>
        [HttpPost]
        [Route("getKameraHareketDtoByEIdDto")]
        public IActionResult getKameraHareketDtoByEIdDto(EIdDTO request)
        {
            var response = new ServiceResponse<KameraHareketDTO>();
            try
            {
                var result = _kameraHareketManager.getKameraHareketDtoById(request.id, isYoksaHataDondur: true);
                response.data = result;
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
                response.message = "Bir hata oluştu";
                return StatusCode(500, response);
            }
        }        /// <summary>
        /// Firmaya göre Kamera hareket kayıtları listele
        /// </summary>
        [HttpPost]
        [Route("getKameraHareketListByFirmaEIdDto")]
        public IActionResult getKameraHareketListByFirmaEIdDto(EIdDTO request)
        {
            var response = new ServiceResponse<List<KameraHareketDTO>>();
            try
            {
                var result = _kameraHareketManager.getKameraHareketDtoListByFirmaId(request.id);
                response.data = result;
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
                response.message = "Bir hata oluştu";
                return StatusCode(500, response);
            }
        }        /// <summary>
        /// Firma cihazına göre Kamera hareket kayıtları listele
        /// </summary>
        [HttpPost]
        [Route("getKameraHareketListByFirmaCihazEIdDto")]
        public IActionResult getKameraHareketListByFirmaCihazEIdDto(EIdDTO request)
        {
            var response = new ServiceResponse<List<KameraHareketDTO>>();
            try
            {
                var result = _kameraHareketManager.getKameraHareketDtoListByFirmaCihazId(request.id);
                response.data = result;
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
                response.message = "Bir hata oluştu";
                return StatusCode(500, response);
            }
        }        /// <summary>
        /// Tarih aralığına göre Kamera hareket kayıtları listele
        /// </summary>
        [HttpPost]
        [Route("getKameraHareketListByDateRange")]
        public IActionResult getKameraHareketListByDateRange(KameraHareketDateRangeRequestDTO request)
        {
            var response = new ServiceResponse<List<KameraHareketDTO>>();
            try
            {
                var result = _kameraHareketManager.getKameraHareketDtoListByFirmaIdAndDateRange(
                    request.firmaId, request.baslangicTarihi, request.bitisTarihi);
                response.data = result;
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
                response.message = "Bir hata oluştu";
                return StatusCode(500, response);
            }
        }

        /// <summary>
        /// Tüm Kamera hareket kayıtlarını listele
        /// </summary>
        [HttpPost]
        [Route("getAllKameraHareketList")]
        public IActionResult getAllKameraHareketList()
        {
            var response = new ServiceResponse<List<KameraHareketDTO>>();
            try
            {
                // Burada manager'da getAllKameraHareketDtoList metodunu çağıracağız
                // Şimdilik boş liste döndürüyoruz
                response.data = new List<KameraHareketDTO>();
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
                response.message = "Bir hata oluştu";
                return StatusCode(500, response);
            }
        }

        /// <summary>
        /// Kamera hareket kaydını sil
        /// </summary>
        [HttpPost]
        [Route("deleteKameraHareketByEIdDto")]
        public IActionResult deleteKameraHareketByEIdDto(EIdDTO request)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                _kameraHareketManager.deleteKameraHareketById(request.id);
                response.messageType = ServiceResponseMessageType.Success;
                response.message = "Kamera hareket kaydı başarıyla silindi";
                response.data = true;
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
                response.message = "Bir hata oluştu";
                return StatusCode(500, response);
            }
        }
    }
}
