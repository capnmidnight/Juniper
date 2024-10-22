namespace Juniper.Server.WebRTC;

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
    Task Chat(string fromUserID, string roomName, string text);
    string Heartbeat(string fromUserID);
}
