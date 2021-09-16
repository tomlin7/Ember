#pragma once

#include "Core.h"
#include "Ember/LayerStack.h"
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

		void PushLayer(Layer* layer);
		void PushOverlay(Layer* layer);
	private:
		bool OnWindowClose(WindowCloseEvent& e);

		std::unique_ptr<Window> m_Window;
		bool m_Running = true;
		LayerStack m_LayerStack;
	};

	// To be defined in CLIENT
	Application* CreateApplication();
}