using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using BackupManager.Helpers;
using BackupManager.Model;
using System.IO;
using System.ComponentModel;
using BackupManager.Classes;
using System.Windows.Documents;

namespace BackupManager
{

    public partial class winSettings : Window, INotifyPropertyChanged
    {

        //Private Fields
        private List<DatabaseViewModel> _mssqlList;
        private List<DatabaseViewModel> _mysqlList;
        private List<FolderViewModel> _folderList;
        private string _messageFolder;
        private string _messageAws;
        private string _messageEmail;
        private AWSS3Setting _awsS3;
        private EmailSetting _email;
        private GeneralSetting _general;

        //ViewModels
        public class DatabaseViewModel
        {
            public string Id { get; set; }
            public string ServerName { get; set; }
            public string DatabaseName { get; set; }
            public DateTime _backuptime { get; set; }
            public DateTime? _lastBackup { get; set; }
            public string BackupTime { get; set; }
            public string LastBackup { get; set; }
            public string Edit { get { return "Edit"; } private set { } }
            public string Delete { get { return "Delete"; } private set { } }
        }

        public class FolderViewModel
        {
            public string Id { get; set; }
            public string FolderName { get; set; }
            public DateTime _backuptime { get; set; }
            public DateTime? _lastBackup { get; set; }
            public string BackupTime { get; set; }
            public string LastBackup { get; set; }
            public string Edit { get { return "Edit"; } private set { } }
            public string Delete { get { return "Delete"; } private set { } }
        }

        //Public Properties
        public List<DatabaseViewModel> MSSQLList { get { return _mssqlList; } set { _mssqlList = value; RaisePropertyChanged("MSSQLList"); } }
        public List<DatabaseViewModel> MySQLList { get { return _mysqlList; } set { _mysqlList = value; RaisePropertyChanged("MySQLList"); } }
        public List<FolderViewModel> FolderList { get { return _folderList; } set { _folderList = value; RaisePropertyChanged("FolderList"); } }
        public string MessageFolder { get { return _messageFolder; } set { _messageFolder = value; RaisePropertyChanged("MessageFolder"); } }
        public string MessageAWS { get { return _messageAws; } set { _messageAws = value; RaisePropertyChanged("MessageAWS"); } }
        public string MessageEmail { get { return _messageEmail; } set { _messageEmail = value; RaisePropertyChanged("MessageEmail"); } }
        public AWSS3Setting AWSS3 { get { return _awsS3; } set { _awsS3 = value; RaisePropertyChanged("AWSS3"); } }
        public EmailSetting Email { get { return _email; } set { _email = value; RaisePropertyChanged("Email"); } }
        public GeneralSetting General { get { return _general; } set { _general = value; RaisePropertyChanged("General"); } }

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
                
        public void RaisePropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }

        #endregion

        public winSettings()
        {
            InitializeComponent();
            this.DataContext = this;

            //Load all values
            //Individual functions, so that we can call them separately
            LoadMSSQL();
            LoadMySQL();
            LoadFolders();
            LoadOtherSettings();
        }

        //To enable draggingg window from title area
        private void lblTitle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }

        #region MSSQL

        private void LoadMSSQL()
        {
            try
            {

                MSSQLHelper helper = new MSSQLHelper();
                List<MSSQLBackup> items = helper.GetConfigList();
                if (items != null && items.Count > 0)
                {
                    MSSQLList = (from t in items
                                 select new DatabaseViewModel
                                 {
                                     Id = t.Id,
                                     ServerName = t.ServerName,
                                     DatabaseName = t.DatabaseName,
                                     _backuptime = t.BackupTime,
                                     _lastBackup = t.LastBackupTime,
                                 }).ToList();

                    MSSQLList.ForEach(a => a.BackupTime = a._backuptime.ToString("HH:mm"));
                    MSSQLList.ForEach(a => a.LastBackup = a._lastBackup == null ? "Never" : ((DateTime)a._lastBackup).ToString("MMM dd, yyyy HH:mm"));
                }
                else
                {
                    MSSQLList = new List<DatabaseViewModel>();
                }
            }
            catch (Exception ex)
            {
                string message = Functions.GetErrorFromException(ex);
                MessageBox.Show("An error occurred while loading MSSQL backup configuration. \n" + message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAddMSSQL_Click(object sender, RoutedEventArgs e)
        {
            winMSSQL win = new winMSSQL { Owner = this };
            win.ShowDialog();
            LoadMSSQL();
        }

        private void MSSQLEditColumn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var rowData = ((Hyperlink)e.OriginalSource).DataContext as DatabaseViewModel;
                if (rowData == null) return;

                winMSSQL win = new winMSSQL (rowData.Id) { Owner = this };
                win.ShowDialog();
                LoadMSSQL();

            }
            catch (Exception ex)
            {
                string message = Functions.GetErrorFromException(ex);
                MessageBox.Show("A problem occurred while opening edit dialog.\n" + message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MSSQLDeleteColumn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var rowData = ((Hyperlink)e.OriginalSource).DataContext as DatabaseViewModel;
                if (rowData == null) return;

                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this backup?", "Confirm Action", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No) return;

                MSSQLHelper helper = new MSSQLHelper();
                helper.DeleteConfig(rowData.Id);

                MessageBox.Show("Backup was successfully deleted.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadMSSQL();

            }
            catch (Exception ex)
            {
                string message = Functions.GetErrorFromException(ex);
                MessageBox.Show("A problem occurred while opening edit dialog.\n" + message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region MySQL

        private void LoadMySQL()
        {
            try
            {

                MYSQLHelper helper = new MYSQLHelper();
                List<MYSQLBackup> items = helper.GetConfigList();
                if (items != null && items.Count > 0)
                {
                    MySQLList = (from t in items
                                 select new DatabaseViewModel
                                 {
                                     Id = t.Id,
                                     ServerName = t.ServerName,
                                     DatabaseName = t.DatabaseName,
                                     _backuptime = t.BackupTime,
                                     _lastBackup = t.LastBackupTime,
                                 }).ToList();

                    MySQLList.ForEach(a => a.BackupTime = a._backuptime.ToString("HH:mm"));
                    MySQLList.ForEach(a => a.LastBackup = a._lastBackup == null ? "Never" : ((DateTime)a._lastBackup).ToString("MMM dd, yyyy HH:mm"));
                }
                else
                {
                    MySQLList = new List<DatabaseViewModel>();
                }
            }
            catch (Exception ex)
            {
                string message = Functions.GetErrorFromException(ex);
                MessageBox.Show("An error occurred while loading MSSQL backup configuration. \n" + message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAddMySQL_Click(object sender, RoutedEventArgs e)
        {
            winMySQL win = new winMySQL { Owner = this };
            win.ShowDialog();
            LoadMySQL();
        }

        private void MySQLEditColumn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var rowData = ((Hyperlink)e.OriginalSource).DataContext as DatabaseViewModel;
                if (rowData == null) return;

                winMySQL win = new winMySQL(rowData.Id) { Owner = this };
                win.ShowDialog();
                LoadMySQL();

            }
            catch (Exception ex)
            {
                string message = Functions.GetErrorFromException(ex);
                MessageBox.Show("A problem occurred while opening edit dialog.\n" + message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MySQLDeleteColumn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var rowData = ((Hyperlink)e.OriginalSource).DataContext as DatabaseViewModel;
                if (rowData == null) return;

                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this backup?", "Confirm Action", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No) return;

                MYSQLHelper helper = new MYSQLHelper();
                helper.DeleteConfig(rowData.Id);

                MessageBox.Show("Backup was successfully deleted.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadMySQL();

            }
            catch (Exception ex)
            {
                string message = Functions.GetErrorFromException(ex);
                MessageBox.Show("A problem occurred while opening edit dialog.\n" + message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Folders

        private void LoadFolders()
        {
            try
            {
                FolderHelper helper = new FolderHelper();
                List<FolderBackup> items = helper.GetConfigList();
                if (items != null && items.Count > 0)
                {
                    FolderList = (from t in items
                                  select new FolderViewModel
                                  {
                                      Id = t.Id,
                                      FolderName = t.FolderName,
                                      _backuptime = t.BackupTime,
                                      _lastBackup = t.LastBackupTime,
                                  }).ToList();

                    FolderList.ForEach(a => a.BackupTime = a._backuptime.ToString("HH:mm"));
                    FolderList.ForEach(a => a.LastBackup = a._lastBackup == null ? "Never" : ((DateTime)a._lastBackup).ToString("MMM dd, yyyy HH:mm"));
                }
                else
                {
                    FolderList = new List<FolderViewModel>();
                }
            }
            catch (Exception ex)
            {
                string message = Functions.GetErrorFromException(ex);
                MessageBox.Show("An error occurred while loading MSSQL backup configuration. \n" + message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAddFolder_Click(object sender, RoutedEventArgs e)
        {
            winFolder win = new winFolder { Owner = this };
            win.ShowDialog();
            LoadFolders();
        }

        private void FolderEditColumn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var rowData = ((Hyperlink)e.OriginalSource).DataContext as FolderViewModel;
                if (rowData == null) return;

                winFolder win = new winFolder(rowData.Id) { Owner = this };
                win.ShowDialog();
                LoadFolders();

            }
            catch (Exception ex)
            {
                string message = Functions.GetErrorFromException(ex);
                MessageBox.Show("A problem occurred while opening edit dialog.\n" + message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FolderDeleteColumn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var rowData = ((Hyperlink)e.OriginalSource).DataContext as FolderViewModel;
                if (rowData == null) return;

                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this backup?", "Confirm Action", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No) return;

                FolderHelper helper = new FolderHelper();
                helper.DeleteConfig(rowData.Id);

                MessageBox.Show("Backup was successfully deleted.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadFolders();

            }
            catch (Exception ex)
            {
                string message = Functions.GetErrorFromException(ex);
                MessageBox.Show("A problem occurred while opening edit dialog.\n" + message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Load Other Settings
        //Email, Local Folder, AWS S3
        
        private void LoadOtherSettings()
        {
            try
            {
                AWSS3Helper awsHelper = new AWSS3Helper();
                AWSS3 = awsHelper.GetConfig();
                if (AWSS3 == null) AWSS3 = new AWSS3Setting();

                EmailHelper emailHelper = new EmailHelper();
                Email = emailHelper.GetConfig();
                if (Email == null) Email = new EmailSetting();

                GeneralSettingHelper generalHelper = new GeneralSettingHelper();
                General = generalHelper.GetConfig();
                if (General == null) General = new GeneralSetting();

            }
            catch (Exception ex)
            {
                string message = Functions.GetErrorFromException(ex);
                MessageBox.Show("A problem occurred while loading settings. \n" + message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        #endregion

        private void btnFolderBrowse_Click(object sender, RoutedEventArgs e)
        {

            System.Windows.Forms.FolderBrowserDialog ofd = new System.Windows.Forms.FolderBrowserDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return;
            string path = ofd.SelectedPath;
            General.LocalFolder = path;
            RaisePropertyChanged("General");

        }

        private void btnValidateGeneral_Click(object sender, RoutedEventArgs e)
        {

            MessageFolder = "";

            try
            {
                //try writing to selected path
                string tmpFile = Path.Combine(General.LocalFolder, "test.txt");
                File.WriteAllText(tmpFile, "File created to check write access for this folder.");

                //try deleting from selected path
                File.Delete(tmpFile);

                GeneralSettingHelper generalHelper = new GeneralSettingHelper();
                generalHelper.SaveConfig(General);
                RaisePropertyChanged("General");

                MessageFolder = "Settings successfully updated";

            }
            catch (Exception ex)
            {
                string message = Functions.GetErrorFromException(ex);
                MessageBox.Show("The selected path is not writable. Make sure the path is writable. \n" + message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void btnValidateAws_Click(object sender, RoutedEventArgs e)
        {

            //validations
            if (string.IsNullOrWhiteSpace(AWSS3.BucketName))
            {
                MessageAWS = "Bucket name cannot be empty";
                return;
            }

            if (!string.IsNullOrWhiteSpace(AWSS3.FolderName))
            {
                if (AWSS3.FolderName.Contains("\\"))
                {
                    MessageAWS = "Folder name should not contain back slashes (\\). Use forward slash (/) to specify sub-folders";
                    return;
                }
            }

            if (string.IsNullOrWhiteSpace(AWSS3.AccessKeyId))
            {
                MessageAWS = "Access Key Id cannot be empty";
                return;
            }

            if (string.IsNullOrWhiteSpace(AWSS3.AccessSecretKey))
            {
                MessageAWS = "Access Secret Key cannot be empty";
                return;
            }

            if (string.IsNullOrWhiteSpace(AWSS3.AWSRegion))
            {
                MessageAWS = "AWS Region cannot be empty";
                return;
            }

            if (AWSS3.BackupTime == null)
            {
                MessageAWS = "Backup time cannot be empty";
                return;
            }

            try
            {

                //Create a temp file
                string tmpFile = Path.Combine(EnVar.AppTempPath, "test_upload_s3_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt");
                File.WriteAllText(tmpFile, "File created to check write access to this bucket.");

                //Try uploading a file to S3
                AWSS3Helper helper = new AWSS3Helper(AWSS3.AccessKeyId, AWSS3.AccessSecretKey, AWSS3.AWSRegion, AWSS3.BucketName, AWSS3.FolderName);

                MessageAWS = "Please wait while testing access to '" + AWSS3.BucketName + "' ...";

                btnValidateAWS.IsEnabled = false;
                AWSS3.IsValid = await helper.UploadToS3Async(tmpFile);
                btnValidateAWS.IsEnabled = true;

                if (AWSS3.IsValid == false)
                {
                    MessageAWS = "AWS S3 access check failed. Settings were still saved.";
                    MessageBox.Show("Could not upload to specified AWS bucket.\n" + helper.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                AWSS3Helper awsHelper = new AWSS3Helper();
                awsHelper.SaveConfig(AWSS3);

                RaisePropertyChanged("AWSS3");
                MessageAWS = "Settings successfully saved";

            }
            catch (Exception ex)
            {
                string message = Functions.GetErrorFromException(ex);
                MessageBox.Show("A problem occurred while validating details.\n" + message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private async void btnValidateEmail_Click(object sender, RoutedEventArgs e)
        {

            //validations
            if (string.IsNullOrWhiteSpace(Email.DisplayName))
            {
                MessageEmail = "Display name cannot be empty";
                return;
            }

            if (string.IsNullOrWhiteSpace(Email.EmailAddress))
            {
                MessageEmail = "E-Mail Address name cannot be empty";
                return;
            }

            if (string.IsNullOrWhiteSpace(Email.Username))
            {
                MessageEmail = "Username cannot be empty";
                return;
            }

            if (string.IsNullOrWhiteSpace(Email.Password))
            {
                MessageEmail = "Password cannot be empty";
                return;
            }

            if (string.IsNullOrWhiteSpace(Email.SmtpServer))
            {
                MessageEmail = "SMTP Server cannot be empty";
                return;
            }

            if (Email.SmtpPort <= 0)
            {
                MessageEmail = "SMTP should be a numeric value (usually 25, 465 or 587)";
                return;
            }

            if (string.IsNullOrWhiteSpace(Email.RecipientEmail))
            {
                MessageEmail = "Recipient email cannot be empty";
                return;
            }

            try
            {
                //try sending email
                EmailHelper helper = new EmailHelper(Email.DisplayName, Email.EmailAddress, Email.Username, Email.Password, Email.SmtpServer, Email.SmtpPort, Email.SmtpSsl);

                MessageEmail = "Please wait while testing email settings...";

                btnValidateEmail.IsEnabled = false;
                string body = "This is a test mail sent by Runtime Backup Manager to validate your outgoing e-mail settings.";
                Email.IsValid = await helper.SendEmail(Email.RecipientEmail, "Test Mail from Runtime Backup Manager", body);
                btnValidateEmail.IsEnabled = true;

                if (Email.IsValid == false)
                {
                    MessageEmail = "E-Mail sending failed. Settings are still saved.";
                    MessageBox.Show("Could not validate e-mail settings. \n" + helper.ResultMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                EmailHelper emailHelper = new EmailHelper();
                emailHelper.SaveConfig(Email);

                RaisePropertyChanged("Email");
                MessageEmail = "Settings successfully saved";


            }
            catch (Exception ex)
            {
                string message = Functions.GetErrorFromException(ex);
                MessageBox.Show("A problem occurred while validating details.\n" + message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
