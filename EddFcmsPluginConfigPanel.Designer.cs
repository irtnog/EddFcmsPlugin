
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
            this.FcmsEmailAddressInput = new System.Windows.Forms.TextBox();
            this.FcmsApiKeyInput = new System.Windows.Forms.TextBox();
            this.PictureBox1 = new System.Windows.Forms.PictureBox();
            this.FcmsEmailAddressInputLabel = new System.Windows.Forms.Label();
            this.FcmsApiKeyInputLabel = new System.Windows.Forms.Label();
            this.FcmsWebsiteLink = new System.Windows.Forms.LinkLabel();
            this.OkButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // FcmsEmailAddressInput
            // 
            this.FcmsEmailAddressInput.Location = new System.Drawing.Point(309, 141);
            this.FcmsEmailAddressInput.Name = "FcmsEmailAddressInput";
            this.FcmsEmailAddressInput.Size = new System.Drawing.Size(256, 20);
            this.FcmsEmailAddressInput.TabIndex = 0;
            // 
            // FcmsApiKeyInput
            // 
            this.FcmsApiKeyInput.Location = new System.Drawing.Point(309, 230);
            this.FcmsApiKeyInput.Name = "FcmsApiKeyInput";
            this.FcmsApiKeyInput.Size = new System.Drawing.Size(256, 20);
            this.FcmsApiKeyInput.TabIndex = 1;
            this.FcmsApiKeyInput.UseSystemPasswordChar = true;
            // 
            // PictureBox1
            // 
            this.PictureBox1.Image = global::EddFcmsPlugin.Properties.Resources.config_panel_background;
            this.PictureBox1.Location = new System.Drawing.Point(-1, -1);
            this.PictureBox1.Name = "PictureBox1";
            this.PictureBox1.Size = new System.Drawing.Size(252, 444);
            this.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PictureBox1.TabIndex = 1;
            this.PictureBox1.TabStop = false;
            // 
            // FcmsEmailAddressInputLabel
            // 
            this.FcmsEmailAddressInputLabel.AutoSize = true;
            this.FcmsEmailAddressInputLabel.Location = new System.Drawing.Point(306, 125);
            this.FcmsEmailAddressInputLabel.Name = "FcmsEmailAddressInputLabel";
            this.FcmsEmailAddressInputLabel.Size = new System.Drawing.Size(103, 13);
            this.FcmsEmailAddressInputLabel.TabIndex = 2;
            this.FcmsEmailAddressInputLabel.Text = "FCMS email address";
            // 
            // FcmsApiKeyInputLabel
            // 
            this.FcmsApiKeyInputLabel.AutoSize = true;
            this.FcmsApiKeyInputLabel.Location = new System.Drawing.Point(306, 214);
            this.FcmsApiKeyInputLabel.Name = "FcmsApiKeyInputLabel";
            this.FcmsApiKeyInputLabel.Size = new System.Drawing.Size(76, 13);
            this.FcmsApiKeyInputLabel.TabIndex = 2;
            this.FcmsApiKeyInputLabel.Text = "FCMS API key";
            // 
            // FcmsWebsiteLink
            // 
            this.FcmsWebsiteLink.AutoSize = true;
            this.FcmsWebsiteLink.Location = new System.Drawing.Point(483, 419);
            this.FcmsWebsiteLink.Name = "FcmsWebsiteLink";
            this.FcmsWebsiteLink.Size = new System.Drawing.Size(129, 13);
            this.FcmsWebsiteLink.TabIndex = 3;
            this.FcmsWebsiteLink.TabStop = true;
            this.FcmsWebsiteLink.Text = "https://fleetcarrier.space/";
            // 
            // OkButton
            // 
            this.OkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OkButton.Location = new System.Drawing.Point(490, 319);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(75, 23);
            this.OkButton.TabIndex = 2;
            this.OkButton.Text = "OK";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // ConfigPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(624, 441);
            this.Controls.Add(this.OkButton);
            this.Controls.Add(this.FcmsWebsiteLink);
            this.Controls.Add(this.FcmsApiKeyInputLabel);
            this.Controls.Add(this.FcmsEmailAddressInputLabel);
            this.Controls.Add(this.PictureBox1);
            this.Controls.Add(this.FcmsApiKeyInput);
            this.Controls.Add(this.FcmsEmailAddressInput);
            this.Name = "ConfigPanel";
            this.Text = "Fleet Carrier Management System";
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox PictureBox1;
        private System.Windows.Forms.Label FcmsEmailAddressInputLabel;
        private System.Windows.Forms.Label FcmsApiKeyInputLabel;
        private System.Windows.Forms.LinkLabel FcmsWebsiteLink;
        private System.Windows.Forms.Button OkButton;
        public System.Windows.Forms.TextBox FcmsEmailAddressInput;
        public System.Windows.Forms.TextBox FcmsApiKeyInput;
    }
}