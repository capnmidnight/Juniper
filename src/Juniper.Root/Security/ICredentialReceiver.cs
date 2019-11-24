using System.IO;

namespace Juniper.Security
{
    public interface ICredentialReceiver
    {
        string CredentialFile { get; }

        void ReceiveCredentials(string[] args);
    }

    public static class ICredentialReceiverExt
    {
        public static void ClearCredentials(this ICredentialReceiver receiver)
        {
            receiver.ReceiveCredentials(null);
        }

        public static void ReceiveCredentials(this ICredentialReceiver receiver)
        {
            receiver.ReceiveCredentials(receiver.CredentialFile);
        }

        public static void ReceiveCredentials(this ICredentialReceiver receiver, string fileName)
        {
            if (File.Exists(fileName))
            {
                receiver.ReceiveCredentials(File.ReadAllLines(fileName));
            }
        }
    }
}