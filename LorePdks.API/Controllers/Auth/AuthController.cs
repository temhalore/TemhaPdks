using LorePdks.API.Filters;
using LorePdks.BAL.Managers.Auth.Interfaces;
using LorePdks.COMMON.DTO.Common;
using LorePdks.COMMON.Enums;
using LorePdks.COMMON.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace LorePdks.API.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthManager _authManager;

        public AuthController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        /// <summary>
        /// Kullanıcı girişi yapar ve token döner
        /// </summary>
        [HttpPost("login")]
        [DirectAccess] // Güvenlik filtresinden muaf olması için
        public IActionResult Login([FromBody] LoginRequestDTO request)
        {
            try
            {
                // IP adresi ve User-Agent bilgisini al
                string ipAdresi = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
                string userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

                // Login işlemini gerçekleştir
                var result = _authManager.Login(request.loginName, request.sifre, ipAdresi, userAgent);

                // Başarılı login
                return Ok(new 
                { 
                    success = true, 
                    token = result.token,
                    kisiId = result.kisiId,
                    loginName = result.loginName,
                    expDate = result.expDate
                });
            }
            catch (AppException ex)
            {
                throw;
                //throw new AppException(MessageCode.ERROR_401_YETKISIZ_ERISIM, "Kullanıcı adı veya şifre hatalı");
            }
            catch (Exception ex)
            {
                throw new AppException(MessageCode.ERROR_401_YETKISIZ_ERISIM, $"Giriş sırasında bir hata oluştu: {ex.Message}");
       
            }
        }

        /// <summary>
        /// Token doğrular
        /// </summary>
        [HttpPost("validate-token")]
        [DirectAccess] // Güvenlik filtresinden muaf olması için
        public IActionResult ValidateToken([FromBody] TokenValidateRequestDTO request)
        {
            try
            {
                var result = _authManager.ValidateToken(request.token);

                return Ok(new 
                { 
                    success = true, 
                    message = "Token geçerli",
                    kisiId = result.kisiId,
                    kisi = result.kisiDto
                });
            }
            catch (AppException ex)
            {
                throw;
                //throw new AppException(MessageCode.ERROR_401_YETKISIZ_ERISIM, "Kullanıcı adı veya şifre hatalı");
            }
            catch (Exception ex)
            {
                throw new AppException(MessageCode.ERROR_401_YETKISIZ_ERISIM, $"Giriş sırasında bir hata oluştu: {ex.Message}");

            }
        }
        /// <summary>
        /// Kullanıcı çıkışı yapar (token'ı geçersiz kılar)
        /// </summary>
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            try
            {
                // Token başlıktan al
                string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest(new { success = false, message = "Token bulunamadı" });
                }

                _authManager.Logout(token);
                return Ok(new { success = true, message = "Başarıyla çıkış yapıldı" });
            }
            catch (AppException ex)
            {
                throw;
                //throw new AppException(MessageCode.ERROR_401_YETKISIZ_ERISIM, "Kullanıcı adı veya şifre hatalı");
            }
            catch (Exception ex)
            {
                throw new AppException(MessageCode.ERROR_401_YETKISIZ_ERISIM, $"Giriş sırasında bir hata oluştu: {ex.Message}");

            }
        }

        /// <summary>
        /// Kullanıcının tüm token'larını geçersiz kılar
        /// </summary>
        [HttpPost("logout-all")]
        public IActionResult LogoutAll([FromBody] LogoutAllRequestDTO request)
        {
            try
            {
                _authManager.LogoutAll(request.kisiId);
                return Ok(new { success = true, message = "Tüm oturumlardan başarıyla çıkış yapıldı" });
            }
            catch (AppException ex)
            {
                throw;
                //throw new AppException(MessageCode.ERROR_401_YETKISIZ_ERISIM, "Kullanıcı adı veya şifre hatalı");
            }
            catch (Exception ex)
            {
                throw new AppException(MessageCode.ERROR_401_YETKISIZ_ERISIM, $"Giriş sırasında bir hata oluştu: {ex.Message}");

            }
        }
    }

    public class TokenValidateRequestDTO
    {
        public string token { get; set; }
    }

    public class LogoutAllRequestDTO
    {
        public int kisiId { get; set; }
    }
}