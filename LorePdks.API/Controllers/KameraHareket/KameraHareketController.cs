using LorePdks.BAL.Managers.Kamera.Interfaces;
using LorePdks.COMMON.DTO.KameraHareket;
using LorePdks.COMMON.Enums;
using LorePdks.COMMON.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace LorePdks.API.Controllers.KameraHareket
{
    [ApiController]
    [Route("api/[controller]")]
    public class KameraHareketController : ControllerBase
    {
        private readonly IKameraHareketManager _kameraHareketManager;

        public KameraHareketController(IKameraHareketManager kameraHareketManager)
        {
            _kameraHareketManager = kameraHareketManager;
        }

        /// <summary>
        /// Kamera hareket kaydı oluştur veya güncelle
        /// </summary>
        [HttpPost("save")]
        public ActionResult<KameraHareketDTO> SaveKameraHareket([FromBody] KameraHareketDTO kameraHareketDto)
        {
            try
            {
                var result = _kameraHareketManager.saveKameraHareket(kameraHareketDto);
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
        /// ID'ye göre Kamera hareket kaydı getir
        /// </summary>
        [HttpGet("{id}")]
        public ActionResult<KameraHareketDTO> GetKameraHareketById(int id)
        {
            try
            {
                var result = _kameraHareketManager.getKameraHareketDtoById(id, isYoksaHataDondur: true);
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
        /// Firmaya göre Kamera hareket kayıtları listele
        /// </summary>
        [HttpGet("firma/{firmaId}")]
        public ActionResult<List<KameraHareketDTO>> GetKameraHareketListByFirmaId(int firmaId)
        {
            try
            {
                var result = _kameraHareketManager.getKameraHareketDtoListByFirmaId(firmaId);
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
        /// Firma cihazına göre Kamera hareket kayıtları listele
        /// </summary>
        [HttpGet("firmacihaz/{firmaCihazId}")]
        public ActionResult<List<KameraHareketDTO>> GetKameraHareketListByFirmaCihazId(int firmaCihazId)
        {
            try
            {
                var result = _kameraHareketManager.getKameraHareketDtoListByFirmaCihazId(firmaCihazId);
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
        /// Tarih aralığına göre Kamera hareket kayıtları listele
        /// </summary>
        [HttpGet("firma/{firmaId}/daterange")]
        public ActionResult<List<KameraHareketDTO>> GetKameraHareketListByDateRange(
            int firmaId, 
            [FromQuery] DateTime baslangicTarihi, 
            [FromQuery] DateTime bitisTarihi)
        {
            try
            {
                var result = _kameraHareketManager.getKameraHareketDtoListByFirmaIdAndDateRange(
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
        /// Kamera hareket kaydını sil
        /// </summary>
        [HttpDelete("{id}")]
        public ActionResult DeleteKameraHareket(int id)
        {
            try
            {
                _kameraHareketManager.deleteKameraHareketById(id);
                return Ok(new { message = "Kamera hareket kaydı başarıyla silindi" });
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
