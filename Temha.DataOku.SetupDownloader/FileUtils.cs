using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Common;

namespace Temha.DataOku.SetupDownloader
{
    public static class FileUtils
    {        /// <summary>
        /// ZIP dosyasını verilen hedef klasöre çıkarır
        /// </summary>
        /// <param name="zipFilePath">ZIP dosya yolu</param>
        /// <param name="extractPath">Çıkarılacak klasör yolu</param>
        public static void ExtractToDirectory(string zipFilePath, string extractPath)
        {
            try
            {
                // Hedef klasör varsa içeriğini temizle (ama klasörü silme)
                if (Directory.Exists(extractPath))
                {
                    // Klasör içindeki tüm dosyaları ve alt klasörleri sil
                    DirectoryInfo di = new DirectoryInfo(extractPath);
                    foreach (FileInfo file in di.GetFiles())
                    {
                        try { file.Delete(); } catch { }
                    }
                    foreach (DirectoryInfo dir in di.GetDirectories())
                    {
                        try { dir.Delete(true); } catch { }
                    }
                }
                else
                {
                    // Klasör yoksa oluştur
                    Directory.CreateDirectory(extractPath);
                }
                
                // ZIP dosyasını çıkar
                using (ZipArchive archive = ZipFile.OpenRead(zipFilePath))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        // Klasör ise
                        if (string.IsNullOrEmpty(entry.Name))
                        {
                            string dirPath = Path.Combine(extractPath, entry.FullName.TrimEnd('/'));
                            Directory.CreateDirectory(dirPath);
                            continue;
                        }
                        
                        // Dosya ise
                        string destinationPath = Path.Combine(extractPath, entry.FullName);
                        
                        // Dosyanın dizini yoksa oluştur
                        string destinationDir = Path.GetDirectoryName(destinationPath);
                        if (!Directory.Exists(destinationDir))
                        {
                            Directory.CreateDirectory(destinationDir);
                        }
                        
                        // Dosyayı çıkar
                        entry.ExtractToFile(destinationPath, true);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"ZIP dosyası çıkartılırken bir hata oluştu: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// RAR dosyasını verilen hedef klasöre çıkarır
        /// </summary>
        /// <param name="rarFilePath">RAR dosya yolu</param>
        /// <param name="extractPath">Çıkarılacak klasör yolu</param>
        public static void ExtractRarToDirectory(string rarFilePath, string extractPath)
        {
            try
            {
                // Hedef klasör varsa içeriğini temizle (ama klasörü silme)
                if (Directory.Exists(extractPath))
                {
                    // Klasör içindeki tüm dosyaları ve alt klasörleri sil
                    DirectoryInfo di = new DirectoryInfo(extractPath);
                    foreach (FileInfo file in di.GetFiles())
                    {
                        try { file.Delete(); } catch { }
                    }
                    foreach (DirectoryInfo dir in di.GetDirectories())
                    {
                        try { dir.Delete(true); } catch { }
                    }
                }
                else
                {
                    // Klasör yoksa oluştur
                    Directory.CreateDirectory(extractPath);
                }
                
                // RAR dosyasını çıkar
                using (var archive = RarArchive.Open(rarFilePath))
                {
                    foreach (var entry in archive.Entries)
                    {
                        if (!entry.IsDirectory)
                        {
                            // Dosyanın tam yolu
                            string destinationPath = Path.Combine(extractPath, entry.Key);
                            
                            // Dosyanın dizini yoksa oluştur
                            string destinationDir = Path.GetDirectoryName(destinationPath);
                            if (!Directory.Exists(destinationDir))
                            {
                                Directory.CreateDirectory(destinationDir);
                            }
                            
                            // Dosyayı çıkar
                            entry.WriteToFile(destinationPath, new ExtractionOptions() 
                            { 
                                ExtractFullPath = true, 
                                Overwrite = true 
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"RAR dosyası çıkartılırken bir hata oluştu: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// Arşiv dosyasını tipine göre çıkarır (ZIP veya RAR)
        /// </summary>
        /// <param name="archiveFilePath">Arşiv dosya yolu</param>
        /// <param name="extractPath">Çıkarılacak klasör yolu</param>
        public static void ExtractArchiveToDirectory(string archiveFilePath, string extractPath)
        {
            string extension = Path.GetExtension(archiveFilePath).ToLower();
            
            if (extension == ".zip")
            {
                ExtractToDirectory(archiveFilePath, extractPath);
            }
            else if (extension == ".rar")
            {
                ExtractRarToDirectory(archiveFilePath, extractPath);
            }
            else
            {
                throw new NotSupportedException($"Desteklenmeyen arşiv formatı: {extension}");
            }
        }

        /// <summary>
        /// Verilen klasördeki tüm dosya ve klasörleri hedef klasöre kopyalar
        /// </summary>
        /// <param name="sourceDir">Kaynak klasör</param>
        /// <param name="targetDir">Hedef klasör</param>
        public static void CopyDirectory(string sourceDir, string targetDir)
        {
            Directory.CreateDirectory(targetDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)), true);
            }

            foreach (var directory in Directory.GetDirectories(sourceDir))
            {
                CopyDirectory(directory, Path.Combine(targetDir, Path.GetFileName(directory)));
            }
        }

        /// <summary>
        /// Dosyayı okur ve içeriğini string olarak döner
        /// </summary>
        /// <param name="filePath">Dosya yolu</param>
        public static string ReadAllText(string filePath)
        {
            return File.ReadAllText(filePath, Encoding.UTF8);
        }

        /// <summary>
        /// String içeriği dosyaya yazar
        /// </summary>
        /// <param name="filePath">Dosya yolu</param>
        /// <param name="content">Dosya içeriği</param>
        public static void WriteAllText(string filePath, string content)
        {
            File.WriteAllText(filePath, content, Encoding.UTF8);
        }
    }
}
