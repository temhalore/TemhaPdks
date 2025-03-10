using LorePdks.COMMON.DTO.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LorePdks.COMMON.DTO.Common
{


    public class PdksHareketDTO : BaseDTO
    {
        public PdksDTO pdksDto { get; set; }
        public HareketDTO hareketDto { get; set; }
     
    }
}
