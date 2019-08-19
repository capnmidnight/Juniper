using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Juniper.HTTP;
using Juniper.Imaging;
using Juniper.Progress;
using Juniper.Serialization;
using Juniper.Streams;

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
                UnityEngine.Debug.Log(streamingAssetsPath);
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
        public static async Task<Response> GetStream(string cacheDirectory, string path, TimeSpan ttl, string mime, IProgress prog = null)
        {
#if UNITY_ANDROID
            if (AndroidJarPattern.IsMatch(path))
            {
                var match = AndroidJarPattern.Match(path);
                var apk = match.Groups[1].Value;
                path = match.Groups[2].Value;
                var cachePath = Uri.EscapeUriString(Path.Combine(cacheDirectory, path));
                if (FileIsGood(cachePath, ttl))
                {
                    return new Response(cachePath, mime, prog);
                }
                else
                {
                    var subProgs = prog?.Split("Unzipping", "Reading");
                    var stream = Compression.Zip.Decompressor.GetFile(apk, path, subProgs[0]);
                    return new Response(new CachingStream(stream, cachePath), HttpStatusCode.OK, mime, stream.Length, subProgs[1]);
                }
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
                return new Response(path, mime);
            }
            else
            {
                return null;
            }
        }

        public static Task<Response> GetStream(string cacheDirectory, string path, TimeSpan ttl, IProgress prog = null)
        {
            return GetStream(cacheDirectory, path, ttl, "application/octet-stream", prog);
        }

        public static Task<Response> GetStream(string cacheDirectory, string path, string mime, IProgress prog = null)
        {
            return GetStream(cacheDirectory, path, DEFAULT_TTL, mime, prog);
        }

        public static Task<Response> GetStream(string cacheDirectory, string path, IProgress prog = null)
        {
            return GetStream(cacheDirectory, path, DEFAULT_TTL, "application/octet-stream", prog);
        }

        private static bool FileIsGood(string path, TimeSpan ttl)
        {
            return File.Exists(path) && File.GetCreationTime(path) - DateTime.Now <= ttl;
        }

        /// <summary>
        /// Gets a texture from disk or the Internet, running in a background thread, with a
        /// coroutine waiting on the thread to finish.
        /// </summary>
        /// <returns>The texture from streaming assets PNGC coroutine.</returns>
        /// <param name="imagePath">Image path.</param>
        /// <param name="resolve">  Resolve.</param>
        /// <param name="reject">   Reject.</param>
        public static async Task<T> GetImage<T>(IImageDecoder<T> decoder, string cacheDirectory, string imagePath, IProgress prog = null)
        {
            var subProgs = prog?.Split("Read", "Deserialize");
            using (var imageFile = await StreamingAssets.GetStream(
                cacheDirectory,
                imagePath,
                ImageData.GetContentType(decoder.Format),
                subProgs[0]))
            {
                if (imageFile != null
                    && imageFile.StatusCode == HttpStatusCode.OK
                    && imageFile.ContentLength > 0
                    && imageFile.Content != null)
                {
                    return decoder.Deserialize(imageFile.Content, imageFile.ContentLength, subProgs[1]);
                }
                else
                {
                    return default;
                }
            }
        }

        /// <summary>
        /// Gets a texture from disk or the Internet, running in a background thread, with a
        /// coroutine waiting on the thread to finish.
        /// </summary>
        /// <returns>The texture from streaming assets PNGC coroutine.</returns>
        /// <param name="imagePath">Image path.</param>
        /// <param name="resolve">  Resolve.</param>
        /// <param name="reject">   Reject.</param>
        public static async Task<T> ReadImage<T>(IImageDecoder<T> decoder, string cacheDirectory, string imagePath, IProgress prog = null)
        {
            var subProgs = prog?.Split("Read", "Deserialize");
            using (var imageFile = await StreamingAssets.GetStream(
                cacheDirectory,
                imagePath,
                ImageData.GetContentType(decoder.Format),
                subProgs[0]))
            {
                if (imageFile != null
                   && imageFile.StatusCode == HttpStatusCode.OK
                   && imageFile.ContentLength > 0
                   && imageFile.Content != null)
                {
                    return decoder.Read(imageFile.Content, imageFile.ContentLength, subProgs[1]);
                }
                else
                {
                    return default;
                }
            }
        }
    }
}