using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Temha.DataOkuConsole.DTO.ResReqDto.Req
{


    public class HareketDTO 
    {
        public FirmaDTO firmaDto { get; set; }
        public KodDTO hareketTipKodDto { get; set; }
        public KodDTO hareketDurumKodDto { get; set; }
        public DateTime? hareketKayitTarih { get; set; }
        public DateTime? hareketIslemeTarih { get; set; }
        public string hareketdata { get; set; }
    }
}
