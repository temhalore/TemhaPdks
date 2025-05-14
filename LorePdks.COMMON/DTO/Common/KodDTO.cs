using LorePdks.COMMON.DTO.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LorePdks.COMMON.DTO.Common
{


    public class KodDTO:BaseDTO
    {
        //public int id { get; set; }
        public int tipId { get; set; }
        public string kod { get; set; }
        public string kisaAd { get; set; }
        public int sira { get; set; }

        public string digerUygEnumAd { get; set; }
        public int digerUygEnumDeger { get; set; }

      

    }
}
