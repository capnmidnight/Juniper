#nullable enable

using System.Net.Sockets;
using System.Text;

namespace Juniper.Processes
{
    public enum SSHProtocol
    {
        [Obsolete(
@"SSH protocol 1 suffers from a number of cryptographic weaknesses and doesn't
support many of the advanced features available for protocol 2. Protocol 1 
should not be used and is only offered to support legacy devices.")]
        SSHProtocol1 = 1,

        SSHProtocol2 = 2
    }

    public enum SCPOutputMode
    {
        Quite,
        Verbose
    }

    public class SCPFileSpec
    {
        public string? UserName { get; set; }
        public string? Host { get; set; }
        public string PathSpec { get; set; }

        public SCPFileSpec(string? userName, string? host, string pathSpec)
        {
            UserName = userName;
            Host = host;
            PathSpec = pathSpec;
        }

        public SCPFileSpec(string? host, string pathSpec)
            : this(null, host, pathSpec)
        {
        }

        public SCPFileSpec(string pathSpec)
            : this(null, null, pathSpec)
        {
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (Host is not null)
            {
                if (UserName is not null)
                {
                    sb.Append($"{UserName}@");
                }
                sb.Append($"{Host}:");
            }

            sb.Append(PathSpec);

            return sb.ToString();
        }
    }

    public class SCPOptions
    {
        /// <summary>
        /// If protocol is not set, the default used is SSH Protocol 2.
        /// </summary>
        public SSHProtocol? Protocol { get; set; }

        /// <summary>
        /// When using SCP to transmit files between two remote machines,
        /// setting this property to "true" routes the files through
        /// the local machine.
        /// </summary>
        public bool ThroughLocal { get; set; }

        /// <summary>
        /// Forces SCP to use <code>AddressFamily.InterNetwork</code> or 
        /// <code>AddressFamily.InterNetworkV6</code>. Any other value
        /// from the AddressFamiliy enumeration is not supported.
        /// </summary>
        public AddressFamily? AddressFamily { get; set; }

        /// <summary>
        /// Selects batch mode (prevents asking for passwords or passphrases).
        /// </summary>
        public bool BatchMode { get; set; }

        /// <summary>
        /// Compression enable. Passes the -C flag to ssh(1) to enable
        /// compression.
        /// </summary>
        public bool EnableCompression { get; set; }

        /// <summary>
        /// Preserves modification times, access times, and modes from the
        /// original file.
        /// </summary>
        public bool PreserveModificationTimes { get; set; }

        public SCPOutputMode? OutputMode { get; set; }

        public bool RecursivelyCopyDirectories { get; set; }

        public string? Cipher { get; set; }

        public FileInfo? SSHConfig { get; set; }

        public FileInfo? IdentityFile { get; set; }

        public string? SSHOption { get; set; }

        public int? LimitKilobitsPerSecond { get; set; }

        public int? Port { get; set; }

        public SCPFileSpec Source { get; set; }

        /// <summary>
        /// If target is not provided, source files will be downloaded locally.
        /// </summary>
        public SCPFileSpec? Target { get; set; }

        public SCPOptions(SCPFileSpec source)
        {
            Source = source;
        }
    }

    public class SCPCommand : ShellCommand
    {
        /// <summary>
        /// Construct a string array for the command line arguments to the SCP command
        /// </summary>
        /// <see cref="https://manpages.org/scp"/>
        /// <see cref="https://en.wikipedia.org/wiki/Secure_copy_protocol"/>
        /// <param name="options"></param>
        /// <returns></returns>
        private static string[] CreateArgs(SCPOptions options)
        {
            var args = new List<string>();

            bool optAdd<T>(string flag, T? value)
            {
                if (args is null || value is null)
                {
                    return true;
                }
                else if (value is not bool)
                {
                    args.Add($"-{flag} {value}");
                    return true;
                }
                else if (true.Equals(value))
                {
                    args.Add($"-{flag}");
                    return true;
                }
                else
                {
                    return false;
                }
            }

            if (options.Protocol is not null)
            {
                if (!Enum.IsDefined(options.Protocol.Value))
                {
                    throw new InvalidOperationException($"{nameof(SCPOptions)}.{nameof(options.Protocol)} must only one of {Enum.GetNames<SSHProtocol>().ToString(", ")}. Was: {options.Protocol}");
                }
                else
                {
                    args.Add($"-{(int)options.Protocol}");
                }
            }

            optAdd("3", options.ThroughLocal);
            optAdd("4", options.AddressFamily == AddressFamily.InterNetwork);
            optAdd("6", options.AddressFamily == AddressFamily.InterNetworkV6);
            optAdd("B", options.BatchMode);
            optAdd("C", options.EnableCompression);
            optAdd("p", options.PreserveModificationTimes);
            optAdd("q", options.OutputMode == SCPOutputMode.Quite);
            optAdd("v", options.OutputMode == SCPOutputMode.Verbose);
            optAdd("r", options.RecursivelyCopyDirectories);

            optAdd("P", options.Port);
            optAdd("c", options.Cipher);
            optAdd("F", options.SSHConfig?.FullName?.QuotePath());
            optAdd("o", options.SSHOption);
            optAdd("l", options.LimitKilobitsPerSecond);
            optAdd("i", options.IdentityFile?.FullName?.QuotePath());

            args.Add(options.Source.ToString());
            if (options.Target is not null)
            {
                args.Add(options.Target.ToString());
            }


            return args.ToArray();
        }

        public SCPCommand(SCPOptions options)
            : base("scp", CreateArgs(options))
        {
        }

        public SCPCommand(DirectoryInfo? workingDir, SCPOptions options)
            : base(workingDir, "scp", CreateArgs(options))
        {
        }
    }
}
