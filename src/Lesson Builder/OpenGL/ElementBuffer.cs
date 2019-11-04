#if OPENGL_ES10
using OpenTK.Graphics.ES10;
using static OpenTK.Graphics.ES10.GL;
#elif OPENGL_ES11
using OpenTK.Graphics.ES11;
using static OpenTK.Graphics.ES11.GL;
#elif OPENGL_ES20
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
    public class ElementBuffer : GLBuffer
    {
        private readonly int length;

        public ElementBuffer(uint[] indices)
            : base(BufferTarget.ElementArrayBuffer)
        {
            length = indices.Length;
            using (Use())
            {
                BufferData(
                    BufferTarget.ElementArrayBuffer,
                    indices.Length * sizeof(uint),
                    indices,
                    BufferUsageHint.StaticDraw);
            }
        }

        public void Draw()
        {
            DrawElements(
                PrimitiveType.Triangles,
                length,
                DrawElementsType.UnsignedInt,
                0);
        }
    }
}
