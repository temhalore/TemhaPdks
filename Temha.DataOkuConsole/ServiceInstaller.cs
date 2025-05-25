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
        }        public static bool Uninstall()
        {
            try
            {
                bool isServiceInstalled = false;

                // Servisin kurulu olup olmadığını kontrol et
                try
                {
                    using (ServiceController sc = new ServiceController(serviceName))
                    {
                        isServiceInstalled = true;
                    }
                }
                catch
                {
                    // Servis kurulamadıysa işlemi tamamlayalım
                    Console.WriteLine($"Servis zaten kurulu değil: {serviceName}");
                    return true;
                }

                if (isServiceInstalled)
                {
                    // Servisi durdur
                    bool serviceStopped = StopService();
                    
                    if (!serviceStopped)
                    {
                        Console.WriteLine("Servis durdurulamadı. Silme işlemi başarısız olabilir.");
                        // Servisi durduramadık, ama yine de silme işlemine devam edelim
                    }
                    
                    // Tüm servis işlemlerinin sonlanmasını bekle
                    Thread.Sleep(2000);
                    
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
                            
                            // Eğer servis hala çalışıyorsa zorla sonlandır
                            try
                            {
                                KillServiceProcess();
                                // Tekrar silmeyi dene
                                Thread.Sleep(1000);
                                using (Process process2 = Process.Start(startInfo))
                                {
                                    process2?.WaitForExit();
                                }
                            }
                            catch(Exception ex)
                            {
                                Console.WriteLine($"Servis işlemi sonlandırılamazken hata: {ex.Message}");
                                return false;
                            }
                        }
                    }

                    // Mutant (tek örnek kontrolü) temizliğini yap
                    CleanupMutex();

                    Console.WriteLine($"Servis başarıyla kaldırıldı: {serviceName}");
                }
                
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
                // Önce ServiceController ile servisi durdurmayı dene
                try
                {
                    using (ServiceController sc = new ServiceController(serviceName))
                    {
                        if (sc.Status != ServiceControllerStatus.Stopped && sc.Status != ServiceControllerStatus.StopPending)
                        {
                            Console.WriteLine($"Servis durduruluyor: {serviceName}");
                            sc.Stop();
                            // Servis durdurulurken yeterince bekle (maksimum 30 saniye)
                            sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
                            return sc.Status == ServiceControllerStatus.Stopped;
                        }
                        // Servis zaten durdurulmuş
                        return true;
                    }
                }
                catch (InvalidOperationException)
                {
                    // Servis bulunamadı, SC.exe ile devam et
                }

                // Servis bulunamazsa SC komutu ile durdur
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
                        Console.WriteLine("Servis durdurma işlemi başlatılamadı.");
                        return false;
                    }
                    
                    process.WaitForExit();
                    // Servis durdurulurken yeterince bekle
                    Thread.Sleep(3000);
                    return process.ExitCode == 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Servis durdurma sırasında hata: {ex.Message}");
                return false;
            }
        }

        private static void KillServiceProcess()
        {
            try
            {
                // Servis işlemlerini bul ve sonlandır
                Process[] processes = Process.GetProcessesByName("Temha.DataOkuConsole");
                foreach (Process process in processes)
                {
                    try
                    {
                        if (!process.HasExited)
                        {
                            process.Kill();
                            process.WaitForExit(5000); // 5 saniye bekle
                        }
                    }
                    catch
                    {
                        // İşlem sonlandırılamadı, devam et
                    }
                    finally
                    {
                        process.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Servis işlemlerini sonlandırırken hata: {ex.Message}");
            }
        }

        private static void CleanupMutex()
        {
            try
            {
                // Windows mutex temizliği
                // Not: Bu Mutex adı Program.cs'deki ile eşleşmeli
                string mutexName = "FileWatcherServiceUniqueMutex";
                
                // OpenExisting ile mutex'i açmayı dene
                bool mutexAbandoned = false;
                System.Threading.Mutex mutex = null;

                try
                {
                    mutex = System.Threading.Mutex.OpenExisting(mutexName);
                    if (mutex != null)
                    {
                        try 
                        {
                            // Mutex'i serbest bırak
                            mutex.ReleaseMutex();
                            mutexAbandoned = true;
                        }
                        catch { /* Mutex zaten serbest bırakılmış olabilir */ }
                    }
                }
                catch (WaitHandleCannotBeOpenedException)
                {
                    // Mutex zaten yok, sorun değil
                    mutexAbandoned = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Mutex temizlenirken hata: {ex.Message}");
                }
                finally
                {
                    mutex?.Dispose();
                }

                // Mutex temizlenemediyse, işlemleri sonlandırmaya çalış
                if (!mutexAbandoned)
                {
                    KillServiceProcess();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Mutex temizleme sırasında hata: {ex.Message}");
            }
        }
    }
}
