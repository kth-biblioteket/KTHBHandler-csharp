using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System;
using System.Data;

namespace KTHBHandler
{
    /*******************
    Klass för databashantering
 
    *******************/
    class DBConnect
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;
        EventLog myLog = new EventLog();
        public DBConnect()
        {
            Initialize();
        }
        
        private void Initialize()
        {
            myLog.Source = "KTHB Handler";
            server = "apps.lib.kth.se";
            database = "tasks";
            uid = "tasks";
            password = "xxxxxxxxxxxx";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";
            connection = new MySqlConnection(connectionString);
        }
        
        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        myLog.WriteEntry("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        myLog.WriteEntry("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }

        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                myLog.WriteEntry(ex.ToString());
                return false;
            }
        }

        public string GetSystemSetting(string name)
        {
            string result = "";
            if (this.OpenConnection() == true)
            {
                string query = @"SELECT *
                                 FROM systemsettings
                                 WHERE name = '" + name + "'";
                
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    result = dataReader["value"].ToString();
                }
                dataReader.Close();
                this.CloseConnection();
                return result;
            }
            else
            {
                return result;
            }
        }
    }
}
