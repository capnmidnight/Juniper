using System.Text.RegularExpressions;

namespace Juniper.Processes
{
    public static class CommandTreeExt
    {
        private enum Por
        {
            All,
            Basic,
            Minified
        }

        private static readonly string[] basicFilesToCopy = {
            "index.js",
            "index.js.map"
        };

        private static readonly string[] minifiedFilesToCopy = {
            "index.min.js",
            "index.min.js.map"
        };

        private static string[]? allFilesToCopy = null;

        private static IEnumerable<ICommand> Copy(string name, DirectoryInfo juniperDir, DirectoryInfo outputDir, Por por)
        {
            if(allFilesToCopy is null)
            {
                allFilesToCopy = basicFilesToCopy.Union(minifiedFilesToCopy).ToArray();
            }

            var toCopy = por == Por.Basic
                ? basicFilesToCopy
                : por == Por.Minified
                    ? minifiedFilesToCopy
                    : allFilesToCopy;

            var from = juniperDir.MkDir("src", "Juniper.TypeScript", name, "dist");
            var to = outputDir.MkDir(name);
            return toCopy.Select(file =>
               new CopyCommand(
                   from.Touch(file).FullName,
                   to.Touch(file).FullName));
        }

        private static ShellCommand NPM(DirectoryInfo juniperDir, string name, string cmd)
        {
            return new ShellCommand(juniperDir.CD("src", "Juniper.TypeScript", name), "npm", "run", cmd);
        }

        private static IEnumerable<string> AllProjects(DirectoryInfo juniperDir, bool all = true)
        {
            return juniperDir.CD("src", "Juniper.TypeScript")
                .EnumerateDirectories()
                .Where(d => all || d.EnumerateDirectories().Any(sd => sd.Name == "src"))
                .Select(x => x.Name);
        }

        public static ICommandTree UpdateJuniper(this ICommandTree commands, DirectoryInfo juniperDir)
        {
            return commands.AddCommands(
                AllProjects(juniperDir)
                    .Select(name =>
                        NPM(juniperDir, name, "update")));
        }

        public static ICommandTree InstallJuniper(this ICommandTree commands, DirectoryInfo juniperDir)
        {
            return commands.AddCommands(
                AllProjects(juniperDir)
                    .Select(name =>
                        NPM(juniperDir, name, "inst")));
        }

        public static ICommandTree BuildJuniper(this ICommandTree commands, DirectoryInfo juniperDir, bool checkAll = false)
        {
            return commands.AddCommands(
                AllProjects(juniperDir, checkAll)
                    .Select(name =>
                        NPM(juniperDir, name, "build")));
        }

        public static ICommandTree WatchJuniper(this ICommandTree commands, DirectoryInfo juniperDir, DirectoryInfo outDir)
        {
            return commands.AddCommands(juniperDir.GetJuniperWatchCommands(outDir));
        }

        private static readonly Regex watchAllDonePattern = new("^done in \\d+(\\.\\d+)?s$", RegexOptions.Compiled);
        private static readonly Regex watchBasicDonePattern = new("^browser bundles rebuilt$", RegexOptions.Compiled);
        private static readonly Regex watchMinifiedDonePattern = new("^minified browser bundles rebuilt$", RegexOptions.Compiled);

        public static IEnumerable<ShellCommand> GetJuniperWatchCommands(this DirectoryInfo juniperDir, DirectoryInfo outDir)
        {
            return AllProjects(juniperDir, false)
                .Where(name => name != "esbuild")
                .Select(name => NPM(juniperDir, name, "watch")
                    .OnStandardOutput(
                        watchAllDonePattern,
                        Copy(name, juniperDir, outDir, Por.All))
                    .OnStandardOutput(
                        watchBasicDonePattern,
                        Copy(name, juniperDir, outDir, Por.Basic))
                    .OnStandardOutput(
                        watchMinifiedDonePattern,
                        Copy(name, juniperDir, outDir, Por.Minified)));
        }

        public static ICommandTree CopyJuniperScripts(this ICommandTree commands, DirectoryInfo juniperDir, DirectoryInfo outputDir)
        {
            var cmds =
                Copy("fetcher-worker", juniperDir, outputDir, Por.All)
                    .Union(Copy("environment", juniperDir, outputDir, Por.All))
                    .Union(Copy("tele", juniperDir, outputDir, Por.All))
                .ToArray();
            return commands.AddCommands(cmds);
        }
    }
}
