using LorePdks.COMMON.DTO.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LorePdks.COMMON.DTO.Common
{


    public class FirmaCihazDTO : BaseDTO
    {
        public FirmaDTO firmaDto { get; set; }
        public int cihazMakineGercekId { get; set; }
        public KodDTO firmaCihazTipKodDto { get; set; }
        public string ad { get; set; }
        public string aciklama { get; set; }
    }
}
