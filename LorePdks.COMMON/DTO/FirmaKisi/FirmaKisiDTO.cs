﻿using LorePdks.COMMON.DTO.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LorePdks.COMMON.DTO.Common
{


    public class FirmaKisiDTO : BaseDTO
    {
        public FirmaDTO firmaDto { get; set; }
        public KisiDTO kisiDto { get; set; }
        public KodDTO firmaKisiTipKodDto { get; set; }
    }
}
