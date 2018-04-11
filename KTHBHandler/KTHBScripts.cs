using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.Net.Mail;
using System.Net;

namespace KTHBHandler
{
    /*******************
    Klass som kör interna KTHB-scripts 
    *******************/
    class KTHBScripts
    {
        DBConnect conn = new DBConnect();
        EventLog myLog = new EventLog();

        public KTHBScripts()
        {
            Initialize();
        }

        private void Initialize()
        {
            myLog.Source = "KTHB Handler";
        }

    }
}
