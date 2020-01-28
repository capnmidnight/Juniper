using System;

#if OPENGL_ES20
using OpenTK.Graphics.ES20;
#elif OPENGL_ES30
using OpenTK.Graphics.ES30;
#elif OPENGL
using OpenTK.Graphics.OpenGL;
#elif OPENGL4
using OpenTK.Graphics.OpenGL4;
#endif

namespace Juniper.OpenGL
{
    public abstract class GLHandle : IDisposable
    {
        public static implicit operator int(GLHandle handle)
        {
            return handle is null
                ? -1
                : handle.handle;
        }

        private readonly int handle;
        private readonly Action<int> delete;

        private bool disposedValue = false;

        protected GLHandle(int handle, Action<int> delete)
        {
            this.handle = handle;
            this.delete = delete;
        }

        ~GLHandle()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                delete(this);
                disposedValue = true;
            }
        }

        public bool IsBuffer => GL.IsBuffer(this);

        public bool IsTexture => GL.IsTexture(this);

        public bool IsFrameBuffer => GL.IsFramebuffer(this);

        public bool IsProgram => GL.IsProgram(this);

        public bool IsRenderBuffer => GL.IsRenderbuffer(this);

        public bool IsShader => GL.IsShader(this);

#if !OPENGL_ES20

        public bool IsQuery => GL.IsQuery(this);

        public bool IsSampler => GL.IsSampler(this);

        public bool IsTransformFeedback => GL.IsTransformFeedback(this);

        public bool IsVertexArray => GL.IsVertexArray(this);

#if !OPENGL_ES30
        public bool IsProgramPipeline => GL.IsProgramPipeline(this);
#endif
#endif
    }
}