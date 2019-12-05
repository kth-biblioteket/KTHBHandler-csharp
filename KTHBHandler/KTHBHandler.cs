using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;

/****************************************************************
 * 
 * Service som startar PHP-script med givet intervall 
 *
 * Parametrar anges i mysql-databasen tasks, tabell systemsettings
 * 
 * Installera/avinstallera(använd /u) med:
 * c:\windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe [/u] c:\program files (x86)\KTHBHandler\KTHBHAndler.exe
 * 
****************************************************************/

namespace KTHBHandler
{
    public partial class KTHBHandler : ServiceBase
    {
        DBConnect conn = new DBConnect();
        KTHBScripts kthbscripts = new KTHBScripts();
        string systemname = "";
        string systemfolder = "";
        string systemftpfolder = "";
        string appsrootfolder = "";
        int handlerinterval = 0;
        string handlerurl = "";
        string token = "";
        public KTHBHandler()
        {
            InitializeComponent();
            if (!System.Diagnostics.EventLog.SourceExists("KTHB Handler"))
            {
                System.Diagnostics.EventLog.CreateEventSource("KTHB Handler", "KTHB");
            }
            eventLog1.Source = "KTHB Handler";
            eventLog1.Log = "KTHB";
        }

        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("In OnStart");
            systemname = conn.GetSystemSetting("systemname");
            systemfolder = conn.GetSystemSetting("systemfolder");
            systemftpfolder = conn.GetSystemSetting("systemftpfolder");
            appsrootfolder = conn.GetSystemSetting("webrootfolder");
            handlerinterval = Int32.Parse(conn.GetSystemSetting("handlerinterval"));
            handlerurl = conn.GetSystemSetting("handlerurl");
            token = conn.GetSystemSetting("token");
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = handlerinterval; // millisekunder
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();
        }

        protected override void OnStop()
        {
            eventLog1.WriteEntry("In onStop.");
        }

        protected override void OnContinue()
        {
            eventLog1.WriteEntry("In OnContinue.");
        }

        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            try
            {
                //Kör PHP-script
                runWS(handlerurl + "?token=" + token);
            }
            catch (Exception e)
            {
                eventLog1.WriteEntry("Error in OnTimer" + e.Message);
            }
        }

        public async Task runWS(string url)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                //request.Timeout = 30000;
                using (HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));
                    var stream = response.GetResponseStream();
                    var sr = new StreamReader(stream);
                    var content = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                eventLog1.WriteEntry("Error in runWS: " + e.Message);
            }
        }
    }
}
