using Microsoft.AspNetCore.Mvc;
using LorePdks.BAL.Managers.Security.Interfaces;
using LorePdks.COMMON.DTO.Security.Role;
using LorePdks.COMMON.DTO.Security.RoleWidget;
using LorePdks.COMMON.Models;
using LorePdks.COMMON.Models.ServiceResponse;

namespace LorePdks.API.Controllers.Security
{
    [Route("Api/Security/RoleWidget")]
    [ApiController]
    public class RoleWidgetController : ControllerBase
    {
        private readonly IRoleWidgetManager _roleWidgetManager;
        private ServiceResponse<object> response = new ServiceResponse<object>();

        public RoleWidgetController(IRoleWidgetManager roleWidgetManager)
        {
            _roleWidgetManager = roleWidgetManager;
        }



        [HttpPost]
        [Route("GetRoleWidgetTreeListForAdmin")]
        public IActionResult GetRoleWidgetTreeListForAdmin(RoleDTO request)
        {
            try
            {
                var data = _roleWidgetManager.GetRoleWidgetTreeListForAdmin(request);
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
        [Route("GetList")]
        public IActionResult GetList(RoleDTO request)
        {
            try
            {
                var data = _roleWidgetManager.GetList(request);
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
        [Route("Set")]
        public IActionResult Set(RoleWidgetDTO request)
        {
            try
            {
                _roleWidgetManager.Set(request);
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