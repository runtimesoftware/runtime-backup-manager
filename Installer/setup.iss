    
[Setup]
AppId=4AEAB4F5-7219-4E89-B760-5588348E2A6F
AppName=Runtime Backup Manager
AppPublisher=Runtime Software Private Limited
AppPublisherURL=https://www.runtimesoftware.in/
AppSupportURL=https://www.runtimesoftware.in/
AppUpdatesURL=https://www.runtimesoftware.in/
AppVersion=1.0.3
AppVerName=1.0.3
Compression=lzma
DefaultDirName={pf}\Runtime Software\Runtime Backup Manager
DisableDirPage=no
DefaultGroupName=Runtime Software
DisableProgramGroupPage=yes
OutputDir=bin
OutputBaseFilename=rbm_setup
SolidCompression=yes
SignTool=signtool
UninstallDisplayIcon={app}\rbm.exe
UninstallDisplayName=Runtime Backup Manager
PrivilegesRequired=admin

DisableReadyPage=no
DisableReadyMemo=no
           
[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"                     

[Files]
Source: "src\Resources\runtime-logo-square.jpg"; DestDir: "{app}\Resources\"; Flags: ignoreversion
Source: "src\Newtonsoft.Json.dll"; DestDir: "{app}";
Source: "src\AWSSDK.Core.dll"; DestDir: "{app}";
Source: "src\AWSSDK.S3.dll"; DestDir: "{app}";
Source: "src\BouncyCastle.Crypto.dll"; DestDir: "{app}";
Source: "src\Google.Protobuf.dll"; DestDir: "{app}";
Source: "src\K4os.Compression.LZ4.dll"; DestDir: "{app}";
Source: "src\K4os.Compression.LZ4.Streams.dll"; DestDir: "{app}";
Source: "src\K4os.Hash.xxHash.dll"; DestDir: "{app}";
Source: "src\MaterialDesignColors.dll"; DestDir: "{app}";
Source: "src\MaterialDesignThemes.Wpf.dll"; DestDir: "{app}";
Source: "src\MySql.Data.dll"; DestDir: "{app}";
Source: "src\MySqlBackup.dll"; DestDir: "{app}";
Source: "src\Newtonsoft.Json.dll"; DestDir: "{app}";
Source: "src\rbm.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "src\rbm.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "src\rbs.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "src\rbs.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "src\Renci.SshNet.dll"; DestDir: "{app}";
Source: "src\System.Buffers.dll"; DestDir: "{app}";
Source: "src\System.Memory.dll"; DestDir: "{app}";
Source: "src\System.Runtime.CompilerServices.Unsafe.dll"; DestDir: "{app}";
Source: "src\Ubiety.Dns.Core.dll"; DestDir: "{app}";
Source: "src\Zstandard.Net.dll"; DestDir: "{app}";

[Icons]
Name: "{group}\Runtime Backup Manager"; Filename: "{app}\rmb.exe"
Name: "{commondesktop}\Runtime Backup Manager"; Filename: "{app}\rbm.exe";
