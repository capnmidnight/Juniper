using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Juniper.Imaging;

using Veldrid;
using Veldrid.SPIRV;

namespace Juniper.VeldridIntegration
{
    public class ShaderProgram<VertexT>
        : IDisposable
        where VertexT : struct
    {
        private readonly Dictionary<string, DeviceBuffer> buffers = new Dictionary<string, DeviceBuffer>();
        private readonly List<(string name, ImageData image)> textureData = new List<(string name, ImageData image)>();
        private readonly Dictionary<string, Texture> textures = new Dictionary<string, Texture>();
        private readonly Mesh<VertexT> mesh;
        private readonly IndexFormat indexFormat;
        private readonly ShaderProgramDescription<VertexT> programDescription;

        private Pipeline pipeline;
        private ResourceLayout[] layouts;
        private IDisposable[] resources;
        private ResourceSet[] resourceSets;

        private DeviceBuffer vertexBuffer;
        private DeviceBuffer indexBuffer;

        public bool IsRunning { get; private set; }

        public ShaderProgram(ShaderProgramDescription<VertexT> programDescription, Mesh<VertexT> mesh)
        {
            if (!programDescription.UseSpirV)
            {
                throw new NotImplementedException("Support for only SPIR-V shaders are currently implemented.");
            }

            this.programDescription = programDescription;
            this.mesh = mesh ?? throw new ArgumentNullException(nameof(mesh));
            indexFormat = mesh.IndexType.ToIndexFormat();
        }

        public void AddTexture(string name, ImageData image)
        {
            if (IsRunning)
            {
                throw new InvalidOperationException("Cannot add textures while the program is running.");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Texture name cannot be null or empty.", nameof(name));
            }

            if (image is null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            textureData.Add((name, image));
        }

        public void Begin(GraphicsDevice device, Framebuffer framebuffer)
        {
            if (IsRunning)
            {
                throw new InvalidOperationException("The program is already running.");
            }

            if (device is null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            if (framebuffer is null)
            {
                throw new ArgumentNullException(nameof(framebuffer));
            }

            var factory = device.ResourceFactory;

            if (factory is null)
            {
                throw new ArgumentException("GraphicsDevice is not ready. It has no ResourceFactory", nameof(device));
            }

            foreach (var (name, image) in textureData)
            {
                var imageData = image.GetData();
                var imageWidth = (uint)image.Info.Dimensions.Width;
                var imageHeight = (uint)image.Info.Dimensions.Height;

                var texture = factory.CreateTexture(new TextureDescription(
                    imageWidth, imageHeight, 1,
                    1, 1,
                    PixelFormat.R8_G8_B8_A8_UNorm,
                    TextureUsage.Sampled,
                    TextureType.Texture2D));

                textures[name] = texture;

                device.UpdateTexture(
                    texture,
                    imageData,
                    0, 0, 0,
                    imageWidth, imageHeight, 1,
                    0, 0);
            }

            textureData.Clear();

            var vertex = programDescription.VertexShader;
            var fragment = programDescription.FragmentShader;

            var layoutsAndResources = vertex.Resources
                .Concat(fragment.Resources)
                .GroupBy(r => r.Set)
                .Select(g => g.ToArray())
                .Select(set =>
                {
                    var elements = set.Select(e => e.ToElementDescription()).ToArray();
                    var layout = factory.CreateResourceLayout(new ResourceLayoutDescription(elements));
                    var resources = set.Select(r => CreateResource(device, factory, r))
                        .ToArray();
                    return (layout, resources);
                })
                .ToArray();

            var pipelineOptions = programDescription.PipelineOptions;

            pipelineOptions.ResourceLayouts = layouts = layoutsAndResources.Select(l => l.layout).ToArray();

            resources = layoutsAndResources.SelectMany(l => l.resources)
                .OfType<IDisposable>()
                .Where(r => r != device.Aniso4xSampler)
                .ToArray();

            resourceSets = layoutsAndResources
                .Select(l => factory.CreateResourceSet(new ResourceSetDescription(l.layout, l.resources)))
                .ToArray();

            (vertexBuffer, indexBuffer) = this.mesh.Prepare(device);

            var layout = programDescription.VertexLayout;
            if (device.BackendType == GraphicsBackend.Direct3D11
                || programDescription.UseSpirV)
            {
                for (var i = 0; i < programDescription.VertexLayout.Elements.Length; ++i)
                {
                    programDescription.VertexLayout.Elements[i].Semantic = VertexElementSemantic.TextureCoordinate;
                }
            }

            pipelineOptions.ShaderSet = new ShaderSetDescription
            {
                VertexLayouts = new VertexLayoutDescription[] { layout },
                Shaders = CreateShaders(factory, programDescription)
            };

            pipelineOptions.Outputs = framebuffer.OutputDescription;
            pipeline = factory.CreateGraphicsPipeline(pipelineOptions);

            IsRunning = true;
        }

        private static Shader[] CreateShaders(ResourceFactory factory, ShaderProgramDescription<VertexT> material)
        {
            if (material.UseSpirV)
            {
                return factory.CreateFromSpirv(material.VertexShader.Description, material.FragmentShader.Description);
            }
            else
            {
                throw new InvalidOperationException("SPIR-V is the only supported shader format.");
            }
        }

        private BindableResource CreateResource(GraphicsDevice device, ResourceFactory factory, ShaderResource r)
        {
            return r.Kind switch
            {
                ResourceKind.Sampler => device.Aniso4xSampler,
                ResourceKind.TextureReadOnly => factory.CreateTextureView(textures[r.Name]),
                ResourceKind.TextureReadWrite => factory.CreateTextureView(textures[r.Name]),
                ResourceKind.UniformBuffer => CreateBuffer(factory, r),
                _ => throw new FormatException("Unknonw resource kind " + r.Kind)
            };
        }

        private DeviceBuffer CreateBuffer(ResourceFactory factory, ShaderResource r)
        {
            return buffers[r.Name] = factory.CreateBuffer(new BufferDescription(r.Size, BufferUsage.UniformBuffer)); ;
        }

        public void UpdateMatrix(string name, CommandList commandList, ref Matrix4x4 matrix)
        {
            if (!IsRunning)
            {
                throw new InvalidOperationException("Cannot update matrices when the program is not running.");
            }

            if (commandList is null)
            {
                throw new ArgumentNullException(nameof(commandList));
            }

            commandList.UpdateBuffer(buffers[name], 0, ref matrix);
        }

        public Camera CreateCamera(string projectionBufferName, string viewBufferName)
        {
            if (string.IsNullOrEmpty(projectionBufferName))
            {
                throw new ArgumentException("Must provide a name for the projection buffer.", nameof(projectionBufferName));
            }

            if (string.IsNullOrEmpty(viewBufferName))
            {
                throw new ArgumentException("Must provide a name for the view buffer", nameof(viewBufferName));
            }

            var proj = buffers[projectionBufferName];
            buffers.Remove(projectionBufferName);

            var view = buffers[viewBufferName];
            buffers.Remove(viewBufferName);

            return new Camera(proj, view);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                IsRunning = false;

                pipeline?.Dispose();
                pipeline = null;
                indexBuffer?.Dispose();
                indexBuffer = null;
                vertexBuffer?.Dispose();
                vertexBuffer = null;

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

                foreach (var texture in textures.Values)
                {
                    texture.Dispose();
                }
                textures.Clear();
            }
        }

        public void Draw(CommandList commandList)
        {
            if (!IsRunning)
            {
                throw new InvalidOperationException("Cannot draw when the program is not running.");
            }

            if (commandList is null)
            {
                throw new ArgumentNullException(nameof(commandList));
            }

            commandList.SetPipeline(pipeline);
            for (var i = 0; i < resourceSets.Length; ++i)
            {
                commandList.SetGraphicsResourceSet((uint)i, resourceSets[i]);
            }
            commandList.SetVertexBuffer(0, vertexBuffer);
            commandList.SetIndexBuffer(indexBuffer, indexFormat);

            commandList.DrawIndexed(
                indexCount: mesh.IndexCount,
                instanceCount: mesh.FaceCount,
                indexStart: 0,
                vertexOffset: 0,
                instanceStart: 0);
        }
    }
}
