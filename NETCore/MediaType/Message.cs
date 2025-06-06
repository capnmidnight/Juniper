namespace Juniper;

public partial class MediaType
{
    public partial class Message : MediaType
    {
        private static List<Message>? _allMessage;
        private static List<Message> AllMsg => _allMessage ??= new();
        public static IReadOnlyCollection<Message> AllMessage => AllMsg;
        public static readonly Message AnyMessage = new("*");

        public Message(string value, params string[] extensions) : base("message", value, extensions)
        {
            if (SubType != "*")
            {
                AllMsg.Add(this);
            }
        }

        public Message(MediaType copy)
            : this(
                  copy.Type == "message"
                  ? copy.FullSubType
                  : throw new ArgumentException("Invalid media type", nameof(copy)),
                  copy.Extensions.ToArray())
        {

        }
    }
}
