@echo off
SETLOCAL EnableDelayedExpansion
title Configuracao Pos-Formatacao - Menu Principal
color 0B

:: ####################################################################################
:: ##                                                                                ##
:: ##              CONFIGURACAO POS-FORMATACAO DO WINDOWS - v1.0                      ##
:: ##                                                                                ##
:: ####################################################################################

:: Verificacao de Administrador
>nul 2>&1 "%SYSTEMROOT%\system32\cacls.exe" "%SYSTEMROOT%\system32\config\system"
if '%errorlevel%' NEQ '0' (
    cls
    echo ====================================================================
    echo  ERRO: Privilegios de Administrador sao necessarios!
    echo ====================================================================
    echo.
    echo   Por favor, clique com o botao direito neste arquivo e selecione
    echo   "Executar como administrador".
    echo.
    pause
    exit
)

:MENU
cls
echo.
echo ============================================================================
echo                CONFIGURACAO POS-FORMATACAO DO WINDOWS
echo ============================================================================
echo.
echo  FASE 1: DRIVERS E RUNTIMES
echo  [1] Instalar Drivers (INSTALE OS DRIVERS)
echo  [2] Instalar Runtimes (All In One Runtimes)
echo.
echo  FASE 2: APLICATIVOS
echo  [3] Instalar Apps via Ninite (Brave, Discord, Steam, WinRAR)
echo  [4] Instalar Apps via Script (VSCode, Node.js, Python, etc.)
echo  [5] Abrir pasta para instalacoes manuais
echo.
echo  FASE 3: ATIVACAO
echo  [6] Ativar Windows/Office (Ver instrucoes KMS)
echo.
echo  FASE 4: CONFIGURACAO
echo  [7] Restaurar Perfil Brave
echo  [8] Otimizar Sistema - Chris Titus Tech
echo  [9] Otimizar Sistema - Ultimate Optimizer
echo  [A] Configurar Windows Defender
echo.
echo  DOCUMENTACAO
echo  [B] Abrir Plano Completo (windows-post-format.plan.md)
echo  [C] Abrir Pasta de Scripts
echo  [D] Ver Checklist de Verificacao
echo.
echo  [X] Sair
echo.
echo ============================================================================
echo.

set /p choice="Digite sua opcao: "

if /i "%choice%"=="1" goto DRIVERS
if /i "%choice%"=="2" goto RUNTIMES
if /i "%choice%"=="3" goto NINITE
if /i "%choice%"=="4" goto SCRIPT_APPS
if /i "%choice%"=="5" goto MANUAL_INSTALLS
if /i "%choice%"=="6" goto ATIVACAO
if /i "%choice%"=="7" goto BRAVE
if /i "%choice%"=="8" goto CHRIS_TITUS
if /i "%choice%"=="9" goto OPTIMIZER
if /i "%choice%"=="A" goto DEFENDER
if /i "%choice%"=="B" goto PLANO
if /i "%choice%"=="C" goto SCRIPTS_FOLDER
if /i "%choice%"=="D" goto CHECKLIST
if /i "%choice%"=="X" goto EXIT

echo Opcao invalida!
timeout /t 2 >nul
goto MENU

:: ####################################################################################

:DRIVERS
cls
echo.
echo ============================================================================
echo  FASE 1.1: INSTALACAO DE DRIVERS
echo ============================================================================
echo.
echo Iniciando instalacao de drivers...
echo.

if exist "INSTALE OS DRIVERS\EXECUTE COMO ADMINISTRADOR.bat" (
    cd "INSTALE OS DRIVERS"
    call "EXECUTE COMO ADMINISTRADOR.bat"
    cd ..
    echo.
    echo Drivers instalados! Reinicie se necessario.
) else (
    echo [ERRO] Script de drivers nao encontrado!
    echo Verifique a pasta: INSTALE OS DRIVERS\
)

echo.
pause
goto MENU

:RUNTIMES
cls
echo.
echo ============================================================================
echo  FASE 1.2: INSTALACAO DE RUNTIMES
echo ============================================================================
echo.
echo Iniciando instalacao de runtimes...
echo.

if exist "FP_All In One Runtimes 4.6.7.zip" (
    echo Arquivo ZIP encontrado: FP_All In One Runtimes 4.6.7.zip
    echo.
    echo Por favor, extraia manualmente e execute o instalador.
    echo.
    explorer.exe "%CD%"
) else if exist "INSTALE OS DRIVERS\All In One Runtimes 4.6.7.exe" (
    echo Executando instalador de runtimes...
    start "" "INSTALE OS DRIVERS\All In One Runtimes 4.6.7.exe"
) else (
    echo [AVISO] Instalador de runtimes nao encontrado!
)

echo.
pause
goto MENU

:NINITE
cls
echo.
echo ============================================================================
echo  FASE 2.1: INSTALACAO VIA NINITE
echo ============================================================================
echo.
echo Instalando: Brave, Discord, Steam, WinRAR
echo.

if exist "Ninite Brave Discord Steam WinRAR Installer.exe" (
    start "" "Ninite Brave Discord Steam WinRAR Installer.exe"
    echo.
    echo Ninite iniciado! Aguarde a instalacao ser concluida.
) else (
    echo [ERRO] Instalador Ninite nao encontrado!
)

echo.
pause
goto MENU

:SCRIPT_APPS
cls
echo.
echo ============================================================================
echo  FASE 2.2: INSTALACAO VIA SCRIPT
echo ============================================================================
echo.
echo Instalando: VSCode, Node.js, Python, System Informer, etc.
echo.

if exist "Scripts\Install APPS.bat" (
    cd Scripts
    call "Install APPS.bat"
    cd ..
    echo.
    echo Apps instalados via script!
) else (
    echo [ERRO] Script Install APPS.bat nao encontrado!
)

echo.
pause
goto MENU

:MANUAL_INSTALLS
cls
echo.
echo ============================================================================
echo  FASE 2.3: INSTALACOES MANUAIS
echo ============================================================================
echo.
echo Abrindo pasta raiz para instalacoes manuais:
echo.
echo  - python-3.13.2-amd64.exe
echo  - Obsidian-1.8.7.exe
echo  - AdsPower-Global-6.12.6-x64.exe
echo  - Cloudflare_WARP_2025.1.861.0.msi
echo  - setup-lightshot.exe
echo.
echo Instale manualmente os programas necessarios.
echo.

explorer.exe "%CD%"

echo.
pause
goto MENU

:ATIVACAO
cls
echo.
echo ============================================================================
echo  FASE 3: ATIVACAO DO WINDOWS/OFFICE
echo ============================================================================
echo.
echo OPCAO 1 (RECOMENDADA): Via PowerShell
echo ----------------------------------------
echo Execute no PowerShell como Admin:
echo.
echo   irm https://massgrave.dev/get ^| iex
echo.
echo.
echo OPCAO 2: Via KMS Tools
echo ----------------------------------------
echo 1. Desative Windows Defender temporariamente
echo 2. Extraia: KMS\KMS Tools Lite Portable.zip
echo 3. Execute como Administrador
echo 4. Ative Windows e/ou Office
echo 5. Reative Windows Defender
echo.
echo.

choice /C 12V /N /M "Escolha: [1] PowerShell | [2] Abrir pasta KMS | [V] Voltar: "

if errorlevel 3 goto MENU
if errorlevel 2 (
    explorer.exe "%CD%\KMS"
    goto ATIVACAO
)
if errorlevel 1 (
    echo.
    echo Abrindo PowerShell como Administrador...
    echo Execute o comando acima no PowerShell.
    echo.
    powershell -Command "Start-Process powershell -Verb RunAs"
    pause
    goto MENU
)

goto MENU

:BRAVE
cls
echo.
echo ============================================================================
echo  FASE 4.1: RESTAURAR PERFIL DO BRAVE
echo ============================================================================
echo.
echo Iniciando gerenciador de backup/restore do Brave...
echo.

if exist "Scripts\Backup_Brave_Perfil.bat" (
    cd Scripts
    call "Backup_Brave_Perfil.bat"
    cd ..
) else (
    echo [ERRO] Script de backup do Brave nao encontrado!
)

echo.
pause
goto MENU

:CHRIS_TITUS
cls
echo.
echo ============================================================================
echo  FASE 4.2: OTIMIZACAO - CHRIS TITUS TECH
echo ============================================================================
echo.
echo Este comando abrira a ferramenta Chris Titus Windows Utility.
echo.
echo Execute no PowerShell como Administrador:
echo.
echo   irm christitus.com/win ^| iex
echo.
echo.
echo RECOMENDACOES:
echo  1. Tab "Tweaks" - Selecionar "Desktop" ou "Laptop"
echo  2. Clicar "Run Tweaks"
echo  3. Tab "Config" - Criar ponto de restauracao
echo.

choice /C SN /N /M "Abrir PowerShell agora? (S/N): "

if errorlevel 2 goto MENU

echo.
echo Abrindo PowerShell como Administrador...
powershell -Command "Start-Process powershell -Verb RunAs"

echo.
pause
goto MENU

:OPTIMIZER
cls
echo.
echo ============================================================================
echo  FASE 4.3: OTIMIZACAO - ULTIMATE OPTIMIZER
echo ============================================================================
echo.
echo Iniciando Windows Optimizer...
echo.

if exist "Scripts\Ultimate_Windows_Optimizer.bat" (
    cd Scripts
    call "Ultimate_Windows_Optimizer.bat"
    cd ..
) else (
    echo [ERRO] Script Ultimate Optimizer nao encontrado!
)

echo.
pause
goto MENU

:DEFENDER
cls
echo.
echo ============================================================================
echo  FASE 4.4: CONFIGURAR WINDOWS DEFENDER
echo ============================================================================
echo.
echo Abrindo pasta de configuracao do Defender...
echo.

if exist "24122024\24122024" (
    echo Ferramentas disponiveis:
    echo  - dControl.exe
    echo  - Defender_Settings.vbs
    echo.
    echo Leia: ReadMe.txt para instrucoes
    echo.
    explorer.exe "%CD%\24122024\24122024"
) else (
    echo [ERRO] Pasta do Defender Control nao encontrada!
)

echo.
pause
goto MENU

:PLANO
cls
echo.
echo ============================================================================
echo  DOCUMENTACAO: PLANO COMPLETO
echo ============================================================================
echo.
echo Abrindo plano de configuracao completo...
echo.

if exist "windows-post-format.plan.md" (
    start "" "windows-post-format.plan.md"
) else (
    echo [ERRO] Arquivo windows-post-format.plan.md nao encontrado!
)

echo.
pause
goto MENU

:SCRIPTS_FOLDER
cls
echo.
echo ============================================================================
echo  PASTA DE SCRIPTS
echo ============================================================================
echo.
echo Abrindo pasta Scripts...
echo.

if exist "Scripts" (
    explorer.exe "%CD%\Scripts"
) else (
    echo [ERRO] Pasta Scripts nao encontrada!
)

echo.
pause
goto MENU

:CHECKLIST
cls
echo.
echo ============================================================================
echo  CHECKLIST DE VERIFICACAO FINAL
echo ============================================================================
echo.
echo HARDWARE E DRIVERS:
echo  [ ] Todos os dispositivos funcionando
echo  [ ] GPU reconhecida e drivers atualizados
echo  [ ] Audio funcionando
echo  [ ] Rede/Wi-Fi funcionando
echo.
echo SOFTWARE ESSENCIAL:
echo  [ ] Windows ativado
echo  [ ] Office ativado (se aplicavel)
echo  [ ] Brave Browser configurado
echo  [ ] Discord funcionando
echo  [ ] Steam instalado
echo  [ ] VS Code instalado
echo  [ ] Python no PATH (python --version)
echo  [ ] Node.js funcionando (node --version)
echo.
echo OTIMIZACOES:
echo  [ ] Plano de energia: Alto Desempenho
echo  [ ] Efeitos visuais otimizados
echo  [ ] Servicos desnecessarios desativados
echo  [ ] Telemetria desativada
echo  [ ] Limpeza de temp realizada
echo.
echo SEGURANCA:
echo  [ ] Windows Defender ativo
echo  [ ] Firewall ativo
echo  [ ] Windows Update configurado
echo  [ ] Cloudflare WARP ativo
echo.
echo BACKUP E RECUPERACAO:
echo  [ ] Ponto de restauracao criado
echo  [ ] Perfil do Brave com backup
echo  [ ] Documentos importantes salvos
echo.
echo ============================================================================
echo.
echo Para detalhes completos, consulte: windows-post-format.plan.md
echo.
pause
goto MENU

:EXIT
cls
echo.
echo ============================================================================
echo  Encerrando...
echo ============================================================================
echo.
echo Obrigado por usar o Configurador Pos-Formatacao!
echo.
echo Lembre-se de:
echo  - Reiniciar o PC apos instalacoes
echo  - Criar ponto de restauracao
echo  - Fazer backup regular
echo.
timeout /t 3 >nul
exit

:: ####################################################################################

