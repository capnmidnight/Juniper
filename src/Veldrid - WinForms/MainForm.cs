using System;
using System.Windows.Forms;

using Juniper.VeldridIntegration.WinFormsSupport;

namespace Juniper
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        public void SetError(Exception exp)
        {
            errorTextBox1.Text = exp.Unroll();
        }

        public VeldridGraphicsDevice Device => veldridGraphicsDevice1;
        public VeldridPanel Panel => veldridPanel1;
    }
}
