using Castle.Components.DictionaryAdapter.Xml;
using System;
namespace LorePdks.COMMON.Enums

{
    public class AppEnums
    {
        public enum KodTipList
        {
            FIRMA_CIHAZ_TIP = 101,
            KISI_TOKEN_DELETE_REASON = 901,


        }

        public enum FIRMA_CIHAZ_TIP{
        QR_MODEL=1010001,
	MODEL_1=1010002,
	MODEL_2=1020003,
	MODEL_3=1020004,
	MODEL_4=1020005,
        }
	


 

        public enum KISI_TOKEN_DELETE_REASON
        {
            YENIDEN_LOGIN_SEBEBI_ILE = 9010001,
            IP_DEGISIKLIGI_SEBEBI_ILE = 9010002,
            YERINE_LOGIN_OLUNMASI_SEBEBI_ILE = 9010003,
            GECERLILIK_TARIHI_DOLMASI_SEBEBI_ILE = 9010004,
            USER_AGENT_DEGISIKLIGI_SEBEBI_ILE = 9010005,
            KENDI_ISTEGI_SEBEBI_ILE = 9010006,
            KONTROLDEN_GECMEDIGI_ICIN = 9010007,
        }
       

        public enum LOG_INDEX_TIP
        {
            LOGIN = 1, // NUMARALAR KULLANILMAYACAK KARŞILIĞI YOK
            GENEL = 2,
           
        }
    }
}
