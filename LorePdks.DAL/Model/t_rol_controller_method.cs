// filepath: d:\Development\ozel\TemhaPdks\LorePdks.DAL\Model\t_rol_controller_method.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LorePdks.DAL.Model;

namespace LorePdks.DAL.Model
{
    public partial class t_rol_controller_method : _BaseModel
    {
        public t_rol_controller_method() 
        {
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]	
        public int ID { get; set;}
        [Required]
        public int ROL_ID { get; set;}
        [Required]
        public string CONTROLLER_NAME { get; set;}
        [Required]
        public string METHOD_NAME { get; set;}
        [Required]
        public int ISDELETED { get; set;}
        public int? CREATEDUSER { get; set;}
        public DateTime? CREATEDDATE { get; set;}
        public int? MODIFIEDUSER { get; set;}
        public DateTime? MODIFIEDDATE { get; set;}
    } 

    public enum t_rol_controller_method_PROPERTIES 
    {
        ID,
        ROL_ID,
        CONTROLLER_NAME,
        METHOD_NAME,
        ISDELETED,
        CREATEDUSER,
        CREATEDDATE,
        MODIFIEDUSER,
        MODIFIEDDATE,
    }
}