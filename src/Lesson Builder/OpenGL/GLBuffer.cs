#if OPENGL_ES20
using OpenTK.Graphics.ES20;
using static OpenTK.Graphics.ES20.GL;
#elif OPENGL_ES30
using OpenTK.Graphics.ES30;
using static OpenTK.Graphics.ES30.GL;
#elif OPENGL
using OpenTK.Graphics.OpenGL;
using static OpenTK.Graphics.OpenGL.GL;
#elif OPENGL4
using OpenTK.Graphics.OpenGL4;

using static OpenTK.Graphics.OpenGL4.GL;
#endif

namespace Juniper.OpenGL
{
    public abstract class GLBuffer : GLScopedHandle
    {
        private readonly BufferTarget type;

        protected GLBuffer(BufferTarget type)
            : base(GenBuffer(), DeleteBuffer)
        {
            this.type = type;
        }

        public override void Enable()
        {
            BindBuffer(type, this);
        }

        public override void Disable()
        {
            BindBuffer(type, 0);
        }
    }
}