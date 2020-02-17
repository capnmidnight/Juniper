using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Threading;

namespace Juniper.Input
{
    public abstract class AbstractKeyEventSource<KeyT>
    {
        public event EventHandler<KeyChangeEvent> KeyChanged;
        public event EventHandler<KeyEvent> KeyDown;
        public event EventHandler<KeyEvent> KeyUp;

        private readonly Dictionary<string, KeyT[]> aliases = new Dictionary<string, KeyT[]>();
        private readonly Dictionary<string, KeyEvent> events = new Dictionary<string, KeyEvent>();
        private readonly Dictionary<string, KeyChangeEvent> upEvents = new Dictionary<string, KeyChangeEvent>();
        private readonly Dictionary<string, KeyChangeEvent> downEvents = new Dictionary<string, KeyChangeEvent>();
        private readonly Dictionary<string, bool> aliasState = new Dictionary<string, bool>();

        protected Dictionary<KeyT, bool> KeyState { get; } = new Dictionary<KeyT, bool>();

        protected IReadOnlyCollection<KeyT> Keys { get; private set; }

        private string[] names;

        private readonly Context origin;

        protected AbstractKeyEventSource()
        {
            origin = Thread.CurrentContext;
        }

        public virtual void Start()
        {
            Keys = KeyState.Keys.ToArray();
            names = aliases.Keys.ToArray();
        }

        public virtual void Stop() { }

        protected void UpdateStates()
        {
            foreach (var name in names)
            {
                var wasDown = aliasState[name];
                var isDown = true;
                foreach (var key in aliases[name])
                {
                    isDown &= KeyState[key];
                }

                aliasState[name] = isDown;

                if (isDown != wasDown)
                {
                    if (isDown)
                    {
                        origin.DoCallBack(() =>
                        {
                            KeyDown?.Invoke(this, events[name]);
                            KeyChanged?.Invoke(this, downEvents[name]);
                        });
                    }
                    else
                    {
                        origin.DoCallBack(() =>
                        {
                            KeyUp?.Invoke(this, events[name]);
                            KeyChanged?.Invoke(this, upEvents[name]);
                        });
                    }
                }
            }
        }

        public void AddKeyAlias(string name, params KeyT[] keys)
        {
            lock (KeyState)
            {
                foreach (var key in keys)
                {
                    KeyState[key] = false;
                }

                aliases[name] = keys;
                aliasState[name] = false;
                events[name] = new KeyEvent(name);
                upEvents[name] = new KeyChangeEvent(name, false);
                downEvents[name] = new KeyChangeEvent(name, true);
            }
        }

        public bool IsDown(string name)
        {
            return aliasState[name];
        }
    }
}
