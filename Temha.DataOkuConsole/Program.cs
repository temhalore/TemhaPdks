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

          
            Console.WriteLine("Kuruluma  devam için 'D' tuşuna, var olan kurulumu sıfırlama için 'S' tuşuna basınız.");
            if (Console.ReadKey(true).Key == ConsoleKey.S)
            {
                Console.WriteLine("Sıfırlama için gerekli özel kodu giriniz ve enter a basınız.");
                if (Console.ReadLine() == "df@ABb9bdNGgSvs62v6f9")
                {
                    Sifirla();
                }
                else
                {
                    LogYaz("Sıfırlama için özel şifre yanlış girildi.!");
                    return;
                }
            }
            //devam dendiğinde kontrol et çalışıyorsa işlem yaptırma sıfırlama yapsaı gerek yada config dosyasını düzneleyebilir
            if (Console.ReadKey(true).Key == ConsoleKey.D)
            {
                if (!mutex.WaitOne(TimeSpan.Zero, true))
                {
                    LogYaz("Uygulama zaten çalışıyor. Yeni instance kapatılıyor.");
                    return;
                }

                // Console.WriteLine("İzlenecek Dosya Yolunu giriniz.Örnek 'C:\\TemhaPdks\\data.txt'");
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

            // Başlangıçta çalıştırma kaydını kontrol et ve ekle oto başlangıç için
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
        configFilePath = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "application.json");

        //dosya yoksa hata verir
        if (!File.Exists(configFilePath))
        {
            LogYaz("configFilePath te application dosyası bulunamadı!");
            return;
        }

        // Tüm gerekli dosya ve klasörleri kontrol et
        DosyaIslemleri.DosyaKlasorKontrol(_configuration.AppSettings.IzlenecekDosya);
        DosyaIslemleri.DosyaKlasorKontrol(_configuration.AppSettings.HataliDosya);
        DosyaIslemleri.DosyaKlasorKontrol(_configuration.AppSettings.LogDosya);

        LoadConfiguration();
    }

    ///// <summary>
    ///// default dataları  app settings e yazar unu dışarıdan ilk çalışmada alabiliriz bakcağız şimdilik buradan sabit yada bu hiç olmayacak
    ///// </summary>
    //private static void CreateDefaultConfiguration(AppSettings appSetiings)
    //{
    //    var defaultConfig = new AppConfiguration
    //    {
    //        AppSettings = new AppSettings
    //        {
    //            FirmaKod = "FIRMA001",
    //            IzlenecekDosya = Path.Combine(
    //                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
    //                "data.txt"),
    //            HataliDosya = Path.Combine(
    //                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
    //                "hatalilar.txt"),
    //            LogDosya = Path.Combine(
    //                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
    //                "service_log.txt"),
    //            IsDebugMode = true
    //        }
    //    };

    //    string jsonContent = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions
    //    {
    //        WriteIndented = true
    //    });

    //    File.WriteAllText(configFilePath, jsonContent);
    //}

    private static void LoadConfiguration()
    {
        string jsonContent = File.ReadAllText(configFilePath);
        _configuration = JsonSerializer.Deserialize<AppConfiguration>(jsonContent);
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

    /// <summary>
    /// confşg osyası takipta değişiklik varsa işlemleri yenilemek için çalışan metod
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void OnConfigurationChanged(object sender, FileSystemEventArgs e)
    {
        try
        {
            Thread.Sleep(1000); // Dosyanın yazılmasını bekle
            LoadConfiguration();

            string jsonSring= JsonSerializer.Serialize(_configuration.AppSettings);
            LogYaz("Yapılandırma dosyası güncellendi.");
            LogYaz($"Yeni Yapılandırma Dosyası: {jsonSring}");
           
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
            File.AppendAllText(_configuration.AppSettings.LogDosya, logMesaj + Environment.NewLine);
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

    // ProcessFile metodunda yapılan değişiklikler
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

                // her işlemde yedek alalım
                var simdi = DateTime.Now;
                string yedekDate = simdi.Year.ToString()+simdi.Month.ToString() + simdi.Day.ToString()+"-"+simdi.Hour.ToString()+simdi.Minute.ToString()+simdi.Second.ToString();
                
                string yedekDosya = Path.Combine(
    Path.GetDirectoryName(_configuration.AppSettings.IzlenecekDosya),
    $"yedek_{yedekDate}_{Path.GetFileName(_configuration.AppSettings.IzlenecekDosya)}");


                if (new FileInfo(_configuration.AppSettings.IzlenecekDosya).Length == 0)
                {
                    isProcessing = false;
                    return;
                }
                File.Copy(_configuration.AppSettings.IzlenecekDosya, yedekDosya, true);
                File.Copy(_configuration.AppSettings.IzlenecekDosya, kopyaDosya, true);
                Thread.Sleep(1000);
                File.WriteAllText(_configuration.AppSettings.IzlenecekDosya, string.Empty);

                int satirSayisi = 0;
                int hataliSatirSayisi = 0;


                LogYaz("Yeni değişiklikler işleniyor:");

                using (StreamReader sr = new StreamReader(kopyaDosya))
                {
                    string satir;
                    while ((satir = sr.ReadLine()) != null)
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
                            using (StreamWriter sw = new StreamWriter(_configuration.AppSettings.HataliDosya, true))
                            {
                                sw.WriteLine($"{DateTime.Now} - Satır {satirSayisi}: {satir} - Hata: {ex.Message}");
                            }
                            LogYaz($"Hata - Satır {satirSayisi}: {ex.Message}");
                        }
                    }
                }
                // kopya dosyamızla işimiz bitti sil
                File.Delete(kopyaDosya);

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
                // Kayıt yoksa ekle
                if (key.GetValue(appName) == null)
                {
                    key.SetValue(appName, appPath);
                    LogYaz("Başlangıç kaydı Registry ye eklendi.");
                }
            }
        }
        catch (Exception ex)
        {
            LogYaz($"Başlangıç kaydı Registry ye eklenirken hata: {ex.Message}");
        }
    }
    /// <summary>
    /// sıfırlama istenirse yapılacak işlemler
    /// </summary>
    private static void Sifirla()
    {
        try
        {
            LogYaz("Sıfırlama işlemi başlatıldı:");


            // her işlemde yedek alalım
            var simdi = DateTime.Now;
            string sifirlamaAniString = simdi.Year.ToString() + simdi.Month.ToString() + simdi.Day.ToString() + "-" + simdi.Hour.ToString() + simdi.Minute.ToString() + simdi.Second.ToString();


            // Registry kaydını sil
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(
                "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
            
                if (key.GetValue(appName) != null)
                {
                    key.DeleteValue(appName);
                    Console.WriteLine("Başlangıç kaydı Registry den silindi.");
                }
            }

            LogYaz("Sıfılama:Registry kaydı silindi:");


            // config.txt dosyasını sil silmeden önce kopyasını yani yedeğini al öyle sil
            string configDosya = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "config.txt");
            if (File.Exists(configDosya))
            {
                string yedekDosya = Path.Combine(
                Path.GetDirectoryName(configDosya),
                $"sifirlamaOncesi_{sifirlamaAniString}_{Path.GetFileName(configDosya)}");
                File.Copy(configDosya, yedekDosya, true);
                LogYaz($"sıfırlama öncesi configDosya yedeklendi yedek:{yedekDosya}");
                Thread.Sleep(1000);
                File.Delete(configDosya);
                LogYaz($"sıfırlama: config.txt dosyası silindi.");
                Console.WriteLine("config.txt dosyası silindi.");
            }

            // Hatalı satır dosyasını silmeden önce kopyasını al öyle sil
            string hataliDosya = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "hatalilar.txt");
            if (File.Exists(hataliDosya))
            {
                string yedekDosya = Path.Combine(
               Path.GetDirectoryName(hataliDosya),
               $"sifirlamaOncesi_{sifirlamaAniString}_{Path.GetFileName(hataliDosya)}");
                File.Copy(hataliDosya, yedekDosya, true);
                LogYaz($"sıfırlama öncesi hataliDosya yedeklendi yedek:{yedekDosya}");
                Thread.Sleep(1000);

                File.Delete(hataliDosya);
                LogYaz($"sıfırlama: hatalilar.txt dosyası silindi.");
                Console.WriteLine("hatalilar.txt dosyası silindi.");
            }

            // Log dosyasını silmeden önce kopyasını yani yedeğini al öyle sil
            string logDosya = Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "service_log.txt");
            if (File.Exists(logDosya))
            {
                string yedekDosya = Path.Combine(
                Path.GetDirectoryName(logDosya),
                $"sifirlamaOncesi_{sifirlamaAniString}_{Path.GetFileName(logDosya)}");
                File.Copy(logDosya, yedekDosya, true);
                Thread.Sleep(1000);
                LogYaz($"sıfırlama öncesi logDosya yedeklendi yedek:{yedekDosya}");

                File.Delete(logDosya);
                Console.WriteLine("service_log.txt dosyası silindi.");
            }

            Console.WriteLine("Uygulama başarıyla sıfırlandı.");
        }
        catch (Exception ex)
        {
            LogYaz($"Sıfırlama sırasında hata oluştu: {ex.Message}");
            Console.WriteLine($"Sıfırlama sırasında hata oluştu: {ex.Message}");
        }
    }
}