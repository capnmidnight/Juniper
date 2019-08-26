namespace Juniper.HTTP
{
    public partial class MediaType
    {
        public sealed class Message : MediaType
        {
            public Message(string value, string[] extensions = null) : base("message/" + value, extensions) {}

            public static readonly Message CPIM = new Message("cpim");
            public static readonly Message Delivery_Status = new Message("delivery-status");
            public static readonly Message Disposition_Notification = new Message("disposition-notification");
            public static readonly Message Example = new Message("example");
            public static readonly Message External_Body = new Message("external-body");
            public static readonly Message Feedback_Report = new Message("feedback-report");
            public static readonly Message Global = new Message("global");
            public static readonly Message Global_Delivery_Status = new Message("global-delivery-status");
            public static readonly Message Global_Disposition_Notification = new Message("global-disposition-notification");
            public static readonly Message Global_Headers = new Message("global-headers");
            public static readonly Message Http = new Message("http");
            public static readonly Message ImdnXml = new Message("imdn+xml", new string[] {"xml"});

            [System.Obsolete("OBSOLETED by RFC5537")]
            public static readonly Message News = new Message("news");

            public static readonly Message Partial = new Message("partial");
            public static readonly Message Rfc822 = new Message("rfc822", new string[] {"eml", "mime"});
            public static readonly Message S_Http = new Message("s-http");
            public static readonly Message Sip = new Message("sip");
            public static readonly Message Sipfrag = new Message("sipfrag");
            public static readonly Message Tracking_Status = new Message("tracking-status");

            [System.Obsolete("OBSOLETED by request")]
            public static readonly Message VendorSiSimp = new Message("vnd.si.simp");

            public static readonly Message VendorWfaWsc = new Message("vnd.wfa.wsc");

            public static readonly new Message[] Values = {
                CPIM,
                Delivery_Status,
                Disposition_Notification,
                Example,
                External_Body,
                Feedback_Report,
                Global,
                Global_Delivery_Status,
                Global_Disposition_Notification,
                Global_Headers,
                Http,
                ImdnXml,
                Partial,
                Rfc822,
                S_Http,
                Sip,
                Sipfrag,
                Tracking_Status,
                VendorWfaWsc,
            };
        }
    }
}
