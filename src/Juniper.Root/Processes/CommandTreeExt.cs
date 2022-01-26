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

        private static readonly string[] toInstall = new[]
        {
            "esbuild",
            "emoji",
            "tslib",
            "mediatypes",
            "dom",
            "fetcher-base",
            "fetcher",
            "fetcher-worker",
            "google-maps",
            "graphics2d",
            "testing",
            "units",
            "audio",
            "webrtc",
            "threejs"
        };

        public static ICommandTree InitJuniper(this ICommandTree commands, DirectoryInfo juniperDir)
        {
            return commands.AddCommands(toInstall.Select(name => NPM(juniperDir, name, "init")));
        }

        public static ICommandTree InstallJuniper(this ICommandTree commands, DirectoryInfo juniperDir)
        {
            return commands.AddCommands(toInstall.Select(name => NPM(juniperDir, name, "inst")));
        }

        private static readonly string[] toBuild = new[]
        {
            "esbuild",
            "fetcher-worker"
        };

        public static ICommandTree BuildJuniper(this ICommandTree commands, DirectoryInfo juniperDir, bool checkAll = false)
        {
            var tasks = checkAll ? toInstall : toBuild;
            return commands.AddCommands(tasks.Select(name => NPM(juniperDir, name, "build")));
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
