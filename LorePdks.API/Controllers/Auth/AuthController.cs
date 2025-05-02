using LorePdks.API.Filters;
using LorePdks.BAL.Managers.Auth.Interfaces;
using LorePdks.BAL.Managers.Helper.Interfaces;
using LorePdks.BAL.Managers.KisiToken.Interfaces;
using LorePdks.COMMON.DTO.Auth;
using LorePdks.COMMON.Enums;
using LorePdks.COMMON.Models;
using LorePdks.COMMON.Models.ServiceResponse;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LorePdks.API.Controllers.Auth
{
    [Route("api/Auth")]
    [ApiController]
    public class AuthController(IAuthManager _authManager, IKisiTokenManager _kisiTokenManager,IHelperManager _helperManager) : ControllerBase
    {



        /// <summary>
        /// Kullanıcı girişi yapar ve token döner
        /// </summary>
        [HttpPost("login")]
        [DirectAccess] // Güvenlik filtresinden muaf olması için
        public IActionResult Login(LoginRequestDTO request)
        {
            var response = new ServiceResponse<object>();
          
            response.data = _authManager.login(request.loginName, request.sifre); ;
            return Ok(response);
    
        }

        [HttpPost("getRolDtoListByKisiId")]
        public IActionResult getRolDtoListByKisiId()
        {
            var response = new ServiceResponse<object>();
            //menü için rol ve ekranlarıda dtoya ekle
            var rolMenuDto = _rolManager.getRolDtoListByKisiId(resultTokenDto.kisiDto.id);

            response.data = _authManager.login(request.loginName, request.sifre); ;
            return Ok(response);

        }

        


        /// <summary>
        /// Kullanıcı çıkışı yapar (token'ı geçersiz kılar)
        /// </summary>
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            _authManager.logout(_helperManager.GetToken());

            var response = new ServiceResponse<object>();
            response.message = "Başarıyla çıkış yapıldı";
            response.IsSuccess = true;
            return Ok(response);

        }

        ///// <summary>
        ///// Kullanıcının tüm token'larını geçersiz kılar
        ///// </summary>
        //[HttpPost("logout-all")]
        //public IActionResult LogoutAll([FromBody] LogoutAllRequestDTO request)
        //{
        //    try
        //    {
        //        _authManager.logoutAll(request.kisiId);
        //        return Ok(new { success = true, message = "Tüm oturumlardan başarıyla çıkış yapıldı" });
        //    }
        //    catch (AppException ex)
        //    {
        //        throw;
        //        //throw new AppException(MessageCode.ERROR_401_YETKISIZ_ERISIM, "Kullanıcı adı veya şifre hatalı");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new AppException(MessageCode.ERROR_401_YETKISIZ_ERISIM, $"Giriş sırasında bir hata oluştu: {ex.Message}");

        //    }
        //}
        

    }

  


}