using System;
using System.IO;
using System.Linq;

using Juniper.Caching;

using NUnit.Framework;
using Assert = NUnit.Framework.Legacy.ClassicAssert;

namespace Juniper.IO;

[TestFixture]
public class CachingTests
{
    public static string TestRootDir
    {
        get
        {
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var assetRoot = Path.Combine(userProfile, "Box", "VR Initiatives", "Engineering", "Assets");
            return assetRoot;
        }
    }
    [Test]
    public void ListFiles()
    {
        var fileLayer = new FileCacheLayer(Path.Combine(TestRootDir, "DevKeys"));
        var files = fileLayer.GetContentReferences(MediaType.Text_Plain)
            .ToArray();
        Assert.AreNotEqual(0, files.Length);
    }

    [Test]
    public void ListMetadatas()
    {
        var fileLayer = new FileCacheLayer(Path.Combine(TestRootDir, "GoogleMaps"));
        var jsonFiles = fileLayer.GetContentReferences(MediaType.Application_Json)
            .ToArray();
        Assert.AreNotEqual(0, jsonFiles.Length);
    }
}
