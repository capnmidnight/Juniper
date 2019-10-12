namespace Juniper
{
    public partial class MediaType
    {
        public sealed class XConference : MediaType
        {
            private XConference(string value, string[] extensions) : base("xconference/" + value, extensions) {}

            private XConference(string value) : this(value, null) {}

            public static readonly XConference X_Cooltalk = new XConference("/x-cooltalk", new string[] {"ice"});

            public static readonly new XConference[] Values = {
                X_Cooltalk,
            };
        }
    }
}
