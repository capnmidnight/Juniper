using System;
using System.Reflection;
using System.Text;

using Veldrid;
using Veldrid.SPIRV;

namespace Juniper.VeldridIntegration
{

    public class Material<VertexT> : Material
        where VertexT : struct
    {
        private readonly ShaderSetDescription shaderSet;

        public Material(ResourceFactory factory, byte[] vertShaderBytes, byte[] fragShaderBytes)
        {
            if (factory is null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (vertShaderBytes is null)
            {
                throw new ArgumentNullException(nameof(vertShaderBytes));
            }

            if (fragShaderBytes is null)
            {
                throw new ArgumentNullException(nameof(fragShaderBytes));
            }

            var vertType = typeof(VertexT);
            var layoutField = vertType.GetField("Layout", BindingFlags.Public | BindingFlags.Static);
            if (layoutField is null)
            {
                throw new ArgumentException($"Type argument {vertType.Name} does not contain a static Layout field.");
            }

            if (layoutField.FieldType != typeof(VertexLayoutDescription))
            {
                throw new ArgumentException($"Type argument {vertType.Name}'s Layout field is not of type VertexLayoutDescription.");
            }

            var layout = (VertexLayoutDescription)layoutField.GetValue(null);

            var vertShaderDesc = new ShaderDescription(ShaderStages.Vertex, vertShaderBytes, "main");
            var fragShaderDesc = new ShaderDescription(ShaderStages.Fragment, fragShaderBytes, "main");

            var shaders = factory.CreateFromSpirv(vertShaderDesc, fragShaderDesc);

            shaderSet = new ShaderSetDescription
            {
                VertexLayouts = new VertexLayoutDescription[] { layout },
                Shaders = shaders
            };
        }

        public static implicit operator ShaderSetDescription(Material<VertexT> material)
        {
            if (material is null)
            {
                return default;
            }

            return material.shaderSet;
        }

        public static implicit operator Material<VertexT>(ShaderSetDescription shaderSet)
        {
            return new Material<VertexT>(shaderSet);
        }

        public Material(ShaderSetDescription shaderSet)
        {
            this.shaderSet = shaderSet;
        }

        public Material(ResourceFactory factory, string vertShaderText, string fragShaderText)
            : this(factory, ToBytes(vertShaderText), ToBytes(fragShaderText))
        { }

        private static byte[] ToBytes(string shaderText)
        {
            if (shaderText is null)
            {
                throw new ArgumentNullException(nameof(shaderText));
            }

            if (shaderText.Length == 0)
            {
                throw new ArgumentException("Empty shader", nameof(shaderText));
            }

            return Encoding.UTF8.GetBytes(shaderText);
        }
    }
}
