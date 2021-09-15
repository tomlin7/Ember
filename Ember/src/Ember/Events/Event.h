#pragma once

#include "../Core.h"

#include <string>
#include <functional>


namespace Ember {
	// Events in Ember are currently blocking, meaning when an event occurs it
	// immediately gets dispatched and must be dealt with right there an there.
	// For the future, a better strategy might be to buffer events in an event
	// bus and process them during the "event" part of the update stage.

	enum class EventType
	{
		None = 0,
		WindowClose, WindowResize, WindowFocus, WindowLostFocus, WindowMoved,
		AppTick, AppUpdate, AppRender,
		KeyPressed, KeyReleased,
		MouseButtonPressed, MouseButtonReleased, 
		MouseButtonPressed, MouseButtonReleased, MouseMoved, MouseScrolled
	};
}