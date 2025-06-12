using LorePdks.BAL.Managers.Pdks.Interfaces;
using LorePdks.COMMON.DTO.PdksHareket;
using LorePdks.COMMON.DTO.Base;
using LorePdks.COMMON.Models.ServiceResponse;
using LorePdks.COMMON.Enums;
using LorePdks.COMMON.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace LorePdks.API.Controllers.PdksHareket
{
    [Route("api/PdksHareket")]
    [ApiController]
    public class PdksHareketController : ControllerBase
    {
        private readonly IPdksHareketManager _pdksHareketManager;

        public PdksHareketController(IPdksHareketManager pdksHareketManager)
        {
            _pdksHareketManager = pdksHareketManager;
        }        /// <summary>
        /// PDKS hareket kaydı oluştur veya güncelle
        /// </summary>
        [HttpPost]
        [Route("savePdksHareketByPdksHareketDto")]
        public IActionResult savePdksHareketByPdksHareketDto(PdksHareketDTO request)
        {
            var response = new ServiceResponse<PdksHareketDTO>();
            try
            {
                var result = _pdksHareketManager.savePdksHareket(request);
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
        /// ID'ye göre PDKS hareket kaydı getir
        /// </summary>
        [HttpPost]
        [Route("getPdksHareketDtoByEIdDto")]
        public IActionResult getPdksHareketDtoByEIdDto(EIdDTO request)
        {
            var response = new ServiceResponse<PdksHareketDTO>();
            try
            {
                var result = _pdksHareketManager.getPdksHareketDtoById(request.id, isYoksaHataDondur: true);
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
        /// Firmaya göre PDKS hareket kayıtları listele
        /// </summary>
        [HttpPost]
        [Route("getPdksHareketListByFirmaEIdDto")]
        public IActionResult getPdksHareketListByFirmaEIdDto(EIdDTO request)
        {
            var response = new ServiceResponse<List<PdksHareketDTO>>();
            try
            {
                var result = _pdksHareketManager.getPdksHareketDtoListByFirmaId(request.id);
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
        /// Kişiye göre PDKS hareket kayıtları listele
        /// </summary>
        [HttpPost]
        [Route("getPdksHareketListByKisiEIdDto")]
        public IActionResult getPdksHareketListByKisiEIdDto(EIdDTO request)
        {
            var response = new ServiceResponse<List<PdksHareketDTO>>();
            try
            {
                var result = _pdksHareketManager.getPdksHareketDtoListByKisiId(request.id);
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
        /// Firma cihazına göre PDKS hareket kayıtları listele
        /// </summary>
        [HttpPost]
        [Route("getPdksHareketListByFirmaCihazEIdDto")]
        public IActionResult getPdksHareketListByFirmaCihazEIdDto(EIdDTO request)
        {
            var response = new ServiceResponse<List<PdksHareketDTO>>();
            try
            {
                var result = _pdksHareketManager.getPdksHareketDtoListByFirmaCihazId(request.id);
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
        /// Tarih aralığına göre PDKS hareket kayıtları listele
        /// </summary>
        [HttpPost]
        [Route("getPdksHareketListByDateRange")]
        public IActionResult getPdksHareketListByDateRange(PdksHareketDateRangeRequestDTO request)
        {
            var response = new ServiceResponse<List<PdksHareketDTO>>();
            try
            {
                var result = _pdksHareketManager.getPdksHareketDtoListByFirmaIdAndDateRange(
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
        }        /// <summary>
        /// PDKS hareket kaydını sil
        /// </summary>
        [HttpPost]
        [Route("deletePdksHareketByEIdDto")]
        public IActionResult deletePdksHareketByEIdDto(EIdDTO request)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                _pdksHareketManager.deletePdksHareketById(request.id);
                response.messageType = ServiceResponseMessageType.Success;
                response.message = "PDKS hareket kaydı başarıyla silindi";
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
        }        /// <summary>
        /// Tüm PDKS hareket kayıtlarını listele
        /// </summary>
        [HttpPost]
        [Route("getAllPdksHareketList")]
        public IActionResult getAllPdksHareketList()
        {
            var response = new ServiceResponse<List<PdksHareketDTO>>();
            try
            {
                // Burada manager'da getAllPdksHareketDtoList metodunu çağıracağız
                // Şimdilik boş liste döndürüyoruz
                response.data = new List<PdksHareketDTO>();
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
