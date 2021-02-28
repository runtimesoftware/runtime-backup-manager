using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using BackupManager.Classes;
using BackupManager.Helpers;

namespace BackupManager.Views
{
    /// <summary>
    /// Interaction logic for winSplash.xaml
    /// </summary>
    public partial class winSplash : Window
    {

        public winSplash()
        {
            InitializeComponent();
        }
        
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lblVersion.Content = "Version: " + EnVar.AppVersion;

            await Task.Delay(500);

            winApplication win = new winApplication();
            win.Show();

            Close();
        }

    }
}
