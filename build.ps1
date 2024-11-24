cd UniverseLib
.\build.ps1
cd ..

# ----------- MelonLoader IL2CPP CoreCLR (net6) -----------
dotnet build src/CinematicUnityExplorer.sln -c Release_ML_Cpp_CoreCLR -p:IS_CI=true
$Path = "Release\CinematicUnityExplorer.MelonLoader.IL2CPP.CoreCLR"

New-Item -ItemType Directory -Path "$Path/output/Mods" -Force
New-Item -ItemType Directory -Path "$Path/output/UserLibs" -Force

# ILRepack
lib/ILRepack.exe /target:library /lib:lib/net6 /lib:lib/interop /lib:$Path /internalize /out:$Path/output/Mods/CinematicUnityExplorer.ML.IL2CPP.CoreCLR.dll $Path/CinematicUnityExplorer.ML.IL2CPP.CoreCLR.dll $Path/mcs.dll

Move-Item -Path $Path/UniverseLib.ML.IL2CPP.Interop.dll -Destination $Path/output/UserLibs -Force

# ----------- MelonLoader IL2CPP (net472) -----------
dotnet build src/CinematicUnityExplorer.sln -c Release_ML_Cpp_net472 -p:IS_CI=true
$Path = "Release/CinematicUnityExplorer.MelonLoader.IL2CPP"

New-Item -ItemType Directory -Path "$Path/output/Mods" -Force
New-Item -ItemType Directory -Path "$Path/output/UserLibs" -Force

# ILRepack
lib/ILRepack.exe /target:library /lib:lib/net472 /lib:lib/net35 /lib:lib/unhollowed /lib:$Path /internalize /out:$Path/output/Mods/CinematicUnityExplorer.ML.IL2CPP.dll $Path/CinematicUnityExplorer.ML.IL2CPP.dll $Path/mcs.dll

# (cleanup and move files)
Move-Item -Path $Path/UniverseLib.IL2CPP.Unhollower.dll -Destination $Path/output/UserLibs -Force

# ----------- BepInEx Unity IL2CPP CoreCLR -----------
dotnet build src/CinematicUnityExplorer.sln -c Release_BIE_Unity_Cpp -p:IS_CI=true
$Path = "Release/CinematicUnityExplorer.BepInEx.Unity.IL2CPP.CoreCLR"
$OutputPath = "$Path/output/BepInEx/plugins/CinematicUnityExplorer"

New-Item -ItemType Directory -Path $OutputPath -Force

# ILRepack
lib/ILRepack.exe /target:library /lib:lib/net472/BepInEx/build647+ /lib:lib/net6/ /lib:lib/interop/ /lib:$Path /internalize /out:$OutputPath/CinematicUnityExplorer.BIE.Unity.IL2CPP.CoreCLR.dll $Path/CinematicUnityExplorer.BIE.Unity.IL2CPP.CoreCLR.dll $Path/mcs.dll $Path/Tomlet.dll

# (cleanup and move files)
Move-Item -Path $Path/UniverseLib.BIE.IL2CPP.Interop.dll -Destination $OutputPath -Force

