using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Juniper.HTTP;
using Juniper.Progress;

using UnityEngine;

namespace Juniper.Data
{
    /// <summary>
    /// Get files out of the Unity StreamingAssets folder.
    /// </summary>
    public static class StreamingAssets
    {
        public static TimeSpan DEFAULT_TTL = TimeSpan.Zero;

        /// <summary>
        /// Parse out the network path.
        /// </summary>
        private const string NetworkPathPatternStr = "^https?://";

        /// <summary>
        /// Parse out the network path.
        /// </summary>
        private static readonly Regex NetworkPathPattern = new Regex(NetworkPathPatternStr, RegexOptions.Compiled);

#if UNITY_ANDROID

        /// <summary>
        /// The pattern to parse out the APK sub-file reference.
        /// </summary>
        private const string AndroidJarPatternStr = @"^jar:file:/([^!]+\.apk)!/(.+)$";

        /// <summary>
        /// The pattern to parse out the APK sub-file reference.
        /// </summary>
        private static readonly Regex AndroidJarPattern = new Regex(AndroidJarPatternStr, RegexOptions.Compiled);

#endif

        private static readonly char[] PATH_SPLIT_ARR = { '/' };

        public static string FormatPath(string streamingAssetsPath, string subPath)
        {
            var parts = streamingAssetsPath.Split(PATH_SPLIT_ARR)
                .Union(subPath.Split(PATH_SPLIT_ARR))
                .ToArray();
            var pathSep = '/';
            if (!NetworkPathPattern.IsMatch(streamingAssetsPath))
            {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WSA || UNITY_IOS
                pathSep = Path.DirectorySeparatorChar;
#elif UNITY_WEBGL
                UnityEngine.Debug.LogWarning(streamingAssetsPath);
#endif
            }

            return parts.Join(pathSep);
        }

        /// <summary>
        /// Open a file as a stream of bytes and save it to a cached location. On subsequent loads,
        /// open from the cached location.
        /// </summary>
        /// <param name="path">The full path to the file in question</param>
        /// <param name="ttl">The maximum age after which to consider a cached file invalidated.</param>
        /// <param name="mime">The mime type of the file, in case we have to request the file over the 'net.</param>
        /// <returns>Progress tracking object</returns>
        public static async Task<Response> GetStream(string cacheDirectory, string path, TimeSpan ttl, string mime, IProgress prog)
        {
#if UNITY_ANDROID
            if (AndroidJarPattern.IsMatch(path))
            {
                var match = AndroidJarPattern.Match(path);
                var apk = match.Groups[1].Value;
                path = match.Groups[2].Value;
                var cachePath = Uri.EscapeUriString(Path.Combine(cacheDirectory, path));
                if (!FileIsGood(cachePath, ttl))
                {
                    Debug.LogWarning("Juniper === Unzipping APK");
                    Debug.LogWarning($"Juniper === APK: {apk}, PATH: {path}, CACHE_PATH: {cachePath}");
                    var subProgs = prog?.Split("Unzipping", "Reading");
                    Compression.Zip.Decompressor.Decompress(apk, cacheDirectory, subProgs[0]);
                }

                return new Response(cachePath, mime, prog);
            }
            else
#endif
            if (NetworkPathPattern.IsMatch(path))
            {
                var uri = new Uri(path);
                var cachePath = Uri.EscapeUriString(Path.Combine(cacheDirectory, uri.PathAndQuery));
                if (FileIsGood(cachePath, ttl))
                {
                    return new Response(cachePath, mime, prog);
                }
                else
                {
                    var requester = HttpWebRequestExt.Create(uri).Accept(mime);
                    return new Response(await requester.Get(), prog);
                }
            }
            else if (File.Exists(path))
            {
                return new Response(path, mime, prog);
            }
            else
            {
                return null;
            }
        }

        public static Task<Response> GetStream(string cacheDirectory, string path, TimeSpan ttl, IProgress prog)
        {
            return GetStream(cacheDirectory, path, ttl, "application/octet-stream", prog);
        }

        public static Task<Response> GetStream(string cacheDirectory, string path, string mime, IProgress prog)
        {
            return GetStream(cacheDirectory, path, DEFAULT_TTL, mime, prog);
        }

        public static Task<Response> GetStream(string cacheDirectory, string path, TimeSpan ttl, string mime)
        {
            return GetStream(cacheDirectory, path, ttl, mime, null);
        }

        public static Task<Response> GetStream(string cacheDirectory, string path, IProgress prog)
        {
            return GetStream(cacheDirectory, path, DEFAULT_TTL, "application/octet-stream", prog);
        }

        public static Task<Response> GetStream(string cacheDirectory, string path, TimeSpan ttl)
        {
            return GetStream(cacheDirectory, path, ttl, "application/octet-stream", null);
        }

        public static Task<Response> GetStream(string cacheDirectory, string path, string mime)
        {
            return GetStream(cacheDirectory, path, DEFAULT_TTL, mime, null);
        }

        public static Task<Response> GetStream(string cacheDirectory, string path)
        {
            return GetStream(cacheDirectory, path, DEFAULT_TTL, "application/octet-stream", null);
        }

        private static bool FileIsGood(string path, TimeSpan ttl)
        {
            return File.Exists(path) && File.GetCreationTime(path) - DateTime.Now <= ttl;
        }
    }
}