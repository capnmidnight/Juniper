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


    public abstract class AbstractGameHub : AbstractWebRTCHub<IGameHubClient>, IGameHubServer
    {
        public async Task UserPosed(string fromUserID, string roomName, float px, float py, float pz, float fx, float fy, float fz, float ux, float uy, float uz, float height)
        {
            await Room(roomName).UserPosed(fromUserID, px, py, pz, fx, fy, fz, ux, uy, uz, height);
        }

        public async Task UserPointer(string fromUserID, string roomName, int name, float px, float py, float pz, float fx, float fy, float fz, float ux, float uy, float uz)
        {
            await Room(roomName).UserPointer(fromUserID, name, px, py, pz, fx, fy, fz, ux, uy, uz);
        }

        public async Task Chat(string fromUserID, string roomName, string msg)
        {
            await Room(roomName).Chat(fromUserID, msg);
        }
    }
}
