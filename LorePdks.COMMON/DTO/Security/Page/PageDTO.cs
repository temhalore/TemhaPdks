using LorePdks.COMMON.DTO.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace LorePdks.COMMON.DTO.Security.Page
{
    public class PageDTO : BaseDTO
    {
        public string name { get; set; }
        public string menuTree { get; set; }
        public string routerLink { get; set; }
        public int orderNo { get; set; }
    }
}
