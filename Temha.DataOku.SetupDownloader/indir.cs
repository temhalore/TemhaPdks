using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.ServiceProcess; // Servis durumu kontrolü için eklendi
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Temha.DataOku.SetupDownloader
{
    public partial class indir : Form
    {
        private const string ApiBaseUrl_prod = "https://api.yourserver.com/"; // API base URL'iniz
        private const string ApiBaseUrl_local = "https://localhost:44374/"; // API base URL'iniz
        private string setupUrl;
        private static readonly HttpClient httpClient = new HttpClient()
        {
            BaseAddress = new Uri(ApiBaseUrl_local)
        };
        
        // API'den dönecek versiyon bilgisi için modeller
        public class FirmaVersiyon
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public string Version { get; set; }
        }
        
        public class SetupVersiyon
        {
            public string Version { get; set; }
            public string SetupUrl { get; set; }
            public string ReleaseNotes { get; set; }
            public string DefaultInstallPath { get; set; }
            public string ExecutablePath { get; set; }
        }
        
        // API yanıtları için ServiceResponse modeli
        public class ServiceResponse<T>
        {
            public T data { get; set; }
            public string messageType { get; set; }
            public string message { get; set; }
            public bool isSuccess { get; set; }
        }

     
        public indir()
        {
            InitializeComponent();
            // HttpClient için default headers ayarla
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }
        
        // Load event metodunu ekleyin
        private void indir_Load(object sender, EventArgs e)
        {
            // Form yüklendiğinde arayüz elemanlarını varsayılan değerlerle doldur
            tx_izlenecekDosya.Text = "C:\\TemhaPdks\\data.txt";
            chk_debugMode.Checked = false;
        }

        private void btn_kontrolEt_Click(object sender, EventArgs e)
        {
         
            string firmaKodu = tx_firmaKod.Text.Trim(); ;

            if (string.IsNullOrEmpty(firmaKodu))
            {
                MessageBox.Show("Firma kodu boş olamaz!",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                VersiyonKontrolEt(firmaKodu);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Versiyon kontrolü sırasında bir hata oluştu: {ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void VersiyonKontrolEt(string firmaKodu)
        {
            try
            {
                // İlk olarak firma versiyonunu kontrol et
                var firmaVersiyon = GetFirmaVersiyon(firmaKodu);
                if (!firmaVersiyon.Success)
                {
                    MessageBox.Show($"Firma bulunamadı: {firmaVersiyon.Message}",
                        "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Setup versiyonunu al
                var setupVersiyon = GetSetupVersiyon(firmaKodu);

                // Versiyonları karşılaştır
                if (IsNewVersionAvailable(firmaVersiyon.Version, setupVersiyon.Version))
                {
                    var result = MessageBox.Show(
                        $"Yeni bir versiyon mevcut!\n\n" +
                        $"Mevcut Versiyon: {firmaVersiyon.Version}\n" +
                        $"Yeni Versiyon: {setupVersiyon.Version}\n\n" +
                        $"Değişiklikler:\n{setupVersiyon.ReleaseNotes}\n\n" +
                        "Güncellemek ister misiniz?",
                        "Güncelleme Mevcut",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);

                    if (result == DialogResult.Yes)
                    {
                        setupUrl = setupVersiyon.SetupUrl;
                        DownloadSetupFile();
                    }
                }
                else
                {
                    MessageBox.Show("En güncel sürümü kullanıyorsunuz.",
                        "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Versiyon kontrolü hatası: {ex.Message}");
            }
        }
        
        private FirmaVersiyon GetFirmaVersiyon(string firmaKodu)
        {
            try
            {
                var response = httpClient.GetStringAsync($"Api/DataOkuConsoleSetup/getFirmaDataOkuVersiyon?firmaKodu={firmaKodu}")
                                       .GetAwaiter()
                                       .GetResult();
                
                // JSON yanıtı önce ServiceResponse tipinde parse et, sonra data alanını al
                var serviceResponse = JsonSerializer.Deserialize<ServiceResponse<FirmaVersiyon>>(response, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                if (serviceResponse == null || serviceResponse.data == null)
                {
                    throw new Exception("Sunucudan geçerli yanıt alınamadı");
                }
                
                return serviceResponse.data;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"API çağrısı başarısız: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Firma versiyon bilgisi alınamadı: {ex.Message}");
            }
        }
        
        private SetupVersiyon GetSetupVersiyon(string firmaKodu)
        {
            try
            {
                var response = httpClient.GetStringAsync($"Api/DataOkuConsoleSetup/getGuncelDataOkuSetupVersiyon?firmaKodu={firmaKodu}")
                                       .GetAwaiter()
                                       .GetResult();
                                       
                // JSON yanıtı önce ServiceResponse tipinde parse et, sonra data alanını al
                var serviceResponse = JsonSerializer.Deserialize<ServiceResponse<SetupVersiyon>>(response, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                if (serviceResponse == null || serviceResponse.data == null)
                {
                    throw new Exception("Sunucudan geçerli yanıt alınamadı");
                }
                
                return serviceResponse.data;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"API çağrısı başarısız: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Setup versiyon bilgisi alınamadı: {ex.Message}");
            }
        }

        private bool IsNewVersionAvailable(string currentVersion, string newVersion)
        {
            try
            {
                Version current = Version.Parse(currentVersion);
                Version newer = Version.Parse(newVersion);
                return newer > current;
            }
            catch (Exception)
            {
                throw new Exception("Versiyon karşılaştırması yapılamadı. Geçersiz versiyon formatı.");
            }
        }
        
        private void DownloadSetupFile()
        {
            try
            {
                lblStatus.Text = "Güncelleme indiriliyor...";
                string localFile;
                
                // Dosya uzantısını URL'den al
                string extension = Path.GetExtension(setupUrl).ToLower();
                localFile = extension == ".zip" ? "setup.zip" : 
                           extension == ".rar" ? "setup.rar" : "setup.exe";
                
                // Web URL mi yoksa yerel dosya mı kontrolü yap
                if (setupUrl.StartsWith("http://") || setupUrl.StartsWith("https://"))
                {
                    // Web URL ise WebClient ile indir
                    using (var webClient = new WebClient())
                    {
                        webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
                        webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;
                        webClient.DownloadFileAsync(new Uri(setupUrl), localFile);
                    }
                }
                else
                {
                    // Yerel dosya ise kopyala
                    if (File.Exists(setupUrl))
                    {
                        // Önce indirme tamamlandı göstergesi
                        progressBar.Value = 100;
                        lblStatus.Text = "Yerel dosya kopyalanıyor...";
                        
                        // Dosyayı kopyala
                        string targetFile = Path.Combine(Application.StartupPath, localFile);
                        File.Copy(setupUrl, targetFile, true);
                        
                        // İndirme tamamlandı olayını manuel tetikle
                        WebClient_DownloadFileCompleted(null, new AsyncCompletedEventArgs(null, false, null));
                    }
                    else
                    {
                        throw new FileNotFoundException($"Belirtilen dosya bulunamadı: {setupUrl}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"İndirme sırasında bir hata oluştu: {ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            lblStatus.Text = $"İndiriliyor: {e.ProgressPercentage}%";
        }
        
        private string installPath;
        private string executablePath;
        
        private void WebClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e != null)
            {
                if (e.Error != null)
                {
                    MessageBox.Show($"İndirme tamamlanamadı: {e.Error.Message}",
                        "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (e.Cancelled)
                {
                    MessageBox.Show("İndirme işlemi iptal edildi.",
                        "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            lblStatus.Text = "İndirme tamamlandı. Kurulum hazırlanıyor...";
            
            try
            {
                // İndirilen dosyanın yolunu belirle
                string downloadedFilePath = Path.Combine(Application.StartupPath, 
                    Path.GetExtension(setupUrl).ToLower() == ".zip" ? "setup.zip" : 
                    Path.GetExtension(setupUrl).ToLower() == ".rar" ? "setup.rar" : "setup.exe");
                
                // Kurulum klasörünü seçme işlevi
                var setupVersiyon = GetSetupVersiyon(tx_firmaKod.Text.Trim());
                installPath = setupVersiyon.DefaultInstallPath ?? "C:\\loreSoft\\";
                executablePath = setupVersiyon.ExecutablePath ?? "Temha.DataOkuConsole.exe";
                
                // Kullanıcıya kurulum konumunu sor
                using (FolderBrowserDialog folderBrowser = new FolderBrowserDialog())
                {
                    folderBrowser.Description = "DataOkuConsole için kurulum klasörünü seçin:";
                    folderBrowser.InitialDirectory = installPath;
                    folderBrowser.ShowNewFolderButton = true;
                    
                    DialogResult result = folderBrowser.ShowDialog();
                    
                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowser.SelectedPath))
                    {
                        installPath = folderBrowser.SelectedPath;
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        MessageBox.Show("Kurulum iptal edildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                
                // Klasörün sonunda '\' karakteri olduğundan emin ol
                if (!installPath.EndsWith("\\"))
                {
                    installPath += "\\";
                }
                
                lblStatus.Text = $"Dosyalar {installPath} konumuna açılıyor...";
                
                // İndirilen dosyanın bir arşiv olup olmadığını kontrol et
                string extension = Path.GetExtension(downloadedFilePath).ToLower();
                if (extension == ".zip" || extension == ".rar")
                {
                    try
                    {
                        lblStatus.Text = $"Dosyalar {installPath} konumuna açılıyor...";
                        
                        // Kurulum klasörünü oluştur
                        Directory.CreateDirectory(installPath);
                        
                        // Arşiv dosyasını kur (ZIP veya RAR)
                        FileUtils.ExtractArchiveToDirectory(downloadedFilePath, installPath);
                        
                        // Yapılandırma dosyasını oluştur
                        CreateConfigurationJson(installPath);
                        
                        // Kurulum tamamlandı mesajı
                        lblStatus.Text = "Kurulum tamamlandı. Windows servisi olarak kuruluyor...";
                        
                        // Kurulumdan sonra exe'yi Windows servisi olarak kur
                        string exePath = Path.Combine(installPath, executablePath);
                        
                        if (File.Exists(exePath))
                        {
                            ProcessStartInfo startInfo = new ProcessStartInfo();
                            startInfo.FileName = exePath;
                            startInfo.Arguments = "service-install"; // Windows servisi olarak kur
                            startInfo.WorkingDirectory = installPath;
                            startInfo.Verb = "runas"; // Yönetici haklarıyla çalıştır
                            startInfo.RedirectStandardOutput = true;
                            startInfo.UseShellExecute = false;
                            startInfo.CreateNoWindow = true;
                            
                            try 
                            {
                                lblStatus.Text = "Windows servisi kuruluyor...";
                                using (Process process = Process.Start(startInfo))
                                {
                                    string output = process.StandardOutput.ReadToEnd();
                                    process.WaitForExit();
                                    
                                    if (process.ExitCode == 0)
                                    {
                                        // Servisin durumunu kontrol et
                                        if (CheckServiceStatus("TemhaDataOkuConsole"))
                                        {
                                            MessageBox.Show("Windows servisi başarıyla kuruldu ve başlatıldı.", 
                                                "Kurulum Tamamlandı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        }
                                        else
                                        {
                                            MessageBox.Show("Windows servisi kuruldu fakat otomatik başlatılamadı. Servisleri açarak manuel başlatabilirsiniz.", 
                                                "Kurulum Tamamlandı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show($"Servis kurulurken bir hata oluştu. Uygulamayı yönetici haklarıyla çalıştırmayı deneyin.\n\nDetay: {output}", 
                                            "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Servis kurulumu sırasında bir hata oluştu: {ex.Message}", 
                                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            Application.Exit();
                        }
                        else
                        {
                            // Console uygulamasını ara
                            string[] exeFiles = Directory.GetFiles(installPath, "*.exe", SearchOption.AllDirectories);
                            if (exeFiles.Length > 0)
                            {                            
                                string foundExe = exeFiles[0]; // İlk .exe dosyasını al
                                MessageBox.Show($"Belirtilen uygulama dosyası bulunamadı ({executablePath}), ancak {Path.GetFileName(foundExe)} bulundu. Windows servisi olarak kuruluyor.",
                                    "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                
                                ProcessStartInfo startInfo = new ProcessStartInfo();
                                startInfo.FileName = foundExe;
                                startInfo.Arguments = "service-install"; // Windows servisi olarak kur
                                startInfo.WorkingDirectory = Path.GetDirectoryName(foundExe);
                                startInfo.Verb = "runas"; // Yönetici haklarıyla çalıştır
                                startInfo.RedirectStandardOutput = true;
                                startInfo.UseShellExecute = false;
                                startInfo.CreateNoWindow = true;
                                
                                try 
                                {
                                    lblStatus.Text = "Alternatif Windows servisi kuruluyor...";
                                    using (Process process = Process.Start(startInfo))
                                    {
                                        string output = process.StandardOutput.ReadToEnd();
                                        process.WaitForExit();
                                        
                                        if (process.ExitCode == 0)
                                        {
                                            // Servisin durumunu kontrol et
                                            if (CheckServiceStatus("TemhaDataOkuConsole"))
                                            {
                                                MessageBox.Show("Windows servisi başarıyla kuruldu ve başlatıldı.", 
                                                    "Kurulum Tamamlandı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            }
                                            else
                                            {
                                                MessageBox.Show("Windows servisi kuruldu fakat otomatik başlatılamadı. Servisleri açarak manuel başlatabilirsiniz.", 
                                                    "Kurulum Tamamlandı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show($"Servis kurulurken bir hata oluştu. Uygulamayı yönetici haklarıyla çalıştırmayı deneyin.\n\nDetay: {output}", 
                                                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show($"Servis kurulumu sırasında bir hata oluştu: {ex.Message}", 
                                        "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                Application.Exit();
                            }
                            else
                            {
                                MessageBox.Show($"Kurulum tamamlandı, ancak uygulama çalıştırılamadı. Kurulum dizini: {installPath}",
                                    "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                // Kurulum klasörünü aç
                                Process.Start("explorer.exe", installPath);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Arşiv dosyası açılırken hata oluştu: {ex.Message}",
                            "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // Arşiv değil normal bir exe ise servis kurulum parametresiyle çalıştır
                    lblStatus.Text = "Kurulum dosyası Windows servisi olarak kuruluyor...";
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = downloadedFilePath;
                    startInfo.Arguments = "service-install"; // Windows servisi olarak kur
                    startInfo.Verb = "runas"; // Yönetici olarak çalıştır
                    startInfo.RedirectStandardOutput = true;
                    startInfo.UseShellExecute = false;
                    startInfo.CreateNoWindow = true;
                      try 
                    {
                        // Önce eski servisi kaldırmayı dene
                        ProcessStartInfo stopServiceInfo = new ProcessStartInfo();
                        stopServiceInfo.FileName = downloadedFilePath;
                        stopServiceInfo.Arguments = "service-uninstall"; 
                        stopServiceInfo.UseShellExecute = true;
                        stopServiceInfo.Verb = "runas"; // Yönetici olarak çalıştır
                        stopServiceInfo.CreateNoWindow = false;
                        
                        try
                        {
                            // Eski servisi kaldırma işlemini çalıştır
                            Process stopProcess = Process.Start(stopServiceInfo);
                            stopProcess?.WaitForExit();
                            // Servis işlemlerinin tamamlanması için bekle
                            Thread.Sleep(3000);
                        }
                        catch (Exception stopEx)
                        {
                            // Servis durdurulurken hata oluştu, devam et
                            lblStatus.Text = "Eski servis kaldırılırken hata oluştu, devam ediliyor...";
                        }
                        
                        // Şimdi servisi yeniden kur
                        ProcessStartInfo installServiceInfo = new ProcessStartInfo();
                        installServiceInfo.FileName = downloadedFilePath;
                        installServiceInfo.Arguments = "service-install";
                        installServiceInfo.UseShellExecute = true;
                        installServiceInfo.Verb = "runas"; // Yönetici olarak çalıştır
                        installServiceInfo.CreateNoWindow = false;
                        
                        Process installProcess = Process.Start(installServiceInfo);
                        if (installProcess != null)
                        {
                            lblStatus.Text = "Servis kuruluyor, lütfen bekleyin...";
                            installProcess.WaitForExit();
                            
                            // Servis kurulduktan sonra bekle
                            Thread.Sleep(2000);
                            
                            // Servisin durumunu kontrol et
                            if (CheckServiceStatus("TemhaDataOkuConsole"))
                            {
                                MessageBox.Show("Windows servisi başarıyla kuruldu ve başlatıldı.", 
                                    "Kurulum Tamamlandı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("Windows servisi kuruldu fakat otomatik başlatılamadı. Servisleri açarak manuel başlatabilirsiniz.", 
                                    "Kurulum Tamamlandı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Servis kurulumu başlatılamadı. Lütfen uygulamayı yönetici olarak çalıştırdığınızdan emin olun.", 
                                "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Servis kurulumu sırasında bir hata oluştu: {ex.Message}", 
                            "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    Application.Exit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kurulum başlatılamadı: {ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDosyaSec_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Metin Dosyaları (*.txt)|*.txt|Tüm Dosyalar (*.*)|*.*";
                openFileDialog.Title = "İzlenecek Dosyayı Seçin";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    tx_izlenecekDosya.Text = openFileDialog.FileName;
                }
            }
        }
        
        // Yapılandırma JSON dosyasını oluştur
        private void CreateConfigurationJson(string installPath)
        {
            try
            {
                // Kullanıcının girdiği değerlere göre JSON oluştur
                var configObject = new
                {
                    AppSettings = new
                    {
                        FirmaKod = tx_firmaKod.Text.Trim(),
                        IzlenecekDosya = tx_izlenecekDosya.Text.Trim(),
                        IsDebugMode = chk_debugMode.Checked
                    },
                    CoreSettings = new
                    {
                        HataliDosya = Path.Combine(installPath, "hatali\\")
                    }
                };

                // JSON formatına çevir
                string jsonConfig = JsonSerializer.Serialize(configObject, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                // Kurulum klasörüne application.json dosyasını oluştur
                string configPath = Path.Combine(installPath, "application.json");
                File.WriteAllText(configPath, jsonConfig);

                // Hatalı dosyalar klasörünü oluştur
                Directory.CreateDirectory(Path.Combine(installPath, "hatali"));
                
                lblStatus.Text = "Yapılandırma dosyası başarıyla oluşturuldu.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Yapılandırma dosyası oluşturulurken hata: {ex.Message}",
                    "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void lblStatus_Click(object sender, EventArgs e)
        {
        }
        
        // CheckServiceStatus metodu - Servisin çalışma durumunu kontrol eder
        private bool CheckServiceStatus(string serviceName)
        {
            try
            {
                // ServiceController kullanarak servis durumunu kontrol et
                using (ServiceController sc = new ServiceController(serviceName))
                {
                    try
                    {
                        // Servis durumunu kontrol et
                        return sc.Status == ServiceControllerStatus.Running;
                    }
                    catch
                    {
                        // Servis bulunamazsa, komut satırı ile kontrol et
                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.FileName = "sc.exe";
                        startInfo.Arguments = $"query {serviceName}";
                        startInfo.RedirectStandardOutput = true;
                        startInfo.UseShellExecute = false;
                        startInfo.CreateNoWindow = true;
                        
                        using (Process process = Process.Start(startInfo))
                        {
                            string output = process.StandardOutput.ReadToEnd();
                            process.WaitForExit();
                            
                            // Servisin durumunu kontrol et
                            return output.Contains("RUNNING") && process.ExitCode == 0;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
