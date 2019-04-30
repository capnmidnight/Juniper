using Juniper.Data;
using Juniper.Progress;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

#if UNITY_ANDROID

using Juniper.Data;

#endif

namespace Juniper.Unity.Data
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
        /// <param name="path">   The full path to the file in question</param>
        /// <param name="ttl">    The maximum age after which to consider a cached file invalidated.</param>
        /// <param name="mime">   
        /// The mime type of the file, in case we have to request the file over the 'net.
        /// </param>
        /// <param name="resolve">A callback to receive the file stream asynchronously.</param>
        /// <param name="reject"> A callback for when there is an error.</param>
        /// <returns>Progress tracking object</returns>
        public static async void GetCachedFile(string cacheDirectory, string path, TimeSpan ttl, string mime, Action<string> resolve, Action<Exception> reject, IProgress prog = null)
        {
            prog?.Report(0);

            Action<string> progResolve = (string resolvePath) =>
            {
                prog?.Report(1);
                resolve(resolvePath);
            };

            try
            {
                if (NetworkPathPattern.IsMatch(path))
                {
                    var uri = new Uri(path);
                    GetOrCacheFile(
                        cacheDirectory,
                        uri.PathAndQuery,
                        ttl,
                        async streamResolve =>
                        {
                            try
                            {
                                using (var result = await HTTP.GetStream(path, mime, prog))
                                {
                                    var reader = new StreamReader(result.Value);
                                    resolve(reader.ReadToEnd());
                                }
                            }
                            catch(Exception exp)
                            {
                                reject(exp);
                            }
                        },
                        progResolve,
                        prog);
                }
#if UNITY_ANDROID
                else if (AndroidJarPattern.IsMatch(path))
                {
                    var match = AndroidJarPattern.Match(path);
                    var apk = match.Groups[1].Value;
                    path = match.Groups[2].Value;
                    GetOrCacheFile(
                        cacheDirectory,
                        path,
                        ttl,
                        streamResolve => Zip.GetFile(apk, path, streamResolve, reject, prog),
                        progResolve,
                        prog);
                }
#endif
                else if (File.Exists(path))
                {
                    progResolve(path);
                }
                else
                {
                    throw new FileNotFoundException("Could not find file " + path, path);
                }
            }
            catch (Exception exp)
            {
                reject(exp);
            }
        }

        public static void GetCachedFile(string cacheDirectory, string path, TimeSpan ttl, Action<string> resolve, Action<Exception> reject, IProgress prog = null)
        {
            GetCachedFile(cacheDirectory, path, ttl, "application/octet-stream", resolve, reject, prog);
        }

        public static void GetCachedFile(string cacheDirectory, string path, string mime, Action<string> resolve, Action<Exception> reject, IProgress prog = null)
        {
            GetCachedFile(cacheDirectory, path, DEFAULT_TTL, mime, resolve, reject, prog);
        }

        public static void GetCachedFile(string cacheDirectory, string path, Action<string> resolve, Action<Exception> reject, IProgress prog = null)
        {
            GetCachedFile(cacheDirectory, path, DEFAULT_TTL, "application/octet-stream", resolve, reject, prog);
        }

        /// <summary>
        /// Open a file as an stream of bytes.
        /// </summary>
        /// <param name="path">   The full path to the file in question</param>
        /// <param name="ttl">    The maximum age after which to consider a cached file invalidated.</param>
        /// <param name="mime">   
        /// The mime type of the file, in case we have to request the file over the 'net.
        /// </param>
        /// <param name="resolve">A callback to receive the file stream asynchronously.</param>
        /// <param name="reject"> A callback for when there is an error.</param>
        /// <returns>Progress tracking object</returns>
        public static void GetStream(string cacheDirectory, string path, TimeSpan ttl, string mime, Action<Stream> resolve, Action<Exception> reject, IProgress prog = null)
        {
            GetCachedFile(cacheDirectory, path, ttl, mime, cachedPath => resolve(File.OpenRead(cachedPath)), reject, prog);
        }

        public static void GetStream(string cacheDirectory, string path, TimeSpan ttl, Action<Stream> resolve, Action<Exception> reject, IProgress prog = null)
        {
            GetStream(cacheDirectory, path, ttl, null, resolve, reject, prog);
        }

        public static void GetStream(string cacheDirectory, string path, string mime, Action<Stream> resolve, Action<Exception> reject, IProgress prog = null)
        {
            GetStream(cacheDirectory, path, DEFAULT_TTL, mime, resolve, reject, prog);
        }

        public static void GetStream(string cacheDirectory, string path, Action<Stream> resolve, Action<Exception> reject, IProgress prog = null)
        {
            GetStream(cacheDirectory, path, DEFAULT_TTL, null, resolve, reject, prog);
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
        public static void GetBytes(string cacheDirectory, string path, TimeSpan ttl, string mime, Action<byte[]> resolve, Action<Exception> reject, IProgress prog = null)
        {
            GetStream(cacheDirectory, path, ttl, mime, stream => resolve(stream.ReadBytes()), reject, prog);
        }

        public static void GetBytes(string cacheDirectory, string path, TimeSpan ttl, Action<byte[]> resolve, Action<Exception> reject, IProgress prog = null)
        {
            GetBytes(cacheDirectory, path, ttl, "application/octet-stream", resolve, reject, prog);
        }

        public static void GetBytes(string cacheDirectory, string path, string mime, Action<byte[]> resolve, Action<Exception> reject, IProgress prog = null)
        {
            GetBytes(cacheDirectory, path, DEFAULT_TTL, mime, resolve, reject, prog);
        }

        public static void GetBytes(string cacheDirectory, string path, Action<byte[]> resolve, Action<Exception> reject, IProgress prog = null)
        {
            GetBytes(cacheDirectory, path, DEFAULT_TTL, "application/octet-stream", resolve, reject, prog);
        }

        /// <summary>
        /// Open a file as a text string
        /// </summary>
        /// <param name="path">   The full path to the file in question</param>
        /// <param name="ttl">    The maximum age after which to consider a cached file invalidated.</param>
        /// <param name="mime">   
        /// The mime type of the file, in case we have to request the file over the 'net.
        /// </param>
        /// <param name="resolve">A callback to receive the file asynchronously.</param>
        /// <param name="reject"> A callback for when there is an error.</param>
        /// <returns>Progress tracking object</returns>
        public static void GetText(string cacheDirectory, string path, TimeSpan ttl, Action<string> resolve, Action<Exception> reject, IProgress prog = null)
        {
            GetStream(cacheDirectory, path, ttl, "text/plain", stream => resolve(stream.ReadString()), reject, prog);
        }

        public static void GetText(string cacheDirectory, string path, Action<string> resolve, Action<Exception> reject, IProgress prog = null)
        {
            GetText(cacheDirectory, path, DEFAULT_TTL, resolve, reject, prog);
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
        /// <param name="resolve">A callback to receive the file asynchronously.</param>
        /// <param name="reject"> A callback for when there is an error.</param>
        /// <returns>Progress tracking object</returns>
        public static void GetJSONObject<T>(string cacheDirectory, string path, TimeSpan ttl, Action<T> resolve, Action<Exception> reject, IProgress prog = null)
        {
            GetStream(cacheDirectory, path, ttl, "application/json", stream => resolve(stream.ReadObject<T>()), reject, prog);
        }

        public static void GetJSONObject<T>(string cacheDirectory, string path, Action<T> resolve, Action<Exception> reject, IProgress prog = null)
        {
            GetJSONObject(cacheDirectory, path, DEFAULT_TTL, resolve, reject, prog);
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

        private static void GetOrCacheFile(string cacheDirectory, string fileName, TimeSpan ttl, Action<Action<Stream>> getStreamResolver, Action<string> resolve, IProgress prog = null)
        {
            prog?.Report(0);

            Action<string> progResolve = (string resolvePath) =>
            {
                prog?.Report(1);
                resolve(resolvePath);
            };

            var cachePath = Uri.EscapeUriString(Path.Combine(cacheDirectory, fileName));

            if (FileIsGood(cachePath, ttl))
            {
                progResolve(cachePath);
            }
            else
            {
                var dir = Path.GetDirectoryName(cachePath);
                getStreamResolver(stream =>
                {
                    DirectoryExt.CreateDirectory(dir);
                    var fileInfo = new FileInfo(cachePath);
                    using (var file = new ProgressStream(new FileStream(cachePath, FileMode.Create), fileInfo.Length, prog))
                    {
                        stream.CopyTo(file);
                    }
                    progResolve(cachePath);
                });
            }
        }
    }
}
