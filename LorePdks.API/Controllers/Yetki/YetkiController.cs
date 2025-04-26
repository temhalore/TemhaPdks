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
        IEkranManager _EkranManager,
        IRolManager _RolManager
        ) : ControllerBase
    {

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
    }
}