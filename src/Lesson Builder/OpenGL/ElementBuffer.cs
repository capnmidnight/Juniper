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
    public class ElementBuffer : GLBuffer
    {
        private readonly int length;

#if OPENGL_ES30
        private readonly uint[] indices;
#endif

        public ElementBuffer(uint[] indices)
            : base(BufferTarget.ElementArrayBuffer)
        {
#if OPENGL_ES30
            this.indices = indices;
#endif
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
#if OPENGL_ES20
                BeginMode.Triangles,
#else
                PrimitiveType.Triangles,
#endif
                length,
                DrawElementsType.UnsignedInt,
#if OPENGL_ES30
                indices
#else
                0
#endif
            );
        }
    }
}
