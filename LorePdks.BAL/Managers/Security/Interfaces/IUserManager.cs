
using LorePdks.DAL.Model;
using LorePdks.COMMON.DTO.Services.DataTable;
using LorePdks.COMMON.DTO.Security.User;

namespace LorePdks.BAL.Managers.Security.Interfaces
{
    public interface IUserManager
    {
        DataTableResponseDTO<UserDTO> GetDataTableList(DataTableRequestDTO<UserDTO> request);
        List<UserDTO> GetUserList();
        List<UserDTO> GetUserListBySearchValue(string request);
        List<UserDTO> GetListById(List<long> userIdList);
        UserDTO GetByToken(string token);
        UserDTO GetUserDtoByUserNameAndPassword(string email, string password);
        UserDTO GetById(long userId);
        UserDTO GetUserDtoByIdentificationNumber(string identificationNumber);
        long AddWithUserDto(UserDTO userDto);
        long Add(T_Pos_AUTH_USER user);
        UserDTO Set(UserDTO userDto);
    }
}
