using LorePdks.COMMON.DTO.Base;
using LorePdks.COMMON.DTO.Common;
using LorePdks.COMMON.DTO.Firma;
using LorePdks.COMMON.DTO.Kisi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LorePdks.COMMON.DTO.FirmaKisi
{


    public class FirmaKisiDTO : BaseDTO
    {
        public FirmaDTO firmaDto { get; set; }
        public KisiDTO kisiDto { get; set; }
        public KodDTO firmaKisiTipKodDto { get; set; }
    }
}
