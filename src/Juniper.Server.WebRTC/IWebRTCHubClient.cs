namespace Juniper.Server.WebRTC;

public interface IWebRTCHubClient
{
    Task UserJoined(string fromUserID, string userName);
    Task UserLeft(string fromUserID);
    Task IceReceived(string fromUserID, string iceJSON);
    Task OfferReceived(string fromUserID, string offerJSON);
    Task AnswerReceived(string fromUserID, string answerJSON);
    Task Chat(string fromUserID, string text);
}
