using System;
using System.IO;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Juniper.VeldridIntegration;

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

        private static readonly Quad<VertexPositionColor>[] quads = new Quad<VertexPositionColor>[]{

            new Quad<VertexPositionColor>(
                new VertexPositionColor(new Vector3(-1.5f, 0.5f, 0), RgbaFloat.Cyan),
                new VertexPositionColor(new Vector3(-0.5f, 0.5f, 0), RgbaFloat.Red),
                new VertexPositionColor(new Vector3(-1.5f, -0.5f, 0), RgbaFloat.Black),
                new VertexPositionColor(new Vector3(-0.5f, -0.5f, 0), RgbaFloat.Blue)),

            new Quad<VertexPositionColor>(
                new VertexPositionColor(new Vector3(-0.5f, 0.5f, 0), RgbaFloat.Red),
                new VertexPositionColor(new Vector3(0.5f, 0.5f, 0), RgbaFloat.Green),
                new VertexPositionColor(new Vector3(-0.5f, -0.5f, 0), RgbaFloat.Blue),
                new VertexPositionColor(new Vector3(0.5f, -0.5f, 0), RgbaFloat.Yellow)),

            new Quad<VertexPositionColor>(
                new VertexPositionColor(new Vector3(0.5f, 0.5f, 0), RgbaFloat.Green),
                new VertexPositionColor(new Vector3(1.5f, 0.5f, 0), RgbaFloat.CornflowerBlue),
                new VertexPositionColor(new Vector3(0.5f, -0.5f, 0), RgbaFloat.Yellow),
                new VertexPositionColor(new Vector3(1.5f, -0.5f, 0), RgbaFloat.DarkRed))
        };

        private static readonly (VertexPositionColor[] verts, ushort[] indices) unpackedQuads = quads.ToVertsShort();
        private static readonly VertexPositionColor[] quadVertices = unpackedQuads.verts;
        private static readonly ushort[] quadIndices = unpackedQuads.indices;


        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += Application_ThreadException;

            using var form = mainForm = new MainForm();
            mainForm.Panel.Ready += Panel_Ready;
            mainForm.Panel.CommandListUpdate += Panel_CommandListUpdate;

            Application.Run(mainForm);
            pipeline?.Dispose();
        }

        private static void Panel_Ready(object sender, EventArgs e)
        {
            var g = mainForm.Device.VeldridDevice;
            var factory = g.ResourceFactory;

            vertexBuffer = factory.CreateBuffer(new BufferDescription(
                (uint)(quadVertices.Length * VertexPositionColor.SizeInBytes),
                BufferUsage.VertexBuffer));
            g.UpdateBuffer(vertexBuffer, 0, quadVertices);

            indexBuffer = factory.CreateBuffer(new BufferDescription(
                (uint)(quadIndices.Length * sizeof(ushort)),
                BufferUsage.IndexBuffer));
            g.UpdateBuffer(indexBuffer, 0, quadIndices);

            var material = Material
                .LoadCachedAsync<VertexPositionColor>(factory, "Shaders\\vert.glsl", "Shaders\\frag.glsl")
                .Result;

            pipeline = factory.CreateGraphicsPipeline(new GraphicsPipelineDescription
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
                ResourceLayouts = Array.Empty<ResourceLayout>(),
                ShaderSet = material,
                Outputs = mainForm.Panel.VeldridSwapChain.Framebuffer.OutputDescription
            });
        }

        private static void Panel_CommandListUpdate(object sender, VeldridIntegration.WinFormsSupport.UpdateCommandListEventArgs e)
        {
            var commandList = e.CommandList;
            var framebuffer = mainForm.Panel.VeldridSwapChain.Framebuffer;
            var width = framebuffer.Width;
            var height = framebuffer.Height;
            var size = Math.Min(width, height);
            var x = (width - size) / 2;
            var y = (height - size) / 2;
            width = height = size;

            commandList.Begin();

            commandList.SetFramebuffer(framebuffer);
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
            commandList.SetPipeline(pipeline);
            commandList.SetVertexBuffer(0, vertexBuffer);
            commandList.SetIndexBuffer(indexBuffer, IndexFormat.UInt16);
            commandList.DrawIndexed(
                indexCount: (uint)quadIndices.Length,
                instanceCount: (uint)quadIndices.Length / 4,
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
