using LorePdks.COMMON.DTO.Base;
using System;

namespace LorePdks.COMMON.DTO.Security.User
{
    public class BlockedIPDTO : BaseDTO
    {
        public string ip { get; set; }
        public string reason { get; set; }
        public DateTime blockedDate { get; set; }
        public DateTime blockedDateExpire { get; set; }
    }
}
