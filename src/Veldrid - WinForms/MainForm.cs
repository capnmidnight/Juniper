using System;
using System.Windows.Forms;

using Veldrid;

namespace Juniper
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private bool prepared;
        public void Prepare()
        {
            if (!prepared)
            {
                veldridPanel1.Prepare();
                prepared = true;
            }
        }

        public void SetError(Exception exp)
        {
            errorTextBox1.Text = exp.Unroll();
        }

        public GraphicsDevice Device => veldridGraphicsDevice1.VeldridDevice;

        public CommandList CommandList
        {
            get
            {
                return veldridPanel1.VeldridCommandList;
            }
            set
            {
                veldridPanel1.VeldridCommandList = value;
            }
        }

        public Framebuffer VeldridFramebuffer => veldridPanel1.VeldridSwapChain.Framebuffer;
    }
}
