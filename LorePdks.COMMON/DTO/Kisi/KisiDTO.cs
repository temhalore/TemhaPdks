using LorePdks.COMMON.DTO.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LorePdks.COMMON.DTO.Common
{


    public class KisiDTO : BaseDTO
    {

        public string ad { get; set; }
        public string soyad { get; set; }
        public string tc { get; set; }
        public string cepTel { get; set; }
        public string email { get; set; }
        public string loginName { get; set; }
        public string sifre { get; set; }

    }
}
