using BackupManager.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace BackupManager.Helpers
{
    public class LogHelper
    {

        /// <summary>
        /// Returns recent 100 logs
        /// </summary>
        /// <returns></returns>
        public static List<string> GetLogs()
        {
            try
            {

                string logFileName = DateTime.Today.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo) + ".txt";
                string tmpPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                EnVar.AppWorkingPath = Path.Combine(tmpPath, "Runtime Backup Manager");
                string logFilePath = Path.Combine(EnVar.AppWorkingPath, logFileName);

                List<string> output = new List<string>();

                if (File.Exists(logFilePath))
                {
                    int lineNo = 0;
                    using (StreamReader reader = new StreamReader(logFilePath))
                    {
                        while (lineNo < 100 && !reader.EndOfStream)
                        {
                            output.Add(reader.ReadLine());
                            lineNo++;
                        }
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

            string logFileName = DateTime.Today.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo) + ".txt";
            string tmpPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            EnVar.AppWorkingPath = Path.Combine(tmpPath, "Runtime Backup Manager");
            string logFilePath = Path.Combine(EnVar.AppWorkingPath, logFileName);

            try
            {
                string lineToWrite = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", DateTimeFormatInfo.InvariantInfo) +
                                     " | " + logType + " | " + logDescription;

                //append at end of log file already exist
                using (StreamWriter writer = File.AppendText(logFilePath))
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
