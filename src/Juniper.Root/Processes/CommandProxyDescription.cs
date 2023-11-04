#nullable enable
using System.Runtime.Serialization;

namespace Juniper.Processes
{
    [Serializable]
    public class CommandProxyDescription : ISerializable
    {
        public int TaskID { get; private set; }
        public string Command { get; private set; }
        public string[] Args { get; private set; }
        public DirectoryInfo? WorkingDir { get; private set; }

        public CommandProxyDescription(string command)
        : this(0, null, command, Array.Empty<string>())
        {
        }

        public CommandProxyDescription(CommandProxyDescription? cmd, string command, params string[] args)
        : this(cmd?.TaskID ?? 0, null, command, args)
        {
        }

        public CommandProxyDescription(int taskID, DirectoryInfo? workingDir, string command, params string[] args)
        {
            TaskID = taskID;
            WorkingDir = workingDir;
            Command = command;
            Args = args;
        }

        private CommandProxyDescription(SerializationInfo info, StreamingContext context)
        {
            var command = info.GetString(nameof(Command))
                ?? throw new InvalidDataException($"Field '{nameof(Command)} not found.");

            TaskID = info.GetInt32(nameof(TaskID));
            var workingDir = info.GetString(nameof(WorkingDir));
            if (workingDir is not null)
            {
                WorkingDir = new(workingDir);
            }
            Command = command;
            Args = info.GetValue<string[]>(nameof(Args))
                ?? Array.Empty<string>();
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
