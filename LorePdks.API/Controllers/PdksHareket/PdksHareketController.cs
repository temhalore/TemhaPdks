using LorePdks.BAL.Managers.Pdks.Interfaces;
using LorePdks.COMMON.DTO.PdksHareket;
using LorePdks.COMMON.Enums;
using LorePdks.COMMON.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace LorePdks.API.Controllers.PdksHareket
{
    [ApiController]
    [Route("api/[controller]")]
    public class PdksHareketController : ControllerBase
    {
        private readonly IPdksHareketManager _pdksHareketManager;

        public PdksHareketController(IPdksHareketManager pdksHareketManager)
        {
            _pdksHareketManager = pdksHareketManager;
        }

        /// <summary>
        /// PDKS hareket kaydı oluştur veya güncelle
        /// </summary>
        [HttpPost("save")]
        public ActionResult<PdksHareketDTO> SavePdksHareket([FromBody] PdksHareketDTO pdksHareketDto)
        {
            try
            {
                var result = _pdksHareketManager.savePdksHareket(pdksHareketDto);
                return Ok(result);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.appMessage, code = ex.errorCode });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Bir hata oluştu", error = ex.Message });
            }
        }

        /// <summary>
        /// ID'ye göre PDKS hareket kaydı getir
        /// </summary>
        [HttpGet("{id}")]
        public ActionResult<PdksHareketDTO> GetPdksHareketById(int id)
        {
            try
            {
                var result = _pdksHareketManager.getPdksHareketDtoById(id, isYoksaHataDondur: true);
                return Ok(result);
            }
            catch (AppException ex)
            {
                return NotFound(new { message = ex.appMessage, code = ex.errorCode });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Bir hata oluştu", error = ex.Message });
            }
        }

        /// <summary>
        /// Firmaya göre PDKS hareket kayıtları listele
        /// </summary>
        [HttpGet("firma/{firmaId}")]
        public ActionResult<List<PdksHareketDTO>> GetPdksHareketListByFirmaId(int firmaId)
        {
            try
            {
                var result = _pdksHareketManager.getPdksHareketDtoListByFirmaId(firmaId);
                return Ok(result);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.appMessage, code = ex.errorCode });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Bir hata oluştu", error = ex.Message });
            }
        }

        /// <summary>
        /// Kişiye göre PDKS hareket kayıtları listele
        /// </summary>
        [HttpGet("kisi/{kisiId}")]
        public ActionResult<List<PdksHareketDTO>> GetPdksHareketListByKisiId(int kisiId)
        {
            try
            {
                var result = _pdksHareketManager.getPdksHareketDtoListByKisiId(kisiId);
                return Ok(result);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.appMessage, code = ex.errorCode });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Bir hata oluştu", error = ex.Message });
            }
        }

        /// <summary>
        /// Firma cihazına göre PDKS hareket kayıtları listele
        /// </summary>
        [HttpGet("firmacihaz/{firmaCihazId}")]
        public ActionResult<List<PdksHareketDTO>> GetPdksHareketListByFirmaCihazId(int firmaCihazId)
        {
            try
            {
                var result = _pdksHareketManager.getPdksHareketDtoListByFirmaCihazId(firmaCihazId);
                return Ok(result);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.appMessage, code = ex.errorCode });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Bir hata oluştu", error = ex.Message });
            }
        }

        /// <summary>
        /// Tarih aralığına göre PDKS hareket kayıtları listele
        /// </summary>
        [HttpGet("firma/{firmaId}/daterange")]
        public ActionResult<List<PdksHareketDTO>> GetPdksHareketListByDateRange(
            int firmaId, 
            [FromQuery] DateTime baslangicTarihi, 
            [FromQuery] DateTime bitisTarihi)
        {
            try
            {
                var result = _pdksHareketManager.getPdksHareketDtoListByFirmaIdAndDateRange(
                    firmaId, baslangicTarihi, bitisTarihi);
                return Ok(result);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.appMessage, code = ex.errorCode });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Bir hata oluştu", error = ex.Message });
            }
        }

        /// <summary>
        /// PDKS hareket kaydını sil
        /// </summary>
        [HttpDelete("{id}")]
        public ActionResult DeletePdksHareket(int id)
        {
            try
            {
                _pdksHareketManager.deletePdksHareketById(id);
                return Ok(new { message = "PDKS hareket kaydı başarıyla silindi" });
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.appMessage, code = ex.errorCode });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Bir hata oluştu", error = ex.Message });
            }
        }
    }
}
