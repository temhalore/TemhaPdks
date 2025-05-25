using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;

namespace Temha.DataOkuConsole
{
    public static class ServiceInstaller
    {
        private const string serviceName = "TemhaDataOkuConsole";
        private const string displayName = "Temha Data Oku Servisi";
        private const string description = "Temha için veri okuma ve işleme Windows servisi";        public static bool Install()
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
                    RedirectStandardError = true,
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
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    
                    if (process.ExitCode != 0)
                    {
                        if (process.ExitCode == 1073 || error.Contains("1073") || output.Contains("1073"))
                        {
                            // 1073 = Servis zaten mevcut hatası
                            Console.WriteLine("Servis zaten kurulu. Güncellemeye çalışılıyor...");
                            
                            // Önce eski servisi kaldır, sonra yeni servisi kur
                            Console.WriteLine("Eski servis kaldırılıyor...");
                            if (!Uninstall())
                            {
                                Console.WriteLine("Eski servis kaldırılamadı.");
                                return false;
                            }
                            
                            // Kısa bir süre bekle
                            Thread.Sleep(2000);
                            
                            // Tekrar kurmayı dene
                            using (Process process2 = Process.Start(startInfo))
                            {
                                if (process2 == null)
                                {
                                    Console.WriteLine("İkinci kurulum denemesi başlatılamadı.");
                                    return false;
                                }
                                
                                process2.WaitForExit();
                                if (process2.ExitCode != 0)
                                {
                                    Console.WriteLine($"İkinci kurulum denemesi de başarısız oldu. Hata kodu: {process2.ExitCode}");
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Servis kurulumu başarısız oldu. Hata kodu: {process.ExitCode}");
                            Console.WriteLine($"Hata: {error}");
                            Console.WriteLine($"Çıktı: {output}");
                            return false;
                        }
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
                    // Açıklama ayarlama hatasını görmezden gel
                }

                // Servisi başlat
                startInfo.Arguments = $"start {serviceName}";
                using (Process process = Process.Start(startInfo))
                {
                    if (process != null)
                    {
                        process.WaitForExit();
                        string output = process.StandardOutput.ReadToEnd();
                        string error = process.StandardError.ReadToEnd();
                        
                        if (process.ExitCode == 0)
                        {
                            Console.WriteLine($"Servis başarıyla başlatıldı: {serviceName}");
                        }
                        else if (process.ExitCode == 1056 || error.Contains("1056") || output.Contains("1056"))
                        {
                            // 1056 = Servis zaten çalışıyor
                            Console.WriteLine("Servis zaten çalışıyor.");
                        }
                        else
                        {
                            Console.WriteLine($"Servis kuruldu fakat başlatılamadı. Hata kodu: {process.ExitCode}");
                            Console.WriteLine($"Hata: {error}");
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
        }public static bool Uninstall()
        {
            try
            {
                bool isServiceInstalled = false;

                // Servisin kurulu olup olmadığını kontrol et
                try
                {
                    // ServiceController.GetServices() ile daha güvenilir kontrol
                    ServiceController[] services = ServiceController.GetServices();
                    isServiceInstalled = services.Any(s => s.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase));
                    
                    if (!isServiceInstalled)
                    {
                        Console.WriteLine($"Servis zaten kurulu değil: {serviceName}");
                        
                        // Yine de işlemleri temizle (eğer zombie process varsa)
                        KillServiceProcess();
                        CleanupMutex();
                        
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Servis kontrolünde hata: {ex.Message}");
                    // Hata durumunda bile temizleme işlemlerini yapalım
                    KillServiceProcess();
                    CleanupMutex();
                    return true;
                }

                if (isServiceInstalled)
                {
                    // Servisi durdur
                    bool serviceStopped = StopService();
                    
                    if (!serviceStopped)
                    {
                        Console.WriteLine("Servis normal yolla durdurulamadı. Zorla sonlandırılmaya çalışılıyor...");
                        // Servisi durduramadık, zorla sonlandırmaya çalış
                        KillServiceProcess();
                    }
                    
                    // Tüm servis işlemlerinin sonlanmasını bekle
                    Thread.Sleep(3000);
                    
                    // Servisi kaldır
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = "sc.exe",
                        Arguments = $"delete {serviceName}",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
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
                        string output = process.StandardOutput.ReadToEnd();
                        string error = process.StandardError.ReadToEnd();
                        
                        if (process.ExitCode != 0)
                        {
                            // Hata kodunu kontrol et
                            if (process.ExitCode == 1060 || error.Contains("1060") || output.Contains("1060"))
                            {
                                // 1060 = Servis mevcut değil hatası - Bu durumda başarılı kabul et
                                Console.WriteLine($"Servis zaten mevcut değil (Hata 1060). Temizlik işlemleri yapılıyor...");
                            }
                            else
                            {
                                Console.WriteLine($"Servis kaldırma işlemi başarısız oldu. Hata kodu: {process.ExitCode}");
                                Console.WriteLine($"Hata mesajı: {error}");
                                Console.WriteLine($"Çıktı: {output}");
                                
                                // Eğer servis hala çalışıyorsa zorla sonlandır
                                try
                                {
                                    KillServiceProcess();
                                    // Tekrar silmeyi dene
                                    Thread.Sleep(2000);
                                    using (Process process2 = Process.Start(startInfo))
                                    {
                                        if (process2 != null)
                                        {
                                            process2.WaitForExit();
                                            if (process2.ExitCode == 0 || process2.ExitCode == 1060)
                                            {
                                                Console.WriteLine("İkinci denemede servis başarıyla kaldırıldı.");
                                            }
                                            else
                                            {
                                                Console.WriteLine($"İkinci denemede de başarısız oldu. Hata kodu: {process2.ExitCode}");
                                                return false;
                                            }
                                        }
                                    }
                                }
                                catch(Exception ex)
                                {
                                    Console.WriteLine($"Servis işlemi sonlandırılamazken hata: {ex.Message}");
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Servis başarıyla kaldırıldı.");
                        }
                    }

                    // Mutant (tek örnek kontrolü) temizliğini yap
                    CleanupMutex();

                    Console.WriteLine($"Servis kaldırma işlemi tamamlandı: {serviceName}");
                }
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Servis kaldırma sırasında hata oluştu: {ex.Message}");
                
                // Hata durumunda bile temizlik işlemlerini yapmaya çalış
                try
                {
                    KillServiceProcess();
                    CleanupMutex();
                }
                catch (Exception cleanupEx)
                {
                    Console.WriteLine($"Temizlik işlemleri sırasında hata: {cleanupEx.Message}");
                }
                
                return false;
            }
        }        public static bool IsServiceInstalled()
        {
            try
            {
                ServiceController[] services = ServiceController.GetServices();
                return services.Any(s => s.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Servis varlığı kontrol edilirken hata: {ex.Message}");
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
                        // Servisin durumunu kontrol et
                        if (sc.Status != ServiceControllerStatus.Stopped && sc.Status != ServiceControllerStatus.StopPending)
                        {
                            Console.WriteLine($"Servis durduruluyor: {serviceName}");
                            sc.Stop();
                            // Servis durdurulurken yeterince bekle (maksimum 30 saniye)
                            sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
                            return sc.Status == ServiceControllerStatus.Stopped;
                        }
                        // Servis zaten durdurulmuş
                        Console.WriteLine($"Servis zaten durdurulmuş: {serviceName}");
                        return true;
                    }
                }
                catch (InvalidOperationException ex)
                {
                    // Servis bulunamadı
                    Console.WriteLine($"ServiceController ile servis bulunamadı: {ex.Message}");
                    // SC.exe ile devam et
                }
                catch (System.ComponentModel.Win32Exception ex)
                {
                    // Erişim hatası vs.
                    Console.WriteLine($"ServiceController erişim hatası: {ex.Message}");
                    // SC.exe ile devam et
                }

                // Servis bulunamazsa veya ServiceController çalışmazsa SC komutu ile durdur
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "sc.exe",
                    Arguments = $"stop {serviceName}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
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
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    
                    // Servis durdurulurken yeterince bekle
                    Thread.Sleep(3000);
                    
                    // Hata kodlarını kontrol et
                    if (process.ExitCode == 0)
                    {
                        Console.WriteLine("Servis SC komutu ile başarıyla durduruldu.");
                        return true;
                    }
                    else if (process.ExitCode == 1060 || error.Contains("1060") || output.Contains("1060"))
                    {
                        // 1060 = Servis mevcut değil hatası - Bu durumda başarılı kabul et
                        Console.WriteLine("Servis zaten mevcut değil (Hata 1060).");
                        return true;
                    }
                    else if (process.ExitCode == 1062 || error.Contains("1062") || output.Contains("1062"))
                    {
                        // 1062 = Servis başlatılmamış - Bu durumda da başarılı kabul et
                        Console.WriteLine("Servis zaten durdurulmuş (Hata 1062).");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"SC durdurma komutu başarısız. Hata kodu: {process.ExitCode}");
                        Console.WriteLine($"Hata: {error}");
                        Console.WriteLine($"Çıktı: {output}");
                        return false;
                    }
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
