using LorePdks.COMMON.DTO.Base;
using LorePdks.COMMON.DTO.Security.Page;

namespace LorePdks.COMMON.DTO.Security.Widget
{
    public class WidgetDTO : BaseDTO
    {
        public string name { get; set; }
        public string selector { get; set; }
        public long pageId { get; set; }
        public PageDTO pageDto { get; set; }
        public long orderNo { get; set; }
        public bool isRoleWidget { get; set; }

    }
}
