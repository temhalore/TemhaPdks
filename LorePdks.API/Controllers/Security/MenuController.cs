using Microsoft.AspNetCore.Mvc;
using LorePdks.BAL.Managers.Security.Interfaces;
using LorePdks.COMMON.DTO.Security.Menu;
using LorePdks.COMMON.DTO.Base;
using LorePdks.COMMON.Models;
using LorePdks.COMMON.Models.ServiceResponse;

namespace LorePdks.API.Controllers.Security
{
    [Route("Api/Security/Menu")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenuManager _menuManager;
        private ServiceResponse<object> response = new ServiceResponse<object>();
        public MenuController(IMenuManager menuManager)
        {
            _menuManager = menuManager;
        }
        //[HttpPost]
        //[Route("GetMenuListForAside")]
        //public IActionResult GetMenuListForAside(SingleValueDTO<long> request)
        //{
        //    try
        //    {
        //        var data = _menuManager.GetMenuListForAside(request.Value);
        //        response.Data = data;
        //        return Ok(response);
        //    }
        //    catch (KSPYSException kspx)
        //    {
        //        response = new KSPYSException().ResponseKSP(kspx);
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        response = new KSPYSException().ResponseEX(ex);
        //        return Ok(response);
        //    }
        //}
        [HttpPost]
        [Route("GetMenuTreeListForAdmin")]
        public IActionResult GetMenuTreeListForAdmin(SingleValueDTO<long> request)
        {
            try
            {
                var data = _menuManager.GetMenuTreeListForAdmin(request.Value);
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
                var data = _menuManager.GetList();
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
        public IActionResult Add(MenuDTO request)
        {
            try
            {
                _menuManager.Add(request);
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
        public IActionResult Set(MenuDTO request)
        {
            try
            {
                _menuManager.Set(request);
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
        public IActionResult Del(MenuDTO request)
        {
            try
            {
                _menuManager.Del(request);
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
        public IActionResult MoveUp(MenuDTO request)
        {
            try
            {
                _menuManager.MoveUp(request);
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
        public IActionResult MoveDown(MenuDTO request)
        {
            try
            {
                _menuManager.MoveDown(request);
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