using BackupManager.Classes;
using BackupManager.Model;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BackupManager.Helpers
{
    public class MYSQLHelper
    {
        /// <summary>
        /// Default Constructor for saving or retrieving configuration
        /// </summary>
        public MYSQLHelper()
        {

        }

        /// <summary>
        /// Constructor for calling BackupDatabase Method
        /// </summary>
        /// <param name="_serverName"></param>
        /// <param name="_databaseName"></param>
        /// <param name="_username"></param>
        /// <param name="_password"></param>
        public MYSQLHelper(string _serverName, string _databaseName, string _username, string _password)
        {
            ServerName = _serverName;
            DatabaseName = _databaseName;
            Username = _username;
            Password = _password;
        }

        readonly string ServerName;
        readonly string DatabaseName;
        readonly string Username;
        readonly string Password;

        public string Message = "";

        public List<MYSQLBackup> GetConfigList()
        {
            try
            {
                string configFile = Path.Combine(EnVar.AppWorkingPath, "mysql.config");
                if (!File.Exists(configFile)) return null;

                string fileText = File.ReadAllText(configFile);
                List<MYSQLBackup> data = JsonConvert.DeserializeObject<List<MYSQLBackup>>(fileText);
                return data;

            }
            catch (Exception ex)
            {
                LogHelper.LogMessage("error", "Unable to load My SQL Settings | " + Functions.GetErrorFromException(ex));
                return null;
            }
        }

        public bool AddConfig(MYSQLBackup backup)
        {
            try
            {
                backup.Id = Functions.RandomString(20);
                List<MYSQLBackup> existingList = GetConfigList() ?? new List<MYSQLBackup>();
                existingList.Add(backup);
                string configFile = Path.Combine(EnVar.AppWorkingPath, "mysql.config");
                File.WriteAllText(configFile, JsonConvert.SerializeObject(existingList));
                return true;
            }
            catch //(Exception ex)
            {
                return false;
            }
        }

        public MYSQLBackup GetConfig(string Id)
        {
            try
            {
                List<MYSQLBackup> existingList = GetConfigList() ?? new List<MYSQLBackup>();
                foreach (MYSQLBackup backup in existingList)
                {
                    if (backup.Id == Id) return backup;
                }

                return null;
            }
            catch //(Exception ex)
            {
                return null;
            }
        }

        public bool DeleteConfig(string Id)
        {
            try
            {

                List<MYSQLBackup> existingList = GetConfigList() ?? new List<MYSQLBackup>();
                foreach (MYSQLBackup backup in existingList)
                {
                    if (backup.Id.ToUpper() == Id)
                    {
                        existingList.Remove(backup);
                        break;
                    }
                }

                string configFile = Path.Combine(EnVar.AppWorkingPath, "mysql.config");
                File.WriteAllText(configFile, JsonConvert.SerializeObject(existingList));

                return true;
            }
            catch //(Exception ex)
            {
                return false;
            }

        }

        public async Task<bool> BackupDatabase(string dumpFile)
        {

            try
            {
                string constring = "server=" + ServerName + ";database=" + DatabaseName +";user=" + Username + ";pwd=" + Password + ";convert zero datetime=True";
                using (MySqlConnection conn = new MySqlConnection(constring))
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        using (MySqlBackup mb = new MySqlBackup(cmd))
                        {
                            cmd.Connection = conn;
                            conn.Open();
                            await Task.Run(() => mb.ExportToFile(dumpFile));
                            conn.Close();
                        }
                    }
                }

                Message = "Success";
                return true;
            }
            catch (Exception ex)
            {
                Message = Functions.GetErrorFromException(ex);
                return false;
            }

        }

    }
}
