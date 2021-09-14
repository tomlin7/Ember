workspace "Ember"
	architecture "x64"

	configurations
	{
		"Debug",
		"Release",
		"Dist"
	}

outputdir = "%{cfg.buildcfg}-%{cfg.system}-%{ctg.architecture}"

project "Ember"
	location "Ember"
	kind "SharedLib"
	language "C++"

	targetdir ("bin" .. outputdir .. "/%{prj.name}")
	objdir ("obj/" .. outputdir .. "/%{prj.name}")
	
	EM_PLATFORM_WINDOWS;EM_BUILD_DLL;
