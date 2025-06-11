using LorePdks.COMMON.DTO.AlarmHareket;
using LorePdks.DAL.Model;
using System;
using System.Collections.Generic;

namespace LorePdks.BAL.Managers.Alarm.Interfaces
{
    /// <summary>
    /// Alarm Hareket Manager Interface - Yeni merkezi log sistemi için
    /// Repo pattern kuralları: Save, Get, GetList, Delete metodları
    /// </summary>
    public interface IAlarmHareketManager
    {
        /// <summary>
        /// Alarm hareket kaydı kaydet/güncelle (ID = 0 ise INSERT, > 0 ise UPDATE)
        /// </summary>
        AlarmHareketDTO saveAlarmHareket(AlarmHareketDTO alarmHareketDto);

        /// <summary>
        /// ID'ye göre Alarm hareket kaydı sil (Soft delete - ISDELETED = 1)
        /// </summary>
        void deleteAlarmHareketById(int alarmHareketId);

        /// <summary>
        /// ID'ye göre Alarm hareket entity getir
        /// </summary>
        t_alarm_hareket getAlarmHareketById(int alarmHareketId, bool isYoksaHataDondur = false);

        /// <summary>
        /// ID'ye göre Alarm hareket DTO getir
        /// </summary>
        AlarmHareketDTO getAlarmHareketDtoById(int alarmHareketId, bool isYoksaHataDondur = false);

        /// <summary>
        /// Firmaya göre Alarm hareket DTO listesi getir
        /// </summary>
        List<AlarmHareketDTO> getAlarmHareketDtoListByFirmaId(int firmaId, bool isYoksaHataDondur = false);

        /// <summary>
        /// Firma cihazına göre Alarm hareket DTO listesi getir
        /// </summary>
        List<AlarmHareketDTO> getAlarmHareketDtoListByFirmaCihazId(int firmaCihazId, bool isYoksaHataDondur = false);

        /// <summary>
        /// Alarm tipine göre Alarm hareket DTO listesi getir
        /// </summary>
        List<AlarmHareketDTO> getAlarmHareketDtoListByAlarmTipi(int alarmTipKid, bool isYoksaHataDondur = false);

        /// <summary>
        /// Tarih aralığına ve firmaya göre Alarm hareket DTO listesi getir
        /// </summary>
        List<AlarmHareketDTO> getAlarmHareketDtoListByFirmaIdAndDateRange(int firmaId, DateTime baslangicTarihi, DateTime bitisTarihi, bool isYoksaHataDondur = false);

        /// <summary>
        /// Alarm hareket kaydedilmeden önce kontrol eder
        /// </summary>
        void checkAlarmHareketDtoKayitEdilebilirMi(AlarmHareketDTO alarmHareketDto);
    }
}
