using LorePdks.COMMON.DTO.Common;
using LorePdks.DAL.Model;

namespace LorePdks.BAL.Managers.Deneme.Interfaces
{
    public interface IFirmaManager
    {
        public FirmaDTO saveFirma(FirmaDTO firmaDto);
        public void deleteFirmaByFirmaId(int firmaId);
        public t_firma getFirmaByFirmaId(int firmaId, bool isYoksaHataDondur = false);
        public FirmaDTO getFirmaDtoById(int firmaId, bool isYoksaHataDondur = false);
        public List<FirmaDTO> getFirmaDtoListById(bool isYoksaHataDondur = false);
    }
}
