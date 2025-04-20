using AutoMapper;
using LorePdks.BAL.Managers.Deneme.Interfaces;
using LorePdks.BAL.Managers.KisiToken.Interfaces;
using LorePdks.COMMON.DTO.Common;
using LorePdks.COMMON.Enums;
using LorePdks.COMMON.Extensions;
using LorePdks.COMMON.Models;
using LorePdks.DAL.Model;
using LorePdks.DAL.Repository;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LorePdks.BAL.Managers.KisiToken
{
    public class KisiTokenManager : IKisiTokenManager
    {
        private readonly IKisiManager _kisiManager;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly GenericRepository<t_kisi_token> _repoKisiToken = new GenericRepository<t_kisi_token>();

        public KisiTokenManager(
            IKisiManager kisiManager,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _kisiManager = kisiManager;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Token bilgilerini kaydeder
        /// </summary>
        public KisiTokenDTO saveKisiToken(KisiTokenDTO kisiTokenDto)
        {
            bool isGuncelleniyor = kisiTokenDto.id > 0;

            var dbKisiToken = isGuncelleniyor ? getKisiTokenById(kisiTokenDto.id, true) : null;

            // Kişinin var olduğunu kontrol et
            if (kisiTokenDto.kisiId > 0 && string.IsNullOrEmpty(kisiTokenDto.loginName))
            {
                var kisi = _kisiManager.getKisiByKisiId(kisiTokenDto.kisiId, true);
                kisiTokenDto.loginName = kisi.LOGIN_NAME;
            }

            // Token oluştur (eğer yeni kayıtsa ve token yoksa)
            if (!isGuncelleniyor && string.IsNullOrEmpty(kisiTokenDto.token))
            {
                kisiTokenDto.token = GeneralExtensions.getTokenUret(kisiTokenDto.loginName);
            }

            // Son kullanma tarihi belirlenmemişse varsayılan olarak 1 gün sonrası
            if (!kisiTokenDto.expDate.HasValue)
            {
                kisiTokenDto.expDate = DateTime.Now.AddDays(1);
            }

            // DTO'yu entity'ye dönüştür
            t_kisi_token kisiToken = _mapper.Map<KisiTokenDTO, t_kisi_token>(kisiTokenDto);

            // Kaydet
            _repoKisiToken.Save(kisiToken);

            // Entity'yi DTO'ya dönüştür
            kisiTokenDto = _mapper.Map<t_kisi_token, KisiTokenDTO>(kisiToken);

            // Kişi bilgilerini DTO'ya ekle
            if (kisiTokenDto.kisiId > 0)
            {
                kisiTokenDto.kisiDto = _kisiManager.getKisiDtoById(kisiTokenDto.kisiId);
            }

            return kisiTokenDto;
        }

        /// <summary>
        /// Token'a göre kişi token bilgilerini getirir
        /// </summary>
        public KisiTokenDTO getKisiTokenDtoByToken(string token, bool isYoksaHataDondur = false)
        {
            // Token ile kişi token kaydını bul
            var kisiToken = _repoKisiToken.GetList("TOKEN=@token AND ISDELETED=0", new { token }).FirstOrDefault();

            if (isYoksaHataDondur && kisiToken == null)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, "Geçerli bir token bulunamadı");
            }

            if (kisiToken == null)
            {
                return null;
            }

            // Süresi dolmuş mu kontrol et
            if (kisiToken.EXP_DATE.HasValue && kisiToken.EXP_DATE.Value < DateTime.Now)
            {
                // Token süresi dolmuşsa token'ı öldür
                killToken(token);
                
                if (isYoksaHataDondur)
                {
                    throw new AppException(MessageCode.ERROR_401_YETKISIZ_ERISIM, "Token süresi dolmuş");
                }
                return null;
            }

            // Entity'yi DTO'ya dönüştür
            KisiTokenDTO kisiTokenDto = _mapper.Map<t_kisi_token, KisiTokenDTO>(kisiToken);

            // Kişi bilgilerini ekle
            if (kisiToken.KISI_ID > 0)
            {
                kisiTokenDto.kisiDto = _kisiManager.getKisiDtoById(kisiToken.KISI_ID);
            }

            return kisiTokenDto;
        }

        /// <summary>
        /// Kişi ID'sine göre aktif token bilgisini getirir
        /// </summary>
        public KisiTokenDTO getAktifKisiTokenByKisiId(int kisiId)
        {
            // Kişi ID'sine ait aktif tokenları bul
            var kisiToken = _repoKisiToken.GetList("KISI_ID=@kisiId AND ISDELETED=0 AND EXP_DATE > @now", 
                new { kisiId, now = DateTime.Now }).FirstOrDefault();

            if (kisiToken == null)
            {
                return null;
            }

            // Entity'yi DTO'ya dönüştür
            KisiTokenDTO kisiTokenDto = _mapper.Map<t_kisi_token, KisiTokenDTO>(kisiToken);

            // Kişi bilgilerini ekle
            if (kisiToken.KISI_ID > 0)
            {
                kisiTokenDto.kisiDto = _kisiManager.getKisiDtoById(kisiToken.KISI_ID);
            }

            return kisiTokenDto;
        }

        /// <summary>
        /// Verilen token'ı geçersiz kılar (siler veya ISDELETED=1 yapar)
        /// </summary>
        public void killToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return;
            }

            // Token ile kişi token kaydını bul
            var kisiToken = _repoKisiToken.GetList("TOKEN=@token AND ISDELETED=0", new { token }).FirstOrDefault();

            if (kisiToken == null)
            {
                return;
            }

            // Token'ı geçersiz kıl (silmek yerine ISDELETED=1 yap)
            kisiToken.ISDELETED = 1;
            kisiToken.MODIFIEDDATE = DateTime.Now;
            _repoKisiToken.Save(kisiToken);
        }

        /// <summary>
        /// Kişiye ait tüm token'ları geçersiz kılar
        /// </summary>
        public void killAllTokensForKisi(int kisiId)
        {
            if (kisiId <= 0)
            {
                return;
            }

            // Kişiye ait tüm aktif tokenları bul
            var kisiTokenList = _repoKisiToken.GetList("KISI_ID=@kisiId AND ISDELETED=0", new { kisiId });

            foreach (var token in kisiTokenList)
            {
                // Her bir token'ı geçersiz kıl
                token.ISDELETED = 1;
                token.MODIFIEDDATE = DateTime.Now;
                _repoKisiToken.Save(token);
            }
        }

        /// <summary>
        /// ID'ye göre kişi token'ı siler
        /// </summary>
        public void deleteKisiTokenById(int kisiTokenId)
        {
            var dbKisiToken = getKisiTokenById(kisiTokenId, true);
            
            if (dbKisiToken != null)
            {
                dbKisiToken.ISDELETED = 1;
                dbKisiToken.MODIFIEDDATE = DateTime.Now;
                _repoKisiToken.Save(dbKisiToken);
            }
        }

        /// <summary>
        /// ID'ye göre kişi token DTO getirir
        /// </summary>
        public KisiTokenDTO getKisiTokenDtoById(int kisiTokenId, bool isYoksaHataDondur = false)
        {
            var kisiToken = getKisiTokenById(kisiTokenId, isYoksaHataDondur);
            
            if (kisiToken == null)
                return null;
                
            KisiTokenDTO kisiTokenDto = _mapper.Map<t_kisi_token, KisiTokenDTO>(kisiToken);
            
            // Kişi bilgisini de ekle
            if (kisiToken.KISI_ID > 0)
            {
                kisiTokenDto.kisiDto = _kisiManager.getKisiDtoById(kisiToken.KISI_ID);
            }
            
            return kisiTokenDto;
        }

        /// <summary>
        /// Kişi ID'sine göre tüm token'ları getirir
        /// </summary>
        public List<KisiTokenDTO> getKisiTokenDtoListByKisiId(int kisiId)
        {
            var kisiTokenList = _repoKisiToken.GetList("KISI_ID=@kisiId", new { kisiId });
            List<KisiTokenDTO> kisiTokenDtoList = _mapper.Map<List<t_kisi_token>, List<KisiTokenDTO>>(kisiTokenList);
            
            // Her tokena kişi bilgisini ekle
            if (kisiTokenDtoList.Any())
            {
                var kisiDto = _kisiManager.getKisiDtoById(kisiId);
                foreach (var tokenDto in kisiTokenDtoList)
                {
                    tokenDto.kisiDto = kisiDto;
                }
            }
            
            return kisiTokenDtoList;
        }

        /// <summary>
        /// ID'ye göre kişi token entity getirir
        /// </summary>
        private t_kisi_token getKisiTokenById(int kisiTokenId, bool isYoksaHataDondur = false)
        {
            if (kisiTokenId <= 0)
            {
                return null;
            }

            var kisiToken = _repoKisiToken.Get(kisiTokenId);

            if (isYoksaHataDondur && kisiToken == null)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"{kisiTokenId} ID'li token bulunamadı");
            }

            return kisiToken;
        }
    }
}