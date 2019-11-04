using OpenTK.Graphics.OpenGL4;

using static OpenTK.Graphics.OpenGL4.GL;

namespace Juniper.OpenGL
{
    public class VertexArray : GLScopedHandle
    {
        public VertexArray(int attrIndex, VertexBuffer vertexBuffer)
            : base(GenVertexArray(), DeleteVertexArray)
        {
            Enable();

            using (var _ = vertexBuffer.Scope())
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
