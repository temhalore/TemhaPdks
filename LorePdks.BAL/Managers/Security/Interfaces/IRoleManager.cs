using LorePdks.COMMON.DTO.Security.Role;

namespace LorePdks.BAL.Managers.Security.Interfaces
{
    public interface IRoleManager
    {
        List<RoleDTO> GetList();
        RoleDTO Get(long roleId);
        List<RoleDTO> GetListByWidgetId(long widgetId);
        long Add(RoleDTO roleDTO);
        void Set(RoleDTO roleDTO);
        void Del(RoleDTO roleDTO);
    }
}
