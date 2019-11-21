using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Juniper.HTTP
{
    public class DefaultFileController
    {
        private static readonly string[] INDEX_FILES = {
            "index.html",
            "index.htm",
            "default.html",
            "default.htm"
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

        private static string MassageRequestPath(string requestPath)
        {
            requestPath = requestPath.Substring(1);

            if (requestPath.Length > 0 && requestPath[requestPath.Length - 1] == '/')
            {
                requestPath = requestPath.Substring(0, requestPath.Length - 1);
            }

            requestPath = requestPath.Replace('/', Path.DirectorySeparatorChar);
            return requestPath;
        }

        private readonly string rootDirectory;

        public event EventHandler<string> Warning;
        private void OnWarning(string message)
        {
            Warning?.Invoke(this, message);
        }

        public DefaultFileController(string rootDirectory)
        {
            this.rootDirectory = rootDirectory;
        }

        [Route(".*", Priority = int.MaxValue)]
        public Task ServeFile(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;
            var requestPath = request.Url.AbsolutePath;
            var requestFile = MassageRequestPath(requestPath);
            var filename = Path.Combine(rootDirectory, requestFile);
            var isDirectory = Directory.Exists(filename);

            if (isDirectory)
            {
                filename = FindDefaultFile(filename);
            }

            var file = new FileInfo(filename);
            var shortName = MakeShortName(rootDirectory, filename);

            if (isDirectory && requestPath[requestPath.Length - 1] != '/')
            {
                response.Redirect(requestPath + "/");
            }
            else if (file.Exists)
            {
                try
                {
                    response.SendFile(file);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception exp)
                {
                    var message = $"ERRRRRROR: '{shortName}' > {exp.Message}";
                    OnWarning(message);
                    response.Error(HttpStatusCode.InternalServerError, message);
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }
            else
            {
                var message = $"request '{shortName}'";
                OnWarning(message);
                response.Error(HttpStatusCode.NotFound, message);
            }

            return Task.CompletedTask;
        }
    }
}