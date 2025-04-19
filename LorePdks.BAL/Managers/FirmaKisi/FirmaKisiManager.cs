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
    public class FirmaKisiManager(
                IHelperManager _helperManager
            , IHttpContextAccessor _httpContextAccessor
            , IMapper _mapper
            , IKodManager _kodManager
        , IFirmaManager _firmaManager
        , IKisiManager _kisiManager
        ) : IFirmaKisiManager
    {

     
        private readonly GenericRepository<t_firma_kisi> _repoFirmaKisi = new GenericRepository<t_firma_kisi>();


        public FirmaKisiDTO saveFirmaKisi(FirmaKisiDTO firmaKisiDto)
        {

            bool isGuncelleniyor = false;

            if (firmaKisiDto.id > 0) isGuncelleniyor = true;

            var dbFirmaKisi = getFirmaKisiByFirmaKisiId(firmaKisiDto.id, isYoksaHataDondur: isGuncelleniyor); // güncelleniyor şeklinde bakılıyorsa bulunamazsa hata dönsün

            checkFirmaKisiDtoKayitEdilebilirMi(firmaKisiDto);


           t_firma_kisi firmaKisi = _mapper.Map<FirmaKisiDTO, t_firma_kisi>(firmaKisiDto);

            //burada using transaction yapılarak önce kişi eklenir sonra firmaKisi eklenir. eğer firmaKisi eklenirken hata alınırsa kişi eklenen transaction geri alınır.
    //        using (var transaction = new TransactionScope(
    //TransactionScopeOption.Required,
    //new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted },
    //TransactionScopeAsyncFlowOption.Enabled))
    //        {
    //            try
    //            {
                    

    //                transaction.Complete();
    //            }
    //            catch (AppException ex)
    //            {
    //                transaction.Dispose();
    //                throw new AppException(MessageCode.ERROR_500_BIR_HATA_OLUSTU, $"Hata:{ex.appMessage}");

    //            }
    _repoFirmaKisi.Save(firmaKisi); 

            firmaKisiDto = _mapper.Map<t_firma_kisi, FirmaKisiDTO>(firmaKisi);

            return firmaKisiDto;
        }

  

        public void deleteFirmaKisiByFirmaKisiId(int firmaKisiId)
        {

            var dbFirmaKisi = getFirmaKisiByFirmaKisiId(firmaKisiId, isYoksaHataDondur: true);
      
            bool isKullanilmis = false;

            if (isKullanilmis)
            {
                throw new AppException(MessageCode.ERROR_500_BIR_HATA_OLUSTU, $"Kullanılmış bir firmaKisi silinemez.");

            }

            _repoFirmaKisi.Delete(dbFirmaKisi);

        }

        public t_firma_kisi getFirmaKisiByFirmaKisiId(int firmaKisiId, bool isYoksaHataDondur = false)
        {

            var firmaKisi = _repoFirmaKisi.Get(firmaKisiId);

            if (isYoksaHataDondur && firmaKisi == null)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"{firmaKisiId} id li firmaKisi sistemde bulunamadı");
            }
            return firmaKisi;

        }

        public List<t_firma_kisi> getFirmaKisiListByFirmaId(int firmaId)
        {

            var modelList = _repoFirmaKisi.GetList("FIRMA_ID=@firmaId", new { firmaId = firmaId });

            
        
            return modelList;

        }

        public List<FirmaKisiDTO> getFirmaKisiDtoListByFirmaId(int firmaId)
        {

            var modelList = getFirmaKisiListByFirmaId(firmaId);

            var dtoList = _mapper.Map< List <t_firma_kisi> , List <FirmaKisiDTO>>(modelList);
            return dtoList;

        }


        public FirmaKisiDTO getFirmaKisiDtoById(int firmaKisiId, bool isYoksaHataDondur = false)
        {
            var firmaKisi = getFirmaKisiByFirmaKisiId(firmaKisiId, isYoksaHataDondur);

            FirmaKisiDTO firmaKisiDto = _mapper.Map<t_firma_kisi, FirmaKisiDTO>(firmaKisi);

            return firmaKisiDto;
        }

        /// <summary>
        /// bir firmaKisi değişiliğe uğramaya çalışırsa yada ilk defa kayıt edilyorsa yapılacak kontroller burada
        /// </summary>
        /// <param name="firmaKisiDto"></param>
        /// <exception cref="AppException"></exception>
        public void checkFirmaKisiDtoKayitEdilebilirMi(FirmaKisiDTO firmaKisiDto)
        {
            if (firmaKisiDto.firmaDto==null || firmaKisiDto.firmaDto.id <= 0)
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, $"firma seçmelisiniz");
            if (firmaKisiDto.kisiDto == null || firmaKisiDto.kisiDto.id <= 0)
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, $"kişi seçmelisiniz");

            var firma = _firmaManager.getFirmaByFirmaId(firmaKisiDto.firmaDto.id,true);

            _kodManager.checkKodDTOIdInTipList(firmaKisiDto.firmaKisiTipKodDto, AppEnums.KodTipList.FIRMA_KISI_TIP, $"firmaKisiTipKodDto alanı uygun değil");

            var allFirmaKisiList = getFirmaKisiListByFirmaId(firmaKisiDto.firmaDto.id);



        }
    }
}