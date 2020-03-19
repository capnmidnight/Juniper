using System;
using System.Collections.Generic;
using System.Linq;

namespace Juniper.Input
{
    public abstract class AbstractKeyEventSource<KeyT> :
        IKeyEventSource
    {
        public event EventHandler<KeyChangeEventArgs> Changed;
        public event EventHandler<KeyEventArgs> Down;
        public event EventHandler<KeyEventArgs> Up;

        private readonly Dictionary<string, KeyT> aliases = new Dictionary<string, KeyT>();
        private readonly Dictionary<string, KeyEventArgs> events = new Dictionary<string, KeyEventArgs>();
        private readonly Dictionary<string, KeyChangeEventArgs> upEvents = new Dictionary<string, KeyChangeEventArgs>();
        private readonly Dictionary<string, KeyChangeEventArgs> downEvents = new Dictionary<string, KeyChangeEventArgs>();
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

        public virtual void Quit()
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
                            Down?.Invoke(this, events[name]);
                            Changed?.Invoke(this, downEvents[name]);
                        }
                        else
                        {
                            Up?.Invoke(this, events[name]);
                            Changed?.Invoke(this, upEvents[name]);
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
                events[name] = new KeyEventArgs(name);
                upEvents[name] = new KeyChangeEventArgs(name, false);
                downEvents[name] = new KeyChangeEventArgs(name, true);
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
