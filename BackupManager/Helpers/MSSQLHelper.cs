using BackupManager.Classes;
using BackupManager.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace BackupManager.Helpers
{
    public class MSSQLHelper
    {

        /// <summary>
        /// Default Constructor for saving or retrieving configuration
        /// </summary>
        public MSSQLHelper()
        {

        }

        /// <summary>
        /// Constructor for calling ExceuteCommand Method
        /// </summary>
        /// <param name="_serverName"></param>
        /// <param name="_databaseName"></param>
        /// <param name="_username"></param>
        /// <param name="_password"></param>
        public MSSQLHelper(string _serverName, string _databaseName, string _username, string _password)
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

        public List<MSSQLBackup> GetConfigList()
        {
            try
            {
                string configFile = Path.Combine(EnVar.AppWorkingPath, "mssql.config");
                if (!File.Exists(configFile)) return null;

                string fileText = File.ReadAllText(configFile);
                List<MSSQLBackup> data = JsonConvert.DeserializeObject<List<MSSQLBackup>>(fileText);
                return data;

            }
            catch (Exception ex)
            {
                LogHelper.LogMessage("error", "Unable to load MS SQL Settings | " + Functions.GetErrorFromException(ex));
                return null;
            }
        }

        public bool AddConfig(MSSQLBackup backup)
        {
            try
            {
                backup.Id = Functions.RandomString(20);
                List<MSSQLBackup> existingList = GetConfigList() ?? new List<MSSQLBackup>();
                existingList.Add(backup);
                string configFile = Path.Combine(EnVar.AppWorkingPath, "mssql.config");
                File.WriteAllText(configFile, JsonConvert.SerializeObject(existingList));
                return true;
            }
            catch //(Exception ex)
            {
                return false;
            }
        }

        public MSSQLBackup GetConfig(string Id)
        {
            try
            {
                List<MSSQLBackup> existingList = GetConfigList() ?? new List<MSSQLBackup>();
                foreach (MSSQLBackup backup in existingList)
                {
                    if (backup.Id == Id) return backup;
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool DeleteConfig(string Id)
        {
            try
            {

                List<MSSQLBackup> existingList = GetConfigList() ?? new List<MSSQLBackup>();
                foreach (MSSQLBackup backup in existingList)
                {
                    if (backup.Id.ToUpper() == Id.ToUpper())
                    {
                        existingList.Remove(backup);
                        break;
                    }
                }

                string configFile = Path.Combine(EnVar.AppWorkingPath, "mssql.config");
                File.WriteAllText(configFile, JsonConvert.SerializeObject(existingList));

                return true;
            }
            catch //(Exception ex)
            {
                return false;
            }

        }

        public async Task<bool> ExceuteCommand(string commandText, SqlParameter[] parameters)
        {

            try
            {
                using (SqlConnection dtCon = new SqlConnection())
                {
                    //Integrated security (without username & password)
                    if (string.IsNullOrWhiteSpace(Username))
                        dtCon.ConnectionString = @"Data Source=" + ServerName + ";Database=" + DatabaseName + ";Trusted_Connection=True;";
                    else
                        dtCon.ConnectionString = @"Data Source=" + ServerName + ";Database=" + DatabaseName + ";User Id=" + Username + ";Password=" + Password + ";";

                    dtCon.Open();

                    if (!string.IsNullOrWhiteSpace(commandText))
                    {
                        SqlCommand sqlCommand = new SqlCommand(commandText, dtCon);
                        foreach(SqlParameter parameter in parameters)
                        {
                            sqlCommand.Parameters.Add(parameter);
                        }
                        sqlCommand.CommandTimeout = 600;    //10 mins
                        await sqlCommand.ExecuteNonQueryAsync();
                    }

                    //-- TODO: check if user has backup rights

                    dtCon.Close();
                }

                Message = "Command successfully executed";
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
