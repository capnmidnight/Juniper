namespace Juniper
{
    public partial class MediaType
    {
        public partial class Multipart : MediaType
        {
            public static readonly Multipart Alternative = new("alternative");
            public static readonly Multipart Appledouble = new("appledouble");
            public static readonly Multipart Byteranges = new("byteranges");
            public static readonly Multipart Digest = new("digest");
            public static readonly Multipart Encrypted = new("encrypted");
            public static readonly Multipart Example = new("example");
            public static readonly Multipart Form_Data = new("form-data");
            public static readonly Multipart Header_Set = new("header-set");
            public static readonly Multipart Mixed = new("mixed");
            public static readonly Multipart Multilingual = new("multilingual");
            public static readonly Multipart Parallel = new("parallel");
            public static readonly Multipart Related = new("related");
            public static readonly Multipart Report = new("report");
            public static readonly Multipart Signed = new("signed");
            public static readonly Multipart VendorBintMed_Plus = new("vnd.bint.med-plus");
            public static readonly Multipart Voice_Message = new("voice-message");
            public static readonly Multipart X_Mixed_Replace = new("x-mixed-replace");

            public static new readonly Multipart[] Values = {
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
                X_Mixed_Replace
            };
        }
    }
}
