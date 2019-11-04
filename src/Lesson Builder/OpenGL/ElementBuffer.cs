using OpenTK.Graphics.OpenGL4;

using static OpenTK.Graphics.OpenGL4.GL;

namespace Juniper.OpenGL
{
    public class ElementBuffer : GLBuffer
    {
        private readonly int Length;

        public ElementBuffer(uint[] indices)
            : base(BufferTarget.ElementArrayBuffer)
        {
            Length = indices.Length;
            BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
        }

        public override void Draw()
        {
            base.Draw();

            DrawElements(PrimitiveType.Triangles, Length, DrawElementsType.UnsignedInt, 0);
        }
    }
}
