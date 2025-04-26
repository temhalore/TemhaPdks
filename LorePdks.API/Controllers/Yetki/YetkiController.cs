using LorePdks.API.Filters;
using LorePdks.BAL.Managers.Auth.Interfaces;
using LorePdks.BAL.Managers.Helper.Interfaces;
using LorePdks.BAL.Managers.KisiToken.Interfaces;
using LorePdks.BAL.Managers.Yetki.Ekran;
using LorePdks.BAL.Managers.Yetki.Ekran.Interfaces;
using LorePdks.BAL.Managers.Yetki.Rol.Interfaces;
using LorePdks.COMMON.DTO.Auth;
using LorePdks.COMMON.DTO.Base;
using LorePdks.COMMON.DTO.Firma;
using LorePdks.COMMON.DTO.Yetki;
using LorePdks.COMMON.DTO.Yetki.Ekran;
using LorePdks.COMMON.DTO.Yetki.Rol;
using LorePdks.COMMON.Enums;
using LorePdks.COMMON.Models;
using LorePdks.COMMON.Models.ServiceResponse;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace LorePdks.API.Controllers.Yetki
{
    [Route("api/Yetki")]
    [ApiController]
    public class YetkiController(
        IHelperManager _helperManager,
        IEkranManager _ekranManager,
        IRolManager _rolManager
        ) : ControllerBase
    {
        #region Controller-Method Listeleme

        [HttpPost]
        [Route("GetControllerAndMethodsList")]
        public IActionResult GetControllerAndMethodsList()
        {
            var response = new ServiceResponse<List<ControllerAndMethodsDTO>>();

            var assembly = Assembly.GetExecutingAssembly();

            var dto = assembly.GetTypes()
                    .Where(a => a.BaseType == typeof(ControllerBase))
                    .Select(a => new ControllerAndMethodsDTO()
                    {
                        controllerName = a.Name,
                        methods = a.GetMethods()
                            .Where(x => x.IsPublic && x.DeclaringType == a)
                            .Select(m => m.Name)
                            .ToList()
                    })
                    .ToList();

            response.data = dto;
            return Ok(response);
        }

        #endregion

        #region Rol Yönetimi

        [HttpPost]
        [Route("getAllRolList")]
        public IActionResult getAllRolList()
        {
            var response = new ServiceResponse<List<RolDTO>>();
            var dto = _rolManager.getRolDtoList();
            response.data = dto;
            return Ok(response);
        }

        [HttpPost]
        [Route("getRolDtoByEIdDto")]
        public IActionResult getRolDtoByEIdDto(EIdDTO request)
        {
            var response = new ServiceResponse<RolDTO>();
            var dto = _rolManager.getRolDtoById(request.id);
            response.data = dto;
            return Ok(response);
        }

        [HttpPost]
        [Route("saveRolByRolDto")]
        public IActionResult saveRolByRolDto(RolDTO request)
        {
            var response = new ServiceResponse<RolDTO>();
            var dto = _rolManager.saveRol(request);
            response.data = dto;
            return Ok(response);
        }

        [HttpPost]
        [Route("deleteRolByEIdDto")]
        public IActionResult deleteRolByEIdDto(EIdDTO request)
        {
            var response = new ServiceResponse<bool>();
            _rolManager.deleteRolByRolId(request.id);
            response.data = true;
            return Ok(response);
        }

        #endregion

        #region Controller-Method Yetkilendirme

        [HttpPost]
        [Route("getRolControllerMethodsByEIdDto")]
        public IActionResult getRolControllerMethodsByEIdDto(EIdDTO request)
        {
            var response = new ServiceResponse<List<RolControllerMethodDTO>>();
            var dto = _rolManager.getRolControllerMethodDtoListByRolId(request.id);
            response.data = dto;
            return Ok(response);
        }

        [HttpPost]
        [Route("saveRolControllerMethodsByRolIdAndControllerMethods")]
        public IActionResult saveRolControllerMethodsByRolIdAndControllerMethods(RolControllerMethodsRequestDTO request)
        {
            var response = new ServiceResponse<bool>();
            var success = _rolManager.saveRolControllerMethods(request.rolId, request.controllerMethods);
            response.data = success;
            return Ok(response);
        }

        #endregion

        #region Ekran-Rol İlişkilendirme

        [HttpPost]
        [Route("getAllEkranList")]
        public IActionResult getAllEkranList()
        {
            var response = new ServiceResponse<List<EkranDTO>>();
            var dto = _ekranManager.getEkranDtoList();
            response.data = dto;
            return Ok(response);
        }

        [HttpPost]
        [Route("getEkransByRolIdDto")]
        public IActionResult getEkransByRolIdDto(EIdDTO request)
        {
            var response = new ServiceResponse<List<EkranDTO>>();
            var dto = _ekranManager.getEkranDtoListByRolId(request.id);
            response.data = dto;
            return Ok(response);
        }

        [HttpPost]
        [Route("addEkranToRolByRolEkranDto")]
        public IActionResult addEkranToRolByRolEkranDto(RolEkranDTO request)
        {
            var response = new ServiceResponse<bool>();
            var success = _rolManager.addEkranToRol(request.rolId, request.ekranId);
            response.data = success;
            return Ok(response);
        }

        [HttpPost]
        [Route("removeEkranFromRolByRolEkranDto")]
        public IActionResult removeEkranFromRolByRolEkranDto(RolEkranDTO request)
        {
            var response = new ServiceResponse<bool>();
            var success = _rolManager.removeEkranFromRol(request.rolId, request.ekranId);
            response.data = success;
            return Ok(response);
        }

        #endregion
    }
}