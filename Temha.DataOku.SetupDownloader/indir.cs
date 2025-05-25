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
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Temha.DataOku.SetupDownloader
{
    public partial class indir : Form
    {
        private const string ApiBaseUrl = "https://api.yourserver.com/"; // API base URL'iniz
        private string setupUrl;
        private static readonly HttpClient httpClient = new HttpClient()
        {
            BaseAddress = new Uri(ApiBaseUrl)
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
        }

     
        public indir()
        {
            InitializeComponent();
            // HttpClient için default headers ayarla
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }        // Load event metodunu ekleyin
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
                var response = httpClient.GetStringAsync($"getFirmaDataOkuVersiyon?firmaKodu={firmaKodu}")
                                       .GetAwaiter()
                                       .GetResult();
                return JsonSerializer.Deserialize<FirmaVersiyon>(response);
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
                var response = httpClient.GetStringAsync($"getGuncelDataOkuSetupVersiyon?firmaKodu={firmaKodu}")
                                       .GetAwaiter()
                                       .GetResult();
                return JsonSerializer.Deserialize<SetupVersiyon>(response);
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
            using (var webClient = new WebClient()) // Büyük dosyalar için WebClient kullanımı daha uygun
            {
                webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
                webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;

                try
                {
                    lblStatus.Text = "Güncelleme indiriliyor...";
                    string localFile = "setup.exe";
                    webClient.DownloadFileAsync(new Uri(setupUrl), localFile);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"İndirme sırasında bir hata oluştu: {ex.Message}",
                        "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            lblStatus.Text = $"İndiriliyor: {e.ProgressPercentage}%";
        }        private void WebClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
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

            lblStatus.Text = "İndirme tamamlandı. Kurulum hazırlanıyor...";
            
            try
            {
                // Setup dosyasını geçici klasöre çıkart ve uygulama verilerini ayarla
                string setupFilePath = Path.Combine(Application.StartupPath, "setup.exe");
                string tempExtractPath = Path.Combine(Path.GetTempPath(), "TemhaPdksSetupTemp");
                string configTargetDirectory = string.Empty;
                
                // Kurulum klasörünü belirle - kullanıcının seçeceği klasör olabilir
                // Şimdilik standart bir yol belirleyelim: C:\\TemhaPdks\\
                configTargetDirectory = "C:\\TemhaPdks\\";
                
                // Yapılandırma klasörünü oluştur
                Directory.CreateDirectory(configTargetDirectory);
                
                // Yapılandırma dosyasını oluştur
                CreateConfigurationJson(configTargetDirectory);
                
                // Setup'ı başlat
                lblStatus.Text = "Kurulum başlatılıyor...";
                
                // setup.exe'yi yönetici olarak çalıştır
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = setupFilePath;
                startInfo.Verb = "runas"; // Yönetici olarak çalıştır
                
                Process.Start(startInfo);
                Application.Exit();
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
                        HataliDosya = "C:\\TemhaPdks\\hatali\\"
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

      
    }
}
