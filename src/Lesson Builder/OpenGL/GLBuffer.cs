using OpenTK.Graphics.OpenGL4;

using static OpenTK.Graphics.OpenGL4.GL;

namespace Lesson_Builder
{
    public class GLBuffer : GLHandle
    {
        private readonly BufferTarget type;

        public GLBuffer(BufferTarget type)
            : base(GenBuffer())
        {
            this.type = type;
            BindBuffer(type, this);
        }

        protected override void OnDispose(bool disposing)
        {
            BindBuffer(type, 0);
            DeleteBuffer(this);
        }

        public virtual void Draw() { }
    }
}
