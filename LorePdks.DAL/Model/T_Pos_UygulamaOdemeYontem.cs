// <auto-generated />
// This file was generated by a T4 template.
// Don't change it directly as your change would get overwritten.  Instead, make changes
// to the .tt file (i.e. the T4 template) and save it to regenerate this file.

// Make sure the compiler doesn't complain about missing Xml comments
#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using DapperExtensions.Mapper;
using System.ComponentModel.DataAnnotations.Schema;

namespace LorePdks.DAL.Model
{


    [Dapper.Contrib.Extensions.Table("Pos.UygulamaOdemeYontem")]

    public partial class T_Pos_UygulamaOdemeYontem    {
		public T_Pos_UygulamaOdemeYontem()
		{
		}

        //public static string SchemeName { get { return "Pos"; } }
        public static string GetSchema() { return "Pos"; }

        //[AutoIncrement]
		[Dapper.Contrib.Extensions.Key]
        public int ID { get; set;}
        [Required]
        public int UYGULAMA_ID { get; set;}
        [Required]
        public int ODEME_YONTEM_KOD_ID { get; set;}
        [Required]
        public DateTime ODEME_YONTEM_GECERLILIK_BASLANGIC_TARIH { get; set;}
        [Required]
        public DateTime ODEME_YONTEM_GECERLILIK_BITIS_TARIH { get; set;}
        [Required]
        public bool IS_AKTIF { get; set;}
        [Required]
        public bool ISDELETED { get; set;}
        public long? CREATEDUSER { get; set;}
        public DateTime? CREATEDDATE { get; set;}
        public long? MODIFIEDUSER { get; set;}
        public DateTime? MODIFIEDDATE { get; set;}
        public string CREATEDIP { get; set;}
        public string MODIFIEDIP { get; set;}
    }
	
    
    public class T_Pos_UygulamaOdemeYontemMapper : AutoClassMapper<T_Pos_UygulamaOdemeYontem>
    {
        public T_Pos_UygulamaOdemeYontemMapper()
            : base()
        {
            Schema("Pos");
        }
    }
 
	public enum T_Pos_UygulamaOdemeYontem_Properties {

		ID,
		UYGULAMA_ID,
		ODEME_YONTEM_KOD_ID,
		ODEME_YONTEM_GECERLILIK_BASLANGIC_TARIH,
		ODEME_YONTEM_GECERLILIK_BITIS_TARIH,
		IS_AKTIF,
		ISDELETED,
		CREATEDUSER,
		CREATEDDATE,
		MODIFIEDUSER,
		MODIFIEDDATE,
		CREATEDIP,
		MODIFIEDIP,
		
	}	
}

#pragma warning restore 1591
