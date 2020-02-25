using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Threading;

namespace Juniper.Input
{
    public abstract class AbstractKeyEventSource<KeyT> :
        IKeyEventSource
    {
        public event EventHandler<KeyChangeEvent> KeyChanged;
        public event EventHandler<KeyEvent> KeyDown;
        public event EventHandler<KeyEvent> KeyUp;

        private readonly Dictionary<string, KeyT> aliases = new Dictionary<string, KeyT>();
        private readonly Dictionary<string, KeyEvent> events = new Dictionary<string, KeyEvent>();
        private readonly Dictionary<string, KeyChangeEvent> upEvents = new Dictionary<string, KeyChangeEvent>();
        private readonly Dictionary<string, KeyChangeEvent> downEvents = new Dictionary<string, KeyChangeEvent>();
        private readonly Dictionary<string, bool> aliasState = new Dictionary<string, bool>();

        protected Dictionary<KeyT, bool> KeyState { get; } = new Dictionary<KeyT, bool>();

        protected IReadOnlyCollection<KeyT> Keys { get; private set; }

        private string[] names;

        private readonly Context origin;

        protected bool IsRunning { get; private set; }

        protected AbstractKeyEventSource()
        {
            origin = Thread.CurrentContext;
        }

        public virtual void Start()
        {
            Keys = KeyState.Keys.ToArray();
            names = aliases.Keys.ToArray();
            IsRunning = true;
        }

        public virtual void Quit()
        {
            IsRunning = false;
        }

        protected void UpdateStates()
        {
            if (IsRunning)
            {
                foreach (var name in names)
                {
                    var key = aliases[name];
                    var wasDown = aliasState[name];
                    var isDown = KeyState[key];

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
        }

        public void AddKeyAlias(string name, KeyT key)
        {
            if (!IsRunning)
            {
                KeyState[key] = false;
                aliases[name] = key;
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

        public abstract bool IsKeyDown(KeyT key);
    }
}
