using System;
using System.Windows.Forms;

using Juniper.Input;

namespace Juniper
{
    public partial class KeyInputForm : Form
    {
        private readonly IKeyEventSource keys;

        public KeyInputForm()
        {
            InitializeComponent();
            var keys = new WinFormsKeyEventSource(this);
            this.keys = keys;
            textBox1.KeyDown += OnKeyDown;
            textBox1.KeyPress += OnKeyPress;
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            keys.Start();
        }

        private void OnKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            textBox1.Text += $"DOWN: Code({e.KeyCode}) Data({e.KeyData})\r\n";
        }

        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            textBox1.Text += $"PRESS: {e.KeyChar}\r\n";
        }
    }
}
