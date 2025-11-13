; Windows Post-Format Setup Tool - Inno Setup Script
; Requires Inno Setup 6.x or later

#define DotNetUrl "https://download.visualstudio.microsoft.com/download/pr/d8cf1fe3-21c2-4baf-988f-f0152996135e/0c00b94713ee93e7ad5b4f82e2b86607/windowsdesktop-runtime-8.0.2-win-x64.exe"
#define DotNetFileName "dotnet8-runtime.exe"

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
  DotNetDownloadPage: TDownloadWizardPage;

function IsDotNetInstalled(): Boolean;
var
  Success: Boolean;
  ResultCode: Integer;
begin
  // Check via PowerShell for .NET 8 Runtime
  Success := Exec('powershell.exe', 
    '-NoProfile -Command "if (Test-Path ''C:\Program Files\dotnet\shared\Microsoft.WindowsDesktop.App\8.*'') { exit 0 } else { exit 1 }"',
    '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
  
  Result := Success and (ResultCode = 0);
end;

procedure InitializeWizard();
begin
  DotNetInstallNeeded := not IsDotNetInstalled();
  
  if DotNetInstallNeeded then
  begin
    // Create download page
    DotNetDownloadPage := CreateDownloadPage(SetupMessage(msgWizardPreparing), SetupMessage(msgPreparingDesc), nil);
  end;
end;

function NextButtonClick(CurPageID: Integer): Boolean;
var
  ResultCode: Integer;
  DotNetInstaller: String;
begin
  Result := True;

  if (CurPageID = wpReady) and DotNetInstallNeeded then
  begin
    DotNetDownloadPage.Clear;
    DotNetDownloadPage.Add('{#DotNetUrl}', '{#DotNetFileName}', '');
    DotNetDownloadPage.Show;
    
    try
      try
        DotNetDownloadPage.Download;
        Result := True;
      except
        if DotNetDownloadPage.AbortedByUser then
          Log('Download aborted by user.')
        else
          SuppressibleMsgBox(AddPeriod(GetExceptionMessage), mbCriticalError, MB_OK, IDOK);
        Result := False;
      end;
    finally
      DotNetDownloadPage.Hide;
    end;

    if Result then
    begin
      // Install .NET 8 Runtime
      DotNetInstaller := ExpandConstant('{tmp}\{#DotNetFileName}');
      
      if FileExists(DotNetInstaller) then
      begin
        MsgBox('.NET 8 Runtime will now be installed.' + #13#10 + 
               'This is required for the application to run.' + #13#10#13#10 +
               'Please wait...', mbInformation, MB_OK);
        
        if Exec(DotNetInstaller, '/install /quiet /norestart', '', SW_SHOW, ewWaitUntilTerminated, ResultCode) then
        begin
          if ResultCode = 0 then
          begin
            MsgBox('.NET 8 Runtime installed successfully!', mbInformation, MB_OK);
            DotNetInstallNeeded := False;
          end
          else if ResultCode = 3010 then
          begin
            MsgBox('.NET 8 Runtime installed successfully!' + #13#10 + 
                   'A system restart is recommended after installation completes.', mbInformation, MB_OK);
            DotNetInstallNeeded := False;
          end
          else
          begin
            MsgBox('.NET 8 Runtime installation failed with code: ' + IntToStr(ResultCode) + #13#10 +
                   'The application may not work correctly.', mbError, MB_OK);
            Result := True; // Continue anyway
          end;
        end
        else
        begin
          MsgBox('Failed to start .NET 8 Runtime installer.' + #13#10 +
                 'The application may not work correctly.', mbError, MB_OK);
          Result := True; // Continue anyway
        end;
      end;
    end;
  end;
end;

function PrepareToInstall(var NeedsRestart: Boolean): String;
begin
  Result := '';
end;

function UpdateReadyMemo(Space, NewLine, MemoUserInfoInfo, MemoDirInfo, MemoTypeInfo, MemoComponentsInfo, MemoGroupInfo, MemoTasksInfo: String): String;
var
  S: String;
begin
  S := '';
  
  if DotNetInstallNeeded then
    S := S + '.NET 8 Runtime will be downloaded and installed automatically.' + NewLine + NewLine;
    
  S := S + MemoDirInfo + NewLine + NewLine;
  
  if MemoTasksInfo <> '' then
    S := S + MemoTasksInfo + NewLine + NewLine;
    
  Result := S;
end;

[Messages]
WelcomeLabel2=This will install [name/ver] on your computer.%n%nL2 Setup is an All-in-One tool that automates:%n%n• Browser profile backup/restore%n• 44+ development tools installation%n• 30+ runtimes (VC++, .NET, DirectX, Java)%n• Windows optimization (customizable)%n• GPU driver detection & installation%n• Windows activation%n%nIt is recommended that you close all other applications before continuing.
