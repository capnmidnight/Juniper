using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Juniper.VeldridIntegration;

using Veldrid;

namespace Juniper
{
    public sealed class VeldridDemoProgram : IDisposable
    {
        private const float MOVE_SPEED = 1.5f;

        private readonly List<WorldObj<VertexPositionTexture>> cubes = new List<WorldObj<VertexPositionTexture>>();
        private readonly GraphicsBackend backend;
        private readonly GraphicsDeviceOptions options;
        private readonly SwapchainSource swapchainSource;
        private readonly uint startWidth;
        private readonly uint startHeight;
        private readonly CancellationToken canceller;
        private GraphicsDevice device;
        private Swapchain swapchain;
        private CommandList commandList;
        private ShaderProgram<VertexPositionTexture> program;
        private Camera camera;
        private Vector3 velocity;
        private Thread updateThread;
        private Thread renderThread;
        private Semaphore rendering;

        private float AspectRatio => (float)swapchain.Framebuffer.Width / swapchain.Framebuffer.Height;

        public event Action<Exception> Error;

        public VeldridDemoProgram(GraphicsBackend backend, GraphicsDeviceOptions options, SwapchainSource swapchainSource, uint startWidth, uint startHeight, CancellationToken token)
        {
            if (!GraphicsDevice.IsBackendSupported(backend))
            {
                throw new NotSupportedException($"Graphics backend {backend} is not supported on this system.");
            }

            if (swapchainSource is null)
            {
                throw new ArgumentNullException(nameof(swapchainSource));
            }

            this.backend = backend;
            this.options = options;
            this.swapchainSource = swapchainSource;
            this.startWidth = startWidth;
            this.startHeight = startHeight;
            canceller = token;
        }

        public async Task StartAsync(string vertexShaderFileName, string fragmentShaderFileName)
        {
            if (string.IsNullOrEmpty(vertexShaderFileName))
            {
                throw new ArgumentException("Must provide a vertex shader file name.", nameof(vertexShaderFileName));
            }

            if (string.IsNullOrEmpty(fragmentShaderFileName))
            {
                throw new ArgumentException("Must provide a fragment shader file name.", nameof(fragmentShaderFileName));
            }

            var cubeProgramDescription = await ShaderProgramDescription.LoadAsync<VertexPositionTexture>(
                vertexShaderFileName,
                fragmentShaderFileName)
                .ConfigureAwait(true);

            Start(cubeProgramDescription);
        }

        private void Start(ShaderProgramDescription<VertexPositionTexture> cubeProgramDescription)
        {
            device = backend switch
            {
                GraphicsBackend.Direct3D11 => GraphicsDevice.CreateD3D11(options),
                GraphicsBackend.Metal => GraphicsDevice.CreateMetal(options),
                GraphicsBackend.Vulkan => GraphicsDevice.CreateVulkan(options),
                _ => null
            };

            if (device is null)
            {
                throw new InvalidOperationException($"Can't create a device for GraphicsBackend value: {backend}");
            }


            var swapchainDescription = new SwapchainDescription
            {
                Source = swapchainSource,
                Width = startWidth,
                Height = startHeight,
                DepthFormat = options.SwapchainDepthFormat,
                SyncToVerticalBlank = options.SyncToVerticalBlank,
                ColorSrgb = options.SwapchainSrgbFormat
            };

            var resourceFactory = device.ResourceFactory;
            swapchain = resourceFactory.CreateSwapchain(swapchainDescription);
            commandList = device.ResourceFactory.CreateCommandList();

            program = new ShaderProgram<VertexPositionTexture>(cubeProgramDescription, Mesh.ConvertVeldridMesh);
            program.LoadOBJ("Models/cube.obj");
            program.Begin(device, swapchain.Framebuffer, "ProjectionBuffer", "ViewBuffer", "WorldBuffer");

            for (var i = 0; i < 3; ++i)
            {
                var cube = program.CreateObject();
                cube.Position = 1.25f * (i - 1) * Vector3.UnitX;
                cubes.Add(cube);
            }

            camera = new Camera
            {
                AspectRatio = AspectRatio,
                Position = 2.5f * Vector3.UnitZ
            };

            GC.Collect();

            rendering = new Semaphore(1, 1);
            updateThread = new Thread(Update);
            renderThread = new Thread(Draw);
            renderThread.Start();
            updateThread.Start();
        }

        public void Resize(uint width, uint height)
        {
            if (swapchain is object
                && camera is object
                && rendering.WaitOne())
            {
                try
                {
                    swapchain.Resize(width, height);
                    camera.AspectRatio = AspectRatio;
                }
                finally
                {
                    rendering.Release();
                }
            }
        }

        public void SetVelocity(float dx, float dz)
        {
            var moving = dx != 0 || dz != 0;
            if (moving)
            {
                velocity = MOVE_SPEED * Vector3.Transform(Vector3.Normalize(new Vector3(dx, 0, dz)), camera.Rotation);
            }
            else
            {
                velocity = Vector3.Zero;
            }

        }

        public void SetMouseRotate(Vector2 delta)
        {
            var dRot = Quaternion.CreateFromYawPitchRoll(
                                Units.Degrees.Radians(delta.X),
                                Units.Degrees.Radians(delta.Y),
                                0);

            camera.Rotation *= dRot;
        }

        private void Update()
        {
            var start = DateTime.Now;
            var last = start;
            try
            {
                while (!canceller.IsCancellationRequested)
                {
                    var time = (float)(DateTime.Now - start).TotalSeconds;
                    var dtime = (float)(DateTime.Now - last).TotalSeconds;
                    last = DateTime.Now;

                    camera.Position += velocity * dtime;

                    for (var i = 0; i < cubes.Count; ++i)
                    {
                        cubes[i].Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, (time * (i - 1)));
                    }
                }
            }
            catch (Exception exp)
            {
                Error?.Invoke(exp);
            }
        }


        private void Draw()
        {
            try
            {
                while (!canceller.IsCancellationRequested)
                {
                    if (rendering.WaitOne())
                    {
                        try
                        {
                            commandList.Begin();
                            commandList.SetFramebuffer(swapchain.Framebuffer);

                            camera.Clear(commandList);

                            program.Activate(commandList, camera);

                            for (var i = 0; i < cubes.Count; ++i)
                            {
                                cubes[i].Draw(commandList);
                            }

                            commandList.End();
                            device.SubmitCommands(commandList);
                            device.SwapBuffers(swapchain);
                            device.WaitForIdle();
                        }
                        finally
                        {
                            rendering.Release();
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Error?.Invoke(exp);
            }
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
                renderThread?.Join();
                updateThread?.Join();
                rendering?.Dispose();
                program?.Dispose();
                commandList?.Dispose();
                swapchain?.Dispose();
                device?.Dispose();
            }
        }
    }
}
