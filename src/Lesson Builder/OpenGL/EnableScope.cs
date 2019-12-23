using System;

namespace Juniper.OpenGL
{
    public struct EnableScope : IDisposable
    {
        private readonly GLScopedHandle buffer;

        public EnableScope(GLScopedHandle buffer)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            this.buffer = buffer;
            buffer.Enable();
        }

        public void Dispose()
        {
            buffer.Disable();
        }
    }
}
