namespace Juniper
{
    public partial class MediaType
    {
        public sealed partial class Text : MediaType
        {
            public static readonly Text Cache_Manifest = new("cache-manifest", new string[] { "appcache" });
            public static readonly Text Calendar = new("calendar", new string[] { "ics", "ifb" });
            public static readonly Text Css = new("css", new string[] { "css" });
            public static readonly Text Csv = new("csv", new string[] { "csv" });
            public static readonly Text Csv_Schema = new("csv-schema");

            [System.Obsolete("DEPRECATED by RFC6350")]
            public static readonly Text Directory = new("directory");

            public static readonly Text Dns = new("dns");

            [System.Obsolete("OBSOLETED in favor of application/ecmascript")]
            public static readonly Text Ecmascript = new("ecmascript");

            public static readonly Text Encaprtp = new("encaprtp");
            public static readonly Text Enriched = new("enriched");
            public static readonly Text Example = new("example");
            public static readonly Text Flexfec = new("flexfec");
            public static readonly Text Fwdred = new("fwdred");
            public static readonly Text Grammar_Ref_List = new("grammar-ref-list");
            public static readonly Text Html = new("html", new string[] { "html", "htm" });

            [System.Obsolete("OBSOLETED in favor of application/javascript")]
            public static readonly Text Javascript = new("javascript");

            public static readonly Text Jcr_Cnd = new("jcr-cnd");
            public static readonly Text Markdown = new("markdown");
            public static readonly Text Mizar = new("mizar");
            public static readonly Text N3 = new("n3", new string[] { "n3" });
            public static readonly Text Parameters = new("parameters");
            public static readonly Text Parityfec = new("parityfec");
            public static readonly Text Plain = new("plain", new string[] { "txt", "text", "conf", "def", "list", "log", "in" });
            public static readonly Text Provenance_Notation = new("provenance-notation");
            public static readonly Text PrsFallensteinRst = new("prs.fallenstein.rst");
            public static readonly Text PrsLinesTag = new("prs.lines.tag", new string[] { "dsc" });
            public static readonly Text PrsPropLogic = new("prs.prop.logic");
            public static readonly Text Raptorfec = new("raptorfec");
            public static readonly Text RED = new("red");
            public static readonly Text Rfc822_Headers = new("rfc822-headers");
            public static readonly Text Richtext = new("richtext", new string[] { "rtx" });
            public static readonly Text Rtf = new("rtf");
            public static readonly Text Rtp_Enc_Aescm128 = new("rtp-enc-aescm128");
            public static readonly Text Rtploopback = new("rtploopback");
            public static readonly Text Rtx = new("rtx");
            public static readonly Text Sgml = new("sgml", new string[] { "sgml", "sgm" });
            public static readonly Text Strings = new("strings");
            public static readonly Text T140 = new("t140");
            public static readonly Text Tab_Separated_Values = new("tab-separated-values", new string[] { "tsv" });
            public static readonly Text Troff = new("troff", new string[] { "t", "tr", "roff", "man", "me", "ms" });
            public static readonly Text Turtle = new("turtle", new string[] { "ttl" });
            public static readonly Text Ulpfec = new("ulpfec");
            public static readonly Text Uri_List = new("uri-list", new string[] { "uri", "uris", "urls" });
            public static readonly Text Vcard = new("vcard", new string[] { "vcard" });
            public static readonly Text Vendor1d_Interleaved_Parityfec = new("1d-interleaved-parityfec");
            public static readonly Text VendorA = new("vnd.a");
            public static readonly Text VendorAbc = new("vnd.abc");
            public static readonly Text VendorAscii_Art = new("vnd.ascii-art");
            public static readonly Text VendorCurl = new("vnd.curl", new string[] { "curl" });
            public static readonly Text VendorCurlDcurl = new("vnd.curl.dcurl", new string[] { "dcurl" });
            public static readonly Text VendorCurlMcurl = new("vnd.curl.mcurl", new string[] { "mcurl" });
            public static readonly Text VendorCurlScurl = new("vnd.curl.scurl", new string[] { "scurl" });
            public static readonly Text VendorDebianCopyright = new("vnd.debian.copyright");
            public static readonly Text VendorDMClientScript = new("vnd.dmclientscript");
            public static readonly Text VendorDvbSubtitle = new("vnd.dvb.subtitle", new string[] { "sub" });
            public static readonly Text VendorEsmertecTheme_Descriptor = new("vnd.esmertec.theme-descriptor");
            public static readonly Text VendorFiclabFlt = new("vnd.ficlab.flt");
            public static readonly Text VendorFly = new("vnd.fly", new string[] { "fly" });
            public static readonly Text VendorFmiFlexstor = new("vnd.fmi.flexstor", new string[] { "flx" });
            public static readonly Text VendorGml = new("vnd.gml");
            public static readonly Text VendorGraphviz = new("vnd.graphviz", new string[] { "gv" });
            public static readonly Text VendorHgl = new("vnd.hgl");
            public static readonly Text VendorIn3d3dml = new("vnd.in3d.3dml", new string[] { "3dml" });
            public static readonly Text VendorIn3dSpot = new("vnd.in3d.spot", new string[] { "spot" });
            public static readonly Text VendorIPTCNewsML = new("vnd.iptc.newsml");
            public static readonly Text VendorIPTCNITF = new("vnd.iptc.nitf");
            public static readonly Text VendorLatex_Z = new("vnd.latex-z");
            public static readonly Text VendorMotorolaReflex = new("vnd.motorola.reflex");
            public static readonly Text VendorMs_Mediapackage = new("vnd.ms-mediapackage");
            public static readonly Text VendorNet2phoneCommcenterCommand = new("vnd.net2phone.commcenter.command");
            public static readonly Text VendorRadisysMsml_Basic_Layout = new("vnd.radisys.msml-basic-layout");
            public static readonly Text VendorSenxWarpscript = new("vnd.senx.warpscript");

            [System.Obsolete("OBSOLETED by request")]
            public static readonly Text VendorSiUricatalogue = new("vnd.si.uricatalogue");

            public static readonly Text VendorSosi = new("vnd.sosi");
            public static readonly Text VendorSunJ2meApp_Descriptor = new("vnd.sun.j2me.app-descriptor", new string[] { "jad" });
            public static readonly Text VendorTrolltechLinguist = new("vnd.trolltech.linguist");
            public static readonly Text VendorWapSi = new("vnd.wap.si");
            public static readonly Text VendorWapSl = new("vnd.wap.sl");
            public static readonly Text VendorWapWml = new("vnd.wap.wml", new string[] { "wml" });
            public static readonly Text VendorWapWmlscript = new("vnd.wap.wmlscript", new string[] { "wmls" });
            public static readonly Text Vtt = new("vtt");
            public static readonly Text X_Asm = new("x-asm", new string[] { "s", "asm" });
            public static readonly Text X_C = new("x-c", new string[] { "c", "cc", "cxx", "cpp", "h", "hh", "dic" });
            public static readonly Text X_Fortran = new("x-fortran", new string[] { "f", "for", "f77", "f90" });
            public static readonly Text X_Java_Source = new("x-java-source", new string[] { "java" });
            public static readonly Text X_Nfo = new("x-nfo", new string[] { "nfo" });
            public static readonly Text X_Opml = new("x-opml", new string[] { "opml" });
            public static readonly Text X_Pascal = new("x-pascal", new string[] { "p", "pas" });
            public static readonly Text X_Setext = new("x-setext", new string[] { "etx" });
            public static readonly Text X_Sfv = new("x-sfv", new string[] { "sfv" });
            public static readonly Text X_Uuencode = new("x-uuencode", new string[] { "uu" });
            public static readonly Text X_Vcalendar = new("x-vcalendar", new string[] { "vcs" });
            public static readonly Text X_Vcard = new("x-vcard", new string[] { "vcf" });
            public static readonly Text Xml = new("xml");
            public static readonly Text Xml_External_Parsed_Entity = new("xml-external-parsed-entity");

            public static new readonly Text[] Values = {
                Cache_Manifest,
                Calendar,
                Css,
                Csv,
                Csv_Schema,
                Dns,
                Encaprtp,
                Enriched,
                Example,
                Flexfec,
                Fwdred,
                Grammar_Ref_List,
                Html,
                Jcr_Cnd,
                Markdown,
                Mizar,
                N3,
                Parameters,
                Parityfec,
                Plain,
                Provenance_Notation,
                PrsFallensteinRst,
                PrsLinesTag,
                PrsPropLogic,
                Raptorfec,
                RED,
                Rfc822_Headers,
                Richtext,
                Rtf,
                Rtp_Enc_Aescm128,
                Rtploopback,
                Rtx,
                Sgml,
                Strings,
                T140,
                Tab_Separated_Values,
                Troff,
                Turtle,
                Ulpfec,
                Uri_List,
                Vcard,
                Vendor1d_Interleaved_Parityfec,
                VendorA,
                VendorAbc,
                VendorAscii_Art,
                VendorCurl,
                VendorCurlDcurl,
                VendorCurlMcurl,
                VendorCurlScurl,
                VendorDebianCopyright,
                VendorDMClientScript,
                VendorDvbSubtitle,
                VendorEsmertecTheme_Descriptor,
                VendorFiclabFlt,
                VendorFly,
                VendorFmiFlexstor,
                VendorGml,
                VendorGraphviz,
                VendorHgl,
                VendorIn3d3dml,
                VendorIn3dSpot,
                VendorIPTCNewsML,
                VendorIPTCNITF,
                VendorLatex_Z,
                VendorMotorolaReflex,
                VendorMs_Mediapackage,
                VendorNet2phoneCommcenterCommand,
                VendorRadisysMsml_Basic_Layout,
                VendorSenxWarpscript,
                VendorSosi,
                VendorSunJ2meApp_Descriptor,
                VendorTrolltechLinguist,
                VendorWapSi,
                VendorWapSl,
                VendorWapWml,
                VendorWapWmlscript,
                Vtt,
                X_Asm,
                X_C,
                X_Fortran,
                X_Java_Source,
                X_Nfo,
                X_Opml,
                X_Pascal,
                X_Setext,
                X_Sfv,
                X_Uuencode,
                X_Vcalendar,
                X_Vcard,
                Xml,
                Xml_External_Parsed_Entity
            };
        }
    }
}
