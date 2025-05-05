using LorePdks.COMMON.DTO.Base;
using System;
using System.Text.Json.Serialization;

namespace LorePdks.COMMON.DTO.Yetki.Rol
{
    public class KisiRolDTO : BaseDTO
    {


        public EIdDTO kisiEidDto { get; set; }

        public EIdDTO rolEidDto { get; set; }
        public string rolAdi { get; set; }
    }
}