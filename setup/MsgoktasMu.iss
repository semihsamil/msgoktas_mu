; Inno Setup 6 script — Mimar Sinan Göktaş Şantiye Takip
; Derlemeden once proje kokunde PUBLISH.bat calistirin.

#define MyAppName "Mimar Sinan Göktaş — Şantiye Takip"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "Mimar Sinan Göktaş"
#define MyAppExeName "MsgoktasMu.exe"
#define PublishDir "..\\publish"

[Setup]
AppId={{A8F3C2E1-9B4D-4F6A-8C1E-2D5E7A9B0C3D}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={localappdata}\MimarSinanGoktas\MsgoktasMu
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputDir=output
OutputBaseFilename=MsgoktasMu_Setup
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
PrivilegesRequired=lowest
UninstallDisplayIcon={app}\{#MyAppExeName}
SetupLogging=yes

[Languages]
Name: "turkish"; MessagesFile: "compiler:Languages\Turkish.isl"

[Tasks]
Name: "desktopicon"; Description: "Masaustu kisayolu olustur"; GroupDescription: "Ek secenekler:"; Flags: unchecked

[Files]
Source: "{#PublishDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "..\KULLANIM_KILAVUZU.md"; DestDir: "{app}\docs"; Flags: ignoreversion
Source: "..\IHTIYAC_ANALIZI.md"; DestDir: "{app}\docs"; Flags: ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\Kullanim Kilavuzu"; Filename: "{app}\docs\KULLANIM_KILAVUZU.md"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Programi baslat"; Flags: nowait postinstall skipifsilent

[Messages]
turkish.WelcomeLabel2=Bu sihirbaz [name/ver] uygulamasini bilgisayariniza kuracaktir.%n%nDevam etmeden once calisan programi kapatmaniz onerilir.
