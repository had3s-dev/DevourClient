# DevourClient - Updated for Current Game Version

## ⚠️ Important Update Notice
This cheat menu has been updated to work with the current version of Devour (2024-2025). The main compatibility improvements include:

- **Enhanced IL2CPP compatibility** - Better handling of Unity IL2CPP changes
- **Improved error handling** - More robust null checks and exception handling
- **Updated game detection** - Better player and entity finding methods
- **Memory safety improvements** - Reduced crash potential
- **Photon Bolt compatibility** - Updated networking references

## 🆕 What's New in This Update

### Compatibility Fixes
- **Fixed player detection** - Now properly finds local player in newer game versions
- **Enhanced entity finding** - Better handling of null objects and missing components
- **Updated flashlight controls** - Improved big flashlight and fullbright features
- **Better error logging** - More detailed debug information when features fail

### New Features
- **Fallback detection methods** - Multiple ways to find game objects
- **Graceful degradation** - Features continue working even if some components fail
- **Improved stability** - Reduced crashes due to missing game objects

## 🎮 Installation (Updated Process)

### Prerequisites
1. **Devour** - Latest version from Steam
2. **MelonLoader v0.6.4+** - [Download here](https://github.com/LavaGang/MelonLoader/releases)
3. **.NET 6 SDK** - [Download here](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

### Quick Install
1. **Install MelonLoader** to your Devour directory
2. **Build the cheat** using the included build script:
   ```
   double-click build.bat
   ```
3. **Copy the DLL** to: `C:\Program Files (x86)\Steam\steamapps\common\Devour\Mods\`
4. **Start the game** - Press INSERT to open the menu

## 🔧 Building from Source (Updated)

### Option 1: Automated Build
```
build.bat
```

### Option 2: Manual Build
```cmd
cd DevourClient
dotnet restore
dotnet build --configuration Release
```

### Option 3: Visual Studio
1. Open `DevourClient.sln` in Visual Studio 2022+
2. Add references (see below)
3. Build in Release mode

### Required References (Updated List)
Add these references to your project:
- `MelonLoader.dll` - From MelonLoader\net6\
- `0Harmony.dll` - From MelonLoader\net6\
- `Il2CppInterop.Runtime.dll` - From MelonLoader\net6\
- `Assembly-CSharp.dll` - From MelonLoader\Il2CppAssemblies\
- All UnityEngine modules from MelonLoader\Il2CppAssemblies\

## 🎯 Features Status (Updated Compatibility)

### ✅ Working Features
- **Visual Modifications**
  - Big Flashlight ✓
  - Fullbright ✓
  - Flashlight Color ✓
  - Crosshair ✓

- **ESP & Visuals**
  - Player ESP ✓
  - Player Skeleton ESP ✓
  - Azazel ESP ✓
  - Item ESP ✓
  - Demon/Entity ESP ✓

- **Game Modifications**
  - Fly Mode ✓
  - Speed Modifier ✓
  - Walk in Lobby ✓
  - Unlimited UV Light ✓

- **Map Features**
  - Instant Win ✓
  - TP to Azazel ✓
  - Burn Ritual Objects ✓
  - Despawn Entities ✓

- **Spawning**
  - Spawn Azazel variants ✓
  - Spawn Demons ✓
  - Spawn Animals ✓

### ⚠️ Potentially Broken Features
- **Achievements Unlocker** - May not work with newer achievement system
- **Steam Name Spoofer** - May be restricted by Steam
- **Server Name Spoofer** - May have new restrictions

## 🛠️ Troubleshooting

### Common Issues

#### "Menu doesn't open"
- Ensure MelonLoader is properly installed
- Check console for error messages
- Try pressing INSERT multiple times

#### "Features not working"
- Check if you're in the correct game state (lobby vs in-game)
- Verify you have host privileges for spawning features
- Check console for specific error messages

#### "Game crashes on startup"
- Ensure all required DLLs are present
- Check MelonLoader version compatibility
- Verify .NET 6 runtime is installed

### Debug Mode
Enable debug logging by adding this to `MelonLoader.cfg`:
```
[Logging]
Debug = true
```

## 📞 Support

### Getting Help
1. **Check console output** - Most issues are logged there
2. **Verify game version** - Ensure you're using latest Devour
3. **Test in single-player** first before multiplayer

### Reporting Issues
When reporting problems, include:
- Devour game version
- MelonLoader version
- Specific error messages from console
- Steps to reproduce the issue

## 🔒 Safety Notes

- **No anti-cheat detected** - Devour currently uses no anti-cheat
- **Use at your own risk** - Always possible of future detection
- **Host-only features** - Some features require host privileges
- **Single-player recommended** - For testing new features

## 🔄 Updates

This cheat will be updated as new Devour versions are released. Check the GitHub repository for the latest compatibility updates.

---

**Last Updated**: January 2025  
**Game Version**: Devour 5.2.11  
**MelonLoader**: v0.6.4+  
**Unity**: IL2CPP (2022.3+)
