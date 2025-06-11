using LorePdks.COMMON.DTO.KameraHareket;
using LorePdks.DAL.Model;
using System;
using System.Collections.Generic;

namespace LorePdks.BAL.Managers.Kamera.Interfaces
{
    /// <summary>
    /// Kamera Hareket Manager Interface - Yeni merkezi log sistemi için
    /// Repo pattern kuralları: Save, Get, GetList, Delete metodları
    /// </summary>
    public interface IKameraHareketManager
    {
        /// <summary>
        /// Kamera hareket kaydı kaydet/güncelle (ID = 0 ise INSERT, > 0 ise UPDATE)
        /// </summary>
        KameraHareketDTO saveKameraHareket(KameraHareketDTO kameraHareketDto);

        /// <summary>
        /// ID'ye göre Kamera hareket kaydı sil (Soft delete - ISDELETED = 1)
        /// </summary>
        void deleteKameraHareketById(int kameraHareketId);

        /// <summary>
        /// ID'ye göre Kamera hareket entity getir
        /// </summary>
        t_kamera_hareket getKameraHareketById(int kameraHareketId, bool isYoksaHataDondur = false);

        /// <summary>
        /// ID'ye göre Kamera hareket DTO getir
        /// </summary>
        KameraHareketDTO getKameraHareketDtoById(int kameraHareketId, bool isYoksaHataDondur = false);

        /// <summary>
        /// Firmaya göre Kamera hareket DTO listesi getir
        /// </summary>
        List<KameraHareketDTO> getKameraHareketDtoListByFirmaId(int firmaId, bool isYoksaHataDondur = false);

        /// <summary>
        /// Firma cihazına göre Kamera hareket DTO listesi getir
        /// </summary>
        List<KameraHareketDTO> getKameraHareketDtoListByFirmaCihazId(int firmaCihazId, bool isYoksaHataDondur = false);

        /// <summary>
        /// Kamera olay tipine göre Kamera hareket DTO listesi getir
        /// </summary>
        List<KameraHareketDTO> getKameraHareketDtoListByOlayTipi(int kameraOlayTipKid, bool isYoksaHataDondur = false);

        /// <summary>
        /// Tarih aralığına ve firmaya göre Kamera hareket DTO listesi getir
        /// </summary>
        List<KameraHareketDTO> getKameraHareketDtoListByFirmaIdAndDateRange(int firmaId, DateTime baslangicTarihi, DateTime bitisTarihi, bool isYoksaHataDondur = false);

        /// <summary>
        /// Kamera hareket kaydedilmeden önce kontrol eder
        /// </summary>
        void checkKameraHareketDtoKayitEdilebilirMi(KameraHareketDTO kameraHareketDto);
    }
}
