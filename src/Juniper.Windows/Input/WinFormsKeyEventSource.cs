using System.Collections.Generic;
using System.Windows.Forms;

namespace Juniper.Input
{
    public class WinFormsKeyEventSource : AbstractKeyEventSource<Keys>
    {
        private readonly Control control;

        public WinFormsKeyEventSource(Control control)
        {
            this.control = control;
        }

        public override void Start()
        {
            base.Start();
            control.KeyDown += Form_KeyDown;
            control.KeyUp += Form_KeyUp;
        }

        public override void Quit()
        {
            control.KeyDown -= Form_KeyDown;
            control.KeyUp -= Form_KeyUp;
            base.Quit();
        }

        private void Form_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            KeyState[e.KeyData] = false;
            UpdateStates();
        }

        private void Form_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            KeyState[e.KeyData] = true;
            UpdateStates();
        }

        public override bool IsKeyDown(Keys key)
        {
            return KeyState.Get(key);
        }
    }
}