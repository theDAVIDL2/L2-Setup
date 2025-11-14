; Note: This URL is for the fallback download method if Winget is not available.
#define DotNetUrl "https://download.visualstudio.microsoft.com/download/pr/d8cf1fe3-21c2-4baf-988f-f0152996135e/0c00b94713ee93e7ad5b4f82e2b86607/windowsdesktop-runtime-8.0.2-win-x64.exe"
#define DotNetFileName "dotnet8-desktop-runtime.exe"

[Setup]
AppName=L2 Setup
AppVersion=1.0.1
AppPublisher=L2 - theDAVIDL2
AppPublisherURL=https://github.com/theDAVIDL2/L2-Setup
AppSupportURL=https://github.com/theDAVIDL2/L2-Setup/issues
AppUpdatesURL=https://github.com/theDAVIDL2/L2-Setup/releases
DefaultDirName={autopf}\L2Setup
DefaultGroupName=L2 Setup
AllowNoIcons=yes
LicenseFile=LICENSE
OutputDir=Release
OutputBaseFilename=L2Setup-Installer
SetupIconFile=assets\icon.ico
Compression=lzma2/max
SolidCompression=yes
PrivilegesRequired=admin
PrivilegesRequiredOverridesAllowed=dialog
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
UninstallDisplayIcon={app}\L2Setup.exe
UninstallDisplayName=L2 Setup
VersionInfoVersion=1.0.1.0
VersionInfoCompany=L2
VersionInfoDescription=L2 Setup - All-in-One Windows Setup Tool
VersionInfoCopyright=Copyright (C) 2025 L2
WizardStyle=modern
DisableProgramGroupPage=yes
DisableWelcomePage=no

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "brazilianportuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "Release\L2Setup.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "assets\RunAsAdmin.bat"; DestDir: "{app}"; Flags: ignoreversion
; Note: L2Setup.exe is self-contained, no DLLs needed

[Icons]
Name: "{group}\L2 Setup"; Filename: "{app}\RunAsAdmin.bat"; WorkingDir: "{app}"; IconFilename: "{app}\L2Setup.exe"
Name: "{group}\{cm:UninstallProgram,L2 Setup}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\L2 Setup"; Filename: "{app}\RunAsAdmin.bat"; WorkingDir: "{app}"; IconFilename: "{app}\L2Setup.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\L2Setup.exe"; Description: "{cm:LaunchProgram,L2 Setup}"; Flags: nowait postinstall skipifsilent shellexec

[Code]
var
  DotNetInstallNeeded: Boolean;

function IsDotNetDesktopRuntimeInstalled(): Boolean;
var
  Version: string;
begin
  // Check for .NET Desktop Runtime 8.x (64-bit)
  if RegQueryStringValue(HKLM64, 'SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.WindowsDesktop.App', '8.0', Version) then
  begin
    Log('.NET 8 Desktop Runtime (x64) found. Version: ' + Version);
    Result := True;
    Exit;
  end;
  Log('.NET 8 Desktop Runtime (x64) not found in registry.');
  Result := False;
end;

procedure InitializeWizard();
begin
  DotNetInstallNeeded := not IsDotNetDesktopRuntimeInstalled();
  if DotNetInstallNeeded then
  begin
    Log('.NET 8 Desktop Runtime is not installed. The setup will attempt to install it.');
  end;
end;

function InstallDotNetWithWinget(var ResultCode: Integer): Boolean;
var
  WingetCmd: String;
begin
  Log('Attempting to install .NET 8 Desktop Runtime via Winget...');
  WizardForm.StatusLabel.Caption := 'Attempting to install .NET 8 via Winget...';
  WizardForm.ProgressGauge.Style := npbstMarquee;
  
  WingetCmd := 'install --id Microsoft.DotNet.DesktopRuntime.8 --silent --accept-package-agreements --accept-source-agreements';
  
  Result := Exec('winget', WingetCmd, '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
end;

function InstallDotNetManually(var ResultCode: Integer): Boolean;
var
  PowerShellCmd: String;
  InstallerPath: String;
begin
  Log('Winget not available or failed. Falling back to manual download...');
  WizardForm.StatusLabel.Caption := 'Downloading .NET 8 Desktop Runtime...';
  
  InstallerPath := ExpandConstant('{tmp}\{#DotNetFileName}');
  PowerShellCmd := 'Invoke-WebRequest -Uri "{#DotNetUrl}" -OutFile "' + InstallerPath + '"';

  // Download using PowerShell
  if not Exec('powershell.exe', '-NoProfile -Command "' + PowerShellCmd + '"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) or (ResultCode <> 0) then
  begin
    Log('PowerShell download failed. Exit code: ' + IntToStr(ResultCode));
    Result := False;
    Exit;
  end;

  Log('Download complete. Starting installer...');
  WizardForm.StatusLabel.Caption := 'Installing .NET 8 Desktop Runtime...';

  // Install the downloaded file
  Result := Exec(InstallerPath, '/install /quiet /norestart', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
end;

function NextButtonClick(CurPageID: Integer): Boolean;
var
  ResultCode: Integer;
begin
  Result := True;
  if (CurPageID = wpReady) and DotNetInstallNeeded then
  begin
    // Try Winget first
    if InstallDotNetWithWinget(ResultCode) and (ResultCode = 0) then
    begin
      Log('.NET 8 Desktop Runtime installed successfully via Winget.');
      DotNetInstallNeeded := False;
    end
    else
    begin
      // If Winget fails (doesn't exist or returns an error), fall back to manual install
      if not InstallDotNetManually(ResultCode) or ((ResultCode <> 0) and (ResultCode <> 3010)) then
      begin
        if MsgBox('Failed to automatically install the .NET 8 Desktop Runtime.' + #13#10 + #13#10 +
               'Do you want to open the download page to install it manually?', mbError, MB_YESNO) = IDYES then
        begin
          ShellExec('open', 'https://dotnet.microsoft.com/download/dotnet/8.0', '', '', SW_SHOWNORMAL, ewNoWait, ResultCode);
        end;
        Result := False; // Stop the installation
      end
      else
      begin
        Log('.NET 8 Desktop Runtime installed successfully via manual download. Exit code: ' + IntToStr(ResultCode));
        DotNetInstallNeeded := False;
      end;
    end;
    WizardForm.ProgressGauge.Style := npbstNormal;
  end;
end;

function UpdateReadyMemo(Space, NewLine, MemoUserInfoInfo, MemoDirInfo, MemoTypeInfo, MemoComponentsInfo, MemoGroupInfo, MemoTasksInfo: String): String;
var
  S: String;
begin
  S := '';
  if DotNetInstallNeeded then
    S := S + 'The following dependency is required and will be installed automatically:' + NewLine +
           '  - .NET 8 Desktop Runtime (x64)' + NewLine + NewLine;
  S := S + MemoDirInfo + NewLine + NewLine;
  if MemoTasksInfo <> '' then
    S := S + MemoTasksInfo + NewLine + NewLine;
  Result := S;
end;
