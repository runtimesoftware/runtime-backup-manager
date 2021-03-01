# Runtime Backup Manager
Backs up databases and folders on a computer (usually a server), uploads them to a remote storage (optional) and sends email alerts to server administrator (optional).

Target Platforms: Windows 7, 8, 10 with .NET Framework 4.6 or higher

## Usage instructions and Available Options
01. Download the setup from Installer \ Bin folder (rsb_setup.exe)
02. Install on the computer you would like to backup
03. Run the software from the shortcut created on desktop 'Runtime Backup Manager'
04. Use the 'Settings' button to configure different settings as described below
05. There are 3 types of backups you can configure:
   a. MSSQL Server
   b. MySQL Server
   c. A Local Folder
06. There are 2 possible backup options:
   a. Local backup on same computer or network drive
   b. Remote backup to Amazon S3 bucket (from a certain local folder)
07. E-Mail alerts can be setup for following 4 types of actions:
   a. Successfull local backup
   b. Successfull remote backup
   c. Failed local backup
   d. Failed remote backup
08. After defining all settings, click 'Close' to come back to main screen.
09. Initially, the status of background service will show 'Background servie is not installed'
10. Click on 'Install' to install the service so that backup tasks can run. This is required to ensure backup tasks run in the background even when no user is logged on the system.
11. If service is successfully installed, the status will change to 'Background service is installed'

## Build Instructions for Latest version

TBD
