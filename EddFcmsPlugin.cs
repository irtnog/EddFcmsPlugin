using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuickJSON;

namespace EddFcmsPlugin
{
    public class EDDClass
    {
        private string FcmsEmailAddress { get; set; }

        private string FcmsApiKey { get; set; }

        private HttpClient FcmsClient = new HttpClient();

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

        void PostCarrierJumpAction(EDDDLLInterfaces.EDDDLLIF.JournalEntry je)
        {
            JObject js = new JObject();
            // FIXME: replace dummy data
            js["cmdr"] = je.cmdrname;
            js["system"] = je.systemname;
            js["station"] = je.stationname;
            js["data"] = JObject.Parse(je.json);
            js["is_beta"] = je.beta;
            js["user"] = FcmsEmailAddress;
            js["key"] = FcmsApiKey;

            // encode the data and post it to the API endpoint
            StringContent content = new StringContent(js.ToString(), Encoding.UTF8, "application/json");
            try
            {
                HttpResponseMessage response = FcmsClient.PostAsync("https://fleetcarrier.space/api", content).Result;
            }
            catch (Exception)
            {
                // no op
            }
        }

        public void EDDNewJournalEntry(EDDDLLInterfaces.EDDDLLIF.JournalEntry je)
        {
            System.Diagnostics.Debug.WriteLine("EddFcmsPlugin New Journal Entry " + je.utctime);
            System.IO.File.AppendAllText(@"c:\code\EddFcmsPlugin.txt", "NJE " + je.json + Environment.NewLine);

            string type = je.eventid;
            switch (type)
            {
                case "CarrierJumpRequest":
                case "CarrierJumpCancelled":
                    Task.Run(() => PostCarrierJumpAction(je));
                    break;
            }
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
            FcmsEmailAddress = js != null ? js["fcmsEmailAddress"].Str() : "";
            FcmsApiKey = js != null ? js["fcmsApiKey"].Str() : "";

            if (editit)
            {
                //istr = Prompt.ShowDialog("Data:", "Message box for config", istr);
                ConfigPanel prompt = new ConfigPanel(FcmsEmailAddress, FcmsApiKey);
                prompt.ShowDialog();
                FcmsEmailAddress = prompt.FcmsEmailAddress;
                FcmsApiKey = prompt.FcmsApiKey;
            }

            JObject jout = new JObject();
            jout["fcmsEmailAddress"] = FcmsEmailAddress;
            jout["fcmsApiKey"] = FcmsApiKey;
            string outconfig = jout.ToString();

            System.Diagnostics.Debug.WriteLine("EddFcmsPlugin EDD Config Event:" + outconfig);
            System.IO.File.AppendAllText(@"c:\code\EddFcmsPlugin.txt", "Config " + outconfig + Environment.NewLine);
            return outconfig;
        }
    }
}
