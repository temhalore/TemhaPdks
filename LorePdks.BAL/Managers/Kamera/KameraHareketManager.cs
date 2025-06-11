using AutoMapper;
using LorePdks.BAL.Managers.Common.Kod.Interfaces;
using LorePdks.BAL.Managers.Helper.Interfaces;
using LorePdks.BAL.Managers.Firma.Interfaces;
using LorePdks.BAL.Managers.FirmaCihaz.Interfaces;
using LorePdks.BAL.Managers.Kamera.Interfaces;
using Microsoft.AspNetCore.Http;
using LorePdks.DAL.Model;
using LorePdks.DAL.Repository;
using LorePdks.COMMON.Enums;
using LorePdks.COMMON.Models;
using LorePdks.COMMON.Extensions;
using LorePdks.COMMON.DTO.KameraHareket;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LorePdks.BAL.Managers.Kamera
{
    /// <summary>
    /// Kamera Hareket Manager - Yeni merkezi log sistemi için
    /// </summary>
    public class KameraHareketManager : IKameraHareketManager
    {
        private readonly IHelperManager _helperManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IKodManager _kodManager;
        private readonly IFirmaManager _firmaManager;
        private readonly IFirmaCihazManager _firmaCihazManager;
        private readonly GenericRepository<t_kamera_hareket> _repoKameraHareket = new GenericRepository<t_kamera_hareket>();

        public KameraHareketManager(
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

        public KameraHareketDTO saveKameraHareket(KameraHareketDTO kameraHareketDto)
        {
            bool isGuncelleniyor = false;

            if (kameraHareketDto.id > 0) isGuncelleniyor = true;

            var dbKameraHareket = getKameraHareketById(kameraHareketDto.id, isYoksaHataDondur: isGuncelleniyor);

            checkKameraHareketDtoKayitEdilebilirMi(kameraHareketDto);

            t_kamera_hareket kameraHareket = _mapper.Map<KameraHareketDTO, t_kamera_hareket>(kameraHareketDto);

            _repoKameraHareket.Save(kameraHareket);

            kameraHareketDto = _mapper.Map<t_kamera_hareket, KameraHareketDTO>(kameraHareket);

            return kameraHareketDto;
        }

        public void deleteKameraHareketById(int kameraHareketId)
        {
            var dbKameraHareket = getKameraHareketById(kameraHareketId, isYoksaHataDondur: true);

            bool isKullanilmis = false;
            // Eğer Kamera Hareket kaydı başka bir yerde kullanılıyorsa burada kontrol edilebilir

            if (isKullanilmis)
            {
                throw new AppException(MessageCode.ERROR_500_BIR_HATA_OLUSTU, $"Kullanılmış bir Kamera Hareket kaydı silinemez.");
            }

            _repoKameraHareket.Delete(dbKameraHareket);
        }

        public t_kamera_hareket getKameraHareketById(int kameraHareketId, bool isYoksaHataDondur = false)
        {
            var kameraHareket = _repoKameraHareket.Get(kameraHareketId);

            if (isYoksaHataDondur && kameraHareket == null)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"{kameraHareketId} id'li Kamera Hareket kaydı sistemde bulunamadı");
            }
            return kameraHareket;
        }

        public KameraHareketDTO getKameraHareketDtoById(int kameraHareketId, bool isYoksaHataDondur = false)
        {
            var kameraHareket = getKameraHareketById(kameraHareketId, isYoksaHataDondur);

            KameraHareketDTO kameraHareketDto = _mapper.Map<t_kamera_hareket, KameraHareketDTO>(kameraHareket);

            return kameraHareketDto;
        }

        public List<KameraHareketDTO> getKameraHareketDtoListByFirmaId(int firmaId, bool isYoksaHataDondur = false)
        {
            var kameraHareketList = _repoKameraHareket.GetList("FIRMA_ID=@firmaId", new { firmaId });
            if (isYoksaHataDondur && kameraHareketList.Count <= 0)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"Firma ID {firmaId} için Kamera Hareket kaydı bulunamadı");
            }
            List<KameraHareketDTO> kameraHareketDtoList = _mapper.Map<List<t_kamera_hareket>, List<KameraHareketDTO>>(kameraHareketList);

            return kameraHareketDtoList;
        }

        public List<KameraHareketDTO> getKameraHareketDtoListByFirmaCihazId(int firmaCihazId, bool isYoksaHataDondur = false)
        {
            var kameraHareketList = _repoKameraHareket.GetList("FIRMA_CIHAZ_ID=@firmaCihazId", new { firmaCihazId });
            if (isYoksaHataDondur && kameraHareketList.Count <= 0)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"Firma Cihaz ID {firmaCihazId} için Kamera Hareket kaydı bulunamadı");
            }
            List<KameraHareketDTO> kameraHareketDtoList = _mapper.Map<List<t_kamera_hareket>, List<KameraHareketDTO>>(kameraHareketList);

            return kameraHareketDtoList;
        }

        public List<KameraHareketDTO> getKameraHareketDtoListByOlayTipi(int kameraOlayTipKid, bool isYoksaHataDondur = false)
        {
            var kameraHareketList = _repoKameraHareket.GetList("KAMERA_OLAY_TIP_KID=@kameraOlayTipKid", new { kameraOlayTipKid });
            if (isYoksaHataDondur && kameraHareketList.Count <= 0)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"Kamera Olay Tip KID {kameraOlayTipKid} için Kamera Hareket kaydı bulunamadı");
            }
            List<KameraHareketDTO> kameraHareketDtoList = _mapper.Map<List<t_kamera_hareket>, List<KameraHareketDTO>>(kameraHareketList);

            return kameraHareketDtoList;
        }

        public List<KameraHareketDTO> getKameraHareketDtoListByFirmaIdAndDateRange(int firmaId, DateTime baslangicTarihi, DateTime bitisTarihi, bool isYoksaHataDondur = false)
        {
            var kameraHareketList = _repoKameraHareket.GetList("FIRMA_ID=@firmaId AND OLAY_TARIHI >= @baslangicTarihi AND OLAY_TARIHI <= @bitisTarihi", 
                new { firmaId, baslangicTarihi, bitisTarihi });
            if (isYoksaHataDondur && kameraHareketList.Count <= 0)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"Belirtilen tarih aralığında Firma ID {firmaId} için Kamera Hareket kaydı bulunamadı");
            }
            List<KameraHareketDTO> kameraHareketDtoList = _mapper.Map<List<t_kamera_hareket>, List<KameraHareketDTO>>(kameraHareketList);

            return kameraHareketDtoList;
        }

        /// <summary>
        /// Bir Kamera Hareket kaydı değişikliğe uğramaya çalışırsa ya da ilk defa kayıt ediliyorsa yapılacak kontroller burada
        /// </summary>
        /// <param name="kameraHareketDto"></param>
        public void checkKameraHareketDtoKayitEdilebilirMi(KameraHareketDTO kameraHareketDto)
        {
            if (kameraHareketDto.firmaDto == null || kameraHareketDto.firmaDto.id <= 0)
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, $"Firma bilgisi boş olamaz");

            if (kameraHareketDto.firmaCihazDto == null || kameraHareketDto.firmaCihazDto.id <= 0)
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, $"Firma cihaz bilgisi boş olamaz");

            if (kameraHareketDto.olayTarihi == DateTime.MinValue)
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, $"Olay tarihi boş olamaz");

            if (kameraHareketDto.kameraOlayTipKodDto == null || kameraHareketDto.kameraOlayTipKodDto.id <= 0)
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, $"Kamera olay tip kodu boş olamaz");

            // İlişkili varlıkların var olup olmadığını kontrol et
            _firmaManager.getFirmaByFirmaId(kameraHareketDto.firmaDto.id, isYoksaHataDondur: true);
            _firmaCihazManager.getFirmaCihazByFirmaCihazId(kameraHareketDto.firmaCihazDto.id, isYoksaHataDondur: true);

            // Kamera Olay Tip kodunu kontrol et
            _kodManager.checkKodDTOIdInTipList(kameraHareketDto.kameraOlayTipKodDto, AppEnums.KodTipList.KAMERA_OLAY_TIPI);

            // Eğer önceden kayıtlı ve güncelleme oluyorsa farklı kontroller yapılması gerekebilir
            if (kameraHareketDto.id > 0)
            {
                // Önce db halini bul ve ne değiştiğini tespit et, buna göre ek kontrollerini yaz
                var dbKameraHareket = getKameraHareketById(kameraHareketDto.id, isYoksaHataDondur: true);
            }
        }
    }
}
