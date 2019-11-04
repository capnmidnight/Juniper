using OpenTK.Graphics.OpenGL4;

using static OpenTK.Graphics.OpenGL4.GL;

namespace Lesson_Builder
{
    public class VertexArray : GLHandle
    {
        private readonly VertexBuffer buffer;

        public VertexArray(int index, float[] vertices)
            : base(GenVertexArray())
        {
            BindVertexArray(this);
            buffer = new VertexBuffer(index, vertices);
            buffer.Enable();
        }

        protected override void OnDispose(bool disposing)
        {
            DeleteVertexArray(this);
            buffer.Dispose();
        }

        public override void Enable()
        {
            base.Enable();
            BindVertexArray(this);
        }

        public void Draw()
        {
            DrawArrays(PrimitiveType.Triangles, 0, buffer.Length);
        }
    }
}
