using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

using Juniper.Mathematics;
using Juniper.VeldridIntegration;

using Veldrid;

namespace Juniper
{
    public sealed class VeldridDemoProgram : IDisposable
    {
        private const float MOVE_SPEED = 1.5f;
        private static readonly float MIN_PITCH = Units.Degrees.Radians(-80);
        private static readonly float MAX_PITCH = Units.Degrees.Radians(80);

        private readonly bool ownDevice;
        private readonly GraphicsDevice device;
        private readonly Swapchain swapchain;
        private readonly CancellationToken canceller;
        private readonly SingleStatisticsCollection updateStats = new SingleStatisticsCollection(10);
        private readonly SingleStatisticsCollection renderStats = new SingleStatisticsCollection(10);
        private readonly CommandList commandList;
        private readonly List<WorldObj<VertexPositionTexture>> cubes = new List<WorldObj<VertexPositionTexture>>();

        private ShaderProgram<VertexPositionTexture> program;
        private Camera camera;
        private Vector3 velocity;
        private float yaw;
        private float pitch;
        private Thread updateThread;
        private Thread renderThread;
        private Semaphore rendering;

        private float AspectRatio => (float)swapchain.Framebuffer.Width / swapchain.Framebuffer.Height;

        public event Action<Exception> Error;
        public event Action<float> Update;

        public VeldridDemoProgram(GraphicsDevice device, CancellationToken token)
        {
            if (device is null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            ownDevice = false;
            this.device = device;
            swapchain = device.MainSwapchain;
            commandList = device.ResourceFactory.CreateCommandList();
            canceller = token;
        }

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

            ownDevice = true;

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

            swapchain = device.ResourceFactory.CreateSwapchain(swapchainDescription);
            commandList = device.ResourceFactory.CreateCommandList();

            canceller = token;
        }

        public float? MinUpdatesPerSecond => updateStats.Minimum;
        public float? MeanUpdatesPerSecond => updateStats.Mean;
        public float? StdDevUpdatesPerSecond => updateStats.StandardDeviation;
        public float? MaxUpdatesPerSecond => updateStats.Maximum;

        public float? MinFramesPerSecond => renderStats.Minimum;
        public float? MeanFramesPerSecond => renderStats.Mean;
        public float? StdDevFramesPerSecond => renderStats.StandardDeviation;
        public float? MaxFramesPerSecond => renderStats.Maximum;

        public void Quit()
        {
            if (rendering.WaitOne())
            {
                updateThread.Join();
                renderThread.Join();
            }
        }

        public async Task StartAsync(string vertexShaderFileName, string fragmentShaderFileName, string modelFileName)
        {
            if (string.IsNullOrEmpty(vertexShaderFileName))
            {
                throw new ArgumentException("Must provide a vertex shader file name.", nameof(vertexShaderFileName));
            }

            if (string.IsNullOrEmpty(fragmentShaderFileName))
            {
                throw new ArgumentException("Must provide a fragment shader file name.", nameof(fragmentShaderFileName));
            }

            if (string.IsNullOrEmpty(modelFileName))
            {
                throw new ArgumentException("Must provide a model file name.", nameof(modelFileName));
            }

            var cubeProgramDescription = await ShaderProgramDescription.LoadAsync<VertexPositionTexture>(
                vertexShaderFileName,
                fragmentShaderFileName)
                .ConfigureAwait(true);

            Start(cubeProgramDescription, modelFileName);
        }

        private void Start(ShaderProgramDescription<VertexPositionTexture> cubeProgramDescription, string modelFileName)
        {
            program = new ShaderProgram<VertexPositionTexture>(cubeProgramDescription, Mesh.ConvertVeldridMesh);
            program.LoadOBJ(modelFileName);
            program.Begin(device, swapchain.Framebuffer, "ProjectionBuffer", "WorldBuffer");

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
            updateThread = new Thread(UpdateThread);
            renderThread = new Thread(DrawThread);
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
                    _ = rendering.Release();
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

        public void SetMouseRotate(int dx, int dy)
        {
            if (camera is object)
            {
                yaw -= Units.Degrees.Radians(dx);
                pitch -= Units.Degrees.Radians(dy);
                if (pitch < MIN_PITCH)
                {
                    pitch = MIN_PITCH;
                }
                else if (pitch > MAX_PITCH)
                {
                    pitch = MAX_PITCH;
                }
                camera.Rotation = Quaternion.CreateFromYawPitchRoll(yaw, pitch, 0);
            }
        }

        private void UpdateThread()
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

                    updateStats.Add(1 / dtime);
                    Update?.Invoke(dtime);

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


        private void DrawThread()
        {
            try
            {
                var last = DateTime.Now;
                while (!canceller.IsCancellationRequested)
                {
                    if (rendering.WaitOne())
                    {
                        var dtime = (float)(DateTime.Now - last).TotalSeconds;
                        last = DateTime.Now;

                        try
                        {
                            renderStats.Add(1 / dtime);
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
                            _ = rendering.Release();
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

                if (ownDevice)
                {
                    swapchain?.Dispose();
                    device?.Dispose();
                }
            }
        }
    }
}
