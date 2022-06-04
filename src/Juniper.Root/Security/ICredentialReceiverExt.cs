namespace Juniper.Security
{

    public static class ICredentialReceiverExt
    {
        public static void ClearCredentials(this ICredentialReceiver receiver)
        {
            if (receiver is null)
            {
                throw new System.ArgumentNullException(nameof(receiver));
            }

            receiver.ReceiveCredentials(null);
        }

        public static void ReceiveCredentials(this ICredentialReceiver receiver)
        {
            if (receiver is null)
            {
                throw new System.ArgumentNullException(nameof(receiver));
            }

            receiver.ReceiveCredentials(receiver.CredentialFile);
        }

        public static void ReceiveCredentials(this ICredentialReceiver receiver, string fileName)
        {
            if (receiver is null)
            {
                throw new System.ArgumentNullException(nameof(receiver));
            }

            if (File.Exists(fileName))
            {
                receiver.ReceiveCredentials(File.ReadAllLines(fileName));
            }
        }
    }
}