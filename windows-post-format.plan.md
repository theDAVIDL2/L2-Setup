# 🚀 Plano de Configuração Pós-Formatação do Windows

Este é um guia completo para configurar seu Windows após formatação usando os recursos disponíveis neste diretório.

---

## 📋 Índice

1. [Fase 1: Instalação de Drivers](#fase-1-instalação-de-drivers)
2. [Fase 2: Instalação de Runtimes](#fase-2-instalação-de-runtimes)
3. [Fase 3: Instalação de Aplicativos](#fase-3-instalação-de-aplicativos)
4. [Fase 4: Ativação do Windows/Office](#fase-4-ativação-do-windowsoffice)
5. [Fase 5: Configuração do Brave](#fase-5-configuração-do-brave)
6. [Fase 6: Otimização do Sistema](#fase-6-otimização-do-sistema)
7. [Fase 7: Configuração de Segurança](#fase-7-configuração-de-segurança)
8. [Checklist Final](#checklist-final)

---

## Fase 1: Instalação de Drivers

### 🎯 Objetivo
Instalar todos os drivers necessários para o funcionamento correto do hardware.

### 📝 Passos

1. **Executar instalador de drivers**
   ```batch
   Navegue para: INSTALE OS DRIVERS\
   Execute como Administrador: EXECUTE COMO ADMINISTRADOR.bat
   ```

2. **Aguardar instalação**
   - O script instalará todos os runtimes necessários
   - Reinicie o PC se solicitado

### ✅ Verificação
- [ ] Drivers instalados com sucesso
- [ ] Dispositivos funcionando no Gerenciador de Dispositivos
- [ ] PC reiniciado (se necessário)

---

## Fase 2: Instalação de Runtimes

### 🎯 Objetivo
Instalar todos os runtimes necessários (.NET, Visual C++, Java, etc.)

### 📝 Passos

1. **Opção A: Via arquivo RAR (Recomendado)**
   ```
   1. Extrair: FP_All In One Runtimes 4.6.7.zip
   2. Executar como Administrador: All In One Runtimes 4.6.7.exe
   3. Seguir o assistente de instalação
   ```

2. **Opção B: Via pasta INSTALE OS DRIVERS**
   ```
   Já inclui os runtimes necessários
   ```

### ✅ Verificação
- [ ] .NET Framework instalado
- [ ] Visual C++ Runtimes instalados
- [ ] DirectX atualizado
- [ ] Java Runtime instalado

---

## Fase 3: Instalação de Aplicativos

### 🎯 Objetivo
Instalar todos os aplicativos essenciais do sistema.

### 📝 Passos

#### 3.1 Aplicativos Automáticos (Via Ninite)

```
Executar: Ninite Brave Discord Steam WinRAR Installer.exe
```

**Instala:**
- ✅ Brave Browser
- ✅ Discord
- ✅ Steam
- ✅ WinRAR

#### 3.2 Aplicativos via Script Personalizado

```batch
Executar como Administrador: Scripts\Install APPS.bat
```

**Instala:**
- ✅ Visual Studio Code
- ✅ Node.js (LTS)
- ✅ Python 3.13.4
- ✅ System Informer (Process Hacker)
- ✅ IObit Unlocker
- ✅ Amazon Corretto 8 (JDK)
- ✅ Logitech G Hub
- ✅ JDownloader 2
- ⚠️ MSI Afterburner (instalação manual necessária)

#### 3.3 Instalações Manuais Individuais

Execute os instaladores na ordem:

```
1. python-3.13.2-amd64.exe
   - ✅ Marcar "Add Python to PATH"
   - ✅ Marcar "Install for all users"

2. Obsidian-1.8.7.exe
   - Instalação padrão

3. AdsPower-Global-6.12.6-x64.exe
   - Ver credenciais em: ADS POWER LOGIN.txt
   
4. Cloudflare_WARP_2025.1.861.0.msi
   - Instalação padrão
   
5. setup-lightshot.exe
   - Instalação padrão
```

### ✅ Verificação
- [ ] Todos os aplicativos do Ninite instalados
- [ ] Script Install APPS executado com sucesso
- [ ] Python instalado e no PATH
- [ ] Node.js instalado e funcionando
- [ ] Obsidian instalado
- [ ] AdsPower instalado
- [ ] Cloudflare WARP instalado
- [ ] Lightshot instalado

---

## Fase 4: Ativação do Windows/Office

### 🎯 Objetivo
Ativar Windows e Office usando KMS Tools.

### ⚠️ IMPORTANTE
- **DESATIVE o Windows Defender temporariamente**
- Execute como Administrador
- Restaure o Defender após ativação

### 📝 Passos

#### 4.1 Via PowerShell (Recomendado)

```powershell
# Ver instruções em:
KMS\ATIVAR VIA POWERSHELL.txt

# Método rápido:
irm https://massgrave.dev/get | iex
```

#### 4.2 Via KMS Tools (Alternativo)

```
1. Extrair: KMS\KMS Tools Lite Portable.zip
2. Executar como Administrador
3. Selecionar ativação do Windows
4. Selecionar ativação do Office (se instalado)
```

#### 4.3 Instalação do Office (Se necessário)

```
Navegar para: KMS\Office\
Seguir instruções em: ReadMe.txt

Ou usar:
KMS\Office\Office_Tool_with_runtime_v10.21.25.0_x64\
```

### ✅ Verificação
- [ ] Windows ativado (Verificar em: Configurações > Atualização e Segurança > Ativação)
- [ ] Office ativado (se instalado)
- [ ] Windows Defender reativado

---

## Fase 5: Configuração do Brave

### 🎯 Objetivo
Restaurar perfil do Brave ou configurar do zero.

### 📝 Passos

#### 5.1 Restaurar Perfil Existente

```batch
1. Executar: Scripts\Backup_Brave_Perfil.bat
2. Selecionar opção [2] - RESTAURAR
3. Escolher o backup disponível
4. Aguardar conclusão
```

#### 5.2 Configuração Manual (Se não houver backup)

```
1. Abrir Brave Browser
2. Configurações > Sincronização
3. Fazer login na conta Brave
4. Aguardar sincronização

OU

1. Copiar manualmente a pasta:
   BRAVE PROFILE\User Data\
   
   Para:
   %LOCALAPPDATA%\BraveSoftware\Brave-Browser\User Data\
```

**Localização do perfil:**
```
Ver: BRAVE PROFILE\Location.txt
```

### ✅ Verificação
- [ ] Brave restaurado com extensões
- [ ] Senhas sincronizadas
- [ ] Favoritos disponíveis
- [ ] Configurações preservadas

---

## Fase 6: Otimização do Sistema

### 🎯 Objetivo
Otimizar o Windows para melhor desempenho.

### 📝 Passos

#### 6.1 Otimização via Chris Titus Tech Utility (Recomendado)

```powershell
# Executar PowerShell como Administrador:
irm christitus.com/win | iex
```

**O que fazer na ferramenta:**
1. ✅ Tab "Tweaks" → Selecionar "Desktop" ou "Laptop"
2. ✅ Run Tweaks
3. ✅ Tab "Updates" → Configurar atualizações
4. ✅ Tab "Config" → Criar ponto de restauração

#### 6.2 Otimização via Script Local

```batch
Executar como Administrador: Scripts\Ultimate_Windows_Optimizer.bat
```

**Opções disponíveis:**
- [1] Otimizar Energia (Desempenho Máximo)
- [2] Otimizar Mouse (Desativar Aceleração)
- [3] Otimizar Efeitos Visuais
- [4] Desativar Serviços Desnecessários
- [5] Otimizar Explorador de Arquivos
- [6] Desativar Telemetria
- [7] Limpeza de Arquivos Temporários
- [8] **APLICAR TODAS AS OTIMIZAÇÕES** ⭐ (Recomendado)

#### 6.3 Otimizações Manuais Adicionais

**Desempenho Visual:**
```
1. Painel de Controle > Sistema > Configurações Avançadas
2. Desempenho > Configurações
3. Ajustar para melhor desempenho
4. Manter apenas:
   ✅ Suavizar bordas de fontes de tela
   ✅ Mostrar miniaturas em vez de ícones
```

**Plano de Energia:**
```
1. Painel de Controle > Opções de Energia
2. Selecionar: Alto Desempenho (ou Ultimate Performance)
```

**Desativar Inicialização Rápida:**
```
1. Painel de Controle > Opções de Energia
2. Escolher o que os botões de energia fazem
3. Alterar configurações não disponíveis
4. ❌ Desmarcar "Ativar inicialização rápida"
```

### ✅ Verificação
- [ ] Tweaks do Chris Titus aplicados
- [ ] Script Ultimate Optimizer executado
- [ ] Efeitos visuais otimizados
- [ ] Plano de energia configurado
- [ ] Sistema mais rápido e responsivo

---

## Fase 7: Configuração de Segurança

### 🎯 Objetivo
Configurar o Windows Defender e segurança do sistema.

### 📝 Passos

#### 7.1 Configuração do Windows Defender

```
Navegar para: 24122024\24122024\
```

**Opção A: Usar dControl**
```
1. Executar: dControl.exe
2. Configurar exclusões necessárias
3. Ajustar níveis de proteção
```

**Opção B: Usar Script VBS**
```
1. Executar como Administrador: Defender_Settings.vbs
2. Seguir instruções
```

**Ler documentação:**
```
24122024\24122024\ReadMe.txt
```

#### 7.2 Configurar Exclusões do Defender

**Para evitar conflitos com ferramentas:**
```
Windows Security > Proteção contra vírus e ameaças > Gerenciar configurações
> Exclusões > Adicionar ou remover exclusões

Adicionar:
- Pasta do KMS Tools (temporariamente)
- Pasta do IObit Unlocker
- Outros falsos positivos conforme necessário
```

#### 7.3 Firewall e Rede

```
1. Configurar Cloudflare WARP (já instalado)
2. Ativar Firewall do Windows
3. Bloquear conexões suspeitas
```

### ✅ Verificação
- [ ] Windows Defender configurado corretamente
- [ ] Exclusões necessárias adicionadas
- [ ] Firewall ativo
- [ ] Cloudflare WARP funcionando
- [ ] Sistema protegido

---

## Checklist Final

### 🔍 Verificação Completa do Sistema

#### Hardware e Drivers
- [ ] Todos os dispositivos funcionando
- [ ] GPU reconhecida e drivers atualizados
- [ ] Áudio funcionando
- [ ] Rede/Wi-Fi funcionando
- [ ] Periféricos (mouse, teclado, etc.) funcionando

#### Software Essencial
- [ ] Windows ativado
- [ ] Office ativado (se aplicável)
- [ ] Brave Browser configurado
- [ ] Discord funcionando
- [ ] Steam instalado
- [ ] VS Code instalado
- [ ] Python no PATH
- [ ] Node.js funcionando
- [ ] Git instalado (verificar: `git --version`)

#### Otimizações
- [ ] Plano de energia: Alto Desempenho
- [ ] Efeitos visuais otimizados
- [ ] Serviços desnecessários desativados
- [ ] Telemetria desativada
- [ ] Limpeza de temp realizada
- [ ] Inicialização rápida desativada

#### Segurança
- [ ] Windows Defender ativo
- [ ] Firewall ativo
- [ ] Windows Update configurado
- [ ] Cloudflare WARP ativo
- [ ] Exclusões configuradas

#### Backup e Recuperação
- [ ] Ponto de restauração criado
- [ ] Perfil do Brave com backup
- [ ] Documentos importantes salvos
- [ ] GitHub recovery codes seguros (ver: github-recovery-codes.txt)

#### Testes Finais
- [ ] Navegação web funcionando
- [ ] Downloads funcionando
- [ ] Áudio/vídeo funcionando
- [ ] Jogos iniciando (Steam)
- [ ] Captura de tela (Lightshot) funcionando
- [ ] Desenvolvimento (VS Code, Python, Node) funcionando

---

## 📚 Arquivos de Referência

### Credenciais e Configurações
```
ADS POWER LOGIN.txt          - Credenciais do AdsPower
github-recovery-codes.txt    - Códigos de recuperação GitHub
BRAVE PROFILE\Location.txt   - Localização do perfil Brave
```

### Documentação
```
24122024\24122024\ReadMe.txt                - Defender Control
KMS\Office\ReadMe.txt                       - Instalação Office
KMS\ATIVAR VIA POWERSHELL.txt              - Ativação via PowerShell
PROJECT_AUTOMATION_GUIDE.md                 - Guia de automação web
```

### Scripts Úteis
```
Scripts\Ultimate_Windows_Optimizer.bat      - Otimização completa
Scripts\Install APPS.bat                    - Instalação de apps
Scripts\Backup_Brave_Perfil.bat            - Backup/Restore Brave
Scripts\Win 11 optimizer.txt               - Chris Titus command
```

---

## 🛠️ Ferramentas Instaladas

### Desenvolvimento
- Visual Studio Code
- Python 3.13.x
- Node.js (LTS)
- Git
- Amazon Corretto 8 (Java)

### Utilitários
- Brave Browser
- Discord
- Obsidian
- Lightshot
- WinRAR
- 7-Zip
- System Informer (Process Hacker)
- IObit Unlocker
- JDownloader 2

### Gaming
- Steam
- MSI Afterburner (manual)

### Periféricos
- Logitech G Hub

### Rede
- Cloudflare WARP
- AdsPower

### Sistema
- All In One Runtimes
- KMS Tools
- dControl (Defender)

---

## 🚨 Troubleshooting

### Problema: Ativação do Windows falha
**Solução:**
1. Desativar Windows Defender temporariamente
2. Executar script como Administrador
3. Verificar conexão com internet
4. Tentar método alternativo (KMS Tools)

### Problema: Brave não restaura perfil
**Solução:**
1. Fechar completamente o Brave (Task Manager)
2. Renomear pasta atual: User Data → User Data.old
3. Executar novamente o script de restauração
4. Verificar caminhos no script

### Problema: Apps não instalam via Ninite
**Solução:**
1. Verificar conexão com internet
2. Desativar antivírus temporariamente
3. Executar como Administrador
4. Instalar manualmente se falhar

### Problema: Otimizações não aplicam
**Solução:**
1. Executar como Administrador
2. Desativar UAC temporariamente
3. Reiniciar PC após aplicar
4. Verificar logs de erro

### Problema: Drivers não instalam
**Solução:**
1. Baixar drivers específicos do fabricante
2. Usar Windows Update para drivers
3. Instalar manualmente do site do fabricante
4. Usar ferramentas como DriverBooster

---

## 📝 Notas Importantes

### ⚠️ Atenção
1. **Sempre** execute scripts como Administrador
2. **Sempre** crie ponto de restauração antes de otimizações
3. **Nunca** desative o Windows Defender permanentemente
4. **Sempre** faça backup antes de modificações importantes

### 💡 Dicas
1. Mantenha este diretório após formatação (em drive externo)
2. Atualize os instaladores periodicamente
3. Faça backup regular do perfil Brave
4. Mantenha lista de senhas em gerenciador seguro
5. Documente alterações personalizadas

### 🔄 Atualizações Recomendadas
Execute mensalmente:
```
1. Windows Update
2. Atualizar drivers (GeForce Experience, etc.)
3. Atualizar aplicativos (winget upgrade --all)
4. Limpeza de disco
5. Backup do perfil Brave
```

---

## ✅ Conclusão

Após completar todas as fases deste plano:

✅ Sistema completamente configurado
✅ Drivers instalados e funcionando
✅ Aplicativos essenciais instalados
✅ Sistema otimizado para desempenho
✅ Segurança configurada adequadamente
✅ Backups e recuperação configurados

**Tempo total estimado:** 2-4 horas (dependendo da velocidade da internet)

---

## 🎯 Próximos Passos

Após a configuração inicial:

1. **Configurar sincronização de arquivos**
   - OneDrive, Google Drive, ou similar
   
2. **Instalar software específico**
   - IDEs adicionais
   - Ferramentas de trabalho
   - Jogos
   
3. **Personalizar ambiente**
   - Papel de parede
   - Temas
   - Atalhos personalizados
   
4. **Configurar projetos de desenvolvimento**
   - Clonar repositórios GitHub
   - Configurar ambientes virtuais
   - Instalar dependências de projetos

---

**Criado:** $(date)
**Versão:** 1.0
**Baseado em:** Conteúdo do diretório "EXECUTAR DEPOIS DE FORMATAR"

---

**BOA SORTE COM SUA CONFIGURAÇÃO! 🚀**

