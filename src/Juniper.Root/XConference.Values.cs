namespace Juniper
{
    public partial class MediaType
    {
        public sealed partial class XConference : MediaType
        {
            public static readonly XConference X_Cooltalk = new XConference("/x-cooltalk", new string[] {"ice"});

            public static readonly new XConference[] Values = {
                X_Cooltalk,
            };
        }
    }
}