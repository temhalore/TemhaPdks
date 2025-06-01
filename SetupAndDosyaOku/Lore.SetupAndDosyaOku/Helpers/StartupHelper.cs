using Microsoft.Win32;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Lore.SetupAndDosyaOku.Helpers
{
    public class StartupHelper
    {
        [DllImport("shell32.dll")]
        static extern bool SHGetSpecialFolderPath(IntPtr hwndOwner, [Out] StringBuilder lpszPath, int nFolder, bool fCreate);
        
        private const int CSIDL_DESKTOP = 0x0000; // Masaüstü
        private const int CSIDL_STARTUP = 0x0007; // Başlangıç klasörü
          private readonly Logger _logger;
        
        public StartupHelper(Logger logger)
        {
            _logger = logger;
        }
        
        /// <summary>
        /// Uygulamayı Windows başlangıcına ekler
        /// </summary>
        public bool AddToStartup()
        {
            try
            {
                _logger.Info("Uygulama Windows başlangıcına ekleniyor...");
                
                string appName = "LoreSetupAndDosyaOku";
                string exePath = Assembly.GetExecutingAssembly().Location;
                
                // 1. Yöntem: Windows başlangıç klasörüne kısayol ekle
                string startupFolderPath = GetSpecialFolderPath(CSIDL_STARTUP);
                string shortcutPath = Path.Combine(startupFolderPath, $"{appName}.lnk");
                
                if (CreateShortcut(shortcutPath, exePath, "Lore SetupAndDosyaOku"))
                {
                    _logger.Info("Başlangıç klasörüne kısayol başarıyla eklendi.");
                }
                else
                {
                    _logger.Warning("Başlangıç klasörüne kısayol eklenemedi.");
                }
                
                // 2. Yöntem: Registry'e kayıt ekle
                using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    if (key != null)
                    {
                        key.SetValue(appName, exePath);
                        _logger.Info("Registry'e başlangıç kaydı başarıyla eklendi.");
                        return true;
                    }
                    else
                    {
                        _logger.Error("Registry'e başlangıç kaydı eklenemedi: Run anahtarı açılamadı.");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Windows başlangıcına ekleme işlemi sırasında hata oluştu", ex);
                return false;
            }
        }
          /// <summary>
        /// Uygulamayı Windows başlangıcından kaldırır
        /// </summary>
        public bool RemoveFromStartup()
        {
            try
            {
                _logger.Info("Uygulama Windows başlangıcından kaldırılıyor...");
                
                string appName = "LoreSetupAndDosyaOku";
                
                // 1. Yöntem: Windows başlangıç klasöründeki kısayolu sil
                string startupFolderPath = GetSpecialFolderPath(CSIDL_STARTUP);
                string shortcutPath = Path.Combine(startupFolderPath, $"{appName}.lnk");
                
                if (File.Exists(shortcutPath))
                {
                    File.Delete(shortcutPath);
                    _logger.Info("Başlangıç klasöründeki kısayol başarıyla silindi.");
                }
                
                // 2. Yöntem: Registry'deki kaydı sil
                using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    if (key != null)
                    {
                        key.DeleteValue(appName, false);
                        _logger.Info("Registry'deki başlangıç kaydı başarıyla silindi.");
                        return true;
                    }
                    else
                    {
                        _logger.Error("Registry'deki başlangıç kaydı silinemedi: Run anahtarı açılamadı.");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Windows başlangıcından kaldırma işlemi sırasında hata oluştu", ex);
                return false;
            }
        }
          /// <summary>
        /// Masaüstüne kısayol oluşturur
        /// </summary>
        public bool CreateDesktopShortcut()
        {
            try
            {
                _logger.Info("Masaüstüne kısayol oluşturuluyor...");
                
                string appName = "LoreSetupAndDosyaOku";
                string exePath = Assembly.GetExecutingAssembly().Location;
                string desktopPath = GetSpecialFolderPath(CSIDL_DESKTOP);
                string shortcutPath = Path.Combine(desktopPath, $"{appName}.lnk");
                
                bool result = CreateShortcut(shortcutPath, exePath, "Lore SetupAndDosyaOku");
                
                if (result)
                    _logger.Info("Masaüstüne kısayol başarıyla oluşturuldu.");
                else
                    _logger.Warning("Masaüstüne kısayol oluşturulamadı.");
                    
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error("Masaüstüne kısayol oluşturulurken hata oluştu", ex);
                return false;
            }
        }        /// <summary>
        /// Kısayol dosyası oluşturur
        /// </summary>
        private bool CreateShortcut(string shortcutPath, string targetPath, string description)
        {
            try
            {
                // Windows Script Host kullanarak kısayol oluştur
                string tempVbsPath = Path.Combine(Path.GetTempPath(), "CreateShortcut.vbs");
                
                // Windows Script Host komutu
                string vbsContent = $@"
Set objWSH = WScript.CreateObject(""WScript.Shell"")
Set objShortcut = objWSH.CreateShortcut(""{shortcutPath.Replace("\\", "\\\\")}"")
objShortcut.TargetPath = ""{targetPath.Replace("\\", "\\\\")}"" 
objShortcut.Description = ""{description}""
objShortcut.WorkingDirectory = ""{Path.GetDirectoryName(targetPath).Replace("\\", "\\\\")}"" 
objShortcut.Save
";
                
                File.WriteAllText(tempVbsPath, vbsContent);
                
                // VBS betiğini çalıştır
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = "cscript.exe";
                    process.StartInfo.Arguments = $"\"{tempVbsPath}\" //Nologo";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();
                    process.WaitForExit();
                    
                    // Geçici VBS dosyasını sil
                    if (File.Exists(tempVbsPath))
                        File.Delete(tempVbsPath);
                    
                    return process.ExitCode == 0;
                }
            }
            catch (Exception ex)
            {
                Logger.WriteError($"Kısayol oluşturulurken hata oluştu: {shortcutPath}", ex);
                return false;
            }
        }
          /// <summary>
        /// Özel klasör yolunu döndürür
        /// </summary>
        private string GetSpecialFolderPath(int csidl)
        {
            StringBuilder path = new StringBuilder(260);
            SHGetSpecialFolderPath(IntPtr.Zero, path, csidl, false);
            return path.ToString();
        }
    }
}
