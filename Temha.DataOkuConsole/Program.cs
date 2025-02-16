using System;
using System.IO;
using System.Threading;
using System.Reflection;
using Microsoft.Win32;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text.Json;
using Temha.DataOkuConsole.DTO;

class Program
{
    private static string appName = "TemhaDosyaOkuYaz";
    private static FileSystemWatcher watcher;
    private static FileSystemWatcher configWatcher;
    private static bool isProcessing = false;
    private static readonly object lockObject = new object();
    private static Mutex mutex = new Mutex(true, "FileWatcherServiceUniqueMutex");
    private static AppConfiguration _configuration;
    private static string configFilePath;

    static async Task Main(string[] args)
    {
        try
        {
            // application.json dosyasını oluştur veya yükle
            InitializeConfiguration();

            // application.json dosyasını izle
            SetupConfigWatcher();

            Console.WriteLine("Kuruluma devam için 'D' tuşuna, var olan kurulumu sıfırlama için 'S' tuşuna basınız.");
            if (Console.ReadKey(true).Key == ConsoleKey.S)
            {
                Console.WriteLine("Sıfırlama için gerekli özel kodu giriniz ve enter'a basınız.");
                if (Console.ReadLine() == "df@ABb9bdNGgSvs62v6f9")
                {
                    Sifirla();
                }
                else
                {
                    LogYaz("Sıfırlama için özel şifre yanlış girildi!");
                    return;
                }
            }

            if (Console.ReadKey(true).Key == ConsoleKey.D)
            {
                if (!mutex.WaitOne(TimeSpan.Zero, true))
                {
                    LogYaz("Uygulama zaten çalışıyor. Yeni instance kapatılıyor.");
                    return;
                }
            }

            if (!File.Exists(_configuration.AppSettings.IzlenecekDosya))
            {
                LogYaz("Belirtilen dosya bulunamadı!");
                return;
            }

            // Dosya izleyiciyi başlat
            SetupFileWatcher();

            LogYaz("Dosya izleme servisi başlatıldı.");
            LogYaz($"Firma İzlenecek Dosya: {_configuration.AppSettings.IzlenecekDosya}");

            // Başlangıçta çalıştırma kaydını kontrol et ve ekle
            StartupKaydiEkle();

            // İlk okuma işlemini başlat
            ProcessFile();

            await Task.Delay(-1);
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

    private static void InitializeConfiguration()
    {
        //KENDİ LOKASYONUNDAN BAKMA KODU
        //configFilePath = Path.Combine(
        //    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
        //    "application.json");
        
        configFilePath = ("C:\\TemhaPdks\\config\\application.json");


        if (!File.Exists(configFilePath))
        {
            LogYaz("Config dosyası bulunamadı!");
            return;
        }

        LoadConfiguration();
    }

    private static void LoadConfiguration()
    {
        string jsonContent = DosyaIslemleri.DosyaOku(configFilePath);
        _configuration = JsonSerializer.Deserialize<AppConfiguration>(jsonContent);

        // Tüm gerekli dosya ve klasörleri kontrol et
        DosyaIslemleri.DosyaKlasorKontrol(_configuration.AppSettings.IzlenecekDosya);
        DosyaIslemleri.DosyaKlasorKontrol(_configuration.AppSettings.HataliDosya);
        DosyaIslemleri.DosyaKlasorKontrol(_configuration.AppSettings.LogDosya);
    }

    private static void SetupConfigWatcher()
    {
        string directory = Path.GetDirectoryName(configFilePath);
        string filename = Path.GetFileName(configFilePath);

        configWatcher = new FileSystemWatcher(directory, filename);
        configWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size;
        configWatcher.Changed += OnConfigurationChanged;
        configWatcher.EnableRaisingEvents = true;
    }

    private static void OnConfigurationChanged(object sender, FileSystemEventArgs e)
    {
        try
        {
            Thread.Sleep(1000); // Dosyanın yazılmasını bekle
            LoadConfiguration();

            string jsonString = JsonSerializer.Serialize(_configuration.AppSettings);
            LogYaz("Yapılandırma dosyası güncellendi.");
            LogYaz($"Yeni Yapılandırma Dosyası: {jsonString}");

            // Dosya izleyiciyi güncelle
            watcher?.Dispose();
            SetupFileWatcher();
        }
        catch (Exception ex)
        {
            LogYaz($"Yapılandırma güncellenirken hata: {ex.Message}");
        }
    }

    private static void LogYaz(string mesaj)
    {
        string logMesaj = $"{DateTime.Now} - {mesaj}";
        Console.WriteLine(logMesaj);

        try
        {
            DosyaIslemleri.DosyayaYazYeniSatir(_configuration.AppSettings.LogDosya, logMesaj);
        }
        catch { }
    }

    private static void SetupFileWatcher()
    {
        string directory = Path.GetDirectoryName(_configuration.AppSettings.IzlenecekDosya);
        string filename = Path.GetFileName(_configuration.AppSettings.IzlenecekDosya);

        watcher = new FileSystemWatcher(directory, filename);
        watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size;
        watcher.Changed += OnFileChanged;
        watcher.EnableRaisingEvents = true;
    }

    private static void ProcessFile()
    {
        if (Monitor.TryEnter(lockObject))
        {
            try
            {
                if (isProcessing) return;
                isProcessing = true;

                Thread.Sleep(1000);

                string kopyaDosya = Path.Combine(
                    Path.GetDirectoryName(_configuration.AppSettings.IzlenecekDosya),
                    $"kopya_{Path.GetFileName(_configuration.AppSettings.IzlenecekDosya)}");

                if (new FileInfo(_configuration.AppSettings.IzlenecekDosya).Length == 0)
                {
                    isProcessing = false;
                    return;
                }

                // Yedek al
                DosyaIslemleri.DosyaYedekAl(_configuration.AppSettings.IzlenecekDosya, "yedek");

                // Kopya oluştur
                DosyaIslemleri.DosyaKopyala(_configuration.AppSettings.IzlenecekDosya, kopyaDosya);

                // Ana dosyayı temizle
                DosyaIslemleri.DosyaTemizle(_configuration.AppSettings.IzlenecekDosya);

                int satirSayisi = 0;
                int hataliSatirSayisi = 0;

                LogYaz("Yeni değişiklikler işleniyor:");

                foreach (string satir in DosyaIslemleri.DosyaSatirlariniOku(kopyaDosya))
                {
                    satirSayisi++;
                    try
                    {
                        if (_configuration.AppSettings.IsDebugMode)
                        {
                            LogYaz($"Debug: İşlenen satır burada apiyi çağıracağız şuan satır bilgisi : {satirSayisi}: {satir}");
                        }
                        // API çağrısı ve diğer işlemler
                    }
                    catch (Exception ex)
                    {
                        hataliSatirSayisi++;
                        DosyaIslemleri.DosyayaYazYeniSatir(
                            _configuration.AppSettings.HataliDosya,
                            $"{DateTime.Now} - Satır {satirSayisi}: {satir} - Hata: {ex.Message}");

                        LogYaz($"Hata - Satır {satirSayisi}: {ex.Message}");
                    }
                }

                // Kopya dosyayı sil
                DosyaIslemleri.DosyaSil(kopyaDosya, false);

                LogYaz($"Toplam {satirSayisi} satır okundu.");
                LogYaz($"Toplam {hataliSatirSayisi} hatalı satır bulundu.");
            }
            catch (Exception ex)
            {
                LogYaz($"Data işleme sırasında hata oluştu: {ex.Message}");
            }
            finally
            {
                isProcessing = false;
                Monitor.Exit(lockObject);
            }
        }
    }

    private static void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        if (e.ChangeType == WatcherChangeTypes.Changed)
        {
            ProcessFile();
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
                if (key.GetValue(appName) == null)
                {
                    key.SetValue(appName, appPath);
                    LogYaz("Başlangıç kaydı Registry'ye eklendi.");
                }
            }
        }
        catch (Exception ex)
        {
            LogYaz($"Başlangıç kaydı Registry'ye eklenirken hata: {ex.Message}");
        }
    }

    private static void Sifirla()
    {
        try
        {
            LogYaz("Sıfırlama işlemi başlatıldı:");

            // Registry kaydını sil
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(
                "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                if (key.GetValue(appName) != null)
                {
                    key.DeleteValue(appName);
                    LogYaz("Registry kaydı silindi.");
                }
            }

            // Config dosyasını sil
            DosyaIslemleri.DosyaSil(configFilePath,true, "sifirlamaOncesi");

            // Hatalı dosyayı sil
            DosyaIslemleri.DosyaSil(_configuration.AppSettings.HataliDosya, true, "sifirlamaOncesi");

            // Log dosyasını sil
            DosyaIslemleri.DosyaSil(_configuration.AppSettings.LogDosya, true, "sifirlamaOncesi");

            LogYaz("Uygulama başarıyla sıfırlandı.");
        }
        catch (Exception ex)
        {
            LogYaz($"Sıfırlama sırasında hata oluştu: {ex.Message}");
        }
    }
}