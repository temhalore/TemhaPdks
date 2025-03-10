using LorePdks.COMMON.DTO.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LorePdks.COMMON.DTO.Common
{


    public class FirmaDTO : BaseDTO
    {
        public string ad { get; set; }
        public string kod { get; set; }
        public string aciklama { get; set; }
        public string adres { get; set; }
        public int mesaiSaat { get; set; }
        public int molaSaat { get; set; }
        public int cumartesiMesaiSaat { get; set; }
        public int cumartesiMolaSaat { get; set; }
    }
}
