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
        public static implicit operator int(GLHandle obj)
        {
            return obj.handle;
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

        public bool IsBuffer
        {
            get
            {
                return GL.IsBuffer(this);
            }
        }

        public bool IsTexture
        {
            get
            {
                return GL.IsTexture(this);
            }
        }

        public bool IsFrameBuffer
        {
            get
            {
                return GL.IsFramebuffer(this);
            }
        }

        public bool IsProgram
        {
            get
            {
                return GL.IsProgram(this);
            }
        }

        public bool IsRenderBuffer
        {
            get
            {
                return GL.IsRenderbuffer(this);
            }
        }

        public bool IsShader
        {
            get
            {
                return GL.IsShader(this);
            }
        }

#if !OPENGL_ES20

        public bool IsQuery
        {
            get
            {
                return GL.IsQuery(this);
            }
        }

        public bool IsSampler
        {
            get
            {
                return GL.IsSampler(this);
            }
        }

        public bool IsTransformFeedback
        {
            get
            {
                return GL.IsTransformFeedback(this);
            }
        }

        public bool IsVertexArray
        {
            get
            {
                return GL.IsVertexArray(this);
            }
        }

#if !OPENGL_ES30
        public bool IsProgramPipeline
        {
            get
            {
                return GL.IsProgramPipeline(this);
            }
        }
#endif
#endif
    }
}