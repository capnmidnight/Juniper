using System.Net;

namespace Juniper.HTTP.Server.Administration.NetSH
{
    public class DeleteSslCert :
        AbstractSslCertCommand
    {
        public DeleteSslCert(IPEndPoint endPoint)
            : base("delete", endPoint)
        { }

        public DeleteSslCert(IPAddress address, int port)
            : this(ValidateAddress(address, port))
        { }

        public DeleteSslCert(string address, int port)
            : this(ValidateAddress(address), port)
        { }
    }
}
