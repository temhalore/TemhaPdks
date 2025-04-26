using LorePdks.COMMON.DTO.Base;
using System;
using System.Text.Json.Serialization;

namespace LorePdks.COMMON.DTO.Yetki
{
    public class ControllerAndMethodsDTO
    {
        public string controllerName { get; set; }
        public List<string> methods { get; set; }
    }
}