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

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FcmsWebsiteLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // cf. https://docs.microsoft.com/en-us/dotnet/desktop/winforms/controls/link-to-an-object-or-web-page-with-wf-linklabel-control?view=netframeworkdesktop-4.8
            try
            {
                VisitFcmsWebsiteLink();
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to open the link that was clicked.");
            }
        }

        private void VisitFcmsWebsiteLink()
        {
            // change color of link text
            FcmsWebsiteLink.LinkVisited = true;

            // open the default browser with the URL
            System.Diagnostics.Process.Start(FcmsWebsiteLink.Text);
        }
    }
}
