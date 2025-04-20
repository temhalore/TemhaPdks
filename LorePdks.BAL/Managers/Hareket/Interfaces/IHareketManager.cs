using LorePdks.COMMON.DTO.Common;
using LorePdks.DAL.Model;

namespace LorePdks.BAL.Managers.Deneme.Interfaces
{
    public interface IHareketManager
    {
        HareketDTO saveHareket(HareketDTO hareketDto);
        List<HareketDTO> getHareketListByFirmaId(int firmaId);
        void deleteHareketById(int hareketId);
        HareketDTO getHareketDtoById(int hareketId);
        t_hareket getHareketByHareketId(int hareketId, bool isYoksaHataDondur = false);
    }
}
