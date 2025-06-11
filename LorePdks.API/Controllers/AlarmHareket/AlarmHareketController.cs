using LorePdks.BAL.Managers.Alarm.Interfaces;
using LorePdks.COMMON.DTO.AlarmHareket;
using LorePdks.COMMON.Enums;
using LorePdks.COMMON.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace LorePdks.API.Controllers.AlarmHareket
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlarmHareketController : ControllerBase
    {
        private readonly IAlarmHareketManager _alarmHareketManager;

        public AlarmHareketController(IAlarmHareketManager alarmHareketManager)
        {
            _alarmHareketManager = alarmHareketManager;
        }

        /// <summary>
        /// Alarm hareket kaydı oluştur veya güncelle
        /// </summary>
        [HttpPost("save")]
        public ActionResult<AlarmHareketDTO> SaveAlarmHareket([FromBody] AlarmHareketDTO alarmHareketDto)
        {
            try
            {
                var result = _alarmHareketManager.saveAlarmHareket(alarmHareketDto);
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
        /// ID'ye göre Alarm hareket kaydı getir
        /// </summary>
        [HttpGet("{id}")]
        public ActionResult<AlarmHareketDTO> GetAlarmHareketById(int id)
        {
            try
            {
                var result = _alarmHareketManager.getAlarmHareketDtoById(id, isYoksaHataDondur: true);
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
        /// Firmaya göre Alarm hareket kayıtları listele
        /// </summary>
        [HttpGet("firma/{firmaId}")]
        public ActionResult<List<AlarmHareketDTO>> GetAlarmHareketListByFirmaId(int firmaId)
        {
            try
            {
                var result = _alarmHareketManager.getAlarmHareketDtoListByFirmaId(firmaId);
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
        /// Firma cihazına göre Alarm hareket kayıtları listele
        /// </summary>
        [HttpGet("firmacihaz/{firmaCihazId}")]
        public ActionResult<List<AlarmHareketDTO>> GetAlarmHareketListByFirmaCihazId(int firmaCihazId)
        {
            try
            {
                var result = _alarmHareketManager.getAlarmHareketDtoListByFirmaCihazId(firmaCihazId);
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
        /// Tarih aralığına göre Alarm hareket kayıtları listele
        /// </summary>
        [HttpGet("firma/{firmaId}/daterange")]
        public ActionResult<List<AlarmHareketDTO>> GetAlarmHareketListByDateRange(
            int firmaId, 
            [FromQuery] DateTime baslangicTarihi, 
            [FromQuery] DateTime bitisTarihi)
        {
            try
            {
                var result = _alarmHareketManager.getAlarmHareketDtoListByFirmaIdAndDateRange(
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
        /// Alarm hareket kaydını sil
        /// </summary>
        [HttpDelete("{id}")]
        public ActionResult DeleteAlarmHareket(int id)
        {
            try
            {
                _alarmHareketManager.deleteAlarmHareketById(id);
                return Ok(new { message = "Alarm hareket kaydı başarıyla silindi" });
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
