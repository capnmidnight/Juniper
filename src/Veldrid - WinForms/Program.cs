using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Juniper.VeldridIntegration;

using Veldrid;

namespace Juniper
{
    public static class Program
    {
        private static MainForm mainForm;
        private static Material<VertexPositionColor> material;
        private static MeshRenderer<VertexPositionColor> renderer;

        private static async Task Main()
        {
            material = await Material.LoadAsync<VertexPositionColor>(
                    "Shaders\\vert.glsl",
                    "Shaders\\frag.glsl")
                .ConfigureAwait(false);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += Application_ThreadException;

            using var form = mainForm = new MainForm();
            mainForm.Panel.Ready += Panel_Ready;
            mainForm.Panel.CommandListUpdate += Panel_CommandListUpdate;
            mainForm.FormClosing += MainForm_FormClosing;
            Application.Run(mainForm);
        }

        private static void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            renderer?.Dispose();
        }

        private static void Panel_Ready(object sender, EventArgs e)
        {
            renderer = new MeshRenderer<VertexPositionColor>(
                mainForm.Device.VeldridDevice,
                mainForm.Panel.VeldridSwapChain.Framebuffer,
                material,
                new Mesh<VertexPositionColor>(
                    new Quad<VertexPositionColor>[]{

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
                    }));
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
            commandList.Render(renderer);
            commandList.End();
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            mainForm.SetError(e.Exception);
        }
    }
}
