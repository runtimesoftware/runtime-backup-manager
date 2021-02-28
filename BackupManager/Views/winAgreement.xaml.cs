using System.Windows;
using System.Windows.Input;
using BackupManager.Views;

namespace BackupManager
{
    /// <summary>
    /// Interaction logic for winAgreement.xaml
    /// </summary>
    public partial class winAgreement : Window
    {
        public winAgreement()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                txtAgreement.Text = Properties.Resources.EULA;
            }
            catch //(Exception ex)
            {
                txtAgreement.Text = "Unable to load license agreement. We regret for the inconvenience caused.";
            }
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        }

        private void BtnAccept_Click(object sender, RoutedEventArgs e)
        {
            winApplication win = new winApplication();
            win.Show();
            Close();
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

    }
}
