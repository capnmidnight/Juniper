using Veldrid;

namespace Juniper.VeldridIntegration
{
    public static class VertexElementFormatExt
    {
        public static int Size(this VertexElementFormat format)
        {
            return format switch
            {
                VertexElementFormat.Float1 => 4,
                VertexElementFormat.Float2 => 8,
                VertexElementFormat.Float3 => 16,
                VertexElementFormat.Float4 => 32,
                VertexElementFormat.Byte2_Norm => 2,
                VertexElementFormat.Byte2 => 2,
                VertexElementFormat.Byte4_Norm => 4,
                VertexElementFormat.Byte4 => 4,
                VertexElementFormat.SByte2_Norm => 2,
                VertexElementFormat.SByte2 => 2,
                VertexElementFormat.SByte4_Norm => 4,
                VertexElementFormat.SByte4 => 4,
                VertexElementFormat.UShort2_Norm => 4,
                VertexElementFormat.UShort2 => 4,
                VertexElementFormat.UShort4_Norm => 8,
                VertexElementFormat.UShort4 => 8,
                VertexElementFormat.Short2_Norm => 4,
                VertexElementFormat.Short2 => 4,
                VertexElementFormat.Short4_Norm => 8,
                VertexElementFormat.Short4 => 8,
                VertexElementFormat.UInt1 => 4,
                VertexElementFormat.UInt2 => 8,
                VertexElementFormat.UInt3 => 16,
                VertexElementFormat.UInt4 => 32,
                VertexElementFormat.Int1 => 4,
                VertexElementFormat.Int2 => 8,
                VertexElementFormat.Int3 => 16,
                VertexElementFormat.Int4 => 32,
                VertexElementFormat.Half1 => 2,
                VertexElementFormat.Half2 => 4,
                VertexElementFormat.Half4 => 8,
                _ => -1
            };
        }
    }
}
