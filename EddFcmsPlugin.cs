using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EddFcmsPlugin
{
    public class EDDClass
    {
        public EDDClass()
        {
            System.Diagnostics.Debug.WriteLine("EddFcmsPlugin Made DLL instance");
            //var x = new Class1();
        }

        EDDDLLInterfaces.EDDDLLIF.EDDCallBacks callbacks;

        public string EDDInitialise(string vstr, string dllfolder, EDDDLLInterfaces.EDDDLLIF.EDDCallBacks cb)
        {
            System.Diagnostics.Debug.WriteLine("EddFcmsPlugin Init func " + vstr + " " + dllfolder);
            System.IO.File.AppendAllText(@"c:\code\EddFcmsPlugin.txt", Environment.NewLine + "Init " + vstr + " in " + dllfolder + Environment.NewLine);
            callbacks = cb;
            return "1.0.0.0";
        }

        public void EDDTerminate()
        {
            System.Diagnostics.Debug.WriteLine("EddFcmsPlugin Unload");
        }

        public void EDDRefresh(string cmdname, EDDDLLInterfaces.EDDDLLIF.JournalEntry lastje)
        {
            System.Diagnostics.Debug.WriteLine("EddFcmsPlugin Refresh");
        }
        public void EDDMainFormShown()
        {
            System.Diagnostics.Debug.WriteLine("EddFcmsPlugin Main form shown");
        }

        public void EDDNewJournalEntry(EDDDLLInterfaces.EDDDLLIF.JournalEntry je)
        {
            System.Diagnostics.Debug.WriteLine("EddFcmsPlugin New Journal Entry " + je.utctime);
            System.IO.File.AppendAllText(@"c:\code\EddFcmsPlugin.txt", "NJE " + je.json + Environment.NewLine);
        }

        public void EDDNewUnfilteredJournalEntry(EDDDLLInterfaces.EDDDLLIF.JournalEntry je)
        {
            System.Diagnostics.Debug.WriteLine("EddFcmsPlugin New Unfiltered Journal Entry " + je.utctime);
            System.IO.File.AppendAllText(@"c:\code\EddFcmsPlugin.txt", "NJE " + je.json + Environment.NewLine);
        }

        public string EDDActionCommand(string cmdname, string[] paras)
        {
            System.Diagnostics.Debug.WriteLine("EddFcmsPlugin EDD Action Command");
            return "";
        }

        public void EDDActionJournalEntry(EDDDLLInterfaces.EDDDLLIF.JournalEntry je)
        {
            System.Diagnostics.Debug.WriteLine("EddFcmsPlugin EDD Action journal entry");
        }

        public void EDDNewUIEvent(string json)
        {
            System.Diagnostics.Debug.WriteLine("EddFcmsPlugin EDD UI Event" + json);
            System.IO.File.AppendAllText(@"c:\code\EddFcmsPlugin.txt", "UI " + json + Environment.NewLine);
        }

        public string EDDConfig(string istr, bool editit)
        {
            if (editit)
            {
                //istr = Prompt.ShowDialog("Data:", "Message box for config", istr);
                Form prompt = new ConfigPanel(istr, istr);
                prompt.ShowDialog();
            }

            System.Diagnostics.Debug.WriteLine("EddFcmsPlugin EDD Config Event:" + istr);
            return istr;
        }

        public static class Prompt
        {
            public static string ShowDialog(string labeltext, string caption, string data)
            {
                Form prompt = new Form()
                {
                    Width = 500,
                    Height = 300,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    Text = caption,
                    StartPosition = FormStartPosition.CenterScreen
                };
                Label fcmsEmailLabel = new Label() { Left = 50, Top = 20, Text = "FCMS Email" };
                TextBox fcmsEmail = new TextBox() { Left = 50, Top = 50, Width = 400, Text = data };
                Label fcmsApiKeyLabel = new Label() { Left = 50, Top = 70, Text = "FCMS API Key" };
                TextBox fcmsApiKey = new TextBox() { Left = 50, Top = 100, Width = 400, Text = data };
                Button confirmation = new Button() { Text = "OK", Left = 350, Width = 100, Top = 150, DialogResult = DialogResult.OK };
                confirmation.Click += (sender, e) => { prompt.Close(); };
                prompt.Controls.Add(fcmsEmailLabel);
                prompt.Controls.Add(fcmsEmail);
                prompt.Controls.Add(fcmsApiKeyLabel);
                prompt.Controls.Add(fcmsApiKey);
                prompt.Controls.Add(confirmation);
                prompt.AcceptButton = confirmation;

                return prompt.ShowDialog() == DialogResult.OK ? fcmsEmail.Text : "";
            }
        }

    }
}
