using LorePdks.COMMON.DTO.Base;
using LorePdks.COMMON.DTO.Firma;
using LorePdks.COMMON.DTO.FirmaCihaz;
using LorePdks.COMMON.DTO.Kisi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LorePdks.COMMON.DTO.Pdks
{


    public class PdksDTO : BaseDTO
    {
        public FirmaDTO firmaDto { get; set; }
        public KisiDTO kisiDto { get; set; }
        public FirmaCihazDTO firmaCihazDto { get; set; }
        public DateTime? girisTarih { get; set; }
        public DateTime? cikisTarih { get; set; }
    }
}
