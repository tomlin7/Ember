#include "empch.h"

#include "OpenGLContext.h"

#include <GLFW/glfw3.h>
#include <glad/glad.h>

namespace Ember {
    OpenGLContext::OpenGLContext(GLFWwindow* windowHandle)
        : m_WindowHandle(windowHandle) {
            EM_CORE_ASSERT(windowHandle, "Handle is null!");
        }
    void OpenGLContext::Init() {
        glfwMakeContextCurrent(m_WindowHandle);
        int status = gladLoadGLLoader((GLADloadproc)glfwGetProcAddress);
        EM_CORE_ASSERT(status, "Failed to initialize Glad!");

        EM_CORE_INFO("OpenGL Renderer: {0}", glGetString(GL_RENDERER));
        EM_CORE_INFO("OpenGL Version: {0}", glGetString(GL_VERSION));
        EM_CORE_INFO("OpenGL Shader Language Version: {0}", glGetString(GL_SHADING_LANGUAGE_VERSION));
    }
    void OpenGLContext::SwapBuffers() { glfwSwapBuffers(m_WindowHandle); }
} // namespace Ember
