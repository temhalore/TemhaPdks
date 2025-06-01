// filepath: D:\Development\ozel\TemhaPdks\SetupAndDosyaOku\Lore.SetupAndDosyaOku\Helpers\FileHelper.cs
using System.Text;

namespace Lore.SetupAndDosyaOku.Helpers
{
    public class FileHelper
    {
        private readonly object _lockObject = new object();
        private readonly Logger _logger;
        private readonly ConfigHelper _configHelper;
        private readonly string _backupDirectory;
        
        public FileHelper(Logger logger, ConfigHelper configHelper)
        {
            _logger = logger;
            _configHelper = configHelper;
            
            // Yedek dizinini ayarla - uygulama dizini altında olsun
            _backupDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");
            
            // Yedek dizini yoksa oluştur
            if (!Directory.Exists(_backupDirectory))
            {
                Directory.CreateDirectory(_backupDirectory);
            }
        }
        
        /// <summary>
        /// Dosyayı güvenli bir şekilde okur, gerekirse birkaç kez deneme yapar
        /// </summary>
        public async Task<string> ReadFileWithRetryAsync(string filePath)
        {
            int maxRetries = 5;
            int retryDelayMs = 500;
            
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    if (!File.Exists(filePath))
                    {
                        _logger.Warning($"Dosya bulunamadı: {filePath}");
                        return string.Empty;
                    }
                    
                    // File.ReadAllTextAsync kullanarak dosyayı güvenli bir şekilde oku
                    return await File.ReadAllTextAsync(filePath);
                }
                catch (IOException ex)
                {
                    _logger.Warning($"Dosya okuma hatası (deneme {i+1}/{maxRetries}): {ex.Message}");
                    
                    if (i < maxRetries - 1)
                    {
                        // Dosya muhtemelen başka bir işlem tarafından kilitli, biraz bekle ve tekrar dene
                        await Task.Delay(retryDelayMs * (i + 1));
                    }
                    else
                    {
                        _logger.Error($"Dosya okunamadı, maksimum deneme sayısına ulaşıldı: {filePath}", ex);
                        throw;
                    }
                }
            }
            
            return string.Empty;
        }
        
        /// <summary>
        /// Dosyayı yedekler
        /// </summary>
        public async Task CreateBackupFileAsync(string filePath, string content)
        {
            if (string.IsNullOrEmpty(content))
                return;
                
            try
            {
                // Zaman damgalı yedek dosya adı oluştur
                string fileName = Path.GetFileName(filePath);
                string fileNameNoExt = Path.GetFileNameWithoutExtension(filePath);
                string extension = Path.GetExtension(filePath);
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string backupFileName = $"{fileNameNoExt}_{timestamp}{extension}";
                
                // Yedek dosya yolu
                string backupPath = Path.Combine(_backupDirectory, backupFileName);
                
                // Yedek dosyasını oluştur
                await File.WriteAllTextAsync(backupPath, content);
                
                _logger.Info($"Yedek dosyası oluşturuldu: {backupPath}");
            }
            catch (Exception ex)
            {
                _logger.Error($"Yedek dosyası oluşturulurken hata oluştu: {filePath}", ex);
            }
        }
        
        /// <summary>
        /// Dosya içeriğini siler
        /// </summary>
        public async Task ClearFileAsync(string filePath)
        {
            try
            {
                // Dosya içeriğini boş bir string ile değiştir
                await File.WriteAllTextAsync(filePath, string.Empty);
                
                _logger.Info($"Dosya içeriği temizlendi: {filePath}");
            }
            catch (Exception ex)
            {
                _logger.Error($"Dosya içeriği temizlenirken hata oluştu: {filePath}", ex);
            }
        }
        
        /// <summary>
        /// API'ye göndermek üzere dosya oluşturur
        /// </summary>
        public async Task<string> CreateFileForSendingAsync(string fileType, string content)
        {
            if (string.IsNullOrEmpty(content))
                return string.Empty;
                
            try
            {
                string firmaKod = _configHelper.GetSettings().FirmaKod;
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string fileName = $"{firmaKod}_{fileType}_{timestamp}.json";
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Outgoing", fileName);
                
                // Outgoing dizini yoksa oluştur
                string outgoingDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Outgoing");
                if (!Directory.Exists(outgoingDir))
                {
                    Directory.CreateDirectory(outgoingDir);
                }
                
                // Dosyayı oluştur
                await File.WriteAllTextAsync(filePath, content);
                
                _logger.Info($"API'ye gönderilecek dosya oluşturuldu: {filePath}");
                
                return filePath;
            }
            catch (Exception ex)
            {
                _logger.Error($"API'ye gönderilecek dosya oluşturulurken hata oluştu", ex);
                return string.Empty;
            }
        }
    }
}
