using LorePdks.BAL.Managers.User;
using LorePdks.COMMON.DTO.Base;
using LorePdks.COMMON.DTO.Common;
using LorePdks.COMMON.Enums;
using LorePdks.COMMON.Models.ServiceResponse;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace LorePdks.API.Controllers.User
{
    [Route("Api/User")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserManager _userManager;

        public UserController(IUserManager userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        [Route("getUserDtoByEIdDto")]
        public IActionResult getUserDtoByEIdDto(EIdDTO request)
        {
            var response = new ServiceResponse<UserDTO>();
            var dto = _userManager.getUserDtoById(request.id);
            response.data = dto;
            return Ok(response);
        }

        [HttpPost]
        [Route("getAllUserListDto")]
        public IActionResult getAllUserListDto()
        {
            var response = new ServiceResponse<List<UserDTO>>();
            var dto = _userManager.getUserDtoList();
            response.data = dto;
            return Ok(response);
        }

        [HttpPost]
        [Route("saveUserByUserDto")]
        public IActionResult saveUserByUserDto(UserDTO request)
        {
            var response = new ServiceResponse<UserDTO>();
            var dto = _userManager.saveUser(request);
            response.data = dto;
            return Ok(response);
        }

        [HttpPost]
        [Route("deleteUserByEidDto")]
        public IActionResult deleteUserByEidDto(EIdDTO request)
        {
            var response = new ServiceResponse<UserDTO>();
            _userManager.deleteUserByUserId(request.id);
            return Ok(response);
        }
    }
}