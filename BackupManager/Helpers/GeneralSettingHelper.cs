using BackupManager.Classes;
using BackupManager.Model;
using Newtonsoft.Json;
using System;
using System.IO;

namespace BackupManager.Helpers
{
    public class GeneralSettingHelper
    {

        /// <summary>
        /// Returns config if saved
        /// </summary>
        /// <returns></returns>
        public GeneralSetting GetConfig()
        {
            try
            {
                string configFile = Path.Combine(EnVar.AppWorkingPath, "general.config");
                if (!File.Exists(configFile)) return null;

                string fileText = File.ReadAllText(configFile);
                GeneralSetting setting = JsonConvert.DeserializeObject<GeneralSetting>(fileText);
                return setting;

            }
            catch (Exception ex)
            {
                LogHelper.LogMessage("error", "Unable to load General Settings | " + Functions.GetErrorFromException(ex));
                return null;
            }
        }

        /// <summary>
        /// Saves Configuration on File
        /// </summary>
        public bool SaveConfig(GeneralSetting setting)
        {
            try
            {
                string configFile = Path.Combine(EnVar.AppWorkingPath, "general.config");
                File.WriteAllText(configFile, JsonConvert.SerializeObject(setting));
                return true;
            }
            catch //(Exception ex)
            {
                return false;
            }
        }

    }
}
