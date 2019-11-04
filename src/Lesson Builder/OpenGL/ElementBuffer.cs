using OpenTK.Graphics.OpenGL4;

using static OpenTK.Graphics.OpenGL4.GL;

namespace Juniper.OpenGL
{
    public class ElementBuffer : GLBuffer
    {
        private readonly int length;

        public ElementBuffer(uint[] indices)
            : base(BufferTarget.ElementArrayBuffer)
        {
            length = indices.Length;
            using (Scope())
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
