using AutoMapper;
using LorePdks.BAL.Managers.Helper.Interfaces;
using LorePdks.BAL.Managers.Kisi.Interfaces;
using LorePdks.COMMON.DTO.Common;
using LorePdks.COMMON.Enums;
using LorePdks.COMMON.Models;
using LorePdks.DAL.Model;
using LorePdks.DAL.Repository;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LorePdks.BAL.Managers.User
{
    public class UserManager : IUserManager
    {
        private readonly IHelperManager _helperManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly IKisiManager _kisiManager;
        private readonly GenericRepository<t_user> _repoUser = new GenericRepository<t_user>();

        public UserManager(
            IHelperManager helperManager,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IKisiManager kisiManager)
        {
            _helperManager = helperManager;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _kisiManager = kisiManager;
        }

        public UserDTO saveUser(UserDTO userDto)
        {
            bool isGuncelleniyor = false;

            if (userDto.id > 0) isGuncelleniyor = true;

            var dbUser = getUserByUserId(userDto.id, isYoksaHataDondur: isGuncelleniyor); // güncelleniyor şeklinde bakılıyorsa bulunamazsa hata dönsün

            checkUserDtoKayitEdilebilirMi(userDto);

            t_user user = _mapper.Map<UserDTO, t_user>(userDto);

            _repoUser.Save(user);

            userDto = _mapper.Map<t_user, UserDTO>(user);

            return userDto;
        }

        public void deleteUserByUserId(int userId)
        {
            var dbUser = getUserByUserId(userId, isYoksaHataDondur: true);
      
            bool isKullanilmis = false;
            // Eğer User kaydı başka bir yerde kullanılıyorsa burada kontrol edilebilir

            if (isKullanilmis)
            {
                throw new AppException(MessageCode.ERROR_500_BIR_HATA_OLUSTU, $"Kullanılmış bir User kaydı silinemez.");
            }

            _repoUser.Delete(dbUser);
        }

        public t_user getUserByUserId(int userId, bool isYoksaHataDondur = false)
        {
            var user = _repoUser.Get(userId);

            if (isYoksaHataDondur && user == null)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"{userId} id'li User kaydı sistemde bulunamadı");
            }
            return user;
        }

        public UserDTO getUserDtoById(int userId, bool isYoksaHataDondur = false)
        {
            var user = getUserByUserId(userId, isYoksaHataDondur);
            if (user == null) return null;

            UserDTO userDto = _mapper.Map<t_user, UserDTO>(user);

            // Kişi bilgilerini doldur
            if (userDto.kisiDto != null && userDto.kisiDto.id > 0)
            {
                userDto.kisiDto = _kisiManager.getKisiDtoById(userDto.kisiDto.id);
            }

            return userDto;
        }

        public List<UserDTO> getUserDtoList(bool isYoksaHataDondur = false)
        {
            var userList = _repoUser.GetList(x => x.ISDELETED == 0);
            if (isYoksaHataDondur && userList.Count <= 0)
            {
                throw new AppException(MessageCode.ERROR_503_GECERSIZ_VERI_GONDERIMI, $"User kaydı bulunamadı");
            }
            
            List<UserDTO> userDtoList = _mapper.Map<List<t_user>, List<UserDTO>>(userList);

            // Kişi bilgilerini doldur
            foreach (var userDto in userDtoList)
            {
                if (userDto.kisiDto != null && userDto.kisiDto.id > 0)
                {
                    userDto.kisiDto = _kisiManager.getKisiDtoById(userDto.kisiDto.id);
                }
            }

            return userDtoList;
        }

        public void checkUserDtoKayitEdilebilirMi(UserDTO userDto)
        {
            if (userDto.kisiDto == null || userDto.kisiDto.id <= 0)
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, $"Kişi bilgisi boş olamaz");
            
            if (string.IsNullOrEmpty(userDto.loginName))
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, $"Kullanıcı adı boş olamaz");

            if (string.IsNullOrEmpty(userDto.sifre) && userDto.id <= 0)
                throw new AppException(MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, $"Şifre boş olamaz");

            // Kişi varlığının geçerliliğini kontrol et
            _kisiManager.getKisiByKisiId(userDto.kisiDto.id, isYoksaHataDondur: true);

            // Kullanıcı adı benzersiz olmalı
            var existingUserWithSameUsername = _repoUser.GetList(x => 
                x.USER_NAME == userDto.loginName && 
                x.ID != userDto.id && 
                x.ISDELETED == 0);

            if (existingUserWithSameUsername.Count > 0)
            {
                throw new AppException(
                    MessageCode.ERROR_502_EKSIK_VERI_GONDERIMI, 
                    $"{userDto.loginName} kullanıcı adı sistemde zaten kayıtlı, lütfen farklı bir kullanıcı adı seçiniz."
                );
            }
        }
    }
}