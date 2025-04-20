using LorePdks.COMMON.DTO.Common;

namespace LorePdks.BAL.Managers.Auth.Interfaces
{
    public interface IAuthManager
    {
        /// <summary>
        /// Kişi login işlemini gerçekleştirir
        /// </summary>
        KisiTokenDTO Login(string loginName, string sifre, string ipAdresi, string userAgent);
        
        /// <summary>
        /// Token kontrol eder
        /// </summary>
        KisiTokenDTO ValidateToken(string token);
        
        /// <summary>
        /// Çıkış yapar (token'ı geçersiz kılar)
        /// </summary>
        void Logout(string token);
        
        /// <summary>
        /// Kişinin tüm token'larını geçersiz kılar
        /// </summary>
        void LogoutAll(int kisiId);
    }
}