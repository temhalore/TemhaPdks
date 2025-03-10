using LorePdks.COMMON.DTO.Common;
using LorePdks.DAL.Model;

namespace LorePdks.BAL.Managers.Deneme.Interfaces
{
    public interface IFirmaCihazManager
    {
        public void deleteFirmaCihazByFirmaCihazId(int firmaCihazId);
        public FirmaCihazDTO saveFirmaCihaz(FirmaCihazDTO firmaCihazDto);
        public t_firma_cihaz getFirmaCihazByFirmaCihazId(int firmaCihazId, bool isYoksaHataDondur = false);
        public FirmaCihazDTO getFirmaCihazDtoById(int firmaCihazId, bool isYoksaHataDondur = false);
        public List<FirmaCihazDTO> getFirmaCihazDtoListByFirmaId(int firmaId);
        public List<t_firma_cihaz> getFirmaCihazListByFirmaId(int firmaId);
    }
}
