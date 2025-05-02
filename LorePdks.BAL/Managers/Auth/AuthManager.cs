using AutoMapper;
using LorePdks.BAL.Managers.Auth.Interfaces;
using LorePdks.BAL.Managers.Helper.Interfaces;
using LorePdks.BAL.Managers.Kisi.Interfaces;
using LorePdks.BAL.Managers.KisiToken.Interfaces;
using LorePdks.BAL.Managers.Yetki.Rol.Interfaces;
using LorePdks.COMMON.DTO.Common;
using LorePdks.COMMON.Enums;
using LorePdks.COMMON.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LorePdks.BAL.Managers.Auth
{
    public class AuthManager(
        IKisiManager _kisiManager,
        IHelperManager _HelperManager,
        IMapper _mapper,
        IKisiTokenManager _kisiTokenManager,
         IRolManager _rolManager
    ) : IAuthManager
    {
        /// <summary>
        /// Kişi giriş işlemini gerçekleştirir
        /// </summary>
        public KisiTokenDTO login(string loginName, string sifre)
        {
            try
            {
                // Kullanıcı adı ve şifre kontrolü
                var kisiDto = _kisiManager.getKisiDtoByLoginNameAndSifre(loginName, sifre, false);

                if (kisiDto == null)
                {
                    throw new AppException(MessageCode.ERROR_401_YETKISIZ_ERISIM, "Kullanıcı adı veya şifre hatalı");
                }

                // Önce aktif olan bir token var mı kontrol et
                var aktifToken = _kisiTokenManager.getAktifKisiTokenByKisiId(kisiDto.id);
                
                // Aktif token varsa onu dön
                if (aktifToken != null)
                {
                    // IP adresi veya user agent değişmiş olabilir, güncelle
                    aktifToken.ipAdresi = _HelperManager.GetIPAddress();
                    aktifToken.userAgent = _HelperManager.GetUserAgent();
                    
                    // Token bilgilerini güncelle
                    var guncelToken = _kisiTokenManager.saveKisiToken(aktifToken);
                    guncelToken.isLogin = true;
                    
                    return guncelToken;
                }

                // Aktif token yoksa yeni token oluştur
                var kisiTokenDto = new KisiTokenDTO
                {
                    kisiId = kisiDto.id,
                    kisiDto = kisiDto,
                    loginName = loginName,
                    ipAdresi = _HelperManager.GetIPAddress(),
                    userAgent = _HelperManager.GetUserAgent(),
                    expDate = DateTime.Now.AddDays(1) // Token 1 gün geçerli
                };

                // Token kaydedilir
                var resultTokenDto = _kisiTokenManager.saveKisiToken(kisiTokenDto);
                resultTokenDto.isLogin = true;
                resultTokenDto.rolDtoList = _rolManager.getRolDtoListByKisiId(resultTokenDto.kisiDto.id);
                return resultTokenDto;
            }
            catch (AppException)
            {
                // Zaten uygun AppException fırlatıldı, tekrar fırlatıyoruz
                throw;
            }
            catch (Exception ex)
            {
                // Diğer hatalar için AppException oluşturup fırlatıyoruz
                throw new AppException(MessageCode.ERROR_500_BIR_HATA_OLUSTU, "Giriş sırasında beklenmeyen bir hata oluştu: " + ex.Message);
            }
        }

        /// <summary>
        /// Token kontrol eder
        /// </summary>
        public KisiTokenDTO validateToken(string token)
        {
            try
            {
                // Token geçerli mi kontrol et
                var kisiTokenDto = _kisiTokenManager.getKisiTokenDtoByToken(token, false);
                
                if (kisiTokenDto == null)
                {
                    // Geçersiz token durumunda token'ı öldür
                    _kisiTokenManager.killToken(token);
                    throw new AppException(MessageCode.ERROR_401_YETKISIZ_ERISIM, "Geçersiz token");
                }

                // Token süresi dolmuş mu kontrol et
                if (kisiTokenDto.expDate.HasValue && kisiTokenDto.expDate.Value < DateTime.Now)
                {
                    // Süresi dolmuş token'ı öldür
                    _kisiTokenManager.killToken(token);
                    throw new AppException(MessageCode.ERROR_401_YETKISIZ_ERISIM, "Token süresi dolmuş");
                }

                kisiTokenDto.isLogin = true;
                return kisiTokenDto;
            }
            catch (AppException)
            {
                // Zaten uygun AppException fırlatıldı, tekrar fırlatıyoruz
                throw;
            }
            catch (Exception ex)
            {
                // Diğer hatalar için AppException oluşturup fırlatıyoruz
                throw new AppException(MessageCode.ERROR_500_BIR_HATA_OLUSTU, "Token doğrulama sırasında beklenmeyen bir hata oluştu: " + ex.Message);
            }
        }

        /// <summary>
        /// Çıkış yapar (token'ı geçersiz kılar)
        /// </summary>
        public void logout(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, "Token bilgisi boş olamaz");
            }
            
            _kisiTokenManager.killToken(token);
        }

        /// <summary>
        /// Kişinin tüm token'larını geçersiz kılar
        /// </summary>
        public void logoutAll(int kisiId)
        {
            if (kisiId <= 0)
            {
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, "Geçerli bir kişi ID'si belirtilmelidir");
            }
            
            _kisiTokenManager.killAllTokensForKisi(kisiId);
        }


    }
}