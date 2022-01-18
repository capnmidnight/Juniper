using Juniper.Logging;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

using static System.ConsoleColor;

namespace Juniper.Terminal
{
    public class ConsoleCommandProcessor :
        IReadOnlyDictionary<ConsoleKey, NamedAction>,
        IInfoDestination,
        IWarningDestination,
        INCSALogDestination,
        IErrorDestination
    {
        private readonly Dictionary<ConsoleKey, NamedAction> actions = new();

        private uint logLevel;

        public void Pump()
        {
            if (Console.KeyAvailable)
            {
                var keyInfo = Console.ReadKey(true);
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
            var maxKeyLen = (from key in actions.Keys
                             let keyName = key.ToString()
                             select keyName.Length)
                        .Max();

            var format = $"  {{0,-{maxKeyLen}}} : {{1}}";

            var lines = (from command in actions
                         select string.Format(CultureInfo.InvariantCulture, format, command.Key, command.Value.Name))
                         .Prepend("Usage:")
                         .Prepend("")
                         .Append("")
                         .ToArray();

            var maxLen = lines.Max(l => l.Length);
            for (var i = 0; i < lines.Length; ++i)
            {
                while (lines[i].Length <= maxLen)
                {
                    lines[i] += " ";
                }

                lines[i] = " " + lines[i];
            }

            Console.WriteLine();
            using var scope = new ColorScope(DarkGray, Yellow);
            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }
        }

        public IEnumerable<ConsoleKey> Keys => ((IReadOnlyDictionary<ConsoleKey, NamedAction>)actions).Keys;

        public IEnumerable<NamedAction> Values => ((IReadOnlyDictionary<ConsoleKey, NamedAction>)actions).Values;

        public int Count => actions.Count;

        public NamedAction this[ConsoleKey key] => actions[key];

        public void Add(ConsoleKey key, string description, Action action)
        {
            actions.Add(key, (description, action));
        }

        public void Add((ConsoleKey key, string description, Action action) entry)
        {
            (var key, var description, var action) = entry;
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
                Console.WriteLine($"Logging level is now {GetName(logLevel)}");
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

        private void Log<T>(uint level, Action<string> logger, ConsoleColor foreground, T e)
        {
            Log(level, logger, foreground, Console.BackgroundColor, e);
        }

        private void Log<T>(uint level, Action<string> logger, ConsoleColor foreground, ConsoleColor background, T e)
        {
            if (level >= logLevel)
            {
                using var scope = new ColorScope(background, foreground);
                logger($"[{GetName(level)}] {e?.ToString()}");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnInfo(object sender, StringEventArgs e)
        {
            OnInfo(e?.Value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnInfo(ConsoleColor color, string msg)
        {
            Log(0, Console.WriteLine, color, msg);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnInfo(string msg)
        {
            Log(0, Console.WriteLine, White, msg);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnLog(object sender, StringEventArgs e)
        {
            Log(1, Console.WriteLine, Cyan, e);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnWarning(object sender, StringEventArgs e)
        {
            Log(2, Console.WriteLine, Yellow, e);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnError(object sender, ErrorEventArgs e)
        {
            Log(3, Console.Error.WriteLine, Red, e);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnError(ErrorEventArgs e)
        {
            Log(3, Console.Error.WriteLine, Red, e);
        }
    }
}
