using LorePdks.COMMON.DTO.Base;
using LorePdks.COMMON.DTO.Yetki.Ekran;
using System;
using System.Collections.Generic;

namespace LorePdks.COMMON.DTO.Yetki.Rol
{
    public class RolDTO : BaseDTO
    {
        public string rolAdi { get; set; }
        public string aciklama { get; set; }
        public List<EkranDTO> ekranlar { get; set; }
    }
}