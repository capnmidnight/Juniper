using OpenTK.Graphics.OpenGL4;

using static OpenTK.Graphics.OpenGL4.GL;

namespace Juniper.OpenGL
{
    public class VertexBuffer : GLBuffer
    {
        private const int NUM_ELEMENTS = 3;

        private readonly float[] vertices;

        public readonly int index;

        public VertexBuffer(int index, float[] vertices)
            : base(BufferTarget.ArrayBuffer)
        {
            this.index = index;
            this.vertices = vertices;
        }

        public int Length
        {
            get
            {
                return vertices.Length / NUM_ELEMENTS;
            }
        }

        public override void Enable()
        {
            base.Enable();

            BufferData(
                BufferTarget.ArrayBuffer,
                vertices.Length * sizeof(float),
                vertices,
                BufferUsageHint.StaticDraw);

            VertexAttribPointer(
                index,
                NUM_ELEMENTS,
                VertexAttribPointerType.Float,
                false,
                NUM_ELEMENTS * sizeof(float),
                0);

            EnableVertexAttribArray(index);
        }

        public override void Draw()
        {
            base.Draw();

            DrawArrays(PrimitiveType.Triangles, 0, 3);
        }
    }
}
