using NUnit.Framework;
using Assert = NUnit.Framework.Legacy.ClassicAssert;

using System;
using System.IO;
using System.Linq;

namespace Juniper.Compression;

[TestFixture]
public class FileTreeTests
{
    private const string TestFile1 = "test.txt";
    private const string TestFile2 = "test2.txt";

    private static readonly string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    private static readonly string TestTarGZ = Path.Combine(appData, "Unity", "Asset Store-5.x", "Oculus", "ScriptingIntegration", "Oculus Integration.unitypackage");

    [Test]
    public void FindPackages()
    {
        var packages = Tar.GZip.Decompressor.FindUnityPackages(Path.GetDirectoryName(TestTarGZ));
        Assert.IsTrue(packages.Contains(TestTarGZ));
    }

    [Test]
    public void GetEntries()
    {
        Tar.GZip.Decompressor.Entries(TestTarGZ).ToArray();
    }

    [Test]
    public void BuildUnityPackageTree()
    {
        Tar.GZip.Decompressor.Entries(TestTarGZ).Tree();
    }

    [Test]
    public void MakeTree()
    {
        using var mem = new MemoryStream(Juniper.Tests.Properties.Resources.test_zip);
        var tree = Zip.Decompressor.Entries(mem).Tree();
        Assert.IsNotNull(tree);
        Assert.AreEqual(2, tree.Children.Count);
        Assert.IsTrue(tree.Children.Any(v => v.Value.FullName == TestFile1));
        Assert.IsTrue(tree.Children.Any(v => v.Value.FullName == TestFile2));
    }
}