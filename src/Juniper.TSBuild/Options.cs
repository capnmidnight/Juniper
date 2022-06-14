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

        public bool Interactive { get; private set; }
        public bool Finished { get; private set; } = true;

        public DirectoryInfo? workingDir;

        public bool NPMInstalls => flags.Contains(nameof(NPMInstalls));
        public bool NPMAudits => flags.Contains(nameof(NPMAudits));
        public bool NPMAuditFixes => flags.Contains(nameof(NPMAuditFixes));
        public bool DeleteNodeModuleDirs => flags.Contains(nameof(DeleteNodeModuleDirs));
        public bool DeletePackageLockJsons => flags.Contains(nameof(DeletePackageLockJsons));
        public bool DeleteTSBuildInfos => flags.Contains(nameof(DeleteTSBuildInfos));
        public bool OpenPackageJsons => flags.Contains(nameof(OpenPackageJsons));
        public bool TypeCheck => flags.Contains(nameof(TypeCheck));
        public bool Build
        {
            get => flags.Contains(nameof(Build));
            set => FlagSetter(nameof(Build))(value);
        }


        private readonly Command[] interactiveCommands;
        private readonly Command[] flagCommands;

        public Options(string[] args)
        {
            var commands = new[]
            {
                new Command("--install", "Install NPM packages", FlagSetter(nameof(NPMInstalls))),
                new Command(null, "Type Check", FlagSetter(nameof(TypeCheck))),
                new Command(null, "Build", FlagSetter(nameof(Build))),
                new Command("--clean", "Delete NPM Packages", FlagSetter(nameof(DeleteNodeModuleDirs))),
                new Command(null, "Delete package-lock.json", FlagSetter(nameof(DeletePackageLockJsons))),
                new Command(null, "Delete tsconfig.tsbuildinfo", FlagSetter(nameof(DeleteTSBuildInfos))),
                new Command("--audit", "Audit NPM packages", FlagSetter(nameof(NPMAudits))),
                new Command("--audit-fix", "Audit and auto-fix NPM packages", FlagSetter(nameof(NPMAuditFixes))),
                new Command(null, "Open package.json files", FlagSetter(nameof(OpenPackageJsons)))
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

            foreach (var arg in args)
            {
                ProcessArg(arg);
            }

            if(curAnyArg is null)
            {
                Build = true;
            }
        }

        private bool ProcessArg(string arg)
        {
            if (arg == "--interactive")
            {
                Interactive = true;
                Finished = false;
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
                    good = Finished
                        = entry == "x"
                        || entry == "q"
                        || entry == "exit"
                        || entry == "quit";
                }
            }
        }
    }
}