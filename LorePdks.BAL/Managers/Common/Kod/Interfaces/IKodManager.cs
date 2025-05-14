using LorePdks.COMMON.DTO.Common;
using LorePdks.COMMON.Enums;

namespace LorePdks.BAL.Managers.Common.Kod.Interfaces
{
    public interface IKodManager
    {
        List<KodDTO> GetKodDtoListByKodTipId(int kodTipID);
        string refreshKodListCache();
        KodDTO GetKodDtoByKodId(int kodId);
        KodDTO saveKod(KodDTO kodDTO);

        public List<KodDTO> allKodDtoList(bool cacheYenilensinMi = false);

        /// <summary>
        /// gelen KodDTO tipindeki değer null ve id sinin 0 dan büyük olması kontrol edilir
        /// ayrıca gelen data genel kod tablosunda varmı diye kontrol edilir 
        /// bunların sonucunda hata yada true dööner
        /// hata mesajı boş geçilebilir boş geçilirse default hata mesajı döner
        /// </summary>
        /// <param name="kodDto"></param>
        /// <param name="hataMesaji"></param>
        /// <returns></returns>
        bool checkKodDTOIsNull(KodDTO kodDto, string hataMesaji = "");

        /// <summary>
        /// gelen kod kendi kod tpleri içinde kontrol edilir eğer kendi tip id bloğunun içinde yoksa hata döner
        /// hata mesajı boş geçilebilir boş geçilirse default hata mesajı döner
        /// 
        /// </summary>
        /// <param name="kodDto"></param>
        /// <param name="enumTip"></param>
        /// <param name="hataMesaji"></param>
        /// <returns></returns>
        bool checkKodDTOIdInTipList(KodDTO kodDto, AppEnums.KodTipList enumTip, string hataMesaji = "");
    }
}
