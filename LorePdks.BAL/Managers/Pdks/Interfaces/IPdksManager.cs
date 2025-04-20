using LorePdks.COMMON.DTO.Pdks;
using LorePdks.DAL.Model;

namespace LorePdks.BAL.Managers.Pdks.Interfaces
{
    public interface IPdksManager
    {
        PdksDTO savePdks(PdksDTO pdksDto);
        void deletePdksByPdksId(int pdksId);
        t_pdks getPdksByPdksId(int pdksId, bool isYoksaHataDondur = false);
        PdksDTO getPdksDtoById(int pdksId, bool isYoksaHataDondur = false);
        List<PdksDTO> getPdksDtoList(bool isYoksaHataDondur = false);
        void checkPdksDtoKayitEdilebilirMi(PdksDTO pdksDto);
    }
}
