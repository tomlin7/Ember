workspace "Ember"
	architecture "x64"

	configurations
	{
		"Debug",
		"Release",
		"Dist"
	}

outputdir = "%{cfg.buildcfg}-%{cfg.system}-%{cfg.architecture}"

project "Ember"
	location "Ember"
	kind "SharedLib"
	language "C++"

	targetdir ("bin/" .. outputdir .. "/%{prj.name}")
	objdir ("obj/" .. outputdir .. "/%{prj.name}")

	files
	{
		"%{prj.name}/src/**.h",
		"%{prj.name}/src/**.cpp"
	}

	includedirs
	{
		"%{prj.name}/src",
		"%{prj.name}/vendor/spdlog/include"
	}

	filter "system:windows"
		cppdialect "C++17"
		staticruntime "On"
		systemversion "latest"

		defines
		{
			"EM_PLATFORM_WINDOWS",
			"EM_BUILD_DLL"
		}

		postbuildcommands
		{
			("{COPY} %{cfg.buildtarget.relpath} ../bin/" .. outputdir .. "/Sandbox")
		}

	filter "configurations:Debug"
		defines "EM_DEBUG"
		symbols "On"

	filter "configurations:Release"
		defines "EM_RELEASE"
		symbols "On"
	
	filter "configurations:Dist"
		defines "EM_DIST"
		symbols "On"

project "Sandbox"
	location "Sandbox"
	kind "ConsoleApp"
	language "C++"

	targetdir ("bin/" .. outputdir .. "/%{prj.name}")
	objdir ("obj/" .. outputdir .. "/%{prj.name}")

	files
	{
		"%{prj.name}/src/**.h",
		"%{prj.name}/src/**.cpp"
	}

	includedirs
	{
		"Ember/vendor/spdlog/include",
		"Ember/src"
	}

	links
	{
		"Ember"
	}

	filter "system:windows"
		cppdialect "C++17"
		staticruntime "On"
		systemversion "latest"

		defines
		{
			"EM_PLATFORM_WINDOWS"
		}

	filter "configurations:Debug"
		defines "EM_DEBUG"
		symbols "On"

	filter "configurations:Release"
		defines "EM_RELEASE"
		symbols "On"
	
	filter "configurations:Dist"
		defines "EM_DIST"
		symbols "On"