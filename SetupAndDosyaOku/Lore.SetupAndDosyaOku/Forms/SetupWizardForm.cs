using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lore.SetupAndDosyaOku.Helpers;
using Lore.SetupAndDosyaOku.Models;

namespace Lore.SetupAndDosyaOku.Forms
{
    public partial class SetupWizardForm : Form
    {
        private readonly ConfigHelper _configHelper;
        private readonly Logger _logger;
        private readonly StartupHelper _startupHelper;
        private readonly ApiHelper _apiHelper;

        private WizardStep _currentStep = WizardStep.FirmaKodu;
        private WizardData _wizardData = new WizardData();

        // Controls for different steps
        private Panel _stepPanel;
        private Button _btnPrevious;
        private Button _btnNext;
        private Button _btnCancel;
        private Label _lblProgress;
        private ProgressBar _progressBar;

        // Step 1 - Firma Kodu
        private TextBox _txtFirmaKodu;
        private Label _lblFirmaKoduError;

        // Step 2 - Modül Seçimi (Server'dan alınan bilgilere göre)
        private CheckBox _chkPdks;
        private CheckBox _chkAlarm;
        private CheckBox _chkKamera;
        private Label _lblModulInfo;

        // Step 3 - Dosya Yolları
        private TextBox _txtPdksYol;
        private TextBox _txtAlarmYol;
        private TextBox _txtKameraYol;
        private Button _btnPdksBrowse;
        private Button _btnAlarmBrowse;
        private Button _btnKameraBrowse;

        // Step 4 - Tamamlama
        private CheckBox _chkStartWithWindows;
        private CheckBox _chkCreateDesktopShortcut;
        private Label _lblSummary;

        public SetupWizardForm(ConfigHelper configHelper, Logger logger, StartupHelper startupHelper, ApiHelper apiHelper)
        {
            _configHelper = configHelper;
            _logger = logger;
            _startupHelper = startupHelper;
            _apiHelper = apiHelper;
            
            InitializeComponent();
            ShowStep(_currentStep);
        }

        #region Designer Generated Code
        
        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // Form settings
            this.Text = "Lore Dosya İzleyici - Kurulum Sihirbazı";
            this.ClientSize = new Size(700, 500);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9F);
            this.BackColor = Color.White;

            // Create header panel
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(0, 122, 204)
            };

            var lblTitle = new Label
            {
                Text = "Lore Dosya İzleyici Kurulum Sihirbazı",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 20),
                Size = new Size(660, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
            headerPanel.Controls.Add(lblTitle);

            var lblSubtitle = new Label
            {
                Text = "Uygulamanızı kullanmaya başlamak için gerekli ayarları yapın",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.LightGray,
                Location = new Point(20, 45),
                Size = new Size(660, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };
            headerPanel.Controls.Add(lblSubtitle);

            this.Controls.Add(headerPanel);

            // Create progress bar
            _progressBar = new ProgressBar
            {
                Location = new Point(20, 90),
                Size = new Size(660, 20),
                Maximum = 4,
                Value = 1,
                Style = ProgressBarStyle.Continuous
            };
            this.Controls.Add(_progressBar);

            _lblProgress = new Label
            {
                Text = "Adım 1/4: Firma Kodu",
                Location = new Point(20, 115),
                Size = new Size(660, 20),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            this.Controls.Add(_lblProgress);

            // Create step panel
            _stepPanel = new Panel
            {
                Location = new Point(20, 145),
                Size = new Size(660, 280),
                BackColor = Color.White
            };
            this.Controls.Add(_stepPanel);

            // Create footer buttons
            _btnCancel = new Button
            {
                Text = "İptal",
                Location = new Point(500, 440),
                Size = new Size(80, 30),
                DialogResult = DialogResult.Cancel
            };
            _btnCancel.Click += (s, e) => this.Close();
            this.Controls.Add(_btnCancel);

            _btnNext = new Button
            {
                Text = "İleri >",
                Location = new Point(590, 440),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _btnNext.Click += BtnNext_Click;
            this.Controls.Add(_btnNext);

            _btnPrevious = new Button
            {
                Text = "< Geri",
                Location = new Point(410, 440),
                Size = new Size(80, 30),
                Enabled = false
            };
            _btnPrevious.Click += BtnPrevious_Click;
            this.Controls.Add(_btnPrevious);

            this.ResumeLayout(false);
        }

        #endregion

        private void ShowStep(WizardStep step)
        {
            _stepPanel.Controls.Clear();
            _currentStep = step;
            _progressBar.Value = (int)step;

            switch (step)
            {
                case WizardStep.FirmaKodu:
                    ShowFirmaKoduStep();
                    _lblProgress.Text = "Adım 1/4: Firma Kodu";
                    _btnPrevious.Enabled = false;
                    _btnNext.Text = "İleri >";
                    break;
                case WizardStep.ModulSecimi:
                    ShowModulSecimiStep();
                    _lblProgress.Text = "Adım 2/4: Modül Seçimi";
                    _btnPrevious.Enabled = true;
                    _btnNext.Text = "İleri >";
                    break;
                case WizardStep.DosyaYollari:
                    ShowDosyaYollariStep();
                    _lblProgress.Text = "Adım 3/4: Dosya Yolları";
                    _btnPrevious.Enabled = true;
                    _btnNext.Text = "İleri >";
                    break;
                case WizardStep.Tamamlama:
                    ShowTamamlamaStep();
                    _lblProgress.Text = "Adım 4/4: Tamamlama";
                    _btnPrevious.Enabled = true;
                    _btnNext.Text = "Bitir";
                    break;
            }
        }

        private void ShowFirmaKoduStep()
        {
            var lblInfo = new Label
            {
                Text = "Lütfen firma kodunuzu girin:",
                Location = new Point(20, 20),
                Size = new Size(620, 30),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold)
            };
            _stepPanel.Controls.Add(lblInfo);

            var lblDescription = new Label
            {
                Text = "Firma kodunuz size sistem yöneticiniz tarafından verilmiştir. Bu kod sistemin hangi firma için çalışacağını belirler.",
                Location = new Point(20, 60),
                Size = new Size(620, 40),
                Font = new Font("Segoe UI", 9F)
            };
            _stepPanel.Controls.Add(lblDescription);

            var lblFirmaKodu = new Label
            {
                Text = "Firma Kodu:",
                Location = new Point(20, 120),
                Size = new Size(100, 20)
            };
            _stepPanel.Controls.Add(lblFirmaKodu);

            _txtFirmaKodu = new TextBox
            {
                Location = new Point(130, 120),
                Size = new Size(200, 23),
                Font = new Font("Segoe UI", 9F),
                Text = _wizardData.FirmaKod
            };
            _stepPanel.Controls.Add(_txtFirmaKodu);

            _lblFirmaKoduError = new Label
            {
                Location = new Point(130, 150),
                Size = new Size(400, 20),
                ForeColor = Color.Red,
                Font = new Font("Segoe UI", 8F)
            };
            _stepPanel.Controls.Add(_lblFirmaKoduError);

            // Focus on textbox
            _txtFirmaKodu.Focus();
        }

        private void ShowModulSecimiStep()
        {
            var lblInfo = new Label
            {
                Text = "Firma modülleriniz otomatik olarak belirlendi:",
                Location = new Point(20, 20),
                Size = new Size(620, 30),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold)
            };
            _stepPanel.Controls.Add(lblInfo);

            _lblModulInfo = new Label
            {
                Text = "Sunucudan modül bilgileri alınıyor...",
                Location = new Point(20, 60),
                Size = new Size(620, 40),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.Gray
            };
            _stepPanel.Controls.Add(_lblModulInfo);

            _chkPdks = new CheckBox
            {
                Text = "PDKS (Personel Devam Kontrol Sistemi)",
                Location = new Point(40, 120),
                Size = new Size(300, 23),
                Enabled = false,
                Checked = _wizardData.IsPdks
            };
            _stepPanel.Controls.Add(_chkPdks);

            _chkAlarm = new CheckBox
            {
                Text = "Alarm Sistemi",
                Location = new Point(40, 150),
                Size = new Size(300, 23),
                Enabled = false,
                Checked = _wizardData.IsAlarm
            };
            _stepPanel.Controls.Add(_chkAlarm);            _chkKamera = new CheckBox
            {
                Text = "Kamera Log Sistemi",
                Location = new Point(40, 180),
                Size = new Size(300, 23),
                Enabled = false,
                Checked = _wizardData.IsKamera
            };
            _stepPanel.Controls.Add(_chkKamera);
            
            // Modül bilgilerini güncelle
            UpdateModulDisplay();
        }

        private void ShowDosyaYollariStep()
        {
            var lblInfo = new Label
            {
                Text = "Dosya yollarını belirleyin:",
                Location = new Point(20, 20),
                Size = new Size(620, 30),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold)
            };
            _stepPanel.Controls.Add(lblInfo);

            var lblDescription = new Label
            {
                Text = "Sistemin izleyeceği dosyaların yollarını belirtiniz. Bu dosyalar sürekli olarak izlenecek ve değişiklikler sunucuya gönderilecektir.",
                Location = new Point(20, 60),
                Size = new Size(620, 40),
                Font = new Font("Segoe UI", 9F)
            };
            _stepPanel.Controls.Add(lblDescription);

            int yPos = 110;

            // PDKS dosya yolu (sadece PDKS aktifse göster)
            if (_wizardData.IsPdks)
            {
                var lblPdks = new Label
                {
                    Text = "PDKS Kayıt Dosya Yolu:",
                    Location = new Point(20, yPos),
                    Size = new Size(150, 20)
                };
                _stepPanel.Controls.Add(lblPdks);

                _txtPdksYol = new TextBox
                {
                    Location = new Point(180, yPos),
                    Size = new Size(400, 23),
                    Text = _wizardData.PdksKayitDosyaYolu
                };
                _stepPanel.Controls.Add(_txtPdksYol);

                _btnPdksBrowse = new Button
                {
                    Text = "...",
                    Location = new Point(590, yPos),
                    Size = new Size(30, 23)
                };
                _btnPdksBrowse.Click += (s, e) => BrowseForFile(_txtPdksYol, "PDKS Kayıt Dosyası (*.txt)|*.txt|Tüm Dosyalar (*.*)|*.*");
                _stepPanel.Controls.Add(_btnPdksBrowse);

                yPos += 35;
            }

            // Alarm dosya yolu (sadece Alarm aktifse göster)
            if (_wizardData.IsAlarm)
            {
                var lblAlarm = new Label
                {
                    Text = "Alarm Kayıt Dosya Yolu:",
                    Location = new Point(20, yPos),
                    Size = new Size(150, 20)
                };
                _stepPanel.Controls.Add(lblAlarm);

                _txtAlarmYol = new TextBox
                {
                    Location = new Point(180, yPos),
                    Size = new Size(400, 23),
                    Text = _wizardData.AlarmKayitDosyaYolu
                };
                _stepPanel.Controls.Add(_txtAlarmYol);

                _btnAlarmBrowse = new Button
                {
                    Text = "...",
                    Location = new Point(590, yPos),
                    Size = new Size(30, 23)
                };
                _btnAlarmBrowse.Click += (s, e) => BrowseForFile(_txtAlarmYol, "Alarm Kayıt Dosyası (*.txt)|*.txt|Tüm Dosyalar (*.*)|*.*");
                _stepPanel.Controls.Add(_btnAlarmBrowse);

                yPos += 35;
            }

            // Kamera dosya yolu (sadece Kamera aktifse göster)
            if (_wizardData.IsKamera)
            {
                var lblKamera = new Label
                {
                    Text = "Kamera Log Dosya Yolu:",
                    Location = new Point(20, yPos),
                    Size = new Size(150, 20)
                };
                _stepPanel.Controls.Add(lblKamera);

                _txtKameraYol = new TextBox
                {
                    Location = new Point(180, yPos),
                    Size = new Size(400, 23),
                    Text = _wizardData.KameraLogDosyaYolu
                };
                _stepPanel.Controls.Add(_txtKameraYol);

                _btnKameraBrowse = new Button
                {
                    Text = "...",
                    Location = new Point(590, yPos),
                    Size = new Size(30, 23)
                };
                _btnKameraBrowse.Click += (s, e) => BrowseForFile(_txtKameraYol, "Kamera Log Dosyası (*.txt)|*.txt|Tüm Dosyalar (*.*)|*.*");
                _stepPanel.Controls.Add(_btnKameraBrowse);
            }
        }

        private void ShowTamamlamaStep()
        {
            var lblInfo = new Label
            {
                Text = "Kurulum tamamlanıyor:",
                Location = new Point(20, 20),
                Size = new Size(620, 30),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold)
            };
            _stepPanel.Controls.Add(lblInfo);

            // Özet bilgileri
            _lblSummary = new Label
            {
                Text = CreateSummaryText(),
                Location = new Point(20, 60),
                Size = new Size(620, 120),
                Font = new Font("Segoe UI", 9F),
                BackColor = Color.FromArgb(245, 245, 245),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10)
            };
            _stepPanel.Controls.Add(_lblSummary);

            // Başlangıç seçenekleri
            _chkStartWithWindows = new CheckBox
            {
                Text = "Windows ile birlikte başlat",
                Location = new Point(20, 200),
                Size = new Size(250, 23),
                Checked = _wizardData.StartWithWindows
            };
            _stepPanel.Controls.Add(_chkStartWithWindows);

            _chkCreateDesktopShortcut = new CheckBox
            {
                Text = "Masaüstü kısayolu oluştur",
                Location = new Point(20, 230),
                Size = new Size(250, 23),
                Checked = _wizardData.CreateDesktopShortcut
            };
            _stepPanel.Controls.Add(_chkCreateDesktopShortcut);
        }

        private string CreateSummaryText()
        {
            var summary = $"Firma Kodu: {_wizardData.FirmaKod}\n\n";
            summary += "Aktif Modüller:\n";
            
            if (_wizardData.IsPdks)
                summary += $"• PDKS: {_wizardData.PdksKayitDosyaYolu}\n";
            if (_wizardData.IsAlarm)
                summary += $"• Alarm: {_wizardData.AlarmKayitDosyaYolu}\n";
            if (_wizardData.IsKamera)
                summary += $"• Kamera: {_wizardData.KameraLogDosyaYolu}\n";

            return summary;
        }

        private void BrowseForFile(TextBox textBox, string filter)
        {
            using var openFileDialog = new OpenFileDialog
            {
                Filter = filter,
                CheckFileExists = false
            };
            
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBox.Text = openFileDialog.FileName;
            }
        }

        private async void BtnNext_Click(object sender, EventArgs e)
        {
            try
            {
                if (await ValidateCurrentStep())
                {
                    SaveCurrentStepData();

                    if (_currentStep == WizardStep.Tamamlama)
                    {
                        await CompleteSetup();
                    }
                    else
                    {
                        ShowStep(_currentStep + 1);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Wizard adım ilerletme hatası: {ex.Message}");
                MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnPrevious_Click(object sender, EventArgs e)
        {
            if (_currentStep > WizardStep.FirmaKodu)
            {
                SaveCurrentStepData();
                ShowStep(_currentStep - 1);
            }
        }

        private async Task<bool> ValidateCurrentStep()
        {
            switch (_currentStep)
            {
                case WizardStep.FirmaKodu:
                    return await ValidateFirmaKodu();
                case WizardStep.ModulSecimi:
                    return true; // Server'dan alınan bilgiler
                case WizardStep.DosyaYollari:
                    return ValidateDosyaYollari();
                case WizardStep.Tamamlama:
                    return true;
                default:
                    return false;
            }
        }

        private async Task<bool> ValidateFirmaKodu()
        {
            _lblFirmaKoduError.Text = "";
            
            if (string.IsNullOrWhiteSpace(_txtFirmaKodu.Text))
            {
                _lblFirmaKoduError.Text = "Firma kodu boş bırakılamaz.";
                return false;
            }

            // API'den firma bilgilerini al
            _btnNext.Enabled = false;
            _btnNext.Text = "Kontrol ediliyor...";
            
            try
            {
                var firmaSetupBilgi = await _apiHelper.GetFirmaDataOkuSetupBilgiAsync(_txtFirmaKodu.Text);
                
                if (firmaSetupBilgi == null)
                {
                    _lblFirmaKoduError.Text = "Firma kodu bulunamadı veya bağlantı hatası oluştu.";
                    return false;
                }                // Wizard data'yı güncelle
                _wizardData.FirmaKod = firmaSetupBilgi.FirmaKod;
                _wizardData.IsPdks = firmaSetupBilgi.isPdks;
                _wizardData.IsAlarm = firmaSetupBilgi.isAlarm;
                _wizardData.IsKamera = firmaSetupBilgi.isKamera;

                // Eğer şu anda ModulSecimi adımındaysak, modül bilgilerini güncelle
                if (_currentStep == WizardStep.ModulSecimi)
                {
                    UpdateModulDisplay();
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"Firma kodu doğrulama hatası: {ex.Message}");
                _lblFirmaKoduError.Text = "Sunucu bağlantısı kurulamadı. İnternet bağlantınızı kontrol edin.";
                return false;
            }
            finally
            {
                _btnNext.Enabled = true;
                _btnNext.Text = "İleri >";
            }
        }

        private bool ValidateDosyaYollari()
        {
            if (_wizardData.IsPdks && string.IsNullOrWhiteSpace(_txtPdksYol?.Text))
            {
                MessageBox.Show("PDKS kayıt dosya yolu boş bırakılamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtPdksYol?.Focus();
                return false;
            }

            if (_wizardData.IsAlarm && string.IsNullOrWhiteSpace(_txtAlarmYol?.Text))
            {
                MessageBox.Show("Alarm kayıt dosya yolu boş bırakılamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtAlarmYol?.Focus();
                return false;
            }

            if (_wizardData.IsKamera && string.IsNullOrWhiteSpace(_txtKameraYol?.Text))
            {
                MessageBox.Show("Kamera log dosya yolu boş bırakılamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtKameraYol?.Focus();
                return false;
            }

            return true;
        }

        private void SaveCurrentStepData()
        {
            switch (_currentStep)
            {
                case WizardStep.FirmaKodu:
                    _wizardData.FirmaKod = _txtFirmaKodu?.Text ?? "";
                    break;
                case WizardStep.DosyaYollari:
                    if (_wizardData.IsPdks) _wizardData.PdksKayitDosyaYolu = _txtPdksYol?.Text ?? "";
                    if (_wizardData.IsAlarm) _wizardData.AlarmKayitDosyaYolu = _txtAlarmYol?.Text ?? "";
                    if (_wizardData.IsKamera) _wizardData.KameraLogDosyaYolu = _txtKameraYol?.Text ?? "";
                    break;
                case WizardStep.Tamamlama:
                    _wizardData.StartWithWindows = _chkStartWithWindows?.Checked ?? true;
                    _wizardData.CreateDesktopShortcut = _chkCreateDesktopShortcut?.Checked ?? true;
                    break;
            }
        }

        private async Task CompleteSetup()
        {
            try
            {
                // Ayarları kaydet
                var settings = new AppSettings
                {
                    FirmaKod = _wizardData.FirmaKod,
                    PdksKayitDosyaYolu = _wizardData.PdksKayitDosyaYolu,
                    AlarmKayitDosyaYolu = _wizardData.AlarmKayitDosyaYolu,
                    KameraLogDosyaYolu = _wizardData.KameraLogDosyaYolu
                };
                
                _configHelper.SaveSettings(settings);
                
                // Başlangıç seçeneklerini ayarla
                var currentDir = AppContext.BaseDirectory;
                if (_wizardData.StartWithWindows)
                {
                    _startupHelper.AddToStartup(currentDir);
                }
                
                if (_wizardData.CreateDesktopShortcut)
                {
                    _startupHelper.CreateDesktopShortcut(currentDir);
                }
                
                _logger.Info("Wizard kurulumu başarıyla tamamlandı.");
                
                MessageBox.Show("Kurulum başarıyla tamamlandı! Uygulama şimdi çalışmaya başlayacak.", 
                    "Kurulum Tamamlandı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                _logger.Error($"Kurulum tamamlama hatası: {ex.Message}");
                MessageBox.Show($"Kurulum tamamlanırken hata oluştu: {ex.Message}", "Hata", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void UpdateModulDisplay()
        {
            if (_chkPdks != null && _chkAlarm != null && _chkKamera != null && _lblModulInfo != null)
            {
                _chkPdks.Checked = _wizardData.IsPdks;
                _chkAlarm.Checked = _wizardData.IsAlarm;
                _chkKamera.Checked = _wizardData.IsKamera;
                
                var activeModules = new List<string>();
                if (_wizardData.IsPdks) activeModules.Add("PDKS");
                if (_wizardData.IsAlarm) activeModules.Add("Alarm");
                if (_wizardData.IsKamera) activeModules.Add("Kamera");
                
                if (activeModules.Count > 0)
                {
                    _lblModulInfo.Text = $"Firmanız için aktif modüller: {string.Join(", ", activeModules)}";
                    _lblModulInfo.ForeColor = Color.Green;
                }
                else
                {
                    _lblModulInfo.Text = "Bu firma için hiç aktif modül bulunamadı.";
                    _lblModulInfo.ForeColor = Color.Red;
                }
            }
        }
    }
}
