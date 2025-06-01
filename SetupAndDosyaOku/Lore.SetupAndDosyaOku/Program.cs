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

namespace Lore.SetupAndDosyaOku;

public static class Program
{
    private static NotifyIcon? _notifyIcon;
    private static bool _firstRun = false;
    private static IHost? _host;
      /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    public static void Main(string[] args)
    {
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
            
            _firstRun = string.IsNullOrEmpty(appSettings.FirmaKod) ||
                        string.IsNullOrEmpty(appSettings.PdksKayitDosyaYolu) ||
                        string.IsNullOrEmpty(appSettings.AlarmKayitDosyaYolu) ||
                        string.IsNullOrEmpty(appSettings.KameraLogDosyaYolu);

            // Initialize logging
            var logger = _host.Services.GetRequiredService<Logger>();
            logger.Info("Application starting");
            
            // If first run or settings incomplete, show setup form
            if (_firstRun)
            {
                logger.Info("First run or incomplete configuration detected. Showing setup form.");
                var setupForm = _host.Services.GetRequiredService<SetupForm>();
                Application.Run(setupForm);
                
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
        }
        finally
        {
            _notifyIcon?.Dispose();
            await _host.StopAsync();
            _host.Dispose();
        }
    }
    
    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostContext, configApp) =>
            {
                configApp.SetBasePath(Directory.GetCurrentDirectory());
                configApp.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                configApp.AddEnvironmentVariables();
            })
            .ConfigureServices((hostContext, services) =>
            {
                // Register your services here
                services.AddSingleton<Logger>();
                services.AddSingleton<ConfigHelper>();
                services.AddSingleton<ApiHelper>();
                services.AddSingleton<FileHelper>();
                services.AddSingleton<StartupHelper>();
                
                // Register forms
                services.AddTransient<SetupForm>();
                
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
    }
    
    private static void InitializeSystemTray()
    {
        _notifyIcon = new NotifyIcon
        {
            Icon = new Icon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.ico")),
            Visible = true,
            Text = "Lore Dosya İzleyici"
        };
        
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
            var setupForm = _host?.Services.GetRequiredService<SetupForm>();
            setupForm?.ShowDialog();
        };
        contextMenu.Items.Add(settingsMenuItem);
        
        // Add separator
        contextMenu.Items.Add(new ToolStripSeparator());
        
        // Add exit option
        var exitMenuItem = new ToolStripMenuItem("Çıkış");
        exitMenuItem.Click += (s, e) => 
        {
            _notifyIcon.Visible = false;
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
        };
        timer.Start();
    }
}
