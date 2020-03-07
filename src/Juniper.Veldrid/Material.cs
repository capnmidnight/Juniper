using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Veldrid;
using Veldrid.SPIRV;

namespace Juniper.VeldridIntegration
{
    public class Material : IDisposable
    {
        public static Material Create<VertexT>(byte[] vertShaderBytes, byte[] fragShaderBytes)
            where VertexT : struct
        {
            return new Material(typeof(VertexT), vertShaderBytes, fragShaderBytes);
        }

        public static Material Create<VertexT>(string vertShaderText, string fragShaderText)
            where VertexT : struct
        {
            if (vertShaderText is null)
            {
                throw new ArgumentNullException(nameof(vertShaderText));
            }

            if (fragShaderText is null)
            {
                throw new ArgumentNullException(nameof(fragShaderText));
            }

            if (vertShaderText.Length == 0)
            {
                throw new ArgumentException("Shader is empty", nameof(vertShaderText));
            }

            if (fragShaderText.Length == 0)
            {
                throw new ArgumentException("Shader is empty", nameof(fragShaderText));
            }

            return Create<VertexT>(Encoding.UTF8.GetBytes(vertShaderText), Encoding.UTF8.GetBytes(fragShaderText));
        }

        public static Material Create<VertexT>(MemoryStream vertShaderMem, MemoryStream fragShaderMem)
            where VertexT : struct
        {
            if (vertShaderMem is null)
            {
                throw new ArgumentNullException(nameof(vertShaderMem));
            }

            if (fragShaderMem is null)
            {
                throw new ArgumentNullException(nameof(fragShaderMem));
            }

            return Create<VertexT>(vertShaderMem.ToArray(), fragShaderMem.ToArray());
        }

        public static async Task<Material> LoadAsync<VertexT>(Stream vertShaderStream, Stream fragShaderStream)
            where VertexT : struct
        {
            if (vertShaderStream is null)
            {
                throw new ArgumentNullException(nameof(vertShaderStream));
            }

            if (fragShaderStream is null)
            {
                throw new ArgumentNullException(nameof(fragShaderStream));
            }

            using var vertShaderMem = new MemoryStream();
            await vertShaderStream.CopyToAsync(vertShaderMem).ConfigureAwait(false);

            using var fragShaderMem = new MemoryStream();
            await fragShaderStream.CopyToAsync(fragShaderMem).ConfigureAwait(false);

            return Create<VertexT>(vertShaderMem, fragShaderMem);
        }

        public static Task<Material> LoadAsync<VertexT>(FileInfo vertShaderFile, FileInfo fragShaderFile)
           where VertexT : struct
        {
            if (vertShaderFile is null)
            {
                throw new ArgumentNullException(nameof(vertShaderFile));
            }

            if (!vertShaderFile.Exists)
            {
                throw new FileNotFoundException("Vertex shader missing", vertShaderFile.FullName);
            }

            if (fragShaderFile is null)
            {
                throw new ArgumentNullException(nameof(fragShaderFile));
            }


            if (!fragShaderFile.Exists)
            {
                throw new FileNotFoundException("Vertex shader missing", fragShaderFile.FullName);
            }

            return LoadAsync<VertexT>(vertShaderFile.OpenRead(), fragShaderFile.OpenRead());
        }

        public static Task<Material> LoadAsync<VertexT>(string vertShaderFileName, string fragShaderFileName)
           where VertexT : struct
        {
            if (string.IsNullOrEmpty(vertShaderFileName))
            {
                throw new ArgumentException("Must provide a filename", nameof(vertShaderFileName));
            }

            if (string.IsNullOrEmpty(fragShaderFileName))
            {
                throw new ArgumentException("Must provide a filename", nameof(fragShaderFileName));
            }

            return LoadAsync<VertexT>(new FileInfo(vertShaderFileName), new FileInfo(fragShaderFileName));
        }

        internal static void SetPipeline(CommandList commandList, Material mat)
        {
            if (commandList is null)
            {
                throw new ArgumentNullException(nameof(commandList));
            }

            commandList.SetPipeline(mat?.pipeline);
        }

        private readonly Type vertexType;
        private readonly ShaderDescription vertShaderDesc;
        private readonly ShaderDescription fragShaderDesc;

        private Pipeline pipeline;

        private GraphicsPipelineDescription pipelineDescription;

        private Material(Type vertType, byte[] vertShaderBytes, byte[] fragShaderBytes)
        {
            if (vertType is null)
            {
                throw new ArgumentNullException(nameof(vertType));
            }

            if (vertShaderBytes is null)
            {
                throw new ArgumentNullException(nameof(vertShaderBytes));
            }

            if (fragShaderBytes is null)
            {
                throw new ArgumentNullException(nameof(fragShaderBytes));
            }

            vertexType = vertType;
            var layoutField = vertexType.GetField("Layout", BindingFlags.Public | BindingFlags.Static);
            if (layoutField is null)
            {
                throw new ArgumentException($"Type argument {vertType.Name} does not contain a static Layout field.");
            }

            if (layoutField.FieldType != typeof(VertexLayoutDescription))
            {
                throw new ArgumentException($"Type argument {vertType.Name}'s Layout field is not of type VertexLayoutDescription.");
            }

#if DEBUG
            const bool debug = true;
#else
            const bool debug = false;
#endif

            vertShaderDesc = new ShaderDescription(ShaderStages.Vertex, vertShaderBytes, "main", debug);
            fragShaderDesc = new ShaderDescription(ShaderStages.Fragment, fragShaderBytes, "main", debug);

            pipelineDescription = new GraphicsPipelineDescription
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
            };

            pipelineDescription.ShaderSet.VertexLayouts = new VertexLayoutDescription[] { (VertexLayoutDescription)layoutField.GetValue(null) };
        }

        public void CreatePipeline(ResourceFactory factory, Framebuffer framebuffer)
        {
            if (factory is null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (framebuffer is null)
            {
                throw new ArgumentNullException(nameof(framebuffer));
            }

            pipelineDescription.ShaderSet.Shaders = factory.CreateFromSpirv(vertShaderDesc, fragShaderDesc);
            pipelineDescription.Outputs = framebuffer.OutputDescription;
            pipeline = factory.CreateGraphicsPipeline(pipelineDescription);
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
                pipeline?.Dispose();
                pipeline = null;
            }
        }
    }
}
