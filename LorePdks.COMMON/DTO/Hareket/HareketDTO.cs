using LorePdks.COMMON.DTO.Base;
using LorePdks.COMMON.DTO.Common;
using LorePdks.COMMON.DTO.Firma;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LorePdks.COMMON.DTO.Hareket
{


    public class HareketDTO : BaseDTO
    {
        public FirmaDTO firmaDto { get; set; }
        public KodDTO hareketTipKodDto { get; set; }
        public KodDTO hareketDurumKodDto { get; set; }
        public DateTime? hareketKayitTarih { get; set; }
        public DateTime? hareketIslemeTarih { get; set; }
        public string hareketdata { get; set; }
    }
}
