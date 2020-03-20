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
        private readonly List<Mesh<VertexT>> meshes = new List<Mesh<VertexT>>();
        private readonly IndexFormat indexFormat;
        private readonly ShaderProgramDescription<VertexT> programDescription;
        private readonly Func<ConstructedMeshInfo, Mesh<VertexT>> convertMesh;

        private Pipeline pipeline;
        private ResourceLayout[] layouts;
        private IDisposable[] resources;
        private ResourceSet[] resourceSets;

        private uint faceCount;
        private uint indexCount;
        private DeviceBuffer vertexBuffer;
        private DeviceBuffer indexBuffer;
        private DeviceBuffer projectionBuffer;
        private DeviceBuffer viewBuffer;
        private DeviceBuffer worldBuffer;

        public bool IsRunning { get; private set; }

        public ShaderProgram(ShaderProgramDescription<VertexT> programDescription, Func<ConstructedMeshInfo, Mesh<VertexT>> convertMesh)
        {
            if (!programDescription.UseSpirV)
            {
                throw new NotImplementedException("Support for only SPIR-V shaders are currently implemented.");
            }

            this.programDescription = programDescription;
            this.convertMesh = convertMesh;

            var meshType = typeof(Mesh<VertexT>);
            var indexFormatAttr = meshType.GetCustomAttribute<IndexTypeAttribute>(false);
            if (indexFormatAttr == null)
            {
                throw new InvalidOperationException("Cannot get vertex index type information");
            }

            indexFormat = indexFormatAttr.Value.ToIndexFormat();
        }

        public void AddMesh(Mesh<VertexT> mesh)
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

        public void LoadOBJ(FileInfo objFile)
        {
            if (objFile is null)
            {
                throw new ArgumentNullException(nameof(objFile));
            }

            if (!objFile.Exists)
            {
                throw new FileNotFoundException("Could not find OBJ file", objFile.FullName);
            }

            var directory = objFile.Directory;

            using var objStream = objFile.OpenRead();
            var objParser = new ObjParser();
            var obj = objParser.Parse(objStream);

            var mtlFileName = Path.Combine(directory.FullName, obj.MaterialLibName);
            var mtlFile = new FileInfo(mtlFileName);

            if (!mtlFile.Exists)
            {
                throw new FileNotFoundException("Could not find MTL file", mtlFile.FullName);
            }

            using var mtlStream = mtlFile.OpenRead();
            var mtlParser = new MtlParser();
            var mtl = mtlParser.Parse(mtlStream);

            foreach (var group in obj.MeshGroups)
            {
                var veldridMesh = obj.GetMesh(group);
                var mesh = convertMesh(veldridMesh);
                AddMesh(mesh);

                if (veldridMesh.MaterialName is object)
                {
                    var materialDef = mtl.Definitions[veldridMesh.MaterialName];
                    if (materialDef.DiffuseTexture is object)
                    {
                        var textureFileName = Path.Combine(directory.FullName, materialDef.DiffuseTexture);
                        AddTexture("SurfaceTexture", ImageDecoderSet.Default.LoadImage(textureFileName));
                    }
                }
            }
        }

        public void LoadOBJ(string objFileName)
        {
            if (string.IsNullOrEmpty(objFileName))
            {
                throw new ArgumentException("Must provide a filename", nameof(objFileName));
            }

            LoadOBJ(new FileInfo(objFileName));
        }

        public void Begin(GraphicsDevice device, Framebuffer framebuffer, string projectionBufferName, string viewBufferName, string worldBufferName)
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
                            ResourceKind.UniformBuffer => CreateBuffer(factory, r),
                            _ => throw new FormatException("Unknonw resource kind " + r.Kind)
                        }))
                        .ToArray();
                    return (layout, resources);
                })
                .ToArray();

            projectionBuffer = buffers[projectionBufferName];
            _ = buffers.Remove(projectionBufferName);

            viewBuffer = buffers[viewBufferName];
            _ = buffers.Remove(viewBufferName);

            worldBuffer = buffers[worldBufferName];
            _ = buffers.Remove(worldBufferName);

            var pipelineOptions = programDescription.PipelineOptions;

            pipelineOptions.ResourceLayouts = layouts = layoutsAndResources.Select(l => l.layout).ToArray();

            resources = layoutsAndResources.SelectMany(l => l.resources)
                .OfType<IDisposable>()
                .Where(r => r != device.Aniso4xSampler)
                .ToArray();

            resourceSets = layoutsAndResources
                .Select(l => factory.CreateResourceSet(new ResourceSetDescription(l.layout, l.resources)))
                .ToArray();

            var mesh = meshes.Aggregate((a, b) => a + b);
            faceCount = mesh.FaceCount;
            indexCount = mesh.IndexCount;
            (vertexBuffer, indexBuffer) = mesh.Prepare(device);

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
                worldBuffer?.Dispose();
                worldBuffer = null;
                viewBuffer?.Dispose();
                viewBuffer = null;
                projectionBuffer?.Dispose();
                projectionBuffer = null;

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

            commandList.DrawIndexed(
                indexCount: indexCount,
                instanceCount: faceCount,
                indexStart: 0,
                vertexOffset: 0,
                instanceStart: 0);
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
            commandList.SetVertexBuffer(0, vertexBuffer);
            commandList.SetIndexBuffer(indexBuffer, indexFormat);

            commandList.UpdateBuffer(projectionBuffer, 0, camera.Projection);
            commandList.UpdateBuffer(viewBuffer, 0, camera.View);
        }
    }
}
