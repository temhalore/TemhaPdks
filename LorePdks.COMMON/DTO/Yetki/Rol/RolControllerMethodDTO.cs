using LorePdks.COMMON.DTO.Base;
using System;
using System.Collections.Generic;

namespace LorePdks.COMMON.DTO.Yetki.Rol
{
    public class RolControllerMethodDTO : BaseDTO
    {
        public EIdDTO rolEidDto { get; set; }
        public string controllerName { get; set; }
        public string methodName { get; set; }
    }
}