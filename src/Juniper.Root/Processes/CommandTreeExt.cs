using System.IO;
using System.Linq;

namespace Juniper.Processes
{
    public static class CommandTreeExt
    {
        private static ShellCommand NPM(DirectoryInfo juniperDir, string name, string cmd)
        {
            return new ShellCommand("npm", "run", "--prefix", Path.Combine(juniperDir.FullName, "src", "Juniper.TypeScript", name), cmd);
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

        private static readonly string[] toBuild = new[]
        {
            "esbuild",
            "fetcher-worker"
        };

        public static ICommandTree BuildJuniper(this ICommandTree commands, DirectoryInfo juniperDir)
        {
            return commands.AddCommands(toBuild.Select(name => NPM(juniperDir, name, "build")));
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