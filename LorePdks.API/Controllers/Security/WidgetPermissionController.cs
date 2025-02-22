using Microsoft.AspNetCore.Mvc;
using LorePdks.BAL.Managers.Security.Interfaces;
using LorePdks.COMMON.DTO.Security.Widget;
using LorePdks.COMMON.DTO.Security.WidgetPermission;
using LorePdks.COMMON.Models;
using LorePdks.COMMON.Models.ServiceResponse;


namespace LorePdks.API.Controllers.Security
{
    [Route("Api/Security/WidgetPermission")]
    [ApiController]
    public class WidgetPermissionController : ControllerBase
    {
        private readonly IWidgetPermissionManager _widgetPermissionManager;
        private ServiceResponse<object> response = new ServiceResponse<object>();

        public WidgetPermissionController(IWidgetPermissionManager widgetPermissionManager)
        {
            _widgetPermissionManager = widgetPermissionManager;
        }
        [HttpPost]
        [Route("GetList")]
        public IActionResult GetList(WidgetDTO request)
        {
            try
            {
                var data = _widgetPermissionManager.GetList(request);
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
        public IActionResult Set(WidgetPermissionDTO request)
        {
            try
            {
                _widgetPermissionManager.Set(request);
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