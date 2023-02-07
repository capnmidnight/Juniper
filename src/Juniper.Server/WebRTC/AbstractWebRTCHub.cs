using Microsoft.AspNetCore.SignalR;

using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Juniper.WebRTC
{
    public interface IWebRTCHubClient
    {
        Task UserJoined(string fromUserID, string userName);
        Task UserLeft(string fromUserID);
        Task IceReceived(string fromUserID, string iceJSON);
        Task OfferReceived(string fromUserID, string offerJSON);
        Task AnswerReceived(string fromUserID, string answerJSON);
    }

    public interface IWebRTCHubServer
    {
        object GetIceServers(string fromUserID);
        object GetRTCConfiguration(string fromUserID);
        string GetNewUserID();
        Task Identify(string fromUserID);
        Task Join(string roomName);
        Task GreetEveryone(string fromUserID, string roomName, string userName);
        Task Greet(string fromUserID, string userID, string userName);
        Task Leave(string fromUserID, string roomName);
        Task SendIce(string fromUserID, string toUserID, string iceJSON);
        Task SendOffer(string fromUserID, string toUserID, string offerJSON);
        Task SendAnswer(string fromUserID, string toUserID, string answerJSON);
    }

    public abstract class AbstractWebRTCHub<ClientT> : Hub<ClientT>, IWebRTCHubServer
        where ClientT : class, IWebRTCHubClient
    {
        private static readonly Regex ICE_TYPE_PATTERN = new("\\btyp (\\w+)\\b", RegexOptions.Compiled);
        private static readonly TimeSpan offset = new(0, 10, 0);

        protected abstract string[] IceTypes { get; }
        protected abstract string CoTURNSecret { get; }
        protected abstract IEnumerable<Uri> TurnServers { get; }

        protected ILogger logger;

        protected AbstractWebRTCHub(ILogger logger)
            : base()
        {
            this.logger = logger;
        }

        public async Task SendIce(string fromUserID, string toUserID, string iceJSON)
        {
            var match = ICE_TYPE_PATTERN.Match(iceJSON);
            var type = match.Success ? match.Groups[1].Value : "unknown";
            if (IceTypes.Contains(type))
            {
                await User(toUserID).IceReceived(fromUserID, iceJSON);
            }
        }

        public object GetIceServers(string userID)
        {
            var iceServers = new List<object>();

            foreach (var server in TurnServers)
            {
                if (server.Scheme.StartsWith("stun"))
                {
                    iceServers.Add(new RTCStunServer(server.Host, server.Port));
                }
                else if (server.Scheme.StartsWith("turn"))
                {
                    using var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(CoTURNSecret));
                    var timestamp = (DateTimeOffset.Now + offset).ToUnixTimeSeconds();
                    var username = $"{timestamp}:{server.Host}:{userID}";
                    var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(username));
                    var credential = Convert.ToBase64String(hash);

                    if (server.Scheme.EndsWith("s"))
                    {
                        iceServers.Add(new RTCTurnsServer(username, credential, server.Host, server.Port));
                    }
                    else
                    {
                        iceServers.Add(new RTCTurnServer(username, credential, server.Host, server.Port));
                    }
                }
            }

            return iceServers;
        }

        public object GetRTCConfiguration(string userID) =>
            new
            {
                iceTransportPolicy = "all",
                iceCandidatePoolSize = 10,
                iceServers = GetIceServers(userID)
            };

        protected ClientT User(string userID) =>
            Clients.OthersInGroup("user_" + userID);

        protected ClientT Room(string name) =>
            Clients.OthersInGroup("room_" + name);

        public string GetNewUserID() =>
            Guid.NewGuid().ToString("D");

        public Task Identify(string userID) =>
            Groups.AddToGroupAsync(Context.ConnectionId, "user_" + userID);

        public Task Join(string roomName) =>
            Groups.AddToGroupAsync(Context.ConnectionId, "room_" + roomName);

        public Task GreetEveryone(string fromUserID, string roomName, string userName) =>
            Room(roomName).UserJoined(fromUserID, userName);

        public Task Greet(string fromUserID, string toUserID, string userName) =>
            User(toUserID).UserJoined(fromUserID, userName);

        public Task Leave(string fromUserID, string roomName) =>
            Task.WhenAll(
                Room(roomName).UserLeft(fromUserID),
                Groups.RemoveFromGroupAsync(Context.ConnectionId, "room_" + roomName)
            );

        public Task SendOffer(string fromUserID, string toUserID, string offerJSON) =>
            User(toUserID).OfferReceived(fromUserID, offerJSON);

        public Task SendAnswer(string fromUserID, string toUserID, string answerJSON) =>
            User(toUserID).AnswerReceived(fromUserID, answerJSON);

    }
}
