namespace Juniper.HTTP
{
    public partial class MediaType
    {
        public sealed class Multipart : MediaType
        {
            public Multipart(string value, string[] extensions) : base("multipart/" + value, extensions) {}

            public Multipart(string value) : this(value, null) {}

            public static readonly Multipart Alternative = new Multipart("alternative");
            public static readonly Multipart Appledouble = new Multipart("appledouble");
            public static readonly Multipart Byteranges = new Multipart("byteranges");
            public static readonly Multipart Digest = new Multipart("digest");
            public static readonly Multipart Encrypted = new Multipart("encrypted");
            public static readonly Multipart Example = new Multipart("example");
            public static readonly Multipart Form_Data = new Multipart("form-data");
            public static readonly Multipart Header_Set = new Multipart("header-set");
            public static readonly Multipart Mixed = new Multipart("mixed");
            public static readonly Multipart Multilingual = new Multipart("multilingual");
            public static readonly Multipart Parallel = new Multipart("parallel");
            public static readonly Multipart Related = new Multipart("related");
            public static readonly Multipart Report = new Multipart("report");
            public static readonly Multipart Signed = new Multipart("signed");
            public static readonly Multipart VendorBintMed_Plus = new Multipart("vnd.bint.med-plus");
            public static readonly Multipart Voice_Message = new Multipart("voice-message");
            public static readonly Multipart X_Mixed_Replace = new Multipart("x-mixed-replace");

            public static readonly new Multipart[] Values = {
                Alternative,
                Appledouble,
                Byteranges,
                Digest,
                Encrypted,
                Example,
                Form_Data,
                Header_Set,
                Mixed,
                Multilingual,
                Parallel,
                Related,
                Report,
                Signed,
                VendorBintMed_Plus,
                Voice_Message,
                X_Mixed_Replace,
            };
        }
    }
}
