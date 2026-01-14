<div align="center">
  <img src="logo.svg" width=1000>
  <a href="https://github.com/users/billyeatcookies/projects/4">Docs</a> ◈
  <a href="./LICENSE.md">License</a> ◈
  <a href="./CHANGELOG.md">Changelog</a>
</div>
<p align="center">
  Game Engine written in CPP
</p>

---

### The Anatomy

```
┌─────────────────────────────────────────────────────────────────┐
│  YOUR GAME                                                      │
├─────────────────────────────────────────────────────────────────┤
│  Layers          │  Events          │  Input                    │
│                  │                  │                           │
│  ░░░░░░░░░░░░░░  │  ○ KeyPressed    │  IsKeyPressed(EM_KEY_W)   │
│  ░░ ImGui ░░░░░  │  ○ MouseMoved    │  IsMouseButtonPressed(0)  │
│  ░░       ░░░░░  │  ○ WindowResize  │  IsMouseButtonPressed(0)  │
│  ░░░░░░░░░░░░░░  │  ○ AppUpdate     │  GetMousePosition()       │
│  ░░ Game  ░░░░░  │  ○ MousePressed  │                           │
│  ░░       ░░░░░  │  ○ ...           │                           │
│  ░░░░░░░░░░░░░░  │                  │                           │
├─────────────────────────────────────────────────────────────────┤
│  EMBER CORE                                                     │
├──────────────────────┬──────────────────────┬───────────────────┤
│  Platform            │  Renderer            │  Logging          │
│  └── Windows         │  └── OpenGL          │  └── spdlog       │
│      └── GLFW        │      └── Glad        │                   │
└──────────────────────┴──────────────────────┴───────────────────┘
```

---

### Hello, Ember

```cpp
#include <Ember.h>

class MyGame : public Ember::Layer
{
    void OnUpdate() override
    {
        if (Ember::Input::IsKeyPressed(EM_KEY_SPACE))
            Jump();
    }
};

class App : public Ember::Application
{
public:
    App() { PushLayer(new MyGame()); }
};

Ember::Application* Ember::CreateApplication()
{
    return new App();
}
```

That's it. No 47-step configuration ritual.

---

### What's Cooking

| Done            | Next            | Someday   |
| --------------- | --------------- | --------- |
| Window creation | 2D Renderer     | Scripting |
| Event dispatch  | Sprite batching | Audio     |
| Layer stack     | Scene graph     | Physics   |
| Input polling   | Asset pipeline  | Editor    |
| ImGui debug     | ECS             | Linux/Mac |
| OpenGL context  | Cameras         | Vulkan    |

---

### Building

```bash
# Generate VS2022 solution
./GenerateProjects.bat

# Open and build
start Ember.sln
```

Requires: Visual Studio 2022, Windows SDK, a pulse.

---

### File Cartography

```
Ember/
├── src/
│   ├── Ember/
│   │   ├── Events/        ← Things that happen
│   │   ├── ImGui/         ← Debug UI wizardry
│   │   ├── Application    ← The heartbeat
│   │   ├── Layer          ← Stack 'em up
│   │   ├── Input          ← Keyboards, mice, and such
│   │   └── Window         ← A portal to pixels
│   └── Platform/
│       ├── Windows/       ← The OS whisperer
│       └── OpenGL/        ← GPU translator
└── vendor/                ← Friends we made along the way
    ├── GLFW
    ├── Glad
    ├── imgui
    └── spdlog
```

---

### The Event Pipeline

```
[ Keyboard ] ──→ ┌────────────────┐
[ Mouse    ] ──→ │ Event          │ ──→ Dispatch ──→ Layers (top to bottom)
[ Window   ] ──→ │ Dispatcher     │         │
                 └────────────────┘         ↓
                                    Handled? Stop propagating.
```

Events flow down the layer stack. First layer to handle it wins.

--

<div align="center">
  <sub>Built with mass amounts of caffeine.</sub>
</div>
