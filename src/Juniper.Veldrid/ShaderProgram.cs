using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

using Juniper.Imaging;
using Juniper.IO;
using Veldrid;
using Veldrid.SPIRV;
using Veldrid.Utilities;

namespace Juniper.VeldridIntegration
{
    public class ShaderProgram<VertexT>
        : IDisposable
        where VertexT : struct
    {
        private readonly List<Model> models = new List<Model>();
        private readonly IDataSource dataSource;
        private readonly ShaderProgramDescription<VertexT> programDescription;

        private Pipeline pipeline;
        private ResourceLayout[] layouts;
        private IDisposable[] disposableResources;
        private ResourceSet resourceSet;

        private DeviceBuffer cameraBuffer;

        public bool IsRunning { get; private set; }

        public ShaderProgram(IDataSource dataSource, ShaderProgramDescription<VertexT> programDescription)
        {
            if (dataSource is null)
            {
                throw new ArgumentNullException(nameof(dataSource));
            }

            if (!programDescription.UseSpirV)
            {
                throw new NotImplementedException("Support for only SPIR-V shaders are currently implemented.");
            }

            this.dataSource = dataSource;
            this.programDescription = programDescription;
        }

        public void LoadModel(string name, GraphicsDevice device = null)
        {
            LoadModel(name, dataSource, device);
        }

        public void LoadModel(string name, IDataSource dataSource, GraphicsDevice device = null)
        {
            AddModel(new Model(name, dataSource), device);
        }

        public void AddModel(Model model, GraphicsDevice device = null)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if(device != null)
            {
                model.Preload(device);
            }

            models.Add(model);
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

            var vertex = programDescription.VertexShader;
            var fragment = programDescription.FragmentShader;

            var pipelineOptions = programDescription.PipelineOptions;
            pipelineOptions.ResourceLayouts = layouts = new ResourceLayout[]
            {
                factory.CreateResourceLayout(new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("ProjectionBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                    new ResourceLayoutElementDescription("DiffuseSampler", ResourceKind.Sampler, ShaderStages.Fragment))),
                factory.CreateResourceLayout(new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("WorldBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex))),
                factory.CreateResourceLayout(new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("DiffuseTexture", ResourceKind.TextureReadOnly, ShaderStages.Fragment)))
            };

            var vertexResources = new BindableResource[]
            {
                cameraBuffer = factory.CreateBuffer(new BufferDescription(vertex.Resources[0].Size, BufferUsage.UniformBuffer)),
                device.Aniso4xSampler
            };

            disposableResources = vertexResources
                .Where(r => r != null
                    && r != device.Aniso4xSampler)
                .OfType<IDisposable>()
                .ToArray();

            resourceSet = factory.CreateResourceSet(new ResourceSetDescription(layouts[0], vertexResources));

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

                cameraBuffer?.Dispose();
                cameraBuffer = null;

                resourceSet?.Dispose();
                resourceSet = null;

                if (disposableResources != null)
                {
                    foreach (var r in disposableResources)
                    {
                        r.Dispose();
                    }
                    disposableResources = null;
                }

                if (layouts != null)
                {
                    foreach (var l in layouts)
                    {
                        l.Dispose();
                    }
                    layouts = null;
                }

                pipeline?.Dispose();
                pipeline = null;

                foreach(var model in models)
                {
                    model.Dispose();
                }
            }
        }

        public WorldObj<VertexT> CreateObject(int modelIndex)
        {
            return new WorldObj<VertexT>(this, modelIndex);
        }

        internal void Draw(GraphicsDevice device, CommandList commandList, Camera camera, int modelIndex, Matrix4x4 worldMatrix)
        {
            if (!IsRunning)
            {
                throw new InvalidOperationException("Cannot draw when the program is not running.");
            }

            if (commandList is null)
            {
                throw new ArgumentNullException(nameof(commandList));
            }

            if (camera is null)
            {
                throw new ArgumentNullException(nameof(camera));
            }

            commandList.SetGraphicsResourceSet(0, resourceSet);

            models[modelIndex].Draw(device, commandList, camera, worldMatrix);
        }

        public void Activate(CommandList commandList, Camera camera)
        {
            if (!IsRunning)
            {
                throw new InvalidOperationException("Cannot draw when the program is not running.");
            }

            if (commandList is null)
            {
                throw new ArgumentNullException(nameof(commandList));
            }

            if (camera is null)
            {
                throw new ArgumentNullException(nameof(camera));
            }

            commandList.SetPipeline(pipeline);
            commandList.UpdateBuffer(cameraBuffer, 0, camera.Projection);
            commandList.UpdateBuffer(cameraBuffer, camera.ViewOffset, camera.View);
        }
    }
}
