using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
// .NET 8'de tam namespace kullanmamız gerekiyor
using System.Windows.Forms; // Microsoft.Windows.Compatibility paketi içinden gelecek
using System.Drawing; // NotifyIcon için gerekli

namespace Temha.DataOkuConsole
{
    public class FileWatcherService : BackgroundService
    {
        private readonly ILogger<FileWatcherService> _logger;
        private FileSystemWatcher _watcher;
        private readonly string _izlenecekDosya;
        private bool _isProcessing = false;
        private readonly object _lockObject = new object();
        private NotifyIcon _trayIcon;

        public FileWatcherService(ILogger<FileWatcherService> logger)
        {
            _logger = logger;
            
            // Konfigürasyonu yükle
            var configFilePath = GetConfigFilePath();
            if (File.Exists(configFilePath))
            {
                try
                {
                    var jsonContent = File.ReadAllText(configFilePath);
                    var config = System.Text.Json.JsonSerializer.Deserialize<DTO.configModel.AppConfiguration>(jsonContent);
                    _izlenecekDosya = config?.AppSettings?.IzlenecekDosya;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Konfigürasyon yüklenirken hata oluştu");
                }
            }
        }        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Windows servisi olarak çalıştığında
            try
            {
                _logger.LogInformation("Temha DataOkuConsole servisi başlatılıyor");
                
                // Program sınıfındaki metotları kullanarak konfigürasyonu yükle
                Program.InitializeConfiguration();
                Program.SetupConfigWatcher();
                Program.SetupFileWatcher();
                
                _logger.LogInformation("Dosya izleme servisi başlatıldı.");
                
                // İlk çalıştırmada dosyayı işle
                Program.ProcessFile();
                
                // Log bilgisi ekle
                Program.LogYaz("Windows servisi olarak başlatıldı.");
                
                // Uygulama servis olarak çalıştığında da sistem tepsisinde görünecek
                SetupTrayIcon();
                
                // Servis çalıştığı sürece
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Servis çalıştırılırken hata oluştu: {Hata}", ex.Message);
            }
            finally
            {
                // Kaynakları temizle
                try
                {
                    if (_trayIcon != null)
                    {
                        _trayIcon.Visible = false;
                        _trayIcon.Dispose();
                        _trayIcon = null;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Tray icon kapatılırken hata oluştu: {Hata}", ex.Message);
                }
            }
        }

        private string GetConfigFilePath()
        {
            // Önce standart kurulum klasöründen dosyayı ara
            var configPath = "C:\\TemhaPdks\\application.json";
            
            // Eğer standart konumda yoksa kendi lokasyonuna bak
            if (!File.Exists(configPath))
            {
                configPath = Path.Combine(
                    Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                    "application.json");
            }

            return configPath;
        }        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed) return;
            
            _logger.LogInformation("Dosya değişikliği algılandı: {DosyaYolu}", e.FullPath);
            
            try
            {
                // Program sınıfındaki ProcessFile metodunu çağır
                Program.ProcessFile();
                _logger.LogInformation("Dosya işleme tamamlandı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dosya değişikliği işlenirken hata oluştu");
            }
        }private void ProcessFile()
        {
            try
            {
                // Program sınıfındaki mevcut ProcessFile metodunu kullan
                Program.ProcessFile();
                _logger.LogInformation("Dosya işleme tamamlandı");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dosya işlenirken hata oluştu");
            }
        }        private void SetupTrayIcon()
        {
            try
            {
                // UI öğelerini oluşturmak için STA thread kullan
                var thread = new Thread(() =>
                {
                    try 
                    {
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
                        
                        _trayIcon = new NotifyIcon
                        {
                            Icon = appIcon,
                            Text = "Temha Data Oku Konsol Servisi",
                            Visible = true,
                            BalloonTipTitle = "Temha Data Oku Servisi",
                            BalloonTipText = "Servis çalışıyor ve veriler izleniyor."
                        };

                        var contextMenuStrip = new ContextMenuStrip();
                        
                        // Servis durumu gösterme menü öğesi
                        contextMenuStrip.Items.Add("Servis Durumu", null, (s, e) => 
                        {
                            try
                            {
                                MessageBox.Show("Servis çalışıyor", "Temha Data Oku", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Durum gösterilirken hata oluştu");
                            }
                        });
                          contextMenuStrip.Items.Add("-"); // Ayırıcı
                        
                        // İzlenen dosyayı manuel işleme seçeneği
                        contextMenuStrip.Items.Add("Dosyayı Şimdi İşle", null, (s, e) =>
                        {
                            try
                            {
                                Program.ProcessFile();
                                MessageBox.Show("Dosya işleme tamamlandı", "Temha Data Oku", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Manuel işleme sırasında hata oluştu");
                                MessageBox.Show($"Hata: {ex.Message}", "Temha Data Oku", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        });
                        
                        contextMenuStrip.Items.Add("-"); // Ayırıcı
                        
                        // Çıkış menü öğesi
                        contextMenuStrip.Items.Add("Çıkış", null, (s, e) => 
                        {
                            try
                            {
                                if (MessageBox.Show("Servisi durdurmak istediğinize emin misiniz?", "Temha Data Oku", 
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                                {
                                    // Tray icon temizleme
                                    if (_trayIcon != null)
                                    {
                                        _trayIcon.Visible = false;
                                        _trayIcon.Dispose();
                                    }
                                    
                                    // Uygulamayı sonlandır
                                    Environment.Exit(0);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Çıkış işlemi sırasında hata oluştu");
                            }
                        });
                        
                        _trayIcon.ContextMenuStrip = contextMenuStrip;
                        
                        // Baloncuk ipucu olayını ekle
                        _trayIcon.ShowBalloonTip(3000);
                        
                        // STA thread'i canlı tutmak için mesaj döngüsünü başlat
                        Application.Run();
                    }
                    catch (Exception threadEx)
                    {
                        _logger.LogError(threadEx, "Tray icon thread'i sırasında hata: {Hata}", threadEx.Message);
                    }
                });
                
                // Thread'i STA modunda ayarla - Windows Forms için gerekli
                thread.SetApartmentState(ApartmentState.STA);
                thread.IsBackground = true;
                thread.Start();
                
                _logger.LogInformation("Sistem tepsisi simgesi başarıyla oluşturuldu");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sistem tepsisi ikonu oluşturulurken hata oluştu: {Hata}", ex.Message);
            }
        }
    }
}
