using System;
using System.ComponentModel;

using Veldrid;

namespace Juniper.VeldridIntegration.WinFormsSupport
{
    public partial class VeldridGraphicsDevice : Component
    {
        public VeldridGraphicsDevice()
        {
            InitializeComponent();
        }

        public VeldridGraphicsDevice(IContainer container)
        {
            if (container is null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.Add(this);

            InitializeComponent();
        }

        private bool prepared;
        public void Prepare()
        {
            if (!prepared)
            {
                VeldridDevice = VeldridBackend switch
                {
                    GraphicsBackend.Direct3D11 => GraphicsDevice.CreateD3D11(veldridDeviceOptions),
                    GraphicsBackend.Metal => GraphicsDevice.CreateMetal(veldridDeviceOptions),
                    //GraphicsBackend.OpenGL => GraphicsDevice.CreateOpenGL(veldridDeviceOptions),
                    //GraphicsBackend.OpenGLES => GraphicsDevice.CreateOpenGLES(veldridDeviceOptions),
                    GraphicsBackend.Vulkan => GraphicsDevice.CreateVulkan(veldridDeviceOptions),
                    _ => null
                };

                if (VeldridDevice is null)
                {
                    throw new InvalidOperationException($"Can't create a device for GraphicsBackend value: {VeldridBackend}");
                }

                prepared = true;
            }
        }

        internal void Draw(CommandList veldridCommandList, Swapchain veldridSwapChain)
        {
            VeldridDevice.SubmitCommands(veldridCommandList);
            VeldridDevice.SwapBuffers(veldridSwapChain);
        }
    }
}
