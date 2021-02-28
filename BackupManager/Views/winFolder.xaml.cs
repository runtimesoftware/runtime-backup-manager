using BackupManager.Helpers;
using BackupManager.Model;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace BackupManager
{
    /// <summary>
    /// Interaction logic 
    /// </summary>
    public partial class winFolder : Window, INotifyPropertyChanged
    {

        //Private Fields
        private string _folderName;
        private DateTime? _backupTime;
        private string _message;
        private bool _isActive;

        //Public Properties
        public string FolderName { get { return _folderName; } set { _folderName = value; RaisePropertyChanged("FolderName"); } }
        public DateTime? BackupTime { get { return _backupTime; } set { _backupTime = value; RaisePropertyChanged("BackupTime"); } }
        public string Message { get { return _message; } set { _message = value; RaisePropertyChanged("Message"); } }
        public bool BackupIsActive { get { return _isActive; } set { _isActive = value; RaisePropertyChanged("BackupIsActive"); } }

        string EditId;

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

        public winFolder(string _editId = "")
        {
            InitializeComponent();
            DataContext = this;
            EditId = _editId;

            //load settings if editing
            if (!string.IsNullOrWhiteSpace(EditId))
            {
                try
                {
                    FolderHelper helper = new FolderHelper();
                    FolderBackup backup = helper.GetConfig(EditId);
                    if (backup == null)
                    {
                        MessageBox.Show("Settings were not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    FolderName = backup.FolderName;
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

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog ofd = new System.Windows.Forms.FolderBrowserDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return;
            string path = ofd.SelectedPath;
            FolderName = path;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //validations
            if (string.IsNullOrWhiteSpace(FolderName))
            {
                Message = "Please select a folder";
                return;
            }

            if (BackupTime == null)
            {
                Message = "Backup time cannot be empty";
                return;
            }

            try
            {

                FolderHelper helper = new FolderHelper();
                if (!string.IsNullOrWhiteSpace(EditId)) helper.DeleteConfig(EditId);

                helper.AddConfig(new FolderBackup
                {
                    Id = Functions.RandomString(20),
                    FolderName = FolderName,
                    BackupTime = (DateTime)BackupTime,
                    IsActive = BackupIsActive,
                    CreatedOn = DateTime.Now,
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
