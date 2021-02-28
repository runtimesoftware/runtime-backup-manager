using BackupManager.Classes;
using BackupManager.Helpers;
using BackupManager.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows;

namespace BackupManager.Views
{
    /// <summary>
    /// Interaction logic 
    /// </summary>
    public partial class winApplication : Window, INotifyPropertyChanged
    {

        //Private Fields
        private int _mssqlCount;
        private int _mysqlCount;
        private int _folderCount;
        private string _awsS3SettingColor;
        private string _emailSettingColor;
        private string _awsS3SettingStatus;
        private string _emailSettingStatus;
        private string _serviceColor;
        private string _serviceStatus;

        //Public Properties
        public int MSSQLCount { get { return _mssqlCount; } set { _mssqlCount = value; RaisePropertyChanged("MSSQLCount"); } }
        public int MySQLCount { get { return _mysqlCount; } set { _mysqlCount = value; RaisePropertyChanged("MySQLCount"); } }
        public int FolderCount { get { return _folderCount; } set { _folderCount = value; RaisePropertyChanged("FolderCount"); } }
        public string AwsS3SettingColor { get { return _awsS3SettingColor; } set { _awsS3SettingColor = value; RaisePropertyChanged("AwsS3SettingColor"); } }
        public string EmailSettingColor { get { return _emailSettingColor; } set { _emailSettingColor = value; RaisePropertyChanged("EmailSettingColor"); } }
        public string AwsS3SettingStatus { get { return _awsS3SettingStatus; } set { _awsS3SettingStatus = value; RaisePropertyChanged("AwsS3SettingStatus"); } }
        public string EmailSettingStatus { get { return _emailSettingStatus; } set { _emailSettingStatus = value; RaisePropertyChanged("EmailSettingStatus"); } }
        public string ServiceStatus { get { return _serviceStatus; } set { _serviceStatus = value; RaisePropertyChanged("ServiceStatus"); } }
        public string ServiceColor { get { return _serviceColor; } set { _serviceColor = value; RaisePropertyChanged("ServiceColor"); } }

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
               
        public winApplication()
        {

            InitializeComponent();

            //Binding Context for application
            this.DataContext = this;

            GenerateSummary();

        }

        private void GenerateSummary()
        {

            AwsS3SettingColor = "LightGray";
            EmailSettingColor = "LightGray";
            ServiceColor = "LightGray";

            try
            {

                MSSQLHelper mssqlHelper = new MSSQLHelper();
                List<MSSQLBackup> sqlitems = mssqlHelper.GetConfigList();
                MSSQLCount = sqlitems != null ? sqlitems.Count() : 0;

                MYSQLHelper mysqlHelper = new MYSQLHelper();
                List<MYSQLBackup> mysqlitems = mysqlHelper.GetConfigList();
                MySQLCount = mysqlitems != null ? mysqlitems.Count() : 0;

                FolderHelper folderHelper = new FolderHelper();
                List<FolderBackup> folderitems = folderHelper.GetConfigList();
                FolderCount = folderitems != null ? folderitems.Count() : 0;

                AWSS3Helper awsHelper = new AWSS3Helper();
                AWSS3Setting awsSetting = awsHelper.GetConfig();
                if (awsSetting == null)
                {
                    AwsS3SettingStatus = "AWS S3 settings not defined";
                }
                else if (awsSetting.IsValid == false)
                {
                    AwsS3SettingStatus = "AWS S3 settings are not valid";
                }
                else if (awsSetting.IsActive == false)
                {
                    AwsS3SettingStatus = "AWS S3 settings are not active";
                }
                else
                {
                    AwsS3SettingStatus = "AWS S3 settings are valid & active";
                    AwsS3SettingColor = "Green";
                }

                EmailHelper emailHelper = new EmailHelper();
                EmailSetting emailSetting = emailHelper.GetConfig();
                if (emailSetting == null)
                {
                    EmailSettingStatus = "E-Mail settings are not defined";
                }
                else if (emailSetting.IsValid == false)
                {
                    EmailSettingStatus = "E-Mail settings are not valid";
                }
                else
                {
                    EmailSettingStatus = "Email settings are valid & active";
                    EmailSettingColor = "Green";
                }

                ServiceStatus = "Background service is not installed";

                GeneralSettingHelper generalHelper = new GeneralSettingHelper();
                GeneralSetting generalSetting = generalHelper.GetConfig();
                if (generalSetting != null && generalSetting.ServiceInstalled == true)
                {
                    ServiceStatus = "Background service is installed";
                    ServiceColor = "Green";
                    btnInstallService.Visibility = Visibility.Collapsed;
                    btnUnInstallService.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                string message = Functions.GetErrorFromException(ex);
                MessageBox.Show("An error occurred. \n" + message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Label_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left) DragMove();
        }

        private async void btnInstallService_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                
                string servicePath = EnVar.AppPath + "rbs.exe";
                if (!File.Exists(servicePath))
                {
                    MessageBox.Show("Unable to find service. Try reinstalling the application.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                ProcessStartInfo startInfoRegistry = new ProcessStartInfo
                {
                    FileName = "sc.exe",
                    Arguments = "create \"Runtime Backup Service\" binpath=\"" + servicePath + "\" start=auto ",
                    CreateNoWindow = true,
                    UseShellExecute = true
                };

                Process processRegistry = new Process() { StartInfo = startInfoRegistry };
                processRegistry.Start();

                await Task.Delay(2000);

                //Start the service
                ServiceController service = new ServiceController("Runtime Backup Service", ".");
                if (service != null && service.Status != ServiceControllerStatus.Running) service.Start();

                ServiceStatus = "Service is installed";
                ServiceColor = "Green";

                GeneralSettingHelper generalHelper = new GeneralSettingHelper();
                GeneralSetting generalSetting = generalHelper.GetConfig();
                if (generalSetting != null)
                {
                    generalSetting.ServiceInstalled = true;
                    generalHelper.SaveConfig(generalSetting);
                }

                btnInstallService.Visibility = Visibility.Collapsed;
                btnUnInstallService.Visibility = Visibility.Visible;

            }
            catch (Exception ex)
            {
                string message = Functions.GetErrorFromException(ex);
                MessageBox.Show("Failed to install the service. \n" + message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnUnInstallService_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Stop the service
                ServiceController service = new ServiceController("Runtime Backup Service", ".");
                if (service != null && service.Status != ServiceControllerStatus.Stopped) service.Stop();

                //Remove Registry Entry
                ProcessStartInfo startInfoDelete = new ProcessStartInfo
                {
                    FileName = "sc.exe",
                    Arguments = "delete \"Runtime Backup Service\"",
                    CreateNoWindow = true,
                    UseShellExecute = true
                };

                Process processDelete = new Process() { StartInfo = startInfoDelete };
                processDelete.Start();

                ServiceStatus = "Background service is not installed";
                ServiceColor = "LightGray";

                GeneralSettingHelper generalHelper = new GeneralSettingHelper();
                GeneralSetting generalSetting = generalHelper.GetConfig();
                if (generalSetting != null)
                {
                    generalSetting.ServiceInstalled = true;
                    generalHelper.SaveConfig(generalSetting);
                }

                btnInstallService.Visibility = Visibility.Visible;
                btnUnInstallService.Visibility = Visibility.Collapsed;

            }
            catch (Exception ex)
            {
                string message = Functions.GetErrorFromException(ex);
                MessageBox.Show("Failed to install the service. \n" + message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }   
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnLogs_Click(object sender, RoutedEventArgs e)
        {
            winLogs win = new winLogs { Owner = this };
            win.ShowDialog();
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            winSettings win = new winSettings() { Owner = this };
            win.ShowDialog();
            GenerateSummary();
        }

    }
}
