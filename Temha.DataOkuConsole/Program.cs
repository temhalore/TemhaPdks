using System;
using System.IO;
using System.Threading;
using System.Reflection;
using Microsoft.Win32;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text.Json;
using Temha.DataOkuConsole.DTO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using Temha.DataOkuConsole.DTO.configModel;

class Program
{
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool AllocConsole();

    // Windows API işlevlerini tanımlıyoruz
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetConsoleWindow();

    // ShowWindow için sabitler
    private const int SW_HIDE = 0; // Konsolu gizle
    private const int SW_SHOW = 5; // Konsolu göster

    private static bool _isKonsolPenceresiAcik = false;

    private static string appName = "TemhaDosyaOkuYaz";
    private static FileSystemWatcher watcher;
    private static FileSystemWatcher configWatcher;
    private static bool isProcessing = false;
    private static readonly object lockObject = new object();
    private static Mutex mutex = new Mutex(true, "FileWatcherServiceUniqueMutex");
    private static AppConfiguration _configuration;
    private static string configFilePath;
    private static string sifirlamaKod = "df@ABb9bdNGgSvs62v6f9";





    static async Task Main(string[] args)
    {
        try
        {
            

            // İlk çalıştırmada konsol penceresini aç
            AllocConsole();
            _isKonsolPenceresiAcik = true;

            // application.json dosyasını oluştur veya yükle
            InitializeConfiguration();

            // application.json dosyasını izle
            SetupConfigWatcher();

            Console.WriteLine("Kuruluma devam için 'D' tuşuna, var olan kurulumu sıfırlama için 'S' tuşuna basınız.");
            if (Console.ReadKey(true).Key == ConsoleKey.S)
            {
                Console.WriteLine("Sıfırlama için gerekli özel kodu giriniz ve enter'a basınız.");
                if (Console.ReadLine() == sifirlamaKod)
                {
                    Sifirla();
                    Console.WriteLine("Programı yeniden başlatma gerekli!!Kapatmak için bir tuşa basın ve kapatın");
                    Console.ReadKey();
                    return;
                    
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

            // İlk kurulum tamamlandıktan sonra debug moduna göre konsolu yönet
            YonetimKonsol();

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
        configFilePath = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "application.json");
        //kurulum olunca nereye kurulursa oradan alsın
        
        //configFilePath = ("C:\\TemhaPdks\\config\\application.json");


        if (!File.Exists(configFilePath))
        {
            //throw new Exception("application.json dosyası bulunamadı!");
            LogYaz("Config dosyası bulunamadı!");
            return;
        }

        LoadConfiguration();
    }

    private static void LoadConfiguration()
    {
        string jsonContent =  DosyaIslemleri.DosyaOku(configFilePath);
        _configuration = JsonSerializer.Deserialize<AppConfiguration>(jsonContent);


        // app settings ten izleneck yolu vs dışarıdan al sonra confta iç tarafta core yolalrı üret 
        //tüm dosyaları kontrol et yoksa oluştur.
        DosyaIslemleri.DosyaKlasorKontrol(_configuration.AppSettings.IzlenecekDosya);
        string directoryAna = Path.GetDirectoryName(configFilePath);
        string directoryIzlenecekAna = Path.GetDirectoryName(_configuration.AppSettings.IzlenecekDosya);

        _configuration.CoreSettings = new CoreSettings();

        _configuration.CoreSettings.HataliDosya = directoryIzlenecekAna + "\\hatalilar.txt";
       

        DosyaIslemleri.DosyaKlasorKontrol(_configuration.CoreSettings.HataliDosya);

        string jsonStringAna = JsonSerializer.Serialize(_configuration);
        LogYaz($"Ana Yapılandırma Dosyası İçerik: {jsonStringAna}");

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
            YonetimKonsol();
        }
        catch (Exception ex)
        {
            LogYaz($"Yapılandırma güncellenirken hata: {ex.Message}");
        }
    }

    private static void LogYaz(string mesaj)
    {
        //önce log dosyasını oluştur
        var anaKalsor = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var logDosyaYolu = anaKalsor + "\\logs\\service_log.txt";
        DosyaIslemleri.DosyaKlasorKontrol(logDosyaYolu);

        string logMesaj = $"{DateTime.Now} - {mesaj}";
        if (_isKonsolPenceresiAcik)
        {
                    Console.WriteLine(logMesaj);
             
        }
       
        try
        {
            DosyaIslemleri.DosyayaYazYeniSatir(logDosyaYolu, logMesaj);
        }
        catch { 
        }
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

                string isleneceklerDosyasi = Path.Combine(
                    Path.GetDirectoryName(_configuration.AppSettings.IzlenecekDosya),
                    $"islencekler_{Path.GetFileName(_configuration.AppSettings.IzlenecekDosya)}");

                if (new FileInfo(_configuration.AppSettings.IzlenecekDosya).Length == 0)
                {
                    isProcessing = false;
                    return;
                }

                // Yedek al
                DosyaIslemleri.DosyaYedekAl(_configuration.AppSettings.IzlenecekDosya, "yedek");

                // Kopya oluştur
                DosyaIslemleri.DosyaKopyala(_configuration.AppSettings.IzlenecekDosya, isleneceklerDosyasi);

                // Ana dosyayı temizle
                DosyaIslemleri.DosyaTemizle(_configuration.AppSettings.IzlenecekDosya);

                int satirSayisi = 0;
                int hataliSatirSayisi = 0;

                LogYaz("Yeni değişiklikler işleniyor:");

                foreach (string satir in DosyaIslemleri.DosyaSatirlariniOku(isleneceklerDosyasi))
                {
                    satirSayisi++;
                    try
                    {
                        
                        LogYaz($"İşlenen satır burada apiyi çağıracağız şuan satır bilgisi : {satirSayisi}: {satir}");
                        Thread.Sleep(1000); //api isteği karşılanmasını bekliyormuş gibi yap 1 saniye bekle
                        // API çağrısı ve diğer işlemler
                    }
                    catch (Exception ex)
                    {
                        hataliSatirSayisi++;
                        DosyaIslemleri.DosyayaYazYeniSatir(
                            _configuration.CoreSettings.HataliDosya,
                            $"{DateTime.Now} - {satir} ");

                        LogYaz($"İşlenen satırda hata!! - {satir} Hata: {ex.Message}");
                    }
                }

                // Kopya dosyayı sil
                DosyaIslemleri.DosyaSil(isleneceklerDosyasi, false);

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

            //// Config dosyasını sil
            //DosyaIslemleri.DosyaSil(configFilePath,true, "sifirlamaOncesi");

            // Hatalı dosyayı sil
            DosyaIslemleri.DosyaSil(_configuration.CoreSettings.HataliDosya, true, "sifirlamaOncesi");

            LogYaz("Uygulama başarıyla sıfırlandı.");
        }
        catch (Exception ex)
        {
            LogYaz($"Sıfırlama sırasında hata oluştu: {ex.Message}");
        }
    }

    // Konsol yönetimi için yardımcı metod
    private static void YonetimKonsol()
    {
        try
        {
            if (_configuration.AppSettings.IsDebugMode && !_isKonsolPenceresiAcik)
            {
                ShowWindow(GetConsoleWindow(), SW_SHOW);
                _isKonsolPenceresiAcik = true;
                LogYaz("Debug modu aktif - Konsol açıldı");
            }
            else if (!_configuration.AppSettings.IsDebugMode && _isKonsolPenceresiAcik)
            {
                Console.Clear();
                ShowWindow(GetConsoleWindow(), SW_HIDE);
                _isKonsolPenceresiAcik = false;
                LogYaz("Debug modu pasif - Konsol kapatıldı");
            }
        }
        catch (Exception ex)
        {
            LogYaz($"Konsol yönetimi hatası: {ex.Message}");
        }
    }



}