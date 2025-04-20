using Castle.Components.DictionaryAdapter.Xml;
using System;
namespace LorePdks.COMMON.Enums

{
    public class AppEnums
    {
        public enum KodTipList
        {
            FIRMA_CIHAZ_TIP = 101,
            FIRMA_KISI_TIP = 102,
            ROL_TIP = 103,
            HAREKET_TIP = 104,
            HAREKET_DURUM = 105,
            KISI_TOKEN_DELETE_REASON = 901,


        }

        public enum FIRMA_CIHAZ_TIP{
        QR_MODEL=1010001,
	MODEL_1=1010002,
	MODEL_2=1010003,
	MODEL_3=1010004,
	MODEL_4=1010005,
        }
	

        public enum FIRMA_KISI_TIP{
            PERSONEL = 1020001,
            YETKILI = 1020002,
        }

        public enum HAREKET_TIP
        {
            PDKS = 1040001,
            QR = 1040002,
        }

        public enum HAREKET_DURUM
        {
            ISLEM_ICIN_BEKLIYOR = 1050001,
            ISLENDI = 1050002,
            ISLEMDE_HATA_ALINDI = 1050002,
        }

        public enum ROL_TIP {
            SUPER_ADMIN = 1030001,
            FIRMA_YETKILISI = 1030002,
            PERSONEL = 1030003,
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
