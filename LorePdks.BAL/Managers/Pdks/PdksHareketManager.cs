using AutoMapper;
using LorePdks.BAL.Managers.Common.Kod.Interfaces;
using LorePdks.BAL.Managers.Helper.Interfaces;
using Microsoft.AspNetCore.Http;
using LorePdks.DAL.Model;
using LorePdks.DAL.Repository;
using LorePdks.COMMON.Enums;
using LorePdks.COMMON.Models;
using Org.BouncyCastle.Asn1;
using LorePdks.COMMON.Extensions;
using System.Collections.Generic;
using LorePdks.BAL.Managers.FirmaCihaz.Interfaces;
using LorePdks.COMMON.DTO.Pdks;
using LorePdks.BAL.Managers.Pdks.Interfaces;
using LorePdks.BAL.Managers.Hareket.Interfaces;

namespace LorePdks.BAL.Managers.Pdks
{
    public class PdksHareketManager : IPdksHareketManager
    {
        private readonly IHelperManager _helperManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IPdksManager _pdksManager;
        private readonly IHareketManager _hareketManager;
        private readonly GenericRepository<t_pdks_hareket> _repoPdksHareket = new GenericRepository<t_pdks_hareket>();

        public PdksHareketManager(
            IHelperManager helperManager,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IPdksManager pdksManager,
            IHareketManager hareketManager)
        {
            _helperManager = helperManager;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _pdksManager = pdksManager;
            _hareketManager = hareketManager;
        }

        public PdksHareketDTO savePdksHareket(PdksHareketDTO pdksHareketDto)
        {
            bool isGuncelleniyor = false;

            if (pdksHareketDto.id > 0) isGuncelleniyor = true;

            var dbPdksHareket = getPdksHareketByPdksHareketId(pdksHareketDto.id, isYoksaHataDondur: isGuncelleniyor);

            checkPdksHareketDtoKayitEdilebilirMi(pdksHareketDto);

            t_pdks_hareket pdksHareket = _mapper.Map<PdksHareketDTO, t_pdks_hareket>(pdksHareketDto);

            _repoPdksHareket.Save(pdksHareket);

            pdksHareketDto = _mapper.Map<t_pdks_hareket, PdksHareketDTO>(pdksHareket);

            return pdksHareketDto;
        }

        public void deletePdksHareketByPdksHareketId(int pdksHareketId)
        {
            var dbPdksHareket = getPdksHareketByPdksHareketId(pdksHareketId, isYoksaHataDondur: true);

            bool isKullanilmis = false;
            // Eğer PDKS Hareket kaydı başka bir yerde kullanılıyorsa burada kontrol edilebilir

            if (isKullanilmis)
            {
                throw new AppException(MessageCode.ERROR_500_BIR_HATA_OLUSTU, $"Kullanılmış bir PDKS Hareket kaydı silinemez.");
            }

            _repoPdksHareket.Delete(dbPdksHareket);
        }

        public t_pdks_hareket getPdksHareketByPdksHareketId(int pdksHareketId, bool isYoksaHataDondur = false)
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
            var pdksHareket = getPdksHareketByPdksHareketId(pdksHareketId, isYoksaHataDondur);

            PdksHareketDTO pdksHareketDto = _mapper.Map<t_pdks_hareket, PdksHareketDTO>(pdksHareket);

            return pdksHareketDto;
        }

        public List<PdksHareketDTO> getPdksHareketDtoList(bool isYoksaHataDondur = false)
        {
            var pdksHareketList = _repoPdksHareket.GetList();
            if (isYoksaHataDondur && pdksHareketList.Count <= 0)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"PDKS Hareket kaydı bulunamadı");
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
            if (pdksHareketDto.pdksDto == null || pdksHareketDto.pdksDto.id <= 0)
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, $"PDKS bilgisi boş olamaz");

            if (pdksHareketDto.hareketDto == null || pdksHareketDto.hareketDto.id <= 0)
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, $"Hareket bilgisi boş olamaz");

            // İlişkili varlıkların var olup olmadığını kontrol et
            _pdksManager.getPdksByPdksId(pdksHareketDto.pdksDto.id, isYoksaHataDondur: true);
            //_hareketManager.getHareketByHareketId(pdksHareketDto.hareketDto.id, isYoksaHataDondur: true);

            // Eğer önceden kayıtlı ve güncelleme oluyorsa farklı kontroller yapılması gerekebilir
            if (pdksHareketDto.id > 0)
            {
                // Önce db halini bul ve ne değiştiğini tespit et, buna göre ek kontrollerini yaz
                var dbPdksHareket = getPdksHareketByPdksHareketId(pdksHareketDto.id, isYoksaHataDondur: true);
            }
        }
    }
}