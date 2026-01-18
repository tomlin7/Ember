#pragma once

#include "Ember/Layer.h"

#include "Ember/Events/ApplicationEvent.h"
#include "Ember/Events/KeyEvent.h"
#include "Ember/Events/MouseEvent.h"

namespace Ember {
	class EMBER_API ImGuiLayer : public Layer
	{
	public:
		ImGuiLayer();
		~ImGuiLayer();

		virtual void OnAttach() override;
		virtual void OnDetach() override;
		virtual void OnImGuiRender() override;

		void Begin();
		void End();
	private:
		float m_Time = 0.0f;
	};
}
