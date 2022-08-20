
namespace EddFcmsPlugin
{
    partial class ConfigPanel
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
            this.fcmsEmailAddress = new System.Windows.Forms.TextBox();
            this.fcmsApiKey = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.fcmsLink = new System.Windows.Forms.LinkLabel();
            this.okButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // fcmsEmailAddress
            // 
            this.fcmsEmailAddress.Location = new System.Drawing.Point(309, 141);
            this.fcmsEmailAddress.Name = "fcmsEmailAddress";
            this.fcmsEmailAddress.Size = new System.Drawing.Size(256, 20);
            this.fcmsEmailAddress.TabIndex = 0;
            // 
            // fcmsApiKey
            // 
            this.fcmsApiKey.Location = new System.Drawing.Point(309, 230);
            this.fcmsApiKey.Name = "fcmsApiKey";
            this.fcmsApiKey.Size = new System.Drawing.Size(256, 20);
            this.fcmsApiKey.TabIndex = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::EddFcmsPlugin.Properties.Resources.config_panel_background;
            this.pictureBox1.Location = new System.Drawing.Point(-1, -1);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(252, 444);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(306, 125);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "FCMS email address";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(306, 214);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "FCMS API key";
            // 
            // fcmsLink
            // 
            this.fcmsLink.AutoSize = true;
            this.fcmsLink.Location = new System.Drawing.Point(483, 419);
            this.fcmsLink.Name = "fcmsLink";
            this.fcmsLink.Size = new System.Drawing.Size(129, 13);
            this.fcmsLink.TabIndex = 3;
            this.fcmsLink.TabStop = true;
            this.fcmsLink.Text = "https://fleetcarrier.space/";
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(490, 319);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // ConfigPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(624, 441);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.fcmsLink);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.fcmsApiKey);
            this.Controls.Add(this.fcmsEmailAddress);
            this.Name = "ConfigPanel";
            this.Text = "Fleet Carrier Management System";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel fcmsLink;
        private System.Windows.Forms.Button okButton;
        public System.Windows.Forms.TextBox fcmsEmailAddress;
        public System.Windows.Forms.TextBox fcmsApiKey;
    }
}