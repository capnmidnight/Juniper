using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Veldrid;
using Veldrid.SPIRV;

namespace Juniper.VeldridIntegration
{
    public sealed class Material<VertexT>
        : Material, IDisposable
        where VertexT : struct
    { 
        private readonly ShaderDescription vertShaderDesc;
        private readonly ShaderDescription fragShaderDesc;
        private readonly List<(ResourceLayout layout, BindableResource[] resources)> layouts = new List<(ResourceLayout layout, BindableResource[] resources)>();
        private readonly List<ResourceSet> resources = new List<ResourceSet>();

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

        ~Material()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var l in layouts)
                {
                    l.layout.Dispose();
                    foreach(var r in l.resources)
                    {
                        if(r is IDisposable d)
                        {
                            d.Dispose();
                        }
                    }
                }

                layouts.Clear();

                foreach (var resource in resources)
                {
                    resource.Dispose();
                }
                resources.Clear();
            }
        }

        internal void SetResources(CommandList commandList)
        {
            for(var i = 0; i < resources.Count; ++i)
            {
                commandList.SetGraphicsResourceSet((uint)i, resources[i]);
            }
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

            var resourceLayouts = new ResourceLayout[layouts.Count];
            for (var i = 0; i < layouts.Count; ++i)
            {
                var l = layouts[i];
                resourceLayouts[i] = l.layout;
                resources.Add(factory.CreateResourceSet(new ResourceSetDescription(l.layout, l.resources)));
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
                ResourceLayouts = resourceLayouts,
                ShaderSet = new ShaderSetDescription
                {
                    VertexLayouts = new VertexLayoutDescription[] { (VertexLayoutDescription)layoutField.GetValue(null) },
                    Shaders = factory.CreateFromSpirv(vertShaderDesc, fragShaderDesc)
                },
                Outputs = framebuffer.OutputDescription
            });
        }

        public void AddResource(ResourceLayout layout, params BindableResource[] resources)
        {
            if (layout is null)
            {
                throw new ArgumentNullException(nameof(layout));
            }

            layouts.Add((layout, resources));
        }
    }
}
