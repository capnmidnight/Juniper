using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Veldrid;

namespace Juniper.VeldridIntegration.WinFormsSupport
{
    public partial class VeldridPanel : UserControl
    {
        public SwapchainSource VeldridSwapchainSource { get; }

        public VeldridPanel()
        {
            InitializeComponent();

            Paint += VeldridPanel_Paint;

            var currentModule = typeof(VeldridPanel).Module;
            var hinstance = Marshal.GetHINSTANCE(currentModule);
            VeldridSwapchainSource = SwapchainSource.CreateWin32(Handle, hinstance);
        }

        private void VeldridPanel_Paint(object sender, PaintEventArgs e)
        {
            using var g = CreateGraphics();
            g.FillRectangle(SystemBrushes.ControlDark, ClientRectangle);

            var icon = SystemIcons.Application;
            var middleRect = ClientRectangle;
            middleRect.X = (middleRect.Width - icon.Width * 2) / 2;
            middleRect.Y = (middleRect.Height - icon.Height * 2) / 2;
            middleRect.Width = icon.Width * 2;
            middleRect.Height = icon.Height * 2;
            g.DrawIcon(icon, middleRect);
        }

        public void StopOwnRender()
        {
            Paint -= VeldridPanel_Paint;
        }
    }
}
