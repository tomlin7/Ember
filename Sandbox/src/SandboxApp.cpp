#include <Ember.h>

class ExampleLayer : public Ember::Layer
{
public:
	ExampleLayer()
		: Layer("Example")
	{

	}

	void OnUpdate() override
	{
		EM_INFO("ExampleLayer::Update");
	}

	void OnEvent(Ember::Event& event) override
	{
		EM_TRACE("{0}", event);
	}
};

class Sandbox : public Ember::Application 
{
public:
	Sandbox()
	{
		PushLayer(new ExampleLayer());
	}

	~Sandbox()
	{

	}
};

Ember::Application* Ember::CreateApplication()
{
	return new Sandbox();
}