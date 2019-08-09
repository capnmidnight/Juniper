using System;
using System.IO;

namespace Yarrow.Server.Console
{
    internal class Program
    {
        private static void Main()
        {
            var myPictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            var cacheDirName = Path.Combine(myPictures, "GoogleMaps");
            var cacheDir = new DirectoryInfo(cacheDirName);
            var keyFileName = Path.Combine(cacheDirName, "keys.txt");
            var keyFile = new FileInfo(keyFileName);
            using (var fileStream = keyFile.OpenRead())
            using (var reader = new StreamReader(fileStream))
            {
                var apiKey = reader.ReadLine();
                var signingKey = reader.ReadLine();
                var server = new YarrowServer(
                    80, 443,
                    System.Console.WriteLine,
                    msg => System.Console.WriteLine($"WARNING: {msg}"),
                    System.Console.Error.WriteLine,
                    apiKey, signingKey,
                    cacheDir);
                server.Start();
            }
        }
    }
}