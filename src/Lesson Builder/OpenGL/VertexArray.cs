#if !OPENGL_ES10 && !OPENGL_ES20

#if OPENGL_ES30
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
    public class VertexArray : GLScopedHandle
    {
        public VertexArray(int attrIndex, VertexBuffer vertexBuffer)
            : base(GenVertexArray(), DeleteVertexArray)
        {
            Enable();

            using (vertexBuffer.Use())
            {
                VertexAttribPointer(
                    attrIndex,
                    VertexBuffer.NUM_ELEMENTS,
                    VertexAttribPointerType.Float,
                    false,
                    VertexBuffer.NUM_ELEMENTS * sizeof(float),
                    0);

                EnableVertexAttribArray(attrIndex);
            }
        }

        public override void Enable()
        {
            BindVertexArray(this);
        }

        public override void Disable()
        {
            BindVertexArray(0);
        }
    }
}

#endif