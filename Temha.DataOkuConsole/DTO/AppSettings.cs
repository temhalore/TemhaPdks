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
        public string KaynakDosyaYolu { get; set; }
        public string HataliDosyaYolu { get; set; }
        public string LogDosyaYolu { get; set; }
        public bool IsDebugMode { get; set; }
        public int MaxRetryCount { get; set; }
    }
}
