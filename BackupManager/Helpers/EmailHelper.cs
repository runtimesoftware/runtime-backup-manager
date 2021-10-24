using BackupManager.Classes;
using BackupManager.Model;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace BackupManager.Helpers
{
    public class EmailHelper
    {

        /// <summary>
        /// Default Constructor for saving or retrieving configuration
        /// </summary>
        public EmailHelper()
        {

        }

        /// <summary>
        /// Constructor for calling SendEmail Method
        /// </summary>
        public EmailHelper(string _displayName, string _emailAddress, string _username, string _password, string _smtpServer, int _smtpPort, bool _smtpSsl)
        {
            DisplayName = _displayName;
            EmailAddress = _emailAddress;
            Username = _username;
            Password = _password;
            SmtpServer = _smtpServer;
            SmtpPort = _smtpPort;
            SmtpSsl = _smtpSsl;
        }

        /// <summary>
        /// Returns config if saved
        /// </summary>
        /// <returns></returns>
        public EmailSetting GetConfig()
        {
            try
            {
                string configFile = Path.Combine(EnVar.AppWorkingPath, "email.config");
                if (!File.Exists(configFile)) return null;

                string fileText = File.ReadAllText(configFile);
                EmailSetting setting = JsonConvert.DeserializeObject<EmailSetting>(fileText);
                return setting;

            }
            catch (Exception ex)
            {
                LogHelper.LogMessage("error", "Unable to load E-Mail Settings | " + Functions.GetErrorFromException(ex));
                return null;
            }
        }

        /// <summary>
        /// Saves Configuration on File
        /// </summary>
        public bool SaveConfig(EmailSetting setting)
        {
            try
            {
                string configFile = Path.Combine(EnVar.AppWorkingPath, "email.config");
                File.WriteAllText(configFile, JsonConvert.SerializeObject(setting));
                return true;
            }
            catch //(Exception ex)
            {
                return false;
            }
        }

        public string ResultMessage = "";

        readonly string DisplayName;
        readonly string EmailAddress;
        readonly string Username;
        readonly string Password;
        readonly string SmtpServer;
        readonly int SmtpPort;
        readonly bool SmtpSsl;

        public async Task<bool> SendEmail(string Recipient, string Subject, string Body)
        {
            try
            {

                //send a test email
                SmtpClient client = new SmtpClient(SmtpServer, SmtpPort)
                {
                    Credentials = new NetworkCredential(Username, Password),
                    EnableSsl = SmtpSsl,
                };

                using (MailMessage mm = new MailMessage())
                {
                    mm.To.Add(new MailAddress(Recipient));
                    mm.From = new MailAddress(EmailAddress, DisplayName);
                    mm.Subject = Subject;
                    mm.Body = Body;
                    mm.IsBodyHtml = false;
                    await Task.Run(() => { client.Send(mm); });
                }

                ResultMessage = "Mail succesfully sent";
                return true;

            }
            catch (Exception ex)
            {
                ResultMessage = Functions.GetErrorFromException(ex);
                //Logger.LogIt("error", "/api/appDaily | Database backup to AWS S3 failed | " + ex.Message, "system");
                return false;
            }
        }

    }
}
