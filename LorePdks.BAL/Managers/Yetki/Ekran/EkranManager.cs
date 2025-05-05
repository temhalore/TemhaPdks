using LorePdks.DAL.Model;
using LorePdks.DAL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using LorePdks.COMMON.Models;
using LorePdks.COMMON.Enums;
using LorePdks.COMMON.DTO.Yetki.Ekran;
using LorePdks.BAL.Managers.Yetki.Ekran.Interfaces;

namespace LorePdks.BAL.Managers.Yetki.Ekran
{
    public class EkranManager : IEkranManager
    {
        private readonly IMapper _mapper;
        private readonly GenericRepository<t_ekran> _repoEkran = new GenericRepository<t_ekran>();
        private readonly GenericRepository<t_rol_ekran> _repoRolEkran = new GenericRepository<t_rol_ekran>();
        private readonly GenericRepository<t_kisi_rol> _repoKisiRol = new GenericRepository<t_kisi_rol>();

        public EkranManager(IMapper mapper)
        {
            _mapper = mapper;
        }

        public EkranDTO saveEkran(EkranDTO ekranDTO)
        {
            bool isGuncelleniyor = false;

            if (ekranDTO.id > 0) isGuncelleniyor = true;

            var dbEkran = getEkranByEkranId(ekranDTO.id, isYoksaHataDondur: isGuncelleniyor);

            // Burada ekran için gerekli kontrolleri yapabiliriz
            checkEkranDtoKayitEdilebilirMi(ekranDTO);

            t_ekran ekran = _mapper.Map<t_ekran>(ekranDTO);

            if (!isGuncelleniyor)
            {
                ekran.CREATEDDATE = DateTime.Now;
                ekran.ISDELETED = 0;
            }
            else
            {
                ekran.MODIFIEDDATE = DateTime.Now;
            }

            _repoEkran.Save(ekran);
            return _mapper.Map<EkranDTO>(ekran);
        }

        public void deleteEkranByEkranId(int ekranId)
        {
            var dbEkran = getEkranByEkranId(ekranId, isYoksaHataDondur: true);

            // Alt ekranları ve bağlı rol-ekran ilişkilerini kontrol etmek gerekebilir
            bool isKullanilmis = false;
            var rolEkranlar = _repoRolEkran.GetList("EKRAN_ID=@ekranId", new { ekranId });

            if (rolEkranlar != null && rolEkranlar.Any())
            {
                isKullanilmis = true;
            }

            // Alt ekranları kontrol et
            var altEkranlar = _repoEkran.GetList("UST_EKRAN_ID=@ekranId", new { ekranId });

            if (altEkranlar != null && altEkranlar.Any())
            {
                isKullanilmis = true;
            }

            if (isKullanilmis)
            {
                throw new AppException(MessageCode.ERROR_500_BIR_HATA_OLUSTU, $"Kullanılmış bir ekran silinemez. Önce rol-ekran ilişkilerini ve alt ekranları kaldırınız.");
            }

            // Direkt olarak Delete metodunu kullanıyoruz
            _repoEkran.Delete(dbEkran);
        }

        public t_ekran getEkranByEkranId(int ekranId, bool isYoksaHataDondur = false)
        {
            var ekran = _repoEkran.Get(ekranId);

            if (isYoksaHataDondur && ekran == null)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"{ekranId} id'li ekran sistemde bulunamadı");
            }
            return ekran;
        }

        public EkranDTO getEkranDtoById(int ekranId, bool isYoksaHataDondur = false)
        {
            var ekran = getEkranByEkranId(ekranId, isYoksaHataDondur);

            if (ekran == null)
                return null;

            EkranDTO ekranDTO = _mapper.Map<EkranDTO>(ekran);
            return ekranDTO;
        }

        public List<EkranDTO> getEkranDtoList(bool isYoksaHataDondur = false)
        {
            // Doğrudan GetList() kullanılmalı, ISDELETED=0 kontrolü otomatik olarak yapılır
            var ekranlar = _repoEkran.GetList();

            if (isYoksaHataDondur && (ekranlar == null || !ekranlar.Any()))
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"Ekran bulunamadı");
            }

            return _mapper.Map<List<EkranDTO>>(ekranlar);
        }

        public List<EkranDTO> getEkranDtoListByRolId(int rolId, bool isYoksaHataDondur = false)
        {
            // SQL sorgusu kullanılmalı, ISDELETED=0 kontrolü otomatik olarak yapılır
            var rolEkranlar = _repoRolEkran.GetList("ROL_ID=@rolId", new { rolId });

            if (isYoksaHataDondur && (rolEkranlar == null || !rolEkranlar.Any()))
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"{rolId} id'li role ait ekran bulunamadı");
            }

            if (rolEkranlar == null || !rolEkranlar.Any())
                return new List<EkranDTO>();

            var ekranIdList = rolEkranlar.Select(x => x.EKRAN_ID).ToList();

            // SQL injection riskini önlemek için parametre kullanımı
            var ekranlar = _repoEkran.GetList("ID IN @ekranIdParams", new { ekranIdParams = ekranIdList });

            return _mapper.Map<List<EkranDTO>>(ekranlar);
        }

        public List<EkranDTO> getEkranDtoListByKisiId(int kisiId, bool isYoksaHataDondur = false)
        {
            // SQL sorgusu kullanılmalı, ISDELETED=0 kontrolü otomatik olarak yapılır
            var kisiRoller = _repoKisiRol.GetList("KISI_ID=@kisiId", new { kisiId });

            if (isYoksaHataDondur && (kisiRoller == null || !kisiRoller.Any()))
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"{kisiId} id'li kişiye ait rol bulunamadı");
            }

            if (kisiRoller == null || !kisiRoller.Any())
                return new List<EkranDTO>();

            var rolIdList = kisiRoller.Select(x => x.ROL_ID).ToList();

            // SQL injection riskini önlemek için parametre kullanımı
            var rolEkranlar = _repoRolEkran.GetList("ROL_ID IN @rolIdParams", new { rolIdParams = rolIdList });

            if (isYoksaHataDondur && (rolEkranlar == null || !rolEkranlar.Any()))
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"{kisiId} id'li kişinin rollerine ait ekran bulunamadı");
            }

            if (rolEkranlar == null || !rolEkranlar.Any())
                return new List<EkranDTO>();

            var ekranIdList = rolEkranlar.Select(x => x.EKRAN_ID).Distinct().ToList();

            // SQL injection riskini önlemek için parametre kullanımı
            var ekranlar = _repoEkran.GetList("ID IN @ekranIdParams", new { ekranIdParams = ekranIdList });

            return _mapper.Map<List<EkranDTO>>(ekranlar);
        }

        public List<EkranDTO> getMenuByKisiId(int kisiId)
        {
            var ekranlar = getEkranDtoListByKisiId(kisiId);

            // Ana menü öğelerini al (üst ekranı null olanlar)
            var menuItems = ekranlar.Where(x => x.ustEkranEidDto == null || x.ustEkranEidDto.id <= 0).OrderBy(x => x.siraNo).ToList();

            // Her ana menü öğesi için alt menüleri bul ve ekle
            foreach (var menuItem in menuItems)
            {
                menuItem.altEkranlar = ekranlar
                    .Where(x => x.ustEkranEidDto.id == menuItem.id)
                    .OrderBy(x => x.siraNo)
                    .ToList();
            }

            return menuItems;
        }

        /// <summary>
        /// Ekran kaydedilebilir mi kontrol eder
        /// </summary>
        /// <param name="ekranDTO"></param>
        /// <exception cref="AppException"></exception>
        private void checkEkranDtoKayitEdilebilirMi(EkranDTO ekranDTO)
        {
            if (string.IsNullOrEmpty(ekranDTO.ekranAdi))
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, $"Ekran adı boş olamaz");

            if (string.IsNullOrEmpty(ekranDTO.ekranYolu))
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, $"Ekran yolu boş olamaz");

            if (string.IsNullOrEmpty(ekranDTO.ekranKodu))
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, $"Ekran kodu boş olamaz");

            // SQL sorgusu kullanılmalı, ISDELETED=0 kontrolü otomatik olarak yapılır
            var allEkranList = _repoEkran.GetList("ID <> @id", new { ekranDTO.id });

            // Ekran kodu benzersiz olmalı
            if (allEkranList.Any(x => x.EKRAN_KODU.Equals(ekranDTO.ekranKodu, StringComparison.OrdinalIgnoreCase)))
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"'{ekranDTO.ekranKodu}' kodlu başka bir ekran zaten mevcut");
            }

            // Ekran yolu benzersiz olmalı
            if (allEkranList.Any(x => x.EKRAN_YOLU.Equals(ekranDTO.ekranYolu, StringComparison.OrdinalIgnoreCase)))
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"'{ekranDTO.ekranYolu}' yollu başka bir ekran zaten mevcut");
            }

            // Üst ekran ID kontrolü
            if (ekranDTO.ustEkranEidDto!=null && ekranDTO.ustEkranEidDto.id> 0)
            {
                var ustEkran = _repoEkran.Get(ekranDTO.ustEkranEidDto.id);
                if (ustEkran == null)
                {
                    throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"Belirtilen üst ekran bulunamadı");
                }
            }
        }
    }
}