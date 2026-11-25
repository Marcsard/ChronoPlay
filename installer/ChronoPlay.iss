#define AppName "ChronoPlay"
#ifndef AppVersion
  #define AppVersion "1.0.0"
#endif

[Setup]
AppId={{9A8B1377-3520-46C9-974D-91B436AFE6CF}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher=KrizMet66
DefaultDirName={localappdata}\Programs\{#AppName}
DefaultGroupName={#AppName}
DisableProgramGroupPage=yes
PrivilegesRequired=lowest
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
SetupIconFile=..\ChronoPlay\Assets\ChronoPlay.ico
UninstallDisplayIcon={app}\ChronoPlay.exe
WizardStyle=modern
Compression=lzma2
SolidCompression=yes
OutputDir=..\artifacts
OutputBaseFilename=ChronoPlay-Setup-{#AppVersion}-win-x64
CloseApplications=yes

[Tasks]
Name: "desktopicon"; Description: "Crear acceso directo en el escritorio"; GroupDescription: "Accesos directos:"; Flags: unchecked

[Files]
Source: "..\ChronoPlay\bin\Release\net10.0-windows\win-x64\publish\ChronoPlay.exe"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{autoprograms}\ChronoPlay"; Filename: "{app}\ChronoPlay.exe"
Name: "{autodesktop}\ChronoPlay"; Filename: "{app}\ChronoPlay.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\ChronoPlay.exe"; Description: "Iniciar ChronoPlay"; Flags: postinstall nowait skipifsilent
