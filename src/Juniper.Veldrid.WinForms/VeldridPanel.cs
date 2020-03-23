using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Veldrid;

namespace Juniper.VeldridIntegration.WinFormsSupport
{
    public class VeldridPanel : UserControl, IVeldridPanel
    {
        public SwapchainSource VeldridSwapchainSource { get; private set; }

        public event EventHandler Ready;
        public event EventHandler Destroying;

        public uint RenderWidth => (uint)Width;
        public uint RenderHeight => (uint)Height;

        public VeldridPanel()
        {
            AutoScaleMode = AutoScaleMode.Font;
            Paint += VeldridPanel_Paint;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var currentModule = typeof(VeldridPanel).Module;
            var hinstance = Marshal.GetHINSTANCE(currentModule);
            VeldridSwapchainSource = SwapchainSource.CreateWin32(Handle, hinstance);
            Ready?.Invoke(this, EventArgs.Empty);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Destroying?.Invoke(this, null);
            }
            base.Dispose(disposing);
        }

        private void VeldridPanel_Paint(object sender, PaintEventArgs e)
        {
            using var g = CreateGraphics();
            g.FillRectangle(SystemBrushes.ControlDark, ClientRectangle);

            var icon = SystemIcons.Application;
            var w = icon.Width * 2;
            var h = icon.Height * 2;
            var middleRect = new Rectangle
            {
                Width = w,
                Height = h,
                X = (ClientRectangle.Width - w) / 2,
                Y = (ClientRectangle.Height - h) / 2
            };
            g.DrawIcon(icon, middleRect);
            Paint -= VeldridPanel_Paint;
        }
    }
}
