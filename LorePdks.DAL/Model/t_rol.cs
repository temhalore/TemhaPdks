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
    public partial class t_rol : _BaseModel
    {
        public t_rol() 
        {
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string ROL_ADI { get; set; }
        public string ACIKLAMA { get; set; }
        public string CONTROLLER_NAME { get; set; }
        public string CONTROLLER_METHOD_NAME { get; set; }
        [Required]
        public int ISDELETED { get; set; }
        public int? CREATEDUSER { get; set; }
        public DateTime? CREATEDDATE { get; set; }
        public int? MODIFIEDUSER { get; set; }
        public DateTime? MODIFIEDDATE { get; set; }
    }
    
    public enum t_rol_PROPERTIES {
        ID,
        ROL_ADI,
        ACIKLAMA,
        CONTROLLER_NAME,
        CONTROLLER_METHOD_NAME,
        ISDELETED,
        CREATEDUSER,
        CREATEDDATE,
        MODIFIEDUSER,
        MODIFIEDDATE,
    }
}
#pragma warning restore 1591