using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EddFcmsPlugin
{
    public partial class ConfigPanel : Form
    {
        public string FcmsEmailAddress => FcmsEmailAddressInput.Text;

        public string FcmsApiKey => FcmsApiKeyInput.Text;

        public ConfigPanel(string email, string apikey)
        {
            InitializeComponent();

            FcmsEmailAddressInput.Text = email;
            FcmsApiKeyInput.Text = apikey;
        }

        private void linkToFcms_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
