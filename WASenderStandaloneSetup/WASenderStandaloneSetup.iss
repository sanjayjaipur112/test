; WASender Standalone Setup Script
; Inno Setup Script

#define MyAppName "WASender"
#define MyAppVersion "1.0"
#define MyAppPublisher "WASender"
#define MyAppURL "https://wasender.com"
#define MyAppExeName "WASender.exe"

[Setup]
AppId={{5D3DDC1C-488F-4511-8CC6-BC86545A1D99}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
LicenseFile=..\LICENSE.txt
OutputDir=Output
OutputBaseFilename=WASenderSetup
Compression=lzma
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=admin

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "..\Standalone\WASender.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Standalone\WASender.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Standalone\LogoWA.ico"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Standalone\chromedriver.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Standalone\*.wav"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Standalone\*.txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Standalone\Resources\*"; DestDir: "{app}\Resources"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "..\Standalone\runtimes\*"; DestDir: "{app}\runtimes"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent
