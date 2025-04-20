using LorePdks.COMMON.DTO.Common;

namespace LorePdks.BAL.Managers.Auth.Interfaces
{
    public interface IAuthManager
    {
        /// <summary>
        /// Kişi login işlemini gerçekleştirir
        /// </summary>
        KisiTokenDTO login(string loginName, string sifre);
        
        /// <summary>
        /// Kişi login işlemini gerçekleştirir ve erişim menülerini de döndürür
        /// </summary>

        
        /// <summary>
        /// Token kontrol eder
        /// </summary>
        KisiTokenDTO validateToken(string token);
        
        /// <summary>
        /// Çıkış yapar (token'ı geçersiz kılar)
        /// </summary>
        void logout(string token);
        
        /// <summary>
        /// Kişinin tüm token'larını geçersiz kılar
        /// </summary>
        void logoutAll(int kisiId);
    }
}