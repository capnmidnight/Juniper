using System;
using System.Collections.Generic;
using System.Numerics;

using Veldrid;
using Veldrid.Utilities;

namespace Juniper.VeldridIntegration
{
    public static class DataTypeSizeExt
    {
        private static readonly Dictionary<Type, uint> sizes = new Dictionary<Type, uint>
        {
            [typeof(RgbaByte)] = 4,
            [typeof(RgbaFloat)] = 4 * sizeof(float),
            [typeof(short)] = sizeof(short),
            [typeof(ushort)] = sizeof(ushort),
            [typeof(int)] = sizeof(int),
            [typeof(uint)] = sizeof(uint),
            [typeof(float)] = sizeof(float),
            [typeof(Vector2)] = 2 * sizeof(float),
            [typeof(Vector3)] = 3 * sizeof(float),
            [typeof(Vector4)] = 4 * sizeof(float),
            [typeof(Matrix3x2)] = 3 * 2 * sizeof(float),
            [typeof(Matrix4x4)] = 4 * 4 * sizeof(float),
            [typeof(double)] = sizeof(double),
            [typeof(VertexPosition)] = VertexPosition.SizeInBytes,
            [typeof(VertexPositionTexture)] = VertexPositionTexture.SizeInBytes,
            [typeof(VertexPositionNormalTexture)] = VertexPositionNormalTexture.SizeInBytes
        };

        public static uint Size(this Type dataType)
        {
            if (dataType is null)
            {
                throw new ArgumentNullException(nameof(dataType));
            }

            if (!sizes.ContainsKey(dataType))
            {
                throw new FormatException($"Unrecognized data type {dataType.Name}");
            }

            return sizes[dataType];
        }

        private static readonly Dictionary<Type, VertexElementFormat> vertTypes = new Dictionary<Type, VertexElementFormat>
        {
            [typeof(RgbaByte)] = VertexElementFormat.Byte4_Norm,
            [typeof(RgbaFloat)] = VertexElementFormat.Float4,
            [typeof(int)] = VertexElementFormat.Int1,
            [typeof(uint)] = VertexElementFormat.UInt1,
            [typeof(float)] = VertexElementFormat.Float1,
            [typeof(Vector2)] = VertexElementFormat.Float2,
            [typeof(Vector3)] = VertexElementFormat.Float3,
            [typeof(Vector4)] = VertexElementFormat.Float4
        };

        public static VertexElementFormat ToVertexElementFormat(this Type dataType)
        {
            if (dataType is null)
            {
                throw new ArgumentNullException(nameof(dataType));
            }

            if (!vertTypes.ContainsKey(dataType))
            {
                throw new FormatException($"Unrecognized data type {dataType.Name}");
            }

            return vertTypes[dataType];
        }

        private static readonly Dictionary<Type, IndexFormat> indexTypes = new Dictionary<Type, IndexFormat>()
        {
            [typeof(ushort)] = IndexFormat.UInt16,
            [typeof(uint)] = IndexFormat.UInt32,
            [typeof(short)] = IndexFormat.UInt16,
            [typeof(int)] = IndexFormat.UInt32
        };

        public static IndexFormat ToIndexFormat(this Type dataType)
        {
            if (dataType is null)
            {
                throw new ArgumentNullException(nameof(dataType));
            }

            if (!indexTypes.ContainsKey(dataType))
            {
                throw new FormatException($"Unrecognized data type {dataType.Name}");
            }

            return indexTypes[dataType];
        }

        public static uint Size(this ShaderDataType dataType)
        {
            return dataType switch
            {
                ShaderDataType.Int => sizeof(int),
                ShaderDataType.IVec2 => 2 * sizeof(int),
                ShaderDataType.IVec3 => 3 * sizeof(int),
                ShaderDataType.IVec4 => 4 * sizeof(int),
                ShaderDataType.UInt => sizeof(uint),
                ShaderDataType.UVec2 => 2 * sizeof(uint),
                ShaderDataType.UVec3 => 3 * sizeof(uint),
                ShaderDataType.UVec4 => 4 * sizeof(uint),
                ShaderDataType.Float => sizeof(float),
                ShaderDataType.Vec2 => 2 * sizeof(float),
                ShaderDataType.Vec3 => 3 * sizeof(float),
                ShaderDataType.Vec4 => 4 * sizeof(float),
                ShaderDataType.Mat2 => 2 * 2 * sizeof(float),
                ShaderDataType.Mat3 => 3 * 3 * sizeof(float),
                ShaderDataType.Mat4 => 4 * 4 * sizeof(float),
                ShaderDataType.Double => sizeof(double),
                ShaderDataType.DVec2 => 2 * sizeof(double),
                ShaderDataType.DVec3 => 3 * sizeof(double),
                ShaderDataType.DVec4 => 4 * sizeof(double),
                _ => throw new FormatException($"Unrecognized data type {dataType}")
            };
        }

        public static int Size(this VertexElementFormat dataType)
        {
            return dataType switch
            {
                VertexElementFormat.Float1 => sizeof(float),
                VertexElementFormat.Float2 => 2 * sizeof(float),
                VertexElementFormat.Float3 => 3 * sizeof(float),
                VertexElementFormat.Float4 => 4 * sizeof(float),
                VertexElementFormat.Byte2_Norm => 2,
                VertexElementFormat.Byte2 => 2,
                VertexElementFormat.Byte4_Norm => 4,
                VertexElementFormat.Byte4 => 4,
                VertexElementFormat.SByte2_Norm => 2,
                VertexElementFormat.SByte2 => 2,
                VertexElementFormat.SByte4_Norm => 4,
                VertexElementFormat.SByte4 => 4,
                VertexElementFormat.UShort2_Norm => 2 * sizeof(ushort),
                VertexElementFormat.UShort2 => 2 * sizeof(ushort),
                VertexElementFormat.UShort4_Norm => 4 * sizeof(ushort),
                VertexElementFormat.UShort4 => 4 * sizeof(ushort),
                VertexElementFormat.Short2_Norm => 2 * sizeof(short),
                VertexElementFormat.Short2 => 2 * sizeof(short),
                VertexElementFormat.Short4_Norm => 4 * sizeof(short),
                VertexElementFormat.Short4 => 4 * sizeof(short),
                VertexElementFormat.UInt1 => sizeof(uint),
                VertexElementFormat.UInt2 => 2 * sizeof(uint),
                VertexElementFormat.UInt3 => 3 * sizeof(uint),
                VertexElementFormat.UInt4 => 4 * sizeof(uint),
                VertexElementFormat.Int1 => sizeof(int),
                VertexElementFormat.Int2 => 2 * sizeof(int),
                VertexElementFormat.Int3 => 3 * sizeof(int),
                VertexElementFormat.Int4 => 4 * sizeof(int),
                VertexElementFormat.Half1 => sizeof(float) / 2,
                VertexElementFormat.Half2 => 2 * sizeof(float) / 2,
                VertexElementFormat.Half4 => 4 * sizeof(float) / 2,
                _ => throw new FormatException($"Unrecognized data type {dataType}")
            };
        }
    }
}
