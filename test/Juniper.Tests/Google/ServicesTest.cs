using Juniper.IO;

using NUnit.Framework;

using System.IO;

namespace Juniper.World.GIS.Google
{
    public abstract class ServicesTests
    {
        protected string apiKey { get; set; }
        protected string signingKey { get; set; }
        protected CachingStrategy cache { get; set; }

        [SetUp]
        public virtual void Init()
        {
            var keyFile = Path.Combine(CachingTests.TestRootDir, "DevKeys", "google-streetview.txt");
            var lines = File.ReadAllLines(keyFile);
            apiKey = lines[0];
            signingKey = lines[1];

            var cacheDirName = Path.Combine(CachingTests.TestRootDir, "GoogleMaps");
            var cacheDir = new DirectoryInfo(cacheDirName);
            cache = new CachingStrategy
            {
                new FileCacheLayer(cacheDir)
            };
        }
    }
}