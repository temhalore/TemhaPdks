using Lore.SetupAndDosyaOku.Models;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Lore.SetupAndDosyaOku.Helpers
{
    public class ConfigHelper
    {
        private readonly string _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
        private readonly object _lockObject = new();
        private AppConfiguration? _config;
        private readonly Logger _logger;
          public ConfigHelper(Logger logger)
        {
            _logger = logger;
            LoadConfig();
        }
        
        public AppConfiguration GetConfig()
        {
            if (_config == null)
            {
                lock (_lockObject)
                {
                    if (_config == null)
                    {
                        LoadConfig();
                    }
                }
            }
            
            return _config ?? new AppConfiguration();
        }
          private void LoadConfig()
        {
            try
            {
                if (File.Exists(_configPath))
                {
                    _logger.Info($"Konfigürasyon dosyası okunuyor: {_configPath}");
                    var configJson = File.ReadAllText(_configPath);
                    _config = JsonSerializer.Deserialize<AppConfiguration>(configJson) ?? new AppConfiguration();
                    _logger.Info("Konfigürasyon başarıyla yüklendi.");
                }
                else
                {
                    _logger.Warning($"Konfigürasyon dosyası bulunamadı: {_configPath}, varsayılan ayarlar kullanılacak.");
                    _config = new AppConfiguration();
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Konfigürasyon dosyası okunurken hata oluştu: {_configPath}", ex);
                _config = new AppConfiguration();
            }
        }
        
        public void SaveConfig(AppConfiguration config)
        {
            try
            {
                lock (_lockObject)
                {
                    _config = config;
                    
                    string dirPath = Path.GetDirectoryName(_configPath) ?? string.Empty;
                    if (!string.IsNullOrEmpty(dirPath) && !Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }
                      var jsonOptions = new JsonSerializerOptions
                    {
                        WriteIndented = true
                    };
                    
                    string jsonString = JsonSerializer.Serialize(config, jsonOptions);
                    File.WriteAllText(_configPath, jsonString);
                    
                    _logger.Info($"Konfigürasyon dosyası başarıyla kaydedildi: {_configPath}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Konfigürasyon dosyası kaydedilirken hata oluştu: {_configPath}", ex);
            }
        }
        
        public bool IsConfigurationValid()
        {
            var config = GetConfig();
            var appSettings = config?.AppSettings;
            
            if (appSettings == null)
                return false;
                
            if (string.IsNullOrWhiteSpace(appSettings.FirmaKod))
                return false;
                
            if (string.IsNullOrWhiteSpace(appSettings.PdksKayitDosyaYolu) &&
                string.IsNullOrWhiteSpace(appSettings.AlarmKayitDosyaYolu) &&                string.IsNullOrWhiteSpace(appSettings.KameraLogDosyaYolu))
                return false;
                
            return true;
        }
        
        /// <summary>
        /// AppSettings'i döndürür. Bu metod Program ve SetupForm tarafından kullanılıyor.
        /// </summary>
        public AppSettings GetSettings()
        {
            return GetConfig().AppSettings ?? new AppSettings();
        }
        
        /// <summary>
        /// AppSettings'i kaydeder. Bu metod SetupForm tarafından kullanılıyor.
        /// </summary>
        public void SaveSettings(AppSettings settings)
        {
            var config = GetConfig();
            config.AppSettings = settings;
            SaveConfig(config);
        }
    }
}
