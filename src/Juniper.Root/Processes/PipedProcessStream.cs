using System.Diagnostics;

namespace Juniper.Processes
{
    public class PipedProcessStream : Stream
    {
        private readonly Process proc;

        public List<string> StdErrorOutput { get; } = new();
        public event EventHandler<StringEventArgs> StdError;

        public PipedProcessStream(string fileName, string args)
        {
            proc = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new()
                {
                    FileName = fileName,
                    Arguments = args,
                    StandardErrorEncoding = System.Text.Encoding.UTF8,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    ErrorDialog = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };
        }

        public bool Start(Stream inputStream)
        {
            var started = proc.Start();
            if (started)
            {
                proc.ErrorDataReceived += (object sender, DataReceivedEventArgs e) =>
                {
                    if (e?.Data is not null)
                    {
                        StdErrorOutput.Add(e.Data);
                        StdError?.Invoke(this, new StringEventArgs(e.Data));
                    }
                };

                proc.BeginErrorReadLine();

                inputStream.CopyTo(proc.StandardInput.BaseStream);
                proc.StandardInput.Flush();
                proc.StandardInput.Close();
            }

            return started;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                proc.Dispose();
            }
        }

        private Stream BaseStream =>
            proc.StandardOutput.BaseStream;

        public override bool CanRead =>
            BaseStream.CanRead;

        public override bool CanSeek =>
            BaseStream.CanSeek;

        public override bool CanWrite =>
            BaseStream.CanWrite;

        public override long Length =>
            BaseStream.Length;

        public override long Position
        {
            get => BaseStream.Position;
            set => BaseStream.Position = value;
        }

        public override void Flush() =>
            BaseStream.Flush();

        public override int Read(byte[] buffer, int offset, int count) =>
            BaseStream.Read(buffer, offset, count);

        public override long Seek(long offset, SeekOrigin origin) =>
            BaseStream.Seek(offset, origin);

        public override void SetLength(long value) =>
            BaseStream.SetLength(value);

        public override void Write(byte[] buffer, int offset, int count) =>
            BaseStream.Write(buffer, offset, count);
    }
}
