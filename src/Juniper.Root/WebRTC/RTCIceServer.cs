namespace Juniper.WebRTC
{
    public abstract class AbstractRTCIceServer
    {
        public string Urls { get; }

        protected AbstractRTCIceServer(string protocol, string host, int defaultPort, int? port)
        {
            if (port is null || port.Value == defaultPort)
            {
                Urls = $"{protocol}:{host}";
            }
            else
            {
                Urls = $"{protocol}:{host}:{port.Value}";
            }
        }
    }

    public abstract class AbstractRTCTurnServer : AbstractRTCIceServer
    {
        public string Username { get; }
        public string Credential { get; }

        protected AbstractRTCTurnServer(string username, string credential, string protocol, string host, int defaultPort, int? port)
            : base(protocol, host, defaultPort, port)
        {
            Username = username;
            Credential = credential;
        }
    }

    public class RTCStunServer : AbstractRTCIceServer
    {
        public RTCStunServer(string host, int? port = null)
            : base("stun", host, 3478, port)
        { }
    }

    public class RTCTurnServer : AbstractRTCTurnServer
    {
        public RTCTurnServer(string username, string credential, string host, int? port = null)
            : base(username, credential, "turn", host, 3478, port)
        { }
    }

    public class RTCTurnsServer : AbstractRTCTurnServer
    {
        public RTCTurnsServer(string username, string credential, string host, int? port = null)
            : base(username, credential, "turns", host, 5349, port)
        { }
    }
}
