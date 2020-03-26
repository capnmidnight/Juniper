using System;
using System.Numerics;
using System.Threading;

using Juniper.IO;

using Veldrid;

namespace Juniper.VeldridIntegration
{
    public abstract class VeldridProgram<VertexT> : IDisposable
        where VertexT : struct
    {
        private readonly bool ownDevice;
        private readonly CancellationToken canceller;
        private readonly CommandList commandList;

        protected ShaderProgram<VertexT> Program { get; private set; }
        protected Camera Camera { get; private set; }

        private Thread updateThread;
        private Thread renderThread;
        private Semaphore rendering;

        protected GraphicsDevice Device { get; }
        private Swapchain Swapchain => Device.MainSwapchain;
        private Framebuffer Framebuffer => Swapchain.Framebuffer;
        private float AspectRatio => (float)Framebuffer.Width / Framebuffer.Height;

        protected IDataSource DataSource { get; }

        public event Action<Exception> Error;
        public event Action<float> Update;

        private static GraphicsBackend PrefferedBackend
        {
            get
            {
                if (GraphicsDevice.IsBackendSupported(GraphicsBackend.Vulkan))
                {
                    return GraphicsBackend.Vulkan;
                }
                if (GraphicsDevice.IsBackendSupported(GraphicsBackend.Direct3D11))
                {
                    return GraphicsBackend.Direct3D11;
                }
                if (GraphicsDevice.IsBackendSupported(GraphicsBackend.Metal))
                {
                    return GraphicsBackend.Metal;
                }
                if (GraphicsDevice.IsBackendSupported(GraphicsBackend.OpenGLES))
                {
                    return GraphicsBackend.OpenGLES;
                }
                if (GraphicsDevice.IsBackendSupported(GraphicsBackend.OpenGL))
                {
                    return GraphicsBackend.OpenGL;
                }

                throw new PlatformNotSupportedException("Couldn't find a supported graphics backend");
            }
        }

        private static GraphicsDevice Init(GraphicsBackend backend, GraphicsDeviceOptions options, SwapchainSource swapchainSource, uint startWidth, uint startHeight)
        {
            if (!GraphicsDevice.IsBackendSupported(backend)
                || backend == GraphicsBackend.OpenGL)
            {
                throw new NotSupportedException($"Graphics backend {backend} is not supported on this system.");
            }

            if (swapchainSource is null)
            {
                throw new ArgumentNullException(nameof(swapchainSource));
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

            var device = backend switch
            {
                GraphicsBackend.Direct3D11 => GraphicsDevice.CreateD3D11(options, swapchainDescription),
                GraphicsBackend.Metal => GraphicsDevice.CreateMetal(options, swapchainDescription),
                GraphicsBackend.Vulkan => GraphicsDevice.CreateVulkan(options, swapchainDescription),
                GraphicsBackend.OpenGLES => GraphicsDevice.CreateOpenGLES(options, swapchainDescription),
                _ => throw new InvalidOperationException($"Can't create a device for GraphicsBackend value: {backend}")
            };

            return device;
        }

        protected VeldridProgram(GraphicsDevice device, IDataSource dataSource, CancellationToken token)
        {
            if (device is null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            if (dataSource is null)
            {
                throw new ArgumentNullException(nameof(dataSource));
            }

            Device = device;
            DataSource = dataSource;
            commandList = device.ResourceFactory.CreateCommandList();
            canceller = token;
            ownDevice = false;

            Camera = new Camera
            {
                AspectRatio = AspectRatio,
                Position = 2.5f * Vector3.UnitZ
            };
        }

        protected VeldridProgram(GraphicsDeviceOptions options, IVeldridPanel panel, IDataSource dataSource, CancellationToken token)
            : this(PrefferedBackend, options, panel, dataSource, token)
        { }

        protected VeldridProgram(GraphicsBackend backend, GraphicsDeviceOptions options, IVeldridPanel panel, IDataSource dataSource, CancellationToken token)
        {
            if (panel is null)
            {
                throw new ArgumentNullException(nameof(panel));
            }

            if (dataSource is null)
            {
                throw new ArgumentNullException(nameof(dataSource));
            }

            DataSource = dataSource;

            Device = Init(
                backend,
                options,
                panel.VeldridSwapchainSource,
                panel.RenderWidth, panel.RenderHeight);

            commandList = Device.ResourceFactory.CreateCommandList();
            canceller = token;
            ownDevice = true;

            Camera = new Camera
            {
                AspectRatio = AspectRatio,
                Position = 2.5f * Vector3.UnitZ
            };

            panel.Resize += (o, e) => Resize(panel.RenderWidth, panel.RenderHeight);
        }

        protected abstract ShaderProgramDescription<VertexT> CreateProgram();

        protected abstract void OnProgramCreated();

        public void Start()
        {
            var programDescription = CreateProgram();

            var dataSource = new AssemblyResourceDataSource(GetType().Assembly);
            Program = new ShaderProgram<VertexT>(dataSource, programDescription);

            OnProgramCreated();

            Program.Begin(Device, Framebuffer);

            GC.Collect();

            rendering = new Semaphore(1, 1);
            updateThread = new Thread(UpdateThread);
            renderThread = new Thread(DrawThread);
            renderThread.Start();
            updateThread.Start();
        }

        public void Resize(uint width, uint height)
        {
            if (Swapchain is object
                && Camera is object
                && rendering.WaitOne())
            {
                try
                {
                    Swapchain.Resize(width, height);
                    Camera.AspectRatio = AspectRatio;
                }
                finally
                {
                    _ = rendering.Release();
                }
            }
        }

        protected abstract void UpdateState(float dtime);

        private void UpdateThread()
        {
            var start = DateTime.Now;
            var last = start;
            try
            {
                while (!canceller.IsCancellationRequested)
                {
                    var dtime = (float)(DateTime.Now - last).TotalSeconds;
                    last = DateTime.Now;
                    Update?.Invoke(dtime);
                    UpdateState(dtime);
                }
            }
            catch (Exception exp)
            {
                Error?.Invoke(exp);
            }
        }

        protected abstract void Draw(CommandList commandList, float dtime);

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
                            commandList.Begin();
                            commandList.SetFramebuffer(Framebuffer);

                            Camera.Clear(commandList);

                            Program.Activate(commandList, Camera);

                            Draw(commandList, dtime);

                            commandList.End();
                            Device.SubmitCommands(commandList);
                            Device.SwapBuffers(Swapchain);
                            Device.WaitForIdle();
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

        public void Quit()
        {
            if (rendering.WaitOne())
            {
                updateThread.Join();
                renderThread.Join();
            }
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
                renderThread?.Join();
                updateThread?.Join();
                rendering?.Dispose();
                Program?.Dispose();
                commandList?.Dispose();

                if (ownDevice)
                {
                    Device?.Dispose();
                }
            }
        }
    }
}