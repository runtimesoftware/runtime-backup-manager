using BackupManager.Classes;
using BackupManager.Views;
using System;
using System.IO;
using System.Net;
using System.Windows;

namespace BackupManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            //hook on error before app really starts
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            base.OnStartup(e);
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //put your tracing or logging code here(I put a message box as an example)
            string msg = e.ExceptionObject.ToString();
            MessageBox.Show("We're sorry. \n\nAn error has occurred and the program needs to close.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //for secure https over tls1.2
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            //Step 1: Initialize Application (Setup Environment Variables)
            try
            {

                //Set App Version
                EnVar.AppVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

                string tmpPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                EnVar.AppWorkingPath = Path.Combine(tmpPath, "Runtime Backup Manager");

                //if directory does not exist, it's a fresh install
                if (!Directory.Exists(EnVar.AppWorkingPath)) Directory.CreateDirectory(EnVar.AppWorkingPath);

                EnVar.AppTempPath = Path.Combine(EnVar.AppWorkingPath, "Temp");
                if (!Directory.Exists(EnVar.AppTempPath)) Directory.CreateDirectory(EnVar.AppTempPath);

                //App exe path for quick access
                EnVar.AppPath = AppDomain.CurrentDomain.BaseDirectory;

                //string dbName = "RBMDATA.DB";
                //string filePath = Path.Combine(EnVar.AppWorkingPath, dbName);
                //EnVar.DbConnectionString = @"data source=" + filePath + ";Version=3;";

                //if (!File.Exists(Path.Combine(EnVar.AppWorkingPath, dbName)))
                //{
                //    DatabaseHelper dbHelper = new DatabaseHelper();
                //    if (dbHelper.CreateDatabase() == false)
                //    {
                //        MessageBox.Show("A problem occurred while creating the config database. \n" + dbHelper.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                //        return;
                //    }
                //}

            }
            catch (Exception ex)
            {
                MessageBox.Show("A problem occurred while initializing the application. \n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            winSplash win = new winSplash();
            win.Show();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            
        }

    }
}
