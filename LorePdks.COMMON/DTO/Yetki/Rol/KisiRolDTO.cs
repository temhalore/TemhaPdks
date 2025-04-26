using LorePdks.COMMON.DTO.Base;
using System;
using System.Text.Json.Serialization;

namespace LorePdks.COMMON.DTO.Yetki.Rol
{
    public class KisiRolDTO : BaseDTO
    {


        public int kisiId { get; set; }

        public int rolId { get; set; }
        public string rolAdi { get; set; }
    }
}