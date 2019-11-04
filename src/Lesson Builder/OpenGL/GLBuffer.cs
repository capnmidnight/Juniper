using OpenTK.Graphics.OpenGL4;

using static OpenTK.Graphics.OpenGL4.GL;

namespace Juniper.OpenGL
{
    public abstract class GLBuffer : GLScopedHandle
    {
        private readonly BufferTarget type;

        public GLBuffer(BufferTarget type)
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
