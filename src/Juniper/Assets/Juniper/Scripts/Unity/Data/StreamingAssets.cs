using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Juniper.HTTP;
using Juniper.Progress;

#if UNITY_ANDROID

using Juniper.Data;

#endif

namespace Juniper.Data
{
    /// <summary>
    /// Get files out of the Unity StreamingAssets folder.
    /// </summary>
    public static class StreamingAssets
    {
        public static TimeSpan DEFAULT_TTL = TimeSpan.Zero;

        public static string FormatPath(string streamingAssetsPath, string dataPath, string subPath)
        {
            var parts = new List<string>(4);
            var pathSep = '/';
            if (!NetworkPathPattern.IsMatch(subPath))
            {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WSA
                pathSep = Path.DirectorySeparatorChar;
                parts.Add(dataPath);
                parts.Add("StreamingAssets");
#elif UNITY_ANDROID || PLATFORM_LUMIN
                parts.Add("jar:file:/");
                parts.Add(dataPath + "!");
                parts.Add("assets");
#elif UNITY_IOS
                pathSep = Path.DirectorySeparatorChar;
                parts.Add(dataPath);
                parts.Add("Raw");
#elif UNITY_WEBGL
                UnityEngine.Debug.Log(dataPath);
                parts.Add(dataPath);
                parts.Add("StreamingAssets");
#endif
            }

            parts.Add(subPath);
            var path = parts.ToArray().Join(pathSep);
            UnityEngine.Debug.Log(streamingAssetsPath);
            UnityEngine.Debug.Log(path);
            return path;
        }

        /// <summary>
        /// Open a file as a stream of bytes and save it to a cached location. On subsequent loads,
        /// open from the cached location.
        /// </summary>
        /// <param name="path">The full path to the file in question</param>
        /// <param name="ttl">The maximum age after which to consider a cached file invalidated.</param>
        /// <param name="mime">The mime type of the file, in case we have to request the file over the 'net.</param>
        /// <returns>Progress tracking object</returns>
        public static async Task<StreamResult> GetStream(string cacheDirectory, string path, TimeSpan ttl, string mime, IProgress prog = null)
        {
            prog?.Report(0);
            try
            {
                if (NetworkPathPattern.IsMatch(path))
                {
                    var uri = new Uri(path);
                    var cachePath = Uri.EscapeUriString(Path.Combine(cacheDirectory, uri.PathAndQuery));
                    if (FileIsGood(cachePath, ttl))
                    {
                        return new StreamResult(HttpStatusCode.OK, mime, File.OpenRead(cachePath));
                    }
                    else
                    {
                        var result = await Requester.GetStream(uri.ToString(), mime, prog);
                        return new StreamResult(result.Status, result.MIMEType, new CachingStream(result.Value, cachePath));
                    }
                }
#if UNITY_ANDROID
                else if (AndroidJarPattern.IsMatch(path))
                {
                    var match = AndroidJarPattern.Match(path);
                    var apk = match.Groups[1].Value;
                    path = match.Groups[2].Value;
                    var cachePath = Uri.EscapeUriString(Path.Combine(cacheDirectory, path));
                    if (FileIsGood(cachePath, ttl))
                    {
                        return new StreamResult(HttpStatusCode.OK, mime, File.OpenRead(cachePath));
                    }
                    else
                    {
                        var stream = Zip.Decompressor.GetFile(apk, path, prog);
                        return new StreamResult(HttpStatusCode.OK, mime, new CachingStream(stream, cachePath));                        
                    }
                }
#endif
                else if (File.Exists(path))
                {
                    return new StreamResult(HttpStatusCode.OK, mime, File.OpenRead(path));
                }
                else
                {
                    throw new FileNotFoundException("Could not find file " + path, path);
                }
            }
            finally
            {
                prog?.Report(1);
            }
        }

        public static Task<StreamResult> GetStream(string cacheDirectory, string path, TimeSpan ttl, IProgress prog = null)
        {
            return GetStream(cacheDirectory, path, ttl, "application/octet-stream", prog);
        }

        public static Task<StreamResult> GetStream(string cacheDirectory, string path, string mime, IProgress prog = null)
        {
            return GetStream(cacheDirectory, path, DEFAULT_TTL, mime, prog);
        }

        public static Task<StreamResult> GetStream(string cacheDirectory, string path, IProgress prog = null)
        {
            return GetStream(cacheDirectory, path, DEFAULT_TTL, "application/octet-stream", prog);
        }

        /// <summary>
        /// Open a file as an array of bytes.
        /// </summary>
        /// <param name="path">   The full path to the file in question</param>
        /// <param name="ttl">    The maximum age after which to consider a cached file invalidated.</param>
        /// <param name="mime">   
        /// The mime type of the file, in case we have to request the file over the 'net.
        /// </param>
        /// <param name="resolve">A callback to receive the file asynchronously.</param>
        /// <param name="reject"> A callback for when there is an error.</param>
        /// <returns>Progress tracking object</returns>
        public static async Task<Result<byte[]>> GetBytes(string cacheDirectory, string path, TimeSpan ttl, string mime, IProgress prog = null)
        {
            var result = await GetStream(cacheDirectory, path, ttl, mime, prog);
            return new Result<byte[]>(result.Status, result.MIMEType, result.Value.ReadBytes());
        }

        public static Task<Result<byte[]>> GetBytes(string cacheDirectory, string path, TimeSpan ttl, IProgress prog = null)
        {
            return GetBytes(cacheDirectory, path, ttl, "application/octet-stream", prog);
        }

        public static Task<Result<byte[]>> GetBytes(string cacheDirectory, string path, string mime, IProgress prog = null)
        {
            return GetBytes(cacheDirectory, path, DEFAULT_TTL, mime, prog);
        }

        public static Task<Result<byte[]>> GetBytes(string cacheDirectory, string path, IProgress prog = null)
        {
            return GetBytes(cacheDirectory, path, DEFAULT_TTL, "application/octet-stream", prog);
        }

        /// <summary>
        /// Open a file as a text string
        /// </summary>
        /// <param name="path">   The full path to the file in question</param>
        /// <param name="ttl">    The maximum age after which to consider a cached file invalidated.</param>
        /// <param name="mime">   
        /// The mime type of the file, in case we have to request the file over the 'net.
        /// </param>
        /// <returns>Progress tracking object</returns>
        public static async Task<Result<string>> GetText(string cacheDirectory, string path, TimeSpan ttl, IProgress prog = null)
        {
            var result = await GetStream(cacheDirectory, path, ttl, "text/plain", prog);
            return new Result<string>(result.Status, result.MIMEType, result.Value.ReadString());
        }

        public static Task<Result<string>> GetText(string cacheDirectory, string path, IProgress prog = null)
        {
            return GetText(cacheDirectory, path, DEFAULT_TTL, prog);
        }

        /// <summary>
        /// Open a file as a deserialized JSON object.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to</typeparam>
        /// <param name="path">   The full path to the file in question</param>
        /// <param name="ttl">    The maximum age after which to consider a cached file invalidated.</param>
        /// <param name="mime">   
        /// The mime type of the file, in case we have to request the file over the 'net.
        /// </param>
        /// <returns>Progress tracking object</returns>
        public static async Task<Result<T>> GetJSONObject<T>(string cacheDirectory, string path, TimeSpan ttl, IProgress prog = null)
        {
            var result = await GetStream(cacheDirectory, path, ttl, "application/json", prog);
            return new Result<T>(result.Status, result.MIMEType, result.Value.ReadObject<T>());
        }

        public static Task<Result<T>> GetJSONObject<T>(string cacheDirectory, string path, IProgress prog = null)
        {
            return GetJSONObject<T>(cacheDirectory, path, DEFAULT_TTL, prog);
        }

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

        private static bool FileIsGood(string path, TimeSpan ttl)
        {
            return File.Exists(path) && File.GetCreationTime(path) - DateTime.Now <= ttl;
        }
    }
}
