using LorePdks.COMMON.DTO.Base;

namespace LorePdks.COMMON.DTO.Security.Permission
{
    public class PermissionDTO : BaseDTO
    {
        public string type { get; set; }
        public string area { get; set; }
        public string controller { get; set; }
        public string action { get; set; }
        public string returnType { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public bool isWidgetPermission { get; set; }
    }
}
