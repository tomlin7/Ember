#pragma once

#ifdef EM_PLATFORM_WINDOWS

extern Ember::Application* Ember::CreateApplication();

int main(int argc, char** argv)
{
	Ember::Log::Init();
	EM_CORE_WARN("Initialized Log!");
	int a = 5;
	EM_CORE_INFO("Hello! Var={0}", a);

	auto app = Ember::CreateApplication();
	app->Run();
	delete app;
}

#endif