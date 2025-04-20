using LorePdks.COMMON.DTO.Common;
using LorePdks.DAL.Model;

namespace LorePdks.BAL.Managers.Kisi.Interfaces
{
    public interface IKisiTokenManager
    {
        KisiTokenDTO saveKisiToken(KisiTokenDTO kisiTokenDto);
        void deleteKisiTokenById(int kisiTokenId);
        t_kisi_token getKisiTokenById(int kisiTokenId, bool isYoksaHataDondur = false);
        KisiTokenDTO getKisiTokenDtoById(int kisiTokenId, bool isYoksaHataDondur = false);
        KisiTokenDTO getKisiTokenDtoByToken(string token, bool isYoksaHataDondur = false);
        List<KisiTokenDTO> getKisiTokenDtoListByKisiId(int kisiId);
        void killToken(string token);
        void killAllTokensForKisi(int kisiId);
    }
}