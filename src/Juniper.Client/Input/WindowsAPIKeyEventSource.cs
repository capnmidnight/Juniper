#if !NETSTANDARD
using System.Windows.Input;

namespace Juniper.Input
{

    public sealed class WindowsAPIKeyEventSource :
        AbstractPollingKeyEventSource<Key>
    {
        protected override bool IsKeyDown(Key key)
        {
            return Keyboard.IsKeyDown(key);
        }
    }
}
#endif