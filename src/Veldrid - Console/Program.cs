using System.IO;
using System.Numerics;
using System.Text;

using Veldrid;
using Veldrid.SPIRV;
using Veldrid.StartupUtilities;

namespace Juniper
{
    public static class Program
    {
        private static DeviceBuffer vertexBuffer;
        private static DeviceBuffer indexBuffer;
        private static Shader[] shaders;

        private static readonly VertexPositionColor[] quadVertices =
        {
            new VertexPositionColor(new Vector2(-.75f, .75f), RgbaFloat.Red),
            new VertexPositionColor(new Vector2(.75f, .75f), RgbaFloat.Green),
            new VertexPositionColor(new Vector2(-.75f, -.75f), RgbaFloat.Blue),
            new VertexPositionColor(new Vector2(.75f, -.75f), RgbaFloat.Yellow)
        };

        private static readonly ushort[] quadIndices = { 0, 1, 2, 3 };

        public static void Main(string[] args)
        {
            var windowCI = new WindowCreateInfo()
            {
                X = 100,
                Y = 100,
                WindowWidth = 960,
                WindowHeight = 540,
                WindowTitle = "Veldrid Tutorial"
            };

            var window = VeldridStartup.CreateWindow(ref windowCI);

            using var g = VeldridStartup.CreateGraphicsDevice(window);
            
            var factory = g.ResourceFactory;

            vertexBuffer = factory.CreateBuffer(new BufferDescription(4 * VertexPositionColor.SizeInBytes, BufferUsage.VertexBuffer));
            indexBuffer = factory.CreateBuffer(new BufferDescription(4 * sizeof(ushort), BufferUsage.IndexBuffer));
            g.UpdateBuffer(vertexBuffer, 0, quadVertices);
            g.UpdateBuffer(indexBuffer, 0, quadIndices);

            var vertexLayout = new VertexLayoutDescription(
                new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
                new VertexElementDescription("Color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4));

            var vertexCode = File.ReadAllText(Path.Combine("Shaders", "vert.glsl"));
            var fragmentCode = File.ReadAllText(Path.Combine("Shaders", "frag.glsl"));

            var vertexShaderDesc = new ShaderDescription(
                ShaderStages.Vertex,
                Encoding.UTF8.GetBytes(vertexCode),
                "main");

            var fragmentShaderDesc = new ShaderDescription(
                ShaderStages.Fragment,
                Encoding.UTF8.GetBytes(fragmentCode),
                "main");

            shaders = factory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);

            using var vertexShader = shaders[0];
            using var fragmentShader = shaders[1];

            var pipelineDescription = new GraphicsPipelineDescription
            {
                BlendState = BlendStateDescription.SingleOverrideBlend,
                DepthStencilState = new DepthStencilStateDescription(
                    depthTestEnabled: true,
                    depthWriteEnabled: true,
                    comparisonKind: ComparisonKind.LessEqual),
                RasterizerState = new RasterizerStateDescription(
                    cullMode: FaceCullMode.Back,
                    fillMode: PolygonFillMode.Solid,
                    frontFace: FrontFace.Clockwise,
                    depthClipEnabled: true,
                    scissorTestEnabled: false),
                PrimitiveTopology = PrimitiveTopology.TriangleStrip,
                ResourceLayouts = System.Array.Empty<ResourceLayout>(),
                ShaderSet = new ShaderSetDescription(
                    new VertexLayoutDescription[] { vertexLayout },
                    new Shader[] { vertexShader, fragmentShader }),
                Outputs = g.SwapchainFramebuffer.OutputDescription
            };

            using var pipeline = factory.CreateGraphicsPipeline(pipelineDescription);
            using var commandList = factory.CreateCommandList();

            commandList.Begin();
            commandList.SetFramebuffer(g.SwapchainFramebuffer);
            commandList.ClearColorTarget(0, RgbaFloat.Black);
            commandList.SetVertexBuffer(0, vertexBuffer);
            commandList.SetIndexBuffer(indexBuffer, IndexFormat.UInt16);
            commandList.SetPipeline(pipeline);
            commandList.DrawIndexed(
                indexCount: 4,
                instanceCount: 1,
                indexStart: 0,
                vertexOffset: 0,
                instanceStart: 0);
            commandList.End();

            while (window.Exists)
            {
                _ = window.PumpEvents();
                g.SubmitCommands(commandList);
                g.SwapBuffers();
            }
        }
    }
}
