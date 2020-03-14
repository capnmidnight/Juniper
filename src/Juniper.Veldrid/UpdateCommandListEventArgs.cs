using System;

using Veldrid;

namespace Juniper.VeldridIntegration
{
    public delegate void UpdateCommandListHandler(object sender, UpdateCommandListEventArgs e);

    public class UpdateCommandListEventArgs : EventArgs
    {
        public CommandList CommandList { get; }
        public uint Width { get; set; }
        public uint Height { get; set; }

        public UpdateCommandListEventArgs(CommandList commandList)
        {
            CommandList = commandList;
        }
    }
}
