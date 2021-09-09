#pragma once

#include "Core.h"

namespace EmberEngine {
	class EMBER_API Application
	{
	public:
		Application();
		~Application();

		void Run();
	};

	// To be defined in CLIENT
	Application* CreateApplication();
}