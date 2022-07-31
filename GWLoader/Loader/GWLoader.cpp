#include "GWLoader.h"
#include "Exports.h"
#include "FileManager.h"

bool GWLoader::bLoadedMods = false;

void GWLoader::Init_Loader()
{
#if (DEBUG)
	ConsoleUtils::Log("Started GWLoader!");
#endif
	FileManager::Initialize();
	if (!Hooking::fnLoadLibraryW)
		Hooking::fnLoadLibraryW = LoadLibraryW;
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	if (!Hooking::fnLoadLibraryW)
		return;
#if (DEBUG)
	ConsoleUtils::Log("Installing LLW hooks...");
#endif
	DetourAttach(&(LPVOID&)Hooking::fnLoadLibraryW, Hooking::_LoadLibraryW);
	DetourTransactionCommit();
}

void GWLoader::Destroy_Loader()
{
	CLRHost::ReleaseCLR();
}

void GWLoader::LoadAssemblies()
{
	bLoadedMods = true;
	if (CLRHost::Initialize())
	{
#if (DEBUG)
		ConsoleUtils::Log("Successfully loaded all NET Mods");
#endif
	}
	else
	{
#if (DEBUG)
		ConsoleUtils::Log("Failed to load NET Mods");
#endif
	}
}

LoadLibraryW_t Hooking::fnLoadLibraryW = NULL;
HMODULE __stdcall Hooking::_LoadLibraryW(LPCWSTR lpLibFileName) {
	HMODULE lib = fnLoadLibraryW(lpLibFileName);
	std::wcout << "Loaded assembly W: " << lpLibFileName << std::endl;
	if (wcsstr(lpLibFileName, L"steam_api64.dll") != 0)
	{
#if (DEBUG)
		ConsoleUtils::Log("Captured steam_api64.dll LLW! Starting hooks...");
#endif
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());

		CLRHost::HostCLR();
		// SteamAPI_Init
	}
	if (wcsstr(lpLibFileName, L"dxgi.dll") != 0 && !GWLoader::bLoadedMods)
	{
#if (DEBUG)
		ConsoleUtils::Log("Unity has been loaded!");
#endif
		GWLoader::LoadAssemblies();

#if (DEBUG)
		ConsoleUtils::Log("Game has been loaded!");
#endif
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
#if (DEBUG)
		ConsoleUtils::Log("Removing LLW hooks...");
#endif
		DetourDetach(&(LPVOID&)fnLoadLibraryW, _LoadLibraryW);
		DetourTransactionCommit();
		CLRHost::OnApplicationStart();
	}
	return lib;
}