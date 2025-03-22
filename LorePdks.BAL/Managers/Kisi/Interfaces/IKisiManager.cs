using LorePdks.COMMON.DTO.Common;
using LorePdks.DAL.Model;

namespace LorePdks.BAL.Managers.Deneme.Interfaces
{
    public interface IKisiManager
    {
        //KisiManager sınıfında bulunan  tüm metodlar buraya yaz
        public KisiDTO saveKisi(KisiDTO kisiDto);
        public void deleteKisiByKisiId(int kisiId);
        public t_kisi getKisiByKisiId(int kisiId, bool isYoksaHataDondur = false);
        public KisiDTO getKisiDtoById(int kisiId, bool isYoksaHataDondur = false);
        public List<KisiDTO> getKisiDtoListById(bool isYoksaHataDondur = false);

        public KisiDTO getKisiDtoByLoginNameAndSifre(string loginName, string sifre, bool isYoksaHataDondur = false);
        public KisiDTO getKisiDtoByLoginName(string loginName, bool isYoksaHataDondur = false);
        public t_kisi getKisiByLoginName(string loginName, bool isYoksaHataDondur = false);
        public KisiDTO getKisiDtoByTc(string tc, bool isYoksaHataDondur = false);
        public List<KisiDTO> getKisiDtoListByAramaText(string aramaText, bool isYoksaHataDondur = false);
    }
}
