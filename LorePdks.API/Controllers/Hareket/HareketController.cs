﻿using Microsoft.AspNetCore.Mvc;
using LorePdks.COMMON.DTO.Base;
using LorePdks.COMMON.Models.ServiceResponse;
using LorePdks.COMMON.DTO.Hareket;
using LorePdks.COMMON.DTO.Firma;
using LorePdks.BAL.Managers.Hareket.Interfaces;

namespace LorePdks.API.Controllers.Hareket
{
    /// <summary>
    /// firmalardan toplanan tüm hareket çağrıları buraya düşecek ilk
    /// </summary>
    [Route("Api/Hareket")]
    [ApiController]
    public class HareketController(ILogger<HareketController> _logger, IHareketManager _hareketManager) : ControllerBase
    {
        //dışarıdan okuma programımız bu metodu çağıracak
        [HttpPost]
        [Route("saveHareketByHareketDto")]
        public IActionResult saveHareketByHareketDto(HareketDTO hareketDto)
        {
            var response = new ServiceResponse<HareketDTO>();
            var dto = _hareketManager.saveHareket(hareketDto);
            response.data = dto;
            return Ok(response);
        }

        [HttpPost]
        [Route("getHareketListByFirmaDto")]
        public IActionResult getHareketListByFirmaDto(FirmaDTO firmaDto)
        {
            var response = new ServiceResponse<List<HareketDTO>>();
            var dto = _hareketManager.getHareketListByFirmaId(firmaDto.id);
            response.data = dto;
            return Ok(response);
        }
        
        [HttpPost]
        [Route("getHareketDtoByEIdDto")]
        public IActionResult getHareketDtoByEIdDto(EIdDTO request)
        {
            var response = new ServiceResponse<HareketDTO>();
            var dto = _hareketManager.getHareketDtoById(request.id);
            response.data = dto;
            return Ok(response);
        }
        
        [HttpPost]
        [Route("deleteHareketByEIdDto")]
        public IActionResult deleteHareketByEIdDto(EIdDTO request)
        {
            var response = new ServiceResponse<HareketDTO>();
            _hareketManager.deleteHareketById(request.id);
            return Ok(response);
        }
    }
}
