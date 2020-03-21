using System;
using System.Windows.Forms;

using Juniper.VeldridIntegration.WinFormsSupport;

namespace Juniper
{
    public partial class MainForm : Form
    {
        private readonly string baseTitle;
        private readonly Action<Exception> setError;
        private readonly Action<float?, float?, float?> setFPS;

        public event EventHandler RequestStats;

        public MainForm()
        {
            InitializeComponent();

            baseTitle = Text;
            setError = SetError;
            setFPS = SetStats;
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (Visible)
            {
                statsTimer.Start();
            }
            else
            {
                statsTimer.Stop();
            }
        }

        private void statsTimer_Tick(object sender, EventArgs e)
        {
            RequestStats?.Invoke(this, EventArgs.Empty);
        }

        public void SetError(Exception exp)
        {
            if (InvokeRequired)
            {
                _ = Invoke(setError, exp);
            }
            else
            {
                errorTextBox1.Text = exp.Unroll();
            }
        }

        public void SetStats(float? minFPS, float? meanFPS, float? maxFPS)
        {
            if (InvokeRequired)
            {
                _ = Invoke(setFPS, minFPS, meanFPS, maxFPS);
            }
            else if(minFPS.HasValue
                && meanFPS.HasValue
                && maxFPS.HasValue)
            {
                Text = $"{baseTitle} - {minFPS:0/}{meanFPS:0/}{maxFPS:0fps}";
            }
            else
            {
                Text = $"{baseTitle} - N/A fps";
            }
        }

        public VeldridGraphicsDevice Device => veldridGraphicsDevice1;
        public VeldridPanel Panel => veldridPanel1;
    }
}
