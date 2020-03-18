using System;
using System.Windows.Forms;

using Juniper.VeldridIntegration.WinFormsSupport;

namespace Juniper
{
    public partial class MainForm : Form
    {
        private readonly Action<Exception> setError;
        public MainForm()
        {
            setError = SetError;
            InitializeComponent();
        }

        public void SetError(Exception exp)
        {
            if (InvokeRequired)
            {
                Invoke(setError, exp);
            }
            else
            {
                errorTextBox1.Text = exp.Unroll();
            }
        }

        public VeldridGraphicsDevice Device => veldridGraphicsDevice1;
        public VeldridPanel Panel => veldridPanel1;
    }
}
