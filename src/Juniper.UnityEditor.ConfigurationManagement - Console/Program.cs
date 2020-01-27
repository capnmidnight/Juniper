using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Juniper.IO;
using static System.Console;

namespace Juniper.ConfigurationManagement
{
    public static class Program
    {
        public static void Main()
        {
            var unityProjectDir = @"C:\Users\smcbeth.DLS-INC\Projects\Yarrow\src\Yarrow - AndroidOculus";

            AbstractPackage2.UnityProjectRoot = unityProjectDir;

            // Zip files
            var juniperPath = Path.Combine(unityProjectDir, "Assets", "Juniper");
            var juniperPlatformsFileName = Path.Combine(juniperPath, "platforms.json");
            var juniperThirdPartyDefinesFileName = Path.Combine(juniperPath, "ThirdParty", "defines.json");

            var configFactory = new JsonFactory<UnityPlatformConfigurations>();

            var packages = new List<AbstractPackage2>();
            UnityAssetStorePackage.GetPackages(packages);
            JuniperZipPackage.GetPackages(packages, unityProjectDir);
            UnityPackageManagerPackage.GetPackages(packages);




            var packageDB = (from package in packages
                             group package by package.Name into grp
                             orderby grp.Key
                             select grp);

            foreach (var group in packageDB)
            {
                WriteLine(group.Key);
                foreach (var version in group)
                {
                    var installed = version.IsInstalled;
                    var avail = version.Available.ToYesNo(nameof(version.Available));
                    var cached = version.Cached.ToYesNo(nameof(version.Cached));

                    WriteLine($"\t[{version.Version ?? "N/A"} {installed} {version.Source} {avail}, {cached}]:> {version.ContentPath}");
                }
            }
        }
    }

    public class UnityPlatformConfigurations
    {

    }
}
