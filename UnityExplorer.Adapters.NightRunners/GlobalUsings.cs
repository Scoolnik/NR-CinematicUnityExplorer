// Global using directives

global using UnityEngine;

#if ML
global using Il2Cpp;
#endif

#if INTEROP
global using Il2CppInterop.Runtime;
#else
global using UnhollowerRuntimeLib;
global using UnhollowerBaseLib;
#endif