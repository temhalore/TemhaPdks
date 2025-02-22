using System.Collections.Generic;
using LorePdks.COMMON.DTO.Security.Widget;
using LorePdks.COMMON.DTO.Security.Role;
using LorePdks.COMMON.DTO.Base;

namespace LorePdks.COMMON.DTO.Security.RoleWidget
{
    public class RoleWidgetDTO
    {
        public RoleDTO roleDto { get; set; }
        public WidgetDTO widgetDto { get; set; }
        public List<WidgetDTO> widgetListDto { get; set; }
        public List<TreeTableDTO<WidgetDTO>> widgetTreeListDto { get; set; }
    }
}
