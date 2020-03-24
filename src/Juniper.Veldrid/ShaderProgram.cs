using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

using Juniper.Imaging;

using Veldrid;
using Veldrid.SPIRV;
using Veldrid.Utilities;

namespace Juniper.VeldridIntegration
{
    public class ShaderProgram<VertexT>
        : IDisposable
        where VertexT : struct
    {
        private readonly Dictionary<string, DeviceBuffer> buffers = new Dictionary<string, DeviceBuffer>();
        private readonly List<(string name, ImageData image)> textureData = new List<(string name, ImageData image)>();
        private readonly Dictionary<string, Texture> textures = new Dictionary<string, Texture>();
        private readonly List<ConstructedMeshInfo> meshes = new List<ConstructedMeshInfo>();
        private readonly IndexFormat indexFormat;
        private readonly ShaderProgramDescription<VertexT> programDescription;

        private Pipeline pipeline;
        private ResourceLayout[] layouts;
        private IDisposable[] resources;
        private ResourceSet[] resourceSets;

        private DeviceBuffer cameraBuffer;
        private DeviceBuffer worldBuffer;
        private DeviceBuffer[] vertexBuffers;
        private DeviceBuffer[] indexBuffers;

        public bool IsRunning { get; private set; }

        public ShaderProgram(ShaderProgramDescription<VertexT> programDescription)
        {
            if (!programDescription.UseSpirV)
            {
                throw new NotImplementedException("Support for only SPIR-V shaders are currently implemented.");
            }

            this.programDescription = programDescription;
            indexFormat = typeof(ushort).ToIndexFormat();
        }

        public void AddMesh(ConstructedMeshInfo mesh)
        {
            if (mesh is null)
            {
                throw new ArgumentNullException(nameof(mesh));
            }

            meshes.Add(mesh);
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

        public void LoadOBJ(string fileID, Func<string, Stream> getStream)
        {
            if (fileID is null)
            {
                throw new ArgumentNullException(nameof(fileID));
            }

            if (getStream is null)
            {
                throw new ArgumentNullException(nameof(getStream));
            }

            using var objStream = getStream(fileID);
            var objParser = new ObjParser();
            var obj = objParser.Parse(objStream);

            var directoryParts = fileID.Split('/')
                .Reverse()
                .Skip(1)
                .Reverse();

            var mtlFileNameParts = directoryParts
                .Append(obj.MaterialLibName)
                .ToArray();

            var mtlFileName = string.Join("/", mtlFileNameParts);
            using var mtlStream = getStream(mtlFileName);
            var mtlParser = new MtlParser();
            var mtl = mtlParser.Parse(mtlStream);

            foreach (var group in obj.MeshGroups)
            {
                var mesh = obj.GetMesh(group);
                if (mesh.Indices.Length > 0)
                {
                    AddMesh(mesh);

                    if (mesh.MaterialName is object)
                    {
                        var materialDef = mtl.Definitions[mesh.MaterialName];
                        if (materialDef.DiffuseTexture is object)
                        {
                            AddTexture("SurfaceTexture", ImageDecoderSet.Default.LoadImage(materialDef.DiffuseTexture, getStream));
                        }
                    }
                }
            }
        }

        public void Begin(GraphicsDevice device, Framebuffer framebuffer, string cameraBufferName, string worldBufferName)
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
                    var resources = set.Select(r => (BindableResource)(r.Kind switch
                    {
                        ResourceKind.Sampler => device.Aniso4xSampler,
                        ResourceKind.TextureReadOnly => factory.CreateTextureView(textures[r.Name]),
                        ResourceKind.TextureReadWrite => factory.CreateTextureView(textures[r.Name]),
                        ResourceKind.UniformBuffer => buffers[r.Name] = factory.CreateBuffer(new BufferDescription(r.Size, BufferUsage.UniformBuffer)),
                        _ => throw new FormatException("Unknonw resource kind " + r.Kind)
                    }))
                        .ToArray();
                    return (layout, resources);
                })
                .ToArray();

            cameraBuffer = buffers[cameraBufferName];
            _ = buffers.Remove(cameraBufferName);

            worldBuffer = buffers[worldBufferName];
            _ = buffers.Remove(worldBufferName);

            var pipelineOptions = programDescription.PipelineOptions;

            pipelineOptions.ResourceLayouts = layouts = layoutsAndResources.Select(l => l.layout).ToArray();

            resources = layoutsAndResources.SelectMany(l => l.resources)
                .Where(r => r != null)
                .OfType<IDisposable>()
                .Where(r => r != device.Aniso4xSampler)
                .ToArray();

            resourceSets = layoutsAndResources
                .Select(l => factory.CreateResourceSet(new ResourceSetDescription(l.layout, l.resources)))
                .ToArray();

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

            vertexBuffers = new DeviceBuffer[meshes.Count];
            indexBuffers = new DeviceBuffer[meshes.Count];

            for (var i = 0; i < meshes.Count; ++i)
            {
                var mesh = meshes[i];
                var vertexBuffer
                    = vertexBuffers[i]
                    = factory.CreateBuffer(new BufferDescription((uint)(mesh.Vertices.Length * typeof(VertexT).Size()), BufferUsage.VertexBuffer));
                device.UpdateBuffer(vertexBuffer, 0, mesh.Vertices);

                var indexBuffer
                    = indexBuffers[i]
                    = factory.CreateBuffer(new BufferDescription((uint)(mesh.Indices.Length * typeof(ushort).Size()), BufferUsage.IndexBuffer));
                device.UpdateBuffer(indexBuffer, 0, mesh.Indices);
            }

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

                if(indexBuffers != null)
                {
                    foreach(var indexBuffer in indexBuffers)
                    {
                        indexBuffer.Dispose();
                    }
                    indexBuffers = null;
                }

                if (vertexBuffers != null)
                {
                    foreach (var vertexBuffer in vertexBuffers)
                    {
                        vertexBuffer.Dispose();
                    }
                    vertexBuffers = null;
                }
                worldBuffer?.Dispose();
                worldBuffer = null;
                cameraBuffer?.Dispose();
                cameraBuffer = null;

                if (resourceSets != null)
                {
                    foreach (var set in resourceSets)
                    {
                        set.Dispose();
                    }
                    resourceSets = null;
                }

                if (resources != null)
                {
                    foreach (var r in resources)
                    {
                        r.Dispose();
                    }
                    resources = null;
                }

                if (textures != null)
                {
                    foreach (var texture in textures.Values)
                    {
                        texture.Dispose();
                    }
                    textures.Clear();
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
            }
        }

        public WorldObj<VertexT> CreateObject()
        {
            return new WorldObj<VertexT>(this);
        }

        internal void Draw(CommandList commandList, Matrix4x4 worldMatrix)
        {
            if (!IsRunning)
            {
                throw new InvalidOperationException("Cannot draw when the program is not running.");
            }

            if (commandList is null)
            {
                throw new ArgumentNullException(nameof(commandList));
            }

            commandList.UpdateBuffer(worldBuffer, 0, ref worldMatrix);


            for (var i = 0; i < meshes.Count; ++i)
            {
                commandList.SetVertexBuffer(0, vertexBuffers[i]);
                commandList.SetIndexBuffer(indexBuffers[i], indexFormat);

                var mesh = meshes[i];
                var indexCount = (uint)(mesh.Indices.Length);
                var faceCount = indexCount / 3;

                commandList.DrawIndexed(
                    indexCount: indexCount,
                    instanceCount: faceCount,
                    indexStart: 0,
                    vertexOffset: 0,
                    instanceStart: 0);
            }
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
            for (var i = 0; i < resourceSets.Length; ++i)
            {
                commandList.SetGraphicsResourceSet((uint)i, resourceSets[i]);
            }

            commandList.UpdateBuffer(cameraBuffer, 0, camera.Projection);
            commandList.UpdateBuffer(cameraBuffer, camera.Projection.GetType().Size(), camera.View);
        }
    }
}
