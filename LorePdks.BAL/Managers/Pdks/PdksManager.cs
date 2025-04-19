using AutoMapper;
using LorePdks.BAL.Managers.Common.Kod.Interfaces;
using LorePdks.BAL.Managers.Deneme.Interfaces;
using LorePdks.BAL.Managers.Helper.Interfaces;
using Microsoft.AspNetCore.Http;
using LorePdks.DAL.Model;
using LorePdks.DAL.Repository;
using LorePdks.COMMON.Enums;
using LorePdks.COMMON.Models;
using Org.BouncyCastle.Asn1;
using LorePdks.COMMON.DTO.Common;
using LorePdks.COMMON.Extensions;
using System.Collections.Generic;
using LorePdks.BAL.Managers.FirmaCihaz.Interfaces;

namespace LorePdks.BAL.Managers.Pdks
{
    public class PdksManager : IPdksManager
    {
        private readonly IHelperManager _helperManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IFirmaManager _firmaManager;
        private readonly IKisiManager _kisiManager;
        private readonly IFirmaCihazManager _firmaCihazManager;
        private readonly GenericRepository<t_pdks> _repoPdks = new GenericRepository<t_pdks>();

        public PdksManager(
            IHelperManager helperManager,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IFirmaManager firmaManager,
            IKisiManager kisiManager,
            IFirmaCihazManager firmaCihazManager)
        {
            _helperManager = helperManager;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _firmaManager = firmaManager;
            _kisiManager = kisiManager;
            _firmaCihazManager = firmaCihazManager;
        }

        public PdksDTO savePdks(PdksDTO pdksDto)
        {
            bool isGuncelleniyor = false;

            if (pdksDto.id > 0) isGuncelleniyor = true;

            var dbPdks = getPdksByPdksId(pdksDto.id, isYoksaHataDondur: isGuncelleniyor);

            checkPdksDtoKayitEdilebilirMi(pdksDto);

            t_pdks pdks = _mapper.Map<PdksDTO, t_pdks>(pdksDto);

            _repoPdks.Save(pdks);

            pdksDto = _mapper.Map<t_pdks, PdksDTO>(pdks);

            return pdksDto;
        }

        public void deletePdksByPdksId(int pdksId)
        {
            var dbPdks = getPdksByPdksId(pdksId, isYoksaHataDondur: true);

            bool isKullanilmis = false;
            // Eğer PDKS kaydı başka bir yerde kullanılıyorsa burada kontrol edilebilir

            if (isKullanilmis)
            {
                throw new AppException(MessageCode.ERROR_500_BIR_HATA_OLUSTU, $"Kullanılmış bir PDKS kaydı silinemez.");
            }

            _repoPdks.Delete(dbPdks);
        }

        public t_pdks getPdksByPdksId(int pdksId, bool isYoksaHataDondur = false)
        {
            var pdks = _repoPdks.Get(pdksId);

            if (isYoksaHataDondur && pdks == null)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"{pdksId} id'li PDKS kaydı sistemde bulunamadı");
            }
            return pdks;
        }

        public PdksDTO getPdksDtoById(int pdksId, bool isYoksaHataDondur = false)
        {
            var pdks = getPdksByPdksId(pdksId, isYoksaHataDondur);

            PdksDTO pdksDto = _mapper.Map<t_pdks, PdksDTO>(pdks);

            return pdksDto;
        }

        public List<PdksDTO> getPdksDtoList(bool isYoksaHataDondur = false)
        {
            var pdksList = _repoPdks.GetList();
            if (isYoksaHataDondur && pdksList.Count <= 0)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"PDKS kaydı bulunamadı");
            }
            List<PdksDTO> pdksDtoList = _mapper.Map<List<t_pdks>, List<PdksDTO>>(pdksList);

            return pdksDtoList;
        }

        /// <summary>
        /// Bir PDKS kaydı değişikliğe uğramaya çalışırsa ya da ilk defa kayıt ediliyorsa yapılacak kontroller burada
        /// </summary>
        /// <param name="pdksDto"></param>
        public void checkPdksDtoKayitEdilebilirMi(PdksDTO pdksDto)
        {
            if (pdksDto.firmaDto == null || pdksDto.firmaDto.id <= 0)
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, $"Firma bilgisi boş olamaz");

            if (pdksDto.kisiDto == null || pdksDto.kisiDto.id <= 0)
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, $"Kişi bilgisi boş olamaz");

            if (pdksDto.firmaCihazDto == null || pdksDto.firmaCihazDto.id <= 0)
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, $"Firma cihaz bilgisi boş olamaz");

            // İlişkili varlıkların var olup olmadığını kontrol et
            _firmaManager.getFirmaByFirmaId(pdksDto.firmaDto.id, isYoksaHataDondur: true);
            _kisiManager.getKisiByKisiId(pdksDto.kisiDto.id, isYoksaHataDondur: true);
            _firmaCihazManager.getFirmaCihazByFirmaCihazId(pdksDto.firmaCihazDto.id, isYoksaHataDondur: true);

            // Eğer önceden kayıtlı ve güncelleme oluyorsa farklı kontroller yapılması gerekebilir
            if (pdksDto.id > 0)
            {
                // Önce db halini bul ve ne değiştiğini tespit et, buna göre ek kontrollerini yaz
                var dbPdks = getPdksByPdksId(pdksDto.id, isYoksaHataDondur: true);
            }
        }
    }
}