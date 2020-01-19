using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using Juniper.Logging;

using static System.Console;
using static Juniper.AnsiColor;

namespace Juniper.Console
{
    public class ConsoleCommandProcessor :
        IReadOnlyDictionary<ConsoleKey, NamedAction>,
        IInfoDestination,
        IWarningDestination,
        INCSALogDestination,
        IErrorDestination
    {
        private readonly Dictionary<ConsoleKey, NamedAction> actions = new Dictionary<ConsoleKey, NamedAction>();

        private uint logLevel = 0;

        public void Pump()
        {
            if (KeyAvailable)
            {
                var keyInfo = ReadKey(true);
                if (actions.ContainsKey(keyInfo.Key))
                {
                    Execute(keyInfo.Key);
                }
                else
                {
                    PrintUsage();
                }
            }
        }

        public void Execute(ConsoleKey key)
        {
            actions[key].Invoke();
        }

        public void PrintUsage()
        {
            WriteLine("Usage:");
            var maxKeyLen = (from key in actions.Keys
                             let keyName = key.ToString()
                             select keyName.Length)
                        .Max();

            var format = $"\t{{0,-{maxKeyLen}}} : {{1}}";
            foreach (var command in actions)
            {
                WriteLine(format, command.Key, command.Value.Name);
            }
        }

        public IEnumerable<ConsoleKey> Keys
        {
            get
            {
                return ((IReadOnlyDictionary<ConsoleKey, NamedAction>)actions).Keys;
            }
        }

        public IEnumerable<NamedAction> Values
        {
            get
            {
                return ((IReadOnlyDictionary<ConsoleKey, NamedAction>)actions).Values;
            }
        }

        public int Count
        {
            get
            {
                return actions.Count;
            }
        }

        public NamedAction this[ConsoleKey key]
        {
            get
            {
                return actions[key];
            }
        }

        public void Add(ConsoleKey key, string description, Action action)
        {
            actions.Add(key, (description, action));
        }

        public void Add((ConsoleKey key, string description, Action action) entry)
        {
            var (key, description, action) = entry;
            Add(key, description, action);
        }

        public bool ContainsKey(ConsoleKey key)
        {
            return actions.ContainsKey(key);
        }

        public bool TryGetValue(ConsoleKey key, out NamedAction value)
        {
            return actions.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<ConsoleKey, NamedAction>> GetEnumerator()
        {
            return ((IReadOnlyDictionary<ConsoleKey, NamedAction>)actions).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IReadOnlyDictionary<ConsoleKey, NamedAction>)actions).GetEnumerator();
        }

        public void SetLogLevel(int direction)
        {
            var nextLogLevel = (uint)(logLevel + direction);
            if (nextLogLevel < 4)
            {
                logLevel = nextLogLevel;
                WriteLine($"Logging level is now {GetName(logLevel)}");
            }
        }

        public static string GetName(uint logLevel)
        {
            return logLevel switch
            {
                0 => "Verbose",
                1 => "Log",
                2 => "Warning",
                3 => "Error",
                _ => "N/A"
            };
        }

        private void Log<T>(uint level, Action<string> logger, string color, T e)
        {
            if(e is null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            if (level >= logLevel)
            {
                logger($"{color}[{GetName(level)}] {e.ToString()}{Reset}");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnInfo(object sender, StringEventArgs e)
        {
            Log(0, WriteLine, White, e);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnInfo(string color, string msg)
        {
            Log(0, WriteLine, color, msg);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnInfo(string msg)
        {
            Log(0, WriteLine, White, msg);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnLog(object sender, StringEventArgs e)
        {
            Log(1, WriteLine, Cyan, e);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnWarning(object sender, StringEventArgs e)
        {
            Log(2, WriteLine, Yellow, e);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnError(object sender, ErrorEventArgs e)
        {
            Log(3, Error.WriteLine, Red, e);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnError(ErrorEventArgs e)
        {
            Log(3, Error.WriteLine, Red, e);
        }
    }
}
