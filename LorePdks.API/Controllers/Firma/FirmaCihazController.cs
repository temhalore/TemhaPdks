﻿using Microsoft.AspNetCore.Mvc;
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
    [Route("Api/FirmaCihaz")]
    [ApiController]
    public class FirmaCihazController(ILogger<FirmaCihazController> _logger, IFirmaCihazManager _firmaCihazManager) : ControllerBase
    {
     

      

        [HttpPost]
        [Route("saveFirmaCihazByFirmaCihazDto")]
        public IActionResult saveFirmaCihazByFirmaCihazDto(FirmaCihazDTO request)
        {

            var response = new ServiceResponse<object>();
            var dto = _firmaCihazManager.saveFirmaCihaz(request);
            response.data = dto;
            return Ok(response);
        }

        [HttpPost]
        [Route("deleteFirmaCihazByEidDto")]
        public IActionResult deleteFirmaCihazByEidDto(EIdDTO request)
        {

            var response = new ServiceResponse<FirmaDTO>();
            _firmaCihazManager.deleteFirmaCihazByFirmaCihazId(request.id);
            
            return Ok(response);
        }

        [HttpPost]
        [Route("getFirmaCihazDtoByEIdDto")]
        public IActionResult getFirmaCihazDtoByEIdDto(EIdDTO request)
        {

            var response = new ServiceResponse<object>();
            var dto = _firmaCihazManager.getFirmaCihazDtoById(request.id);
            response.data = dto;
            return Ok(response);
        }

        [HttpPost]
        [Route("getFirmaCihazDtoListByFirmaDto")]
        public IActionResult getFirmaCihazDtoListByFirmaDto(FirmaDTO request)
        {

            var response = new ServiceResponse<object>();
            var dto = _firmaCihazManager.getFirmaCihazDtoListByFirmaId(request.id);
            response.data = dto;
            return Ok(response);
        }

    }
}
