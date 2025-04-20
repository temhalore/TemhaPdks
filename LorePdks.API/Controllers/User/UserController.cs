using LorePdks.BAL.Managers.User;
using LorePdks.COMMON.DTO.Common;
using LorePdks.COMMON.Enums;
using LorePdks.COMMON.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LorePdks.API.Controllers.User
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserManager _userManager;

        public UserController(IUserManager userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("{id}")]
        public ActionResult GetUserById(int id)
        {
            try
            {
                var user = _userManager.getUserDtoById(id);
                if (user == null)
                {
                    return NotFound(new ResponseDTO() { IsSuccess = false, ResultCode = ((int)MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI).ToString() });
                }

                return Ok(new ResponseDTO<UserDTO>() { IsSuccess = true, Data = user });
            }
            catch (AppException ex)
            {
                return BadRequest(new ResponseDTO() { IsSuccess = false, ResultMessage = ex.Message, ResultCode = ex.ResultCode });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDTO() { IsSuccess = false, ResultMessage = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult GetAllUsers()
        {
            try
            {
                var users = _userManager.getUserDtoList();
                return Ok(new ResponseDTO<UserDTO>() { IsSuccess = true, DataList = users.ToList() });
            }
            catch (AppException ex)
            {
                return BadRequest(new ResponseDTO() { IsSuccess = false, ResultMessage = ex.Message, ResultCode = ex.ResultCode });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDTO() { IsSuccess = false, ResultMessage = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult SaveUser([FromBody] UserDTO userDto)
        {
            try
            {
                var savedUser = _userManager.saveUser(userDto);
                return Ok(new ResponseDTO<UserDTO>() { IsSuccess = true, Data = savedUser });
            }
            catch (AppException ex)
            {
                return BadRequest(new ResponseDTO() { IsSuccess = false, ResultMessage = ex.Message, ResultCode = ex.ResultCode });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDTO() { IsSuccess = false, ResultMessage = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteUser(int id)
        {
            try
            {
                _userManager.deleteUserByUserId(id);
                return Ok(new ResponseDTO() { IsSuccess = true });
            }
            catch (AppException ex)
            {
                return BadRequest(new ResponseDTO() { IsSuccess = false, ResultMessage = ex.Message, ResultCode = ex.ResultCode });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseDTO() { IsSuccess = false, ResultMessage = ex.Message });
            }
        }
    }
}