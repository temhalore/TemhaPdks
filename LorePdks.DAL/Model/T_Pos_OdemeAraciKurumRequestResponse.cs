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


    [Dapper.Contrib.Extensions.Table("Pos.OdemeAraciKurumRequestResponse")]

    public partial class T_Pos_OdemeAraciKurumRequestResponse    {
		public T_Pos_OdemeAraciKurumRequestResponse()
		{
		}

        //public static string SchemeName { get { return "Pos"; } }
        public static string GetSchema() { return "Pos"; }

        //[AutoIncrement]
		[Dapper.Contrib.Extensions.Key]
        public int ID { get; set;}
        [Required]
        public int ODEME_ID { get; set;}
        [Required]
        public int ARACI_KURUM_KOD_ID { get; set;}
        public string ARACI_GONDERILEN_JSON { get; set;}
        public string ARACI_DONEN_JSON { get; set;}
        [Required]
        public bool ISDELETED { get; set;}
        public long? CREATEDUSER { get; set;}
        public DateTime? CREATEDDATE { get; set;}
        public long? MODIFIEDUSER { get; set;}
        public DateTime? MODIFIEDDATE { get; set;}
        public string CREATEDIP { get; set;}
        public string MODIFIEDIP { get; set;}
        public string ARACI_USER_DONEN_JSON { get; set;}
    }
	
    
    public class T_Pos_OdemeAraciKurumRequestResponseMapper : AutoClassMapper<T_Pos_OdemeAraciKurumRequestResponse>
    {
        public T_Pos_OdemeAraciKurumRequestResponseMapper()
            : base()
        {
            Schema("Pos");
        }
    }
 
	public enum T_Pos_OdemeAraciKurumRequestResponse_Properties {

		ID,
		ODEME_ID,
		ARACI_KURUM_KOD_ID,
		ARACI_GONDERILEN_JSON,
		ARACI_DONEN_JSON,
		ISDELETED,
		CREATEDUSER,
		CREATEDDATE,
		MODIFIEDUSER,
		MODIFIEDDATE,
		CREATEDIP,
		MODIFIEDIP,
		ARACI_USER_DONEN_JSON,
		
	}	
}

#pragma warning restore 1591
