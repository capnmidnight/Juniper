using System;
using System.Collections.Generic;
using System.Linq;

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
        private readonly Dictionary<string, (string, string)> axes = new Dictionary<string, (string, string)>();

        protected Dictionary<KeyT, bool> KeyState { get; } = new Dictionary<KeyT, bool>();

        protected KeyT[] Keys { get; private set; }

        private string[] names;

        protected bool IsRunning { get; private set; }

        public virtual void Start()
        {
            Keys = KeyState.Keys.ToArray();
            names = aliases.Keys.ToArray();
            IsRunning = true;
        }

        public virtual void Join()
        {
            IsRunning = false;
        }

        protected void UpdateStates()
        {
            if (IsRunning)
            {
                for(var i = 0; i < names.Length; ++i)
                {
                    var name = names[i];
                    var key = aliases[name];
                    var wasDown = aliasState[name];
                    var isDown = KeyState[key];

                    aliasState[name] = isDown;

                    if (isDown != wasDown)
                    {
                        if (isDown)
                        {
                            KeyDown?.Invoke(this, events[name]);
                            KeyChanged?.Invoke(this, downEvents[name]);
                        }
                        else
                        {
                            KeyUp?.Invoke(this, events[name]);
                            KeyChanged?.Invoke(this, upEvents[name]);
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

        public float GetValue(string name)
        {
            return IsDown(name) ? 1 : 0;
        }

        public abstract bool IsKeyDown(KeyT key);

        public void DefineAxis(string name, string negative, string positive)
        {
            axes[name] = (negative, positive);
        }

        public float GetAxis(string name)
        {
            var (negative, positive) = axes[name];
            return GetValue(positive) - GetValue(negative);
        }
    }
}
