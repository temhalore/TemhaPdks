using LorePdks.BAL.Services.LogParsing.Interfaces;
using LorePdks.COMMON.DTO.LogParser;
using LorePdks.COMMON.DTO.Base;
using LorePdks.COMMON.Models.ServiceResponse;
using LorePdks.COMMON.Enums;
using LorePdks.COMMON.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace LorePdks.API.Controllers.LogParser
{
    [Route("api/LogParser")]
    [ApiController]
    public class LogParserController : ControllerBase
    {
        private readonly ILogParserService _logParserService;

        public LogParserController(ILogParserService logParserService)
        {
            _logParserService = logParserService;
        }

        /// <summary>
        /// Log parser konfigürasyonu oluştur veya güncelle
        /// </summary>
        [HttpPost]
        [Route("saveLogParserByLogParserDto")]
        public IActionResult saveLogParserByLogParserDto(LogParserDTO request)
        {
            var response = new ServiceResponse<LogParserDTO>();
            try
            {
                var result = _logParserService.saveLogParser(request);
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
        /// ID'ye göre log parser konfigürasyonu getir
        /// </summary>
        [HttpPost]
        [Route("getLogParserDtoByEIdDto")]
        public IActionResult getLogParserDtoByEIdDto(EIdDTO request)
        {
            var response = new ServiceResponse<LogParserDTO>();
            try
            {
                var result = _logParserService.getLogParserById(request.id);
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
        /// Firmaya göre log parser konfigürasyonları listele
        /// </summary>
        [HttpPost]
        [Route("getLogParserListByFirmaEIdDto")]
        public IActionResult getLogParserListByFirmaEIdDto(EIdDTO request)
        {
            var response = new ServiceResponse<List<LogParserDTO>>();
            try
            {
                var result = _logParserService.getLogParserListByFirmaId(request.id);
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
        /// Tüm log parser konfigürasyonlarını listele
        /// </summary>
        [HttpPost]
        [Route("getAllLogParserList")]
        public IActionResult getAllLogParserList()
        {
            var response = new ServiceResponse<List<LogParserDTO>>();
            try
            {
                var result = _logParserService.getAllLogParsers();
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
        /// Log verisini parse et
        /// </summary>
        [HttpPost]
        [Route("parseLogData")]
        public IActionResult parseLogData(LogParseRequestDTO request)
        {
            var response = new ServiceResponse<Dictionary<string, object>>();
            try
            {
                var result = _logParserService.parseLogData(request.rawLogData, request.configId);
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
        /// Log parser konfigürasyonunu test et
        /// </summary>
        [HttpPost]
        [Route("testLogParserConfig")]
        public IActionResult testLogParserConfig(LogParserTestRequestDTO request)
        {
            var response = new ServiceResponse<Dictionary<string, object>>();
            try
            {
                var result = _logParserService.testLogParserConfig(request.sampleLogData, request.config);
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
        /// Log parser konfigürasyonunu sil
        /// </summary>
        [HttpPost]
        [Route("deleteLogParserByEIdDto")]
        public IActionResult deleteLogParserByEIdDto(EIdDTO request)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                _logParserService.deleteLogParser(request.id);
                response.messageType = ServiceResponseMessageType.Success;
                response.message = "Log parser konfigürasyonu başarıyla silindi";
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
