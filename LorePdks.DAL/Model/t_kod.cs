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
using System.ComponentModel.DataAnnotations.Schema;
using LorePdks.DAL.Model;
namespace LorePdks.DAL.Model
{
    public partial class t_kod  : _BaseModel
    {
		public t_kod() 
		{
		}

        [Required]
		[Key]
        public int ID { get; set;}
        public int? TIP_ID { get; set;}
        public string KOD { get; set;}
        public int? SIRA { get; set;}
        public string KISA_AD { get; set;}
        public string DIGER_UYG_ENUM_AD { get; set;}
        public int? DIGER_UYG_ID { get; set;}
        [Required]
        public int ISDELETED { get; set;}
        public int? CREATEDUSER { get; set;}
        public DateTime? CREATEDDATE { get; set;}
        public int? MODIFIEDUSER { get; set;}
        public DateTime? MODIFIEDDATE { get; set;}
    } 
	public enum t_kod_PROPERTIES {

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
		
	}
}
#pragma warning restore 1591
