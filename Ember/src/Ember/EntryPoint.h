#pragma once

#ifdef EM_PLATFORM_WINDOWS

extern EmberEngine::Application* EmberEngine::CreateApplication();

int main(int argc, char** argv)
{
	printf("Ember Engine\n");

	Ember::Log::Init();
	EM_CORE_WARN("Initialized Log!");
	int a = 5;
	EM_CORE_INFO("Hello! Var={0}", a);

	auto app = EmberEngine::CreateApplication();
	app->Run();
	delete app;
}

#endif