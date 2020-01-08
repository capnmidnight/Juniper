using System;

namespace Juniper.OpenGL
{
    public struct ShaderBinding : IDisposable
    {
        private readonly Program program;
        private readonly Shader shader;

        public ShaderBinding(Program program, Shader shader)
        {
            if (program is null)
            {
                throw new ArgumentNullException(nameof(program));
            }

            if (shader is null)
            {
                throw new ArgumentNullException(nameof(shader));
            }

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