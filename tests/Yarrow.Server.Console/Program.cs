using System;
using System.IO;

namespace Yarrow.Server.Console
{
    internal class Program
    {
        private static void Main(string[] args)
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
                    args,
                    System.Console.WriteLine,
                    System.Console.WriteLine,
                    System.Console.Error.WriteLine,
                    apiKey,
                    signingKey,
                    cacheDir);
                server.Start();
            }
        }
    }
}