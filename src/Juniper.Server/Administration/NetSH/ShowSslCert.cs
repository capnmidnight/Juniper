using System.Net;

namespace Juniper.HTTP.Server.Administration.NetSH
{
    public class ShowSslCert :
        AbstractSslCertCommand
    {
        public ShowSslCert(IPEndPoint endPoint)
            : base("show", endPoint)
        { }

        public ShowSslCert(IPAddress address, int port)
            : this(ValidateAddress(address, port))
        { }

        public ShowSslCert(string address, int port)
            : this(ValidateAddress(address), port)
        { }
    }
}
