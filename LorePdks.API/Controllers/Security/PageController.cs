using Microsoft.AspNetCore.Mvc;
using LorePdks.BAL.Managers.Security.Interfaces;
using LorePdks.COMMON.DTO.Security.Page;
using LorePdks.COMMON.Models;
using LorePdks.COMMON.Models.ServiceResponse;

namespace LorePdks.API.Controllers.Security
{
    [Route("Api/Security/Page")]
    [ApiController]
    public class PageController : ControllerBase
    {
        private readonly IPageManager _pageManager;
        private ServiceResponse<object> response = new ServiceResponse<object>();

        public PageController(IPageManager pageManager)
        {
            _pageManager = pageManager;
        }

        [HttpPost]
        [Route("Get")]
        public IActionResult Get(PageDTO request)
        {
            try
            {
                var data = _pageManager.Get(request.id);
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
        public IActionResult GetList()
        {
            try
            {
                var data = _pageManager.GetList();
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
        public IActionResult Add(PageDTO request)
        {
            try
            {
                _pageManager.Add(request);
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
        public IActionResult Set(PageDTO request)
        {
            try
            {
                _pageManager.Set(request);
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
        public IActionResult Del(PageDTO request)
        {
            try
            {
                _pageManager.Del(request);
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
        [Route("MoveUp")]
        public IActionResult MoveUp(PageDTO request)
        {
            try
            {
                _pageManager.MoveUp(request);
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
        [Route("MoveDown")]
        public IActionResult MoveDown(PageDTO request)
        {
            try
            {
                _pageManager.MoveDown(request);
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