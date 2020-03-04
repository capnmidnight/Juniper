using System;
using System.Windows.Forms;

using Juniper.VeldridIntegration.WinFormsSupport;

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
        public VeldridPanel Panel => veldridPanel1;
    }
}
