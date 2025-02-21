using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Temha.DataOkuConsole.DTO.configModel
{
    public class AppSettings
    {
        public string FirmaKod { get; set; }
        public string IzlenecekDosya { get; set; }
        public bool IsDebugMode { get; set; } = false;
    }
}
