using Juniper.Units;

namespace Juniper.TSBuild
{
    record Command(string? Flag, string? Description, Action<bool> Action);

    class Options
    {
        private string? curAnyArg;
        private bool AnyOnly => curAnyArg is not null;

        private readonly HashSet<string> flags = new();

        private void SetFlag(string name, bool value)
        {
            if (value != flags.Contains(name))
            {
                if (value)
                {
                    flags.Add(name);
                }
                else
                {
                    flags.Remove(name);
                }
            }
        }

        private Action<bool> FlagSetter(string name)
        {
            return (value) => SetFlag(name, value);
        }

        public DirectoryInfo? workingDir = null;

        public bool interactive;
        public bool complete = true;
        private bool parseLevel;
        public Level level = Level.None;

        public bool VersionOnly => flags.Contains(nameof(VersionOnly));
        public bool InstallOnly => flags.Contains(nameof(InstallOnly));
        public bool DetectCyclesOnly => flags.Contains(nameof(DetectCyclesOnly));
        public bool CheckOnly => flags.Contains(nameof(CheckOnly));
        public bool AuditOnly => flags.Contains(nameof(AuditOnly));
        public bool AuditFixOnly => flags.Contains(nameof(AuditFixOnly));
        public bool CleanOnly => flags.Contains(nameof(CleanOnly));
        public bool PrintDependencyTreeOnly => flags.Contains(nameof(PrintDependencyTreeOnly));
        public bool DeletePackageLockOnly => flags.Contains(nameof(DeletePackageLockOnly));


        private readonly Command[] interactiveCommands;
        private readonly Command[] flagCommands;

        public Options(string[] args)
        {
            var commands = new[]
            {
                new Command("--clean", "Delete NPM Packages", FlagSetter(nameof(CleanOnly))),
                new Command("--install", "Install NPM packages", FlagSetter(nameof(InstallOnly))),
                new Command("--check", "Run TypeScript check", FlagSetter(nameof(CheckOnly))),
                new Command(null, "Delete package-lock.json", FlagSetter(nameof(DeletePackageLockOnly))),
                new Command("--audit", "Audit NPM packages", FlagSetter(nameof(AuditOnly))),
                new Command("--audit-fix", "Audit and auto-fix NPM packages", FlagSetter(nameof(AuditFixOnly))),
                new Command("--detect-cycles", "Detect NPM package cycles", FlagSetter(nameof(DetectCyclesOnly))),
                new Command("--dep-tree", "Print NPM package tree", FlagSetter(nameof(PrintDependencyTreeOnly))),
                new Command(null, "Build (level: Low)", (_) => level = Level.Low),
                new Command(null, "Build (level: Med)", (_) => level = Level.Medium),
                new Command(null, "Build (level: High)", (_) => level = Level.High),
                new Command("--version", null, FlagSetter(nameof(VersionOnly)))
            };

            interactiveCommands = commands
                .Where(cmd => cmd.Description is not null)
                .ToArray();

            flagCommands = commands
                .Where(cmd => cmd.Flag is not null)
                .ToArray();

            var lastOpt = args
                .Where(ProcessArg)
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(lastOpt))
            {
                workingDir = new DirectoryInfo(lastOpt);
            }
        }

        private bool ProcessArg(string arg)
        {
            if (arg == "--interactive")
            {
                interactive = true;
                complete = false;
                return false;
            }

            if (arg == "--level")
            {
                parseLevel = true;
                return false;
            }

            if (parseLevel)
            {
                if (AnyOnly)
                {
                    Console.Error.WriteLine("Can't specify level when running exclusive argument {0}", curAnyArg);
                }
                else
                {
                    level = Enum.Parse<Level>(arg);
                }
                parseLevel = false;
                return false;
            }

            foreach (var cmd in flagCommands)
            {
                if (cmd.Flag == arg)
                {
                    cmd.Action(WarnIfAnyOnly(arg));
                    return false;
                }
            }

            return true;
        }

        private bool WarnIfAnyOnly(string arg)
        {
            if (AnyOnly)
            {
                Console.Error.WriteLine("Can't use {0} switch. Already running {1}", arg, curAnyArg);
                return false;
            }
            else
            {
                curAnyArg = arg;
                return true;
            }
        }

        public void REPL()
        {
            flags.Clear();
            level = Level.None;

            Console.WriteLine("Enter command:");
            for (int i = 0; i < interactiveCommands.Length; i++)
            {
                Console.WriteLine("\t{0}: {1}", i + 1, interactiveCommands[i].Description);
            }

            Console.WriteLine("\tX: Quit");

            var good = false;
            while (!good)
            {
                Console.Write(":> ");
                var entry = Console.ReadLine();
                if (int.TryParse(entry, out var choice)
                    && 1 <= choice
                    && choice <= interactiveCommands.Length)
                {
                    good = true;
                    interactiveCommands[choice - 1].Action(true);
                }
                else
                {
                    entry = entry?.ToLowerInvariant();
                    good = complete
                        = entry == "x"
                        || entry == "q"
                        || entry == "exit"
                        || entry == "quit";
                }
            }
        }
    }
}