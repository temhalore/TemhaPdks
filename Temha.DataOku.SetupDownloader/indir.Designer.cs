namespace Temha.DataOku.SetupDownloader
{
    partial class indir
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            progressBar = new ProgressBar();
            lblStatus = new Label();
            btn_download = new Button();
            label1 = new Label();
            label2 = new Label();
            SuspendLayout();
            // 
            // progressBar
            // 
            progressBar.Location = new Point(42, 225);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(687, 29);
            progressBar.TabIndex = 0;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(335, 272);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(101, 15);
            lblStatus.TabIndex = 1;
            lblStatus.Text = "ilerleme durumu..";
            lblStatus.Click += lblStatus_Click;
            // 
            // btn_download
            // 
            btn_download.Font = new Font("Segoe UI", 15F);
            btn_download.Location = new Point(337, 177);
            btn_download.Name = "btn_download";
            btn_download.Size = new Size(99, 42);
            btn_download.TabIndex = 2;
            btn_download.Text = "Başlat";
            btn_download.UseVisualStyleBackColor = true;
            btn_download.Click += button1_Click;
            // 
            // label1
            // 
            label1.Font = new Font("Segoe UI", 15F);
            label1.Location = new Point(42, 53);
            label1.Name = "label1";
            label1.Size = new Size(687, 76);
            label1.TabIndex = 3;
            label1.Text = "Pdks İzleme programı kurulacak.Bu adoımda en güncel versiyon indirilecek ve sisteminize kurulum başlatılacaktır.";
            label1.TextAlign = ContentAlignment.TopCenter;
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.Font = new Font("Segoe UI", 15F);
            label2.Location = new Point(42, 129);
            label2.Name = "label2";
            label2.RightToLeft = RightToLeft.No;
            label2.Size = new Size(694, 45);
            label2.TabIndex = 4;
            label2.Text = "Devam etmek için Başlat tuşuna basınız.";
            label2.TextAlign = ContentAlignment.TopCenter;
            // 
            // indir
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(btn_download);
            Controls.Add(lblStatus);
            Controls.Add(progressBar);
            Name = "indir";
            Text = "indir";
            Load += indir_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ProgressBar progressBar;
        private Label lblStatus;
        private Button btn_download;
        private Label label1;
        private Label label2;
    }
}