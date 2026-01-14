#include "Ember/Events/Event.h"
#include "Ember/Keycodes.h"
#include <Ember.h>

class ExampleLayer : public Ember::Layer {
  public:
    ExampleLayer() : Layer("Example") {}

    void OnUpdate() override {
        // EM_TRACE("ExampleLayer::Update");

        if (Ember::Input::IsKeyPressed(EM_KEY_TAB))
            EM_INFO("Tab key pressed");
    }

    void OnEvent(Ember::Event& event) override {
        if (event.GetEventType() == Ember::EventType::KeyPressed) {
            Ember::KeyPressedEvent& e = (Ember::KeyPressedEvent&)event;

            if (e.GetKeyCode() == EM_KEY_TAB)
                EM_TRACE("Tab key is pressed!");
            EM_TRACE("{0}", (char)e.GetKeyCode());
        }
    }
};

class Sandbox : public Ember::Application {
  public:
    Sandbox() {
        PushLayer(new ExampleLayer());
        PushOverlay(new Ember::ImGuiLayer());
    }

    ~Sandbox() {}
};

Ember::Application* Ember::CreateApplication() { return new Sandbox(); }
