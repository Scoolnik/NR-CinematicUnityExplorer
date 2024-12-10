# ----------- BepInEx Unity IL2CPP CoreCLR -----------
$Path = "Release/CinematicUnityExplorer.BepInEx.Unity.IL2CPP.CoreCLR"
$OutputPath = "$Path/output/BepInEx/plugins/CinematicUnityExplorer"

dotnet build src/CinematicUnityExplorer.sln -c Release_BIE_Unity_Cpp -p:IS_CI=true

New-Item -ItemType Directory -Path $OutputPath -Force

# ILRepack
lib/ILRepack.exe /target:library /lib:lib/net472/BepInEx/build647+ /lib:lib/net6/ /lib:lib/interop/ /lib:$Path /internalize /out:$OutputPath/CinematicUnityExplorer.BIE.Unity.IL2CPP.CoreCLR.dll $Path/CinematicUnityExplorer.BIE.Unity.IL2CPP.CoreCLR.dll $Path/mcs.dll $Path/Tomlet.dll

# (cleanup and move files)
Move-Item -Path $Path/UniverseLib.BIE.IL2CPP.Interop.dll -Destination $OutputPath -Force
Move-Item -Path $Path/CinematicUnityExplorer.Adapters*.dll -Destination $OutputPath -Force

