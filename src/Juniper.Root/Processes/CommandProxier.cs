using Juniper.IO;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juniper.Processes
{
    public class CommandProxier : IDisposable
    {
        private ShellCommand processManager;
        private bool disposedValue;

        private JsonFactory<CommandProxyDescription> cmdFactory = new();

        public CommandProxier(DirectoryInfo juniperDir)
        {
            if(juniperDir is null)
            {
                throw new ArgumentNullException(nameof(juniperDir));
            }

            processManager = new ShellCommand(juniperDir.CD("src", "Juniper.ProcessManager"), "dotnet", "run");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    
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
