
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Temha.DataOkuConsole.DTO.ResReqDto.Req
{


    public class FirmaDTO 
    {
        public string ad { get; set; }
        public string kod { get; set; }
        public string aciklama { get; set; }
        public string adres { get; set; }
        public decimal? mesaiSaat { get; set; }
        public decimal? molaSaat { get; set; }
        public decimal? cumartesiMesaiSaat { get; set; }
        public decimal? cumartesiMolaSaat { get; set; }
    }
}
