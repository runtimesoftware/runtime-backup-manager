using BackupManager.Helpers;
using System.ServiceProcess;
using System.Timers;

namespace BackupService
{

    partial class BackupService : ServiceBase
    {
        public BackupService()
        {
            InitializeComponent();
        }

        BackupHelper helper = new BackupHelper();

        protected override void OnStart(string[] args)
        {
            Timer timer = new Timer();
            timer.Interval = 60000; // 60 seconds
            timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
            timer.Start();

            LogHelper.LogMessage("Info", "Backup service has started");
        }

        public async void OnTimer(object sender, ElapsedEventArgs args)
        {
            await helper.ExecuteBackups();
        }

        protected override void OnStop()
        {
            LogHelper.LogMessage("Info", "Backup service has stopped");
        }

    }
}
