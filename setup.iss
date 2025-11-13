; Windows Post-Format Setup Tool - Inno Setup Script
; Requires Inno Setup 6.x or later

[Setup]
AppName=Windows Post-Format Setup Tool
AppVersion=1.0.0
AppPublisher=grilojr09br
AppPublisherURL=https://github.com/grilojr09br/Post-format-tools
AppSupportURL=https://github.com/grilojr09br/Post-format-tools/issues
AppUpdatesURL=https://github.com/grilojr09br/Post-format-tools/releases
DefaultDirName={autopf}\WindowsPostFormatSetup
DefaultGroupName=Windows Setup Tool
AllowNoIcons=yes
LicenseFile=LICENSE
OutputDir=Release
OutputBaseFilename=WindowsSetup-Installer
SetupIconFile=assets\icon.ico
Compression=lzma2/max
SolidCompression=yes
PrivilegesRequired=admin
PrivilegesRequiredOverridesAllowed=dialog
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
UninstallDisplayIcon={app}\WindowsSetup.exe
UninstallDisplayName=Windows Post-Format Setup Tool
VersionInfoVersion=1.0.0.0
VersionInfoCompany=grilojr09br
VersionInfoDescription=Windows Post-Format Setup Tool Installer
VersionInfoCopyright=Copyright (C) 2025
WizardStyle=modern
DisableProgramGroupPage=yes
DisableWelcomePage=no

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "brazilianportuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "Release\WindowsSetup.exe"; DestDir: "{app}"; Flags: ignoreversion
; Note: WindowsSetup.exe is self-contained, no DLLs needed

[Icons]
Name: "{group}\Windows Setup Tool"; Filename: "{app}\WindowsSetup.exe"
Name: "{group}\{cm:UninstallProgram,Windows Setup Tool}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\Windows Setup Tool"; Filename: "{app}\WindowsSetup.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\WindowsSetup.exe"; Description: "{cm:LaunchProgram,Windows Setup Tool}"; Flags: nowait postinstall skipifsilent

[Code]
function IsDotNetInstalled(): Boolean;
begin
  Result := RegKeyExists(HKLM, 'SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedhost\8.0') or
            RegKeyExists(HKLM, 'SOFTWARE\WOW6432Node\dotnet\Setup\InstalledVersions\x64\sharedhost\8.0');
end;

function InitializeSetup(): Boolean;
var
  ErrorCode: Integer;
begin
  Result := True;
  
  if not IsDotNetInstalled() then
  begin
    if MsgBox('.NET 8 Runtime is required but not installed.' + #13#10 + #13#10 +
              'Would you like to download and install it now?' + #13#10 +
              '(This will open the Microsoft download page)', mbConfirmation, MB_YESNO) = IDYES then
    begin
      ShellExec('open', 'https://dotnet.microsoft.com/download/dotnet/8.0', '', '', SW_SHOW, ewNoWait, ErrorCode);
    end;
    
    Result := False;
  end;
end;

[Messages]
WelcomeLabel2=This will install [name/ver] on your computer.%n%nThis tool helps automate Windows post-format setup by:%n%n• Backing up and restoring browser profiles%n• Installing 44+ development tools%n• Optimizing Windows for performance%n• GPU driver detection and installation%n• Activating Windows%n%nIt is recommended that you close all other applications before continuing.
