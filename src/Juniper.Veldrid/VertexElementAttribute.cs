using System;
using System.Reflection;

using Veldrid;

namespace Juniper.VeldridIntegration
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class VertexElementAttribute : Attribute
    {
        public VertexElementSemantic Semantics { get; }

        public VertexElementFormat? Format { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// If this field is an array, set the number of elements in the array. Defaults to 1
        /// for fields that aren't arrays.
        /// </summary>
        public uint ElementCount { get; set; } = 1;

        public uint ElementSize { get; private set; }

        public uint Size =>
            ElementSize * ElementCount;

        public VertexElementAttribute(VertexElementSemantic semantics)
        {
            Semantics = semantics;
        }

        public void Resolve(MemberInfo member)
        {
            if (member is null)
            {
                throw new ArgumentNullException(nameof(member));
            }

            Name ??= member.Name;
            var valueType = member.MemberType == MemberTypes.Field
                ? ((FieldInfo)member).FieldType
                : ((PropertyInfo)member).PropertyType;

            Format ??= valueType.ToVertexElementFormat();
            ElementSize = valueType.Size();
        }
    }
}
