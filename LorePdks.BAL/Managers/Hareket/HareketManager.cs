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
using System.Transactions;

namespace LorePdks.BAL.Managers.Deneme
{
    public class HareketManager(
        IMapper _mapper,
        IKodManager _kodManager
    ) : IHareketManager
    {
        private readonly GenericRepository<t_hareket> _repoHareket = new GenericRepository<t_hareket>();

        public HareketDTO saveHareket(HareketDTO hareketDto)
        {
            // Validasyon
            checkHareketKayitEdilebilirMi(hareketDto);

            // KodDTO'ları doğrula ve ID'lerini al
            if (hareketDto.hareketTipKodDto != null)
            {
                hareketDto.hareketTipKodDto = _kodManager.GetKodDtoByKodId(hareketDto.hareketTipKodDto.id);
            }

            if (hareketDto.hareketDurumKodDto != null)
            {
                hareketDto.hareketDurumKodDto = _kodManager.GetKodDtoByKodId(hareketDto.hareketDurumKodDto.id);
            }

            // DTO'dan Entity'ye dönüşüm
            t_hareket hareket = _mapper.Map<HareketDTO, t_hareket>(hareketDto);

            // Repository ile kaydetme işlemi
            _repoHareket.Save(hareket);

            // Entity'den DTO'ya dönüşüm
            hareketDto = _mapper.Map<t_hareket, HareketDTO>(hareket);
            return hareketDto;
        }

        public List<HareketDTO> getHareketListByFirmaId(int firmaId)
        {
            var hareketList = _repoHareket.GetList("FIRMA_ID=@firmaId", new { firmaId });
            return _mapper.Map<List<t_hareket>, List<HareketDTO>>(hareketList);
        }

        public void deleteHareketById(int hareketId)
        {
            var dbHareket = getHareketByHareketId(hareketId, isYoksaHataDondur: true);

            bool isKullanilmis = false;
            // Eğer hareket kaydı başka bir yerde kullanılıyorsa burada kontrol edilebilir
            // Örneğin PdksHareket tablosunda bu hareket ID'si var mı diye kontrol edilebilir

            if (isKullanilmis)
            {
                throw new AppException(MessageCode.ERROR_500_BIR_HATA_OLUSTU, $"Kullanılmış bir hareket kaydı silinemez.");
            }

            _repoHareket.Delete(dbHareket);
        }

        public HareketDTO getHareketDtoById(int hareketId)
        {
            var hareket = getHareketByHareketId(hareketId, isYoksaHataDondur: true);
            return _mapper.Map<t_hareket, HareketDTO>(hareket);
        }

        public t_hareket getHareketByHareketId(int hareketId, bool isYoksaHataDondur = false)
        {
            var hareket = _repoHareket.Get(hareketId);

            if (isYoksaHataDondur && hareket == null)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"{hareketId} id'li hareket kaydı sistemde bulunamadı");
            }

            return hareket;
        }

        private void checkHareketKayitEdilebilirMi(HareketDTO hareketDto)
        {
            if (hareketDto.firmaDto == null || hareketDto.firmaDto.id <= 0)
            {
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, "Firma bilgisi eksik.");
            }

            if (string.IsNullOrEmpty(hareketDto.hareketdata))
            {
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, "Hareket verisi eksik.");
            }

            if (hareketDto.hareketTipKodDto == null || hareketDto.hareketTipKodDto.id <= 0)
            {
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, "Hareket tipi kodu eksik.");
            }

            if (hareketDto.hareketDurumKodDto == null || hareketDto.hareketDurumKodDto.id <= 0)
            {
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, "Hareket durumu kodu eksik.");
            }
        }
    }

}