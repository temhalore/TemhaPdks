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
using LorePdks.COMMON.DTO.FirmaCihaz;
using LorePdks.BAL.Managers.Firma.Interfaces;

namespace LorePdks.BAL.Managers.FirmaCihaz
{
    public class FirmaCihazManager(
                IHelperManager _helperManager
            , IHttpContextAccessor _httpContextAccessor
            , IMapper _mapper
            , IKodManager _kodManager
        , IFirmaManager _firmaManager
        ) : IFirmaCihazManager
    {


        private readonly GenericRepository<t_firma_cihaz> _repoFirmaCihaz = new GenericRepository<t_firma_cihaz>();


        public FirmaCihazDTO saveFirmaCihaz(FirmaCihazDTO firmaCihazDto)
        {

            bool isGuncelleniyor = false;

            if (firmaCihazDto.id > 0) isGuncelleniyor = true;

            var dbFirmaCihaz = getFirmaCihazByFirmaCihazId(firmaCihazDto.id, isYoksaHataDondur: isGuncelleniyor); // güncelleniyor şeklinde bakılıyorsa bulunamazsa hata dönsün

            checkFirmaCihazDtoKayitEdilebilirMi(firmaCihazDto);

            t_firma_cihaz firmaCihaz = _mapper.Map<FirmaCihazDTO, t_firma_cihaz>(firmaCihazDto);

            _repoFirmaCihaz.Save(firmaCihaz);

            firmaCihazDto = _mapper.Map<t_firma_cihaz, FirmaCihazDTO>(firmaCihaz);

            return firmaCihazDto;
        }

        public void deleteFirmaCihazByFirmaCihazId(int firmaCihazId)
        {

            var dbFirmaCihaz = getFirmaCihazByFirmaCihazId(firmaCihazId, isYoksaHataDondur: true);

            bool isKullanilmis = false;

            if (isKullanilmis)
            {
                throw new AppException(MessageCode.ERROR_500_BIR_HATA_OLUSTU, $"Kullanılmış bir firmaCihaz silinemez.");

            }

            _repoFirmaCihaz.Delete(dbFirmaCihaz);

        }

        public t_firma_cihaz getFirmaCihazByFirmaCihazId(int firmaCihazId, bool isYoksaHataDondur = false)
        {

            var firmaCihaz = _repoFirmaCihaz.Get(firmaCihazId);

            if (isYoksaHataDondur && firmaCihaz == null)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"{firmaCihazId} id li firmaCihaz sistemde bulunamadı");
            }
            return firmaCihaz;

        }

        public List<t_firma_cihaz> getFirmaCihazListByFirmaId(int firmaId)
        {

            var modelList = _repoFirmaCihaz.GetList("FIRMA_ID=@firmaId", new { firmaId });



            return modelList;

        }

        public List<FirmaCihazDTO> getFirmaCihazDtoListByFirmaId(int firmaId)
        {

            var modelList = getFirmaCihazListByFirmaId(firmaId);

            var dtoList = _mapper.Map<List<t_firma_cihaz>, List<FirmaCihazDTO>>(modelList);
            return dtoList;

        }


        public FirmaCihazDTO getFirmaCihazDtoById(int firmaCihazId, bool isYoksaHataDondur = false)
        {
            var firmaCihaz = getFirmaCihazByFirmaCihazId(firmaCihazId, isYoksaHataDondur);

            FirmaCihazDTO firmaCihazDto = _mapper.Map<t_firma_cihaz, FirmaCihazDTO>(firmaCihaz);

            return firmaCihazDto;
        }

        /// <summary>
        /// bir firmaCihaz değişiliğe uğramaya çalışırsa yada ilk defa kayıt edilyorsa yapılacak kontroller burada
        /// </summary>
        /// <param name="firmaCihazDto"></param>
        /// <exception cref="AppException"></exception>
        public void checkFirmaCihazDtoKayitEdilebilirMi(FirmaCihazDTO firmaCihazDto)
        {
            if (firmaCihazDto.firmaDto == null)
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, $"firma seçmelisiniz");
            if (firmaCihazDto.firmaDto.id <= 0)
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, $"firma seçmelisiniz");
            if (string.IsNullOrEmpty(firmaCihazDto.ad))
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, $"ad alanı boş olamaz");
            if (Convert.ToInt32(firmaCihazDto.cihazMakineGercekId) == 0)
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, $"cihazMakineGercekId alanı boş olamaz");

            var firma = _firmaManager.getFirmaByFirmaId(firmaCihazDto.firmaDto.id, true);

            _kodManager.checkKodDTOIdInTipList(firmaCihazDto.firmaCihazTipKodDto, AppEnums.KodTipList.FIRMA_CIHAZ_TIP, $"firmaCihazTipKodDto alanı uygun değil");


            var allFirmaCihazList = getFirmaCihazListByFirmaId(firmaCihazDto.firmaDto.id);

            allFirmaCihazList = allFirmaCihazList.Where(x => x.ID != firmaCihazDto.id).ToList();

            //firmaCihaz ad kontrolü
            foreach (var item in allFirmaCihazList)
            {


                if (item.CIHAZ_MAKINE_GERCEK_ID == firmaCihazDto.cihazMakineGercekId)
                {
                    throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"Kayıt etmeye çalıştığınız firmaCihaz cihazMakineGercekId olarak aynı firmada zaten kaytlı");
                }
            }

            // eğer önceden kayıtlı ve güncelleme oluyorsa farklı kontroller yapılması gerek
            if (firmaCihazDto.id > 0)
            {
                //önce db halini bul ve ne değiştiğini tespit et buna göre ek kontrolletini yaz 
                var dbFirmaCihaz = getFirmaCihazByFirmaCihazId(firmaCihazDto.id, isYoksaHataDondur: true);



            }

        }
    }
}