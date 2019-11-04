using OpenTK.Graphics.OpenGL4;

using static OpenTK.Graphics.OpenGL4.GL;

namespace Juniper.OpenGL
{
    public abstract class GLBuffer : GLHandle
    {
        private readonly BufferTarget type;

        public GLBuffer(BufferTarget type)
            : base(GenBuffer(), DeleteBuffer)
        {
            this.type = type;
        }

        public virtual void Enable()
        {
            BindBuffer(type, this);
        }

        public virtual void Disable()
        {
            BindBuffer(type, 0);
        }

        public virtual void Draw() { }
    }
}
