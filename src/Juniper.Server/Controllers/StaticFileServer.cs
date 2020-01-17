using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using static Juniper.Logic.LogicConstructor;

namespace Juniper.HTTP.Server.Controllers
{
    public class StaticFileServer : AbstractResponse
    {
        private static readonly string[] INDEX_FILES = {
            "index.html",
            "index.htm",
            "default.html",
            "default.htm"
        };

        private static readonly MediaType[] DEFAULT_MEDIA_TYPES =
        {
            MediaType.Application.Javascript,
            MediaType.Application.Json,
            MediaType.Application.Xml,
            MediaType.Text.Html,
            MediaType.Text.Css,
            MediaType.Text.Plain,
            MediaType.Text.Xml,
            MediaType.Image.Png,
            MediaType.Image.Jpeg,
            MediaType.Image.Gif,
            MediaType.Image.SvgXml
        };

        private static string MakeShortName(string rootDirectory, string filename)
        {
            var shortName = filename.Replace(rootDirectory, "");
            if (shortName.Length > 0 && shortName[0] == Path.DirectorySeparatorChar)
            {
                shortName = shortName.Substring(1);
            }

            return shortName;
        }

        private static string FindDefaultFile(string filename)
        {
            if (Directory.Exists(filename))
            {
                for (var i = 0; i < INDEX_FILES.Length; ++i)
                {
                    var test = Path.Combine(filename, INDEX_FILES[i]);
                    if (File.Exists(test))
                    {
                        filename = test;
                        break;
                    }
                }
            }

            return filename;
        }

        private static DirectoryInfo ValidateDirectoryPath(string rootDirectoryPath)
        {
            if (rootDirectoryPath is null)
            {
                throw new ArgumentNullException(nameof(rootDirectoryPath));
            }

            return new DirectoryInfo(rootDirectoryPath);
        }

        private static IReadOnlyList<MediaType> ValidateAcceptTypes(IReadOnlyList<MediaType> acceptTypes)
        {
            if (acceptTypes is null
                || acceptTypes.Count == 0)
            {
                acceptTypes = DEFAULT_MEDIA_TYPES;
            }

            return acceptTypes;
        }

        private static string MassageRequestPath(string requestPath)
        {
            requestPath = requestPath.Substring(1);

            if (requestPath.Length > 0 && requestPath[requestPath.Length - 1] == '/')
            {
                requestPath = requestPath.Substring(0, requestPath.Length - 1);
            }

            return requestPath.Replace('/', Path.DirectorySeparatorChar);
        }

        private readonly DirectoryInfo rootDirectory;

        public StaticFileServer(DirectoryInfo rootDirectory, params MediaType[] acceptTypes)
            : base(int.MaxValue - 2,
                HttpProtocols.Default,
                HttpMethods.GET,
                AllRoutes,
                AllAuthSchemes,
                Or(acceptTypes),
                Or(HttpStatusCode.Continue, HttpStatusCode.NotFound),
                NoHeaders)
        {
            if (rootDirectory is null)
            {
                throw new ArgumentNullException(nameof(rootDirectory));
            }

            if (!rootDirectory.Exists)
            {
                throw new InvalidOperationException($"Directory {rootDirectory.FullName} does not exist");
            }

            this.rootDirectory = rootDirectory;
        }

        public StaticFileServer(string rootDirectoryPath, params MediaType[] mediaTypeWhiteList)
            : this(ValidateDirectoryPath(rootDirectoryPath),
                  mediaTypeWhiteList)
        { }

        protected override async Task InvokeAsync(HttpListenerContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var response = context.Response;
            var request = context.Request;
            var requestPath = request.Url.AbsolutePath;
            var requestFile = MassageRequestPath(requestPath);
            var fileName = Path.Combine(rootDirectory.FullName, requestFile);
            var directory = new DirectoryInfo(fileName);

            if (directory.Exists)
            {
                fileName = FindDefaultFile(fileName);
            }

            var file = new FileInfo(fileName);
            
            if (!IsAcceptable(request)
                || (!file.Exists && !directory.Exists)
                || (file.Exists && !rootDirectory.Contains(file))
                || (directory.Exists && !rootDirectory.Contains(directory)))
            {
                response.SetStatus(HttpStatusCode.NotFound);
                await response.SendTextAsync(MediaType.Text.Plain, "Not found")
                    .ConfigureAwait(false);
                OnWarning(NCSALogger.FormatLogMessage(context));
            }
            else if (file.Exists)
            {
                await response.SendFileAsync(file)
                   .ConfigureAwait(false);
            }
            else
            {
                await ListDirectoryAsync(response, directory)
                    .ConfigureAwait(false);
            }
        }

        private async Task ListDirectoryAsync(HttpListenerResponse response, DirectoryInfo dir)
        {
            var sb = new StringBuilder();
            var shortName = MakeShortName(rootDirectory.FullName, dir.FullName);
            _ = sb.Append("<!DOCTYPE html><html><head><meta charset=\"utf-8\"><title>")
                .Append(shortName)
                .Append("</title></head><body><h1>Directory Listing: ")
                .Append(shortName)
                .Append("</h1><ul>");

            var paths = from subPath in dir.GetFileSystemInfos()
                        select MakeShortName(dir.FullName, subPath.FullName);

            if (string.CompareOrdinal(dir.Parent.FullName, rootDirectory.FullName) == 0)
            {
                paths = paths.Prepend("..");
            }

            foreach (var subPath in paths)
            {
                _ = sb.Append("<li><a href=\"")
                  .Append(subPath)
                  .Append("\">")
                  .Append(subPath)
                  .Append("</a></li>");
            }

            _ = sb.Append("</ul></body></html>");

            response.SetStatus(HttpStatusCode.OK);
            await response.SendTextAsync(MediaType.Text.Html, sb.ToString())
                .ConfigureAwait(false);
        }
    }
}