using System;

namespace Juniper.OpenGL
{
    public struct ShaderBinding : IDisposable
    {
        private readonly Program program;
        private readonly Shader shader;

        public ShaderBinding(Program program, Shader shader)
        {
            this.program = program;
            this.shader = shader;
            program.Attach(shader);
        }

        public void Dispose()
        {
            program.Detach(shader);
        }
    }
}