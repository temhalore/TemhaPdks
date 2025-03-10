using LorePdks.COMMON.DTO.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LorePdks.COMMON.DTO.Common
{


    public class UserDTO : BaseDTO
    {
        public KisiDTO kisiDto { get; set; }
        public string loginName { get; set; }
        public string sifre { get; set; }
        public bool isAktif { get; set; }
    }
}
