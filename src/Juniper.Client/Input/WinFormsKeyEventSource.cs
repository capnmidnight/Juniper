using System;
using System.Windows.Forms;

namespace Juniper.Input
{
    public class WinFormsKeyEventSource : AbstractKeyEventSource<Keys>
    {
        private readonly Control control;

        public WinFormsKeyEventSource(Control control)
        {
            this.control = control ?? throw new ArgumentNullException(nameof(control));
        }

        public override void Start()
        {
            base.Start();
            control.KeyDown += Form_KeyDown;
            control.KeyUp += Form_KeyUp;
        }

        public override void Stop()
        {
            control.KeyDown -= Form_KeyDown;
            control.KeyUp -= Form_KeyUp;
            base.Stop();
        }

        private void Form_KeyUp(object sender, KeyEventArgs e)
        {
            KeyState[e.KeyCode] = false;
            UpdateStates();
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            KeyState[e.KeyCode] = true;
            UpdateStates();
        }
    }
}