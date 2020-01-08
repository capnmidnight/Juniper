using System;
using System.Globalization;
using System.IO;
using System.Linq;

using static System.Console;
using static System.IO.Path;

namespace FileCopier
{
    internal static class Program
    {
        private const string NEWTONSOFT_JSON_DLL = "Newtonsoft.Json.dll";
        private const string NETSTANDARD = "netstandard";
        private const string EXCLUDE_DEPENDENCIES_KEY = "--excludeDeps";

        public static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Error.WriteLine("Command expects at least three arguments");
                for (var i = 0; i < args.Length; ++i)
                {
                    Error.WriteLine("{0}: {1}", i, args[i]);
                }
            }
            else
            {
                for (var i = 0; i < args.Length; ++i)
                {
                    if (args[i].EndsWith("\"", StringComparison.CurrentCulture)
                        || args[i].EndsWith("'", StringComparison.CurrentCulture))
                    {
                        args[i] = args[i].Substring(1, args[i].Length - 2);
                    }
                }

                var allArgs = args.ToList();

                var excludeDependencies = allArgs.Contains(EXCLUDE_DEPENDENCIES_KEY);
                if (excludeDependencies)
                {
                    allArgs.Remove(EXCLUDE_DEPENDENCIES_KEY);
                    args = allArgs.ToArray();
                }

                var projectName = args[0];

                var source = new DirectoryInfo(args[1]);
                if (!source.Exists)
                {
                    Error.WriteLine("Source directory does not exist");
                }
                else if (source.Name != "netstandard2.0")
                {
                    var dest1 = new DirectoryInfo(args[2]);
                    var dest2 = dest1.Parent;

                    WriteLine("Copying from {0} to {1}", source.FullName, dest1.FullName);

                    foreach (var sourceFile in source.GetFiles()
                        .Select(ReplaceNewtonsoft))
                    {
                        var isDependency = !sourceFile.Name.StartsWith(projectName, false, CultureInfo.InvariantCulture);
                        var dest = isDependency
                            ? dest2
                            : dest1;

                        if ((!isDependency
                                || !excludeDependencies)
                            && (sourceFile.Extension != ".pdb"
                                || dest.Name.EndsWith("Debug", false, CultureInfo.InvariantCulture)))
                        {
                            dest.Create();

                            var destFileName = Combine(
                                dest.FullName,
                                sourceFile.Name);

                            WriteLine(destFileName);
                            sourceFile.CopyTo(destFileName, true);
                        }
                    }
                }
            }
        }

        private static FileInfo ReplaceNewtonsoft(FileInfo file)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (file.Name == NEWTONSOFT_JSON_DLL)
            {
                var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                var newtonsoft = new DirectoryInfo(Combine(userProfile, ".nuget", "packages", "newtonsoft.json"));
                if (!newtonsoft.Exists)
                {
                    WriteLine("No Newtonsoft NuGet package found");
                }
                else
                {
                    var maxVersionDir = (from dir in newtonsoft.EnumerateDirectories()
                                         let version = new Version(dir.Name)
                                         orderby version descending
                                         select dir)
                                    .FirstOrDefault();
                    if (maxVersionDir is null)
                    {
                        WriteLine("No versions of Newtonsoft package installed");
                    }
                    else
                    {
                        var versionDir = new DirectoryInfo(Combine(maxVersionDir.FullName, "lib"));
                        if (!versionDir.Exists)
                        {
                            WriteLine("Error constructing version directory {0}", versionDir.FullName);
                        }
                        else
                        {
                            var suitableDir = (from dir in versionDir.EnumerateDirectories()
                                               where dir.Name.StartsWith(NETSTANDARD, false, CultureInfo.InvariantCulture)
                                               let versionName = dir.Name.Substring(NETSTANDARD.Length)
                                               let version = new Version(versionName)
                                               orderby version descending
                                               select dir)
                                            .FirstOrDefault();
                            if (suitableDir is null)
                            {
                                WriteLine("No NetStandard versions found");
                            }
                            else
                            {
                                var suitable = new FileInfo(Combine(suitableDir.FullName, NEWTONSOFT_JSON_DLL));
                                if (!suitable.Exists)
                                {
                                    WriteLine("No suitable version of Newtonsoft.JSON found.");
                                }
                                else
                                {
                                    file = suitable;
                                    WriteLine("Found a suitable version of Newtonsoft.JSON at {0}", file.FullName);
                                }
                            }
                        }
                    }
                }
            }

            return file;
        }
    }
}
