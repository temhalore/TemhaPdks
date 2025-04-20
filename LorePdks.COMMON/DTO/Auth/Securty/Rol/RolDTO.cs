using LorePdks.COMMON.DTO.Auth.Securty.Ekran;
using LorePdks.COMMON.DTO.Base;
using System;
using System.Collections.Generic;

namespace LorePdks.COMMON.DTO.Auth.Securty.Rol
{
    public class RolDTO : BaseDTO
    {
        public string rolAdi { get; set; }
        public string aciklama { get; set; }
        public List<EkranDTO> ekranlar { get; set; }
    }
}