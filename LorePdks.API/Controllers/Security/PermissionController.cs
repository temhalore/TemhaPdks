using Microsoft.AspNetCore.Mvc;
using LorePdks.BAL.Managers.Security.Interfaces;
using LorePdks.COMMON.DTO.Security.Permission;
using LorePdks.COMMON.Models;
using LorePdks.COMMON.Models.ServiceResponse;
using LorePdks.API.Filters;
using System.Reflection;

namespace LorePdks.API.Controllers.Security
{
    [Route("Api/Security/Permission")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionManager _permissionManager;
        private ServiceResponse<object> response = new ServiceResponse<object>();

        public PermissionController(IPermissionManager permissionManager)
        {
            _permissionManager = permissionManager;
        }
        [HttpPost]
        [Route("GetList")]
        public IActionResult GetList()
        {
            try
            {
                var data = _permissionManager.GetList();
                response.data = data;
                return Ok(response);
            }
            catch (AppException appEx)
            {
                response = new ServiceResponse<object>(appEx);
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.messageType = "error";
                response.message = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPost]
        [DirectAccess]
        [Route("Check")]
        public IActionResult Check()
        {
            try
            {
                List<PermissionDTO> permissionListDto = new List<PermissionDTO>();

                Assembly asm = Assembly.GetExecutingAssembly();
                var ApiControllers = asm.GetTypes()
                    .Where(type => typeof(ControllerBase).IsAssignableFrom(type))
                    .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
                    .Where(m => !m.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), true).Any())
                    .Select(m => new { Areas = m.DeclaringType.Namespace.Split('.').Reverse().First(), Controller = m.DeclaringType.Name.Replace("Controller", ""), Action = m.Name, ReturnType = m.ReturnType.Name, ReturnParameter = m.ReturnParameter.Name, Attributes = string.Join(",", m.GetCustomAttributes().Select(a => a.GetType().Name.Replace("Attribute", ""))) })
                    .OrderBy(m => m.Areas).ThenBy(m => m.Controller).ThenBy(m => m.Action).ToList();


                foreach (var item in ApiControllers)
                {
                    PermissionDTO permissionDto = new PermissionDTO()
                    {
                        type = "Api",
                        area = item.Areas,
                        controller = item.Controller,
                        action = item.Action,
                        returnType = item.ReturnType
                    };
                    permissionListDto.Add(permissionDto);
                }

                _permissionManager.Check(permissionListDto);
                return Ok(response);
            }
            catch (AppException appEx)
            {
                response = new ServiceResponse<object>(appEx);
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.messageType = "error";
                response.message = ex.Message;
                return BadRequest(response);
            }
        }
    }
}