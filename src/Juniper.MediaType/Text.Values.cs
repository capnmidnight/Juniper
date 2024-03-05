namespace Juniper
{
    public partial class MediaType
    {
        public static readonly Text Text_Cache_Manifest = new("cache-manifest", "appcache");
        public static readonly Text Text_Calendar = new("calendar", "ics", "ifb");
        public static readonly Text Text_Css = new("css", "css");
        public static readonly Text Text_Csv = new("csv", "csv");
        public static readonly Text Text_Csv_Schema = new("csv-schema");

        [System.Obsolete("DEPRECATED by RFC6350")]
        public static readonly Text Text_Directory = new("directory");

        public static readonly Text Text_Dns = new("dns");

        [System.Obsolete("OBSOLETED in favor of application/ecmascript")]
        public static readonly Text Text_Ecmascript = new("ecmascript");

        public static readonly Text Text_Encaprtp = new("encaprtp");
        public static readonly Text Text_Enriched = new("enriched");
        public static readonly Text Text_Event_Stream = new("event-stream");
        public static readonly Text Text_Example = new("example");
        public static readonly Text Text_Flexfec = new("flexfec");
        public static readonly Text Text_Fwdred = new("fwdred");
        public static readonly Text Text_Grammar_Ref_List = new("grammar-ref-list");
        public static readonly Text Text_Html = new("html", "html", "htm");

        [System.Obsolete("OBSOLETED in favor of application/javascript")]
        public static readonly Text Text_Javascript = new("javascript");

        public static readonly Text Text_Jcr_Cnd = new("jcr-cnd");
        public static readonly Text Text_Markdown = new("markdown");
        public static readonly Text Text_Mizar = new("mizar");
        public static readonly Text Text_N3 = new("n3", "n3");
        public static readonly Text Text_Parameters = new("parameters");
        public static readonly Text Text_Parityfec = new("parityfec");
        public static readonly Text Text_Plain = new("plain", "txt", "text", "conf", "def", "list", "log", "in");
        public static readonly Text Text_Provenance_Notation = new("provenance-notation");
        public static readonly Text Text_PrsFallensteinRst = new("prs.fallenstein.rst");
        public static readonly Text Text_PrsLinesTag = new("prs.lines.tag", "dsc");
        public static readonly Text Text_PrsPropLogic = new("prs.prop.logic");
        public static readonly Text Text_Raptorfec = new("raptorfec");
        public static readonly Text Text_RED = new("red");
        public static readonly Text Text_Rfc822_Headers = new("rfc822-headers");
        public static readonly Text Text_Richtext = new("richtext", "rtx");
        public static readonly Text Text_Rtf = new("rtf");
        public static readonly Text Text_Rtp_Enc_Aescm128 = new("rtp-enc-aescm128");
        public static readonly Text Text_Rtploopback = new("rtploopback");
        public static readonly Text Text_Rtx = new("rtx");
        public static readonly Text Text_Sgml = new("sgml", "sgml", "sgm");
        public static readonly Text Text_Strings = new("strings");
        public static readonly Text Text_T140 = new("t140");
        public static readonly Text Text_Tab_Separated_Values = new("tab-separated-values", "tsv");
        public static readonly Text Text_Troff = new("troff", "t", "tr", "roff", "man", "me", "ms");
        public static readonly Text Text_Turtle = new("turtle", "ttl");
        public static readonly Text Text_Ulpfec = new("ulpfec");
        public static readonly Text Text_Uri_List = new("uri-list", "uri", "uris", "urls");
        public static readonly Text Text_Vcard = new("vcard", "vcard");
        public static readonly Text Text_Vendor1d_Interleaved_Parityfec = new("1d-interleaved-parityfec");
        public static readonly Text Text_VendorA = new("vnd.a");
        public static readonly Text Text_VendorAbc = new("vnd.abc");
        public static readonly Text Text_VendorAscii_Art = new("vnd.ascii-art");
        public static readonly Text Text_VendorCurl = new("vnd.curl", "curl");
        public static readonly Text Text_VendorCurlDcurl = new("vnd.curl.dcurl", "dcurl");
        public static readonly Text Text_VendorCurlMcurl = new("vnd.curl.mcurl", "mcurl");
        public static readonly Text Text_VendorCurlScurl = new("vnd.curl.scurl", "scurl");
        public static readonly Text Text_VendorDebianCopyright = new("vnd.debian.copyright");
        public static readonly Text Text_VendorDMClientScript = new("vnd.dmclientscript");
        public static readonly Text Text_VendorDvbSubtitle = new("vnd.dvb.subtitle", "sub");
        public static readonly Text Text_VendorEsmertecTheme_Descriptor = new("vnd.esmertec.theme-descriptor");
        public static readonly Text Text_VendorFiclabFlt = new("vnd.ficlab.flt");
        public static readonly Text Text_VendorFly = new("vnd.fly", "fly");
        public static readonly Text Text_VendorFmiFlexstor = new("vnd.fmi.flexstor", "flx");
        public static readonly Text Text_VendorGml = new("vnd.gml");
        public static readonly Text Text_VendorGraphviz = new("vnd.graphviz", "gv");
        public static readonly Text Text_VendorHgl = new("vnd.hgl");
        public static readonly Text Text_VendorIn3d3dml = new("vnd.in3d.3dml", "3dml");
        public static readonly Text Text_VendorIn3dSpot = new("vnd.in3d.spot", "spot");
        public static readonly Text Text_VendorIPTCNewsML = new("vnd.iptc.newsml");
        public static readonly Text Text_VendorIPTCNITF = new("vnd.iptc.nitf");
        public static readonly Text Text_VendorLatex_Z = new("vnd.latex-z");
        public static readonly Text Text_VendorMotorolaReflex = new("vnd.motorola.reflex");
        public static readonly Text Text_VendorMs_Mediapackage = new("vnd.ms-mediapackage");
        public static readonly Text Text_VendorNet2phoneCommcenterCommand = new("vnd.net2phone.commcenter.command");
        public static readonly Text Text_VendorRadisysMsml_Basic_Layout = new("vnd.radisys.msml-basic-layout");
        public static readonly Text Text_VendorSenxWarpscript = new("vnd.senx.warpscript");

        [System.Obsolete("OBSOLETED by request")]
        public static readonly Text Text_VendorSiUricatalogue = new("vnd.si.uricatalogue");

        public static readonly Text Text_VendorSosi = new("vnd.sosi");
        public static readonly Text Text_VendorSunJ2meApp_Descriptor = new("vnd.sun.j2me.app-descriptor", "jad");
        public static readonly Text Text_VendorTrolltechLinguist = new("vnd.trolltech.linguist");
        public static readonly Text Text_VendorWapSi = new("vnd.wap.si");
        public static readonly Text Text_VendorWapSl = new("vnd.wap.sl");
        public static readonly Text Text_VendorWapWml = new("vnd.wap.wml", "wml");
        public static readonly Text Text_VendorWapWmlscript = new("vnd.wap.wmlscript", "wmls");
        public static readonly Text Text_Vtt = new("vtt");
        public static readonly Text Text_Wgsl = new("wgsl", "wgsl");
        public static readonly Text Text_X_Asm = new("x-asm", "s", "asm");
        public static readonly Text Text_X_C = new("x-c", "c", "cc", "cxx", "cpp", "h", "hh", "dic");
        public static readonly Text Text_X_Fortran = new("x-fortran", "f", "for", "f77", "f90");
        public static readonly Text Text_X_Java_Source = new("x-java-source", "java");
        public static readonly Text Text_X_Nfo = new("x-nfo", "nfo");
        public static readonly Text Text_X_Opml = new("x-opml", "opml");
        public static readonly Text Text_X_Pascal = new("x-pascal", "p", "pas");
        public static readonly Text Text_X_Python = new("x-python", "py");
        public static readonly Text Text_X_Processing = new("x-processing", "pde");
        public static readonly Text Text_X_Sass = new("x-sass", "sass");
        public static readonly Text Text_X_Scss = new("x-scss", "scss");
        public static readonly Text Text_X_Script_Python = new("x.script-python", "py");
        public static readonly Text Text_X_Setext = new("x-setext", "etx");
        public static readonly Text Text_X_Sfv = new("x-sfv", "sfv");
        public static readonly Text Text_X_Uuencode = new("x-uuencode", "uu");
        public static readonly Text Text_X_Vcalendar = new("x-vcalendar", "vcs");
        public static readonly Text Text_X_Vcard = new("x-vcard", "vcf");
        public static readonly Text Text_Xml = new("xml");
        public static readonly Text Text_Xml_External_Parsed_Entity = new("xml-external-parsed-entity");
    }
}
