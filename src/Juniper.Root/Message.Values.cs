namespace Juniper
{
    public partial class MediaType
    {
        public sealed partial class Message : MediaType
        {
            public static readonly Message CPIM = new("cpim");
            public static readonly Message Delivery_Status = new("delivery-status");
            public static readonly Message Disposition_Notification = new("disposition-notification");
            public static readonly Message Example = new("example");
            public static readonly Message External_Body = new("external-body");
            public static readonly Message Feedback_Report = new("feedback-report");
            public static readonly Message Global = new("global");
            public static readonly Message Global_Delivery_Status = new("global-delivery-status");
            public static readonly Message Global_Disposition_Notification = new("global-disposition-notification");
            public static readonly Message Global_Headers = new("global-headers");
            public static readonly Message Http = new("http");
            public static readonly Message ImdnXml = new("imdn+xml", new string[] { "xml" });

            [System.Obsolete("OBSOLETED by RFC5537")]
            public static readonly Message News = new("news");

            public static readonly Message Partial = new("partial");
            public static readonly Message Rfc822 = new("rfc822", new string[] { "eml", "mime" });
            public static readonly Message S_Http = new("s-http");
            public static readonly Message Sip = new("sip");
            public static readonly Message Sipfrag = new("sipfrag");
            public static readonly Message Tracking_Status = new("tracking-status");

            [System.Obsolete("OBSOLETED by request")]
            public static readonly Message VendorSiSimp = new("vnd.si.simp");

            public static readonly Message VendorWfaWsc = new("vnd.wfa.wsc");

            public static new readonly Message[] Values = {
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
                VendorWfaWsc
            };
        }
    }
}
