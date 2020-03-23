using System;
using System.Threading;
using System.Windows.Forms;

using Juniper.VeldridIntegration.WinFormsSupport;

namespace Juniper
{
    public partial class MainWindow : Form
    {
        private readonly SynchronizationContext sync;
        private readonly string baseTitle;

        public event EventHandler RequestStats;

        public MainWindow()
        {
            sync = SynchronizationContext.Current;
            InitializeComponent();

            baseTitle = Text;
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

        private void StatsTimer_Tick(object sender, EventArgs e)
        {
            RequestStats?.Invoke(this, EventArgs.Empty);
        }

        public void SetError(Exception exp)
        {
            sync.Post(_ => errorTextBox1.Text = exp.Unroll(), null);
        }

        public void SetStats(float? minFPS, float? meanFPS, float? maxFPS)
        {
            sync.Post(_ =>
            {
                if (minFPS.HasValue
                && meanFPS.HasValue
                && maxFPS.HasValue)
                {
                    Text = $"{baseTitle} - {minFPS:0/}{meanFPS:0/}{maxFPS:0fps}";
                }
                else
                {
                    Text = $"{baseTitle} - N/A fps";
                }
            }, null);
        }

        public VeldridPanel Panel => veldridPanel1;
    }
}
