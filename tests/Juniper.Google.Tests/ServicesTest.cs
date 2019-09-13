using System;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.Google.Maps.Tests
{
    public abstract class ServicesTests
    {
        protected GoogleMapsRequestConfiguration service;
        protected GoogleMapsRequestConfiguration noCacheService;
        protected DirectoryInfo cacheDir;

        [TestInitialize]
        public virtual void Init()
        {
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var keyFile = Path.Combine(home, "Projects", "DevKeys", "google-streetview.txt");
            var lines = File.ReadAllLines(keyFile);
            var apiKey = lines[0];
            var signingKey = lines[1];

            var myPictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            var cacheDirName = Path.Combine(myPictures, "GoogleMaps");

            cacheDir = new DirectoryInfo(cacheDirName);

            service = new GoogleMapsRequestConfiguration(apiKey, signingKey, cacheDir);
            noCacheService = new GoogleMapsRequestConfiguration(apiKey, signingKey);
        }
    }
}