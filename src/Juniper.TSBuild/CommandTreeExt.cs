using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Juniper.Processes
{
    public static class CommandTreeExt
    {
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
            return commands.AddCommands(
                AllProjects(juniperDir, false)
                    .Select(name => NPM(juniperDir, name, "watch")
                        .OnStandardOutput(
                            new Regex("done in \\d+(\\.\\d+)?s|browser bundles rebuilt", RegexOptions.Compiled),
                            Copy(name, juniperDir, outDir))));
        }

        private static readonly string[] filesToCopy =
        {
            "index.js",
            "index.js.map",
            "index.min.js",
            "index.min.js.map"
        };

        private static IEnumerable<ICommand> Copy(string name, DirectoryInfo juniperDir, DirectoryInfo outputDir)
        {
            var from = juniperDir.MkDir("src", "Juniper.TypeScript", name, "dist");
            var to = outputDir.MkDir(name);
            return filesToCopy.Select(file =>
               new CopyCommand(
                   from.Touch(file).FullName,
                   to.Touch(file).FullName));
        }

        public static ICommandTree CopyJuniperScripts(this ICommandTree commands, DirectoryInfo juniperDir, DirectoryInfo outputDir)
        {
            return commands.AddCommands(
                Copy("fetcher-worker", juniperDir, outputDir)
                    .Union(Copy("environment", juniperDir, outputDir))
                    .Union(Copy("tele", juniperDir, outputDir)));
        }
    }
}
