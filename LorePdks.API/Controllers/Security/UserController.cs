using Microsoft.AspNetCore.Mvc;
using LorePdks.BAL.Managers.Security.Interfaces;
using LorePdks.COMMON.Models;
using LorePdks.COMMON.DTO.Services.DataTable;
using LorePdks.COMMON.DTO.Base;
using LorePdks.COMMON.DTO.Security.User;
using LorePdks.COMMON.Models.ServiceResponse;

namespace LorePdks.API.Controllers.Security
{
    [Route("Api/Security/User")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserManager _userManager;
        private ServiceResponse<object> response = new ServiceResponse<object>();

        public UserController(IUserManager userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        [Route("GetList")]
        public IActionResult GetDataTableList(DataTableRequestDTO<UserDTO> request)
        {
            try
            {
                var data = _userManager.GetDataTableList(request);
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
        [Route("GetUserList")]
        public IActionResult GetUserList()
        {
            try
            {
                var data = _userManager.GetUserList();
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
        [Route("GetUserListBySearchValue")]
        public IActionResult GetUserListBySearchValue(SingleValueDTO<string> request)
        {
            try
            {
                var data = _userManager.GetUserListBySearchValue(request.Value);
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

        //[HttpPost]
        //[Route("SetUserLanguage")]
        //public IActionResult SetUserLanguage(UserDTO request)
        //{
        //    try
        //    {
        //        var data = _userManager.SetUserLanguage(request);
        //        response.data = data;
        //        return Ok(response);
        //    }
        //    catch (AppException appEx)
        //    {
        //        response = new ServiceResponse<object>(appEx);
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        response.messageType = "error";
        //        response.message = ex.Message;
        //        return BadRequest(response);
        //    }
        //}
        //[HttpPost]
        //[Route("SetUserImage")]
        //public IActionResult SetUserImage(UserDTO request)
        //{
        //    try
        //    {
        //       var data = _userManager.SetUserImage(request);
        //        response.Data = data;
        //        return Ok(response);
        //    }
        //    catch (AppException appEx)
        //    {
        //        response = new ServiceResponse<object>(appEx);
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        response.messageType = "error";
        //        response.message = ex.Message;
        //        return BadRequest(response);
        //    }
        //}
        [HttpPost]
        [Route("Set")]
        public IActionResult Set(UserDTO request)
        {
            try
            {
                var data = _userManager.Set(request);
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

        //#region List
        //[Area("Admin")]
        //[Route("{area}/user/list")]
        //public IActionResult List()
        //{
        //    List<UserDTO> userList = _userManager.GetUserList();
        //    return View(userList);
        //}
        //#endregion
        //#region Add
        //[Area("Admin")]
        //[Route("{area}/user/add")]
        //public IActionResult Add()
        //{
        //    return View();
        //}
        //[HttpPost]
        //[Area("Admin")]
        //[Route("{area}/user/add")]
        //public IActionResult Add(UserAddRequestDTO request)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View();
        //    }

        //    List<string> errorList = _userManager.Add(request);
        //    if (errorList.Any())
        //    {
        //        foreach (string eror in errorList)
        //        {
        //            ModelState.AddModelError("error", eror);
        //        }
        //        return View();
        //    }
        //    else
        //    {
        //        //Burada User'ı login edebilriiz'
        //        return RedirectToAction("List", "User");
        //    }

        //}
        //[Area("Admin")]
        //[Route("{area}/user/set/{userid:int}")]
        //#endregion
        //#region Set
        //public IActionResult Set(int userid)
        //{
        //    UserSetRequestDTO user = _userManager.GetUserForSet(userid);
        //    return View(user);
        //}
        //[HttpPost]
        //[Area("Admin")]
        //[Route("{area}/user/set/{userid:int}")]
        //public IActionResult Set(UserSetRequestDTO request)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View();
        //    }
        //    List<string> errorList = _userManager.Set(request);
        //    UserSetRequestDTO user = _userManager.GetUserForSet(request.Id);
        //    return View(user);
        //}
        //#endregion
        //#region Del
        //[Area("Admin")]
        //[Route("{area}/user/del/{userid:int}")]
        //public IActionResult Del(int userid)
        //{
        //    UserDTO user = _userManager.GetUser(userid);
        //    return View(user);
        //}

        //[HttpPost]
        //[Area("Admin")]
        //[Route("{area}/user/del/{userid:int}")]
        //public IActionResult Del(UserDTO user)
        //{
        //    List<string> errorList = _userManager.Del(user.Id);
        //    if (errorList.Any())
        //    {
        //        foreach (string eror in errorList)
        //        {
        //            ModelState.AddModelError("error", eror);
        //        }
        //        return RedirectToAction("Del", "User");
        //    }
        //    else
        //    {
        //        return RedirectToAction("List", "User");
        //    }
        //}
        //#endregion
    }
}