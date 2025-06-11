using LorePdks.COMMON.DTO.Base;
using LorePdks.COMMON.DTO.Common;
using LorePdks.COMMON.DTO.Firma;
using LorePdks.COMMON.DTO.FirmaCihaz;
using System;

namespace LorePdks.COMMON.DTO.KameraHareket
{
    /// <summary>
    /// Kamera Hareket DTO - Yeni merkezi log sistemi için
    /// </summary>
    public class KameraHareketDTO : BaseDTO
    {
        public FirmaDTO firmaDto { get; set; }
        public FirmaCihazDTO firmaCihazDto { get; set; }
        public DateTime olayTarihi { get; set; }
        public KodDTO kameraOlayTipKodDto { get; set; }
        public string dosyaYol { get; set; }
        public string aciklama { get; set; }
        public string hamLogVerisi { get; set; }
    }
}
