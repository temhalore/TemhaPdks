using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Temha.DataOku.SetupDownloader
{
    public partial class indir : Form
    {
        private const string ApiBaseUrl = "https://api.yourserver.com/"; // API base URL'iniz
        private readonly HttpClient httpClient;
        private string setupUrl;
        private string firmaKodu;

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
        }

        private void indir_Load(object sender, EventArgs e)
        {

        }

        private void DownloadSetupFile()
        {
            using (var webClient = new WebClient())
            {
                webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
                webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;

                try
                {
                    // Asenkron indirmeyi başlat.
                    webClient.DownloadFileAsync(new Uri(SetupUrl), LocalSetupFileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"İndirme sırasında bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            // ProgressBar ve label’ı güncelle.
            progressBar.Value = e.ProgressPercentage;
            lblStatus.Text = $"İndiriliyor: {e.ProgressPercentage}%";
        }

        private void WebClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show($"İndirme tamamlanamadı: {e.Error.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (e.Cancelled)
            {
                MessageBox.Show("İndirme işlemi iptal edildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            lblStatus.Text = "İndirme tamamlandı. Kurulum başlatılıyor...";
            try
            {
                // İndirilen dosyayı çalıştır.
                Process.Start(LocalSetupFileName);
                Application.Exit(); // Bootstrapper uygulamasını kapat.
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kurulum başlatılamadı: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            // İndirme işlemini başlat.
            DownloadSetupFile();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void lblStatus_Click(object sender, EventArgs e)
        {

        }
    }
}
