using Microsoft.AspNetCore.Mvc;
using LorePdks.BAL.Managers.Security.Interfaces;
using LorePdks.COMMON.DTO.Security.Role;
using LorePdks.COMMON.DTO.Security.RoleUser;
using LorePdks.COMMON.Models;
using LorePdks.COMMON.Models.ServiceResponse;

namespace LorePdks.API.Controllers.Security
{
    [Route("Api/Security/RoleUser")]
    [ApiController]
    public class RoleUserController : ControllerBase
    {
        private readonly IRoleUserManager _roleUserManager;
        private ServiceResponse<object> response = new ServiceResponse<object>();

        public RoleUserController(IRoleUserManager roleUserManager)
        {
            _roleUserManager = roleUserManager;
        }
        [HttpPost]
        [Route("GetList")]
        public IActionResult GetList(RoleDTO request)
        {
            try
            {
                var data = _roleUserManager.GetList(request);
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
        public IActionResult Set(RoleUserDTO request)
        {
            try
            {
                _roleUserManager.Set(request);
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