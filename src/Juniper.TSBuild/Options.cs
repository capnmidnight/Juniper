using Juniper.Units;

namespace Juniper.TSBuild
{
    record Command(string? Flag, string? Description, Action<bool> Action);

    class Options
    {
        private string? curAnyArg;
        private bool AnyOnly => curAnyArg is not null;

        public DirectoryInfo? workingDir = null;

        public bool interactive = false;
        public bool complete = true;
        private bool parseLevel = false;

        public bool versionOnly = false;
        public bool installOnly = false;
        public bool checkOnly = false;
        public bool auditOnly = false;
        public bool auditFixOnly = false;
        public bool cleanOnly = false;
        public Level level = Level.None;

        private readonly Command[] interactiveCommands;
        private readonly Command[] flagCommands;

        public Options(string[] args)
        {
            var commands = new[]
            {
                new Command("--clean", "Clean", (set) => cleanOnly = set),
                new Command("--install", "Install NPM packages", (set) => installOnly = set),
                new Command("--audit", "Audit NPM packages", (set) => auditOnly = set),
                new Command("--audit-fix", "Audit and auto-fix NPM packages", (set) => auditFixOnly = set),
                new Command("--check", "Run TypeScript check", (set) => checkOnly = set),
                new Command(null, "Build (level: Low)", (_) => level = Level.Low),
                new Command(null, "Build (level: Med)", (_) => level = Level.Medium),
                new Command(null, "Build (level: High)", (_) => level = Level.High),
                new Command("--version", null, (set) => versionOnly = set),
                new Command(null, "Quit", (set) => complete = set)
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
            versionOnly = false;
            installOnly = false;
            checkOnly = false;
            auditOnly = false;
            auditFixOnly = false;
            cleanOnly = false;
            level = Level.None;

            Console.WriteLine("Enter command:");
            for (int i = 0; i < interactiveCommands.Length; i++)
            {
                Console.WriteLine("\t{0}: {1}", i + 1, interactiveCommands[i].Description);
            }

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
            }
        }
    }
}