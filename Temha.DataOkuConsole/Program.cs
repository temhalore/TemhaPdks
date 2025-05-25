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
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
// .NET 8'de tam namespace kullanmamız gerekiyor
using System.Windows.Forms; // Microsoft.Windows.Compatibility paketi içinden gelecek
using System.Drawing; // Microsoft.Windows.Compatibility paketi içinden gelecek

namespace Temha.DataOkuConsole
{
    public class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        // Windows API işlevlerini tanımlıyoruz
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();        // ShowWindow için sabitler
        private const int SW_HIDE = 0; // Konsolu gizle
        private const int SW_SHOW = 5; // Konsolu göster
        
        private static bool _isKonsolPenceresiAcik = false;

        private static string appName = "TemhaDosyaOkuYaz";
        private static FileSystemWatcher watcher;
        private static FileSystemWatcher configWatcher;
        private static bool isProcessing = false;
        private static readonly object lockObject = new object();
        private static Mutex mutex;
        private const string mutexName = "FileWatcherServiceUniqueMutex";
        private static AppConfiguration _configuration;
        private static string configFilePath;
        private static string sifirlamaKod = "df@ABb9bdNGgSvs62v6f9";
        private static NotifyIcon trayIcon;

        private static string hareketUrl_local = "https://localhost:44374/Api/Hareket/saveHareketByHareketDto";
        private static string hareketUrl_prod = "https://localhost:44374/Api/Hareket/saveHareketByHareketDto";

        static async Task Main(string[] args)
        {
            // Windows servisi yükleme veya kaldırma işlemi için argümanları kontrol et
            if (args.Length > 0)
            {
                switch (args[0].ToLower())
                {
                    case "install":
                        // Kurulum modunu başlat
                        RunInstallMode();
                        return;                    case "service-install":
                        // Önce mevcut servisi kaldır (varsa)
                        Console.WriteLine("Var olan servis kontrol ediliyor ve kaldırılıyor...");
                        
                        // Servisi kaldır
                        if (ServiceInstaller.Uninstall())
                        {
                            Console.WriteLine("Eski servis kaldırıldı veya zaten mevcut değildi.");
                        }
                        else
                        {
                            Console.WriteLine("Eski servis kaldırılırken bir sorun oluştu, yeni kuruluma devam ediliyor.");
                        }
                        
                        // Servis işlemlerinin sonlanmasını bekle
                        Thread.Sleep(2000);
                        
                        // Windows servisi olarak yükle
                        if (ServiceInstaller.Install())
                        {
                            Console.WriteLine("Windows servisi başarıyla yüklendi.");
                            Console.WriteLine("Servis otomatik olarak başlatılacak.");
                        }
                        else
                        {
                            Console.WriteLine("Windows servisi yüklenirken bir hata oluştu.");
                        }
                        return;

                    case "service-uninstall":
                        // Windows servisini kaldır
                        if (ServiceInstaller.Uninstall())
                        {
                            Console.WriteLine("Windows servisi başarıyla kaldırıldı.");
                        }
                        else
                        {
                            Console.WriteLine("Windows servisi kaldırılırken bir hata oluştu.");
                        }
                        return;
                }
            }            // Tek örnek (instance) kontrolü yap - mutex kullanarak
            bool mutexCreated = false;
            
            try
            {
                // Mutex'i oluştur, eğer aynı isimli başka bir mutex varsa mutexCreated false olur
                mutex = new Mutex(true, mutexName, out mutexCreated);
                
                if (!mutexCreated)
                {
                    // Başka bir örnek zaten çalışıyor
                    Console.WriteLine("Uygulama zaten çalışıyor. Yeni instance kapatılıyor.");
                    return;
                }
                
                // Tek örnek olarak çalışıyoruz, servisi veya uygulamayı başlat
                await CreateHostBuilder(args).Build().RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Uygulama başlatılırken bir hata oluştu: {ex.Message}");
            }
            finally
            {
                // Bu kısım genellikle çalışmaz çünkü RunAsync zaten bir while döngüsü içindedir
                // Ancak mutex'i temiz bir şekilde kapatmak için burada bulunuyor
                if (mutexCreated && mutex != null)
                {
                    mutex.ReleaseMutex();
                    mutex.Dispose();
                }
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService(options =>
                {
                    options.ServiceName = "TemhaDataOkuConsole";
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<FileWatcherService>();
                });

        private static void RunInstallMode()
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

                Console.WriteLine("Uygulama Kurulum Menüsü:");
                Console.WriteLine("------------------------");
                Console.WriteLine("1. Normal mod - Uygulama olarak devam et (D)");
                Console.WriteLine("2. Windows servisi olarak kur (W)");
                Console.WriteLine("3. Var olan kurulumu sıfırla (S)");
                Console.WriteLine("------------------------");
                Console.WriteLine("Seçiminizi yapın (D/W/S):");

                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.W)
                {
                    Console.WriteLine("Windows servisi kurulumu başlatılıyor...");
                    if (ServiceInstaller.Install())
                    {
                        Console.WriteLine("Windows servisi başarıyla yüklendi!");
                        Console.WriteLine("Servisi başlatmak için 'sc start TemhaDataOkuConsole' komutunu kullanabilirsiniz.");
                        Console.WriteLine("Çıkmak için bir tuşa basın...");
                        Console.ReadKey();
                    }
                    else
                    {
                        Console.WriteLine("Servis kurulumunda bir hata oluştu. Yönetici izinleriyle uygulamayı çalıştırmayı deneyin.");
                        Console.WriteLine("Çıkmak için bir tuşa basın...");
                        Console.ReadKey();
                    }
                    return;
                }
                else if (key == ConsoleKey.S)
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

                if (key == ConsoleKey.D)
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

                // Sistem tepsisi simgesini oluştur
                InitializeTrayIcon();

                Application.Run();
            }
            catch (Exception ex)
            {
                LogYaz($"Kritik hata: {ex.Message}");
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }        private static void InitializeTrayIcon()
        {
            try
            {
                // UI thread'inde çalıştırılmalı - sistem tepsisi için önemli
                Thread staThread = new Thread(() =>
                {
                    try
                    {
                        // Eğer daha önce oluşturulmuşsa temizle
                        if (trayIcon != null)
                        {
                            trayIcon.Visible = false;
                            trayIcon.Dispose();
                            trayIcon = null;
                        }

                        // Özel bir ikon kullanmak için
                        Icon appIcon = null;
                        string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.ico");
                        
                        if (File.Exists(iconPath))
                        {
                            try
                            {
                                appIcon = new Icon(iconPath);
                            }
                            catch 
                            {
                                appIcon = SystemIcons.Application;
                            }
                        }
                        else
                        {
                            appIcon = SystemIcons.Application;
                        }

                        // NotifyIcon oluştur
                        trayIcon = new NotifyIcon
                        {
                            Icon = appIcon,
                            Text = "Temha Data Oku Servisi",
                            Visible = true,
                            BalloonTipTitle = "Temha Data Oku Servisi",
                            BalloonTipText = "Servis çalışıyor ve veri dosyanız izleniyor."
                        };

                        // Context menu oluştur
                        var contextMenu = new ContextMenuStrip();
                        contextMenu.Items.Add("Durum", null, (s, e) => ShowNotification("Servis durumu", "Servis aktif olarak çalışıyor ve veri dosyanız izleniyor."));
                        contextMenu.Items.Add("Göster", null, ShowForm_Click);
                        contextMenu.Items.Add("Çıkış", null, Exit_Click);
                        trayIcon.ContextMenuStrip = contextMenu;
                        trayIcon.DoubleClick += ShowForm_Click;
                        
                        // Başlangıçta bildirim göster
                        trayIcon.ShowBalloonTip(3000);
                        
                        // Message loop çalıştır - tray ikon için gerekli
                        Application.Run();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Sistem tepsisi ikonu oluşturulurken hata: {ex.Message}");
                    }
                });
                
                staThread.SetApartmentState(ApartmentState.STA);
                staThread.IsBackground = true;
                staThread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Tray icon başlatılamadı: {ex.Message}");
            }
        }

        private static void ShowNotification(string title, string message)
        {
            if (trayIcon != null)
            {
                trayIcon.BalloonTipTitle = title;
                trayIcon.BalloonTipText = message;
                trayIcon.ShowBalloonTip(3000);
            }
        }

        private static void ShowForm_Click(object sender, EventArgs e)
        {
            // Konsol penceresini göster
            ShowWindow(GetConsoleWindow(), SW_SHOW);
            _isKonsolPenceresiAcik = true;
        }

        private static void Exit_Click(object sender, EventArgs e)
        {
            try
            {
                // Uygulamayı sonlandır
                if (trayIcon != null)
                {
                    trayIcon.Visible = false;
                    trayIcon.Dispose();
                    trayIcon = null;
                }
                
                // Mutex'i serbest bırak
                try { mutex?.ReleaseMutex(); } catch { }
                mutex?.Dispose();
                
                // Dosya izleyicileri kapat
                try { watcher?.Dispose(); } catch { }
                try { configWatcher?.Dispose(); } catch { }
                
                // Uygulamayı sonlandır
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Uygulama kapatılırken hata: {ex.Message}");
                Environment.Exit(1);
            }
        }

        public static void InitializeConfiguration()
        {
            // Önce standart kurulum klasöründen dosyayı ara
            configFilePath = "C:\\TemhaPdks\\application.json";

            // Eğer standart konumda yoksa kendi lokasyonuna bak
            if (!File.Exists(configFilePath))
            {
                configFilePath = Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    "application.json");
                
                LogYaz($"Yapılandırma dosyası standart konumda bulunamadı, şuradan deneniyor: {configFilePath}");
            }

            if (!File.Exists(configFilePath))
            {
                LogYaz("Config dosyası bulunamadı! Uygulama başlatılamıyor.");
                Console.WriteLine("Yapılandırma dosyası bulunamadı. Devam etmek için bir tuşa basın.");
                Console.ReadKey();
                Environment.Exit(1);
                return;
            }

            LogYaz($"Yapılandırma dosyası bulundu: {configFilePath}");
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

        public static void SetupConfigWatcher()
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

        public static void LogYaz(string mesaj)
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

        public static void SetupFileWatcher()
        {
            string directory = Path.GetDirectoryName(_configuration.AppSettings.IzlenecekDosya);
            string filename = Path.GetFileName(_configuration.AppSettings.IzlenecekDosya);

            watcher = new FileSystemWatcher(directory, filename);
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size;
            watcher.Changed += OnFileChanged;
            watcher.EnableRaisingEvents = true;
        }

        public static void ProcessFile()
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

                // Servis varsa kaldır
                try
                {
                    ServiceInstaller.Uninstall();
                    LogYaz("Windows servisi kaldırıldı.");
                }
                catch
                {
                    // Servis yoksa geç
                }

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
}