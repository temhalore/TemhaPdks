using Microsoft.AspNetCore.Mvc;
using LorePdks.BAL.Managers.Security.Interfaces;
using LorePdks.COMMON.DTO.Security.Page;
using LorePdks.COMMON.DTO.Security.Widget;
using LorePdks.COMMON.Models;
using LorePdks.COMMON.Models.ServiceResponse;

namespace LorePdks.API.Controllers.Security
{
    [Route("Api/Security/Widget")]
    [ApiController]
    public class WidgetController : ControllerBase
    {
        private readonly IWidgetManager _widgetManager;
        private ServiceResponse<object> response = new ServiceResponse<object>();

        public WidgetController(IWidgetManager widgetManager)
        {
            _widgetManager = widgetManager;
        }
        [HttpPost]
        [Route("Get")]
        public IActionResult Get(WidgetDTO request)
        {
            try
            {
                var data = _widgetManager.Get(request.id);
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
        [Route("GetListByPageDto")]
        public IActionResult GetListByPageDto(PageDTO request)
        {
            try
            {
                var data = _widgetManager.GetListByPageDto(request);
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
        public IActionResult Add(WidgetDTO request)
        {
            try
            {
                _widgetManager.Add(request);
                response.messageType = "success";
                response.message = "Başarıyla Eklendi";
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
        public IActionResult Set(WidgetDTO request)
        {
            try
            {
                _widgetManager.Set(request);
                response.messageType = "success";
                response.message = "Başarıyla Güncellendi";
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
        public IActionResult Del(WidgetDTO request)
        {
            try
            {
                _widgetManager.Del(request);
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