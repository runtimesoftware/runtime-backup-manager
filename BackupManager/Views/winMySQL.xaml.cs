using BackupManager.Helpers;
using System;
using System.Windows;
using MySql.Data.MySqlClient;
using System.ComponentModel;
using BackupManager.Model;
using System.Threading.Tasks;
using System.IO;
using BackupManager.Classes;

namespace BackupManager
{
    /// <summary>
    /// Interaction logic 
    /// </summary>
    public partial class winMySQL : Window, INotifyPropertyChanged
    {
       
        //Private Fields
        private string _serverName;
        private string _databaseName;
        private string _username;
        private string _password;
        private string _message;
        private DateTime? _backupTime;
        private bool _isActive;

        //Public Properties
        public string ServerName { get { return _serverName; } set { _serverName = value; RaisePropertyChanged("ServerName"); } }
        public string DatabaseName { get { return _databaseName; } set { _databaseName = value; RaisePropertyChanged("DatabaseName"); } }
        public string Username { get { return _username; } set { _username = value; RaisePropertyChanged("Username"); } }
        public string Password { get { return _password; } set { _password = value; RaisePropertyChanged("Password"); } }
        public string Message { get { return _message; } set { _message = value; RaisePropertyChanged("Message"); } }
        public DateTime? BackupTime { get { return _backupTime; } set { _backupTime = value; RaisePropertyChanged("BackupTime"); } }
        public bool BackupIsActive { get { return _isActive; } set { _isActive = value; RaisePropertyChanged("BackupIsActive"); } }
   
        private readonly string EditId = "";

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

        public winMySQL(string _editId = "")
        {

            InitializeComponent();
            DataContext = this;
            EditId = _editId;

            //load settings if editing
            if (!string.IsNullOrWhiteSpace(EditId))
            {
                try
                {
                    MYSQLHelper helper = new MYSQLHelper();
                    MYSQLBackup backup = helper.GetConfig(EditId);
                    if (backup == null)
                    {
                        MessageBox.Show("Settings were not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    ServerName = backup.ServerName;
                    DatabaseName = backup.DatabaseName;
                    Username = backup.Username;
                    Password = backup.Password;
                    BackupIsActive = backup.IsActive;
                    BackupTime = backup.BackupTime;
                }
                catch (Exception ex)
                {
                    string message = Functions.GetErrorFromException(ex);
                    MessageBox.Show("A problem occurred while loading backup details.\n" + message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Label_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left) DragMove();
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {

            //validations
            if (string.IsNullOrWhiteSpace(ServerName))
            {
                Message = "Server name cannot be empty";
                return;
            }

            if (string.IsNullOrWhiteSpace(DatabaseName))
            {
                Message = "Database name cannot be empty";
                return;
            }

            if (string.IsNullOrWhiteSpace(Username))
            {
                Message = "Username cannot be empty";
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                Message = "Password cannot be empty";
                return;
            }

            if (BackupTime == null)
            {
                Message = "Backup time cannot be empty";
                return;
            }


            //Try connecting
            try
            {

                btnSave.IsEnabled = false;
                Message = "Please wait while checking connection...";

                MYSQLHelper helper = new MYSQLHelper(ServerName, DatabaseName, Username, Password);

                string tempFile = Path.Combine(EnVar.AppTempPath, Functions.RandomString(10) + ".sqldump");
                bool Result = await helper.BackupDatabase(tempFile);
                                
                btnSave.IsEnabled = true;
                Message = "";

                if (Result == false)
                {
                    Message = "Could not connect to database server. \n" + helper.Message;
                    return;
                }

                if (File.Exists(tempFile)) File.Delete(tempFile);

                //connection successful, save settings

                //delete if editing
                if (!string.IsNullOrWhiteSpace(EditId)) helper.DeleteConfig(EditId);

                helper.AddConfig(new MYSQLBackup
                {
                    Id = Functions.RandomString(20),
                    ServerName = ServerName,
                    DatabaseName = DatabaseName,
                    Username = Username,
                    Password = Password,
                    BackupTime = (DateTime)BackupTime,
                    CreatedOn = DateTime.UtcNow,
                    IsActive = BackupIsActive,
                });
                
                Close();

            }
            catch (Exception ex)
            {
                string message = Functions.GetErrorFromException(ex);
                MessageBox.Show("A problem occurred while connecting to server.\n" + message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


    }
}
