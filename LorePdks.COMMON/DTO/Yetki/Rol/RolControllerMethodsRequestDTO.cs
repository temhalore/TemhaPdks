using LorePdks.COMMON.DTO.Base;
using LorePdks.COMMON.DTO.Common;
using LorePdks.COMMON.DTO.Yetki;
using System.Collections.Generic;

namespace LorePdks.COMMON.DTO.Yetki.Rol
{
    public class RolControllerMethodsRequestDTO
    {
        public EIdDTO rolEidDto { get; set; }
        public List<ControllerAndMethodsDTO> controllerMethods { get; set; }
    }
}