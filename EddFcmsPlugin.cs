using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuickJSON;

namespace EddFcmsPlugin
{
    public class EDDClass
    {
        private string fcmsEmailAddress { get; set; }

        private string fcmsApiKey { get; set; }

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
            JObject js = JObject.Parse(istr);
            fcmsEmailAddress = js != null ? js["fcmsEmailAddress"].Str() : "";
            fcmsApiKey = js != null ? js["fcmsApiKey"].Str() : "";

            if (editit)
            {
                //istr = Prompt.ShowDialog("Data:", "Message box for config", istr);
                ConfigPanel prompt = new ConfigPanel(fcmsEmailAddress, fcmsApiKey);
                prompt.ShowDialog();
                fcmsEmailAddress = prompt.FcmsEmailAddress;
                fcmsApiKey = prompt.FcmsApiKey;
            }

            JObject jout = new JObject();
            jout["fcmsEmailAddress"] = fcmsEmailAddress;
            jout["fcmsApiKey"] = fcmsApiKey;
            string outconfig = jout.ToString();

            System.Diagnostics.Debug.WriteLine("EddFcmsPlugin EDD Config Event:" + outconfig);
            System.IO.File.AppendAllText(@"c:\code\EddFcmsPlugin.txt", "Config " + outconfig + Environment.NewLine);
            return outconfig;
        }
    }
}
