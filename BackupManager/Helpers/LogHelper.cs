using BackupManager.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace BackupManager.Helpers
{
    public class LogHelper
    {

        protected readonly static string configFileName = "log.txt";

        /// <summary>
        /// Returns recent 100 logs
        /// </summary>
        /// <returns></returns>
        public static List<string> GetLogs()
        {
            try
            {
                string configFile = Path.Combine(EnVar.AppWorkingPath, configFileName);

                //-- TODO
                //This is huge performance penalty as entire file will be read in memory
                //We need to split files by date and allow log view for one date only
                int lineNo = 0;
                List<string> output = new List<string>();
                using(StreamReader reader = new StreamReader(configFile))
                {
                    while (lineNo < 100 || reader.EndOfStream)
                    {
                        output.Add(reader.ReadLine());
                        lineNo++;
                    }
                }

                return output;

            }
            catch //(Exception ex)
            {
                return null;
            }
        }

        public static void LogMessage(string logType, string logDescription)
        {
            try
            {
                string configFile = Path.Combine(EnVar.AppWorkingPath, configFileName);

                string lineToWrite = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", DateTimeFormatInfo.InvariantInfo) +
                                     " | " + logType + " | " + logDescription;

                //append at end of log file already exist
                using (StreamWriter writer = File.AppendText(configFile))
                {
                    writer.WriteLine(lineToWrite);
                }
            }
            catch //(Exception ex)
            {
                //-- TODO log to text file to address failed logging
            }
        }

    }
}
