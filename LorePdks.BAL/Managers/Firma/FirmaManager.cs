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

namespace LorePdks.BAL.Managers.Deneme
{
    public class FirmaManager(
                IHelperManager _helperManager
            , IHttpContextAccessor _httpContextAccessor
            , IMapper _mapper
            , IKodManager _kodManager
        ) : IFirmaManager
    {

     
        private readonly GenericRepository<t_firma> _repoFirma = new GenericRepository<t_firma>();



        public FirmaDTO saveFirma(FirmaDTO firmaDto)
        {

            bool isGuncelleniyor = false;

            if (firmaDto.id > 0) isGuncelleniyor = true;

            var dbFirma = getFirmaByFirmaId(firmaDto.id, isYoksaHataDondur: isGuncelleniyor); // güncelleniyor şeklinde bakılıyorsa bulunamazsa hata dönsün

            checkFirmaDtoKayitEdilebilirMi(firmaDto);

            t_firma firma = _mapper.Map<FirmaDTO, t_firma>(firmaDto);

            _repoFirma.Save(firma);

            firmaDto = _mapper.Map<t_firma, FirmaDTO>(firma);

            return firmaDto;
        }

        public void deleteFirmaByFirmaId(int firmaId)
        {

            var dbFirma = getFirmaByFirmaId(firmaId, isYoksaHataDondur: true);
      
            bool isKullanilmis = false;

            if (isKullanilmis)
            {
                throw new AppException(MessageCode.ERROR_500_BIR_HATA_OLUSTU, $"Kullanılmış bir firma silinemez.");

            }

            _repoFirma.Delete(dbFirma);

        }

        public t_firma getFirmaByFirmaId(int firmaId, bool isYoksaHataDondur = false)
        {

            var firma = _repoFirma.Get(firmaId);

            if (isYoksaHataDondur && firma == null)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"{firmaId} id li firma sistemde bulunamadı");
            }
            return firma;

        }

        public FirmaDTO getFirmaDtoById(int firmaId, bool isYoksaHataDondur = false)
        {

            var firma = getFirmaByFirmaId(firmaId, isYoksaHataDondur);

            FirmaDTO firmaDto = _mapper.Map<t_firma, FirmaDTO>(firma);

            return firmaDto;
        }

        public List<FirmaDTO> getFirmaDtoListById( bool isYoksaHataDondur = false)
        {
            var firmaList = _repoFirma.GetList();
            if (isYoksaHataDondur && firmaList.Count<=0)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"Firma bulunamadı");
            }
            List<FirmaDTO> firmaDtoList = _mapper.Map<List<t_firma>, List<FirmaDTO>>(firmaList);

            return firmaDtoList;
        }

        /// <summary>
        /// bir firma değişiliğe uğramaya çalışırsa yada ilk defa kayıt edilyorsa yapılacak kontroller burada
        /// </summary>
        /// <param name="firmaDto"></param>
        /// <exception cref="AppException"></exception>
        public void checkFirmaDtoKayitEdilebilirMi(FirmaDTO firmaDto)
        {
            if (String.IsNullOrEmpty(firmaDto.ad))
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, $"ad alanı boş olamaz");
            if (String.IsNullOrEmpty(firmaDto.kod))
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, $"kod alanı boş olamaz");

            var allFirmaList = _repoFirma.GetList();

            allFirmaList = allFirmaList.Where(x => x.ID != firmaDto.id).ToList();

            //firma ad kontrolü
            foreach (var item in allFirmaList)
            {
             
                if (item.AD.ToLower().ToTurkishChars().Contains(firmaDto.ad.ToLower().ToTurkishChars()))
                {
                    throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"Kayıt etmeye çalıştığınız firma {item.AD} firma adı ile zaten kaytlı lütfen bu firmayı kullanınız");
                }
                if (item.KOD.ToLower().ToTurkishChars().Contains(firmaDto.kod.ToLower().ToTurkishChars()))
                {
                    throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"Kayıt etmeye çalıştığınız firma {item.AD} firma adı ile zaten kaytlı lütfen bu firmayı kullanınız");
                }
            }

            // eğer önceden kayıtlı ve güncelleme oluyorsa farklı kontroller yapılması gerek
            if (firmaDto.id > 0)
            {
                //önce db halini bul ve ne değiştiğini tespit et buna göre ek kontrolletini yaz 
                var dbFirma = getFirmaByFirmaId(firmaDto.id, isYoksaHataDondur: true);

              

            }

        }
    }
}