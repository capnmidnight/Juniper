using System;
using System.IO;

using Juniper.IO;

using NUnit.Framework;

namespace Juniper.World.GIS.Google.Tests
{
    public abstract class ServicesTests
    {
        protected string apiKey { get; set; }
        protected string signingKey { get; set; }
        protected CachingStrategy cache { get; set; }

        [SetUp]
        public virtual void Init()
        {
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var keyFile = Path.Combine(home, "Projects", "DevKeys", "google-streetview.txt");
            var lines = File.ReadAllLines(keyFile);
            apiKey = lines[0];
            signingKey = lines[1];

            var myPictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            var cacheDirName = Path.Combine(myPictures, "GoogleMaps");

            var cacheDir = new DirectoryInfo(cacheDirName);
            var fileCache = new FileCacheLayer(cacheDir);
            cache = new CachingStrategy()
                .AppendLayer(fileCache);
        }
    }
}