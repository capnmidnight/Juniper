using System;
using System.Threading;
using System.Windows;

namespace Juniper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        private readonly SynchronizationContext sync;
        private readonly string baseTitle;
        private readonly Timer timer;

        public event EventHandler RequestStats;

        public MainWindow()
        {
            sync = SynchronizationContext.Current;
            timer = new Timer(new TimerCallback(Tick), null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(0.25));
            InitializeComponent();

            baseTitle = Title;
        }

        private void Tick(object source)
        {
            RequestStats?.Invoke(this, EventArgs.Empty);
        }

        public void SetError(Exception exp)
        {
            sync.Post(_ => textBlock.Text = exp.Unroll(), null);
        }

        public void SetStats(float? minFPS, float? meanFPS, float? maxFPS)
        {
            sync.Post(_ =>
            {
                if (minFPS.HasValue
                && meanFPS.HasValue
                && maxFPS.HasValue)
                {
                    Title = $"{baseTitle} - {minFPS:0/}{meanFPS:0/}{maxFPS:0fps}";
                }
                else
                {
                    Title = $"{baseTitle} - N/A fps";
                }
            }, null);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                timer.Dispose();
            }
        }
    }
}
