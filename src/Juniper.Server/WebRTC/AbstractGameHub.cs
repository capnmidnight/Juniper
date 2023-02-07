namespace Juniper.WebRTC
{

    public interface IGameHubClient : IWebRTCHubClient
    {
        Task UserPosed(string fromUserID, float px, float py, float pz, float fx, float fy, float fz, float ux, float uy, float uz, float height);
        Task UserPointer(string fromUserID, int name, float px, float py, float pz, float fx, float fy, float fz, float ux, float uy, float uz);
        Task Chat(string fromUserID, string text);
    }

    public interface IGameHubServer
    {
        Task UserPosed(string fromUserID, string roomName, float px, float py, float pz, float fx, float fy, float fz, float ux, float uy, float uz, float height);
        Task UserPointer(string fromUserID, string roomName, int name, float px, float py, float pz, float fx, float fy, float fz, float ux, float uy, float uz);
        Task Chat(string fromUserID, string roomName, string text);
    }


    public abstract class AbstractGameHub<ClientT> : AbstractWebRTCHub<ClientT>, IGameHubServer
        where ClientT : class, IGameHubClient
    {
        protected AbstractGameHub(ILogger logger) : base(logger) { }

        public Task UserPosed(string fromUserID, string roomName, float px, float py, float pz, float fx, float fy, float fz, float ux, float uy, float uz, float height) =>
            Room(roomName).UserPosed(fromUserID, px, py, pz, fx, fy, fz, ux, uy, uz, height);

        public Task UserPointer(string fromUserID, string roomName, int name, float px, float py, float pz, float fx, float fy, float fz, float ux, float uy, float uz) =>
            Room(roomName).UserPointer(fromUserID, name, px, py, pz, fx, fy, fz, ux, uy, uz);

        public Task Chat(string fromUserID, string roomName, string msg) =>
            Room(roomName).Chat(fromUserID, msg);
    }
}
