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
        // Asıl setup dosyasının URL'sini buraya girin.
        private const string SetupUrl = "https://example.com/your-setup-file.exe";
        // İndirilecek setup dosyasının yerel adını belirleyin.
        private const string LocalSetupFileName = "setup.exe";

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
    }
}
