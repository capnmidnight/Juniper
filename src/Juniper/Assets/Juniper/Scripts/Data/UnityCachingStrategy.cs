using System;
using System.IO;

using Juniper.IO;

namespace Juniper.Data
{
    public class UnityCachingStrategy : CachingStrategy
    {
        public static string InUserProfile(string subPath)
        {
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return Path.Combine(userProfile, subPath);
        }

        public static string InAssets(string subPath)
        {
            return Path.Combine("Assets", subPath);
        }

        public UnityCachingStrategy(string fileCachDirectory)
        {
            if (fileCachDirectory != null)
            {
#if UNITY_EDITOR
                var cacheDirName = PathExt.FixPath(fileCachDirectory);
#elif UNITY_ANDROID
                var cacheDirName = UnityEngine.Application.persistentDataPath;
#endif
                var cacheLocation = new DirectoryInfo(cacheDirName);
                var fileCache = new FileCacheLayer(cacheLocation);
                AddLayer(fileCache);
            }

            AddLayer(new StreamingAssetsCacheLayer());
        }

        public UnityCachingStrategy()
            : this(null)
        { }
    }
}
