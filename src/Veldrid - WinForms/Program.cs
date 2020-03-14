using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Juniper.Imaging;
using Juniper.IO;
using Juniper.VeldridIntegration;

using Veldrid;

namespace Juniper
{
    public static class Program
    {

        private static ShaderProgramDescription<VertexPositionColor> quadMaterial;
        private static ShaderProgram<VertexPositionColor> quadRenderer;
        private static readonly Mesh<VertexPositionColor> quad = new Mesh<VertexPositionColor>(
            new Quad<VertexPositionColor>(
                new VertexPositionColor(new Vector2(-.75f, .75f), RgbaFloat.Red),
                new VertexPositionColor(new Vector2(.75f, .75f), RgbaFloat.Green),
                new VertexPositionColor(new Vector2(-.75f, -.75f), RgbaFloat.Blue),
                new VertexPositionColor(new Vector2(.75f, -.75f), RgbaFloat.Yellow)));

        private static ShaderProgramDescription<VertexPositionTexture> cubeMaterial;
        private static ShaderProgram<VertexPositionTexture> cubeRenderer;
        private static readonly Mesh<VertexPositionTexture> cube = new Mesh<VertexPositionTexture>(
            // Top
            new Quad<VertexPositionTexture>(
                new VertexPositionTexture(new Vector3(-0.5f, +0.5f, -0.5f), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(+0.5f, +0.5f, -0.5f), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(-0.5f, +0.5f, +0.5f), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(+0.5f, +0.5f, +0.5f), new Vector2(1, 1))),
            // Bottom
            new Quad<VertexPositionTexture>(
                new VertexPositionTexture(new Vector3(-0.5f,-0.5f, +0.5f),  new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(+0.5f,-0.5f, +0.5f),  new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(-0.5f,-0.5f, -0.5f),  new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(+0.5f,-0.5f, -0.5f),  new Vector2(1, 1))),
            // Left
            new Quad<VertexPositionTexture>(
                new VertexPositionTexture(new Vector3(-0.5f, +0.5f, -0.5f), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(-0.5f, +0.5f, +0.5f), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(-0.5f, -0.5f, +0.5f), new Vector2(1, 1))),
            // Right
            new Quad<VertexPositionTexture>(
                new VertexPositionTexture(new Vector3(+0.5f, +0.5f, +0.5f), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(+0.5f, +0.5f, -0.5f), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(+0.5f, -0.5f, +0.5f), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(+0.5f, -0.5f, -0.5f), new Vector2(1, 1))),
            // Back
            new Quad<VertexPositionTexture>(
                new VertexPositionTexture(new Vector3(+0.5f, +0.5f, -0.5f), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(-0.5f, +0.5f, -0.5f), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(+0.5f, -0.5f, -0.5f), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector2(1, 1))),
            // Front
            new Quad<VertexPositionTexture>(
                new VertexPositionTexture(new Vector3(-0.5f, +0.5f, +0.5f), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(+0.5f, +0.5f, +0.5f), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(-0.5f, -0.5f, +0.5f), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(+0.5f, -0.5f, +0.5f), new Vector2(1, 1))));

        private static readonly Dictionary<string, ImageData> images = new Dictionary<string, ImageData>();
        private static MainForm mainForm;
        private static Texture surfaceTexture;
        private static DateTime start;

        private static async Task Main()
        {
            var imageDir = new DirectoryInfo("Images");
            if (imageDir.Exists)
            {
                var decoders = new Dictionary<MediaType, IImageCodec<ImageData>>()
                {
                    [MediaType.Image.Png] = new HjgPngcsCodec().Pipe(new HjgPngcsImageDataTranscoder()),
                    [MediaType.Image.Jpeg] = new LibJpegNETCodec().Pipe(new LibJpegNETImageDataTranscoder(padAlpha: true))
                };

                foreach (var file in imageDir.EnumerateFiles())
                {
                    var name = Path.GetFileNameWithoutExtension(file.Name)
                        .ToLowerInvariant();
                    var type = (from t in MediaType.GuessByFile(file)
                                where t is MediaType.Image
                                select t)
                            .FirstOrDefault();
                    if (decoders.ContainsKey(type))
                    {
                        images[name] = decoders[type].Deserialize(file);
                    }
                }
            }

            quadMaterial = await ShaderProgramDescription.LoadAsync<VertexPositionColor>(
                    "Shaders\\color-quad-vert.glsl",
                    "Shaders\\color-quad-frag.glsl")
                .ConfigureAwait(false);

            cubeMaterial = await ShaderProgramDescription.LoadAsync<VertexPositionTexture>(
                    "Shaders\\tex-cube-vert.glsl",
                    "Shaders\\tex-cube-frag.glsl")
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

        private static void Panel_Ready(object sender, EventArgs e)
        {
            var device = mainForm.Device.VeldridDevice;
            var factory = device.ResourceFactory;

            quadRenderer = new ShaderProgram<VertexPositionColor>(
                device,
                mainForm.Panel.VeldridSwapChain.Framebuffer,
                quad,
                quadMaterial);

            var image = images["rock"];
            surfaceTexture = factory.CreateTexture(new TextureDescription(
                (uint)image.Info.Dimensions.Width, (uint)image.Info.Dimensions.Height, 1,
                1, 1,
                PixelFormat.R8_G8_B8_A8_UNorm,
                TextureUsage.Sampled,
                TextureType.Texture2D));

            var imageData = image.GetData();
            device.UpdateTexture(
                surfaceTexture,
                imageData,
                0, 0, 0,
                (uint)image.Info.Dimensions.Width, (uint)image.Info.Dimensions.Height, 1,
                0, 0);

            cubeRenderer = new ShaderProgram<VertexPositionTexture>(
                device,
                mainForm.Panel.VeldridSwapChain.Framebuffer,
                cube,
                cubeMaterial,
                ("SurfaceTexture", surfaceTexture));

            start = DateTime.Now;
        }

        private static void Panel_CommandListUpdate(object sender, VeldridIntegration.WinFormsSupport.UpdateCommandListEventArgs e)
        {
            var commandList = e.CommandList;
            var framebuffer = mainForm.Panel.VeldridSwapChain.Framebuffer;

            var width = framebuffer.Width;
            var height = framebuffer.Height;
            var aspectRatio = (float)width / height;

            var time = (float)(DateTime.Now - start).TotalSeconds;
            var showQuad = (((int)(time / 5)) % 2) == 0;

            commandList.Begin();

            commandList.SetFramebuffer(framebuffer);
            commandList.ClearColorTarget(0, RgbaFloat.Black);
            commandList.ClearDepthStencil(1);

            if (showQuad)
            {
                var size = Math.Min(width, height);

                commandList.SetViewport(0, new Viewport(
                    x: (width - size) / 2,
                    y: (height - size) / 2,
                    width: size,
                    height: size,
                    minDepth: 0,
                    maxDepth: 1));

                quadRenderer.Draw(commandList);
            }
            else
            {
                var projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(Units.Degrees.Radians(60), aspectRatio, 0.5f, 100);
                var viewMatrix = Matrix4x4.CreateLookAt(Vector3.UnitZ * 2.5f, Vector3.Zero, Vector3.UnitY);
                cubeRenderer.UpdateMatrix("ProjectionBuffer", commandList, ref projectionMatrix);
                cubeRenderer.UpdateMatrix("ViewBuffer", commandList, ref viewMatrix);

                var worldMatrix = Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, time)
                    * Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, time / 3);
                cubeRenderer.UpdateMatrix("WorldBuffer", commandList, ref worldMatrix);
                cubeRenderer.Draw(commandList);
            }

            commandList.End();

            mainForm.Panel.Invalidate();
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            mainForm.SetError(e.Exception);
        }

        private static void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            cubeRenderer?.Dispose();
            surfaceTexture?.Dispose();
            quadRenderer?.Dispose();
        }
    }
}
