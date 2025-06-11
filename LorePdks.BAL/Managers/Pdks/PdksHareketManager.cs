using AutoMapper;
using LorePdks.BAL.Managers.Common.Kod.Interfaces;
using LorePdks.BAL.Managers.Helper.Interfaces;
using LorePdks.BAL.Managers.Firma.Interfaces;
using LorePdks.BAL.Managers.Kisi.Interfaces;
using LorePdks.BAL.Managers.FirmaCihaz.Interfaces;
using LorePdks.BAL.Managers.Pdks.Interfaces;
using Microsoft.AspNetCore.Http;
using LorePdks.DAL.Model;
using LorePdks.DAL.Repository;
using LorePdks.COMMON.Enums;
using LorePdks.COMMON.Models;
using LorePdks.COMMON.Extensions;
using LorePdks.COMMON.DTO.PdksHareket;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LorePdks.BAL.Managers.Pdks
{
    /// <summary>
    /// PDKS Hareket Manager - Yeni merkezi log sistemi için
    /// </summary>
    public class PdksHareketManager : IPdksHareketManager
    {
        private readonly IHelperManager _helperManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IKodManager _kodManager;
        private readonly IFirmaManager _firmaManager;
        private readonly IKisiManager _kisiManager;
        private readonly IFirmaCihazManager _firmaCihazManager;
        private readonly GenericRepository<t_pdks_hareket> _repoPdksHareket = new GenericRepository<t_pdks_hareket>();

        public PdksHareketManager(
            IHelperManager helperManager,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IKodManager kodManager,
            IFirmaManager firmaManager,
            IKisiManager kisiManager,
            IFirmaCihazManager firmaCihazManager)
        {
            _helperManager = helperManager;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _kodManager = kodManager;
            _firmaManager = firmaManager;
            _kisiManager = kisiManager;
            _firmaCihazManager = firmaCihazManager;
        }

        public PdksHareketDTO savePdksHareket(PdksHareketDTO pdksHareketDto)
        {
            bool isGuncelleniyor = false;

            if (pdksHareketDto.id > 0) isGuncelleniyor = true;

            var dbPdksHareket = getPdksHareketById(pdksHareketDto.id, isYoksaHataDondur: isGuncelleniyor);

            checkPdksHareketDtoKayitEdilebilirMi(pdksHareketDto);

            t_pdks_hareket pdksHareket = _mapper.Map<PdksHareketDTO, t_pdks_hareket>(pdksHareketDto);

            _repoPdksHareket.Save(pdksHareket);

            pdksHareketDto = _mapper.Map<t_pdks_hareket, PdksHareketDTO>(pdksHareket);

            return pdksHareketDto;
        }

        public void deletePdksHareketById(int pdksHareketId)
        {
            var dbPdksHareket = getPdksHareketById(pdksHareketId, isYoksaHataDondur: true);

            bool isKullanilmis = false;
            // Eğer PDKS Hareket kaydı başka bir yerde kullanılıyorsa burada kontrol edilebilir

            if (isKullanilmis)
            {
                throw new AppException(MessageCode.ERROR_500_BIR_HATA_OLUSTU, $"Kullanılmış bir PDKS Hareket kaydı silinemez.");
            }

            _repoPdksHareket.Delete(dbPdksHareket);
        }

        public t_pdks_hareket getPdksHareketById(int pdksHareketId, bool isYoksaHataDondur = false)
        {
            var pdksHareket = _repoPdksHareket.Get(pdksHareketId);

            if (isYoksaHataDondur && pdksHareket == null)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"{pdksHareketId} id'li PDKS Hareket kaydı sistemde bulunamadı");
            }
            return pdksHareket;
        }

        public PdksHareketDTO getPdksHareketDtoById(int pdksHareketId, bool isYoksaHataDondur = false)
        {
            var pdksHareket = getPdksHareketById(pdksHareketId, isYoksaHataDondur);

            PdksHareketDTO pdksHareketDto = _mapper.Map<t_pdks_hareket, PdksHareketDTO>(pdksHareket);

            return pdksHareketDto;
        }

        public List<PdksHareketDTO> getPdksHareketDtoListByFirmaId(int firmaId, bool isYoksaHataDondur = false)
        {
            var pdksHareketList = _repoPdksHareket.GetList("FIRMA_ID=@firmaId", new { firmaId });
            if (isYoksaHataDondur && pdksHareketList.Count <= 0)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"Firma ID {firmaId} için PDKS Hareket kaydı bulunamadı");
            }
            List<PdksHareketDTO> pdksHareketDtoList = _mapper.Map<List<t_pdks_hareket>, List<PdksHareketDTO>>(pdksHareketList);

            return pdksHareketDtoList;
        }

        public List<PdksHareketDTO> getPdksHareketDtoListByKisiId(int kisiId, bool isYoksaHataDondur = false)
        {
            var pdksHareketList = _repoPdksHareket.GetList("KISI_ID=@kisiId", new { kisiId });
            if (isYoksaHataDondur && pdksHareketList.Count <= 0)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"Kişi ID {kisiId} için PDKS Hareket kaydı bulunamadı");
            }
            List<PdksHareketDTO> pdksHareketDtoList = _mapper.Map<List<t_pdks_hareket>, List<PdksHareketDTO>>(pdksHareketList);

            return pdksHareketDtoList;
        }

        public List<PdksHareketDTO> getPdksHareketDtoListByFirmaCihazId(int firmaCihazId, bool isYoksaHataDondur = false)
        {
            var pdksHareketList = _repoPdksHareket.GetList("FIRMA_CIHAZ_ID=@firmaCihazId", new { firmaCihazId });
            if (isYoksaHataDondur && pdksHareketList.Count <= 0)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"Firma Cihaz ID {firmaCihazId} için PDKS Hareket kaydı bulunamadı");
            }
            List<PdksHareketDTO> pdksHareketDtoList = _mapper.Map<List<t_pdks_hareket>, List<PdksHareketDTO>>(pdksHareketList);

            return pdksHareketDtoList;
        }

        public List<PdksHareketDTO> getPdksHareketDtoListByFirmaIdAndDateRange(int firmaId, DateTime baslangicTarihi, DateTime bitisTarihi, bool isYoksaHataDondur = false)
        {
            var pdksHareketList = _repoPdksHareket.GetList("FIRMA_ID=@firmaId AND HAREKET_TARIHI >= @baslangicTarihi AND HAREKET_TARIHI <= @bitisTarihi", 
                new { firmaId, baslangicTarihi, bitisTarihi });
            if (isYoksaHataDondur && pdksHareketList.Count <= 0)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"Belirtilen tarih aralığında Firma ID {firmaId} için PDKS Hareket kaydı bulunamadı");
            }
            List<PdksHareketDTO> pdksHareketDtoList = _mapper.Map<List<t_pdks_hareket>, List<PdksHareketDTO>>(pdksHareketList);

            return pdksHareketDtoList;
        }

        /// <summary>
        /// Bir PDKS Hareket kaydı değişikliğe uğramaya çalışırsa ya da ilk defa kayıt ediliyorsa yapılacak kontroller burada
        /// </summary>
        /// <param name="pdksHareketDto"></param>
        public void checkPdksHareketDtoKayitEdilebilirMi(PdksHareketDTO pdksHareketDto)
        {
            if (pdksHareketDto.firmaDto == null || pdksHareketDto.firmaDto.id <= 0)
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, $"Firma bilgisi boş olamaz");

            if (pdksHareketDto.firmaCihazDto == null || pdksHareketDto.firmaCihazDto.id <= 0)
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, $"Firma cihaz bilgisi boş olamaz");

            if (pdksHareketDto.hareketTarihi == DateTime.MinValue)
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, $"Hareket tarihi boş olamaz");

            if (pdksHareketDto.pdksYonKodDto == null || pdksHareketDto.pdksYonKodDto.id <= 0)
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, $"PDKS yön kodu boş olamaz");

            // İlişkili varlıkların var olup olmadığını kontrol et
            _firmaManager.getFirmaByFirmaId(pdksHareketDto.firmaDto.id, isYoksaHataDondur: true);
            _firmaCihazManager.getFirmaCihazByFirmaCihazId(pdksHareketDto.firmaCihazDto.id, isYoksaHataDondur: true);
            
            // Kişi bilgisi opsiyonel olabilir
            if (pdksHareketDto.kisiDto != null && pdksHareketDto.kisiDto.id > 0)
            {
                _kisiManager.getKisiByKisiId(pdksHareketDto.kisiDto.id, isYoksaHataDondur: true);
            }

            // PDKS Yön kodunu kontrol et
            _kodManager.checkKodDTOIdInTipList(pdksHareketDto.pdksYonKodDto, AppEnums.KodTipList.PDKS_YON);

            // Eğer önceden kayıtlı ve güncelleme oluyorsa farklı kontroller yapılması gerekebilir
            if (pdksHareketDto.id > 0)
            {
                // Önce db halini bul ve ne değiştiğini tespit et, buna göre ek kontrollerini yaz
                var dbPdksHareket = getPdksHareketById(pdksHareketDto.id, isYoksaHataDondur: true);
            }
        }
    }
}