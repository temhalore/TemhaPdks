using LorePdks.COMMON.DTO.Base;

namespace LorePdks.COMMON.DTO.Security.User
{
    public class IPCountDTO : BaseDTO
    {
        public string ip { get; set; }
        public int count { get; set; }
    }
}
