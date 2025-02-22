using Microsoft.AspNetCore.Mvc;
using LorePdks.BAL.Managers.Security.Interfaces;
using LorePdks.COMMON.DTO.Security.Auth;
using LorePdks.COMMON.Models;
using LorePdks.COMMON.Models.ServiceResponse;
using LorePdks.API.Filters;

namespace LorePdks.API.Controllers.Security
{
    [Route("Api/Security/Auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthManager _authManager;

        public AuthController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpPost]
        [Route("LoginWithToken")]
        [DirectAccess]
        public IActionResult LoginWithToken(LoginWithSSORequestDTO request)
        {


            var response = new ServiceResponse<object>();
            try
            {
                var data = _authManager.LoginWithToken(request);
                response.data = data;
                response.message = "Login Bilgisi Dönüldü";
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
