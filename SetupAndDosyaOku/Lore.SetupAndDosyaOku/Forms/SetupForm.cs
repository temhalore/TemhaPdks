using System;
using System.IO;
using System.Windows.Forms;
using Lore.SetupAndDosyaOku.Helpers;
using Lore.SetupAndDosyaOku.Models;

namespace Lore.SetupAndDosyaOku.Forms
{
    public partial class SetupForm : Form
    {
        private readonly ConfigHelper _configHelper;
        private readonly Logger _logger;
        private readonly StartupHelper _startupHelper;

        // Controls
        private TextBox txtFirmaKodu;
        private TextBox txtPdksKayitDosyaYolu;
        private TextBox txtAlarmKayitDosyaYolu;
        private TextBox txtKameraLogDosyaYolu;
        private TextBox txtApiEndpoint;
        private CheckBox chkStartWithWindows;
        private CheckBox chkCreateDesktopShortcut;

        public SetupForm(ConfigHelper configHelper, Logger logger, StartupHelper startupHelper)
        {
            _configHelper = configHelper;
            _logger = logger;
            _startupHelper = startupHelper;
            
            InitializeComponent();
        }

        #region Designer Generated Code
        
        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // Form settings
            this.Text = "Lore Dosya İzleyici - Kurulum";
            this.ClientSize = new System.Drawing.Size(600, 400);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new System.Drawing.Font("Segoe UI", 9F);

            // Create title label
            var lblTitle = new Label
            {
                Text = "Lore Dosya İzleyici Ayarları",
                Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold),
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(560, 30),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblTitle);

            // Create instruction label
            var lblInstructions = new Label
            {
                Text = "Lütfen gerekli ayarları yapınız:",
                Location = new System.Drawing.Point(20, 60),
                Size = new System.Drawing.Size(560, 20)
            };
            this.Controls.Add(lblInstructions);

            // Create firma kodu label and textbox
            var lblFirmaKodu = new Label
            {
                Text = "Firma Kodu:",
                Location = new System.Drawing.Point(20, 90),
                Size = new System.Drawing.Size(150, 20),
                TextAlign = System.Drawing.ContentAlignment.MiddleRight
            };
            this.Controls.Add(lblFirmaKodu);

            txtFirmaKodu = new TextBox
            {
                Location = new System.Drawing.Point(180, 90),
                Size = new System.Drawing.Size(250, 23)
            };
            this.Controls.Add(txtFirmaKodu);

            // Create PDKS file path label and textbox with browse button
            var lblPdksKayitDosyaYolu = new Label
            {
                Text = "PDKS Kayıt Dosya Yolu:",
                Location = new System.Drawing.Point(20, 120),
                Size = new System.Drawing.Size(150, 20),
                TextAlign = System.Drawing.ContentAlignment.MiddleRight
            };
            this.Controls.Add(lblPdksKayitDosyaYolu);

            txtPdksKayitDosyaYolu = new TextBox
            {
                Location = new System.Drawing.Point(180, 120),
                Size = new System.Drawing.Size(250, 23)
            };
            this.Controls.Add(txtPdksKayitDosyaYolu);

            var btnPdksBrowse = new Button
            {
                Text = "...",
                Location = new System.Drawing.Point(440, 120),
                Size = new System.Drawing.Size(30, 23)
            };
            btnPdksBrowse.Click += (sender, e) => BrowseForFile(txtPdksKayitDosyaYolu, "PDKS Kayıt Dosyası (*.txt)|*.txt|Tüm Dosyalar (*.*)|*.*");
            this.Controls.Add(btnPdksBrowse);

            // Create alarm file path label and textbox with browse button
            var lblAlarmKayitDosyaYolu = new Label
            {
                Text = "Alarm Kayıt Dosya Yolu:",
                Location = new System.Drawing.Point(20, 150),
                Size = new System.Drawing.Size(150, 20),
                TextAlign = System.Drawing.ContentAlignment.MiddleRight
            };
            this.Controls.Add(lblAlarmKayitDosyaYolu);

            txtAlarmKayitDosyaYolu = new TextBox
            {
                Location = new System.Drawing.Point(180, 150),
                Size = new System.Drawing.Size(250, 23)
            };
            this.Controls.Add(txtAlarmKayitDosyaYolu);

            var btnAlarmBrowse = new Button
            {
                Text = "...",
                Location = new System.Drawing.Point(440, 150),
                Size = new System.Drawing.Size(30, 23)
            };
            btnAlarmBrowse.Click += (sender, e) => BrowseForFile(txtAlarmKayitDosyaYolu, "Alarm Kayıt Dosyası (*.txt)|*.txt|Tüm Dosyalar (*.*)|*.*");
            this.Controls.Add(btnAlarmBrowse);

            // Create kamera log file path label and textbox with browse button
            var lblKameraLogDosyaYolu = new Label
            {
                Text = "Kamera Log Dosya Yolu:",
                Location = new System.Drawing.Point(20, 180),
                Size = new System.Drawing.Size(150, 20),
                TextAlign = System.Drawing.ContentAlignment.MiddleRight
            };
            this.Controls.Add(lblKameraLogDosyaYolu);

            txtKameraLogDosyaYolu = new TextBox
            {
                Location = new System.Drawing.Point(180, 180),
                Size = new System.Drawing.Size(250, 23)
            };
            this.Controls.Add(txtKameraLogDosyaYolu);

            var btnKameraBrowse = new Button
            {
                Text = "...",
                Location = new System.Drawing.Point(440, 180),
                Size = new System.Drawing.Size(30, 23)
            };
            btnKameraBrowse.Click += (sender, e) => BrowseForFile(txtKameraLogDosyaYolu, "Kamera Log Dosyası (*.txt)|*.txt|Tüm Dosyalar (*.*)|*.*");
            this.Controls.Add(btnKameraBrowse);

            // Create API endpoint label and textbox
            var lblApiEndpoint = new Label
            {
                Text = "API Endpoint:",
                Location = new System.Drawing.Point(20, 210),
                Size = new System.Drawing.Size(150, 20),
                TextAlign = System.Drawing.ContentAlignment.MiddleRight
            };
            this.Controls.Add(lblApiEndpoint);

            txtApiEndpoint = new TextBox
            {
                Location = new System.Drawing.Point(180, 210),
                Size = new System.Drawing.Size(250, 23)
            };
            this.Controls.Add(txtApiEndpoint);

            // Create startup options
            chkStartWithWindows = new CheckBox
            {
                Text = "Windows ile başlat",
                Location = new System.Drawing.Point(180, 250),
                Size = new System.Drawing.Size(250, 20),
                Checked = true
            };
            this.Controls.Add(chkStartWithWindows);

            chkCreateDesktopShortcut = new CheckBox
            {
                Text = "Masaüstüne kısayol oluştur",
                Location = new System.Drawing.Point(180, 280),
                Size = new System.Drawing.Size(250, 20),
                Checked = true
            };
            this.Controls.Add(chkCreateDesktopShortcut);

            // Create save and cancel buttons
            var btnSave = new Button
            {
                Text = "Kaydet",
                Location = new System.Drawing.Point(180, 330),
                Size = new System.Drawing.Size(100, 30)
            };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            var btnCancel = new Button
            {
                Text = "İptal",
                Location = new System.Drawing.Point(300, 330),
                Size = new System.Drawing.Size(100, 30)
            };
            btnCancel.Click += BtnCancel_Click;
            this.Controls.Add(btnCancel);

            this.ResumeLayout(false);
            this.PerformLayout();

            // Load settings
            LoadSettings();
        }

        #endregion

        private void BrowseForFile(TextBox textBox, string filter)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = filter;
                openFileDialog.CheckFileExists = false;
                
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    textBox.Text = openFileDialog.FileName;
                }
            }
        }

        private void LoadSettings()
        {
            try
            {
                var settings = _configHelper.GetSettings();
                txtFirmaKodu.Text = settings.FirmaKod;
                txtPdksKayitDosyaYolu.Text = settings.PdksKayitDosyaYolu;
                txtAlarmKayitDosyaYolu.Text = settings.AlarmKayitDosyaYolu;
                txtKameraLogDosyaYolu.Text = settings.KameraLogDosyaYolu;
                txtApiEndpoint.Text = settings.ApiEndpoint;
            }
            catch (Exception ex)
            {
                _logger.Error($"Ayarlar yüklenirken hata oluştu: {ex.Message}");
                MessageBox.Show($"Ayarlar yüklenirken hata oluştu: {ex.Message}", "Hata", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateSettings()
        {
            if (string.IsNullOrWhiteSpace(txtFirmaKodu.Text))
            {
                MessageBox.Show("Firma kodu boş bırakılamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFirmaKodu.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPdksKayitDosyaYolu.Text))
            {
                MessageBox.Show("PDKS kayıt dosya yolu boş bırakılamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPdksKayitDosyaYolu.Focus();
                return false;
            }
            
            if (string.IsNullOrWhiteSpace(txtAlarmKayitDosyaYolu.Text))
            {
                MessageBox.Show("Alarm kayıt dosya yolu boş bırakılamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAlarmKayitDosyaYolu.Focus();
                return false;
            }
            
            if (string.IsNullOrWhiteSpace(txtKameraLogDosyaYolu.Text))
            {
                MessageBox.Show("Kamera log dosya yolu boş bırakılamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtKameraLogDosyaYolu.Focus();
                return false;
            }
            
            if (string.IsNullOrWhiteSpace(txtApiEndpoint.Text))
            {
                MessageBox.Show("API endpoint boş bırakılamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtApiEndpoint.Focus();
                return false;
            }

            return true;
        }

        private void SaveSettings()
        {
            try
            {
                // Update settings
                var settings = _configHelper.GetSettings();
                settings.FirmaKod = txtFirmaKodu.Text;
                settings.PdksKayitDosyaYolu = txtPdksKayitDosyaYolu.Text;
                settings.AlarmKayitDosyaYolu = txtAlarmKayitDosyaYolu.Text;
                settings.KameraLogDosyaYolu = txtKameraLogDosyaYolu.Text;
                settings.ApiEndpoint = txtApiEndpoint.Text;
                
                // Save to file
                _configHelper.SaveSettings(settings);
                
                // Configure startup options
                if (chkStartWithWindows.Checked)
                {
                    _startupHelper.AddToStartup();
                }
                else
                {
                    _startupHelper.RemoveFromStartup();
                }
                
                // Create desktop shortcut if requested
                if (chkCreateDesktopShortcut.Checked)
                {
                    _startupHelper.CreateDesktopShortcut();
                }
                
                _logger.Info("Ayarlar başarıyla kaydedildi.");
                MessageBox.Show("Ayarlar başarıyla kaydedildi.", "Bilgi", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                _logger.Error($"Ayarlar kaydedilirken hata oluştu: {ex.Message}");
                MessageBox.Show($"Ayarlar kaydedilirken hata oluştu: {ex.Message}", "Hata", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (ValidateSettings())
            {
                SaveSettings();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
