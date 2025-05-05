using LorePdks.COMMON.DTO.Base;
using LorePdks.COMMON.DTO.Common;
using LorePdks.COMMON.DTO.Yetki;

namespace LorePdks.COMMON.DTO.Yetki.Rol
{
    public class RolEkranDTO:BaseDTO
    {
        public EIdDTO rolEidDto { get; set; }
        public EIdDTO ekranEidDto { get; set; }
    }
}