using LorePdks.COMMON.DTO.Auth.Securty.Ekran;
using System.Collections.Generic;

namespace LorePdks.BAL.Managers.Auth.Yetki.Ekran.Interfaces
{
    public interface IEkranManager
    {
        List<EkranDTO> getEkranDtoList(bool isYoksaHataDondur = false);
        EkranDTO getEkranDtoById(int ekranId, bool isYoksaHataDondur = false);
        List<EkranDTO> getEkranDtoListByRolId(int rolId, bool isYoksaHataDondur = false);
        List<EkranDTO> getEkranDtoListByKisiId(int kisiId, bool isYoksaHataDondur = false);
        List<EkranDTO> getMenuByKisiId(int kisiId);
        EkranDTO saveEkran(EkranDTO ekranDTO);
        void deleteEkranByEkranId(int ekranId);
    }
}