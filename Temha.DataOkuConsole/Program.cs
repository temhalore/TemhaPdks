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
        if (!mutex.WaitOne(TimeSpan.Zero, true))
        {
            LogYaz("Uygulama zaten çalışıyor. Yeni instance kapatılıyor.");
            return;
        }

        try
        {
            // Başlangıçta çalıştırma kaydını kontrol et ve ekle
            StartupKaydiEkle();

            // application.json dosyasını oluştur veya yükle
            InitializeConfiguration();

            // application.json dosyasını izle
            SetupConfigWatcher();

            Console.WriteLine("Sıfırlama yapmak istiyorsanız 'S' tuşuna basın.");
            if (Console.ReadKey(true).Key == ConsoleKey.S)
            {
                Sifirla();
            }

            if (!File.Exists(_configuration.AppSettings.KaynakDosyaYolu))
            {
                LogYaz("Belirtilen dosya bulunamadı!");
                return;
            }

            // Dosya izleyiciyi başlat
            SetupFileWatcher();

            LogYaz("Dosya izleme servisi başlatıldı.");
            LogYaz($"Firma Kodu: {_configuration.AppSettings.FirmaKod}");

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

        if (!File.Exists(configFilePath))
        {
            CreateDefaultConfiguration();
        }

        LoadConfiguration();
    }

    private static void CreateDefaultConfiguration()
    {
        var defaultConfig = new AppConfiguration
        {
            AppSettings = new AppSettings
            {
                FirmaKod = "FIRMA001",
                KaynakDosyaYolu = Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    "data.txt"),
                HataliDosyaYolu = Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    "hatalilar.txt"),
                LogDosyaYolu = Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    "service_log.txt"),
                IsDebugMode = true,
                MaxRetryCount = 3
            }
        };

        string jsonContent = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(configFilePath, jsonContent);
    }

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

    private static void OnConfigurationChanged(object sender, FileSystemEventArgs e)
    {
        try
        {
            Thread.Sleep(100); // Dosyanın yazılmasını bekle
            LoadConfiguration();
            LogYaz("Yapılandırma dosyası güncellendi.");
            LogYaz($"Yeni Firma Kodu: {_configuration.AppSettings.FirmaKod}");

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
            File.AppendAllText(_configuration.AppSettings.LogDosyaYolu, logMesaj + Environment.NewLine);
        }
        catch { }
    }

    private static void SetupFileWatcher()
    {
        string directory = Path.GetDirectoryName(_configuration.AppSettings.KaynakDosyaYolu);
        string filename = Path.GetFileName(_configuration.AppSettings.KaynakDosyaYolu);

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

                Thread.Sleep(100);

                string kopyaDosyaYolu = Path.Combine(
                    Path.GetDirectoryName(_configuration.AppSettings.KaynakDosyaYolu),
                    $"kopya_{Path.GetFileName(_configuration.AppSettings.KaynakDosyaYolu)}");

                if (new FileInfo(_configuration.AppSettings.KaynakDosyaYolu).Length == 0)
                {
                    isProcessing = false;
                    return;
                }

                File.Copy(_configuration.AppSettings.KaynakDosyaYolu, kopyaDosyaYolu, true);
                File.WriteAllText(_configuration.AppSettings.KaynakDosyaYolu, string.Empty);

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
                            if (_configuration.AppSettings.IsDebugMode)
                            {
                                LogYaz($"Debug: İşlenen satır {satirSayisi}: {satir}");
                            }
                            // API çağrısı ve diğer işlemler
                        }
                        catch (Exception ex)
                        {
                            hataliSatirSayisi++;
                            using (StreamWriter sw = new StreamWriter(_configuration.AppSettings.HataliDosyaYolu, true))
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
                    LogYaz("Başlangıç kaydı eklendi.");
                }
            }
        }
        catch (Exception ex)
        {
            LogYaz($"Başlangıç kaydı eklenirken hata: {ex.Message}");
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
    // Diğer metodlar aynı kalacak (OnFileChanged, StartupKaydiEkle, Sifirla)
}