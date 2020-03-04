using System;
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
        private static Pipeline pipeline;
        private static CommandList commandList;

        private static readonly VertexPositionColor[] quadVertices =
        {
            new VertexPositionColor(new Vector2(-.75f, .75f), RgbaFloat.Red),
            new VertexPositionColor(new Vector2(.75f, .75f), RgbaFloat.Green),
            new VertexPositionColor(new Vector2(-.75f, -.75f), RgbaFloat.Blue),
            new VertexPositionColor(new Vector2(.75f, -.75f), RgbaFloat.Yellow)
        };

        private static readonly ushort[] quadIndices = { 0, 1, 2, 3 };

        private static ShaderDescription ReadShader(ShaderStages stage, string name)
        {
            var code = File.ReadAllText(Path.Combine("Shaders", $"{name}.glsl"));
            return new ShaderDescription(
                stage,
                Encoding.UTF8.GetBytes(code),
                "main");
        }

        private static Shader[] LoadShaders(ResourceFactory factory, string vertName, string fragName)
        {
            var vertexShaderDesc = ReadShader(ShaderStages.Vertex, vertName);
            var fragmentShaderDesc = ReadShader(ShaderStages.Fragment, fragName);

            return factory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);
        }

        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += Application_ThreadException;

            using var form = mainForm = new MainForm();
            mainForm.Resize += Form_Resize;
            mainForm.FormClosing += MainForm_FormClosing;
            mainForm.Prepare();

            var g = mainForm.Device;
            var factory = g.ResourceFactory;

            var vertexLayout = new VertexLayoutDescription(
                new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
                new VertexElementDescription("Color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float4));

            var shaderSet = new ShaderSetDescription(
                    new VertexLayoutDescription[] { vertexLayout },
                    LoadShaders(factory, "vert", "frag"));

            vertexBuffer = factory.CreateBuffer(new BufferDescription(4 * VertexPositionColor.SizeInBytes, BufferUsage.VertexBuffer));
            g.UpdateBuffer(vertexBuffer, 0, quadVertices);

            indexBuffer = factory.CreateBuffer(new BufferDescription(4 * sizeof(ushort), BufferUsage.IndexBuffer));
            g.UpdateBuffer(indexBuffer, 0, quadIndices);

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
                ShaderSet = shaderSet,
                Outputs = mainForm.VeldridFramebuffer.OutputDescription
            };

            using var p = pipeline = factory.CreateGraphicsPipeline(pipelineDescription);

            commandList = mainForm.Device.ResourceFactory.CreateCommandList();
            CreateCommandList();
            mainForm.CommandList = commandList;

            Application.Run(mainForm);

        }

        private static void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            commandList?.Dispose();
        }

        private static void Form_Resize(object sender, System.EventArgs e)
        {
            CreateCommandList();
        }

        private static void CreateCommandList()
        {
            var width = mainForm.VeldridFramebuffer.Width;
            var height = mainForm.VeldridFramebuffer.Height;
            var size = Math.Min(width, height);
            var x = (width - size) / 2;
            var y = (height - size) / 2;
            width = height = size;

            commandList.Begin();
            commandList.SetFramebuffer(mainForm.VeldridFramebuffer);
            commandList.SetViewport(0, new Viewport
            {
                X = x,
                Y = y,
                Width = width,
                Height = height,
                MinDepth = 0,
                MaxDepth = 1
            });
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
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            mainForm.SetError(e.Exception);
        }
    }
}
