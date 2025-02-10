using System;
using System.IO;
using System.Threading;
using System.Reflection;
using Microsoft.Win32;
using System.Threading.Tasks;
using System.Diagnostics;

class Program
{
    private static string appName="TemhaDosyaOkuYaz";
    private static FileSystemWatcher watcher;
    private static bool isProcessing = false;
    private static string kaynakDosyaYolu;
    private static readonly object lockObject = new object();
    private static Mutex mutex = new Mutex(true, "FileWatcherServiceUniqueMutex");
    private static string logDosyaYolu = Path.Combine(
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
        "service_log.txt");

    static async Task Main(string[] args)
    {
        // Uygulama zaten çalışıyor mu kontrol et
        if (!mutex.WaitOne(TimeSpan.Zero, true))
        {
            LogYaz("Uygulama zaten çalışıyor. Yeni instance kapatılıyor.");
            return;
        }

        try
        {
            // Başlangıçta çalıştırma kaydını kontrol et ve ekle
            StartupKaydiEkle();

            // Yapılandırma dosyasından dosya yolunu oku
            string configDosyaYolu = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "config.txt");

            if (!File.Exists(configDosyaYolu))
            {
                Console.WriteLine("Lütfen izlemek istediğiniz txt dosyasının yolunu giriniz:");
                kaynakDosyaYolu = Console.ReadLine();
                File.WriteAllText(configDosyaYolu, kaynakDosyaYolu);
            }
            else
            {
                kaynakDosyaYolu = File.ReadAllText(configDosyaYolu).Trim();
            }

            Console.WriteLine("Sıfırlama yapmak istiyorsanız 'S' tuşuna basın.");
            if (Console.ReadKey(true).Key == ConsoleKey.S)
            {
                Sifirla();
            }

            if (!File.Exists(kaynakDosyaYolu))
            {
                LogYaz("Belirtilen dosya bulunamadı!");
                return;
            }

            // Watcher'ı başlat
            SetupFileWatcher();

            LogYaz("Dosya izleme servisi başlatıldı.");

            // İlk okuma işlemini başlat
            ProcessFile();

            // Programı çalışır durumda tut
            await Task.Delay(-1); // Süresiz beklet
        }
        catch (Exception ex)
        {
            LogYaz($"Kritik hata: {ex.Message}");
        }
        finally
        {
            mutex.ReleaseMutex();
        }
    }
    private static void Sifirla()
    {
        try
        {
            // Registry kaydını sil
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(
                "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
              
                if (key.GetValue(appName) != null)
                {
                    key.DeleteValue(appName);
                    Console.WriteLine("Başlangıç kaydı silindi.");
                }
            }

            // config.txt dosyasını sil
            string configDosyaYolu = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "config.txt");
            if (File.Exists(configDosyaYolu))
            {
                File.Delete(configDosyaYolu);
                Console.WriteLine("config.txt dosyası silindi.");
            }

            // Log dosyasını sil
            string logDosyaYolu = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "service_log.txt");
            if (File.Exists(logDosyaYolu))
            {
                File.Delete(logDosyaYolu);
                Console.WriteLine("service_log.txt dosyası silindi.");
            }

            // Hatalı satır dosyasını sil
            string hataliDosyaYolu = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "hatalilar.txt");
            if (File.Exists(hataliDosyaYolu))
            {
                File.Delete(hataliDosyaYolu);
                Console.WriteLine("hatalilar.txt dosyası silindi.");
            }

            Console.WriteLine("Uygulama başarıyla sıfırlandı.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Sıfırlama sırasında hata oluştu: {ex.Message}");
        }
    }
    private static void StartupKaydiEkle()
    {
        try
        {
            string appPath = Assembly.GetExecutingAssembly().Location;
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(
                "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run",
                true))
            {
        

                // Kayıt yoksa ekle
                if (key.GetValue(appName) == null)
                {
                    key.SetValue(appName, appPath);
                    LogYaz("Başlangıç kaydı eklendi.");
                }
            }
        }
        catch (Exception ex)
        {
            LogYaz($"Başlangıç kaydı eklenirken hata: {ex.Message}");
        }
    }

    private static void LogYaz(string mesaj)
    {
        string logMesaj = $"{DateTime.Now} - {mesaj}";
        Console.WriteLine(logMesaj);

        try
        {
            File.AppendAllText(logDosyaYolu, logMesaj + Environment.NewLine);
        }
        catch { } // Log yazma hatalarını görmezden gel
    }

    private static void SetupFileWatcher()
    {
        string directory = Path.GetDirectoryName(kaynakDosyaYolu);
        string filename = Path.GetFileName(kaynakDosyaYolu);

        watcher = new FileSystemWatcher(directory, filename);
        watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size;
        watcher.Changed += OnFileChanged;
        watcher.EnableRaisingEvents = true;
    }

    private static void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        if (e.ChangeType == WatcherChangeTypes.Changed)
        {
            ProcessFile();
        }
    }

    private static void ProcessFile()
    {
        if (Monitor.TryEnter(lockObject))
        {
            try
            {
                if (isProcessing) return;
                isProcessing = true;

                Thread.Sleep(100);

                string kopyaDosyaYolu = Path.Combine(
                    Path.GetDirectoryName(kaynakDosyaYolu),
                    $"kopya_{Path.GetFileName(kaynakDosyaYolu)}");

                string hataliSatirlarDosyaYolu = Path.Combine(
                    Path.GetDirectoryName(kaynakDosyaYolu),
                    "hatalilar.txt");

                if (new FileInfo(kaynakDosyaYolu).Length == 0)
                {
                    isProcessing = false;
                    return;
                }

                File.Copy(kaynakDosyaYolu, kopyaDosyaYolu, true);
                File.WriteAllText(kaynakDosyaYolu, string.Empty);

                int satirSayisi = 0;
                int hataliSatirSayisi = 0;

                LogYaz("Yeni değişiklikler işleniyor:");

                using (StreamReader sr = new StreamReader(kopyaDosyaYolu))
                {
                    string satir;
                    while ((satir = sr.ReadLine()) != null)
                    {
                        satirSayisi++;
                        try
                        {
                            // http ile bizim api i çağırıp girişi çıkış kaydını yazacağız satır kaydını
                        }
                        catch (Exception ex)
                        {
                            hataliSatirSayisi++;
                            using (StreamWriter sw = new StreamWriter(hataliSatirlarDosyaYolu, true))
                            {
                                sw.WriteLine($"{DateTime.Now} - Satır {satirSayisi}: {satir} - Hata: {ex.Message}");
                            }
                            LogYaz($"Hata - Satır {satirSayisi}: {ex.Message}");
                        }
                    }
                }

                File.Delete(kopyaDosyaYolu);

                LogYaz($"Toplam {satirSayisi} satır okundu.");
                LogYaz($"Toplam {hataliSatirSayisi} hatalı satır bulundu.");

                if (hataliSatirSayisi > 0)
                {
                    LogYaz($"Hatalı satırlar '{hataliSatirlarDosyaYolu}' dosyasına kaydedildi.");
                }
            }
            catch (Exception ex)
            {
                LogYaz($"İşlem sırasında hata oluştu: {ex.Message}");
            }
            finally
            {
                isProcessing = false;
                Monitor.Exit(lockObject);
            }
        }
    }
}