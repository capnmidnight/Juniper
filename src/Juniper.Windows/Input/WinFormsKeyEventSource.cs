using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Juniper.Input
{
    public class WinFormsKeyEventSource : AbstractKeyEventSource<Keys>
    {
        private readonly WeakReference<Control> controlRef;

        public WinFormsKeyEventSource(Control control)
        {
            controlRef = new WeakReference<Control>(control ?? throw new ArgumentNullException(nameof(control)));
        }

        public override void Start()
        {
            if (controlRef.TryGetTarget(out var control))
            {
                base.Start();
                control.KeyDown += Form_KeyDown;
                control.KeyUp += Form_KeyUp;
            }
        }

        public override void Quit()
        {
            if (controlRef.TryGetTarget(out var control))
            {
                control.KeyDown -= Form_KeyDown;
                control.KeyUp -= Form_KeyUp;
                base.Quit();
            }
        }

        private void Form_KeyUp(object sender, KeyEventArgs e)
        {
            KeyState[e.KeyData] = false;
            UpdateStates();
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
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