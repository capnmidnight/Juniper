namespace Juniper
{
    public partial class MediaType
    {
        public sealed partial class Text : MediaType
        {
            public static readonly Text Cache_Manifest = new Text("cache-manifest", new string[] { "appcache" });
            public static readonly Text Calendar = new Text("calendar", new string[] { "ics", "ifb" });
            public static readonly Text Css = new Text("css", new string[] { "css" });
            public static readonly Text Csv = new Text("csv", new string[] { "csv" });
            public static readonly Text Csv_Schema = new Text("csv-schema");

            [System.Obsolete("DEPRECATED by RFC6350")]
            public static readonly Text Directory = new Text("directory");

            public static readonly Text Dns = new Text("dns");

            [System.Obsolete("OBSOLETED in favor of application/ecmascript")]
            public static readonly Text Ecmascript = new Text("ecmascript");

            public static readonly Text Encaprtp = new Text("encaprtp");
            public static readonly Text Enriched = new Text("enriched");
            public static readonly Text Example = new Text("example");
            public static readonly Text Flexfec = new Text("flexfec");
            public static readonly Text Fwdred = new Text("fwdred");
            public static readonly Text Grammar_Ref_List = new Text("grammar-ref-list");
            public static readonly Text Html = new Text("html", new string[] { "html", "htm" });

            [System.Obsolete("OBSOLETED in favor of application/javascript")]
            public static readonly Text Javascript = new Text("javascript");

            public static readonly Text Jcr_Cnd = new Text("jcr-cnd");
            public static readonly Text Markdown = new Text("markdown");
            public static readonly Text Mizar = new Text("mizar");
            public static readonly Text N3 = new Text("n3", new string[] { "n3" });
            public static readonly Text Parameters = new Text("parameters");
            public static readonly Text Parityfec = new Text("parityfec");
            public static readonly Text Plain = new Text("plain", new string[] { "txt", "text", "conf", "def", "list", "log", "in" });
            public static readonly Text Provenance_Notation = new Text("provenance-notation");
            public static readonly Text PrsFallensteinRst = new Text("prs.fallenstein.rst");
            public static readonly Text PrsLinesTag = new Text("prs.lines.tag", new string[] { "dsc" });
            public static readonly Text PrsPropLogic = new Text("prs.prop.logic");
            public static readonly Text Raptorfec = new Text("raptorfec");
            public static readonly Text RED = new Text("red");
            public static readonly Text Rfc822_Headers = new Text("rfc822-headers");
            public static readonly Text Richtext = new Text("richtext", new string[] { "rtx" });
            public static readonly Text Rtf = new Text("rtf");
            public static readonly Text Rtp_Enc_Aescm128 = new Text("rtp-enc-aescm128");
            public static readonly Text Rtploopback = new Text("rtploopback");
            public static readonly Text Rtx = new Text("rtx");
            public static readonly Text Sgml = new Text("sgml", new string[] { "sgml", "sgm" });
            public static readonly Text Strings = new Text("strings");
            public static readonly Text T140 = new Text("t140");
            public static readonly Text Tab_Separated_Values = new Text("tab-separated-values", new string[] { "tsv" });
            public static readonly Text Troff = new Text("troff", new string[] { "t", "tr", "roff", "man", "me", "ms" });
            public static readonly Text Turtle = new Text("turtle", new string[] { "ttl" });
            public static readonly Text Ulpfec = new Text("ulpfec");
            public static readonly Text Uri_List = new Text("uri-list", new string[] { "uri", "uris", "urls" });
            public static readonly Text Vcard = new Text("vcard", new string[] { "vcard" });
            public static readonly Text Vendor1d_Interleaved_Parityfec = new Text("1d-interleaved-parityfec");
            public static readonly Text VendorA = new Text("vnd.a");
            public static readonly Text VendorAbc = new Text("vnd.abc");
            public static readonly Text VendorAscii_Art = new Text("vnd.ascii-art");
            public static readonly Text VendorCurl = new Text("vnd.curl", new string[] { "curl" });
            public static readonly Text VendorCurlDcurl = new Text("vnd.curl.dcurl", new string[] { "dcurl" });
            public static readonly Text VendorCurlMcurl = new Text("vnd.curl.mcurl", new string[] { "mcurl" });
            public static readonly Text VendorCurlScurl = new Text("vnd.curl.scurl", new string[] { "scurl" });
            public static readonly Text VendorDebianCopyright = new Text("vnd.debian.copyright");
            public static readonly Text VendorDMClientScript = new Text("vnd.dmclientscript");
            public static readonly Text VendorDvbSubtitle = new Text("vnd.dvb.subtitle", new string[] { "sub" });
            public static readonly Text VendorEsmertecTheme_Descriptor = new Text("vnd.esmertec.theme-descriptor");
            public static readonly Text VendorFiclabFlt = new Text("vnd.ficlab.flt");
            public static readonly Text VendorFly = new Text("vnd.fly", new string[] { "fly" });
            public static readonly Text VendorFmiFlexstor = new Text("vnd.fmi.flexstor", new string[] { "flx" });
            public static readonly Text VendorGml = new Text("vnd.gml");
            public static readonly Text VendorGraphviz = new Text("vnd.graphviz", new string[] { "gv" });
            public static readonly Text VendorHgl = new Text("vnd.hgl");
            public static readonly Text VendorIn3d3dml = new Text("vnd.in3d.3dml", new string[] { "3dml" });
            public static readonly Text VendorIn3dSpot = new Text("vnd.in3d.spot", new string[] { "spot" });
            public static readonly Text VendorIPTCNewsML = new Text("vnd.iptc.newsml");
            public static readonly Text VendorIPTCNITF = new Text("vnd.iptc.nitf");
            public static readonly Text VendorLatex_Z = new Text("vnd.latex-z");
            public static readonly Text VendorMotorolaReflex = new Text("vnd.motorola.reflex");
            public static readonly Text VendorMs_Mediapackage = new Text("vnd.ms-mediapackage");
            public static readonly Text VendorNet2phoneCommcenterCommand = new Text("vnd.net2phone.commcenter.command");
            public static readonly Text VendorRadisysMsml_Basic_Layout = new Text("vnd.radisys.msml-basic-layout");
            public static readonly Text VendorSenxWarpscript = new Text("vnd.senx.warpscript");

            [System.Obsolete("OBSOLETED by request")]
            public static readonly Text VendorSiUricatalogue = new Text("vnd.si.uricatalogue");

            public static readonly Text VendorSosi = new Text("vnd.sosi");
            public static readonly Text VendorSunJ2meApp_Descriptor = new Text("vnd.sun.j2me.app-descriptor", new string[] { "jad" });
            public static readonly Text VendorTrolltechLinguist = new Text("vnd.trolltech.linguist");
            public static readonly Text VendorWapSi = new Text("vnd.wap.si");
            public static readonly Text VendorWapSl = new Text("vnd.wap.sl");
            public static readonly Text VendorWapWml = new Text("vnd.wap.wml", new string[] { "wml" });
            public static readonly Text VendorWapWmlscript = new Text("vnd.wap.wmlscript", new string[] { "wmls" });
            public static readonly Text X_Asm = new Text("x-asm", new string[] { "s", "asm" });
            public static readonly Text X_C = new Text("x-c", new string[] { "c", "cc", "cxx", "cpp", "h", "hh", "dic" });
            public static readonly Text X_Fortran = new Text("x-fortran", new string[] { "f", "for", "f77", "f90" });
            public static readonly Text X_Java_Source = new Text("x-java-source", new string[] { "java" });
            public static readonly Text X_Nfo = new Text("x-nfo", new string[] { "nfo" });
            public static readonly Text X_Opml = new Text("x-opml", new string[] { "opml" });
            public static readonly Text X_Pascal = new Text("x-pascal", new string[] { "p", "pas" });
            public static readonly Text X_Setext = new Text("x-setext", new string[] { "etx" });
            public static readonly Text X_Sfv = new Text("x-sfv", new string[] { "sfv" });
            public static readonly Text X_Uuencode = new Text("x-uuencode", new string[] { "uu" });
            public static readonly Text X_Vcalendar = new Text("x-vcalendar", new string[] { "vcs" });
            public static readonly Text X_Vcard = new Text("x-vcard", new string[] { "vcf" });
            public static readonly Text Xml = new Text("xml");
            public static readonly Text Xml_External_Parsed_Entity = new Text("xml-external-parsed-entity");

            new public static readonly Text[] Values = {
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
