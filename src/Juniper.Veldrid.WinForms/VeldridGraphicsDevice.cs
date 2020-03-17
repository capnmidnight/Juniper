using System;
using System.ComponentModel;

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
    }
}
