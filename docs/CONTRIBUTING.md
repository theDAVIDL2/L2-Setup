# Contributing to Windows Post-Format Setup Tool

First off, thank you for considering contributing to this project! 🎉

## Code of Conduct

This project follows a simple code of conduct:
- Be respectful and inclusive
- Provide constructive feedback
- Focus on what is best for the community
- Show empathy towards other community members

## How Can I Contribute?

### Reporting Bugs

Before creating bug reports, please check the existing issues to avoid duplicates.

When you create a bug report, include as many details as possible:

- **Use a clear and descriptive title**
- **Describe the exact steps to reproduce the problem**
- **Provide specific examples**
- **Describe the behavior you observed and what you expected**
- **Include screenshots if applicable**
- **Include your Windows version and .NET version**

### Suggesting Enhancements

Enhancement suggestions are tracked as GitHub issues. When creating an enhancement suggestion:

- **Use a clear and descriptive title**
- **Provide a detailed description of the suggested enhancement**
- **Explain why this enhancement would be useful**
- **List any similar features in other tools if applicable**

### Pull Requests

1. Fork the repo and create your branch from `main`
2. If you've added code that should be tested, add tests
3. Ensure your code follows the existing style
4. Update documentation if needed
5. Make sure your code builds without errors
6. Issue the pull request!

## Development Setup

### Prerequisites

- Windows 10/11
- Visual Studio 2022 or later
- .NET 8 SDK
- Git

### Setting Up Development Environment

```bash
# Clone your fork
git clone https://github.com/YOUR-USERNAME/windows-post-format-setup.git
cd windows-post-format-setup

# Add upstream remote
git remote add upstream https://github.com/ORIGINAL-OWNER/windows-post-format-setup.git

# Install dependencies
dotnet restore src/WindowsSetup.App/WindowsSetup.App.csproj

# Build
dotnet build src/WindowsSetup.App/WindowsSetup.App.csproj

# Run
dotnet run --project src/WindowsSetup.App/WindowsSetup.App.csproj
```

## Style Guide

### C# Code Style

- Use 4 spaces for indentation
- Use `camelCase` for local variables
- Use `PascalCase` for methods, properties, and classes
- Use `_camelCase` for private fields
- Always use `{ }` braces for control structures
- Add XML documentation comments for public methods

```csharp
/// <summary>
/// Downloads a file with progress tracking
/// </summary>
/// <param name="item">The download item</param>
/// <param name="progress">Progress reporter</param>
/// <returns>Task representing the async operation</returns>
public async Task DownloadWithProgressAsync(DownloadItem item, IProgress<DownloadProgress>? progress = null)
{
    var retryCount = 0;
    // ... implementation
}
```

### XAML Style

- Use 4 spaces for indentation
- One attribute per line for complex elements
- Organize attributes in logical groups
- Use meaningful names for x:Name attributes

```xaml
<Button x:Name="InstallButton"
        Content="Install Tools"
        Style="{StaticResource MaterialDesignRaisedButton}"
        Margin="0,0,8,0"
        Click="InstallButton_Click"/>
```

### Commit Messages

- Use the present tense ("Add feature" not "Added feature")
- Use the imperative mood ("Move cursor to..." not "Moves cursor to...")
- Limit the first line to 72 characters
- Reference issues and pull requests after the first line

```
Add: Browser profile backup feature

- Implement backup service with compression
- Add UI controls for backup/restore
- Include unit tests

Fixes #123
```

**Commit prefixes:**
- `Add:` New feature
- `Fix:` Bug fix
- `Update:` Update existing feature
- `Refactor:` Code refactoring
- `Docs:` Documentation changes
- `Style:` Code style changes (formatting, etc.)
- `Test:` Adding or updating tests
- `Chore:` Maintenance tasks

## Project Structure

```
windows-post-format-setup/
├── src/
│   └── WindowsSetup.App/
│       ├── Services/       # Business logic
│       ├── Models/         # Data models
│       ├── Utils/          # Utilities
│       ├── MainWindow.xaml # Main UI
│       └── App.xaml        # App config
├── assets/                 # Icons, images
├── docs/                   # Documentation
├── .github/workflows/      # CI/CD
└── README.md
```

## Adding New Features

### Adding a New Tool to Install

1. Add tool definition in `ToolInstallerService.GetAllToolDefinitions()`:

```csharp
new ToolDefinition 
{ 
    Name = "NewTool",
    WingetId = "Publisher.NewTool",  // or use Method = "direct"
    Priority = 100,
    Essential = false
}
```

2. Test installation on clean Windows
3. Update README with new tool in list
4. Submit PR with clear description

### Adding a New Optimization

1. Add method to `WindowsOptimizerService`:

```csharp
private async Task OptimizeNewFeature()
{
    _logger.LogInfo("Optimizing new feature...");
    
    try
    {
        // Implementation
        _logger.LogSuccess("New feature optimized");
    }
    catch (Exception ex)
    {
        _logger.LogError($"Optimization failed: {ex.Message}");
    }
    
    await Task.CompletedTask;
}
```

2. Call from `ApplyAllOptimizations()`
3. Test on VM
4. Update documentation
5. Submit PR

### Adding a New Service

1. Create new service in `Services/` folder
2. Follow existing service patterns
3. Inject `Logger` in constructor
4. Implement async methods
5. Add to `MainWindow.xaml.cs`
6. Add UI controls in `MainWindow.xaml`
7. Write documentation
8. Submit PR

## Testing

### Manual Testing Checklist

Before submitting a PR, test:

- ✅ Build succeeds without errors
- ✅ Application starts and shows UI
- ✅ Admin check works correctly
- ✅ Backup creates valid ZIP file
- ✅ Restore works from backup
- ✅ Tool installation completes successfully
- ✅ Optimizations apply without errors
- ✅ Logging works correctly
- ✅ Error handling displays appropriate messages

### Testing on VM

For system-level changes:
1. Create VM snapshot
2. Test changes
3. Verify system state
4. Revert snapshot
5. Test again to ensure repeatability

## Documentation

When adding features, update:

- `README.md` - User-facing documentation
- `docs/ARCHITECTURE.md` - Technical architecture
- `docs/CHANGELOG.md` - Version history
- Code comments - Inline documentation
- XML comments - API documentation

## Questions?

Feel free to:
- Open an issue with your question
- Start a discussion in GitHub Discussions
- Comment on existing issues/PRs

## License

By contributing, you agree that your contributions will be licensed under the MIT License.

---

**Thank you for contributing! 🚀**

