using System;
using System.IO;
using System.Runtime.Serialization;

namespace Juniper.Processes
{
    [Serializable]
    public class CommandProxyDescription : ISerializable
    {
        public int TaskID { get; private set; }
        public string Command { get; private set; }
        public string[] Args { get; private set; }
        public DirectoryInfo WorkingDir { get; private set; }

        public CommandProxyDescription() { }

        public CommandProxyDescription(CommandProxyDescription cmd, string command, params string[] args)
        {
            TaskID = cmd?.TaskID ?? 0;
            Command = command;
            Args = args;
        }

        public CommandProxyDescription(int taskID, DirectoryInfo workingDir, string command, params string[] args)
        {
            TaskID = taskID;
            WorkingDir = workingDir;
            Command = command;
            Args = args;
        }

        private CommandProxyDescription(SerializationInfo info, StreamingContext context)
        {
            TaskID = info.GetInt32(nameof(TaskID));
            var workingDir = info.GetString(nameof(WorkingDir));
            if (workingDir is not null)
            {
                WorkingDir = new(workingDir);
            }
            Command = info.GetString(nameof(Command));
            Args = info.GetValue<string[]>(nameof(Args));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(TaskID), TaskID);
            info.AddValue(nameof(WorkingDir), WorkingDir?.FullName);
            info.AddValue(nameof(Command), Command);
            info.AddValue(nameof(Args), Args);
        }
    }
}
