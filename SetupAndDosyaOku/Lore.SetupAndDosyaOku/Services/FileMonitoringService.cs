using System;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Lore.SetupAndDosyaOku.Helpers;
using Lore.SetupAndDosyaOku.Models;

namespace Lore.SetupAndDosyaOku.Services
{
    public class FileMonitoringService : BackgroundService
    {
        private readonly Logger _logger;
        private readonly ConfigHelper _configHelper;
        private readonly FileHelper _fileHelper;
        private readonly ApiHelper _apiHelper;
        private readonly LogSenderService _logSenderService;
        private readonly UpdateService _updateService;
        
        private bool _isFirstRun = true;
        private FileSystemWatcher? _pdksWatcher;
        private FileSystemWatcher? _alarmWatcher;
        private FileSystemWatcher? _kameraWatcher;
        
        public FileMonitoringService(
            Logger logger,
            ConfigHelper configHelper,
            FileHelper fileHelper,
            ApiHelper apiHelper,
            LogSenderService logSenderService,
            UpdateService updateService)
        {
            _logger = logger;
            _configHelper = configHelper;
            _fileHelper = fileHelper;
            _apiHelper = apiHelper;
            _logSenderService = logSenderService;
            _updateService = updateService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Info("File monitoring service starting");
            
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (_isFirstRun)
                    {
                        _isFirstRun = false;
                        InitializeWatchers();
                    }
                    
                    // Check for configuration changes
                    var configSettings = _configHelper.GetSettings();
                    if (configSettings != null && 
                        (_pdksWatcher == null || _alarmWatcher == null || _kameraWatcher == null || 
                         !AreWatchersConfigured(configSettings)))
                    {
                        _logger.Info("Configuration has changed, reinitializing watchers");
                        DisposeWatchers();
                        InitializeWatchers();
                    }

                    // Periodic tasks that should run even when no file changes detected
                    await CheckApiConnectivityAsync();
                }
                catch (Exception ex)
                {
                    _logger.Error($"Error in file monitoring service: {ex.Message}");
                }
                
                // Wait before the next cycle
                await Task.Delay(30000, stoppingToken); // Check every 30 seconds
            }
            
            _logger.Info("File monitoring service stopping");
            DisposeWatchers();
        }

        private void InitializeWatchers()
        {
            try
            {
                var settings = _configHelper.GetSettings();
                if (settings == null) return;
                
                // Initialize PDKS file watcher
                if (!string.IsNullOrEmpty(settings.PdksKayitDosyaYolu) && File.Exists(settings.PdksKayitDosyaYolu))
                {
                    string directory = Path.GetDirectoryName(settings.PdksKayitDosyaYolu)!;
                    string filename = Path.GetFileName(settings.PdksKayitDosyaYolu);
                    
                    _pdksWatcher = new FileSystemWatcher(directory)
                    {
                        Filter = filename,
                        NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName,
                        EnableRaisingEvents = true
                    };
                    
                    _pdksWatcher.Changed += (s, e) => OnPdksFileChanged(s, e, settings.PdksKayitDosyaYolu);
                    _logger.Info($"PDKS file watcher initialized for: {settings.PdksKayitDosyaYolu}");
                }
                
                // Initialize Alarm file watcher
                if (!string.IsNullOrEmpty(settings.AlarmKayitDosyaYolu) && File.Exists(settings.AlarmKayitDosyaYolu))
                {
                    string directory = Path.GetDirectoryName(settings.AlarmKayitDosyaYolu)!;
                    string filename = Path.GetFileName(settings.AlarmKayitDosyaYolu);
                    
                    _alarmWatcher = new FileSystemWatcher(directory)
                    {
                        Filter = filename,
                        NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName,
                        EnableRaisingEvents = true
                    };
                    
                    _alarmWatcher.Changed += (s, e) => OnAlarmFileChanged(s, e, settings.AlarmKayitDosyaYolu);
                    _logger.Info($"Alarm file watcher initialized for: {settings.AlarmKayitDosyaYolu}");
                }
                
                // Initialize Kamera log file watcher
                if (!string.IsNullOrEmpty(settings.KameraLogDosyaYolu) && File.Exists(settings.KameraLogDosyaYolu))
                {
                    string directory = Path.GetDirectoryName(settings.KameraLogDosyaYolu)!;
                    string filename = Path.GetFileName(settings.KameraLogDosyaYolu);
                    
                    _kameraWatcher = new FileSystemWatcher(directory)
                    {
                        Filter = filename,
                        NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName,
                        EnableRaisingEvents = true
                    };
                    
                    _kameraWatcher.Changed += (s, e) => OnKameraFileChanged(s, e, settings.KameraLogDosyaYolu);
                    _logger.Info($"Kamera log file watcher initialized for: {settings.KameraLogDosyaYolu}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error initializing file watchers: {ex.Message}");
            }
        }

        private bool AreWatchersConfigured(AppSettings settings)
        {
            try
            {
                if (_pdksWatcher != null && !string.IsNullOrEmpty(settings.PdksKayitDosyaYolu))
                {
                    string directory = Path.GetDirectoryName(settings.PdksKayitDosyaYolu)!;
                    if (_pdksWatcher.Path != directory)
                        return false;
                }
                
                if (_alarmWatcher != null && !string.IsNullOrEmpty(settings.AlarmKayitDosyaYolu))
                {
                    string directory = Path.GetDirectoryName(settings.AlarmKayitDosyaYolu)!;
                    if (_alarmWatcher.Path != directory)
                        return false;
                }
                
                if (_kameraWatcher != null && !string.IsNullOrEmpty(settings.KameraLogDosyaYolu))
                {
                    string directory = Path.GetDirectoryName(settings.KameraLogDosyaYolu)!;
                    if (_kameraWatcher.Path != directory)
                        return false;
                }
                
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void DisposeWatchers()
        {
            if (_pdksWatcher != null)
            {
                _pdksWatcher.EnableRaisingEvents = false;
                _pdksWatcher.Dispose();
                _pdksWatcher = null;
            }
            
            if (_alarmWatcher != null)
            {
                _alarmWatcher.EnableRaisingEvents = false;
                _alarmWatcher.Dispose();
                _alarmWatcher = null;
            }
            
            if (_kameraWatcher != null)
            {
                _kameraWatcher.EnableRaisingEvents = false;
                _kameraWatcher.Dispose();
                _kameraWatcher = null;
            }
        }        private async void OnPdksFileChanged(object sender, FileSystemEventArgs e, string filePath)
        {
            try
            {
                _logger.Info($"PDKS file changed: {e.FullPath}");
                string content = await _fileHelper.ReadFileWithRetryAsync(filePath);
                
                if (!string.IsNullOrEmpty(content))
                {
                    _logger.Info($"PDKS file content read: {content.Length} bytes");
                    
                    // Create a backup of the file
                    await _fileHelper.CreateBackupFileAsync(filePath, content);
                    
                    // Get firma kod from settings
                    var settings = _configHelper.GetSettings();
                    if (settings != null && !string.IsNullOrEmpty(settings.FirmaKod))
                    {
                        // Send log data to API using new LogSenderService
                        bool success = await _logSenderService.SendPdksLogAsync(settings.FirmaKod, content);
                        
                        // Clear the source file if successful
                        if (success)
                        {
                            await _fileHelper.ClearFileAsync(filePath);
                            _logger.Info("PDKS file successfully processed and cleared");
                        }
                        else
                        {
                            _logger.Warning("Failed to send PDKS log data to API");
                        }
                    }
                    else
                    {
                        _logger.Warning("FirmaKod not found in settings, cannot send PDKS log data");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error processing PDKS file: {ex.Message}");
            }
        }        private async void OnAlarmFileChanged(object sender, FileSystemEventArgs e, string filePath)
        {
            try
            {
                _logger.Info($"Alarm file changed: {e.FullPath}");
                string content = await _fileHelper.ReadFileWithRetryAsync(filePath);
                
                if (!string.IsNullOrEmpty(content))
                {
                    _logger.Info($"Alarm file content read: {content.Length} bytes");
                    
                    // Create a backup of the file
                    await _fileHelper.CreateBackupFileAsync(filePath, content);
                    
                    // Get firma kod from settings
                    var settings = _configHelper.GetSettings();
                    if (settings != null && !string.IsNullOrEmpty(settings.FirmaKod))
                    {
                        // Send log data to API using new LogSenderService
                        bool success = await _logSenderService.SendAlarmLogAsync(settings.FirmaKod, content);
                        
                        // Clear the source file if successful
                        if (success)
                        {
                            await _fileHelper.ClearFileAsync(filePath);
                            _logger.Info("Alarm file successfully processed and cleared");
                        }
                        else
                        {
                            _logger.Warning("Failed to send Alarm log data to API");
                        }
                    }
                    else
                    {
                        _logger.Warning("FirmaKod not found in settings, cannot send Alarm log data");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error processing Alarm file: {ex.Message}");
            }
        }        private async void OnKameraFileChanged(object sender, FileSystemEventArgs e, string filePath)
        {
            try
            {
                _logger.Info($"Kamera log file changed: {e.FullPath}");
                string content = await _fileHelper.ReadFileWithRetryAsync(filePath);
                
                if (!string.IsNullOrEmpty(content))
                {
                    _logger.Info($"Kamera log file content read: {content.Length} bytes");
                    
                    // Create a backup of the file
                    await _fileHelper.CreateBackupFileAsync(filePath, content);
                    
                    // Get firma kod from settings
                    var settings = _configHelper.GetSettings();
                    if (settings != null && !string.IsNullOrEmpty(settings.FirmaKod))
                    {
                        // Send log data to API using new LogSenderService
                        bool success = await _logSenderService.SendKameraLogAsync(settings.FirmaKod, content);
                        
                        // Clear the source file if successful
                        if (success)
                        {
                            await _fileHelper.ClearFileAsync(filePath);
                            _logger.Info("Kamera log file successfully processed and cleared");
                        }
                        else
                        {
                            _logger.Warning("Failed to send Kamera log data to API");
                        }
                    }
                    else
                    {
                        _logger.Warning("FirmaKod not found in settings, cannot send Kamera log data");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error processing Kamera log file: {ex.Message}");
            }
        }        private async Task CheckApiConnectivityAsync()
        {
            try
            {
                bool isConnected = await _apiHelper.CheckApiConnectivityAsync();
                
                if (isConnected)
                {
                    _logger.Debug("API connection successful");
                      // Check for updates using the new UpdateService
                    try
                    {
                        var appSettings = _configHelper.GetSettings();
                        var firmaKod = appSettings.FirmaKod;
                        var currentVersion = appSettings.Version;
                          if (!string.IsNullOrWhiteSpace(firmaKod))
                        {
                            var updateResult = await _updateService.CheckForUpdatesAsync(firmaKod, currentVersion);
                            if (updateResult?.UpdateAvailable == true)
                            {
                                _logger.Info($"Update available. Version: {updateResult.LatestVersion}, Download URL: {updateResult.DownloadUrl}");
                                
                                // TODO: Implement update notification to user
                                // For now just log the availability
                                // In the future, you might want to show a notification in the system tray
                                // or automatically download and install the update
                            }
                            else
                            {
                                _logger.Debug("Application is up to date");
                            }
                        }
                        else
                        {
                            _logger.Warning("FirmaKod is not configured, skipping update check.");
                        }
                    }
                    catch (Exception updateEx)
                    {
                        _logger.Warning($"Failed to check for updates: {updateEx.Message}");
                    }
                }
                else
                {
                    _logger.Warning("Cannot connect to API endpoint");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error checking API connectivity: {ex.Message}");
            }
        }
    }
}
