namespace Juniper.HTTP.MediaTypes
{
    public static class Message
    {
        public const string CPIM = "message/CPIM";
        public const string DeliveryStatus = "message/delivery-status";
        public const string DispositionNotification = "message/disposition-notification";
        public const string Example = "message/example";
        public const string FeedbackReport = "message/feedback-report";
        public const string Global = "message/global";
        public const string GlobalDeliveryStatus = "message/global-delivery-status";
        public const string GlobalDispositionNotification = "message/global-disposition-notification";
        public const string GlobalHeaders = "message/global-headers";
        public const string Http = "message/http";
        public const string ImdnXml = "message/imdn+xml";

        [System.Obsolete("OBSOLETED by RFC5537")]
        public const string News = "message/news";

        public const string SHttp = "message/s-http";
        public const string Sip = "message/sip";
        public const string Sipfrag = "message/sipfrag";
        public const string TrackingStatus = "message/tracking-status";

        [System.Obsolete("OBSOLETED by request")]
        public const string VendorSiSimp = "message/vnd.si.simp";

        public const string VendorWfaWsc = "message/vnd.wfa.wsc";
    }
}
