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
    public partial class t_firma_cihaz  : _BaseModel
    {
		public t_firma_cihaz() 
		{
		}

     //   [AutoIncrement]
     [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]	
	  public int ID { get; set;}
        [Required]
        public int FIRMA_ID { get; set;}
        [Required]
        public int CIHAZ_MAKINE_GERCEK_ID { get; set;}
        public int? FIRMA_CIHAZ_TIP_KID { get; set;}
        public string AD { get; set;}
        public string ACIKLAMA { get; set;}
        [Required]
        public int ISDELETED { get; set;}
        public int? CREATEDUSER { get; set;}
        public DateTime? CREATEDDATE { get; set;}
        public int? MODIFIEDUSER { get; set;}
        public DateTime? MODIFIEDDATE { get; set;}
    } 
	public enum t_firma_cihaz_PROPERTIES {

		ID,
		FIRMA_ID,
		CIHAZ_MAKINE_GERCEK_ID,
		FIRMA_CIHAZ_TIP_KID,
		AD,
		ACIKLAMA,
		ISDELETED,
		CREATEDUSER,
		CREATEDDATE,
		MODIFIEDUSER,
		MODIFIEDDATE,
		
	}
}
#pragma warning restore 1591
