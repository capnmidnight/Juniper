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

        private static IEnumerable<ICommand> Copy(string name, DirectoryInfo inputDir, DirectoryInfo outputDir, Por por)
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

            var from = inputDir.MkDir(name, "dist");
            var to = outputDir.MkDir(name);
            return toCopy.Select(file =>
               new CopyCommand(
                   from.Touch(file).FullName,
                   to.Touch(file).FullName));
        }

        public static ShellCommand NPM(DirectoryInfo tsRootDir, string name, string cmd)
        {
            return new ShellCommand(tsRootDir.CD(name), "npm", "run", cmd);
        }

        private static IEnumerable<string> AllProjectsNames(DirectoryInfo juniperDir)
        {
            return juniperDir.GetJuniperProjectDirectories()
                .Select(x => x.Name);
        }

        private static IEnumerable<DirectoryInfo> AllBundles(DirectoryInfo juniperDir)
        {
            return juniperDir.GetJuniperProjectDirectories()
                .Where(d => d.EnumerateFiles().Any(f => f.Name == "esbuild.config.js"));
        }

        private static IEnumerable<string> AllBundleNames(DirectoryInfo juniperDir)
        {
            return AllBundles(juniperDir)
                .Select(d => d.Name);
        }

        private static IEnumerable<DirectoryInfo> GetJuniperProjectDirectories(this DirectoryInfo juniperDir)
        {
            return juniperDir.CD("src", "Juniper.TypeScript")
                .EnumerateDirectories()
                .Where(d => d.EnumerateFiles()
                    .Any(f => f.Name == "package.json"));
        }

        public static ICommandTree UpdateJuniper(this ICommandTree commands, DirectoryInfo juniperDir)
        {
            var juniperTSDir = juniperDir.CD("src", "Juniper.TypeScript");
            return commands.AddCommands(
                AllProjectsNames(juniperDir)
                    .Select(name =>
                        NPM(juniperTSDir, name, "update")));
        }

        public static ICommandTree InstallJuniper(this ICommandTree commands, DirectoryInfo juniperDir)
        {
            var juniperTSDir = juniperDir.CD("src", "Juniper.TypeScript");
            return commands.AddCommands(
                AllProjectsNames(juniperDir)
                    .Select(name =>
                        NPM(juniperTSDir, name, "inst")));
        }

        public static ICommandTree BuildAllJuniperProjects(this ICommandTree commands, DirectoryInfo juniperDir)
        {
            var juniperTSDir = juniperDir.CD("src", "Juniper.TypeScript");
            return commands.AddCommands(
                AllProjectsNames(juniperDir)
                    .Select(name =>
                        NPM(juniperTSDir, name, "build")));
        }

        public static ICommandTree BuildAllJuniperBundles(this ICommandTree commands, DirectoryInfo juniperDir)
        {
            var juniperTSDir = juniperDir.CD("src", "Juniper.TypeScript");
            return commands.AddCommands(
                AllBundleNames(juniperDir)
                    .Select(name =>
                        NPM(juniperTSDir, name, "build")));
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
            var juniperTSDir = juniperDir.CD("src", "Juniper.TypeScript");
            return AllBundleNames(juniperDir)
                .Select(name => NPM(juniperTSDir, name, "watch")
                    .OnStandardOutput(
                        watchAllDonePattern,
                        Copy(name, juniperTSDir, outDir, Por.All))
                    .OnStandardOutput(
                        watchBasicDonePattern,
                        Copy(name, juniperTSDir, outDir, Por.Basic))
                    .OnStandardOutput(
                        watchMinifiedDonePattern,
                        Copy(name, juniperTSDir, outDir, Por.Minified)));
        }

        public static ICommandTree CopyJuniperScripts(this ICommandTree commands, DirectoryInfo juniperDir, DirectoryInfo outputDir)
        {
            var juniperTSDir = juniperDir.CD("src", "Juniper.TypeScript");
            var cmds =
                Copy("fetcher-worker", juniperTSDir, outputDir, Por.All)
                    .Union(Copy("environment", juniperTSDir, outputDir, Por.All))
                    .Union(Copy("tele", juniperTSDir, outputDir, Por.All))
                .ToArray();
            return commands.AddCommands(cmds);
        }
    }
}
