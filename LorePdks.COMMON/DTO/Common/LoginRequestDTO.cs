using System.ComponentModel.DataAnnotations;

namespace LorePdks.COMMON.DTO.Common
{
    public class LoginRequestDTO
    {
        [Required]
        public string loginName { get; set; }
        
        [Required]
        public string sifre { get; set; }
    }
}