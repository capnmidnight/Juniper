using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

using Juniper.Imaging;
using Juniper.IO;

using Veldrid;
using Veldrid.Utilities;

namespace Juniper.VeldridIntegration
{
    public sealed class Model : IDisposable
    {
        private readonly ConstructedMeshInfo[] meshes;
        private readonly BoundingSphere[] bounds;
        private readonly DeviceBuffer[] vertexBuffers;
        private readonly DeviceBuffer[] indexBuffers;
        private readonly ResourceSet[] resourceSets;
        private readonly Dictionary<string, string>[] meshMapNames;
        private readonly Dictionary<string, Texture> textures = new Dictionary<string, Texture>();
        private readonly Dictionary<string, TextureView> textureViews = new Dictionary<string, TextureView>();
        private readonly IReadOnlyDictionary<string, MaterialDefinition> materials;
        private readonly IDataSource dataSource;
        private Task loading = Task.CompletedTask;

        public Model(string objFileName, IDataSource dataSource)
        {
            if (string.IsNullOrEmpty(objFileName))
            {
                throw new ArgumentException("Must provide a file name for the OBJ file", nameof(objFileName));
            }

            if (dataSource is null)
            {
                throw new ArgumentNullException(nameof(dataSource));
            }

            this.dataSource = dataSource;

            using var objStream = dataSource.GetStream(objFileName);
            var objLoader = new ObjDeserializer();
            var obj = objLoader.Deserialize(objStream);

            var directoryParts = objFileName.Split('/')
                .Reverse()
                .Skip(1)
                .Reverse();

            var mtlFileNameParts = directoryParts
                .Append(obj.MaterialLibName)
                .ToArray();

            var mtlFileName = string.Join("/", mtlFileNameParts);
            using var mtlStream = dataSource.GetStream(mtlFileName);
            var mtlLoader = new MtlDeserializer();
            var mtl = mtlLoader.Deserialize(mtlStream);

            materials = mtl.Definitions;

            meshes = new ConstructedMeshInfo[obj.MeshGroups.Length];
            bounds = new BoundingSphere[meshes.Length];
            meshMapNames = new Dictionary<string, string>[meshes.Length];
            vertexBuffers = new DeviceBuffer[meshes.Length];
            indexBuffers = new DeviceBuffer[meshes.Length];
            resourceSets = new ResourceSet[meshes.Length];

            for (var i = 0; i < obj.MeshGroups.Length; i++)
            {
                var group = obj.MeshGroups[i];
                var mesh
                    = meshes[i]
                    = obj.GetMesh(group);
                var mapNames
                    = meshMapNames[i]
                    = new Dictionary<string, string>();

                bounds[i] = mesh.GetBoundingSphere();

                if (mesh.MaterialName is object
                    && materials.ContainsKey(mesh.MaterialName))
                {
                    var mat = materials[mesh.MaterialName];
                    foreach (var (name, path) in GetMaps(mat))
                    {
                        if (path is object)
                        {
                            mapNames[name] = path;
                        }
                    }
                }
            }
        }

        private IEnumerable<(string name, string path)> GetMaps(MaterialDefinition mat)
        {
            if (mat is null)
            {
                throw new ArgumentNullException(nameof(mat));
            }

            if (mat.DiffuseTexture is object)
            {
                yield return (nameof(mat.DiffuseTexture), mat.DiffuseTexture);
            }

            if (mat.AlphaMap is object)
            {
                yield return (nameof(mat.AlphaMap), mat.AlphaMap);
            }

            if (mat.BumpMap is object)
            {
                yield return (nameof(mat.BumpMap), mat.BumpMap);
            }

            if (mat.DisplacementMap is object)
            {
                yield return (nameof(mat.DisplacementMap), mat.DisplacementMap);
            }

            if (mat.AmbientTexture is object)
            {
                yield return (nameof(mat.AmbientTexture), mat.AmbientTexture);
            }

            if (mat.SpecularColorTexture is object)
            {
                yield return (nameof(mat.SpecularColorTexture), mat.SpecularColorTexture);
            }

            if (mat.SpecularHighlightTexture is object)
            {
                yield return (nameof(mat.SpecularHighlightTexture), mat.SpecularHighlightTexture);
            }

            if (mat.StencilDecalTexture is object)
            {
                yield return (nameof(mat.StencilDecalTexture), mat.StencilDecalTexture);
            }
        }


        public void Draw(GraphicsDevice device, CommandList commandList, Camera camera, Matrix4x4 worldMatrix)
        {
            if (device is null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            if (commandList is null)
            {
                throw new ArgumentNullException(nameof(commandList));
            }

            if (camera is null)
            {
                throw new ArgumentNullException(nameof(camera));
            }

            for (var i = 0; i < meshes.Length; ++i)
            {
                var mesh = meshes[i];
                if (mesh.Indices.Length > 0)
                {
                    var bound = bounds[i];
                    var center = Vector3.Transform(bound.Center, worldMatrix);
                    var toCenter = Vector3.Normalize(center - camera.Position);
                    var angleTest = Vector3.Dot(camera.Forward, toCenter);
                    if (angleTest > 0)
                    {
                        var loaded = resourceSets[i] is object;
                        if (loaded)
                        {
                            var indexCount = (uint)(mesh.Indices.Length);
                            var faceCount = indexCount / 3;
                            commandList.SetVertexBuffer(0, vertexBuffers[i]);
                            commandList.SetIndexBuffer(indexBuffers[i], IndexFormat.UInt16);
                            commandList.SetGraphicsResourceSet(2, resourceSets[i]);
                            commandList.DrawIndexed(
                                indexCount: indexCount,
                                instanceCount: faceCount,
                                indexStart: 0,
                                vertexOffset: 0,
                                instanceStart: 0);
                        }
                        else if (!loading.IsRunning())
                        {
                            loading = LoadMeshAsync(device, i);
                        }
                    }
                }
            }
        }

        public void Preload(GraphicsDevice device)
        {
            if (device is null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            for (var i = 0; i < meshes.Length; ++i)
            {
                LoadMesh(device, i);
            }
        }

        private Task LoadMeshAsync(GraphicsDevice device, int index)
        {
            return Task.Run(() => LoadMesh(device, index));
        }

        private void LoadMesh(GraphicsDevice device, int index)
        {
            var factory = device.ResourceFactory;
            var mesh = meshes[index];
            var vertexBuffer
                = vertexBuffers[index]
                = factory.CreateBuffer(new BufferDescription((uint)(mesh.Vertices.Length * typeof(VertexPositionNormalTexture).Size()), BufferUsage.VertexBuffer));
            device.UpdateBuffer(vertexBuffer, 0, mesh.Vertices);

            var indexBuffer
                = indexBuffers[index]
                = factory.CreateBuffer(new BufferDescription((uint)(mesh.Indices.Length * typeof(ushort).Size()), BufferUsage.IndexBuffer));
            device.UpdateBuffer(indexBuffer, 0, mesh.Indices);

            var mapNames = meshMapNames[index];
            var texturesToLoad = mapNames
                .Values
                .Distinct()
                .Where(path => !textureViews.ContainsKey(path))
                .ToArray();

            for (var j = 0; j < texturesToLoad.Length; j++)
            {
                var path = texturesToLoad[j];
                if (!textures.ContainsKey(path))
                {
                    var decoder = MediaType.GuessByFileName(path)
                        .Where(ImageDecoderSet.Default.ContainsKey)
                        .Select(type => ImageDecoderSet.Default[type])
                        .FirstOrDefault();
                    using var stream = dataSource.GetStream(path);
                    var image = decoder.Deserialize(stream);
                    var imageData = image.GetData();
                    var imageWidth = (uint)image.Info.Dimensions.Width;
                    var imageHeight = (uint)image.Info.Dimensions.Height;

                    var texture = factory.CreateTexture(new TextureDescription(
                        imageWidth, imageHeight, 1,
                        1, 1,
                        PixelFormat.R8_G8_B8_A8_UNorm,
                        TextureUsage.Sampled,
                        TextureType.Texture2D));

                    textures[path] = texture;
                    textureViews[path] = factory.CreateTextureView(texture);

                    device.UpdateTexture(
                        texture,
                        imageData,
                        0, 0, 0,
                        imageWidth, imageHeight, 1,
                        0, 0);
                }
            }

            var elements = mapNames.Keys.Select(name => new ResourceLayoutElementDescription()
            {
                Name = name,
                Kind = ResourceKind.TextureReadOnly,
                Stages = ShaderStages.Fragment
            }).ToArray();

            var layout = factory.CreateResourceLayout(new ResourceLayoutDescription(elements));
            var resources = new BindableResource[elements.Length];
            for (var i = 0; i < elements.Length; ++i)
            {
                resources[i] = textureViews[mapNames[elements[i].Name]];
            }
            resourceSets[index] = factory.CreateResourceSet(new ResourceSetDescription(layout, resources));
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
                foreach (var resourceSet in resourceSets)
                {
                    resourceSet?.Dispose();
                }

                foreach (var textureView in textureViews.Values)
                {
                    textureView?.Dispose();
                }
                textureViews.Clear();

                foreach (var texture in textures.Values)
                {
                    texture?.Dispose();
                }
                textures.Clear();

                foreach (var vertexBuffer in vertexBuffers)
                {
                    vertexBuffer?.Dispose();
                }

                foreach (var indexBuffer in indexBuffers)
                {
                    indexBuffer?.Dispose();
                }
            }
        }
    }
}
