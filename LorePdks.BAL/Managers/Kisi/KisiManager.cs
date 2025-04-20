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
using LorePdks.COMMON.DTO.Kisi;
using LorePdks.BAL.Managers.Kisi.Interfaces;

namespace LorePdks.BAL.Managers.Kisi
{
    public class KisiManager(
                IHelperManager _helperManager
            , IHttpContextAccessor _httpContextAccessor
            , IMapper _mapper
            , IKodManager _kodManager
        ) : IKisiManager
    {


        private readonly GenericRepository<t_kisi> _repoKisi = new GenericRepository<t_kisi>();


        public KisiDTO saveKisi(KisiDTO kisiDto)
        {

            bool isGuncelleniyor = false;

            if (kisiDto.id > 0) isGuncelleniyor = true;

            var dbKisi = getKisiByKisiId(kisiDto.id, isYoksaHataDondur: isGuncelleniyor); // güncelleniyor şeklinde bakılıyorsa bulunamazsa hata dönsün

            checkKisiDtoKayitEdilebilirMi(kisiDto);

            t_kisi kisi = _mapper.Map<KisiDTO, t_kisi>(kisiDto);

            _repoKisi.Save(kisi);

            kisiDto = _mapper.Map<t_kisi, KisiDTO>(kisi);

            return kisiDto;
        }

        public void deleteKisiByKisiId(int kisiId)
        {

            var dbKisi = getKisiByKisiId(kisiId, isYoksaHataDondur: true);

            bool isKullanilmis = false;

            if (isKullanilmis)
            {
                throw new AppException(MessageCode.ERROR_500_BIR_HATA_OLUSTU, $"Kullanılmış bir kisi silinemez.");
            }

            _repoKisi.Delete(dbKisi);

        }

        public t_kisi getKisiByKisiId(int kisiId, bool isYoksaHataDondur = false)
        {

            var kisi = _repoKisi.Get(kisiId);

            if (isYoksaHataDondur && kisi == null)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"{kisiId} id li kisi sistemde bulunamadı");
            }
            return kisi;

        }

        public KisiDTO getKisiDtoById(int kisiId, bool isYoksaHataDondur = false)
        {

            var kisi = getKisiByKisiId(kisiId, isYoksaHataDondur);

            KisiDTO kisiDto = _mapper.Map<t_kisi, KisiDTO>(kisi);

            return kisiDto;
        }

        //verilen login name ve kişiyi var olan diğer metodlardan bulur ve şifre kontrolü yapar
        public KisiDTO getKisiDtoByLoginNameAndSifre(string loginName, string sifre, bool isYoksaHataDondur = false)
        {
            var kisi = getKisiByLoginName(loginName, isYoksaHataDondur: false);
            if (isYoksaHataDondur && kisi == null)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"loginName ile eşleşen kisi bulunamadı");
            }
            if (kisi.SIFRE != sifre)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"loginName ve şifre eşleşmedi");
            }
            KisiDTO kisiDto = _mapper.Map<t_kisi, KisiDTO>(kisi);
            return kisiDto;
        }

        //kişiyi sadece loginName e göre getirir    
        public KisiDTO getKisiDtoByLoginName(string loginName, bool isYoksaHataDondur = false)
        {
            var kisi = getKisiByLoginName(loginName, isYoksaHataDondur: false);
            if (isYoksaHataDondur && kisi == null)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"loginName ile eşleşen kisi bulunamadı");
            }
            KisiDTO kisiDto = _mapper.Map<t_kisi, KisiDTO>(kisi);
            return kisiDto;
        }

        //kişiyi sadece loginName e göre getirir ama kişi model döner
        public t_kisi getKisiByLoginName(string loginName, bool isYoksaHataDondur = false)
        {
            var kisi = _repoKisi.GetList("LOGIN_NAME=@loginName", new { loginName }).FirstOrDefault();
            if (isYoksaHataDondur && kisi == null)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"loginName ile eşleşen kisi bulunamadı");
            }
            return kisi;
        }

        //kişiyi sadece tc ye göre getirir
        public KisiDTO getKisiDtoByTc(string tc, bool isYoksaHataDondur = false)
        {
            var kisi = getKisiByTc(tc, isYoksaHataDondur: false);
            if (isYoksaHataDondur && kisi == null)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"tc ile eşleşen kisi bulunamadı");
            }
            KisiDTO kisiDto = _mapper.Map<t_kisi, KisiDTO>(kisi);
            return kisiDto;
        }

        //kişiyi sadece tc ye göre getirir ama kişi model döner
        public t_kisi getKisiByTc(string tc, bool isYoksaHataDondur = false)
        {
            var kisi = _repoKisi.GetList("TC=@tc", new { tc }).FirstOrDefault();
            if (isYoksaHataDondur && kisi == null)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"tc ile eşleşen kisi bulunamadı");
            }
            return kisi;
        }

        //kişiyi arama text e göre getirir
        public List<KisiDTO> getKisiDtoListByAramaText(string aramaText, bool isYoksaHataDondur = false)
        {
            var kisiList = getKisiListByAramaText(aramaText, isYoksaHataDondur: false);
            List<KisiDTO> kisiDtoList = _mapper.Map<List<t_kisi>, List<KisiDTO>>(kisiList);
            return kisiDtoList;
        }

        //kişiyi arama text e göre getirir ama kişi model döner
        public List<t_kisi> getKisiListByAramaText(string aramaText, bool isYoksaHataDondur = false)
        {
            var kisiList = _repoKisi.GetList("AD LIKE @aramaText OR SOYAD LIKE @aramaText OR TC LIKE @aramaText OR LOGIN_NAME LIKE @aramaText", new { aramaText = "%" + aramaText + "%" });
            if (isYoksaHataDondur && kisiList.Count <= 0)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"Arama sonucu kisi bulunamadı");
            }
            return kisiList;
        }

        public List<KisiDTO> getKisiDtoListById(bool isYoksaHataDondur = false)
        {
            var kisiList = _repoKisi.GetList();
            if (isYoksaHataDondur && kisiList.Count <= 0)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"Kisi bulunamadı");
            }
            List<KisiDTO> kisiDtoList = _mapper.Map<List<t_kisi>, List<KisiDTO>>(kisiList);

            return kisiDtoList;
        }

        /// <summary>
        /// bir kisi değişiliğe uğramaya çalışırsa yada ilk defa kayıt edilyorsa yapılacak kontroller burada
        /// </summary>
        /// <param name="kisiDto"></param>
        /// <exception cref="AppException"></exception>
        public void checkKisiDtoKayitEdilebilirMi(KisiDTO kisiDto)
        {
            if (string.IsNullOrEmpty(kisiDto.ad))
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, $"ad alanı boş olamaz");
            if (string.IsNullOrEmpty(kisiDto.soyad))
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, $"soyad alanı boş olamaz");
            if (string.IsNullOrEmpty(kisiDto.loginName))
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, $"loginName alanı boş olamaz");
            if (string.IsNullOrEmpty(kisiDto.sifre))
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, $"sifre alanı boş olamaz");
            if (string.IsNullOrEmpty(kisiDto.tc))
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, $"tc alanı boş olamaz");

            var allKisiList = _repoKisi.GetList();

            allKisiList = allKisiList.Where(x => x.ID != kisiDto.id).ToList();

            var kisiKayitliMiTc = allKisiList.Where(x => x.TC.ToLower().ToTurkishChars().Contains(kisiDto.tc.ToLower().ToTurkishChars())).FirstOrDefault();

            var kisiKayitliMiLoginName = allKisiList.Where(x => x.LOGIN_NAME.ToLower().ToTurkishChars().Contains(kisiDto.loginName.ToLower().ToTurkishChars())).FirstOrDefault();

            if (kisiKayitliMiTc != null)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"Kayıt etmeye çalıştığınız kimlik numaralı kisi {kisiKayitliMiTc.AD} {kisiKayitliMiTc.SOYAD} kisi adı ile zaten kaytlı lütfen bu kisiyi kullanınız");
            }

            if (kisiKayitliMiLoginName != null)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"Kayıt etmeye çalıştığınız loginName li kisi {kisiKayitliMiLoginName.AD} {kisiKayitliMiLoginName.SOYAD} kisi adı ile zaten kaytlı lütfen bu kisiyi kullanınız");
            }

            // eğer önceden kayıtlı ve güncelleme oluyorsa farklı kontroller yapılması gerek
            if (kisiDto.id > 0)
            {
                //önce db halini bul ve ne değiştiğini tespit et buna göre ek kontrolletini yaz 
                var dbKisi = getKisiByKisiId(kisiDto.id, isYoksaHataDondur: true);

            }

        }


    }
}