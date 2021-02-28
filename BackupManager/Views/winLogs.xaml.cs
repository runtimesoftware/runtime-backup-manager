using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using BackupManager.Helpers;
using System.ComponentModel;

namespace BackupManager
{

    public partial class winLogs : Window, INotifyPropertyChanged
    {

        //Private Fields
        private List<string> _logs;
        private string _message;

        //Public Properties
        public List<string> Logs { get { return _logs; } set { _logs = value; RaisePropertyChanged("Logs"); } }
        public string Message { get { return _message; } set { _message = value; RaisePropertyChanged("Message"); } }
   
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

        public winLogs()
        {
            InitializeComponent();
            this.DataContext = this;

            Message = "";

            try
            {
                Logs = LogHelper.GetLogs();
                if (Logs == null || Logs.Count == 0)
                {
                    Message = "No log entires found for the day.";
                }
                else if (Logs.Count > 99)
                {
                    Message = "Recent 100 items are shown. Check actual log file for full details.";
                }
            }
            catch (Exception ex)
            {
                Message = Functions.GetErrorFromException(ex);
            }
        }

        //To enable dragging window from title area
        private void lblTitle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
