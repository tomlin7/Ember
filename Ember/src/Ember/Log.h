#pragma once

#include <memory>

#include "Core.h"
#include "spdlog/spdlog.h"
#include "spdlog/fmt/ostr.h"

namespace Ember {
	class EMBER_API Log
	{
	public:
		static void Init();

		inline static std::shared_ptr<spdlog::logger>& GetCoreLogger() { return s_CoreLogger; }
		inline static std::shared_ptr<spdlog::logger>& GetClientLogger() { return s_ClientLogger; }
	private:
		static std::shared_ptr<spdlog::logger> s_CoreLogger;
		static std::shared_ptr<spdlog::logger> s_ClientLogger;
	};
}

// Core log macros
#define EM_CORE_TRACE(...)   ::Ember::Log::GetCoreLogger()->trace(__VA_ARGS__)
#define EM_CORE_INFO(...)    ::Ember::Log::GetCoreLogger()->info(__VA_ARGS__)
#define EM_CORE_WARN(...)    ::Ember::Log::GetCoreLogger()->warn(__VA_ARGS__)
#define EM_CORE_ERROR(...)   ::Ember::Log::GetCoreLogger()->error(__VA_ARGS__)
#define EM_CORE_FATAL(...)   ::Ember::Log::GetCoreLogger()->fatal(__VA_ARGS__)

// Client log macros
#define EM_TRACE(...)        ::Ember::Log::GetClientLogger()->trace(__VA_ARGS__)
#define EM_INFO(...)         ::Ember::Log::GetClientLogger()->info(__VA_ARGS__)
#define EM_WARN(...)         ::Ember::Log::GetClientLogger()->warn(__VA_ARGS__)
#define EM_ERROR(...)        ::Ember::Log::GetClientLogger()->error(__VA_ARGS__)
#define EM_FATAL(...)        ::Ember::Log::GetClientLogger()->fatal(__VA_ARGS__)