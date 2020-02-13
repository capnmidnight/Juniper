using System;
using System.Linq;

namespace Juniper.ConfigurationManagement
{
    public static class Program
    {
        public static void Main()
        {
            Project.UnityProjectRoot = @"D:\Projects\Juniper\examples\Juniper - Android";
            var packageDB = AbstractPackage.Load();
            var manifest = UnityPackageManifest.Load();
            var platforms = Platforms.Load();

            var pkg = (from versions in packageDB.Values
                       from p in versions
                       where p.Source == PackageSources.Juniper
                         && p.Name.StartsWith("K", StringComparison.OrdinalIgnoreCase)
                       select p)
                               .FirstOrDefault();

            pkg.Install();
            //foreach (var package in platforms.Packages)
            //{
            //    PrintPackageOps(packageDB, manifest, package);
            //}


            //foreach (var configuration in platforms.Configurations.Values)
            //{
            //    WriteLine("============================");
            //    WriteLine(configuration.Name);
            //    foreach (var package in configuration.Packages)
            //    {
            //        PrintPackageOps(packageDB, manifest, package);
            //    }
            //}
        }

        /*
    private static void PrintPackageOps(IReadOnlyDictionary<string, IReadOnlyCollection<AbstractPackage>> packageDB, UnityPackageManifest manifest, PackageReference req)
    {
        Write(req);
        Write(" ");
        WriteLine(Project.IsAvailable(req).ToYesNo("Available"));
        if (req.ForRemoval)
        {
            Write("Removing... ");
            if (!manifest.ContainsKey(req.PackageID))
            {
                WriteLine("no need to remove, not in manifest.");
            }
            else
            {
                WriteLine(manifest[req.PackageID]);
            }
        }
        else
        {
            Write("Adding... ");
            if (!packageDB.ContainsKey(req.PackageID))
            {
                WriteLine("no package of any version found!");
            }
            else
            {
                var match = packageDB[req.PackageID]
                    .OrderByDescending(p => p.CompareTo(req))
                    .FirstOrDefault();

                if (match == req)
                {
                    WriteLine(match);
                }
                else if (match is null)
                {
                    WriteLine("couldn't find a matching version of the package.");
                }
                else if (match < req)
                {
                    WriteLine("couldn't find the package, but an older version already exists in the manifest: " + match);
                }
                else
                {
                    WriteLine("couldn't find the package, but an newer version already exists in the manifest: " + match);
                }
            }

            if (req.Source == PackageSources.UnityPackageManager
                && manifest.ContainsKey(req.PackageID))
            {
                Write("The package exists in the manifest ");

                var match = manifest[req.PackageID];
                if (req == match)
                {
                    WriteLine("and is an exact match.");
                }
                else if (match < req)
                {
                    WriteLine(" but the version in the manifest is older: " + match);
                }
                else
                {
                    WriteLine(" but the version in the manifest is newer: " + match);
                }
            }
        }

        WriteLine();
    }
        */
    }
}
