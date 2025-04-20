//using AutoMapper;
//using LorePdks.BAL.Managers.Common.Kod.Interfaces;
//using LorePdks.BAL.Managers.Deneme.Interfaces;
//using LorePdks.BAL.Managers.Helper.Interfaces;
//using LorePdks.BAL.Managers.Kisi.Interfaces;
//using Microsoft.AspNetCore.Http;
//using LorePdks.DAL.Model;
//using LorePdks.DAL.Repository;
//using LorePdks.COMMON.Enums;
//using LorePdks.COMMON.Models;
//using LorePdks.COMMON.DTO.Common;
//using LorePdks.COMMON.Extensions;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace LorePdks.BAL.Managers.Kisi
//{
//    public class KisiTokenManager(
//            IHelperManager _helperManager,
//            IHttpContextAccessor _httpContextAccessor,
//            IMapper _mapper,
//            IKisiManager _kisiManager
//        ) : IKisiTokenManager
//    {
//        private readonly GenericRepository<t_kisi_token> _repoKisiToken = new GenericRepository<t_kisi_token>();

//        /// <summary>
//        /// Kişi token kaydeder
//        /// </summary>
//        public KisiTokenDTO saveKisiToken(KisiTokenDTO kisiTokenDto)
//        {
//            bool isGuncelleniyor = kisiTokenDto.id > 0;

//            var dbKisiToken = isGuncelleniyor ? getKisiTokenById(kisiTokenDto.id, true) : null;
            
//            // Kişinin var olduğunu kontrol et
//            if (kisiTokenDto.kisiId > 0)
//            {
//                var kisi = _kisiManager.getKisiByKisiId(kisiTokenDto.kisiId, true);
//                kisiTokenDto.loginName = kisi.LOGIN_NAME;
//            }

//            // Token yoksa yeni token oluştur
//            if (string.IsNullOrEmpty(kisiTokenDto.token))
//            {
//                kisiTokenDto.token = Guid.NewGuid().ToString();
//            }

//            // Son kullanma tarihi belirlenmemişse varsayılan olarak 1 gün sonrası
//            if (!kisiTokenDto.expDate.HasValue)
//            {
//                kisiTokenDto.expDate = DateTime.Now.AddDays(1);
//            }

//            // DTO'dan entity'e dönüşüm
//            t_kisi_token kisiToken = _mapper.Map<KisiTokenDTO, t_kisi_token>(kisiTokenDto);
            
//            // Kaydet
//            _repoKisiToken.Save(kisiToken);
            
//            // Sonuç DTO'su
//            kisiTokenDto = _mapper.Map<t_kisi_token, KisiTokenDTO>(kisiToken);
            
//            return kisiTokenDto;
//        }

//        /// <summary>
//        /// ID'ye göre kişi token'ı siler
//        /// </summary>
//        public void deleteKisiTokenById(int kisiTokenId)
//        {
//            var dbKisiToken = getKisiTokenById(kisiTokenId, true);
//            _repoKisiToken.Delete(dbKisiToken);
//        }

//        /// <summary>
//        /// ID'ye göre kişi token entity getirir
//        /// </summary>
//        public t_kisi_token getKisiTokenById(int kisiTokenId, bool isYoksaHataDondur = false)
//        {
//            var kisiToken = _repoKisiToken.Get(kisiTokenId);

//            if (isYoksaHataDondur && kisiToken == null)
//            {
//                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"{kisiTokenId} id'li token sistemde bulunamadı");
//            }
            
//            return kisiToken;
//        }

//        /// <summary>
//        /// ID'ye göre kişi token DTO getirir
//        /// </summary>
//        public KisiTokenDTO getKisiTokenDtoById(int kisiTokenId, bool isYoksaHataDondur = false)
//        {
//            var kisiToken = getKisiTokenById(kisiTokenId, isYoksaHataDondur);
            
//            if (kisiToken == null)
//                return null;
                
//            KisiTokenDTO kisiTokenDto = _mapper.Map<t_kisi_token, KisiTokenDTO>(kisiToken);
            
//            // Kişi bilgisini de ekle
//            if (kisiToken.KISI_ID > 0)
//            {
//                kisiTokenDto.kisiDto = _kisiManager.getKisiDtoById(kisiToken.KISI_ID);
//            }
            
//            return kisiTokenDto;
//        }

//        /// <summary>
//        /// Token değerine göre kişi token DTO getirir
//        /// </summary>
//        public KisiTokenDTO getKisiTokenDtoByToken(string token, bool isYoksaHataDondur = false)
//        {
//            var kisiToken = _repoKisiToken.GetList("TOKEN=@token", new { token }).FirstOrDefault();
            
//            if (isYoksaHataDondur && kisiToken == null)
//            {
//                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, "Geçersiz token");
//            }
            
//            if (kisiToken == null)
//                return null;
                
//            // Süresi dolmuş mu kontrol et
//            if (kisiToken.EXP_DATE.HasValue && kisiToken.EXP_DATE.Value < DateTime.Now)
//            {
//                // Token süresi dolmuşsa token'ı öldür
//                killToken(token);
                
//                if (isYoksaHataDondur)
//                {
//                    throw new AppException(MessageCode.ERROR_401_YETKISIZ_ERISIM, "Token süresi dolmuş");
//                }
//                return null;
//            }
            
//            KisiTokenDTO kisiTokenDto = _mapper.Map<t_kisi_token, KisiTokenDTO>(kisiToken);
            
//            // Kişi bilgisini de ekle
//            if (kisiToken.KISI_ID > 0)
//            {
//                kisiTokenDto.kisiDto = _kisiManager.getKisiDtoById(kisiToken.KISI_ID);
//            }
            
//            return kisiTokenDto;
//        }

//        /// <summary>
//        /// Kişi ID'sine göre tüm token'ları getirir
//        /// </summary>
//        public List<KisiTokenDTO> getKisiTokenDtoListByKisiId(int kisiId)
//        {
//            var kisiTokenList = _repoKisiToken.GetList("KISI_ID=@kisiId", new { kisiId });
//            List<KisiTokenDTO> kisiTokenDtoList = _mapper.Map<List<t_kisi_token>, List<KisiTokenDTO>>(kisiTokenList);
            
//            // Her tokena kişi bilgisini ekle
//            if (kisiTokenDtoList.Any())
//            {
//                var kisiDto = _kisiManager.getKisiDtoById(kisiId);
//                foreach (var tokenDto in kisiTokenDtoList)
//                {
//                    tokenDto.kisiDto = kisiDto;
//                }
//            }
            
//            return kisiTokenDtoList;
//        }

//        /// <summary>
//        /// Belirli bir token'ı geçersiz kılar (öldürür)
//        /// </summary>
//        public void killToken(string token)
//        {
//            var kisiToken = _repoKisiToken.GetList("TOKEN=@token", new { token }).FirstOrDefault();
            
//            if (kisiToken != null)
//            {
//                _repoKisiToken.Delete(kisiToken); // Delete metodu ile ISDELETED alanını otomatik güncelle
//            }
//        }

//        /// <summary>
//        /// Bir kişinin tüm token'larını geçersiz kılar (öldürür)
//        /// </summary>
//        public void killAllTokensForKisi(int kisiId)
//        {
//            var kisiTokenList = _repoKisiToken.GetList("KISI_ID=@kisiId", new { kisiId });
            
//            foreach (var token in kisiTokenList)
//            {
//                _repoKisiToken.Delete(token); // Delete metodu ile ISDELETED alanını otomatik güncelle
//            }
//        }
//    }
//}