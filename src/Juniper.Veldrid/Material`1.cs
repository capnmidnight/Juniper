using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Veldrid;
using Veldrid.SPIRV;

namespace Juniper.VeldridIntegration
{
    public sealed class Material<VertexT>
        : Material, IDisposable
        where VertexT : struct
    {
        private readonly VertexLayoutDescription vertLayout;

        private readonly ParsedShader vertex;
        private readonly ParsedShader fragment;

        private ResourceLayout[] layouts;
        private IDisposable[] resources;
        private ResourceSet[] resourceSets;
        private readonly Dictionary<string, Texture> textures = new Dictionary<string, Texture>();
        private readonly Dictionary<string, DeviceBuffer> buffers = new Dictionary<string, DeviceBuffer>();

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

            var info = VertexTypeCache.GetDescription<VertexT>();
            vertLayout = info.layout;

            vertex = new ParsedShader(ShaderStages.Vertex, vertShaderBytes);
            fragment = new ParsedShader(ShaderStages.Fragment, fragShaderBytes);

            ValidateVertShaderInputsMatchVertLayout();
            ValidateVertShaderOutputsMatchFragShaderOutputs();
        }

        private void ValidateVertShaderInputsMatchVertLayout()
        {
            var vertInputs = vertex.Attributes.Where(a => a.Direction == ShaderAttributeDirection.In).ToArray();
            if (vertInputs.Length != vertLayout.Elements.Length)
            {
                throw new FormatException($"Vertex shader input count ({vertInputs.Length}) does not match vert type layout elements ({vertLayout.Elements.Length})");
            }

            for (var i = 0; i < vertInputs.Length; ++i)
            {
                var vertInput = vertInputs[i];
                var vertLayoutElement = vertLayout.Elements[i];
                var size = vertLayoutElement.Format.Size();

                if (vertInput.Name != vertLayoutElement.Name)
                {
                    throw new FormatException($"Vertex shader input '{vertInput}' name is {vertInput.Name}, but vertex layout description expected {vertLayoutElement.Name}.");
                }
                if (vertInput.DataType.Size() != size)
                {
                    throw new FormatException($"Vertex shader input '{vertInput}' size is {vertInput.DataType.Size()}, but vertex layout description expected {size}.");
                }
                if (vertInput.Component != vertLayoutElement.Offset)
                {
                    throw new FormatException($"Vertex shader input '{vertInput}' offset is {vertInput.Component}, but vertex layout description expected {vertLayoutElement.Offset}.");
                }
            }
        }

        private void ValidateVertShaderOutputsMatchFragShaderOutputs()
        {
            var vertOutputs = vertex.Attributes.Where(a => a.Direction == ShaderAttributeDirection.Out).ToArray();
            var fragInputs = fragment.Attributes.Where(a => a.Direction == ShaderAttributeDirection.In).ToArray();

            if (vertOutputs.Length != fragInputs.Length)
            {
                throw new FormatException($"Vertex shader output count ({vertOutputs.Length}) does not match frag shader input count ({fragInputs.Length}");
            }

            for (var i = 0; i < vertOutputs.Length; ++i)
            {
                var vertOutput = vertOutputs[i];
                var fragInput = fragInputs[i];
                if (!vertOutput.PipesTo(fragInput))
                {
                    throw new FormatException($"Vertex shader output '{vertOutput}' does not match frag shader input `{fragInput}'.");
                }
            }
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
                    l.Dispose();
                }
                layouts = null;

                foreach (var r in resources)
                {
                    r.Dispose();
                }
                resources = null;

                foreach (var set in resourceSets)
                {
                    set.Dispose();
                }
                resourceSets = null;
            }
        }

        public void CreateResources(GraphicsDevice device, ResourceFactory factory)
        {
            if (factory is null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            var layoutsAndResources = vertex.Resources
                .Concat(fragment.Resources)
                .GroupBy(r => r.Set)
                .Select(g => g.ToArray())
                .Select(set =>
                {
                    var elements = set.Select(e => (ResourceLayoutElementDescription)e).ToArray();
                    var layout = factory.CreateResourceLayout(new ResourceLayoutDescription(elements));
                    var resources = set.Select(r => CreateResource(device, factory, r))
                        .ToArray();
                    return (layout, resources);
                })
                .ToArray();

            layouts = layoutsAndResources.Select(l => l.layout).ToArray();
            resources = layoutsAndResources.SelectMany(l => l.resources)
                .OfType<IDisposable>()
                .Where(r => r != device.Aniso4xSampler)
                .ToArray();

            resourceSets = layoutsAndResources
                .Select(l => factory.CreateResourceSet(new ResourceSetDescription(l.layout, l.resources)))
                .ToArray();
        }

        private BindableResource CreateResource(GraphicsDevice device, ResourceFactory factory, ShaderResource r)
        {
            return r.Kind switch
            {
                ResourceKind.Sampler => device.Aniso4xSampler,
                ResourceKind.TextureReadOnly => CreateTextureView(factory, r),
                ResourceKind.UniformBuffer => CreateBuffer(factory, r),
                _ => throw new FormatException("Unknonw resource kind " + r.Kind)
            };
        }

        private DeviceBuffer CreateBuffer(ResourceFactory factory, ShaderResource r)
        {
            var buffer = factory.CreateBuffer(new BufferDescription(r.Size, BufferUsage.UniformBuffer));
            buffers[r.Name] = buffer;
            return buffer;
        }

        public void UpdateMatrix(string name, CommandList commandList, ref Matrix4x4 matrix)
        {
            if (commandList is null)
            {
                throw new ArgumentNullException(nameof(commandList));
            }

            commandList.UpdateBuffer(buffers[name], 0, ref matrix);
        }

        private TextureView CreateTextureView(ResourceFactory factory, ShaderResource r)
        {
            return factory.CreateTextureView(textures[r.Name]);
        }

        public void AddTexture(string name, Texture texture)
        {
            textures[name] = texture;
        }

        internal void SetResources(CommandList commandList)
        {
            for (var i = 0; i < resourceSets.Length; ++i)
            {
                commandList.SetGraphicsResourceSet((uint)i, resourceSets[i]);
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
                ResourceLayouts = layouts,
                ShaderSet = new ShaderSetDescription
                {
                    VertexLayouts = new VertexLayoutDescription[] { vertLayout },
                    Shaders = factory.CreateFromSpirv(vertex.Description, fragment.Description)
                },
                Outputs = framebuffer.OutputDescription
            });
        }
    }
}
