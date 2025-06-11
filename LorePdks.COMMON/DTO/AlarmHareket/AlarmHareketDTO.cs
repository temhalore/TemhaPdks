using LorePdks.COMMON.DTO.Base;
using LorePdks.COMMON.DTO.Common;
using LorePdks.COMMON.DTO.Firma;
using LorePdks.COMMON.DTO.FirmaCihaz;
using System;

namespace LorePdks.COMMON.DTO.AlarmHareket
{
    /// <summary>
    /// Alarm Hareket DTO - Yeni merkezi log sistemi için
    /// </summary>
    public class AlarmHareketDTO : BaseDTO
    {
        public FirmaDTO firmaDto { get; set; }
        public FirmaCihazDTO firmaCihazDto { get; set; }
        public DateTime alarmTarihi { get; set; }
        public KodDTO alarmTipKodDto { get; set; }
        public KodDTO alarmSeviyeKodDto { get; set; }
        public string sensorBilgisi { get; set; }
        public string aciklama { get; set; }
        public string hamLogVerisi { get; set; }
    }
}
