#include "Application.h"

#include "Ember/Events/ApplicationEvent.h"
#include "Ember/Log.h"

namespace Ember {
	Application::Application()
	{

	}

	Application::~Application()
	{

	}

	void Application::Run() 
	{
		WindowResizeEvent e(1200, 720);
		EM_TRACE(e);

		while (true);
	}
}