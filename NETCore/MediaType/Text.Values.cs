namespace Juniper;

public partial class MediaType
{
    public static readonly Text Text_Cache_Manifest = new("cache-manifest", "appcache");
    public static readonly Text Text_Calendar = new("calendar", "ics", "ifb");
    public static readonly Text Text_Cmd = new("cmd");
    public static readonly Text Text_Coffeescript = new("coffeescript", "coffee", "litcoffee");
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
    public static readonly Text Text_Jade = new("jade", "jade");

    [System.Obsolete("OBSOLETED in favor of application/javascript")]
    public static readonly Text Text_Javascript = new("javascript");

    public static readonly Text Text_Jcr_Cnd = new("jcr-cnd");
    public static readonly Text Text_Jsx = new("jsx", "jsx");
    public static readonly Text Text_Less = new("less", "less");
    public static readonly Text Text_Markdown = new("markdown", "md");
    public static readonly Text Text_Mdx = new("mdx", "mdx");
    public static readonly Text Text_Mizar = new("mizar");
    public static readonly Text Text_N3 = new("n3", "n3");
    public static readonly Text Text_Parameters = new("parameters");
    public static readonly Text Text_Parityfec = new("parityfec");
    public static readonly Text Text_Plain = new("plain", "txt", "text", "conf", "def", "list", "log", "in");
    public static readonly Text Text_PlainUTF8 = new("plain; charset=UTF-8", "txt", "text", "conf", "def", "list", "log", "in");
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
    public static readonly Text Text_Shex = new("shex", "shex");
    public static readonly Text Text_Slim = new("slim", "slim", "slm");
    public static readonly Text Text_Strings = new("strings");
    public static readonly Text Text_Stylus = new("stylus", "stylus", "styl");
    public static readonly Text Text_T140 = new("t140");
    public static readonly Text Text_Tab_Separated_Values = new("tab-separated-values", "tsv");
    public static readonly Text Text_Troff = new("troff", "t", "tr", "roff", "man", "me", "ms");
    public static readonly Text Text_Turtle = new("turtle", "ttl");
    public static readonly Text Text_Ulpfec = new("ulpfec");
    public static readonly Text Text_Uri_List = new("uri-list", "uri", "uris", "urls");
    public static readonly Text Text_Vcard = new("vcard", "vcard");
    public static readonly Text Text_Vendor_1d_Interleaved_Parityfec = new("1d-interleaved-parityfec");
    public static readonly Text Text_Vendor_A = new("vnd.a");
    public static readonly Text Text_Vendor_Abc = new("vnd.abc");
    public static readonly Text Text_Vendor_Ascii_Art = new("vnd.ascii-art");
    public static readonly Text Text_Vendor_Curl = new("vnd.curl", "curl");
    public static readonly Text Text_Vendor_CurlDcurl = new("vnd.curl.dcurl", "dcurl");
    public static readonly Text Text_Vendor_CurlMcurl = new("vnd.curl.mcurl", "mcurl");
    public static readonly Text Text_Vendor_CurlScurl = new("vnd.curl.scurl", "scurl");
    public static readonly Text Text_Vendor_DebianCopyright = new("vnd.debian.copyright");
    public static readonly Text Text_Vendor_DMClientScript = new("vnd.dmclientscript");
    public static readonly Text Text_Vendor_DvbSubtitle = new("vnd.dvb.subtitle", "sub");
    public static readonly Text Text_Vendor_EsmertecTheme_Descriptor = new("vnd.esmertec.theme-descriptor");
    public static readonly Text Text_Vendor_FiclabFlt = new("vnd.ficlab.flt");
    public static readonly Text Text_Vendor_Fly = new("vnd.fly", "fly");
    public static readonly Text Text_Vendor_FmiFlexstor = new("vnd.fmi.flexstor", "flx");
    public static readonly Text Text_Vendor_Gml = new("vnd.gml");
    public static readonly Text Text_Vendor_Graphviz = new("vnd.graphviz", "gv");
    public static readonly Text Text_Vendor_Hgl = new("vnd.hgl");
    public static readonly Text Text_Vendor_In3d3dml = new("vnd.in3d.3dml", "3dml");
    public static readonly Text Text_Vendor_In3dSpot = new("vnd.in3d.spot", "spot");
    public static readonly Text Text_Vendor_IPTCNewsML = new("vnd.iptc.newsml");
    public static readonly Text Text_Vendor_IPTCNITF = new("vnd.iptc.nitf");
    public static readonly Text Text_Vendor_Latex_Z = new("vnd.latex-z");
    public static readonly Text Text_Vendor_MotorolaReflex = new("vnd.motorola.reflex");
    public static readonly Text Text_Vendor_Ms_Mediapackage = new("vnd.ms-mediapackage");
    public static readonly Text Text_Vendor_Net2phoneCommcenterCommand = new("vnd.net2phone.commcenter.command");
    public static readonly Text Text_Vendor_RadisysMsml_Basic_Layout = new("vnd.radisys.msml-basic-layout");
    public static readonly Text Text_Vendor_SenxWarpscript = new("vnd.senx.warpscript");

    [System.Obsolete("OBSOLETED by request")]
    public static readonly Text Text_Vendor_SiUricatalogue = new("vnd.si.uricatalogue");

    public static readonly Text Text_Vendor_Sosi = new("vnd.sosi");
    public static readonly Text Text_Vendor_SunJ2meApp_Descriptor = new("vnd.sun.j2me.app-descriptor", "jad");
    public static readonly Text Text_Vendor_TrolltechLinguist = new("vnd.trolltech.linguist");
    public static readonly Text Text_Vendor_WapSi = new("vnd.wap.si");
    public static readonly Text Text_Vendor_WapSl = new("vnd.wap.sl");
    public static readonly Text Text_Vendor_WapWml = new("vnd.wap.wml", "wml");
    public static readonly Text Text_Vendor_WapWmlscript = new("vnd.wap.wmlscript", "wmls");
    public static readonly Text Text_Vtt = new("vtt");
    public static readonly Text Text_Wgsl = new("wgsl", "wgsl");
    public static readonly Text Text_X_Asm = new("x-asm", "s", "asm");
    public static readonly Text Text_X_C = new("x-c", "c", "cc", "cxx", "cpp", "h", "hh", "dic");
    public static readonly Text Text_X_Fortran = new("x-fortran", "f", "for", "f77", "f90");
    public static readonly Text Text_X_Gwt_Rpc = new("x-gwt-rpc");
    public static readonly Text Text_X_Handlebars_Template = new("x-handlebars-template", "hbs");
    public static readonly Text Text_X_Java_Source = new("x-java-source", "java");
    public static readonly Text Text_X_JavaScript_Module = new("x-javascript-module", "mjs");
    public static readonly Text Text_X_Jqry_Tmpl = new("x-jquery-tmpl");
    public static readonly Text Text_X_Lua = new("x-lua", "lua");
    public static readonly Text Text_X_Markdown = new("x-markdown", "md");
    public static readonly Text Text_X_Nfo = new("x-nfo", "nfo");
    public static readonly Text Text_X_Opml = new("x-opml", "opml");
    public static readonly Text Text_X_Org = new("x-org", "org");
    public static readonly Text Text_X_Pascal = new("x-pascal", "p", "pas");
    public static readonly Text Text_X_PowerShell = new("x-powershell", "ps1");
    public static readonly Text Text_X_Processing = new("x-processing", "pde");
    public static readonly Text Text_X_Python = new("x-python", "py");
    public static readonly Text Text_X_Sass = new("x-sass", "sass");
    public static readonly Text Text_X_Scss = new("x-scss", "scss");
    public static readonly Text Text_X_Script_Python = new("x.script-python", "py");
    public static readonly Text Text_X_Setext = new("x-setext", "etx");
    public static readonly Text Text_X_Sfv = new("x-sfv", "sfv");
    public static readonly Text Text_X_Suse_Ymp = new("x-suse-ymp", "ymp");
    public static readonly Text Text_X_Uuencode = new("x-uuencode", "uu");
    public static readonly Text Text_X_Vcalendar = new("x-vcalendar", "vcs");
    public static readonly Text Text_X_Vcard = new("x-vcard", "vcf");
    public static readonly Text Text_Xml = new("xml");
    public static readonly Text Text_Xml_External_Parsed_Entity = new("xml-external-parsed-entity");
    public static readonly Text Text_Yaml = new("yaml", "yaml", "yml");
}
