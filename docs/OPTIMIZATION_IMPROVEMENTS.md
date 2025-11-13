# 🚀 Windows Optimization Improvements

Este documento detalha as melhorias planejadas para o sistema de otimização do Windows.

## 📋 Estado Atual

### Otimizações Implementadas (v1.0.0)

✅ **Poder (Power Management)**
- Plano de energia "High Performance"
- Desativar sleep/hibernação quando conectado

✅ **Mouse**
- Desativar aceleração do ponteiro
- Registry: MouseSpeed, MouseThreshold1/2

✅ **Efeitos Visuais**
- Configurar para melhor performance
- Desativar animações desnecessárias

✅ **Serviços**
- Desativar: SysMain (Superfetch), DiagTrack, MapsBroker, RetailDemo

✅ **Explorador de Arquivos**
- Mostrar extensões de arquivos
- Mostrar arquivos ocultos
- Abrir no "Este Computador"

✅ **Telemetria**
- Desativar coleta de dados da Microsoft
- Registry: DataCollection policies

✅ **Limpeza**
- Arquivos temporários (%TEMP%, Windows\Temp)

✅ **Segurança**
- Criar restore point antes de otimizações

✅ **Avançado**
- Integração com Chris Titus Tech script

## 🎯 Melhorias Planejadas (v1.1.0+)

### 1. Interface de Seleção Customizada

**Problema Atual:** Aplica todas otimizações de uma vez

**Melhoria:**
```csharp
// Nova interface com checkboxes para cada otimização
public class OptimizationSettings
{
    public bool OptimizePower { get; set; } = true;
    public bool OptimizeMouse { get; set; } = true;
    public bool OptimizeVisualEffects { get; set; } = true;
    public bool DisableServices { get; set; } = true;
    public bool OptimizeExplorer { get; set; } = true;
    public bool DisableTelemetry { get; set; } = true;
    public bool CleanTemp { get; set; } = true;
    public bool DisableCortana { get; set; } = false; // NOVO
    public bool DisableOneDrive { get; set; } = false; // NOVO
    public bool OptimizeStartup { get; set; } = true; // NOVO
    public bool OptimizeNetwork { get; set; } = true; // NOVO
    public bool OptimizeStorage { get; set; } = true; // NOVO
}
```

**UI Mockup:**
```
┌─────────────────────────────────────────┐
│ Choose Optimizations                     │
├─────────────────────────────────────────┤
│ ☑ Power Management (Recommended)        │
│ ☑ Mouse Settings (Recommended)          │
│ ☑ Visual Effects (Recommended)          │
│ ☑ Disable Unnecessary Services          │
│ ☑ File Explorer Settings                │
│ ☑ Disable Telemetry                     │
│ ☑ Clean Temporary Files                 │
│ ☐ Disable Cortana                       │
│ ☐ Disable OneDrive                      │
│ ☑ Optimize Startup Programs             │
│ ☑ Network Optimization                  │
│ ☑ Storage Optimization                  │
│                                          │
│ [Select All] [Deselect All] [Defaults]  │
│                                          │
│ [Create Restore Point] ☑ (Recommended)  │
│                                          │
│          [Apply Selected]  [Cancel]      │
└─────────────────────────────────────────┘
```

### 2. Otimizações de Rede (NOVO)

```csharp
public async Task OptimizeNetwork()
{
    _logger.LogInfo("Optimizing network settings...");
    
    try
    {
        // Desativar auto-tuning (pode causar problemas em algumas redes)
        // await _commandRunner.RunCommandAsync("netsh", "interface tcp set global autotuninglevel=disabled");
        
        // Otimizar TCP/IP
        await _commandRunner.RunCommandAsync("netsh", "int tcp set global chimney=enabled");
        await _commandRunner.RunCommandAsync("netsh", "int tcp set global rss=enabled");
        await _commandRunner.RunCommandAsync("netsh", "int tcp set global netdma=enabled");
        
        // DNS Cache settings
        await _commandRunner.RunCommandAsync("netsh", "interface ip set dns \"Ethernet\" dhcp");
        
        // Desativar throttling de rede
        Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile",
            "NetworkThrottlingIndex", 0xFFFFFFFF, RegistryValueKind.DWord);
        
        _logger.LogSuccess("Network optimized!");
    }
    catch (Exception ex)
    {
        _logger.LogError($"Network optimization failed: {ex.Message}");
    }
}
```

### 3. Otimização de Inicialização (NOVO)

```csharp
public async Task OptimizeStartup()
{
    _logger.LogInfo("Optimizing startup programs...");
    
    try
    {
        // Analisar programas de inicialização
        var startupApps = GetStartupApplications();
        
        // Lista de apps seguros para desativar (não críticos)
        var safeToDisable = new[]
        {
            "Spotify",
            "Discord",
            "Steam",
            "Epic Games Launcher",
            "Adobe Creative Cloud",
            "Microsoft Teams (classic)",
            "Skype"
        };
        
        foreach (var app in startupApps)
        {
            if (safeToDisable.Any(safe => app.Name.Contains(safe, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogInfo($"Disabling startup: {app.Name}");
                DisableStartupApp(app);
            }
        }
        
        // Otimizar Boot Configuration
        await _commandRunner.RunCommandAsync("bcdedit", "/set {current} bootmenupolicy Legacy");
        await _commandRunner.RunCommandAsync("bcdedit", "/timeout 3");
        
        _logger.LogSuccess("Startup optimized!");
    }
    catch (Exception ex)
    {
        _logger.LogError($"Startup optimization failed: {ex.Message}");
    }
}

private List<StartupApp> GetStartupApplications()
{
    var apps = new List<StartupApp>();
    
    // Registry: Current User
    var runKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
    if (runKey != null)
    {
        foreach (var valueName in runKey.GetValueNames())
        {
            apps.Add(new StartupApp
            {
                Name = valueName,
                Path = runKey.GetValue(valueName)?.ToString() ?? "",
                Location = "HKCU\\Run"
            });
        }
    }
    
    // Registry: Local Machine
    var localMachineRun = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
    if (localMachineRun != null)
    {
        foreach (var valueName in localMachineRun.GetValueNames())
        {
            apps.Add(new StartupApp
            {
                Name = valueName,
                Path = localMachineRun.GetValue(valueName)?.ToString() ?? "",
                Location = "HKLM\\Run"
            });
        }
    }
    
    return apps;
}
```

### 4. Otimização de Armazenamento (NOVO)

```csharp
public async Task OptimizeStorage()
{
    _logger.LogInfo("Optimizing storage...");
    
    try
    {
        // Desativar hibernação (libera espaço = tamanho da RAM)
        await _commandRunner.RunCommandAsync("powercfg", "-h off");
        _logger.LogInfo("Hibernation disabled (freed RAM size space)");
        
        // Limpar Windows Update cache
        await _commandRunner.RunCommandAsync("dism", "/online /Cleanup-Image /StartComponentCleanup");
        _logger.LogInfo("Windows Update cache cleaned");
        
        // Limpar arquivos de log
        CleanDirectory(@"C:\Windows\Logs");
        CleanDirectory(@"C:\Windows\Temp");
        
        // Desativar System Restore em drives não-sistema (opcional)
        // await DisableSystemRestoreOnDataDrives();
        
        // Otimizar SSD (TRIM)
        await _commandRunner.RunCommandAsync("fsutil", "behavior set DisableDeleteNotify 0");
        _logger.LogInfo("TRIM enabled for SSD");
        
        // Desativar indexação em drives não-sistema
        await OptimizeIndexing();
        
        _logger.LogSuccess("Storage optimized!");
    }
    catch (Exception ex)
    {
        _logger.LogError($"Storage optimization failed: {ex.Message}");
    }
}

private async Task OptimizeIndexing()
{
    // Desativar indexação em drives de dados (manter apenas em C:)
    var drives = DriveInfo.GetDrives()
        .Where(d => d.DriveType == DriveType.Fixed && d.Name != "C:\\");
    
    foreach (var drive in drives)
    {
        _logger.LogInfo($"Disabling indexing on {drive.Name}");
        // Implementar desativação de indexação
    }
}
```

### 5. Desativar Cortana (NOVO)

```csharp
public async Task DisableCortana()
{
    _logger.LogInfo("Disabling Cortana...");
    
    try
    {
        // Registry para desativar Cortana
        Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\Windows Search",
            "AllowCortana", 0, RegistryValueKind.DWord);
        
        // Desativar pesquisa web na barra de tarefas
        Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Search",
            "BingSearchEnabled", 0, RegistryValueKind.DWord);
        
        Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Search",
            "CortanaConsent", 0, RegistryValueKind.DWord);
        
        _logger.LogSuccess("Cortana disabled!");
    }
    catch (Exception ex)
    {
        _logger.LogError($"Failed to disable Cortana: {ex.Message}");
    }
}
```

### 6. Desativar OneDrive (NOVO)

```csharp
public async Task DisableOneDrive()
{
    _logger.LogInfo("Disabling OneDrive...");
    
    try
    {
        // Matar processo do OneDrive
        var oneDriveProcesses = Process.GetProcessesByName("OneDrive");
        foreach (var process in oneDriveProcesses)
        {
            process.Kill();
            await process.WaitForExitAsync();
        }
        
        // Registry para desativar OneDrive
        Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\OneDrive",
            "DisableFileSyncNGSC", 1, RegistryValueKind.DWord);
        
        // Desinstalar OneDrive (Windows 10/11)
        var oneDriveSetup = @"C:\Windows\SysWOW64\OneDriveSetup.exe";
        if (File.Exists(oneDriveSetup))
        {
            await _commandRunner.RunCommandAsync(oneDriveSetup, "/uninstall");
            _logger.LogInfo("OneDrive uninstalled");
        }
        
        // Remover OneDrive do File Explorer
        Registry.SetValue(@"HKEY_CLASSES_ROOT\CLSID\{018D5C66-4533-4307-9B53-224DE2ED1FE6}",
            "System.IsPinnedToNameSpaceTree", 0, RegistryValueKind.DWord);
        
        _logger.LogSuccess("OneDrive disabled and uninstalled!");
    }
    catch (Exception ex)
    {
        _logger.LogError($"Failed to disable OneDrive: {ex.Message}");
    }
}
```

### 7. Otimizações de Gaming (NOVO)

```csharp
public async Task OptimizeForGaming()
{
    _logger.LogInfo("Applying gaming optimizations...");
    
    try
    {
        // Desativar Game DVR
        Registry.SetValue(@"HKEY_CURRENT_USER\System\GameConfigStore",
            "GameDVR_Enabled", 0, RegistryValueKind.DWord);
        
        Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\GameDVR",
            "AppCaptureEnabled", 0, RegistryValueKind.DWord);
        
        // Hardware-accelerated GPU scheduling (Windows 10 2004+)
        Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\GraphicsDrivers",
            "HwSchMode", 2, RegistryValueKind.DWord);
        _logger.LogInfo("Hardware-accelerated GPU scheduling enabled");
        
        // Desativar Fullscreen Optimizations
        Registry.SetValue(@"HKEY_CURRENT_USER\System\GameConfigStore",
            "GameDVR_FSEBehaviorMode", 2, RegistryValueKind.DWord);
        
        // Game Mode
        Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\GameBar",
            "AutoGameModeEnabled", 1, RegistryValueKind.DWord);
        
        _logger.LogSuccess("Gaming optimizations applied!");
    }
    catch (Exception ex)
    {
        _logger.LogError($"Gaming optimization failed: {ex.Message}");
    }
}
```

### 8. Análise e Relatório Pré-Otimização (NOVO)

```csharp
public async Task<OptimizationReport> AnalyzeSystem()
{
    _logger.LogInfo("Analyzing system...");
    
    var report = new OptimizationReport
    {
        SystemInfo = await GetSystemInfo(),
        CurrentSettings = await GetCurrentOptimizationSettings(),
        Recommendations = GenerateRecommendations(),
        EstimatedImpact = CalculateEstimatedImpact()
    };
    
    return report;
}

public class OptimizationReport
{
    public SystemInfo SystemInfo { get; set; }
    public CurrentSettings CurrentSettings { get; set; }
    public List<Recommendation> Recommendations { get; set; }
    public ImpactEstimate EstimatedImpact { get; set; }
}
```

### 9. Perfis de Otimização (NOVO)

Criar perfis pré-definidos:

```csharp
public enum OptimizationProfile
{
    Balanced,        // Balanço entre performance e recursos
    Performance,     // Máxima performance
    Gaming,          // Otimizado para jogos
    ContentCreation, // Otimizado para produção de conteúdo
    Battery,         // Economizar bateria (laptops)
    Custom           // Customizado pelo usuário
}

public OptimizationSettings GetProfileSettings(OptimizationProfile profile)
{
    return profile switch
    {
        OptimizationProfile.Performance => new OptimizationSettings
        {
            OptimizePower = true,
            OptimizeMouse = true,
            OptimizeVisualEffects = true,
            DisableServices = true,
            DisableCortana = true,
            OptimizeStartup = true,
            OptimizeNetwork = true
        },
        OptimizationProfile.Gaming => new OptimizationSettings
        {
            OptimizePower = true,
            OptimizeVisualEffects = true,
            DisableServices = true,
            OptimizeNetwork = true,
            OptimizeForGaming = true
        },
        // ... outros perfis
        _ => GetDefaultSettings()
    };
}
```

### 10. Backup e Rollback de Configurações (NOVO)

```csharp
public async Task<ConfigurationBackup> BackupCurrentConfiguration()
{
    _logger.LogInfo("Backing up current configuration...");
    
    var backup = new ConfigurationBackup
    {
        Timestamp = DateTime.Now,
        PowerPlan = GetCurrentPowerPlan(),
        RegistrySettings = BackupRegistrySettings(),
        ServiceStates = BackupServiceStates(),
        StartupApps = GetStartupApplications()
    };
    
    // Salvar em JSON
    var backupPath = Path.Combine(_backupDir, $"Config_Backup_{DateTime.Now:yyyy-MM-dd_HHmmss}.json");
    await File.WriteAllTextAsync(backupPath, JsonConvert.SerializeObject(backup, Formatting.Indented));
    
    _logger.LogSuccess($"Configuration backed up to: {backupPath}");
    return backup;
}

public async Task RestoreConfiguration(string backupPath)
{
    _logger.LogInfo("Restoring configuration...");
    
    var backup = JsonConvert.DeserializeObject<ConfigurationBackup>(
        await File.ReadAllTextAsync(backupPath));
    
    // Restaurar settings
    RestorePowerPlan(backup.PowerPlan);
    RestoreRegistrySettings(backup.RegistrySettings);
    RestoreServiceStates(backup.ServiceStates);
    
    _logger.LogSuccess("Configuration restored!");
}
```

## 📊 Métricas e Monitoramento (NOVO)

```csharp
public class OptimizationMetrics
{
    public DateTime AppliedAt { get; set; }
    public TimeSpan BootTimeBeforeafter { get; set; }
    public long FreeSpaceFreed { get; set; } // em bytes
    public int ServicesDisabled { get; set; }
    public int StartupAppsDisabled { get; set; }
    public Dictionary<string, bool> OptimizationsApplied { get; set; }
}
```

## 🎨 Nova Interface para Otimizações

```
┌────────────────────────────────────────────────┐
│ System Optimization                             │
├────────────────────────────────────────────────┤
│                                                 │
│ Choose Profile:                                 │
│ ○ Balanced (Recommended)                        │
│ ○ Performance                                   │
│ ○ Gaming                                        │
│ ○ Content Creation                              │
│ ● Custom                                        │
│                                                 │
│ ┌─────────────────────────────────────────┐   │
│ │ Custom Optimizations:                    │   │
│ │ ☑ Power Management          [Details]   │   │
│ │ ☑ Mouse Settings            [Details]   │   │
│ │ ☑ Visual Effects            [Details]   │   │
│ │ ☑ Services                  [Details]   │   │
│ │ ☑ File Explorer             [Details]   │   │
│ │ ☑ Telemetry                 [Details]   │   │
│ │ ☑ Cleanup                   [Details]   │   │
│ │ ☐ Cortana                   [Details]   │   │
│ │ ☐ OneDrive                  [Details]   │   │
│ │ ☑ Startup Programs          [Details]   │   │
│ │ ☑ Network                   [Details]   │   │
│ │ ☑ Storage                   [Details]   │   │
│ │ ☐ Gaming Mode               [Details]   │   │
│ └─────────────────────────────────────────┘   │
│                                                 │
│ ☑ Create Restore Point                         │
│ ☑ Backup Current Configuration                 │
│                                                 │
│ [Analyze System] [Apply] [Restore Previous]    │
└────────────────────────────────────────────────┘
```

## 📅 Roadmap

### v1.1.0 (Próxima versão)
- [ ] Interface de seleção customizada
- [ ] Perfis de otimização
- [ ] Backup/Restore de configurações
- [ ] Análise pré-otimização

### v1.2.0
- [ ] Otimizações de rede
- [ ] Otimizações de storage
- [ ] Gaming optimizations
- [ ] Métricas e relatórios

### v1.3.0
- [ ] Desativar Cortana/OneDrive
- [ ] Otimização de startup
- [ ] Scheduled optimizations
- [ ] Perfis salvos pelo usuário

### v2.0.0
- [ ] Machine learning para recomendar otimizações
- [ ] Comparação antes/depois com métricas
- [ ] Community-shared optimization profiles
- [ ] Windows 12 support (quando lançar)

## 🚀 Como Contribuir

Se você quer ajudar a implementar essas melhorias:

1. Escolha uma feature da lista
2. Crie uma Issue no GitHub
3. Fork o projeto
4. Implemente a feature
5. Faça Pull Request

---

**Mantenha o Windows otimizado! ⚡**

