using LorePdks.COMMON.DTO.Security.Widget;
using LorePdks.COMMON.DTO.Security.WidgetPermission;

namespace LorePdks.BAL.Managers.Security.Interfaces
{
    public interface IWidgetPermissionManager
    {
        WidgetPermissionDTO GetList(WidgetDTO widgetDto);
        void Set(WidgetPermissionDTO widgetPermissionDto);
    }
}
