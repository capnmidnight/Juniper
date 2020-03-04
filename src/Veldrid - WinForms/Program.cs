using System.IO;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Veldrid;
using Veldrid.SPIRV;

namespace Juniper
{
    public static class Program
    {
        private static MainForm mainForm;
        private static DeviceBuffer vertexBuffer;
        private static DeviceBuffer indexBuffer;

        private static readonly VertexPositionColor[] quadVertices =
        {
            new VertexPositionColor(new Vector2(-.75f, .75f), RgbaFloat.Red),
            new VertexPositionColor(new Vector2(.75f, .75f), RgbaFloat.Green),
            new VertexPositionColor(new Vector2(-.75f, -.75f), RgbaFloat.Blue),
            new VertexPositionColor(new Vector2(.75f, -.75f), RgbaFloat.Yellow)
        };

        private static readonly ushort[] quadIndices = { 0, 1, 2, 3 };

        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += Application_ThreadException;
            using var form = mainForm = new MainForm();
            form.Prepare();
            var g = form.Device;
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

            var shaders = factory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);
            using var vertexShader = shaders[0];
            using var fragmentShader = shaders[1];

            var frameBuffer = mainForm.VeldridFramebuffer;
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
                Outputs = frameBuffer.OutputDescription
            };

            using var pipeline = factory.CreateGraphicsPipeline(pipelineDescription);
            using var commandList = factory.CreateCommandList();
            commandList.Begin();
            commandList.SetFramebuffer(frameBuffer);
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
            form.CommandList = commandList;
            Application.Run(form);
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            mainForm.SetError(e.Exception);
        }
    }
}
