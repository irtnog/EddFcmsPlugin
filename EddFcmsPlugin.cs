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
        private string CmdrName { get; set; }
        
        private JObject FcmsCredentials { get; set; }

        private static readonly HttpClient FcmsClient = new HttpClient();

        public EDDClass()
        {
            System.Diagnostics.Debug.WriteLine("EddFcmsPlugin Made DLL instance");
        }

        EDDDLLInterfaces.EDDDLLIF.EDDCallBacks callbacks;

        public string EDDInitialise(string vstr, string dllfolder, EDDDLLInterfaces.EDDDLLIF.EDDCallBacks cb)
        {
            System.Diagnostics.Debug.WriteLine("EddFcmsPlugin Init func " + vstr + " " + dllfolder);

            CmdrName = null;
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

            CmdrName = cmdname;
        }
        public void EDDMainFormShown()
        {
            System.Diagnostics.Debug.WriteLine("EddFcmsPlugin Main form shown");
        }

        void PostCarrierJumpAction(EDDDLLInterfaces.EDDDLLIF.JournalEntry je)
        {
            if ((FcmsCredentials[je.cmdrname] == null) ||
                (FcmsCredentials[je.cmdrname]["FcmsEmailAddress"].Str() == "") ||
                (FcmsCredentials[je.cmdrname]["FcmsApiKey"].Str() == ""))
            {
                System.Diagnostics.Debug.WriteLine("EddFcmsPlugin skipping PostCarrierJumpAction due to missing credentials");
                return;
            }
            JObject js = new JObject();
            js["cmdr"] = je.cmdrname;
            js["system"] = je.systemname;
            js["station"] = je.stationname;
            js["data"] = JObject.Parse(je.json);
            js["is_beta"] = je.beta;
            js["user"] = FcmsCredentials[je.cmdrname]["FcmsEmailAddress"].Str();
            js["key"] = FcmsCredentials[je.cmdrname]["FcmsApiKey"].Str();

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
        }

        public string EDDConfig(string istr, bool editit)
        {
            // deserialize the list of FCMS credentials
            FcmsCredentials = JObject.Parse(istr);

            if (editit && CmdrName != null)
            {
                if (FcmsCredentials[CmdrName] == null)
                {
                    // init credentials for this CMDR
                    FcmsCredentials[CmdrName] = new JObject();
                    FcmsCredentials[CmdrName]["FcmsEmailAddress"] = "";
                    FcmsCredentials[CmdrName]["FcmsApiKey"] = "";
                }

                // ask this CMDR for new credentials
                ConfigPanel prompt = new ConfigPanel(FcmsCredentials[CmdrName]["FcmsEmailAddress"].Str(), FcmsCredentials[CmdrName]["FcmsApiKey"].Str());
                prompt.ShowDialog();

                // store them
                FcmsCredentials[CmdrName]["FcmsEmailAddress"] = prompt.FcmsEmailAddress;
                FcmsCredentials[CmdrName]["FcmsApiKey"] = prompt.FcmsApiKey;
            }
            else if (editit && CmdrName == null)
            {
                MessageBox.Show("Please wait for the history refresh to finish and try again.");
            }

            // serialize the list of FCMS credentials
            string outconfig = FcmsCredentials.ToString();

            System.Diagnostics.Debug.WriteLine("EddFcmsPlugin EDD Config Event:" + outconfig);
            return outconfig;
        }
    }
}
