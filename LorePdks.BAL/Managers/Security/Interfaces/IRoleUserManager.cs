using LorePdks.COMMON.DTO.Security.Role;
using LorePdks.COMMON.DTO.Security.RoleUser;

namespace LorePdks.BAL.Managers.Security.Interfaces
{
    public interface IRoleUserManager
    {
        //List<RoleUserDTO> GetListByUserId(long userId);
        //List<RoleUserDTO> GetListByRoleId(long roleId);

        RoleUserDTO GetList(RoleDTO roleDto);
        void Set(RoleUserDTO roleUserDto);
        void AddList(List<int> roleIdList, int userId);
    }
}
