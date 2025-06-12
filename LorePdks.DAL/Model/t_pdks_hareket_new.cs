using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LorePdks.DAL.Model
{
    public partial class t_pdks_hareket : _BaseModel
    {
        public t_pdks_hareket() 
        {
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        
        [Required]
        public int FIRMA_ID { get; set; }
        
        public int? KISI_ID { get; set; }
        
        [Required]
        public int FIRMA_CIHAZ_ID { get; set; }
        
        public string KULLANICI_CIHAZ_KODU { get; set; }
        
        public DateTime? HAREKET_TARIHI { get; set; }
        
        public int? HAREKET_YONU_KID { get; set; } // t_kod referansı (Giriş/Çıkış)
        
        public string CIHAZ_LOG_DATA { get; set; } // Ham log verisi
        
        public int? PARSING_DURUMU_KID { get; set; } // t_kod referansı
        
        public string HATA_MESAJI { get; set; }
        
        [Required]
        public int ISDELETED { get; set; }
        
        public int? CREATEDUSER { get; set; }
        
        public DateTime? CREATEDDATE { get; set; }
        
        public int? MODIFIEDUSER { get; set; }
        
        public DateTime? MODIFIEDDATE { get; set; }
    }

    public enum t_pdks_hareket_PROPERTIES 
    {
        ID,
        FIRMA_ID,
        KISI_ID,
        FIRMA_CIHAZ_ID,
        KULLANICI_CIHAZ_KODU,
        HAREKET_TARIHI,
        HAREKET_YONU_KID,
        CIHAZ_LOG_DATA,
        PARSING_DURUMU_KID,
        HATA_MESAJI,
        ISDELETED,
        CREATEDUSER,
        CREATEDDATE,
        MODIFIEDUSER,
        MODIFIEDDATE
    }
}
