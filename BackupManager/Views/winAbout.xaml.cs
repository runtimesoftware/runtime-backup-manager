using AttendanceSync.Classes;
using System.Windows;

namespace AttendanceSync
{
    /// <summary>
    /// Interaction logic for winAbout.xaml
    /// </summary>
    public partial class winAbout : Window
    {
        public winAbout()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lblVersion.Content = "Ver. " + EnVar.AppVersion;
            lblClientName.Content = EnVar.ClientName;
            lblValidity.Content = EnVar.ValidTill.ToString("MMM dd, yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        
    }
}
