#pragma once
#include "stdafx.h"

typedef VOID MonoObject;
typedef VOID MonoDomain;
typedef VOID MonoAssembly;
typedef VOID MonoImage;
typedef VOID MonoClass;
typedef VOID MonoMethod;
typedef VOID MonoImageOpenStatus;

typedef void(__cdecl* t_mono_thread_attach)(MonoDomain*);
typedef MonoDomain* (__cdecl* t_mono_get_root_domain)(void);
typedef MonoAssembly* (__cdecl* t_mono_assembly_open)(const char*, MonoImageOpenStatus*);
typedef MonoImage* (__cdecl* t_mono_assembly_get_image)(MonoAssembly*);
typedef MonoClass* (__cdecl* t_mono_class_from_name)(MonoImage*, const char*, const char*);
typedef MonoMethod* (__cdecl* t_mono_class_get_method_from_name)(MonoClass*, const char*, int);
typedef MonoObject* (__cdecl* t_mono_runtime_invoke)(MonoMethod*, void*, void**, MonoObject**);