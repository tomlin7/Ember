#pragma once

#ifdef EM_PLATFORM_WINDOWS

extern EmberEngine::Application* EmberEngine::CreateApplication();

int main(int argc, char** argv)
{
	printf("Ember Engine\n");
	auto app = EmberEngine::CreateApplication();
	app->Run();
	delete app;
}

#endif