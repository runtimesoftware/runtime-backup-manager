    
[Setup]
AppId=4AEAB4F5-7219-4E89-B760-5588348E2A6F
AppName=Runtime Backup Manager
AppPublisher=Runtime Software Private Limited
AppPublisherURL=https://www.runtimesoftware.in/
AppSupportURL=https://www.runtimesoftware.in/
AppUpdatesURL=https://www.runtimesoftware.in/
AppVersion=1.0
AppVerName=1.0
Compression=lzma
DefaultDirName={userpf}\Runtime Software\Runtime Backup Manager
DisableDirPage=yes
DefaultGroupName=Runtime Software
DisableProgramGroupPage=yes
OutputDir=bin
OutputBaseFilename=rbm_setup
SolidCompression=yes
SignTool=signtool
UninstallDisplayIcon={userpf}\Runtime Software\Runtime Backup Manager\rbm.exe
UninstallDisplayName=Runtime Backup Manager
PrivilegesRequired=lowest

DisableReadyPage=no
DisableReadyMemo=no
           
[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"                     

[Files]
Source: "src\Resources\runtime-logo-square.jpg"; DestDir: "{userpf}\Runtime Software\Runtime Backup Manager\Resources\"; Flags: ignoreversion
Source: "src\Newtonsoft.Json.dll"; DestDir: "{userpf}\Runtime Software\Runtime Backup Manager";
Source: "src\AWSSDK.Core.dll"; DestDir: "{userpf}\Runtime Software\Runtime Backup Manager";
Source: "src\AWSSDK.S3.dll"; DestDir: "{userpf}\Runtime Software\Runtime Backup Manager";
Source: "src\BouncyCastle.Crypto.dll"; DestDir: "{userpf}\Runtime Software\Runtime Backup Manager";
Source: "src\Google.Protobuf.dll"; DestDir: "{userpf}\Runtime Software\Runtime Backup Manager";
Source: "src\K4os.Compression.LZ4.dll"; DestDir: "{userpf}\Runtime Software\Runtime Backup Manager";
Source: "src\K4os.Compression.LZ4.Streams.dll"; DestDir: "{userpf}\Runtime Software\Runtime Backup Manager";
Source: "src\K4os.Hash.xxHash.dll"; DestDir: "{userpf}\Runtime Software\Runtime Backup Manager";
Source: "src\MaterialDesignColors.dll"; DestDir: "{userpf}\Runtime Software\Runtime Backup Manager";
Source: "src\MaterialDesignThemes.Wpf.dll"; DestDir: "{userpf}\Runtime Software\Runtime Backup Manager";
Source: "src\MySql.Data.dll"; DestDir: "{userpf}\Runtime Software\Runtime Backup Manager";
Source: "src\MySqlBackup.dll"; DestDir: "{userpf}\Runtime Software\Runtime Backup Manager";
Source: "src\Newtonsoft.Json.dll"; DestDir: "{userpf}\Runtime Software\Runtime Backup Manager";
Source: "src\rbm.exe"; DestDir: "{userpf}\Runtime Software\Runtime Backup Manager"; Flags: ignoreversion
Source: "src\rbm.exe.config"; DestDir: "{userpf}\Runtime Software\Runtime Backup Manager"; Flags: ignoreversion
Source: "src\rbs.exe"; DestDir: "{userpf}\Runtime Software\Runtime Backup Manager"; Flags: ignoreversion
Source: "src\rbs.exe.config"; DestDir: "{userpf}\Runtime Software\Runtime Backup Manager"; Flags: ignoreversion
Source: "src\Renci.SshNet.dll"; DestDir: "{userpf}\Runtime Software\Runtime Backup Manager";
Source: "src\System.Buffers.dll"; DestDir: "{userpf}\Runtime Software\Runtime Backup Manager";
Source: "src\System.Memory.dll"; DestDir: "{userpf}\Runtime Software\Runtime Backup Manager";
Source: "src\System.Runtime.CompilerServices.Unsafe.dll"; DestDir: "{userpf}\Runtime Software\Runtime Backup Manager";
Source: "src\Ubiety.Dns.Core.dll"; DestDir: "{userpf}\Runtime Software\Runtime Backup Manager";
Source: "src\Zstandard.Net.dll"; DestDir: "{userpf}\Runtime Software\Runtime Backup Manager";

[Icons]
Name: "{group}\Runtime Backup Manager"; Filename: "{userpf}\Runtime Software\Runtime Backup Manager\rmb.exe"
Name: "{commondesktop}\Runtime Backup Manager"; Filename: "{userpf}\Runtime Software\Runtime Backup Manager\rbm.exe";
