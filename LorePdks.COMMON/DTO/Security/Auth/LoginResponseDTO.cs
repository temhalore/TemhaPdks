
using LorePdks.COMMON.DTO.Security.Widget;
using LorePdks.COMMON.DTO.Security.Page;
using LorePdks.COMMON.DTO.Security.Menu;
using LorePdks.COMMON.DTO.Security.Permission;

namespace LorePdks.COMMON.DTO.Security.Auth
{
    public class LoginResponseDTO
    {
        public UserTokenDTO userTokenDto { get; set; } = new UserTokenDTO();
        public List<MenuDTO> menuListDto { get; set; }
        public List<WidgetDTO> widgetListDto { get; set; }
        public List<PermissionDTO> permissionListDto { get; set; }
        public List<PageDTO> pageListDto { get; set; }
    }
}
