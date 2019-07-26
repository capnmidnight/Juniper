using System;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.Google.Maps.Tests
{
    public abstract class ServicesTests
    {
        protected Endpoint service;
        protected DirectoryInfo cacheDir;

        [TestInitialize]
        public virtual void Init()
        {
            var myPictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            var cacheDirName = Path.Combine(myPictures, "GoogleMaps");
            cacheDir = new DirectoryInfo(cacheDirName);
            var keyFile = Path.Combine(cacheDirName, "keys.txt");
            var lines = File.ReadAllLines(keyFile);
            var apiKey = lines[0];
            var signingKey = lines[1];
            var json = new Json.JsonFactory();
            service = new Endpoint(json, apiKey, signingKey, cacheDir);
        }
    }
}