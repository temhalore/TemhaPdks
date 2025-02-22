using Microsoft.AspNetCore.Mvc;
using LorePdks.BAL.Managers.Security.Interfaces;
using LorePdks.COMMON.DTO.Security.Role;
using LorePdks.COMMON.Models;
using LorePdks.COMMON.Models.ServiceResponse;

namespace LorePdks.API.Controllers.Security
{
    [Route("Api/Security/Role")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleManager _roleManager;
        private ServiceResponse<object> response = new ServiceResponse<object>();

        public RoleController(IRoleManager roleManager)
        {
            _roleManager = roleManager;
        }

        [HttpPost]
        [Route("GetList")]
        public IActionResult GetList()
        {
            try
            {
                var data = _roleManager.GetList();
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
        [Route("Get")]
        public IActionResult Get(RoleDTO request)
        {
            try
            {
                var data = _roleManager.Get(request.id);
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
        [Route("Add")]
        public IActionResult Add(RoleDTO request)
        {
            try
            {
                _roleManager.Add(request);
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
        public IActionResult Set(RoleDTO request)
        {
            try
            {
                _roleManager.Set(request);
                response.messageType = "success";
                response.message = "Role başarıyla güncellendi";
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
        [Route("Del")]
        public IActionResult Del(RoleDTO request)
        {
            try
            {
                _roleManager.Del(request);
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