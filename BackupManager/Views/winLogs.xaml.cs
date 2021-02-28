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
        //private DateTime _dateFrom;
        //private DateTime _dateTo;
        private string _message;

        ////ViewModels
        //public class LogViewModel
        //{
        //    public DateTime _logtime { get; set; }
        //    public string DateTime { get; set; }
        //    public string LogType { get; set; }
        //    public string Description { get; set; }
        //}

        //Public Properties
        public List<string> Logs { get { return _logs; } set { _logs = value; RaisePropertyChanged("Logs"); } }
        public string Message { get { return _message; } set { _message = value; RaisePropertyChanged("Message"); } }
        //public DateTime DateFrom { get { return _dateFrom; } set { _dateFrom = value; RaisePropertyChanged("DateFrom"); } }
        //public DateTime DateTo { get { return _dateTo; } set { _dateTo = value; RaisePropertyChanged("DateTo"); } }

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

            //DateFrom = DateTime.Now;
            //DateTo = DateTime.Now;
        }

        //To enable dragging window from title area
        private void lblTitle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {

            Message = "";

            //if (DateFrom.Year < 1900)
            //{
            //    Message = "Please select Date From";
            //    return;
            //}

            //if (DateTo.Year < 1900)
            //{
            //    Message = "Please select Date From";
            //    return;
            //}

            //DateFrom = DateFrom.Date;
            //DateTo = DateTo.Date.AddDays(1).AddSeconds(-1);

            try
            {

                Logs = LogHelper.GetLogs();
                RaisePropertyChanged("Logs");

            }
            catch (Exception ex)
            {
                Message = Functions.GetErrorFromException(ex);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
