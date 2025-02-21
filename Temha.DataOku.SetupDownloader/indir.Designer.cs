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
            label1 = new Label();
            label2 = new Label();
            btn_kontrolEt = new Button();
            label3 = new Label();
            tx_firmaKod = new TextBox();
            SuspendLayout();
            // 
            // progressBar
            // 
            progressBar.Location = new Point(42, 299);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(687, 29);
            progressBar.TabIndex = 0;
            // 
            // lblStatus
            // 
            lblStatus.Font = new Font("Segoe UI", 15F);
            lblStatus.Location = new Point(42, 331);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(687, 54);
            lblStatus.TabIndex = 1;
            lblStatus.Text = "ilerleme durumu..";
            lblStatus.TextAlign = ContentAlignment.TopCenter;
            lblStatus.Click += lblStatus_Click;
            // 
            // label1
            // 
            label1.Font = new Font("Segoe UI", 15F);
            label1.Location = new Point(42, 53);
            label1.Name = "label1";
            label1.Size = new Size(687, 76);
            label1.TabIndex = 3;
            label1.Text = "Lore PDKS İzleme programı kurulacak.Bu adımda en güncel versiyon indirilecek ve sisteminize kurulum başlatılacaktır.";
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
            label2.Text = "Devam etmek için Firma Konuzu Yazınız ve Başlat tuşuna basınız.";
            label2.TextAlign = ContentAlignment.TopCenter;
            // 
            // btn_kontrolEt
            // 
            btn_kontrolEt.Font = new Font("Segoe UI", 15F);
            btn_kontrolEt.Location = new Point(334, 243);
            btn_kontrolEt.Name = "btn_kontrolEt";
            btn_kontrolEt.Size = new Size(106, 38);
            btn_kontrolEt.TabIndex = 5;
            btn_kontrolEt.Text = "Başlat";
            btn_kontrolEt.UseVisualStyleBackColor = true;
            btn_kontrolEt.Click += btn_kontrolEt_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 15F);
            label3.Location = new Point(54, 175);
            label3.Name = "label3";
            label3.Size = new Size(153, 28);
            label3.TabIndex = 6;
            label3.Text = "Firma Kodunuz :";
            // 
            // tx_firmaKod
            // 
            tx_firmaKod.Font = new Font("Segoe UI", 15F);
            tx_firmaKod.Location = new Point(213, 173);
            tx_firmaKod.Name = "tx_firmaKod";
            tx_firmaKod.Size = new Size(516, 34);
            tx_firmaKod.TabIndex = 7;
            // 
            // indir
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(tx_firmaKod);
            Controls.Add(label3);
            Controls.Add(btn_kontrolEt);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(lblStatus);
            Controls.Add(progressBar);
            Name = "indir";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "indir";
            Load += indir_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ProgressBar progressBar;
        private Label lblStatus;
        private Label label1;
        private Label label2;
        private Button btn_kontrolEt;
        private Label label3;
        private TextBox tx_firmaKod;
    }
}