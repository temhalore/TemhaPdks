using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Temha.DataOkuConsole.DTO
{
    public class AppSettings
    {
        public string FirmaKod { get; set; }
        public string IzlenecekDosya { get; set; }
        public string HataliDosya { get; set; }
        public string LogDosya { get; set; }
        public bool IsDebugMode { get; set; }
    }
}
