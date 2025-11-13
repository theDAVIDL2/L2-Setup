# 🎭 System Identity Spoofer

## Overview

The System Identity Spoofer is a comprehensive tool for security testing that allows you to modify system-level identifiers and network settings. It's integrated into the L2 Setup application as a specialized tab.

## ⚠️ WARNING

**This is an advanced tool intended for security testing, research, and legitimate privacy purposes only.**

### Potential Risks:
- Windows activation issues
- Software licensing problems
- Network connectivity issues
- System instability
- Hardware compatibility issues

### Best Practices:
- ✅ Always create backups before spoofing
- ✅ Test on a virtual machine first
- ✅ Understand what each option does
- ✅ Requires Administrator privileges
- ✅ Some changes require system reboot

---

## Features

### 1. Machine GUID Spoofing 🔧

**What it does:**
- Changes Windows Machine GUID
- Modifies SQM Client ID
- Updates Hardware Profile GUID

**Registry locations modified:**
- `HKLM\SOFTWARE\Microsoft\Cryptography\MachineGuid`
- `HKLM\SOFTWARE\Microsoft\SQMClient\MachineId`
- `HKLM\SYSTEM\CurrentControlSet\Control\IDConfigDB\Hardware Profiles\0001\HwProfileGuid`

**Use cases:**
- Security testing
- Privacy research
- System reinstallation simulation

**Reboot required:** ⚠️ Recommended

---

### 2. Hardware ID Spoofing 🖥️

**What it does:**
- Spoofs disk serial numbers (registry-based)
- Modifies motherboard UUID entries
- Updates computer hardware ID

**Limitations:**
- Some hardware IDs are firmware-based and cannot be fully changed
- This tool modifies registry entries that most software checks
- Real SMBIOS is in firmware but registry entries are spoofed

**Registry locations modified:**
- `HKLM\SYSTEM\CurrentControlSet\Services\Disk\Enum`
- `HKLM\SYSTEM\CurrentControlSet\Control\SystemInformation`
- `HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate`

**Reboot required:** ⚠️ Highly recommended

---

### 3. MAC Address Spoofing 🌐

**What it does:**
- Real-time MAC address modification
- Per-adapter control
- Automatic adapter restart

**How it works:**
1. Enumerates all network adapters via WMI
2. Modifies registry entry for selected adapter
3. Restarts the network adapter to apply changes
4. Generates valid locally administered MAC addresses

**Registry location:**
- `HKLM\SYSTEM\CurrentControlSet\Control\Class\{4D36E972-E325-11CE-BFC1-08002BE10318}\[AdapterID]\NetworkAddress`

**Features:**
- ✅ No reboot required (real-time)
- ✅ Per-adapter selection
- ✅ Automatic MAC generation
- ✅ Valid format (locally administered bit set)

**Reboot required:** ❌ No (changes are immediate)

---

### 4. Volume Serial Spoofing 💽

**Status:** Experimental / Placeholder

**What it does:**
- Displays volume information
- Shows current serial numbers

**Limitations:**
- ⚠️ **Actual spoofing is DISABLED for safety**
- Changing volume serials may cause:
  - Windows activation issues
  - Software malfunctions
  - Data corruption risks

**Why it's disabled:**
- Volume serial modification requires low-level disk access or external tools (like VolumeID.exe)
- High risk of system instability
- Intentionally left as a placeholder with warnings

---

### 5. Tor Integration 🧅

**What it does:**
- Downloads Tor Expert Bundle
- Configures Tor for anonymous browsing
- Starts/Stops Tor service
- Configures Windows proxy settings
- Checks current IP address

**Features:**
- ✅ Automatic download and installation (~10-15 MB)
- ✅ SOCKS5 proxy on `127.0.0.1:9050`
- ✅ Control port on `9051`
- ✅ System proxy auto-configuration
- ✅ Real-time IP checking
- ✅ Start/Stop toggle

**Installation location:**
- `%LocalAppData%\L2Setup\Tor\`

**How to use:**
1. Click "Download Tor" button
2. Wait for download and extraction (~10-15 seconds)
3. Toggle "Start Tor" to enable
4. Your connection is now routed through Tor
5. Check your IP to verify anonymity

**Reboot required:** ❌ No

---

### 6. OpenVPN Support 🔐

**Status:** Placeholder / Coming Soon

**Planned features:**
- Import .ovpn configuration files
- Connect/Disconnect VPN
- Store credentials securely (DPAPI)
- Connection status monitoring

---

## Backup & Restore System

### Automatic Backups

**What is backed up:**
- Registry entries (all spoofed values)
- Original MAC addresses
- Original volume serials
- Network proxy settings

**Backup location:**
- `%LocalAppData%\L2Setup\SystemIdBackups\`

**Backup format:**
- JSON files with unique IDs
- Timestamped for easy identification

### How to Restore

1. Go to "Backups" tab
2. Select the backup you want to restore
3. Click "Restore Selected Backup"
4. Reboot (recommended)

**Note:** Auto-backup is enabled by default before any spoofing operation.

---

## Quick Actions

### Spoof All Machine IDs

**What it does:**
- Spoofs all Machine GUIDs
- Spoofs all Hardware IDs
- Changes all MAC addresses

**Includes:**
- Automatic backup creation
- Progress logging
- Detailed completion report

**Use this when:**
- You want a complete system identity change
- Testing anti-fingerprinting effectiveness
- Simulating a new machine

### Restore All to Original

**What it does:**
- Restores the most recent backup
- Reverts all spoofed identifiers
- Restores network settings

**Use this when:**
- You want to undo all changes
- Experiencing issues after spoofing
- Returning to normal operation

---

## Safety Features

### 1. Pre-Flight Checks
- Administrator privileges verification
- Existing backup detection
- User confirmation dialogs

### 2. Automatic Rollback
- Failed operations trigger automatic restore (where possible)
- Backup created before every operation
- Error logging for troubleshooting

### 3. User Warnings
- Clear warnings for risky operations
- Detailed explanations of what will change
- Reboot recommendations

### 4. Detailed Logging
- All operations are logged
- Timestamped entries
- Success/failure indicators
- Error messages with details

---

## Technical Details

### Technologies Used
- **WMI (Windows Management Instrumentation)**: Network adapter enumeration
- **Windows Registry**: GUID, Hardware ID, and MAC address modifications
- **Tor Expert Bundle**: Anonymous networking
- **SharpCompress**: Archive extraction
- **Material Design**: Modern UI

### Registry-Based Approach
- No Test Mode or driver signature enforcement bypass required
- All changes are registry-based
- Compatible with Secure Boot
- Works on all Windows versions (10/11)

### MAC Address Generation
- Uses cryptographically random bytes
- Sets locally administered bit (bit 1 of first byte)
- Clears multicast bit (bit 0 of first byte)
- Format: `02-XX-XX-XX-XX-XX` or `06-XX-XX-XX-XX-XX`, etc.

---

## Troubleshooting

### Common Issues

#### "Failed to start Tor"
- **Solution**: Download Tor again, check firewall settings, run as Administrator

#### "MAC address spoofing failed"
- **Solution**: Some adapters enforce hardware MAC, try a different adapter

#### "System activation issues after spoofing"
- **Solution**: Restore from backup, re-activate Windows if needed

#### "Network not working after MAC change"
- **Solution**: Restart computer, restore MAC address from backup

### Recovery Steps

1. **Use Restore Functionality**: Go to Backups tab → Select backup → Restore
2. **Manual Registry Restore**: Use Windows Registry Editor (backup registry first!)
3. **System Restore Point**: Windows may have created an automatic restore point
4. **Safe Mode**: Boot into Safe Mode and restore changes

---

## Limitations

### Cannot Be Changed via Software:
1. **CPU Serial Number**: Burned into the processor
2. **BIOS Serial** (real): Stored in firmware
3. **GPU Serial**: Hardware-based
4. **Motherboard Serial** (hardware): Physical serial on board

### Partially Changed:
1. **Disk Serials**: Registry entries changed, but hardware serial remains
2. **Motherboard UUID**: Registry entries changed, SMBIOS in firmware unchanged

### Fully Changed:
1. **Windows Machine GUID**: ✅ Complete change
2. **MAC Addresses**: ✅ Complete change (registry-based)
3. **SQM Client ID**: ✅ Complete change
4. **Hardware Profile GUID**: ✅ Complete change

---

## Use Cases

### Legitimate Use Cases:
1. **Security Research**: Testing anti-fingerprinting techniques
2. **Privacy Testing**: Evaluating tracking mechanisms
3. **System Migration**: Simulating hardware changes
4. **VM Testing**: Testing software on "different" machines
5. **Development**: Testing hardware-dependent software

### ⚠️ Prohibited Use Cases:
- Ban evasion
- License fraud
- Illegal activities
- Unauthorized access
- Deceiving anti-cheat systems

---

## FAQ

**Q: Will this bypass hardware bans?**
A: Maybe. It depends on what identifiers the software checks. This tool changes registry-based identifiers, but not hardware-level identifiers.

**Q: Is this detectable?**
A: Yes. Anti-cheat software and security tools can detect registry modifications and inconsistencies between hardware and registry values.

**Q: Can I use this with games?**
A: Not recommended. Many anti-cheat systems will detect and ban you for this.

**Q: Will Windows activation be affected?**
A: Possibly. Windows ties activation to hardware IDs. Restore from backup if issues occur.

**Q: Is Tor really anonymous?**
A: Tor provides strong anonymity but is not perfect. Use proper operational security (OpSec) practices.

**Q: Can I customize MAC addresses?**
A: Currently, random generation only. Custom MAC input is a potential future feature.

---

## Credits

- **Tor Project**: https://www.torproject.org/
- **Material Design in XAML**: UI framework
- **SharpCompress**: Archive extraction library

---

## Legal Disclaimer

This tool is provided for educational and security research purposes only. The authors and contributors are not responsible for misuse of this tool. Users are responsible for ensuring their use complies with all applicable laws and regulations.

**Use at your own risk.**

---

## Version History

### v1.0.0 (Current)
- ✅ Core infrastructure
- ✅ Machine GUID spoofing
- ✅ Hardware ID spoofing
- ✅ MAC address spoofing (real-time)
- ✅ Backup & restore system
- ✅ Tor integration (download, install, start/stop)
- ✅ IP address checking
- ⏸️ Volume serial spoofing (placeholder)
- ⏸️ OpenVPN integration (placeholder)

### Planned Features
- Custom MAC address input
- Advanced Tor configuration
- OpenVPN full integration
- Hardware spoofer for additional components
- Scheduled auto-restore
- Stealth mode (hide changes from detection)

---

## Support

For issues, questions, or feature requests, please refer to the main L2 Setup repository or documentation.

**Remember: Always backup first, test on VM, understand the risks.**

🎭 **Happy (ethical) spoofing!**

