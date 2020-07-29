using System;
using System.Collections.Generic;
using System.Net;

using Juniper.Processes;

namespace Juniper.HTTP.Server.Administration.NetSH
{
    public abstract class AbstractSslCertCommand :
        AbstractShellCommand
    {
        protected static IPEndPoint ValidateAddress(IPAddress address, int port)
        {
            if (address is null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            return new IPEndPoint(address, port);
        }

        protected static IPAddress ValidateAddress(string address)
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

        private IPEndPoint endPoint;

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

        private readonly string command;

        protected AbstractSslCertCommand(string command, IPEndPoint endPoint)
            : base("netsh")
        {
            this.command = command ?? throw new ArgumentNullException(nameof(command));
            this.endPoint = endPoint ?? throw new ArgumentNullException(nameof(endPoint));
        }

        protected override IEnumerable<string> Arguments
        {
            get
            {
                yield return "http";
                yield return command;
                yield return "sslcert";
                yield return $"ipport={EndPoint}";
            }
        }
    }
}
