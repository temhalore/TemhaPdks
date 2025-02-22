using LorePdks.COMMON.DTO.Security.Role;
using LorePdks.COMMON.DTO.Security.RoleWidget;

namespace LorePdks.BAL.Managers.Security.Interfaces
{
    public interface IRoleWidgetManager
    {
        RoleWidgetDTO GetRoleWidgetTreeListForAdmin(RoleDTO roleDto);
        RoleWidgetDTO GetList(RoleDTO roleDto);
        void Set(RoleWidgetDTO roleWidgetDto);
        //long SetRegisterAdminRoleAndRoleWidgets(OrganizationDTO organizationDto);
    }
}
