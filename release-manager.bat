@echo off
chcp 65001 >nul 2>&1
setlocal enabledelayedexpansion enableextensions
color 0B

:: ================================================================
:: Universal Release Manager - Configurable Edition
:: Version: 2.0.0
:: 
:: Configuration file: release-config.json
:: ================================================================

:: Check for configuration file
if not exist "release-config.json" (
    cls
    echo.
    echo ╔════════════════════════════════════════════════════════════╗
    echo ║                                                            ║
    echo ║              ⚠️  CONFIGURATION FILE NOT FOUND              ║
    echo ║                                                            ║
    echo ╚════════════════════════════════════════════════════════════╝
    echo.
    echo ❌ Error: release-config.json not found!
    echo.
    echo 💡 First time setup:
    echo    1. Copy release-config.example.json to release-config.json
    echo    2. Edit release-config.json with your project details
    echo    3. Run this script again
    echo.
    echo 📝 Quick setup command:
    echo    copy release-config.example.json release-config.json
    echo.
    
    set /p create="Create config from example now? (Y/N): "
    if /i "!create!"=="Y" (
        if exist "release-config.example.json" (
            copy release-config.example.json release-config.json >nul
            echo.
            echo ✅ Configuration file created!
            echo    Please edit release-config.json with your project details.
            echo.
            pause
            start notepad release-config.json
            exit /b 1
        ) else (
            echo.
            echo ❌ Example file not found!
            echo.
        )
    )
    pause
    exit /b 1
)

:: Load configuration using PowerShell
echo Loading configuration...
for /f "usebackq delims=" %%i in (`powershell -NoProfile -ExecutionPolicy Bypass -Command "$config = Get-Content 'release-config.json' -Raw | ConvertFrom-Json; Write-Output $config.project.name"`) do set "PROJECT_NAME=%%i"
for /f "usebackq delims=" %%i in (`powershell -NoProfile -ExecutionPolicy Bypass -Command "$config = Get-Content 'release-config.json' -Raw | ConvertFrom-Json; Write-Output $config.project.version"`) do set "PROJECT_VERSION=%%i"
for /f "usebackq delims=" %%i in (`powershell -NoProfile -ExecutionPolicy Bypass -Command "$config = Get-Content 'release-config.json' -Raw | ConvertFrom-Json; Write-Output $config.repository.url"`) do set "REPO_URL=%%i"
for /f "usebackq delims=" %%i in (`powershell -NoProfile -ExecutionPolicy Bypass -Command "$config = Get-Content 'release-config.json' -Raw | ConvertFrom-Json; Write-Output $config.repository.owner"`) do set "REPO_OWNER=%%i"
for /f "usebackq delims=" %%i in (`powershell -NoProfile -ExecutionPolicy Bypass -Command "$config = Get-Content 'release-config.json' -Raw | ConvertFrom-Json; Write-Output $config.repository.name"`) do set "REPO_NAME=%%i"
for /f "usebackq delims=" %%i in (`powershell -NoProfile -ExecutionPolicy Bypass -Command "$config = Get-Content 'release-config.json' -Raw | ConvertFrom-Json; Write-Output $config.repository.defaultBranch"`) do set "DEFAULT_BRANCH=%%i"
for /f "usebackq delims=" %%i in (`powershell -NoProfile -ExecutionPolicy Bypass -Command "$config = Get-Content 'release-config.json' -Raw | ConvertFrom-Json; Write-Output $config.build.solutionFile"`) do set "SOLUTION_FILE=%%i"
for /f "usebackq delims=" %%i in (`powershell -NoProfile -ExecutionPolicy Bypass -Command "$config = Get-Content 'release-config.json' -Raw | ConvertFrom-Json; Write-Output $config.build.projectDir"`) do set "PROJECT_DIR=%%i"
for /f "usebackq delims=" %%i in (`powershell -NoProfile -ExecutionPolicy Bypass -Command "$config = Get-Content 'release-config.json' -Raw | ConvertFrom-Json; Write-Output $config.build.projectFile"`) do set "PROJECT_FILE=%%i"
for /f "usebackq delims=" %%i in (`powershell -NoProfile -ExecutionPolicy Bypass -Command "$config = Get-Content 'release-config.json' -Raw | ConvertFrom-Json; Write-Output $config.build.outputDir"`) do set "OUTPUT_DIR=%%i"
for /f "usebackq delims=" %%i in (`powershell -NoProfile -ExecutionPolicy Bypass -Command "$config = Get-Content 'release-config.json' -Raw | ConvertFrom-Json; Write-Output $config.build.installerScript"`) do set "INNO_SETUP_SCRIPT=%%i"
for /f "usebackq delims=" %%i in (`powershell -NoProfile -ExecutionPolicy Bypass -Command "$config = Get-Content 'release-config.json' -Raw | ConvertFrom-Json; Write-Output $config.advanced.tempDir"`) do set "TEMP_DIR=%%i"
for /f "usebackq delims=" %%i in (`powershell -NoProfile -ExecutionPolicy Bypass -Command "$config = Get-Content 'release-config.json' -Raw | ConvertFrom-Json; Write-Output $config.advanced.logFile"`) do set "LOG_FILE=%%i"

:: Create temp directory if it doesn't exist
if not exist "%TEMP_DIR%" mkdir "%TEMP_DIR%"

:: Set title with project name
title %PROJECT_NAME% - Release Manager v2.0

:MENU
cls
echo.
echo ╔════════════════════════════════════════════════════════════╗
echo ║                                                            ║
echo ║        %PROJECT_NAME% - Release Manager v2.0              ║
echo ║                  Universal Configurable Edition            ║
echo ║                                                            ║
echo ╚════════════════════════════════════════════════════════════╝
echo.
echo 📦 Project: %PROJECT_NAME%
echo 🔗 Repository: %REPO_URL%
echo 📌 Version: %PROJECT_VERSION%
echo.
echo ════════════════════════════════════════════════════════════
echo.
echo  [1] 📤 Quick Push (Git Add + Commit + Push)
echo  [2] 🏗️  Build Project (Debug)
echo  [3] 📦 Build Release (Production)
echo  [4] 🎁 Create Installer (Inno Setup)
echo  [5] 🚀 Full Release (Build + Installer + Push)
echo  [6] 🏷️  Create GitHub Release Tag
echo  [7] 🤖 Automated Release (GitHub Actions)
echo  [8] 📊 Check Project Status
echo  [9] 🧹 Clean Build Artifacts
echo  [10] 🔧 Advanced Options
echo  [11] ⚙️  Edit Configuration
echo  [0] ❌ Exit
echo.
echo ════════════════════════════════════════════════════════════
echo.

set /p choice="Enter your choice (0-11): "

if "%choice%"=="1" goto QUICK_PUSH
if "%choice%"=="2" goto BUILD_DEBUG
if "%choice%"=="3" goto BUILD_RELEASE
if "%choice%"=="4" goto CREATE_INSTALLER
if "%choice%"=="5" goto FULL_RELEASE
if "%choice%"=="6" goto CREATE_RELEASE_TAG
if "%choice%"=="7" goto AUTOMATED_RELEASE
if "%choice%"=="8" goto CHECK_STATUS
if "%choice%"=="9" goto CLEAN_BUILD
if "%choice%"=="10" goto ADVANCED_OPTIONS
if "%choice%"=="11" goto EDIT_CONFIG
if "%choice%"=="0" goto EXIT
echo Invalid choice. Please try again.
timeout /t 2 >nul
goto MENU

:: ================================================================
:: EDIT CONFIGURATION
:: ================================================================
:EDIT_CONFIG
cls
echo.
echo ╔════════════════════════════════════════════════════════════╗
echo ║                  Edit Configuration ⚙️                     ║
echo ╚════════════════════════════════════════════════════════════╝
echo.
echo Opening release-config.json in notepad...
echo.
start notepad release-config.json
echo.
echo 💡 After editing, save and close notepad.
echo    Changes will be loaded on next menu access.
echo.
pause
goto MENU

:: ================================================================
:: LOGGING FUNCTION
:: ================================================================
:LOG
echo [%date% %time%] %~1 >> "%LOG_FILE%"
goto :eof

:: ================================================================
:: QUICK PUSH TO GITHUB
:: ================================================================
:QUICK_PUSH
cls
call :LOG "Quick Push started"
echo.
echo ╔════════════════════════════════════════════════════════════╗
echo ║                  Quick Push to GitHub 📤                   ║
echo ╚════════════════════════════════════════════════════════════╝
echo.
echo 🔗 Repository: %REPO_URL%
echo 🌿 Default Branch: %DEFAULT_BRANCH%
echo.

:: Check if git is installed
git --version >nul 2>&1
if errorlevel 1 (
    call :LOG "ERROR: Git not installed"
    echo ❌ ERROR: Git is not installed or not in PATH!
    echo.
    echo 💡 Please install Git from: https://git-scm.com/downloads
    echo.
    pause
    goto MENU
)

:: Check if we're in a git repository
git rev-parse --git-dir >nul 2>&1
if errorlevel 1 (
    call :LOG "ERROR: Not a git repository"
    echo ❌ ERROR: Not a git repository!
    echo.
    echo 💡 Initialize a git repository first:
    echo    git init
    echo    git remote add origin %REPO_URL%
    echo.
    pause
    goto MENU
)

:: Show current git status
echo ┌────────────────────────────────────────────────────────────┐
echo │                    Current Git Status                      │
echo └────────────────────────────────────────────────────────────┘
echo.
git status --short
echo.

:: Get current branch
for /f "tokens=*" %%a in ('git rev-parse --abbrev-ref HEAD 2^>nul') do set "CURRENT_BRANCH=%%a"
echo 🌿 Current Branch: %CURRENT_BRANCH%
echo.

:: Verify remote URL matches config
for /f "tokens=*" %%a in ('git remote get-url origin 2^>nul') do set "REMOTE_URL=%%a"
if not "!REMOTE_URL!"=="%REPO_URL%" (
    echo ⚠️  WARNING: Remote URL doesn't match configuration!
    echo    Config:  %REPO_URL%
    echo    Remote:  !REMOTE_URL!
    echo.
    set /p update="Update remote URL to match config? (Y/N): "
    if /i "!update!"=="Y" (
        git remote set-url origin %REPO_URL%
        call :LOG "Updated remote URL to %REPO_URL%"
        echo    ✅ Remote URL updated
        echo.
    )
)

echo ════════════════════════════════════════════════════════════
echo.

:: Select commit type
:SELECT_COMMIT_TYPE
echo ┌────────────────────────────────────────────────────────────┐
echo │                    Select Commit Type                      │
echo └────────────────────────────────────────────────────────────┘
echo.
echo  [1] ✨ feat      - New feature
echo  [2] 🐛 fix       - Bug fix
echo  [3] 📚 docs      - Documentation changes
echo  [4] 💎 style     - Code style/formatting
echo  [5] ♻️  refactor - Code refactoring
echo  [6] ✅ test      - Adding/updating tests
echo  [7] 🔧 chore     - Maintenance tasks
echo  [8] ⚡ perf      - Performance improvements
echo  [9] 🎨 ui        - UI/UX improvements
echo  [10] 🚀 release  - Release version
echo  [0] 🔙 Back to Main Menu
echo.

set /p commit_type="Enter commit type (0-10): "

if "%commit_type%"=="1" set "COMMIT_PREFIX=feat"
if "%commit_type%"=="2" set "COMMIT_PREFIX=fix"
if "%commit_type%"=="3" set "COMMIT_PREFIX=docs"
if "%commit_type%"=="4" set "COMMIT_PREFIX=style"
if "%commit_type%"=="5" set "COMMIT_PREFIX=refactor"
if "%commit_type%"=="6" set "COMMIT_PREFIX=test"
if "%commit_type%"=="7" set "COMMIT_PREFIX=chore"
if "%commit_type%"=="8" set "COMMIT_PREFIX=perf"
if "%commit_type%"=="9" set "COMMIT_PREFIX=ui"
if "%commit_type%"=="10" set "COMMIT_PREFIX=release"
if "%commit_type%"=="0" goto MENU

if not defined COMMIT_PREFIX (
    echo.
    echo ❌ Invalid choice. Please try again.
    echo.
    timeout /t 2 >nul
    goto SELECT_COMMIT_TYPE
)

echo.
echo ════════════════════════════════════════════════════════════
echo.

:: Get commit message
echo ┌────────────────────────────────────────────────────────────┐
echo │                    Commit Message                          │
echo └────────────────────────────────────────────────────────────┘
echo.
echo 💡 Enter a clear, descriptive message
echo    Example: "add user authentication system"
echo.

set /p "COMMIT_MSG=📝 Message: "

if not defined COMMIT_MSG (
    echo.
    echo ❌ Commit message cannot be empty!
    echo.
    pause
    goto QUICK_PUSH
)

:: Build full commit message
set "FULL_COMMIT_MSG=%COMMIT_PREFIX%: %COMMIT_MSG%"

echo.
echo ════════════════════════════════════════════════════════════
echo.

:: Show summary
echo ┌────────────────────────────────────────────────────────────┐
echo │                    Commit Summary                          │
echo └────────────────────────────────────────────────────────────┘
echo.
echo 🌿 Branch:     %CURRENT_BRANCH%
echo 🔗 Repository: %REPO_URL%
echo 📦 Type:       %COMMIT_PREFIX%
echo 📝 Message:    %FULL_COMMIT_MSG%
echo.
echo ⚠️  This will execute:
echo    1. git add .
echo    2. git commit -m "%FULL_COMMIT_MSG%"
echo    3. git push origin %CURRENT_BRANCH%
echo.

set /p confirm="✅ Confirm and push? (Y/N): "
if /i not "%confirm%"=="Y" (
    call :LOG "Quick Push cancelled by user"
    echo.
    echo ❌ Operation cancelled.
    echo.
    pause
    goto MENU
)

echo.
echo ════════════════════════════════════════════════════════════
echo.

:: Execute git workflow
echo ┌────────────────────────────────────────────────────────────┐
echo │                    Executing Git Push                      │
echo └────────────────────────────────────────────────────────────┘
echo.

:: Step 1: Add all files
echo [1/3] 📂 Adding all files...
call :LOG "Adding files: git add ."
git add .
if errorlevel 1 (
    call :LOG "ERROR: Failed to add files"
    echo    ❌ Failed to add files!
    echo.
    pause
    goto MENU
)
echo      ✅ Files added successfully
echo.

:: Step 2: Commit
echo [2/3] 💾 Committing changes...
call :LOG "Committing: %FULL_COMMIT_MSG%"
git commit -m "%FULL_COMMIT_MSG%"
if errorlevel 1 (
    call :LOG "ERROR: Commit failed"
    echo    ❌ Commit failed!
    echo.
    echo 💡 This might happen if there are no changes to commit.
    echo.
    pause
    goto MENU
)
echo      ✅ Commit created successfully
echo.

:: Step 3: Push
echo [3/3] 🚀 Pushing to remote...
call :LOG "Pushing to origin %CURRENT_BRANCH%"
git push origin %CURRENT_BRANCH%
if errorlevel 1 (
    call :LOG "ERROR: Push failed, trying with --set-upstream"
    echo    ❌ Push failed!
    echo.
    echo 💡 Trying with --set-upstream flag...
    git push --set-upstream origin %CURRENT_BRANCH%
    if errorlevel 1 (
        call :LOG "ERROR: Push failed completely"
        echo    ❌ Still failed. Please check your git configuration.
        pause
        goto MENU
    )
    echo    ✅ Push successful with upstream!
) else (
    echo      ✅ Push completed successfully
)

echo.
echo ╔════════════════════════════════════════════════════════════╗
echo ║              Git Push Completed Successfully! 🎉           ║
echo ╚════════════════════════════════════════════════════════════╝
echo.
echo 📤 Commit: %FULL_COMMIT_MSG%
echo 🌿 Branch: %CURRENT_BRANCH%
echo ✅ Status: Pushed to remote
echo.
echo 🔗 Repository: %REPO_URL%
echo.

call :LOG "Quick Push completed successfully"
pause
goto MENU

:: ================================================================
:: BUILD DEBUG
:: ================================================================
:BUILD_DEBUG
cls
call :LOG "Build Debug started"
echo.
echo ╔════════════════════════════════════════════════════════════╗
echo ║                  Building Debug Version...                 ║
echo ╚════════════════════════════════════════════════════════════╝
echo.

call :CHECK_DOTNET
if errorlevel 1 goto MENU

echo 🔨 Restoring NuGet packages...
call :LOG "Restoring packages for %SOLUTION_FILE%"
dotnet restore "%SOLUTION_FILE%"
if errorlevel 1 (
    call :LOG "ERROR: Failed to restore packages"
    echo ❌ Failed to restore packages!
    pause
    goto MENU
)
echo    ✅ Packages restored
echo.

echo 🏗️  Building project (Debug)...
call :LOG "Building Debug configuration"
dotnet build "%SOLUTION_FILE%" --configuration Debug --no-restore
if errorlevel 1 (
    call :LOG "ERROR: Debug build failed"
    echo ❌ Build failed!
    pause
    goto MENU
)

echo.
echo ╔════════════════════════════════════════════════════════════╗
echo ║              Debug Build Completed! ✅                     ║
echo ╚════════════════════════════════════════════════════════════╝
echo.
echo 📁 Output: %PROJECT_DIR%\bin\Debug\
echo.

call :LOG "Debug build completed successfully"
pause
goto MENU

:: ================================================================
:: BUILD RELEASE
:: ================================================================
:BUILD_RELEASE
cls
call :LOG "Build Release started"
echo.
echo ╔════════════════════════════════════════════════════════════╗
echo ║                Building Release Version...                 ║
echo ╚════════════════════════════════════════════════════════════╝
echo.

call :CHECK_DOTNET
if errorlevel 1 goto MENU

echo 🔨 Restoring NuGet packages...
call :LOG "Restoring packages"
dotnet restore "%SOLUTION_FILE%"
if errorlevel 1 (
    call :LOG "ERROR: Failed to restore packages"
    echo ❌ Failed to restore packages!
    pause
    goto MENU
)
echo    ✅ Packages restored
echo.

echo 🏗️  Building project (Release)...
call :LOG "Building Release configuration"
dotnet build "%SOLUTION_FILE%" --configuration Release --no-restore
if errorlevel 1 (
    call :LOG "ERROR: Release build failed"
    echo ❌ Build failed!
    pause
    goto MENU
)

echo.
echo 📦 Publishing self-contained executable...
call :LOG "Publishing to %OUTPUT_DIR%"
dotnet publish "%PROJECT_FILE%" ^
    --configuration Release ^
    --runtime win-x64 ^
    --self-contained true ^
    --output "%OUTPUT_DIR%" ^
    /p:PublishSingleFile=true ^
    /p:IncludeNativeLibrariesForSelfExtract=true
if errorlevel 1 (
    call :LOG "ERROR: Publish failed"
    echo ❌ Publish failed!
    pause
    goto MENU
)

echo.
echo ╔════════════════════════════════════════════════════════════╗
echo ║              Release Build Completed! ✅                   ║
echo ╚════════════════════════════════════════════════════════════╝
echo.
echo 📁 Output Directory: %OUTPUT_DIR%\
echo 📦 Executable: Ready for distribution
echo.
dir "%OUTPUT_DIR%\*.exe" /B 2>nul
echo.

call :LOG "Release build completed successfully"
pause
goto MENU

:: ================================================================
:: CREATE INSTALLER
:: ================================================================
:CREATE_INSTALLER
cls
call :LOG "Create Installer started"
echo.
echo ╔════════════════════════════════════════════════════════════╗
echo ║              Creating Installer with Inno Setup            ║
echo ╚════════════════════════════════════════════════════════════╝
echo.

:: Check if Inno Setup is installed
set "ISCC="
if exist "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" (
    set "ISCC=C:\Program Files (x86)\Inno Setup 6\ISCC.exe"
)
if exist "C:\Program Files\Inno Setup 6\ISCC.exe" (
    set "ISCC=C:\Program Files\Inno Setup 6\ISCC.exe"
)

if not defined ISCC (
    call :LOG "ERROR: Inno Setup not found"
    echo ❌ ERROR: Inno Setup not found!
    echo.
    echo 💡 Please install Inno Setup 6 from:
    echo    https://jrsoftware.org/isdl.php
    echo.
    pause
    goto MENU
)

echo ✅ Found Inno Setup: %ISCC%
echo.

:: Check if Release build exists
if not exist "%OUTPUT_DIR%\*.exe" (
    call :LOG "WARNING: Release build not found"
    echo ⚠️  Release build not found!
    echo.
    set /p build="Build release first? (Y/N): "
    if /i "!build!"=="Y" (
        call :BUILD_RELEASE_SILENT
        if errorlevel 1 (
            echo ❌ Build failed!
            pause
            goto MENU
        )
    ) else (
        goto MENU
    )
)

echo 📦 Compiling installer script...
call :LOG "Compiling installer: %INNO_SETUP_SCRIPT%"
"%ISCC%" "%INNO_SETUP_SCRIPT%"
if errorlevel 1 (
    call :LOG "ERROR: Installer compilation failed"
    echo ❌ Installer compilation failed!
    pause
    goto MENU
)

echo.
echo ╔════════════════════════════════════════════════════════════╗
echo ║           Installer Created Successfully! 🎁               ║
echo ╚════════════════════════════════════════════════════════════╝
echo.
echo 📁 Installer Location: Output\
dir "Output\*.exe" /B 2>nul
echo.
echo 💡 Installer is ready for distribution!
echo.

call :LOG "Installer created successfully"
pause
goto MENU

:: ================================================================
:: FULL RELEASE WORKFLOW
:: ================================================================
:FULL_RELEASE
cls
call :LOG "Full Release workflow started"
echo.
echo ╔════════════════════════════════════════════════════════════╗
echo ║                Full Release Workflow 🚀                    ║
echo ╚════════════════════════════════════════════════════════════╝
echo.
echo This will perform:
echo   1. Clean build artifacts
echo   2. Build release version
echo   3. Create installer
echo   4. Commit and push to GitHub
echo.
set /p version="Enter version number (e.g., %PROJECT_VERSION%): "
if not defined version set "version=%PROJECT_VERSION%"

echo.
echo ⚠️  WARNING: This will execute the full release workflow!
echo.
set /p confirm="Continue? (YES to confirm): "
if /i not "%confirm%"=="YES" (
    call :LOG "Full Release cancelled by user"
    goto MENU
)

echo.
echo ════════════════════════════════════════════════════════════
echo.

:: Step 1: Clean
echo [1/4] 🧹 Cleaning build artifacts...
call :CLEAN_BUILD_SILENT
call :LOG "Clean completed"
echo      ✅ Clean complete
echo.

:: Step 2: Build Release
echo [2/4] 🏗️  Building release...
call :BUILD_RELEASE_SILENT
if errorlevel 1 (
    call :LOG "ERROR: Build failed in full release"
    echo ❌ Build failed!
    pause
    goto MENU
)
call :LOG "Build completed"
echo      ✅ Build complete
echo.

:: Step 3: Create Installer
echo [3/4] 📦 Creating installer...
if defined ISCC (
    "%ISCC%" "%INNO_SETUP_SCRIPT%" >nul 2>&1
    if errorlevel 1 (
        call :LOG "WARNING: Installer creation failed"
        echo      ⚠️  Installer creation failed, continuing...
    ) else (
        call :LOG "Installer created"
        echo      ✅ Installer created
    )
) else (
    echo      ⚠️  Inno Setup not found, skipping installer
)
echo.

:: Step 4: Git Push
echo [4/4] 📤 Pushing to GitHub...
call :LOG "Committing release: %version%"
git add .
git commit -m "release: version %version%"
git push origin %DEFAULT_BRANCH%
if errorlevel 1 (
    git push --set-upstream origin %DEFAULT_BRANCH%
)
call :LOG "Pushed to GitHub"
echo      ✅ Pushed to GitHub
echo.

echo ╔════════════════════════════════════════════════════════════╗
echo ║         Full Release Workflow Completed! 🎉                ║
echo ╚════════════════════════════════════════════════════════════╝
echo.
echo 📦 Version: %version%
echo 🔗 Repository: %REPO_URL%
echo.
echo 💡 Next steps:
echo    1. Create GitHub release tag (Option 6)
echo    2. Upload installer to GitHub release
echo.

call :LOG "Full Release completed successfully"
pause
goto MENU

:: ================================================================
:: CREATE GITHUB RELEASE TAG
:: ================================================================
:CREATE_RELEASE_TAG
cls
call :LOG "Create Release Tag started"
echo.
echo ╔════════════════════════════════════════════════════════════╗
echo ║              Create GitHub Release Tag 🏷️                  ║
echo ╚════════════════════════════════════════════════════════════╝
echo.

call :CHECK_GIT
if errorlevel 1 goto MENU

echo Enter release information:
echo.
set /p tag_version="Version (e.g., v%PROJECT_VERSION%): "
if not defined tag_version set "tag_version=v%PROJECT_VERSION%"

set /p tag_message="Release message: "
if not defined tag_message set "tag_message=Release %tag_version%"

echo.
echo ════════════════════════════════════════════════════════════
echo.
echo 📋 Release Summary:
echo    🏷️  Tag: %tag_version%
echo    📝 Message: %tag_message%
echo    🔗 Repository: %REPO_URL%
echo.
set /p confirm="Create and push tag? (Y/N): "
if /i not "%confirm%"=="Y" (
    call :LOG "Create tag cancelled"
    goto MENU
)

echo.
echo 🏷️  Creating tag...
call :LOG "Creating tag: %tag_version%"
git tag -a "%tag_version%" -m "%tag_message%"
if errorlevel 1 (
    call :LOG "ERROR: Failed to create tag"
    echo ❌ Failed to create tag!
    pause
    goto MENU
)
echo    ✅ Tag created
echo.

echo 📤 Pushing tag to GitHub...
call :LOG "Pushing tag to GitHub"
git push origin "%tag_version%"
if errorlevel 1 (
    call :LOG "ERROR: Failed to push tag"
    echo ❌ Failed to push tag!
    pause
    goto MENU
)
echo    ✅ Tag pushed
echo.

echo ╔════════════════════════════════════════════════════════════╗
echo ║           GitHub Release Tag Created! 🎉                   ║
echo ╚════════════════════════════════════════════════════════════╝
echo.
echo 🏷️  Tag: %tag_version%
echo 🔗 GitHub: %REPO_URL%/releases/tag/%tag_version%
echo.
echo 💡 Next steps:
echo    1. Go to: %REPO_URL%/releases
echo    2. Edit the release
echo    3. Upload installer from Output\ folder
echo    4. Publish the release
echo.

call :LOG "Release tag created successfully: %tag_version%"
pause
goto MENU

:: ================================================================
:: AUTOMATED RELEASE (GitHub Actions)
:: ================================================================
:AUTOMATED_RELEASE
cls
call :LOG "Automated Release started"
echo.
echo ╔════════════════════════════════════════════════════════════╗
echo ║        🤖 Automated Release (GitHub Actions) 🤖            ║
echo ╚════════════════════════════════════════════════════════════╝
echo.
echo 📝 This will create a release using GitHub Actions workflow.
echo.
echo ⚙️  How it works:
echo    1. Creates a version tag (e.g., v%PROJECT_VERSION%)
echo    2. Pushes the tag to GitHub
echo    3. GitHub Actions automatically:
echo       • Builds the project
echo       • Creates the installer
echo       • Publishes the release
echo       • Uploads both executable and installer
echo.
echo ════════════════════════════════════════════════════════════
echo.

call :CHECK_GIT
if errorlevel 1 goto MENU

:: Check if current branch is main/master
for /f "usebackq delims=" %%i in (`git rev-parse --abbrev-ref HEAD`) do set "current_branch=%%i"
if not "%current_branch%"=="%DEFAULT_BRANCH%" (
    echo ⚠️  WARNING: You're on branch '%current_branch%'
    echo    Releases should be created from '%DEFAULT_BRANCH%' branch.
    echo.
    set /p switch_branch="Switch to %DEFAULT_BRANCH% branch? (Y/N): "
    if /i "!switch_branch!"=="Y" (
        git checkout %DEFAULT_BRANCH%
        if errorlevel 1 (
            echo ❌ Failed to switch branch!
            pause
            goto MENU
        )
        git pull origin %DEFAULT_BRANCH%
    ) else (
        echo Continuing on current branch...
    )
    echo.
)

:: Check if there are uncommitted changes
git diff --quiet
if errorlevel 1 (
    echo ⚠️  You have uncommitted changes!
    echo.
    set /p commit_now="Commit changes now? (Y/N): "
    if /i "!commit_now!"=="Y" (
        echo.
        set /p commit_msg="Commit message: "
        if not defined commit_msg set "commit_msg=Pre-release commit"
        
        git add .
        git commit -m "!commit_msg!"
        if errorlevel 1 (
            echo ❌ Commit failed!
            pause
            goto MENU
        )
        
        git push origin %current_branch%
        if errorlevel 1 (
            echo ❌ Push failed!
            pause
            goto MENU
        )
        echo ✅ Changes committed and pushed!
        echo.
    ) else (
        echo ⚠️  Proceeding with uncommitted changes...
        echo.
    )
)

echo Enter release information:
echo.
set /p tag_version="Version tag (e.g., v%PROJECT_VERSION%): "
if not defined tag_version set "tag_version=v%PROJECT_VERSION%"

set /p tag_message="Release message: "
if not defined tag_message set "tag_message=Release %tag_version%"

echo.
echo ════════════════════════════════════════════════════════════
echo.
echo 📋 Automated Release Summary:
echo    🏷️  Tag: %tag_version%
echo    📝 Message: %tag_message%
echo    🌿 Branch: %current_branch%
echo    🔗 Repository: %REPO_URL%
echo.
echo 🤖 What will happen:
echo    1. Git tag created: %tag_version%
echo    2. Tag pushed to GitHub
echo    3. GitHub Actions workflow triggered
echo    4. Automated build process starts
echo    5. Release created with artifacts
echo.
set /p confirm="Start automated release? (Y/N): "
if /i not "%confirm%"=="Y" (
    call :LOG "Automated release cancelled"
    goto MENU
)

echo.
echo ════════════════════════════════════════════════════════════
echo.
echo 🏷️  Creating tag...
call :LOG "Creating tag: %tag_version%"
git tag -a "%tag_version%" -m "%tag_message%"
if errorlevel 1 (
    call :LOG "ERROR: Failed to create tag"
    echo ❌ Failed to create tag!
    echo.
    echo 💡 Possible reasons:
    echo    • Tag already exists
    echo    • Invalid tag name
    echo.
    pause
    goto MENU
)
echo    ✅ Tag created locally
echo.

echo 📤 Pushing tag to GitHub...
call :LOG "Pushing tag to GitHub"
git push origin "%tag_version%"
if errorlevel 1 (
    call :LOG "ERROR: Failed to push tag"
    echo ❌ Failed to push tag to GitHub!
    echo.
    echo 💡 Possible reasons:
    echo    • Network issue
    echo    • Authentication problem
    echo    • Remote tag already exists
    echo.
    echo Cleaning up local tag...
    git tag -d "%tag_version%" >nul 2>&1
    pause
    goto MENU
)

call :LOG "Tag pushed successfully - GitHub Actions triggered"
echo    ✅ Tag pushed successfully!
echo.
echo ════════════════════════════════════════════════════════════
echo.
echo 🎉 AUTOMATED RELEASE TRIGGERED!
echo.
echo 🤖 GitHub Actions is now building your release...
echo.
echo ⏱️  Expected time: 5-10 minutes
echo.
echo 📊 Monitor progress:
echo    🔗 Actions: %REPO_URL%/actions
echo.
echo 📦 When complete, release will be available at:
echo    🔗 Releases: %REPO_URL%/releases/tag/%tag_version%
echo.
echo 📥 The release will include:
echo    • L2Setup-Installer-%tag_version%.exe (~49 MB)
echo    • L2Setup-%tag_version%.exe (~166 MB)
echo    • Release notes (auto-generated)
echo.
echo ════════════════════════════════════════════════════════════
echo.
echo 💡 Next steps:
echo    1. Wait for GitHub Actions to complete (5-10 min)
echo    2. Check %REPO_URL%/actions for status
echo    3. Download and test the release artifacts
echo    4. Update CHANGELOG.md if needed
echo    5. Share the release link!
echo.
set /p open_actions="Open GitHub Actions page in browser? (Y/N): "
if /i "%open_actions%"=="Y" (
    start "" "%REPO_URL%/actions"
)

echo.
pause
goto MENU

:: ================================================================
:: CHECK PROJECT STATUS
:: ================================================================
:CHECK_STATUS
cls
echo.
echo ╔════════════════════════════════════════════════════════════╗
echo ║                    Project Status 📊                       ║
echo ╚════════════════════════════════════════════════════════════╝
echo.

echo ┌────────────────────────────────────────────────────────────┐
echo │                    Configuration                           │
echo └────────────────────────────────────────────────────────────┘
echo.
echo 📦 Project:    %PROJECT_NAME%
echo 📌 Version:    %PROJECT_VERSION%
echo 🔗 Repository: %REPO_URL%
echo 🌿 Branch:     %DEFAULT_BRANCH%
echo 📁 Solution:   %SOLUTION_FILE%
echo.

echo ┌────────────────────────────────────────────────────────────┐
echo │                    Git Repository                          │
echo └────────────────────────────────────────────────────────────┘
echo.

git --version >nul 2>&1
if errorlevel 1 (
    echo ❌ Git: Not installed
) else (
    echo ✅ Git: Installed
    git rev-parse --git-dir >nul 2>&1
    if errorlevel 1 (
        echo ⚠️  Repository: Not initialized
    ) else (
        for /f "tokens=*" %%a in ('git rev-parse --abbrev-ref HEAD 2^>nul') do echo 🌿 Branch: %%a
        for /f "tokens=*" %%a in ('git remote get-url origin 2^>nul') do echo 🔗 Remote: %%a
        echo.
        echo 📊 Git Status:
        git status --short
    )
)

echo.
echo ┌────────────────────────────────────────────────────────────┐
echo │                    Build Tools                             │
echo └────────────────────────────────────────────────────────────┘
echo.

dotnet --version >nul 2>&1
if errorlevel 1 (
    echo ❌ .NET SDK: Not installed
) else (
    echo ✅ .NET SDK: 
    dotnet --version
)

if defined ISCC (
    echo ✅ Inno Setup: Installed
) else (
    echo ⚠️  Inno Setup: Not found
)

echo.
echo ┌────────────────────────────────────────────────────────────┐
echo │                    Build Artifacts                         │
echo └────────────────────────────────────────────────────────────┘
echo.

if exist "%OUTPUT_DIR%\*.exe" (
    echo ✅ Release Build: Found
    dir "%OUTPUT_DIR%\*.exe" | find ".exe"
) else (
    echo ⚠️  Release Build: Not found
)

if exist "Output\*.exe" (
    echo ✅ Installer: Found
    dir "Output\*.exe" /B 2>nul
) else (
    echo ⚠️  Installer: Not found
)

echo.
echo ════════════════════════════════════════════════════════════
pause
goto MENU

:: ================================================================
:: CLEAN BUILD ARTIFACTS
:: ================================================================
:CLEAN_BUILD
cls
call :LOG "Clean build artifacts started"
echo.
echo ╔════════════════════════════════════════════════════════════╗
echo ║              Cleaning Build Artifacts 🧹                   ║
echo ╚════════════════════════════════════════════════════════════╝
echo.

set /p confirm="This will delete all build outputs. Continue? (Y/N): "
if /i not "%confirm%"=="Y" (
    call :LOG "Clean cancelled"
    goto MENU
)

echo.
call :CLEAN_BUILD_SILENT

echo.
echo ╔════════════════════════════════════════════════════════════╗
echo ║              Clean Completed Successfully! ✅              ║
echo ╚════════════════════════════════════════════════════════════╝
echo.

call :LOG "Clean completed successfully"
pause
goto MENU

:: ================================================================
:: ADVANCED OPTIONS
:: ================================================================
:ADVANCED_OPTIONS
cls
echo.
echo ╔════════════════════════════════════════════════════════════╗
echo ║                   Advanced Options 🔧                      ║
echo ╚════════════════════════════════════════════════════════════╝
echo.
echo  [1] 🔍 View Git Logs
echo  [2] 🌿 Manage Branches
echo  [3] 📋 View Recent Commits
echo  [4] 🏷️  List All Tags
echo  [5] 📦 NuGet Package Restore
echo  [6] 🔄 Reset to Remote (Discard local changes)
echo  [7] 📊 View Configuration
echo  [8] 🗑️  Delete Temp Files
echo  [9] 🔙 Back to Main Menu
echo.

set /p adv="Enter choice (1-9): "

if "%adv%"=="1" goto VIEW_LOGS
if "%adv%"=="2" goto MANAGE_BRANCHES
if "%adv%"=="3" goto VIEW_COMMITS
if "%adv%"=="4" goto LIST_TAGS
if "%adv%"=="5" goto NUGET_RESTORE
if "%adv%"=="6" goto RESET_TO_REMOTE
if "%adv%"=="7" goto VIEW_CONFIG
if "%adv%"=="8" goto DELETE_TEMP
if "%adv%"=="9" goto MENU
goto ADVANCED_OPTIONS

:VIEW_LOGS
cls
echo.
echo ═══ Git Logs ═══
echo.
git log --oneline --graph --decorate --all -20
echo.
pause
goto ADVANCED_OPTIONS

:MANAGE_BRANCHES
cls
echo.
echo ═══ Branches ═══
echo.
git branch -a
echo.
pause
goto ADVANCED_OPTIONS

:VIEW_COMMITS
cls
echo.
echo ═══ Recent Commits ═══
echo.
git log --oneline -10
echo.
pause
goto ADVANCED_OPTIONS

:LIST_TAGS
cls
echo.
echo ═══ All Tags ═══
echo.
git tag -l
echo.
pause
goto ADVANCED_OPTIONS

:NUGET_RESTORE
cls
echo.
echo 📦 Restoring NuGet packages...
dotnet restore "%SOLUTION_FILE%"
echo.
pause
goto ADVANCED_OPTIONS

:RESET_TO_REMOTE
cls
echo.
echo ⚠️  WARNING: This will discard ALL local changes!
echo.
set /p confirm="Type YES to confirm: "
if /i not "%confirm%"=="YES" goto ADVANCED_OPTIONS

git fetch origin
git reset --hard origin/%DEFAULT_BRANCH%
echo.
echo ✅ Reset to remote complete!
pause
goto ADVANCED_OPTIONS

:VIEW_CONFIG
cls
echo.
echo ═══ Current Configuration ═══
echo.
type release-config.json
echo.
echo ════════════════════════════════════════════════════════════
echo.
pause
goto ADVANCED_OPTIONS

:DELETE_TEMP
cls
echo.
echo 🗑️  Deleting temporary files...
if exist "%TEMP_DIR%" (
    rmdir /S /Q "%TEMP_DIR%"
    mkdir "%TEMP_DIR%"
    echo ✅ Temporary files deleted
) else (
    echo ℹ️  No temporary files found
)
echo.
pause
goto ADVANCED_OPTIONS

:: ================================================================
:: HELPER FUNCTIONS
:: ================================================================

:CHECK_DOTNET
dotnet --version >nul 2>&1
if errorlevel 1 (
    call :LOG "ERROR: .NET SDK not found"
    echo ❌ ERROR: .NET SDK not found!
    echo.
    echo 💡 Please install .NET 8 SDK from:
    echo    https://dotnet.microsoft.com/download
    echo.
    pause
    exit /b 1
)
exit /b 0

:CHECK_GIT
git --version >nul 2>&1
if errorlevel 1 (
    call :LOG "ERROR: Git not found"
    echo ❌ ERROR: Git not found!
    echo.
    echo 💡 Please install Git from:
    echo    https://git-scm.com/downloads
    echo.
    pause
    exit /b 1
)
exit /b 0

:BUILD_RELEASE_SILENT
dotnet restore "%SOLUTION_FILE%" >nul 2>&1
dotnet build "%SOLUTION_FILE%" --configuration Release --no-restore >nul 2>&1
if errorlevel 1 exit /b 1
dotnet publish "%PROJECT_FILE%" ^
    --configuration Release ^
    --runtime win-x64 ^
    --self-contained true ^
    --output "%OUTPUT_DIR%" ^
    /p:PublishSingleFile=true ^
    /p:IncludeNativeLibrariesForSelfExtract=true >nul 2>&1
exit /b 0

:CLEAN_BUILD_SILENT
echo 🧹 Cleaning bin and obj folders...
for /d /r %%d in (bin,obj) do (
    if exist "%%d" (
        rmdir /S /Q "%%d" 2>nul
    )
)

if exist "%OUTPUT_DIR%" (
    rmdir /S /Q "%OUTPUT_DIR%" 2>nul
    echo    ✅ Deleted %OUTPUT_DIR%\
)

if exist "Output" (
    rmdir /S /Q "Output" 2>nul
    echo    ✅ Deleted Output\
)
exit /b 0

:: ================================================================
:: EXIT
:: ================================================================
:EXIT
cls
call :LOG "Release Manager exited"
echo.
echo ╔════════════════════════════════════════════════════════════╗
echo ║                                                            ║
echo ║        Thanks for using Universal Release Manager!        ║
echo ║                                                            ║
echo ║                    Happy Releasing! 🚀                     ║
echo ║                                                            ║
echo ╚════════════════════════════════════════════════════════════╝
echo.
timeout /t 2 >nul
exit
