using System;

namespace LorePdks.COMMON.DTO.AlarmHareket
{
    /// <summary>
    /// Alarm Hareket tarih aralığı isteği için DTO
    /// </summary>
    public class AlarmHareketDateRangeRequestDTO
    {
        public int firmaId { get; set; }
        public DateTime baslangicTarihi { get; set; }
        public DateTime bitisTarihi { get; set; }
    }
}
