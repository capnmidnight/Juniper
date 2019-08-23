namespace Juniper.HTTP
{
    public partial class MediaType
    {
        public sealed class XConference : MediaType
        {
            public XConference(string value, string[] extensions = null) : base("xconference/" + value, extensions) {}

            public static readonly XConference XCooltalk = new XConference("/x-cooltalk", new string[] {"ice"});
        }
    }
}
