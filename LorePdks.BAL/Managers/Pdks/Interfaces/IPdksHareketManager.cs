using LorePdks.COMMON.DTO.Pdks;
using LorePdks.DAL.Model;

namespace LorePdks.BAL.Managers.Pdks.Interfaces
{
    public interface IPdksHareketManager
    {
        PdksHareketDTO savePdksHareket(PdksHareketDTO pdksHareketDto);
        void deletePdksHareketByPdksHareketId(int pdksHareketId);
        t_pdks_hareket getPdksHareketByPdksHareketId(int pdksHareketId, bool isYoksaHataDondur = false);
        PdksHareketDTO getPdksHareketDtoById(int pdksHareketId, bool isYoksaHataDondur = false);
        List<PdksHareketDTO> getPdksHareketDtoList(bool isYoksaHataDondur = false);
        void checkPdksHareketDtoKayitEdilebilirMi(PdksHareketDTO pdksHareketDto);
    }
}