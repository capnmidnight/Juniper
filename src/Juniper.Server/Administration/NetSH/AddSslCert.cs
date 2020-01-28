using System;
using System.Collections.Generic;
using System.Net;

namespace Juniper.HTTP.Server.Administration.NetSH
{
    public class AddSslCert :
        AbstractSslCertCommand
    {

        private Guid appID;

        public Guid AppID
        {
            get
            {
                return appID;
            }
            set
            {
                if (value == Guid.Empty)
                {
                    throw new InvalidOperationException("Must provide a valid Guid value");
                }

                appID = value;
            }
        }

        private string certHash;

        public string CertHash
        {
            get
            {
                return certHash;
            }
            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                certHash = value;
            }
        }

        public AddSslCert(IPEndPoint endPoint, string certHash, Guid appID)
            : base("add", endPoint)
        {
            CertHash = certHash;
            AppID = appID;
        }

        public AddSslCert(IPAddress address, int port, string certHash, Guid appID)
            : this(ValidateAddress(address, port), certHash, appID)
        { }

        public AddSslCert(string address, int port, string certHash, Guid appID)
            : this(ValidateAddress(address), port, certHash, appID)
        { }

        protected override IEnumerable<string> Arguments
        {
            get
            {
                foreach (var arg in base.Arguments)
                {
                    yield return arg;
                }

                yield return $"certhash={CertHash}";
                yield return $"appid={{{AppID}}}";
            }
        }
    }
}
