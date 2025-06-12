using System;

namespace LorePdks.COMMON.DTO.KameraHareket
{
    /// <summary>
    /// Kamera Hareket tarih aralığı isteği için DTO
    /// </summary>
    public class KameraHareketDateRangeRequestDTO
    {
        public int firmaId { get; set; }
        public DateTime baslangicTarihi { get; set; }
        public DateTime bitisTarihi { get; set; }
    }
}
