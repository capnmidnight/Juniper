namespace Juniper;

public partial class MediaType
{
    public static readonly Message Message_CPIM = new("cpim");
    public static readonly Message Message_Delivery_Status = new("delivery-status");
    public static readonly Message Message_Disposition_Notification = new("disposition-notification");
    public static readonly Message Message_Example = new("example");
    public static readonly Message Message_External_Body = new("external-body");
    public static readonly Message Message_Feedback_Report = new("feedback-report");
    public static readonly Message Message_Global = new("global");
    public static readonly Message Message_Global_Delivery_Status = new("global-delivery-status");
    public static readonly Message Message_Global_Disposition_Notification = new("global-disposition-notification");
    public static readonly Message Message_Global_Headers = new("global-headers");
    public static readonly Message Message_Http = new("http");
    public static readonly Message Message_ImdnXml = new("imdn+xml", "xml");

    [System.Obsolete("OBSOLETED by RFC5537")]
    public static readonly Message Message_News = new("news");

    public static readonly Message Message_Partial = new("partial");
    public static readonly Message Message_Rfc822 = new("rfc822", "eml", "mime");
    public static readonly Message Message_S_Http = new("s-http");
    public static readonly Message Message_Sip = new("sip");
    public static readonly Message Message_Sipfrag = new("sipfrag");
    public static readonly Message Message_Tracking_Status = new("tracking-status");

    [System.Obsolete("OBSOLETED by request")]
    public static readonly Message Message_Vendor_SiSimp = new("vnd.si.simp");

    public static readonly Message Message_Vendor_WfaWsc = new("vnd.wfa.wsc");
}
