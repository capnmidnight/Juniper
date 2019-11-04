using OpenTK.Graphics.OpenGL4;

using static OpenTK.Graphics.OpenGL4.GL;

namespace Juniper.OpenGL
{
    public class VertexArray : GLHandle
    {
        private readonly int length;

        public VertexArray(VertexBuffer buffer)
            : base(GenVertexArray(), DeleteVertexArray)
        {
            length = buffer.Length;
            Enable();
            buffer.Enable();
            buffer.Disable();
        }

        public void Enable()
        {
            BindVertexArray(this);
        }

        public void Draw()
        {
            DrawArrays(PrimitiveType.Triangles, 0, length);
        }
    }
}
