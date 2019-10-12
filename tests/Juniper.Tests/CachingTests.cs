using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.IO.Tests
{
    [TestClass]
    public class CachingTests
    {
        [TestMethod]
        public void ListFiles()
        {
            var fileLayer = new FileCacheLayer(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            var files = fileLayer.Get(MediaType.Text.Plain)
                .ToArray();
            Assert.AreNotEqual(0, files.Length);
        }

        [TestMethod]
        public void ListMetadatas()
        {
            var fileLayer = new FileCacheLayer(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\GoogleMaps");
            var jsonFiles = fileLayer.Get(MediaType.Application.Json)
                .ToArray();
            Assert.AreNotEqual(0, jsonFiles.Length);
        }
    }
}
