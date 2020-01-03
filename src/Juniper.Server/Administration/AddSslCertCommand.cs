using System;
using System.Collections.Generic;
using System.Net;
using Juniper.Processes;

namespace Juniper.HTTP.Server.Administration
{
    public class AddSslCertCommand :
        AbstractShellCommand
    {
        private Guid appID;
        private IPEndPoint endPoint;
        private string certHash;

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
            get
            {
                return endPoint;
            }
            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                endPoint = value;
            }
        }

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

        public AddSslCertCommand(IPEndPoint endPoint, string certHash, Guid appID)
            : base("netsh")
        {
            EndPoint = endPoint;
            CertHash = certHash;
            AppID = appID;
        }

        public AddSslCertCommand(IPAddress address, int port, string certHash, Guid appID)
            : this(ValidateAddress(address, port), certHash, appID)
        { }

        public AddSslCertCommand(string address, int port, string certHash, Guid appID)
            : this(ValidateAddress(address), port, certHash, appID)
        { }

        protected override IEnumerable<string> Arguments
        {
            get
            {
                yield return "http";
                yield return "add";
                yield return $"ipport={EndPoint}";
                yield return $"certhash={CertHash}";
                yield return $"appid={{{AppID}}}";
            }
        }
    }
}
