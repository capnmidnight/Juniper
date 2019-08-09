using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Juniper.Security
{
    public interface ICredentialReceiver
    {
        string CredentialFile { get; }

        void ReceiveCredentials(string[] args);

        void ClearCredentials();
    }

    public static class ICredentialReceiverExt
    {
        public static void ReceiveCredentials(this ICredentialReceiver receiver, string fileName)
        {
            receiver.ReceiveCredentials(File.ReadAllLines(fileName));
        }

        public static void ReceiveCredentials(this ICredentialReceiver receiver)
        {
            receiver.ReceiveCredentials(receiver.CredentialFile);
        }

        public static void ReceiveCredentials(this ICredentialReceiver receiver, FileInfo file)
        {
            receiver.ReceiveCredentials(file.FullName);
        }

        public static void ReceiveCredentials(this ICredentialReceiver receiver, Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                var lines = new List<string>();
                while (!reader.EndOfStream)
                {
                    lines.Add(reader.ReadLine());
                }

                receiver.ReceiveCredentials(lines.ToArray());
            }
        }

        public static void ParseCredentials(this ICredentialReceiver receiver, string text)
        {
            using (var mem = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            {
                receiver.ReceiveCredentials(mem);
            }
        }
    }
}