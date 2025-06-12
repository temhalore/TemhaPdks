using System;

namespace LorePdks.COMMON.DTO.PdksHareket
{
    /// <summary>
    /// PDKS Hareket tarih aralığı isteği için DTO
    /// </summary>
    public class PdksHareketDateRangeRequestDTO
    {
        public int firmaId { get; set; }
        public DateTime baslangicTarihi { get; set; }
        public DateTime bitisTarihi { get; set; }
    }
}
