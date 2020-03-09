using System;
using System.Reflection;

using Veldrid;
using Veldrid.SPIRV;

namespace Juniper.VeldridIntegration
{
    public class Material<VertexT> : Material
        where VertexT : struct
    { 
        private readonly ShaderDescription vertShaderDesc;
        private readonly ShaderDescription fragShaderDesc;

        internal Material(byte[] vertShaderBytes, byte[] fragShaderBytes)
        {
            if (vertShaderBytes is null)
            {
                throw new ArgumentNullException(nameof(vertShaderBytes));
            }

            if (fragShaderBytes is null)
            {
                throw new ArgumentNullException(nameof(fragShaderBytes));
            }

#if DEBUG
            const bool debug = true;
#else
            const bool debug = false;
#endif

            vertShaderDesc = new ShaderDescription(ShaderStages.Vertex, vertShaderBytes, "main", debug);
            fragShaderDesc = new ShaderDescription(ShaderStages.Fragment, fragShaderBytes, "main", debug);
        }

        public Pipeline Prepare(ResourceFactory factory, Framebuffer framebuffer)
        {
            if (factory is null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (framebuffer is null)
            {
                throw new ArgumentNullException(nameof(framebuffer));
            }

            var vertexType = typeof(VertexT);
            var layoutField = vertexType.GetField("Layout", BindingFlags.Public | BindingFlags.Static);
            if (layoutField is null)
            {
                throw new ArgumentException($"Type argument {vertexType.Name} does not contain a static Layout field.");
            }

            if (layoutField.FieldType != typeof(VertexLayoutDescription))
            {
                throw new ArgumentException($"Type argument {vertexType.Name}'s Layout field is not of type VertexLayoutDescription.");
            }


            return factory.CreateGraphicsPipeline(new GraphicsPipelineDescription
            {
                BlendState = BlendStateDescription.SingleOverrideBlend,
                DepthStencilState = new DepthStencilStateDescription
                {
                    DepthTestEnabled = true,
                    DepthWriteEnabled = true,
                    DepthComparison = ComparisonKind.LessEqual
                },
                RasterizerState = new RasterizerStateDescription
                {
                    CullMode = FaceCullMode.Back,
                    FillMode = PolygonFillMode.Solid,
                    FrontFace = FrontFace.Clockwise,
                    DepthClipEnabled = true,
                    ScissorTestEnabled = false
                },
                PrimitiveTopology = PrimitiveTopology.TriangleStrip,
                ResourceBindingModel = ResourceBindingModel.Improved,
                ResourceLayouts = Array.Empty<ResourceLayout>(),
                ShaderSet = new ShaderSetDescription
                {
                    VertexLayouts = new VertexLayoutDescription[] { (VertexLayoutDescription)layoutField.GetValue(null) },
                    Shaders = factory.CreateFromSpirv(vertShaderDesc, fragShaderDesc)
                },
                Outputs = framebuffer.OutputDescription
            });
        }
    }
}
