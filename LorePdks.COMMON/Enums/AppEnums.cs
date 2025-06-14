using Castle.Components.DictionaryAdapter.Xml;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
namespace LorePdks.COMMON.Enums

{
    public class AppEnums
    {
        public enum KodTipList
        {
            FIRMA_CIHAZ_TIP = 101,
            PARSE_METODU = 205,
            PDKS_LOG_ALANLARI = 206,
            PDKS_YON = 207,
            ALARM_TIPI = 208,
            ALARM_SEVIYESI = 209,
            KAMERA_OLAY_TIPI = 210,
            FIRMA_KISI_TIP = 102,
            ROL_TIP = 103,
            KISI_TOKEN_DELETE_REASON = 901,
            LOG_INDEX_TIP = 1,
            LOG_ALAN_TIPI = 218,
            LOG_TANIMLI_ALAN = 219,
            LOG_TARIH_FORMATI = 216,
            LOG_SAAT_FORMATI = 217,
            LOG_AYIRICI = 215,  
        }
 public enum LOG_AYIRICI{
        VIRGUL=2150001,	
	NOKTALI_VIRGUL=2150002,	
	TAB=2150003,	
	BOSLUK=2150004,	
	BORU=2150005,	
	TIRE=2150006,	
	IKI_NOKTA=2150007,
        }	
		
		
 public enum LOG_TARIH_FORMATI{
            YIL_AY_GUN_1 = 2160001,	
	GUN_AY_YIL_2 = 2160002,	
	AY_GUN_YIL_3 = 2160003,	
	GUN_AY_YIL_4 = 2160004,	
	AY_GUN_YIL_5 = 2160005,	
	YIL_AY_GUN_6 = 2160006,	
	GUN_AY_YIL_7 = 2160007,	
	AY_GUN_YIL_8 = 2160008,	
        }
	


		
public enum LOG_SAAT_FORMATI{


    SAAT_DAKIKA_SANIYE_1 = 2170001,	
	SAAT_DAKIKA_2 = 2170002,
	_12S_D_S_AM_PM_3 = 2170003,
	_12S_D_S_AM_PM_TEK_4 = 2170004,	
	SAAT_DAKIKA_SANIYE_5 = 2170005,	
	SAAT_DAKIKA_SANIYE_6 = 2170006,	
        }	

		
 public enum LOG_ALAN_TIPI{


    METIN = 2180001,	
	SAYI = 2180002,	
	TARIH = 2180003,	
	SAAT = 2180004,	
	TARIH_SAAT = 2180005,	
	MANTIKSAL = 2180006,	
        }		

		
 public enum LOG_TANIMLI_ALAN{
      KULLANICI_ID = 2190001,	
	TARIH = 2190002,	
	SAAT = 2190003,	
	TARIH_SAAT = 2190004,	
	DURUM = 2190005,	
	ISLEM = 2190006,	
	OLAY_KODU = 2190007,	
	CIHAZ_ID = 2190008,	
	KART_NO = 2190009,	
	PERSONEL_NO = 2190010,	
	SICIL_NO = 2190011,	
	MESAJ = 2190012,	
	LOG_SEVIYESI = 2190013,	
	KAPI_ID = 2190014,	
	YON = 2190015,	
	IP_ADRESI = 2190016,	
	MAC_ADRESI = 2190017,	
	SERI_NO = 2190018,	
	YAZILIM_SURUMU = 2190019,	
	KONUM = 2190020,
        }	

  	





        public enum FIRMA_CIHAZ_TIP{
            PDKS_CIHAZI_QR = 1010001,
            KAMERA = 1010002,
            PDKS = 1010003,
            ALARM = 1010004,
            HAREKET_SENSORU = 1010005,
            HAREKET_SENSORU_KAMERA_UZERINDE = 1010006,
            YANGIN_SENSORU = 1010007,
            SU_SENSORU = 1010008,
            DUMAN_DEDEKTORU = 1010009,

        }

        public enum PARSE_METODU{
        
	CSV_FORMAT=2050001,	
	KELIME_BAZLI=2050002,
        }		
		
public enum PDKS_LOG_ALANLARI{
            KULLANICI_ID = 2060001,
            TARIH = 2060002,
            SAAT = 2060003,
            YON = 2060004,
            CIHAZ_ID = 2060005,
        }
		

 
		
 public enum PDKS_YON{

            GIRIS = 2070001,
            CIKIS = 2070002,
        }		
	
		
public enum ALARM_TIPI{

            HAREKET_ALGILAMA = 2080001,
            KAPI_ACIK = 2080002,
            GUVENLIK_IHLALI = 2080003,
            SISTEM_HATASI = 2080004,
            ALARM_KAPATILDI = 2080005,
        }		
	
		
 public enum ALARM_SEVIYESI{

            DUSUK = 2090001,
            ORTA = 2090002,
            YUKSEK = 2090003,
            KRITIK = 2090004,
        }	

		
 public enum KAMERA_OLAY_TIPI{

            HAREKET = 2100001,
            KAYIT_BASLAMA = 2100002,
            KAYIT_BITME = 2100003,
            BAGLANTI_KESILMESI = 2100004,
        }	


        public enum FIRMA_KISI_TIP{
            PERSONEL = 1020001,
            YETKILI = 1020002,
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
