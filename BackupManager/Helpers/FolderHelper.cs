using BackupManager.Classes;
using BackupManager.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace BackupManager.Helpers
{
    public class FolderHelper
    {

        protected readonly string configFileName = "folder.config";

        public List<FolderBackup> GetConfigList()
        {
            try
            {
                string configFile = Path.Combine(EnVar.AppWorkingPath, configFileName);
                if (!File.Exists(configFile)) return null;

                string fileText = File.ReadAllText(configFile);
                List<FolderBackup> data = JsonConvert.DeserializeObject<List<FolderBackup>>(fileText);
                return data;

            }
            catch //(Exception ex)
            {
                return null;
            }
        }

        public bool AddConfig(FolderBackup backup)
        {
            try
            {
                List<FolderBackup> existingList = GetConfigList() ?? new List<FolderBackup>();
                existingList.Add(backup);
                string configFile = Path.Combine(EnVar.AppWorkingPath, configFileName);
                File.WriteAllText(configFile, JsonConvert.SerializeObject(existingList));
                return true;
            }
            catch //(Exception ex)
            {
                return false;
            }
        }

        public FolderBackup GetConfig(string Id)
        {
            try
            {
                List<FolderBackup> existingList = GetConfigList() ?? new List<FolderBackup>();
                foreach (FolderBackup backup in existingList)
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

                List<FolderBackup> existingList = GetConfigList() ?? new List<FolderBackup>();
                foreach (FolderBackup backup in existingList)
                {
                    if (backup.Id.ToUpper() == Id)
                    {
                        existingList.Remove(backup);
                        break;
                    }
                }

                string configFile = Path.Combine(EnVar.AppWorkingPath, configFileName);
                File.WriteAllText(configFile, JsonConvert.SerializeObject(existingList));

                return true;
            }
            catch //(Exception ex)
            {
                return false;
            }

        }

    }
}
