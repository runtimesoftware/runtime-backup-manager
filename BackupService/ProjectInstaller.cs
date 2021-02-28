using System.ComponentModel;

namespace BackupService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
            this.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceInstaller1.Description = "Backups up databases and folders on a computer and optionally uploads them to remote storage";
            this.serviceInstaller1.DisplayName = "Runtime Backup Service";
        }
    }
}
