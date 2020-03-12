using System;

namespace Juniper.VeldridIntegration
{
    public enum ShaderDataType
    {
        Int,
        IVec2,
        IVec3,
        IVec4,

        UInt,
        UVec2,
        UVec3,
        UVec4,

        Float,
        Vec2,
        Vec3,
        Vec4,

        Double,
        DVec2,
        DVec3,
        DVec4,

        Mat2,
        Mat3,
        Mat4
    }

    public static class ShaderDataTypeExt
    {
        public static uint Size(this ShaderDataType dataType)
        {
            return dataType switch
            {
                ShaderDataType.Int => 4,
                ShaderDataType.IVec2 => 8,
                ShaderDataType.IVec3 => 16,
                ShaderDataType.IVec4 => 32,
                ShaderDataType.UInt => 4,
                ShaderDataType.UVec2 => 8,
                ShaderDataType.UVec3 => 16,
                ShaderDataType.UVec4 => 32,
                ShaderDataType.Float => 4,
                ShaderDataType.Vec2 => 8,
                ShaderDataType.Vec3 => 16,
                ShaderDataType.Vec4 => 32,
                ShaderDataType.Double => 8,
                ShaderDataType.DVec2 => 32,
                ShaderDataType.DVec3 => 64,
                ShaderDataType.DVec4 => 128,
                ShaderDataType.Mat2 => 16,
                ShaderDataType.Mat3 => 36,
                ShaderDataType.Mat4 => 64,
                _ => throw new FormatException($"Unrecognized data type {dataType}")
            };
        }
    }
}
