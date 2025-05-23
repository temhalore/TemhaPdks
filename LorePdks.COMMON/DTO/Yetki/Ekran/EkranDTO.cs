using LorePdks.COMMON.DTO.Base;
using System;
using System.Collections.Generic;

namespace LorePdks.COMMON.DTO.Yetki.Ekran
{
    public class EkranDTO : BaseDTO
    {
        public string ekranAdi { get; set; }
        public string ekranYolu { get; set; }
        public string ekranKodu { get; set; }
        public string aciklama { get; set; }
        public EIdDTO ustEkranEidDto { get; set; }
        public int siraNo { get; set; }
        public string ikon { get; set; }
        public bool aktif { get; set; }
        public List<EkranDTO> altEkranlar { get; set; }
    }
}