using BackupManager.Classes;
using BackupManager.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Windows;

namespace BackupManager.Helpers
{

    /// <summary>
    /// This class is consumed by BackupService
    /// </summary>
    public class BackupHelper
    {

        private AWSS3Setting awsS3Setting;
        private EmailSetting emailSetting;
        private GeneralSetting generalSetting;

        List<MSSQLBackup> mssqlBackups = new List<MSSQLBackup>();
        List<MYSQLBackup> mysqlBackups = new List<MYSQLBackup>();
        List<FolderBackup> folderBackups = new List<FolderBackup>();

        public async Task ExecuteBackups()
        {
            //Read current configuration
            //As user may have updated settings
            try
            {

                string tmpPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                EnVar.AppWorkingPath = Path.Combine(tmpPath, "Runtime Backup Manager");

                AWSS3Helper awsHelper = new AWSS3Helper();
                awsS3Setting = awsHelper.GetConfig();

                EmailHelper emailHelper = new EmailHelper();
                emailSetting = emailHelper.GetConfig();

                GeneralSettingHelper generalHelper = new GeneralSettingHelper();
                generalSetting = generalHelper.GetConfig();

                MSSQLHelper mssqlHelper = new MSSQLHelper();
                mssqlBackups = mssqlHelper.GetConfigList();

                MYSQLHelper mysqlHelper = new MYSQLHelper();
                mysqlBackups = mysqlHelper.GetConfigList();

                FolderHelper folderHelper = new FolderHelper();
                folderBackups = folderHelper.GetConfigList();

            }
            catch (Exception ex)
            {
                LogHelper.LogMessage("Error", "Unable to load backup settings." + Functions.GetErrorFromException(ex));
                return;
            }

            try
            {
                int Hour = DateTime.Now.Hour;
                int Minute = DateTime.Now.Minute;

                if (generalSetting == null || 
                    string.IsNullOrWhiteSpace(generalSetting.LocalFolder) || 
                    !Directory.Exists(generalSetting.LocalFolder)) return;

                EmailHelper emailHelper = null;
                if (emailSetting != null && emailSetting.IsValid == true)
                {
                    emailHelper = new EmailHelper(emailSetting.DisplayName, emailSetting.EmailAddress, emailSetting.Username, emailSetting.Password, emailSetting.SmtpServer, emailSetting.SmtpPort, emailSetting.SmtpSsl);
                }

                //Backup SQL
                if (mssqlBackups != null && mssqlBackups.Count > 0)
                {
                    foreach (MSSQLBackup backup in mssqlBackups)
                    {
                        if (backup.BackupTime.Hour == Hour && backup.BackupTime.Minute == Minute)
                        {
                            MSSQLHelper helper = new MSSQLHelper(backup.ServerName, backup.DatabaseName, backup.Username, backup.Password);
                            string fileName = Path.Combine(generalSetting.LocalFolder, backup.DatabaseName + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".bak");
                            bool Result = await helper.ExceuteCommand("BACKUP DATABASE " + backup.DatabaseName + " TO DISK=@filename", 
                                                                     new System.Data.SqlClient.SqlParameter[] { new System.Data.SqlClient.SqlParameter { ParameterName = "@filename", Value = fileName} });

                            if (Result == true)
                            {
                                //log message
                                LogHelper.LogMessage("Info", "MSSQL database '" + backup.DatabaseName + "' backed up successfully");
                                
                                //email config
                                if (emailHelper != null && emailSetting.LocalSuccess)
                                {
                                    string body = "Dear Admin, \n\n" +
                                        "This is to notify you that following scheduled task has completed successfully:\n" +
                                        "Backup Type: MSSQL\n" + 
                                        "Database Name: " + backup.DatabaseName + "\n" +
                                        "Backup Location: Local\n" + 
                                        "Backup Time: " + DateTime.Now.ToString("MMM dd, yyyy HH:mm:ss", DateTimeFormatInfo.InvariantInfo) + ".\n\n" +
                                        "Runtime Backup Manager";

                                    bool emailSuccess = await emailHelper.SendEmail(emailSetting.RecipientEmail, "Success: Backup for MSSQL '" + backup.DatabaseName + "'", body);
                                    if (emailSuccess == false)
                                    {
                                        LogHelper.LogMessage("Error", "E-Mail sending failed. " + emailHelper.ResultMessage);
                                    }
                                }
                            }
                            else
                            {
                                //log message
                                LogHelper.LogMessage("Error", "MSSQL backup failed for " + backup.DatabaseName + ". " + helper.Message);

                                //email config
                                if (emailHelper != null && emailSetting.LocalFailure)
                                {
                                    string body = "Dear Admin, \n\n" +
                                        "This is to notify you that following scheduled task has failed:\n" +
                                        "Backup Type: MSSQL\n" +
                                        "Database Name: " + backup.DatabaseName + "\n" +
                                        "Backup Location: Local\n" +
                                        "Backup Time: " + DateTime.Now.ToString("MMM dd, yyyy HH:mm:ss", DateTimeFormatInfo.InvariantInfo) + ".\n\n" +
                                        "Runtime Backup Manager";

                                    bool emailSuccess = await emailHelper.SendEmail(emailSetting.RecipientEmail, "Failure: Backup for MSSQL '" + backup.DatabaseName + "'", body);
                                    if (emailSuccess == false)
                                    {
                                        LogHelper.LogMessage("Error", "E-Mail sending failed. " + emailHelper.ResultMessage);
                                    }
                                }
                            }
                        }
                    }
                }

                //Backup MySQL
                if (mysqlBackups != null && mysqlBackups.Count > 0)
                {
                    foreach (MYSQLBackup backup in mysqlBackups)
                    {
                        if (backup.BackupTime.Hour == Hour && backup.BackupTime.Minute == Minute)
                        {
                            MYSQLHelper helper = new MYSQLHelper(backup.ServerName, backup.DatabaseName, backup.Username, backup.Password);
                            string fileName = Path.Combine(generalSetting.LocalFolder, backup.DatabaseName + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".sqldump");
                            bool Result = await helper.BackupDatabase(fileName);

                            if (Result == true)
                            {
                                //log message
                                LogHelper.LogMessage("Info", "MySQL database '" + backup.DatabaseName + "' backed up successfully");

                                //email config
                                if (emailHelper != null && emailSetting.LocalSuccess)
                                {
                                    string body = "Dear Admin, \n\n" +
                                        "This is to notify you that following scheduled task has completed successfully:\n" +
                                        "Backup Type: MySQL\n" +
                                        "Database Name: " + backup.DatabaseName + "\n" +
                                        "Backup Location: Local\n" +
                                        "Backup Time: " + DateTime.Now.ToString("MMM dd, yyyy HH:mm:ss", DateTimeFormatInfo.InvariantInfo) + ".\n\n" +
                                        "Runtime Backup Manager";

                                    bool emailSuccess = await emailHelper.SendEmail(emailSetting.RecipientEmail, "Success: Backup for MySQL '" + backup.DatabaseName + "'", body);
                                    if (emailSuccess == false)
                                    {
                                        LogHelper.LogMessage("Error", "E-Mail sending failed. " + emailHelper.ResultMessage);
                                    }
                                }
                            }
                            else
                            {
                                //log Message
                                LogHelper.LogMessage("Error", "MySQL backup failed for " + backup.DatabaseName + ". " + helper.Message);

                                //email config
                                if (emailHelper != null && emailSetting.LocalFailure)
                                {
                                    string body = "Dear Admin, \n\n" +
                                        "This is to notify you that following backup task has failed:\n" +
                                        "Backup Type: MySQL\n" +
                                        "Database Name: " + backup.DatabaseName + "\n" +
                                        "Backup Location: Local\n" +
                                        "Backup Time: " + DateTime.Now.ToString("MMM dd, yyyy HH:mm:ss", DateTimeFormatInfo.InvariantInfo) + ".\n\n" +
                                        "Runtime Backup Manager";

                                    bool emailSuccess = await emailHelper.SendEmail(emailSetting.RecipientEmail, "Failure: Backup for MySQL '" + backup.DatabaseName + "'", body);
                                    if (emailSuccess == false)
                                    {
                                        LogHelper.LogMessage("Error", "E-Mail sending failed. " + emailHelper.ResultMessage);
                                    }
                                }
                            }
                        }
                    }
                }

                //Backup a folder
                if (folderBackups != null && folderBackups.Count > 0)
                {
                    foreach (FolderBackup backup in folderBackups)
                    {
                        if (!Directory.Exists(backup.FolderName))
                        {
                            LogHelper.LogMessage("Warning", "Backup folder not found: " + backup.FolderName);
                            continue;
                        }

                        if (backup.BackupTime.Hour == Hour && backup.BackupTime.Minute == Minute)
                        {
                            try
                            {
                                string backupFolderName = Path.GetFileName(backup.FolderName);

                                string destinationPath = Path.Combine(generalSetting.LocalFolder, backupFolderName + "-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".zip");
                                ZipFile.CreateFromDirectory(backup.FolderName, destinationPath, CompressionLevel.Optimal, true);

                                //log Message
                                LogHelper.LogMessage("Info", "Folder '" + backup.FolderName + "' backed up successfully at '" + destinationPath + "'");

                                //email config
                                if (emailHelper != null && emailSetting.LocalSuccess)
                                {
                                    string body = "Dear Admin, \n\n" +
                                        "This is to notify you that following backup task has completed successfully:\n" +
                                        "Backup Type: Folder\n" +
                                        "Folder Name: " + backup.FolderName + "\n" +
                                        "Backup Location: Local\n" +
                                        "Backup Time: " + DateTime.Now.ToString("MMM dd, yyyy HH:mm:ss", DateTimeFormatInfo.InvariantInfo) + ".\n\n" +
                                        "Runtime Backup Manager";

                                    bool emailSuccess = await emailHelper.SendEmail(emailSetting.RecipientEmail, "Success: Backup for Folder '" + backup.FolderName + "'", body);
                                    if (emailSuccess == false)
                                    {
                                        LogHelper.LogMessage("Error", "E-Mail sending failed. " + emailHelper.ResultMessage);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {

                                string error = Functions.GetErrorFromException(ex);

                                //log Message
                                LogHelper.LogMessage("Error", "Folder backup failed for '" + backup.FolderName + "'. " + error);

                                //email config
                                if (emailHelper != null && emailSetting.LocalFailure)
                                {
                                    string body = "Dear Admin, \n\n" +
                                        "This is to notify you that following backup task has failed:\n" +
                                        "Backup Type: Folder\n" +
                                        "Folder Name: " + backup.FolderName + "\n" +
                                        "Backup Location: Local\n" +
                                        "Backup Time: " + DateTime.Now.ToString("MMM dd, yyyy HH:mm:ss", DateTimeFormatInfo.InvariantInfo) + ".\n\n" +
                                        "Runtime Backup Manager";

                                    bool emailSuccess = await emailHelper.SendEmail(emailSetting.RecipientEmail, "Failure: Backup for Folder '" + backup.FolderName + "'", body);
                                    if (emailSuccess == false)
                                    {
                                        LogHelper.LogMessage("Error", "E-Mail sending failed. " + emailHelper.ResultMessage);
                                    }
                                }
                            }
                        }
                    }
                }

                //Upload to AWS S3
                if (awsS3Setting != null && awsS3Setting.BackupTime.Hour == Hour && awsS3Setting.BackupTime.Minute == Minute)
                {
                    if (generalSetting != null && Directory.Exists(generalSetting.LocalFolder))
                    {
                        AWSS3Helper helper = new AWSS3Helper(awsS3Setting.AccessKeyId, awsS3Setting.AccessSecretKey, awsS3Setting.AWSRegion, awsS3Setting.BucketName);

                        foreach (var file in Directory.GetFiles(generalSetting.LocalFolder))
                        {
                            bool Result = await helper.UploadToS3Async(file, awsS3Setting.DeleteAfterBackup);
                            if (Result == true)
                            {
                                //log Message
                                LogHelper.LogMessage("Info", "File uploaded to S3: " + file);

                                //email config
                                if (emailHelper != null && emailSetting.RemoteSuccess)
                                {
                                    string body = "Dear Admin, \n\n" +
                                        "This is to notify you that following backup task has completed successfully:\n" +
                                        "File Name: " + file + "\n" +
                                        "Backup Location: Remote\n" +
                                        "Backup Time: " + DateTime.Now.ToString("MMM dd, yyyy HH:mm:ss", DateTimeFormatInfo.InvariantInfo) + ".\n\n" +
                                        "Runtime Backup Manager";

                                    bool emailSuccess = await emailHelper.SendEmail(emailSetting.RecipientEmail, "Success: Remote Backup to Amazon S3", body);
                                    if (emailSuccess == false)
                                    {
                                        LogHelper.LogMessage("Error", "E-Mail sending failed. " + emailHelper.ResultMessage);
                                    }
                                }
                            }
                            else
                            {
                                //log Message
                                LogHelper.LogMessage("Error", "File upload to S3 Failed. " + helper.Message);

                                //email config
                                if (emailHelper != null && emailSetting.RemoteFailure)
                                {
                                    string body = "Dear Admin, \n\n" +
                                        "This is to notify you that following backup task has failed:\n" +
                                        "File Name: " + file + "\n" +
                                        "Backup Location: Remote\n" +
                                        "Backup Time: " + DateTime.Now.ToString("MMM dd, yyyy HH:mm:ss", DateTimeFormatInfo.InvariantInfo) + ".\n\n" +
                                        "Runtime Backup Manager";

                                    bool emailSuccess = await emailHelper.SendEmail(emailSetting.RecipientEmail, "Failure: Remote Backup to Amazon S3", body);
                                    if (emailSuccess == false)
                                    {
                                        LogHelper.LogMessage("Error", "E-Mail sending failed. " + emailHelper.ResultMessage);
                                    }
                                }

                            }
                        }
                    }
                }

                //Delete local folder files after specified days
                if (generalSetting != null && generalSetting.AutoDelete == true && generalSetting.DeleteAfterDays != null)
                {
                    DateTime dateTimeBefore = DateTime.Now.AddDays((int)generalSetting.DeleteAfterDays * -1);
                    foreach(string file in Directory.GetFiles(generalSetting.LocalFolder))
                    {
                        DateTime filetime = File.GetCreationTime(file);
                        if (filetime < dateTimeBefore)
                        {
                            try
                            {
                                File.Delete(file);
                                LogHelper.LogMessage("Info", "File deletion failed for '" + file + "'");
                            }
                            catch (Exception ex)
                            {
                                LogHelper.LogMessage("Error", "File deletion failed for '" + file + "' \n" + Functions.GetErrorFromException(ex));
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                string message = Functions.GetErrorFromException(ex);
                MessageBox.Show("An error occurred. \n" + message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
            
    }

}
