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

Ember::Application* Ember::CreateApplication()
{
	return new Sandbox();
}