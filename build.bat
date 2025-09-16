@echo off
echo Building DevourClient for Devour 5.2.11...
echo ===========================================

REM Check if .NET 6 SDK is available
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo Error: .NET 6 SDK is not installed. Please install it from https://dotnet.microsoft.com/en-us/download/dotnet/6.0
    pause
    exit /b 1
)

REM Set Devour path
echo Locating Devour installation...
set "DEVOUR_PATH=D:\SteamLibrary\steamapps\common\Devour"

REM Check if Devour exists
if not exist "%DEVOUR_PATH%" (
    echo Error: Devour not found at %DEVOUR_PATH%
    echo Please ensure Devour is installed via Steam
    pause
    exit /b 1
)

REM Check for MelonLoader
echo Checking MelonLoader installation...
if not exist "%DEVOUR_PATH%\MelonLoader\net6\MelonLoader.dll" (
    echo Error: MelonLoader not found!
    echo Please install MelonLoader v0.6.4+ to Devour first:
    echo 1. Download from: https://github.com/LavaGang/MelonLoader/releases
    echo 2. Install to: %DEVOUR_PATH%
    echo 3. Run Devour once to complete installation
    pause
    exit /b 1
)

echo MelonLoader found! Proceeding with build...

REM Create temporary project file with correct references
echo Creating build configuration...
set "PROJECT_PATH=%~dp0DevourClient"
set "MELON_LOADER_PATH=%DEVOUR_PATH%\MelonLoader\net6"
set "IL2CPP_ASSEMBLIES_PATH=%DEVOUR_PATH%\MelonLoader\Il2CppAssemblies"

REM Verify all required assemblies exist
echo Verifying assembly locations...
    
set "MISSING_ASSEMBLIES="
if not exist "%IL2CPP_ASSEMBLIES_PATH%\UnityEngine.dll" set "MISSING_ASSEMBLIES=%MISSING_ASSEMBLIES% UnityEngine"
if not exist "%IL2CPP_ASSEMBLIES_PATH%\UnityEngine.CoreModule.dll" set "MISSING_ASSEMBLIES=%MISSING_ASSEMBLIES% UnityEngine.CoreModule"
if not exist "%IL2CPP_ASSEMBLIES_PATH%\Assembly-CSharp.dll" set "MISSING_ASSEMBLIES=%MISSING_ASSEMBLIES% Assembly-CSharp"
if not exist "%IL2CPP_ASSEMBLIES_PATH%\Il2Cppmscorlib.dll" set "MISSING_ASSEMBLIES=%MISSING_ASSEMBLIES% Il2Cppmscorlib"
    
if not "%MISSING_ASSEMBLIES%"=="" (
    echo ERROR: Missing assemblies:%MISSING_ASSEMBLIES%
    echo.
    echo This usually means:
    echo 1. MelonLoader isn't fully installed
    echo 2. Devour hasn't been run after MelonLoader installation
    echo 3. Wrong MelonLoader version
    echo.
    echo SOLUTION: Run Devour once with MelonLoader installed to generate assemblies
    pause
    exit /b 1
)

REM Create a clean project file without duplicate AssemblyInfo
echo ^<Project Sdk="Microsoft.NET.Sdk"^> > "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo   ^<PropertyGroup^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^<TargetFramework^>net6.0^</TargetFramework^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^<LangVersion^>10.0^</LangVersion^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^<AllowUnsafeBlocks^>true^</AllowUnsafeBlocks^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^<Nullable^>enable^</Nullable^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^<AssemblyName^>DevourClient^</AssemblyName^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^<GenerateAssemblyInfo^>false^</GenerateAssemblyInfo^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo   ^</PropertyGroup^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo   ^<ItemGroup^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^<Reference Include="MelonLoader"^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo       ^<HintPath^>%MELON_LOADER_PATH%\MelonLoader.dll^</HintPath^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^</Reference^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^<Reference Include="0Harmony"^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo       ^<HintPath^>%MELON_LOADER_PATH%\0Harmony.dll^</HintPath^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^</Reference^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^<Reference Include="Il2CppInterop.Runtime"^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo       ^<HintPath^>%MELON_LOADER_PATH%\Il2CppInterop.Runtime.dll^</HintPath^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^</Reference^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^<Reference Include="UnityEngine"^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo       ^<HintPath^>%IL2CPP_ASSEMBLIES_PATH%\UnityEngine.dll^</HintPath^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^</Reference^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^<Reference Include="UnityEngine.CoreModule"^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo       ^<HintPath^>%IL2CPP_ASSEMBLIES_PATH%\UnityEngine.CoreModule.dll^</HintPath^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^</Reference^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^<Reference Include="UnityEngine.IMGUIModule"^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo       ^<HintPath^>%IL2CPP_ASSEMBLIES_PATH%\UnityEngine.IMGUIModule.dll^</HintPath^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^</Reference^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^<Reference Include="UnityEngine.UI"^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo       ^<HintPath^>%IL2CPP_ASSEMBLIES_PATH%\UnityEngine.UI.dll^</HintPath^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^</Reference^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^<Reference Include="Assembly-CSharp"^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo       ^<HintPath^>%IL2CPP_ASSEMBLIES_PATH%\Assembly-CSharp.dll^</HintPath^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^</Reference^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^<Reference Include="Il2Cppmscorlib"^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo       ^<HintPath^>%IL2CPP_ASSEMBLIES_PATH%\Il2Cppmscorlib.dll^</HintPath^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^</Reference^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^<Reference Include="Il2CppPhoton.Bolt"^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo       ^<HintPath^>%IL2CPP_ASSEMBLIES_PATH%\Il2CppPhoton.Bolt.dll^</HintPath^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^</Reference^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^<Reference Include="Il2CppOpsive.UltimateCharacterController"^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo       ^<HintPath^>%IL2CPP_ASSEMBLIES_PATH%\Il2CppOpsive.UltimateCharacterController.dll^</HintPath^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^</Reference^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^<Reference Include="Il2CppPhoton.Bolt"^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo       ^<HintPath^>%IL2CPP_ASSEMBLIES_PATH%\Il2CppPhoton.Bolt.dll^</HintPath^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^</Reference^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^<Reference Include="Il2CppPhoton.Bolt.User"^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo       ^<HintPath^>%IL2CPP_ASSEMBLIES_PATH%\Il2CppPhoton.Bolt.User.dll^</HintPath^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^</Reference^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^<Reference Include="Il2CppSteamworks"^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo       ^<HintPath^>%IL2CPP_ASSEMBLIES_PATH%\Il2Cppcom.rlabrecque.steamworks.net.dll^</HintPath^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^</Reference^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^<Reference Include="Il2CppUdpKit"^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo       ^<HintPath^>%IL2CPP_ASSEMBLIES_PATH%\Il2Cppudpkit.dll^</HintPath^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^</Reference^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^<Reference Include="Il2CppUdpKit.Platform.Photon"^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo       ^<HintPath^>%IL2CPP_ASSEMBLIES_PATH%\Il2Cppudpkit.platform.photon.dll^</HintPath^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^</Reference^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^<Reference Include="Il2Cppbolt.user"^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo       ^<HintPath^>%IL2CPP_ASSEMBLIES_PATH%\Il2Cppbolt.user.dll^</HintPath^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^</Reference^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^<Reference Include="Il2Cppbolt"^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo       ^<HintPath^>%IL2CPP_ASSEMBLIES_PATH%\Il2Cppbolt.dll^</HintPath^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^</Reference^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^<Reference Include="UnityEngine.AnimationModule"^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo       ^<HintPath^>%IL2CPP_ASSEMBLIES_PATH%\UnityEngine.AnimationModule.dll^</HintPath^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^</Reference^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^<Reference Include="UnityEngine.PhysicsModule"^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo       ^<HintPath^>%IL2CPP_ASSEMBLIES_PATH%\UnityEngine.PhysicsModule.dll^</HintPath^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^</Reference^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^<Reference Include="UnityEngine.InputModule"^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo       ^<HintPath^>%IL2CPP_ASSEMBLIES_PATH%\UnityEngine.InputModule.dll^</HintPath^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^</Reference^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^<Reference Include="UnityEngine.InputLegacyModule"^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo       ^<HintPath^>%IL2CPP_ASSEMBLIES_PATH%\UnityEngine.InputLegacyModule.dll^</HintPath^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^</Reference^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^<Reference Include="UnityEngine.UIModule"^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo       ^<HintPath^>%IL2CPP_ASSEMBLIES_PATH%\UnityEngine.UIModule.dll^</HintPath^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^</Reference^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^<Reference Include="Unity.TextMeshPro"^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo       ^<HintPath^>%IL2CPP_ASSEMBLIES_PATH%\Unity.TextMeshPro.dll^</HintPath^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^</Reference^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^<Reference Include="Il2Cppudpkit.common"^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo       ^<HintPath^>%IL2CPP_ASSEMBLIES_PATH%\Il2Cppudpkit.common.dll^</HintPath^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^</Reference^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^<Reference Include="mscorlib"^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo       ^<HintPath^>%IL2CPP_ASSEMBLIES_PATH%\Il2Cppmscorlib.dll^</HintPath^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo     ^</Reference^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo   ^</ItemGroup^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo ^</Project^> >> "%PROJECT_PATH%\DevourClient_5_2_11.csproj"

echo Building with Devour 5.2.11 references...
cd "%PROJECT_PATH%"
dotnet build DevourClient_5_2_11.csproj --configuration Release --verbosity minimal

if %errorlevel% neq 0 (
    echo.
    echo Build failed! Check the error messages above.
    echo Common issues:
    echo 1. Ensure Devour 5.2.11 is installed via Steam
    echo 2. Ensure MelonLoader v0.6.4+ is properly installed
    echo 3. Run Devour once after installing MelonLoader
    pause
    del "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
    exit /b 1
)

echo.
echo ===========================================
echo Build completed successfully for Devour 5.2.11!
echo ===========================================
echo.
REM Cleanup output folder: keep only DevourClient.dll
set "OUTPUT_DIR=%PROJECT_PATH%\bin\Release\net6.0"
echo Cleaning build output (keeping only DevourClient.dll)...
if exist "%OUTPUT_DIR%" (
    for %%F in ("%OUTPUT_DIR%\*.dll") do (
        if /I not "%%~nxF"=="DevourClient.dll" del /q "%%~fF" >nul 2>&1
    )
    del /q "%OUTPUT_DIR%\*.pdb" >nul 2>&1
    del /q "%OUTPUT_DIR%\*.xml" >nul 2>&1
    del /q "%OUTPUT_DIR%\*.json" >nul 2>&1
)
echo Cleanup complete.
echo.
echo The compiled DLL can be found in:
echo   DevourClient\bin\Release\net6.0\DevourClient.dll
echo.
echo To install:
echo 1. Copy DevourClient.dll to:
echo    %DEVOUR_PATH%\Mods\
echo 2. Start Devour 5.2.11
echo 3. Press INSERT to open the cheat menu
echo.
echo Cleaning up temporary files...
del "%PROJECT_PATH%\DevourClient_5_2_11.csproj"
echo Done!
pause

echo.
echo ===========================================
echo Build completed successfully for Devour 5.2.11!
echo ===========================================
echo.
echo The compiled DLL can be found in:
echo   DevourClient\bin\Release\net6.0\DevourClient.dll
echo.
echo To install:
echo 1. Copy DevourClient.dll to:
echo    C:\Program Files (x86)\Steam\steamapps\common\Devour\Mods\
echo 2. Ensure MelonLoader v0.6.4+ is installed
echo 3. Start Devour 5.2.11
echo 4. Press INSERT to open the cheat menu
echo.
echo For Devour 5.2.11 support, ensure you're using the latest MelonLoader!
pause
