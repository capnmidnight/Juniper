using System.Collections.Generic;
using System.IO;
using System.Linq;

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
                .Where(d => all || d.EnumerateDirectories().Any(sd => sd.Name == "dist"))
                .Select(x => x.Name);
        }

        public static ICommandTree InitJuniper(this ICommandTree commands, DirectoryInfo juniperDir)
        {
            return commands.AddCommands(
                AllProjects(juniperDir)
                    .Select(name =>
                        NPM(juniperDir, name, "init")));
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

        static CopyCommand Copy(DirectoryInfo juniperDir, DirectoryInfo outputDir, string from, string to)
        {
            return new CopyCommand(
                Path.Combine(
                    juniperDir.FullName,
                    "src",
                    "Juniper.TypeScript",
                    PathExt.FixPath(from)),
                Path.Combine(
                    outputDir.FullName,
                    "wwwroot",
                    PathExt.FixPath(to)));
        }

        public static ICommandTree CopyJuniperScripts(this ICommandTree commands, DirectoryInfo juniperDir, DirectoryInfo outputDir)
        {
            return commands.AddCommands(
                Copy(juniperDir, outputDir, "fetcher-worker/dist/index.js", "workers/fetcher/index.js"),
                Copy(juniperDir, outputDir, "fetcher-worker/dist/index.js.map", "workers/fetcher/index.js.map"),
                Copy(juniperDir, outputDir, "fetcher-worker/dist/index.min.js", "workers/fetcher/index.min.js"),
                Copy(juniperDir, outputDir, "fetcher-worker/dist/index.min.js.map", "workers/fetcher/index.min.js.map"));
        }
    }
}
