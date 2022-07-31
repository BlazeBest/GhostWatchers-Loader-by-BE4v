#pragma once

#include "..\InternalHelpers.hpp"
#include "HookTypes.h"
#include "il2cpp.h"
#include "CLRHost.h"
#include "detours/detours.h"
#include <queue>
#include <functional>

class GWLoader
{
public:
	static bool bLoadedMods;

	static void Init_Loader();
	static void Destroy_Loader();

	static void LoadAssemblies();
};

class Hooking
{
public:
	static LoadLibraryW_t fnLoadLibraryW;
	static HMODULE __stdcall _LoadLibraryW(LPCWSTR lpLibFileName);
};