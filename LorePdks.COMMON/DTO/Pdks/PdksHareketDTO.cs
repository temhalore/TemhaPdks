using LorePdks.COMMON.DTO.Base;
using LorePdks.COMMON.DTO.Hareket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LorePdks.COMMON.DTO.Pdks
{


    public class PdksHareketDTO : BaseDTO
    {
        public PdksDTO pdksDto { get; set; }
        public HareketDTO hareketDto { get; set; }

    }
}
