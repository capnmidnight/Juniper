namespace Juniper.Security
{
    public interface ICredentialReceiver
    {
        string CredentialFile { get; }

        void ReceiveCredentials(string[] args);
    }
}