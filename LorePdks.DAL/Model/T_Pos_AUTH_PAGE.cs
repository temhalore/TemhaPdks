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


    [Dapper.Contrib.Extensions.Table("Pos.AUTH_PAGE")]

    public partial class T_Pos_AUTH_PAGE    {
		public T_Pos_AUTH_PAGE()
		{
		}

        //public static string SchemeName { get { return "Pos"; } }
        public static string GetSchema() { return "Pos"; }

        //[AutoIncrement]
		[Dapper.Contrib.Extensions.Key]
        public int ID { get; set;}
        public string NAME { get; set;}
        public string ROUTER_LINK { get; set;}
        public int? ORDER_NO { get; set;}
        public long? CREATEDUSER { get; set;}
        public DateTime? CREATEDDATE { get; set;}
        public long? MODIFIEDUSER { get; set;}
        public DateTime? MODIFIEDDATE { get; set;}
        public string CREATEDIP { get; set;}
        public string MODIFIEDIP { get; set;}
        [Required]
        public bool ISDELETED { get; set;}
    }
	
    
    public class T_Pos_AUTH_PAGEMapper : AutoClassMapper<T_Pos_AUTH_PAGE>
    {
        public T_Pos_AUTH_PAGEMapper()
            : base()
        {
            Schema("Pos");
        }
    }
 
	public enum T_Pos_AUTH_PAGE_Properties {

		ID,
		NAME,
		ROUTER_LINK,
		ORDER_NO,
		CREATEDUSER,
		CREATEDDATE,
		MODIFIEDUSER,
		MODIFIEDDATE,
		CREATEDIP,
		MODIFIEDIP,
		ISDELETED,
		
	}	
}

#pragma warning restore 1591
