#if !NETSTANDARD
using System.Windows.Input;

namespace Juniper.Input
{

    public sealed class WindowsAPIKeyEventSource :
        AbstractPollingKeyEventSource<Key>
    {

        delegate void Action2();
        public override bool IsKeyDown(Key key)
        {
            Action2 x = delegate
            {
            };
            var y = x.BeginInvoke(new System.AsyncCallback((res) =>
            {
            }), null);
            return (Keyboard.GetKeyStates(key) & KeyStates.Down) != 0;
        }
    }
}
#endif