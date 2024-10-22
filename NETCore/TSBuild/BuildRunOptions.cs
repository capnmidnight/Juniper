namespace Juniper.TSBuild;

record CommandDefinition(string? Flag, string? Description, Func<BuildRunOptions, Action<bool>> Definition);
record Command(string? Flag, string? Description, Action<bool> Action);

public enum PublishLevel
{
    Patch,
    Minor,
    Major
}

public class BuildRunOptions
{
    private static readonly CommandDefinition[] commands =
    [
        new CommandDefinition("--upgrade-js", "Open package.json files", opt => opt.FlagSetter(nameof(OpenPackageJsons))),
        new CommandDefinition("--upgrade-ts", "Open tsconfig.json files", opt => opt.FlagSetter(nameof(OpenTSConfigJsons))),
        new CommandDefinition("--clean-js", "Delete NPM Packages", opt => opt.FlagSetter(nameof(DeleteNodeModuleDirs))),
        new CommandDefinition("--clean-ts", "Delete tsconfig.tsbuildinfo", opt => opt.FlagSetter(nameof(DeleteTSBuildInfos))),
        new CommandDefinition("--install", "Install NPM packages", opt => opt.FlagSetter(nameof(NPMInstalls))),
        new CommandDefinition("--check", "Type Check", opt => opt.FlagSetter(nameof(TypeCheck))),
        new CommandDefinition("--build", "Build", opt => opt.FlagSetter(nameof(Build))),
        new CommandDefinition("--watch", "Watch", opt => opt.FlagSetter(nameof(Watch))),
        new CommandDefinition("--interactive", null, opt => opt.FlagSetter(nameof(Interactive)))
    ];

    public static bool IsBuildCommand(string[] args)
    {
        var flags = commands.Select(c => c.Flag).ToHashSet();
        foreach(var arg in args)
        {
            if (flags.Contains(arg))
            {
                return true;
            }
        }

        return false;
    }

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
    public bool DeleteNodeModuleDirs => flags.Contains(nameof(DeleteNodeModuleDirs));
    public bool DeletePackageLockJsons => flags.Contains(nameof(DeletePackageLockJsons));
    public bool DeleteTSBuildInfos => flags.Contains(nameof(DeleteTSBuildInfos));
    public bool OpenPackageJsons => flags.Contains(nameof(OpenPackageJsons));
    public bool OpenTSConfigJsons => flags.Contains(nameof(OpenTSConfigJsons));
    public bool TypeCheck => flags.Contains(nameof(TypeCheck));
    public bool Watch => flags.Contains(nameof(Watch));

    public bool Build
    {
        get => flags.Contains(nameof(Build));
        set => FlagSetter(nameof(Build))(value);
    }


    private readonly Command[] interactiveCommands;
    private readonly Command[] flagCommands;

    public BuildRunOptions(string[] args)
    {
        interactiveCommands = commands
            .Where(cmd => cmd.Description is not null)
            .Select(cmd => new Command(cmd.Flag, cmd.Description, cmd.Definition(this)))
            .ToArray();

        flagCommands = commands
            .Where(cmd => cmd.Flag is not null)
            .Select(cmd => new Command(cmd.Flag, cmd.Description, cmd.Definition(this)))
            .ToArray();

        var lastOpt = args
            .Where(Directory.Exists)
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

        if (curAnyArg is null)
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