# 🚀 Windows Post-Format Setup Tool

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)
![Platform](https://img.shields.io/badge/platform-Windows-blue.svg)

## 📋 Sobre

Ferramenta completa de automação para configuração pós-formatação do Windows. Economize horas instalando ferramentas, restaurando perfis de navegador e otimizando o sistema com apenas alguns cliques!

**⚠️ IMPORTANTE:** Use ANTES de formatar para fazer backup e DEPOIS para restaurar e configurar tudo automaticamente!

## ✨ Funcionalidades

### 🌐 Gerenciamento de Browsers (PRINCIPAL)

- ✅ **Backup completo do profile Brave**
  - Extensões, favoritos, senhas, configurações
  - Compressão automática para economia de espaço
  - Suporte para salvar em USB ou nuvem
- ✅ **Restore pós-formatação com 1 clique**
  - Detecção automática de backups em USB
  - Restauração completa do perfil
- ✅ **Configurar Brave como navegador padrão automaticamente**
- ✅ Suporte para Chrome, Edge, Firefox (em desenvolvimento)

### 🔧 Instalação de Ferramentas

**Ferramentas de Desenvolvimento:**
- Git, Python 3.13, Node.js LTS
- Visual Studio Code, Cursor IDE
- Notepad++, IntelliJ IDEA Community, PyCharm Community
- Amazon Corretto JDK 8 & 17
- Postman, DBeaver, FileZilla, PuTTY
- Yarn, pnpm, Composer

**Navegadores:**
- Brave Browser (configurado como padrão)
- Comet (Perplexity AI Browser)

**Aplicativos Essenciais:**
- Discord, Steam, WinRAR
- Lightshot, AdsPower
- System Informer, IObit Unlocker
- MSI Afterburner, Logitech G Hub
- JDownloader 2

**Runtimes:**
- Visual C++ Redistributables (2005-2022)
- .NET Framework 4.8
- DirectX End-User Runtime

### ⚡ Otimizações do Windows

- ✅ Desempenho máximo (power plan)
- ✅ Desativar aceleração do mouse
- ✅ Otimizar efeitos visuais para performance
- ✅ Desativar serviços desnecessários
- ✅ Desativar telemetria e coleta de dados
- ✅ Configurar File Explorer (mostrar extensões, arquivos ocultos)
- ✅ Limpeza de arquivos temporários
- ✅ Integração com Chris Titus Tech script (opção avançada)

### 🔑 Ativação do Windows

- ✅ Ativação automática com 1 clique
- ✅ Usa Microsoft Activation Scripts
- ✅ Verificação de status de ativação
- ✅ Método HWID (Hardware ID)

## 📥 Instalação

### Método 1: Instalador (Recomendado)

1. Baixe `WindowsPostFormatSetup_v1.0.0.exe` da [última release](https://github.com/yourusername/windows-post-format-setup/releases)
2. Execute como administrador
3. Siga o wizard de instalação
4. Inicie o aplicativo

### Método 2: Build do Código-Fonte

```bash
# Clone o repositório
git clone https://github.com/yourusername/windows-post-format-setup.git
cd windows-post-format-setup

# Restaurar dependências
dotnet restore

# Build
dotnet build --configuration Release

# Executar
cd src/WindowsSetup.App/bin/Release/net8.0-windows
./WindowsSetup.exe
```

## 🎯 Como Usar

### ANTES de Formatar:

1. **Abra o aplicativo**
2. **Vá em "Browser Management"**
3. **Clique em "Backup Brave Profile"**
4. **Escolha onde salvar:**
   - Pasta local
   - USB drive (recomendado)
   - OneDrive/Dropbox
5. **Aguarde a conclusão** (geralmente 1-5 minutos)
6. **Confirme** que o backup foi criado

### DEPOIS de Formatar:

1. **Instale o Windows Setup Tool**
2. **Execute como administrador**

3. **Instalar Ferramentas:**
   - Clique em "Install Essentials" (Git, Python, Node, VS Code, Brave)
   - OU "Install All Tools" para instalação completa (~40-60 min)

4. **Restaurar Profile do Brave:**
   - Vá em "Browser Management"
   - Clique em "Restore Brave Profile"
   - Selecione o backup (detectado automaticamente se em USB)
   - Aguarde a restauração

5. **Otimizar Windows:**
   - Vá em "System Optimization"
   - Clique em "Apply Optimizations"
   - Um restore point será criado automaticamente

6. **Ativar Windows (opcional):**
   - Vá em "Windows Activation"
   - Clique em "Activate Windows"
   - Aguarde o processo automático

7. **Reinicie o PC** para aplicar todas as mudanças

## ⏱️ Tempo Estimado

| Etapa | Tempo |
|-------|-------|
| Download de ferramentas | 15-30 min |
| Instalação | 20-40 min |
| Restore de browser | 2-5 min |
| Otimizações | 2-5 min |
| **Total** | **~40-80 min** |

**vs ~3-4 horas manualmente** ✅

## 🛠️ Tecnologias

- **C# .NET 8** - Framework principal
- **WPF** (Windows Presentation Foundation) - Interface gráfica
- **Material Design in XAML** - Design moderno
- **Inno Setup** - Criação do instalador
- **Winget** - Gerenciador de pacotes Windows
- **SharpCompress** - Compressão de arquivos

## 📦 Requisitos

- Windows 10/11 (64-bit)
- .NET 8 Runtime (instalado automaticamente)
- Privilégios de Administrador
- Conexão com Internet

## 📸 Screenshots

_Screenshots serão adicionados em breve_

## 🔧 Desenvolvimento

### Requisitos para Desenvolvimento

- Visual Studio 2022 ou superior
- .NET 8 SDK
- Inno Setup (para criar instalador)

### Estrutura do Projeto

```
windows-post-format-setup/
├── src/
│   └── WindowsSetup.App/        # Aplicativo WPF principal
│       ├── Services/            # Lógica de negócio
│       ├── Models/              # Modelos de dados
│       ├── Utils/               # Utilitários
│       ├── MainWindow.xaml      # Interface principal
│       └── AppSettings.json     # Configurações
├── assets/                      # Recursos (ícones)
├── docs/                        # Documentação
├── .github/workflows/           # CI/CD
└── README.md
```

### Compilar

```bash
# Debug
dotnet build

# Release
dotnet build --configuration Release

# Publicar
dotnet publish -c Release -r win-x64 --self-contained false

# Criar instalador (requer Inno Setup instalado)
iscc setup.iss
```

## 🤝 Contribuindo

Contribuições são muito bem-vindas! 

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/MinhaFeature`)
3. Commit suas mudanças (`git commit -m 'Add: Minha nova feature'`)
4. Push para a branch (`git push origin feature/MinhaFeature`)
5. Abra um Pull Request

**Veja [CONTRIBUTING.md](docs/CONTRIBUTING.md) para mais detalhes.**

## ⚠️ Disclaimers Importantes

### Sobre Backup de Dados
- **SEMPRE faça backup** dos seus dados importantes antes de formatar
- Este tool facilita o backup do navegador, mas você é responsável por seus dados
- Teste a restauração do backup antes de formatar

### Sobre Ativação do Windows
- Este tool usa Microsoft Activation Scripts (código aberto)
- **Use apenas se você possui uma licença válida do Windows**
- Não nos responsabilizamos pelo uso indevido
- Método recomendado oficial: comprar chave de produto Microsoft

### Sobre Otimizações
- Algumas otimizações alteram configurações do sistema
- Um restore point é criado automaticamente
- Você pode reverter alterações via System Restore
- Use por sua conta e risco

### Sobre Privilégios de Administrador
- Este aplicativo requer privilégios de administrador
- Necessário para instalar software e modificar configurações do sistema
- Revise o código-fonte se tiver dúvidas sobre segurança

## 📝 Licença

Este projeto está licenciado sob a Licença MIT - veja o arquivo [LICENSE](LICENSE) para detalhes.

## 🙏 Agradecimentos

- [Chris Titus Tech](https://christitus.com/) - Windows optimization script
- [Microsoft Activation Scripts](https://github.com/massgravel/Microsoft-Activation-Scripts) - Activation tools
- [Material Design in XAML](http://materialdesigninxaml.net/) - Beautiful UI components
- Comunidade open source do Windows

## 📞 Suporte

- 🐛 [Reportar Bug](https://github.com/yourusername/windows-post-format-setup/issues)
- 💡 [Sugerir Feature](https://github.com/yourusername/windows-post-format-setup/issues)
- 📖 [Documentação Completa](docs/)
- ⭐ **Deixe uma estrela se este projeto foi útil!**

## 🗺️ Roadmap

- [x] Backup/Restore de perfil Brave
- [x] Instalação automática de ferramentas
- [x] Otimizações do Windows
- [x] Ativação automática do Windows
- [ ] Interface para seleção customizada de ferramentas
- [ ] Suporte para Firefox e outros navegadores
- [ ] Backup na nuvem integrado
- [ ] Perfis de instalação (Dev, Gaming, Office)
- [ ] Modo silencioso (CLI)
- [ ] Agendamento de backups automáticos
- [ ] Suporte para múltiplos idiomas

## 📊 Estatísticas

- **Ferramentas suportadas:** 30+
- **Tempo economizado:** ~3 horas por formatação
- **Linhas de código:** ~3,500+
- **Testes:** Em desenvolvimento

---

**Desenvolvido com ❤️ para a comunidade Windows**

```
  _       ___           __                   ____           __               ______            __
 | |     / (_)___  ____/ /___ _      _______/ __ \____  ___/ /_     ____   / ____/___  ___  / /
 | | /| / / / __ \/ __  / __ \ | /| / / ___/ /_/ / __ \/ __  (_)   / __ \ / /_  / __ \/ _ \/ / 
 | |/ |/ / / / / / /_/ / /_/ / |/ |/ (__  ) ____/ /_/ / /_/ /     / /_/ // __/ / /_/ /  __/_/  
 |__/|__/_/_/ /_/\__,_/\____/|__/|__/____/_/    \____/\__,_(_)    \____//_/    \____/\___(_)   
                                                                                                 
```

**⚡ Formate com confiança. Restaure com rapidez. ⚡**

