using OpenTK.Graphics.OpenGL4;

using static OpenTK.Graphics.OpenGL4.GL;

namespace Juniper.OpenGL
{
    public class VertexBuffer : GLBuffer
    {
        public const int NUM_ELEMENTS = 3;

        public VertexBuffer(float[] vertices)
            : base(BufferTarget.ArrayBuffer)
        {
            Length = vertices.Length / NUM_ELEMENTS;
            using (Use())
            {
                BufferData(
                    BufferTarget.ArrayBuffer,
                    vertices.Length * sizeof(float),
                    vertices,
                    BufferUsageHint.StaticDraw);
            }
        }

        public int Length
        {
            get;
        }

        public void Draw()
        {
            DrawArrays(PrimitiveType.Triangles, 0, Length);
        }
    }
}
