using System.Collections.Generic;
using LorePdks.COMMON.DTO.Security.Widget;
using LorePdks.COMMON.DTO.Security.Permission;

namespace LorePdks.COMMON.DTO.Security.WidgetPermission
{
    public class WidgetPermissionDTO
    {
        public WidgetDTO widgetDto { get; set; }
        public PermissionDTO permissionDto { get; set; }
        public List<PermissionDTO> permissionListDto { get; set; }
    }
}
