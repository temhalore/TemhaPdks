using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;

namespace Temha.DataOkuConsole
{
    public static class ServiceInstaller
    {
        private const string serviceName = "TemhaDataOkuConsole";
        private const string displayName = "Temha Data Oku Servisi";
        private const string description = "Temha için veri okuma ve işleme Windows servisi";

        public static bool Install()
        {
            try
            {
                // Uygulama dosya yolunu al
                string exePath = Assembly.GetExecutingAssembly().Location;
                
                // Servis kurulumunu UAC yükseltilmiş haklar ile yapmak için
                // sc.exe komutunu kullanıyoruz (Windows Servis Kontrol aracı)
                
                // Önce servisi oluşturalım
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "sc.exe",
                    // Servisin otomatik başlatılması için "start= auto" parametresi önemli
                    Arguments = $"create {serviceName} binPath= \"{exePath}\" start= auto DisplayName= \"{displayName}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(startInfo))
                {
                    if (process == null)
                    {
                        Console.WriteLine("Servis kurulum işlemi başlatılamadı.");
                        return false;
                    }
                    
                    process.WaitForExit();
                    if (process.ExitCode != 0)
                    {
                        Console.WriteLine($"Servis kurulumu başarısız oldu. Hata kodu: {process.ExitCode}");
                        return false;
                    }
                }

                // Servis açıklamasını ayarla
                startInfo.Arguments = $"description {serviceName} \"{description}\"";
                using (Process process = Process.Start(startInfo))
                {
                    if (process == null)
                    {
                        Console.WriteLine("Servis açıklama ayarlama işlemi başlatılamadı.");
                        return true; // Servis kuruldu ama açıklama ayarlanamadı, yine de başarılı sayıyoruz
                    }
                    
                    process.WaitForExit();
                }

                // Servisi başlat
                startInfo.Arguments = $"start {serviceName}";
                using (Process process = Process.Start(startInfo))
                {
                    if (process != null)
                    {
                        process.WaitForExit();
                        if (process.ExitCode == 0)
                        {
                            Console.WriteLine($"Servis başarıyla başlatıldı: {serviceName}");
                        }
                        else
                        {
                            Console.WriteLine($"Servis kuruldu fakat başlatılamadı. Hata kodu: {process.ExitCode}");
                        }
                    }
                }

                Console.WriteLine($"Servis başarıyla kuruldu: {serviceName}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Servis kurulumu sırasında hata oluştu: {ex.Message}");
                return false;
            }
        }

        public static bool Uninstall()
        {
            try
            {
                // Servisi durdur
                bool serviceStopped = StopService();
                
                // Servisi kaldır
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "sc.exe",
                    Arguments = $"delete {serviceName}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(startInfo))
                {
                    if (process == null)
                    {
                        Console.WriteLine("Servis kaldırma işlemi başlatılamadı.");
                        return false;
                    }
                    
                    process.WaitForExit();
                    if (process.ExitCode != 0)
                    {
                        Console.WriteLine($"Servis kaldırma işlemi başarısız oldu. Hata kodu: {process.ExitCode}");
                        return false;
                    }
                }

                Console.WriteLine($"Servis başarıyla kaldırıldı: {serviceName}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Servis kaldırma sırasında hata oluştu: {ex.Message}");
                return false;
            }
        }
        
        private static bool StopService()
        {
            try
            {
                // Servisi durdur
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "sc.exe",
                    Arguments = $"stop {serviceName}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(startInfo))
                {
                    if (process == null)
                    {
                        return false;
                    }
                    
                    process.WaitForExit();
                    // Servis durdurulurken yeterince bekle
                    Thread.Sleep(2000);
                    return process.ExitCode == 0;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
