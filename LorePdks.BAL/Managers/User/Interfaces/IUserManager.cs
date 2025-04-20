using LorePdks.COMMON.DTO.Common;
using LorePdks.DAL.Model;
using System.Collections.Generic;

namespace LorePdks.BAL.Managers.User
{
    public interface IUserManager
    {
        UserDTO saveUser(UserDTO userDto);
        void deleteUserByUserId(int userId);
        t_user getUserByUserId(int userId, bool isYoksaHataDondur = false);
        UserDTO getUserDtoById(int userId, bool isYoksaHataDondur = false);
        List<UserDTO> getUserDtoList(bool isYoksaHataDondur = false);
        void checkUserDtoKayitEdilebilirMi(UserDTO userDto);
    }
}