#include <Ember.h>

class Sandbox : public Ember::Application 
{
public:
	Sandbox()
	{

	}

	~Sandbox()
	{

	}
};

Ember::Application* EmberEngine::CreateApplication()
{
	return new Sandbox();
}