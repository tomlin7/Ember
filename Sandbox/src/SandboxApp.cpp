#include <Ember.h>

class Sandbox : public EmberEngine::Application 
{
public:
	Sandbox()
	{

	}

	~Sandbox()
	{

	}
};

EmberEngine::Application* EmberEngine::CreateApplication()
{
	return new Sandbox();
}