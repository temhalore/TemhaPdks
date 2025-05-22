using LorePdks.COMMON.DTO.Common;
using LorePdks.COMMON.DTO.FirmaKisi;
using LorePdks.DAL.Model;

namespace LorePdks.BAL.Managers.FirmaKisi.Interfaces
{
    public interface IFirmaKisiManager
    {
        FirmaKisiDTO saveFirmaKisi(FirmaKisiDTO firmaKisiDto);
        void deleteFirmaKisiByFirmaKisiId(int firmaKisiId);
        t_firma_kisi getFirmaKisiByFirmaKisiId(int firmaKisiId, bool isYoksaHataDondur = false);
        List<t_firma_kisi> getFirmaKisiListByFirmaId(int firmaId);
        List<FirmaKisiDTO> getFirmaKisiDtoListByFirmaId(int firmaId);
        FirmaKisiDTO getFirmaKisiDtoById(int firmaKisiId, bool isYoksaHataDondur = false);
        void checkFirmaKisiDtoKayitEdilebilirMi(FirmaKisiDTO firmaKisiDto);
    }
}
