using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace Temha.DataOku.SetupDownloader
{
    public static class FileUtils
    {
        /// <summary>
        /// ZIP dosyasını verilen hedef klasöre çıkarır
        /// </summary>
        /// <param name="zipFilePath">ZIP dosya yolu</param>
        /// <param name="extractPath">Çıkarılacak klasör yolu</param>
        public static void ExtractToDirectory(string zipFilePath, string extractPath)
        {
            if (Directory.Exists(extractPath))
            {
                Directory.Delete(extractPath, true);
            }
            
            Directory.CreateDirectory(extractPath);
            ZipFile.ExtractToDirectory(zipFilePath, extractPath);
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
