using LorePdks.COMMON.DTO.Common;
using LorePdks.DAL.Model;
using System.Collections.Generic;

namespace LorePdks.BAL.Managers.KisiToken.Interfaces
{
    public interface IKisiTokenManager
    {
        /// <summary>
        /// Token bilgilerini kaydeder
        /// </summary>
        KisiTokenDTO saveKisiToken(KisiTokenDTO kisiTokenDto);

        /// <summary>
        /// Token'a göre kişi token bilgilerini getirir
        /// </summary>
        KisiTokenDTO getKisiTokenDtoByToken(string token, bool isYoksaHataDondur = false);

        /// <summary>
        /// Verilen token'ı geçersiz kılar (siler veya ISDELETED=1 yapar)
        /// </summary>
        void killToken(string token);

        /// <summary>
        /// Kişiye ait tüm token'ları geçersiz kılar
        /// </summary>
        void killAllTokensForKisi(int kisiId);

        /// <summary>
        /// Kişi ID'sine göre aktif token bilgisini getirir
        /// </summary>
        KisiTokenDTO getAktifKisiTokenByKisiId(int kisiId);

        /// <summary>
        /// ID'ye göre kişi token'ı siler
        /// </summary>
        void deleteKisiTokenById(int kisiTokenId);

        /// <summary>
        /// ID'ye göre kişi token DTO getirir
        /// </summary>
        KisiTokenDTO getKisiTokenDtoById(int kisiTokenId, bool isYoksaHataDondur = false);

        /// <summary>
        /// Kişi ID'sine göre tüm token'ları getirir
        /// </summary>
        List<KisiTokenDTO> getKisiTokenDtoListByKisiId(int kisiId);
    }
}