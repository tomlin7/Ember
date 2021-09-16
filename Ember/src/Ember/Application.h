#pragma once

#include "Core.h"
#include "Events/Event.h"
#include "Ember/Events/ApplicationEvent.h"

#include "Window.h"

namespace Ember {
	class EMBER_API Application
	{
	public:
		Application();
		~Application();

		void Run();
		
		void OnEvent(Event& e);
	private:
		bool OnWindowClose(WindowCloseEvent& e);

		std::unique_ptr<Window> m_Window;
		bool m_Running = true;
	};

	// To be defined in CLIENT
	Application* CreateApplication();
}