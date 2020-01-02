using System;
using System.Collections.Generic;
using System.Net;

namespace Juniper.HTTP.Server.Administration
{
    public class NetShHttpAddSslCertCommand :
        AbstractNetShHttpAddCommand
    {
        private static IPEndPoint ValidateAddress(IPAddress address, int port)
        {
            if (address is null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            return new IPEndPoint(address, port);
        }

        private static IPAddress ValidateAddress(string address)
        {
            if (address is null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            if (address.Length == 0)
            {
                throw new InvalidOperationException($"{nameof(address)} cannot be an empty string");
            }

            return IPAddress.Parse(address);
        }

        public IPEndPoint EndPoint
        {
            get;
            set;
        }

        public string CertHash
        {
            get;
            set;
        }

        public Guid AppID
        {
            get;
            set;
        }

        public NetShHttpAddSslCertCommand(IPEndPoint endPoint, string certHash, Guid appID)
            : base("sslCert")
        {
            EndPoint = endPoint;
            CertHash = certHash;
            AppID = appID;
        }

        public NetShHttpAddSslCertCommand(IPAddress address, int port, string certHash, Guid appID)
            : this(ValidateAddress(address, port), certHash, appID)
        { }

        public NetShHttpAddSslCertCommand(string address, int port, string certHash, Guid appID)
            : this(ValidateAddress(address), port, certHash, appID)
        { }

        protected override IEnumerable<string> Arguments
        {
            get
            {
                yield return $"ipport={EndPoint}";
                yield return $"certhash={CertHash}";
                yield return $"appid={{{AppID}}}";
            }
        }
    }
}
