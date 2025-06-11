using LorePdks.COMMON.DTO.PdksHareket;
using LorePdks.DAL.Model;
using System;
using System.Collections.Generic;

namespace LorePdks.BAL.Managers.Pdks.Interfaces
{
    /// <summary>
    /// PDKS Hareket Manager Interface - Yeni merkezi log sistemi için
    /// Repo pattern kuralları: Save, Get, GetList, Delete metodları
    /// </summary>
    public interface IPdksHareketManager
    {
        /// <summary>
        /// PDKS hareket kaydı kaydet/güncelle (ID = 0 ise INSERT, > 0 ise UPDATE)
        /// </summary>
        PdksHareketDTO savePdksHareket(PdksHareketDTO pdksHareketDto);

        /// <summary>
        /// ID'ye göre PDKS hareket kaydı sil (Soft delete - ISDELETED = 1)
        /// </summary>
        void deletePdksHareketById(int pdksHareketId);

        /// <summary>
        /// ID'ye göre PDKS hareket entity getir
        /// </summary>
        t_pdks_hareket getPdksHareketById(int pdksHareketId, bool isYoksaHataDondur = false);

        /// <summary>
        /// ID'ye göre PDKS hareket DTO getir
        /// </summary>
        PdksHareketDTO getPdksHareketDtoById(int pdksHareketId, bool isYoksaHataDondur = false);

        /// <summary>
        /// Firmaya göre PDKS hareket DTO listesi getir
        /// </summary>
        List<PdksHareketDTO> getPdksHareketDtoListByFirmaId(int firmaId, bool isYoksaHataDondur = false);

        /// <summary>
        /// Kişiye göre PDKS hareket DTO listesi getir
        /// </summary>
        List<PdksHareketDTO> getPdksHareketDtoListByKisiId(int kisiId, bool isYoksaHataDondur = false);

        /// <summary>
        /// Firma cihazına göre PDKS hareket DTO listesi getir
        /// </summary>
        List<PdksHareketDTO> getPdksHareketDtoListByFirmaCihazId(int firmaCihazId, bool isYoksaHataDondur = false);

        /// <summary>
        /// Tarih aralığına ve firmaya göre PDKS hareket DTO listesi getir
        /// </summary>
        List<PdksHareketDTO> getPdksHareketDtoListByFirmaIdAndDateRange(int firmaId, DateTime baslangicTarihi, DateTime bitisTarihi, bool isYoksaHataDondur = false);

        /// <summary>
        /// PDKS hareket kaydedilmeden önce kontrol eder
        /// </summary>
        void checkPdksHareketDtoKayitEdilebilirMi(PdksHareketDTO pdksHareketDto);
    }
}