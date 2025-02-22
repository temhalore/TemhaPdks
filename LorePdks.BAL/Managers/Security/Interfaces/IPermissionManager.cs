using LorePdks.COMMON.DTO.Security.Permission;

namespace LorePdks.BAL.Managers.Security.Interfaces
{
    public interface IPermissionManager
    {
        void Check(List<PermissionDTO> permissionDTOList);
        List<PermissionDTO> GetList();
        List<PermissionDTO> GetPermissionListByUserId(long userId);
    }
}
