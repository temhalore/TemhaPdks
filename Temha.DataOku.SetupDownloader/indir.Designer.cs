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
            SuspendLayout();
            // 
            // progressBar
            // 
            progressBar.Location = new Point(195, 225);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(411, 29);
            progressBar.TabIndex = 0;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(346, 266);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(100, 15);
            lblStatus.TabIndex = 1;
            lblStatus.Text = "indirme durumu..";
            // 
            // btn_download
            // 
            btn_download.Location = new Point(346, 158);
            btn_download.Name = "btn_download";
            btn_download.Size = new Size(75, 23);
            btn_download.TabIndex = 2;
            btn_download.Text = "Başla";
            btn_download.UseVisualStyleBackColor = true;
            btn_download.Click += button1_Click;
            // 
            // indir
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
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
    }
}