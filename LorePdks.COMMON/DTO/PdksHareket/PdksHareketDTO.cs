using LorePdks.COMMON.DTO.Base;
using LorePdks.COMMON.DTO.Common;
using LorePdks.COMMON.DTO.Firma;
using LorePdks.COMMON.DTO.FirmaCihaz;
using LorePdks.COMMON.DTO.Kisi;
using System;

namespace LorePdks.COMMON.DTO.PdksHareket
{
    /// <summary>
    /// PDKS Hareket DTO - Yeni merkezi log sistemi için
    /// </summary>
    public class PdksHareketDTO : BaseDTO
    {
        public FirmaDTO firmaDto { get; set; }
        public KisiDTO kisiDto { get; set; }
        public FirmaCihazDTO firmaCihazDto { get; set; }
        public string kullaniciCihazKodu { get; set; }
        public DateTime hareketTarihi { get; set; }
        public KodDTO pdksYonKodDto { get; set; }
        public string hamLogVerisi { get; set; }
    }
}
