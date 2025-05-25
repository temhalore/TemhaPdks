using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Temha.DataOku.SetupDownloader
{
    public class AppConfiguration
    {
        public AppSettings AppSettings { get; set; }
        public CoreSettings CoreSettings { get; set; }
    }

    public class AppSettings
    {
        public string FirmaKod { get; set; }
        public string IzlenecekDosya { get; set; }
        public bool IsDebugMode { get; set; } = false;
    }

    public class CoreSettings
    {
        public string HataliDosya { get; set; }
    }
}
