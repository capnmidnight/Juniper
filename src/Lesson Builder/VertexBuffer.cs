using OpenTK.Graphics.OpenGL4;

using static OpenTK.Graphics.OpenGL4.GL;

namespace Lesson_Builder
{
    public class VertexBuffer : GLBuffer
    {
        private const int NUM_ELEMENTS = 3;

        public readonly int Length;

        public readonly int index;

        public VertexBuffer(int index, float[] vertices)
            : base(BufferTarget.ArrayBuffer)
        {
            this.index = index;

            Length = vertices.Length / NUM_ELEMENTS;

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
        }

        public override void Enable()
        {
            base.Enable();

            EnableVertexAttribArray(index);
        }
    }
}
