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
        public ConfigPanel(string email, string apikey)
        {
            InitializeComponent();

            fcmsEmailAddress.Text = email;
            fcmsApiKey.Text = apikey;
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
