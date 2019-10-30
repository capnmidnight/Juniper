using System;
using System.IO;
using System.Linq;

using static System.IO.Path;
using static System.Console;

namespace File_Copier
{
    class Program
    {
        private const string NEWTONSOFT_JSON_DLL = "Newtonsoft.Json.dll";
        private const string NETSTANDARD = "netstandard";

        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Error.WriteLine("Command expects two arguments");
                for (int i = 0; i < args.Length; ++i)
                {
                    Error.WriteLine("{0}: {1}", i, args[i]);
                }
            }
            else
            {
                for (int i = 0; i < args.Length; ++i)
                {
                    if (args[i].EndsWith("\"")
                        || args[i].EndsWith("'"))
                    {
                        args[i] = args[i].Substring(1, args[i].Length - 2);
                    }
                }

                var source = new DirectoryInfo(args[0]);
                if (!source.Exists)
                {
                    Error.WriteLine("Source directory does not exist");
                }
                else if(source.Name != "netstandard2.0")
                {
                    var dest1 = new DirectoryInfo(args[1]);
                    var dest2 = dest1.Parent;

                    WriteLine("Copying from {0} to {1}", source.FullName, dest1.FullName);

                    foreach (var file in source.GetFiles())
                    {
                        var sourceFile = file;
                        if (sourceFile.Name == NEWTONSOFT_JSON_DLL)
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
                                if (maxVersionDir == null)
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
                                                           where dir.Name.StartsWith(NETSTANDARD)
                                                           let versionName = dir.Name.Substring(NETSTANDARD.Length)
                                                           let version = new Version(versionName)
                                                           orderby version descending
                                                           select dir)
                                                        .FirstOrDefault();
                                        if (suitableDir == null)
                                        {
                                            WriteLine("No Netstandard versions found");
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
                                                sourceFile = suitable;
                                                WriteLine("Found a suitable version of Newtonsoft.JSON at {0}", sourceFile.FullName);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        var dest = sourceFile.Name.StartsWith("Juniper")
                            ? dest1
                            : dest2;

                        if (sourceFile.Extension != ".pdb"
                            || dest.Name.EndsWith("Debug"))
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
    }
}