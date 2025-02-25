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


    [Dapper.Contrib.Extensions.Table("Pos.Kod")]

    public partial class T_Pos_Kod    {
		public T_Pos_Kod()
		{
		}

        //public static string SchemeName { get { return "Pos"; } }
        public static string GetSchema() { return "Pos"; }

        [Required]
		[Dapper.Contrib.Extensions.Key]
        public int ID { get; set;}
        [Required]
        public int TIP_ID { get; set;}
        public string KOD { get; set;}
        [Required]
        public int SIRA { get; set;}
        public string KISA_AD { get; set;}
        public string DIGER_UYG_ENUM_AD { get; set;}
        public int? DIGER_UYG_ID { get; set;}
        [Required]
        public bool ISDELETED { get; set;}
        public long? CREATEDUSER { get; set;}
        public DateTime? CREATEDDATE { get; set;}
        public long? MODIFIEDUSER { get; set;}
        public DateTime? MODIFIEDDATE { get; set;}
        public string CREATEDIP { get; set;}
        public string MODIFIEDIP { get; set;}
    }
	
    
    public class T_Pos_KodMapper : AutoClassMapper<T_Pos_Kod>
    {
        public T_Pos_KodMapper()
            : base()
        {
            Schema("Pos");
        }
    }
 
	public enum T_Pos_Kod_Properties {

		ID,
		TIP_ID,
		KOD,
		SIRA,
		KISA_AD,
		DIGER_UYG_ENUM_AD,
		DIGER_UYG_ID,
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
