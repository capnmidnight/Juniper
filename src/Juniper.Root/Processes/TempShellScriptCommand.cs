#nullable enable

using Juniper.IO;

namespace Juniper.Processes
{
    public class TempShellScriptCommand : ShellCommand, IDisposable
    {
        public static readonly string TEMP_FILE_PATH_PLACEHOLDER = "Juniper.Processes.TepmShellScriptCommand.FilePathPlaceholder";

        private readonly TempFile tempFile;

        private static TempFile WriteTempFile(string scriptContents, MediaType contentType)
        {
            var tempFile = new TempFile(contentType);
            using var stream = tempFile.Create();
            using var writer = new StreamWriter(stream);
            writer.Write(scriptContents);
            writer.Flush();
            return tempFile;
        }

        private static TempFile WriteTempFile(byte[] data, MediaType contentType)
        {
            var tempFile = new TempFile(contentType);
            using var stream = tempFile.Create();
            stream.Write(data, 0, data.Length);
            return tempFile;
        }

        /// <summary>
        /// Create a new command that executes by reading the contents of a file. 
        /// 
        /// You provide the file contents, the file gets created in a temp location,
        /// and the file gets deleted at the end of the command execution.
        /// </summary>
        /// <param name="workingDir">Set an explicit working directory for the command</param>
        /// <param name="scriptContents">The text content of the temp file</param>
        /// <param name="mediaType">The content type of the temp file</param>
        /// <param name="command">The shell command to execute</param>
        /// <param name="args">Arguments to pass to the shell command. Use <see cref="TEMP_FILE_PATH_PLACEHOLDER"/>
        /// to position the file path to the temp file in the argument list. If <see cref="TEMP_FILE_PATH_PLACEHOLDER"/>
        /// is not provided, the file path will be added to the end of the argument list.</param>
        public TempShellScriptCommand(DirectoryInfo workingDir, string scriptContents, MediaType mediaType, string command, params string[] args)
            : base(workingDir, command, args)
        {
            tempFile = WriteTempFile(scriptContents, mediaType);
            ReplaceFilePathPlaceholder();
        }

        /// <summary>
        /// Create a new command that executes by reading the contents of a file. 
        /// 
        /// You provide the file contents, the file gets created in a temp location,
        /// and the file gets deleted at the end of the command execution.
        /// </summary>
        /// <param name="scriptContents">The text content of the temp file</param>
        /// <param name="mediaType">The content type of the temp file</param>
        /// <param name="command">The shell command to execute</param>
        /// <param name="args">Arguments to pass to the shell command. Use <see cref="TEMP_FILE_PATH_PLACEHOLDER"/>
        /// to position the file path to the temp file in the argument list. If <see cref="TEMP_FILE_PATH_PLACEHOLDER"/>
        /// is not provided, the file path will be added to the end of the argument list.</param>
        public TempShellScriptCommand(string scriptContents, MediaType mediaType, string command, params string[] args)
            : base(command, args)
        {
            tempFile = WriteTempFile(scriptContents, mediaType);
            ReplaceFilePathPlaceholder();
        }

        /// <summary>
        /// Create a new command that executes by reading the contents of a file. 
        /// 
        /// You provide the file contents, the file gets created in a temp location,
        /// and the file gets deleted at the end of the command execution.
        /// </summary>
        /// <param name="workingDir">Set an explicit working directory for the command</param>
        /// <param name="fileContents">The raw binary content of the temp file</param>
        /// <param name="mediaType">The content type of the temp file</param>
        /// <param name="command">The shell command to execute</param>
        /// <param name="args">Arguments to pass to the shell command. Use <see cref="TEMP_FILE_PATH_PLACEHOLDER"/>
        /// to position the file path to the temp file in the argument list. If <see cref="TEMP_FILE_PATH_PLACEHOLDER"/>
        /// is not provided, the file path will be added to the end of the argument list.</param>
        public TempShellScriptCommand(DirectoryInfo workingDir, byte[] fileContents, MediaType mediaType, string command, params string[] args)
            : base(workingDir, command, args)
        {
            tempFile = WriteTempFile(fileContents, mediaType);
            ReplaceFilePathPlaceholder();
        }

        /// <summary>
        /// Create a new command that executes by reading the contents of a file. 
        /// 
        /// You provide the file contents, the file gets created in a temp location,
        /// and the file gets deleted at the end of the command execution.
        /// </summary>
        /// <param name="fileContents">The raw binary content of the temp file</param>
        /// <param name="mediaType">The content type of the temp file</param>
        /// <param name="command">The shell command to execute</param>
        /// <param name="args">Arguments to pass to the shell command. Use <see cref="TEMP_FILE_PATH_PLACEHOLDER"/>
        /// to position the file path to the temp file in the argument list. If <see cref="TEMP_FILE_PATH_PLACEHOLDER"/>
        /// is not provided, the file path will be added to the end of the argument list.</param>
        public TempShellScriptCommand(byte[] fileContents, MediaType mediaType, string command, params string[] args)
            : base(command, args)
        {
            tempFile = WriteTempFile(fileContents, mediaType);
            ReplaceFilePathPlaceholder();
        }

        private void ReplaceFilePathPlaceholder()
        {
            var anyFound = false;
            for(var i = 0; i < args.Count; ++i)
            {
                if (args[i] == TEMP_FILE_PATH_PLACEHOLDER)
                {
                    args[i] = tempFile.FilePath;
                    anyFound = true;
                }
            }

            if (!anyFound)
            {
                args.Add(tempFile.FilePath);
            }
        }

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    tempFile.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
