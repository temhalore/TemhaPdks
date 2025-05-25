using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LorePdks.COMMON.DTO.DataOkuConsole
{
    /// <summary>
    /// DataOkuConsole versiyonu için kullanılan DTO sınıfı
    /// </summary>
    public class DataOkuVersiyon
    {
        /// <summary>
        /// İşlem sonucu başarılı mı
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// İşlem mesajı
        /// </summary>
        public string Message { get; set; }
        
        /// <summary>
        /// Versiyon numarası
        /// </summary>
        public string Version { get; set; }
    }
      /// <summary>
    /// DataOkuConsole Setup versiyonu ve indirme bilgileri için kullanılan DTO sınıfı
    /// </summary>
    public class DataOkuSetupVersiyon
    {
        /// <summary>
        /// Versiyon numarası
        /// </summary>
        public string Version { get; set; }
        
        /// <summary>
        /// Setup dosyasının indirme bağlantısı
        /// </summary>
        public string SetupUrl { get; set; }
        
        /// <summary>
        /// Sürüm notları
        /// </summary>
        public string ReleaseNotes { get; set; }

        /// <summary>
        /// Varsayılan kurulum yolu
        /// </summary>
        public string DefaultInstallPath { get; set; }

        /// <summary>
        /// Çalıştırılacak uygulamanın exe dosya adı
        /// </summary>
        public string ExecutablePath { get; set; }
    }
}
