using OpenTK.Graphics.OpenGL4;

using static OpenTK.Graphics.OpenGL4.GL;

namespace Juniper.OpenGL
{
    public class VertexArray : GLHandle
    {
        private readonly int index;
        private readonly VertexBuffer buffer;

        public VertexArray(int index, float[] vertices)
            : base(GenVertexArray(), DeleteVertexArray)
        {
            this.index = index;
            Enable();
            buffer = new VertexBuffer(index, vertices);
            buffer.Enable();
            buffer.Disable();
        }

        public void Enable()
        {
            BindVertexArray(this);
            EnableVertexAttribArray(index);
        }

        protected override void Dispose(bool disposing)
        {
            buffer.Dispose();
            base.Dispose(disposing);
        }

        public void Draw()
        {
            DrawArrays(PrimitiveType.Triangles, 0, buffer.Length);
        }
    }
}
