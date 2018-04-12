using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System;
using System.Data;
using System.Configuration;

namespace KTHBHandler
{
    /*******************
     * 
     * Klass för databashantering
     * 
     * *******************/
    class DBConnect
    {
        private MySqlConnection connection;
        EventLog myLog = new EventLog();
        public DBConnect()
        {
            Initialize();
        }
        
        private void Initialize()
        {
            var config= ConfigurationManager.ConnectionStrings["DefaultConnection"];
            string connString = config.ConnectionString;

            myLog.Source = "KTHB Handler";
            connection = new MySqlConnection(connString);
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
