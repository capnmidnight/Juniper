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