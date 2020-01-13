using System;
using System.Linq;

using NUnit.Framework;

namespace Juniper.IO.Tests
{
    [TestFixture]
    public class CachingTests
    {
        [Test]
        public void ListFiles()
        {
            var fileLayer = new FileCacheLayer(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            var files = fileLayer.GetContentReferences(MediaType.Text.Plain)
                .ToArray();
            Assert.AreNotEqual(0, files.Length);
        }

        [Test]
        public void ListMetadatas()
        {
            var fileLayer = new FileCacheLayer(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\GoogleMaps");
            var jsonFiles = fileLayer.GetContentReferences(MediaType.Application.Json)
                .ToArray();
            Assert.AreNotEqual(0, jsonFiles.Length);
        }
    }
}
