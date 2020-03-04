using System;

using Veldrid;

namespace Juniper.VeldridIntegration.WinFormsSupport
{
    public delegate void UpdateCommandListHandler(object sender, UpdateCommandListEventArgs e);

    public class UpdateCommandListEventArgs : EventArgs
    {
        public CommandList CommandList { get; }
        public UpdateCommandListEventArgs(CommandList commandList)
        {
            CommandList = commandList;
        }
    }
}
