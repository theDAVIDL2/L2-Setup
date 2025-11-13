; Windows Post-Format Setup Tool - Inno Setup Script
; Requires Inno Setup 6.x or later

[Setup]
AppName=Windows Post-Format Setup Tool
AppVersion=1.0.0
AppPublisher=Your Name
AppPublisherURL=https://github.com/yourusername/windows-post-format-setup
AppSupportURL=https://github.com/yourusername/windows-post-format-setup/issues
AppUpdatesURL=https://github.com/yourusername/windows-post-format-setup/releases
DefaultDirName={autopf}\WindowsPostFormatSetup
DefaultGroupName=Windows Setup Tool
AllowNoIcons=yes
LicenseFile=LICENSE
OutputDir=output
OutputBaseFilename=WindowsPostFormatSetup_v1.0.0
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
VersionInfoCompany=Your Company
VersionInfoDescription=Windows Post-Format Setup Tool Installer
VersionInfoCopyright=Copyright (C) 2024
WizardStyle=modern
DisableProgramGroupPage=yes
DisableWelcomePage=no

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "brazilianportuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 6.1; Check: not IsAdminInstallMode

[Files]
Source: "src\WindowsSetup.App\bin\Release\net8.0-windows\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\Windows Setup Tool"; Filename: "{app}\WindowsSetup.exe"
Name: "{group}\{cm:UninstallProgram,Windows Setup Tool}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\Windows Setup Tool"; Filename: "{app}\WindowsSetup.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\WindowsSetup.exe"; Description: "{cm:LaunchProgram,Windows Setup Tool}"; Flags: nowait postinstall skipifsilent

[Code]
function IsDotNetInstalled(): Boolean;
var
  ResultCode: Integer;
begin
  // Check if .NET 8 Runtime is installed
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
WelcomeLabel2=This will install [name/ver] on your computer.%n%nThis tool helps automate Windows post-format setup by:%n%n• Backing up and restoring browser profiles%n• Installing development tools automatically%n• Optimizing Windows for better performance%n• Activating Windows%n%nIt is recommended that you close all other applications before continuing.

