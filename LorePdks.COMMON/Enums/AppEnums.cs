using Castle.Components.DictionaryAdapter.Xml;
using System;
namespace LorePdks.COMMON.Enums

{
    public class AppEnums
    {


        public enum ParaBirim
        {
            Empty = 0,
            TL = 1100001,
            USD = 1100002,
            EUR = 1100003,
            GBP = 1100004,

        }
        public enum KodTipList
        {
            Odeme_Yontem = 101,
            Odeme_Yontem_Guvenlik = 102,
            Odeme_Durum = 103,
            Araci_Kurum = 104,
            Iade_Iptal_Islem_Tip = 105,
            Iade_Iptal_Durum = 109,
            KISI_TOKEN_DELETE_REASON = 106,
            LOGIN_ARACI_KURUM_TIP = 107,
            Para_Birim = 110,
            OdemeKeyUretimTip = 111,


        }

        public enum KISI_TOKEN_DELETE_REASON
        {
            YENIDEN_LOGIN_SEBEBI_ILE = 1060001,
            IP_DEGISIKLIGI_SEBEBI_ILE = 1060002,
            YERINE_LOGIN_OLUNMASI_SEBEBI_ILE = 1060003,
            GECERLILIK_TARIHI_DOLMASI_SEBEBI_ILE = 1060004,
            USER_AGENT_DEGISIKLIGI_SEBEBI_ILE = 1060005,
            KENDI_ISTEGI_SEBEBI_ILE = 1060006,
            KONTROLDEN_GECMEDIGI_ICIN = 1060007,
        }
       

        public enum LOG_INDEX_TIP
        {
            LOGIN = 1, // NUMARALAR KULLANILMAYACAK KARŞILIĞI YOK
            GENEL = 2,
           
        }
    }
}
