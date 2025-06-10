using Lore.SetupAndDosyaOku.Helpers;
using Lore.SetupAndDosyaOku.Models;
using Lore.SetupAndDosyaOku.Services;
using Lore.SetupAndDosyaOku.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

using System.Threading;

namespace Lore.SetupAndDosyaOku;

public static class Program
{
    private static NotifyIcon? _notifyIcon;
    private static bool _firstRun = false;
    private static IHost? _host;
    private static Mutex _mutex = new Mutex(true, "Global\\LoreSetupAndDosyaOku_Instance");
      /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    public static void Main(string[] args)
    {
        // Uygulamanın sadece bir örneğinin çalıştığından emin ol
        if (!_mutex.WaitOne(TimeSpan.Zero, true))
        {
            MessageBox.Show("Uygulama zaten çalışıyor!", "Lore Dosya İzleyici", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        
        // Async işlemleri senkron olarak çalıştır
        MainAsync(args).GetAwaiter().GetResult();
    }
    
    private static async Task MainAsync(string[] args)
    {
        
        // Initialize services and configuration
        _host = CreateHostBuilder(args).Build();
        
        try
        {
            // Check if this is the first run or if config needs to be set up
            var configHelper = _host.Services.GetRequiredService<ConfigHelper>();
            var appSettings = configHelper.GetSettings();
              _firstRun = string.IsNullOrEmpty(appSettings.FirmaKod);// Initialize logging
            var logger = _host.Services.GetRequiredService<Logger>();
            logger.Info("Application starting");
            
            // Check for updates on startup
            await CheckForUpdatesOnStartupAsync();
              // If first run or settings incomplete, show setup wizard
            if (_firstRun)
            {
                logger.Info("First run or incomplete configuration detected. Showing setup wizard.");
                var setupWizardForm = _host.Services.GetRequiredService<SetupWizardForm>();
                Application.Run(setupWizardForm);
                
                // Check if the setup was completed successfully
                if (string.IsNullOrEmpty(configHelper.GetSettings().FirmaKod))
                {
                    logger.Warning("Setup was not completed. Exiting application.");
                    return;
                }
            }
            
            // Initialize system tray
            InitializeSystemTray();
            
            // Start file monitoring services
            await StartServicesAsync();
            
            // Run the application
            Application.Run();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Uygulama başlatılırken bir hata oluştu: {ex.Message}", "Hata", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            Debug.WriteLine($"Unhandled exception: {ex}");
        }        finally
        {
            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = false;
                _notifyIcon.Dispose();
                _notifyIcon = null;
            }
            
            if (_host != null)
            {
                try
                {
                    await _host.StopAsync(TimeSpan.FromSeconds(5));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error stopping host: {ex.Message}");
                }
                finally
                {
                    _host.Dispose();
                }
            }
            
            _mutex.ReleaseMutex();
        }
    }
    
    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostContext, configApp) =>
            {
                configApp.SetBasePath(Directory.GetCurrentDirectory());
                configApp.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                configApp.AddEnvironmentVariables();
            })            .ConfigureServices((hostContext, services) =>
            {
                // Register HttpClient
                services.AddHttpClient();
                
                // Register your services here
                services.AddSingleton<Logger>();
                services.AddSingleton<ConfigHelper>();
                services.AddSingleton<ApiHelper>();
                services.AddSingleton<FileHelper>();
                services.AddSingleton<StartupHelper>();
                services.AddSingleton<UpdateService>();
                services.AddSingleton<LogSenderService>();                // Register forms
                services.AddTransient<SetupWizardForm>();
                
                // Register monitoring services
                services.AddHostedService<FileMonitoringService>();
            });
    
    private static async Task StartServicesAsync()
    {
        try
        {
            var logger = _host?.Services.GetRequiredService<Logger>();
            logger?.Info("Starting background services");
            await _host!.StartAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Servisler başlatılırken bir hata oluştu: {ex.Message}", "Hata", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }      private static void InitializeSystemTray()
    {
        // If the previous instance didn't clean up properly
        if (_notifyIcon != null)
        {
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
            _notifyIcon = null;
        }
        
        // Create new notifyIcon instance
        _notifyIcon = new NotifyIcon
        {
            Visible = false,  // Initially invisible to avoid ghost icons
            Text = "Lore Dosya İzleyici"
        };
        
        // Get icon path
        string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.ico");
        if (File.Exists(iconPath))
        {
            try
            {
                _notifyIcon.Icon = new Icon(iconPath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load icon: {ex.Message}");
                _notifyIcon.Icon = SystemIcons.Application;  // Fallback to system icon
            }
        }
        else
        {
            _notifyIcon.Icon = SystemIcons.Application;  // Fallback to system icon
        }
          
        // Now make it visible after everything is set up
        _notifyIcon.Visible = true;
        
        // Create context menu
        var contextMenu = new ContextMenuStrip();
        
        // Add uptime menu item that will be dynamically updated
        var uptimeMenuItem = new ToolStripMenuItem("Çalışma Süresi: Hesaplanıyor...");
        uptimeMenuItem.Enabled = false;
        contextMenu.Items.Add(uptimeMenuItem);
        
        // Add separator
        contextMenu.Items.Add(new ToolStripSeparator());
        
        // Add open logs directory
        var openLogsMenuItem = new ToolStripMenuItem("Logları Aç");
        openLogsMenuItem.Click += (s, e) => 
        {
            var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            if (Directory.Exists(logPath))
                Process.Start("explorer.exe", logPath);
            else
                MessageBox.Show("Log dizini bulunamadı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        };
        contextMenu.Items.Add(openLogsMenuItem);
          // Add settings option
        var settingsMenuItem = new ToolStripMenuItem("Ayarlar");
        settingsMenuItem.Click += async (s, e) => 
        {
            var setupWizardForm = _host?.Services.GetRequiredService<SetupWizardForm>();
            setupWizardForm?.ShowDialog();
        };
        contextMenu.Items.Add(settingsMenuItem);
        
        // Add separator
        contextMenu.Items.Add(new ToolStripSeparator());
          // Add exit option
        var exitMenuItem = new ToolStripMenuItem("Çıkış");
        exitMenuItem.Click += (s, e) => 
        {
            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = false;
                _notifyIcon.Dispose();
                _notifyIcon = null;
            }
            Application.Exit();
        };
        contextMenu.Items.Add(exitMenuItem);
        
        // Assign context menu to notify icon
        _notifyIcon.ContextMenuStrip = contextMenu;
        
        // Set up timer to update uptime display
        var timer = new System.Windows.Forms.Timer
        {
            Interval = 1000, // Update every second
            Enabled = true
        };        timer.Tick += (s, e) =>
        {
            var logger = _host?.Services.GetRequiredService<Logger>();
            if (logger != null)
            {
                var uptime = logger.GetUptime();
                uptimeMenuItem.Text = $"Çalışma Süresi: {uptime.Days} gün, {uptime.Hours} saat, {uptime.Minutes} dakika";
            }
        };        timer.Start();
    }
      private static async Task CheckForUpdatesOnStartupAsync()
    {
        try
        {
            if (_host == null) return;
            
            var logger = _host.Services.GetRequiredService<Logger>();
            var updateService = _host.Services.GetRequiredService<UpdateService>();
            var configHelper = _host.Services.GetRequiredService<ConfigHelper>();
            
            logger.Info("Checking for updates on startup...");
            
            // Get required parameters from configuration
            var appSettings = configHelper.GetSettings();
            var firmaKod = appSettings.FirmaKod;
            var currentVersion = appSettings.Version;
            
            if (string.IsNullOrWhiteSpace(firmaKod))
            {
                logger.Warning("FirmaKod is not configured, skipping update check.");
                return;
            }
            
            var updateResult = await updateService.CheckForUpdatesAsync(firmaKod, currentVersion);
            if (updateResult?.UpdateAvailable == true)
            {
                logger.Info($"Update available: Version {updateResult.LatestVersion}");
                
                // Show notification in system tray if available
                if (_notifyIcon != null)
                {
                    _notifyIcon.BalloonTipTitle = "Güncelleme Mevcut";
                    _notifyIcon.BalloonTipText = $"Yeni versiyon mevcut: {updateResult.LatestVersion}";
                    _notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                    _notifyIcon.ShowBalloonTip(5000);
                }
            }
            else
            {
                logger.Info("Application is up to date");
            }
        }
        catch (Exception ex)
        {
            var logger = _host?.Services.GetRequiredService<Logger>();
            logger?.Warning($"Failed to check for updates on startup: {ex.Message}");
        }
    }
}
