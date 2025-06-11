using AutoMapper;
using LorePdks.BAL.Managers.Common.Kod.Interfaces;
using LorePdks.BAL.Managers.Helper.Interfaces;
using LorePdks.BAL.Managers.Firma.Interfaces;
using LorePdks.BAL.Managers.FirmaCihaz.Interfaces;
using LorePdks.BAL.Managers.Alarm.Interfaces;
using Microsoft.AspNetCore.Http;
using LorePdks.DAL.Model;
using LorePdks.DAL.Repository;
using LorePdks.COMMON.Enums;
using LorePdks.COMMON.Models;
using LorePdks.COMMON.Extensions;
using LorePdks.COMMON.DTO.AlarmHareket;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LorePdks.BAL.Managers.Alarm
{
    /// <summary>
    /// Alarm Hareket Manager - Yeni merkezi log sistemi için
    /// </summary>
    public class AlarmHareketManager : IAlarmHareketManager
    {
        private readonly IHelperManager _helperManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IKodManager _kodManager;
        private readonly IFirmaManager _firmaManager;
        private readonly IFirmaCihazManager _firmaCihazManager;
        private readonly GenericRepository<t_alarm_hareket> _repoAlarmHareket = new GenericRepository<t_alarm_hareket>();

        public AlarmHareketManager(
            IHelperManager helperManager,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IKodManager kodManager,
            IFirmaManager firmaManager,
            IFirmaCihazManager firmaCihazManager)
        {
            _helperManager = helperManager;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _kodManager = kodManager;
            _firmaManager = firmaManager;
            _firmaCihazManager = firmaCihazManager;
        }

        public AlarmHareketDTO saveAlarmHareket(AlarmHareketDTO alarmHareketDto)
        {
            bool isGuncelleniyor = false;

            if (alarmHareketDto.id > 0) isGuncelleniyor = true;

            var dbAlarmHareket = getAlarmHareketById(alarmHareketDto.id, isYoksaHataDondur: isGuncelleniyor);

            checkAlarmHareketDtoKayitEdilebilirMi(alarmHareketDto);

            t_alarm_hareket alarmHareket = _mapper.Map<AlarmHareketDTO, t_alarm_hareket>(alarmHareketDto);

            _repoAlarmHareket.Save(alarmHareket);

            alarmHareketDto = _mapper.Map<t_alarm_hareket, AlarmHareketDTO>(alarmHareket);

            return alarmHareketDto;
        }

        public void deleteAlarmHareketById(int alarmHareketId)
        {
            var dbAlarmHareket = getAlarmHareketById(alarmHareketId, isYoksaHataDondur: true);

            bool isKullanilmis = false;
            // Eğer Alarm Hareket kaydı başka bir yerde kullanılıyorsa burada kontrol edilebilir

            if (isKullanilmis)
            {
                throw new AppException(MessageCode.ERROR_500_BIR_HATA_OLUSTU, $"Kullanılmış bir Alarm Hareket kaydı silinemez.");
            }

            _repoAlarmHareket.Delete(dbAlarmHareket);
        }

        public t_alarm_hareket getAlarmHareketById(int alarmHareketId, bool isYoksaHataDondur = false)
        {
            var alarmHareket = _repoAlarmHareket.Get(alarmHareketId);

            if (isYoksaHataDondur && alarmHareket == null)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"{alarmHareketId} id'li Alarm Hareket kaydı sistemde bulunamadı");
            }
            return alarmHareket;
        }

        public AlarmHareketDTO getAlarmHareketDtoById(int alarmHareketId, bool isYoksaHataDondur = false)
        {
            var alarmHareket = getAlarmHareketById(alarmHareketId, isYoksaHataDondur);

            AlarmHareketDTO alarmHareketDto = _mapper.Map<t_alarm_hareket, AlarmHareketDTO>(alarmHareket);

            return alarmHareketDto;
        }

        public List<AlarmHareketDTO> getAlarmHareketDtoListByFirmaId(int firmaId, bool isYoksaHataDondur = false)
        {
            var alarmHareketList = _repoAlarmHareket.GetList("FIRMA_ID=@firmaId", new { firmaId });
            if (isYoksaHataDondur && alarmHareketList.Count <= 0)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"Firma ID {firmaId} için Alarm Hareket kaydı bulunamadı");
            }
            List<AlarmHareketDTO> alarmHareketDtoList = _mapper.Map<List<t_alarm_hareket>, List<AlarmHareketDTO>>(alarmHareketList);

            return alarmHareketDtoList;
        }

        public List<AlarmHareketDTO> getAlarmHareketDtoListByFirmaCihazId(int firmaCihazId, bool isYoksaHataDondur = false)
        {
            var alarmHareketList = _repoAlarmHareket.GetList("FIRMA_CIHAZ_ID=@firmaCihazId", new { firmaCihazId });
            if (isYoksaHataDondur && alarmHareketList.Count <= 0)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"Firma Cihaz ID {firmaCihazId} için Alarm Hareket kaydı bulunamadı");
            }
            List<AlarmHareketDTO> alarmHareketDtoList = _mapper.Map<List<t_alarm_hareket>, List<AlarmHareketDTO>>(alarmHareketList);

            return alarmHareketDtoList;
        }

        public List<AlarmHareketDTO> getAlarmHareketDtoListByAlarmTipi(int alarmTipKid, bool isYoksaHataDondur = false)
        {
            var alarmHareketList = _repoAlarmHareket.GetList("ALARM_TIP_KID=@alarmTipKid", new { alarmTipKid });
            if (isYoksaHataDondur && alarmHareketList.Count <= 0)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"Alarm Tip KID {alarmTipKid} için Alarm Hareket kaydı bulunamadı");
            }
            List<AlarmHareketDTO> alarmHareketDtoList = _mapper.Map<List<t_alarm_hareket>, List<AlarmHareketDTO>>(alarmHareketList);

            return alarmHareketDtoList;
        }

        public List<AlarmHareketDTO> getAlarmHareketDtoListByFirmaIdAndDateRange(int firmaId, DateTime baslangicTarihi, DateTime bitisTarihi, bool isYoksaHataDondur = false)
        {
            var alarmHareketList = _repoAlarmHareket.GetList("FIRMA_ID=@firmaId AND ALARM_TARIHI >= @baslangicTarihi AND ALARM_TARIHI <= @bitisTarihi", 
                new { firmaId, baslangicTarihi, bitisTarihi });
            if (isYoksaHataDondur && alarmHareketList.Count <= 0)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"Belirtilen tarih aralığında Firma ID {firmaId} için Alarm Hareket kaydı bulunamadı");
            }
            List<AlarmHareketDTO> alarmHareketDtoList = _mapper.Map<List<t_alarm_hareket>, List<AlarmHareketDTO>>(alarmHareketList);

            return alarmHareketDtoList;
        }

        /// <summary>
        /// Bir Alarm Hareket kaydı değişikliğe uğramaya çalışırsa ya da ilk defa kayıt ediliyorsa yapılacak kontroller burada
        /// </summary>
        /// <param name="alarmHareketDto"></param>
        public void checkAlarmHareketDtoKayitEdilebilirMi(AlarmHareketDTO alarmHareketDto)
        {
            if (alarmHareketDto.firmaDto == null || alarmHareketDto.firmaDto.id <= 0)
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, $"Firma bilgisi boş olamaz");

            if (alarmHareketDto.firmaCihazDto == null || alarmHareketDto.firmaCihazDto.id <= 0)
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, $"Firma cihaz bilgisi boş olamaz");

            if (alarmHareketDto.alarmTarihi == DateTime.MinValue)
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, $"Alarm tarihi boş olamaz");

            if (alarmHareketDto.alarmTipKodDto == null || alarmHareketDto.alarmTipKodDto.id <= 0)
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, $"Alarm tip kodu boş olamaz");

            // İlişkili varlıkların var olup olmadığını kontrol et
            _firmaManager.getFirmaByFirmaId(alarmHareketDto.firmaDto.id, isYoksaHataDondur: true);
            _firmaCihazManager.getFirmaCihazByFirmaCihazId(alarmHareketDto.firmaCihazDto.id, isYoksaHataDondur: true);

            // Alarm Tip kodunu kontrol et
            _kodManager.checkKodDTOIdInTipList(alarmHareketDto.alarmTipKodDto, AppEnums.KodTipList.ALARM_TIPI);

            // Alarm Seviye kodu opsiyonel ama varsa kontrol et
            if (alarmHareketDto.alarmSeviyeKodDto != null && alarmHareketDto.alarmSeviyeKodDto.id > 0)
            {
                _kodManager.checkKodDTOIdInTipList(alarmHareketDto.alarmSeviyeKodDto, AppEnums.KodTipList.ALARM_SEVIYESI);
            }

            // Eğer önceden kayıtlı ve güncelleme oluyorsa farklı kontroller yapılması gerekebilir
            if (alarmHareketDto.id > 0)
            {
                // Önce db halini bul ve ne değiştiğini tespit et, buna göre ek kontrollerini yaz
                var dbAlarmHareket = getAlarmHareketById(alarmHareketDto.id, isYoksaHataDondur: true);
            }
        }
    }
}
