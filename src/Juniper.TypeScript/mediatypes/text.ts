import { deprecate, specialize } from "./util";

const text = specialize("text");

export const anyText = text("*");
export const Text_Cache_Manifest = text("cache-manifest", "appcache");
export const Text_Calendar = text("calendar", "ics", "ifb");
export const Text_Calender = text("calender");
export const Text_Cmd = text("cmd");
export const Text_Coffeescript = text("coffeescript", "coffee", "litcoffee");
export const Text_Css = text("css", "css");
export const Text_Csv = text("csv", "csv");
export const Text_Csv_Schema = text("csv-schema");
export const Text_Directory = deprecate(text("directory"), "by RFC6350");
export const Text_Dns = text("dns");
export const Text_Ecmascript = deprecate(text("ecmascript"), "in favor of application/ecmascript");
export const Text_Encaprtp = text("encaprtp");
export const Text_Enriched = text("enriched");
export const Text_Example = text("example");
export const Text_Flexfec = text("flexfec");
export const Text_Fwdred = text("fwdred");
export const Text_Grammar_Ref_List = text("grammar-ref-list");
export const Text_Html = text("html", "html", "htm");
export const Text_Jade = text("jade", "jade");
export const Text_Javascript = deprecate(text("javascript"), "in favor of application/javascript");
export const Text_Jcr_Cnd = text("jcr-cnd");
export const Text_Jsx = text("jsx", "jsx");
export const Text_Less = text("less", "less");
export const Text_Markdown = text("markdown");
export const Text_Mdx = text("mdx", "mdx");
export const Text_Mizar = text("mizar");
export const Text_N3 = text("n3", "n3");
export const Text_Parameters = text("parameters");
export const Text_Parityfec = text("parityfec");
export const Text_Plain = text("plain", "txt", "text", "conf", "def", "list", "log", "in");
export const Text_Provenance_Notation = text("provenance-notation");
export const Text_PrsFallensteinRst = text("prs.fallenstein.rst");
export const Text_PrsLinesTag = text("prs.lines.tag", "dsc");
export const Text_PrsPropLogic = text("prs.prop.logic");
export const Text_Raptorfec = text("raptorfec");
export const Text_RED = text("red");
export const Text_Rfc822_Headers = text("rfc822-headers");
export const Text_Richtext = text("richtext", "rtx");
export const Text_Rtf = text("rtf");
export const Text_Rtp_Enc_Aescm128 = text("rtp-enc-aescm128");
export const Text_Rtploopback = text("rtploopback");
export const Text_Rtx = text("rtx");
export const Text_Sgml = text("sgml", "sgml", "sgm");
export const Text_Shex = text("shex", "shex");
export const Text_Slim = text("slim", "slim", "slm");
export const Text_Strings = text("strings");
export const Text_Stylus = text("stylus", "stylus", "styl");
export const Text_T140 = text("t140");
export const Text_Tab_Separated_Values = text("tab-separated-values", "tsv");
export const Text_Troff = text("troff", "t", "tr", "roff", "man", "me", "ms");
export const Text_Turtle = text("turtle", "ttl");
export const Text_Ulpfec = text("ulpfec");
export const Text_Uri_List = text("uri-list", "uri", "uris", "urls");
export const Text_Vcard = text("vcard", "vcard");
export const Text_Vendor_1d_Interleaved_Parityfec = text("1d-interleaved-parityfec");
export const Text_Vendor_A = text("vnd.a");
export const Text_Vendor_Abc = text("vnd.abc");
export const Text_Vendor_Ascii_Art = text("vnd.ascii-art");
export const Text_Vendor_Curl = text("vnd.curl", "curl");
export const Text_Vendor_CurlDcurl = text("vnd.curl.dcurl", "dcurl");
export const Text_Vendor_CurlMcurl = text("vnd.curl.mcurl", "mcurl");
export const Text_Vendor_CurlScurl = text("vnd.curl.scurl", "scurl");
export const Text_Vendor_DebianCopyright = text("vnd.debian.copyright");
export const Text_Vendor_DMClientScript = text("vnd.dmclientscript");
export const Text_Vendor_DvbSubtitle = text("vnd.dvb.subtitle", "sub");
export const Text_Vendor_EsmertecTheme_Descriptor = text("vnd.esmertec.theme-descriptor");
export const Text_Vendor_FiclabFlt = text("vnd.ficlab.flt");
export const Text_Vendor_Fly = text("vnd.fly", "fly");
export const Text_Vendor_FmiFlexstor = text("vnd.fmi.flexstor", "flx");
export const Text_Vendor_Gml = text("vnd.gml");
export const Text_Vendor_Graphviz = text("vnd.graphviz", "gv");
export const Text_Vendor_Hgl = text("vnd.hgl");
export const Text_Vendor_In3d3dml = text("vnd.in3d.3dml", "3dml");
export const Text_Vendor_In3dSpot = text("vnd.in3d.spot", "spot");
export const Text_Vendor_IPTCNewsML = text("vnd.iptc.newsml");
export const Text_Vendor_IPTCNITF = text("vnd.iptc.nitf");
export const Text_Vendor_Latex_Z = text("vnd.latex-z");
export const Text_Vendor_MotorolaReflex = text("vnd.motorola.reflex");
export const Text_Vendor_Ms_Mediapackage = text("vnd.ms-mediapackage");
export const Text_Vendor_Net2phoneCommcenterCommand = text("vnd.net2phone.commcenter.command");
export const Text_Vendor_RadisysMsml_Basic_Layout = text("vnd.radisys.msml-basic-layout");
export const Text_Vendor_SenxWarpscript = text("vnd.senx.warpscript");
export const Text_Vendor_SiUricatalogue = deprecate(text("vnd.si.uricatalogue"), "by request");
export const Text_Vendor_Sosi = text("vnd.sosi");
export const Text_Vendor_SunJ2meApp_Descriptor = text("vnd.sun.j2me.app-descriptor", "jad");
export const Text_Vendor_TrolltechLinguist = text("vnd.trolltech.linguist");
export const Text_Vendor_WapSi = text("vnd.wap.si");
export const Text_Vendor_WapSl = text("vnd.wap.sl");
export const Text_Vendor_WapWml = text("vnd.wap.wml", "wml");
export const Text_Vendor_WapWmlscript = text("vnd.wap.wmlscript", "wmls");
export const Text_Vtt = text("vtt");
export const Text_X_Asm = text("x-asm", "s", "asm");
export const Text_X_C = text("x-c", "c", "cc", "cxx", "cpp", "h", "hh", "dic");
export const Text_X_Fortran = text("x-fortran", "f", "for", "f77", "f90");
export const Text_X_Gwt_Rpc = text("x-gwt-rpc");
export const Text_X_Handlebars_Template = text("x-handlebars-template", "hbs");
export const Text_X_Java_Source = text("x-java-source", "java");
export const Text_X_Jquery_Tmpl = text("x-jquery-tmpl");
export const Text_X_Lua = text("x-lua", "lua");
export const Text_X_Markdown = text("x-markdown", "mkd");
export const Text_X_Nfo = text("x-nfo", "nfo");
export const Text_X_Opml = text("x-opml", "opml");
export const Text_X_Org = text("x-org", "org");
export const Text_X_Pascal = text("x-pascal", "p", "pas");
export const Text_X_Processing = text("x-processing", "pde");
export const Text_X_Sass = text("x-sass", "sass");
export const Text_X_Scss = text("x-scss", "scss");
export const Text_X_Setext = text("x-setext", "etx");
export const Text_X_Sfv = text("x-sfv", "sfv");
export const Text_X_Suse_Ymp = text("x-suse-ymp", "ymp");
export const Text_X_Uuencode = text("x-uuencode", "uu");
export const Text_X_Vcalendar = text("x-vcalendar", "vcs");
export const Text_X_Vcard = text("x-vcard", "vcf");
export const Text_Xml = text("xml");
export const Text_Xml_External_Parsed_Entity = text("xml-external-parsed-entity");
export const Text_Yaml = text("yaml", "yaml", "yml");
export const allText = [
    Text_Cache_Manifest,
    Text_Calendar,
    Text_Calender,
    Text_Cmd,
    Text_Coffeescript,
    Text_Css,
    Text_Csv,
    Text_Csv_Schema,
    Text_Dns,
    Text_Encaprtp,
    Text_Enriched,
    Text_Example,
    Text_Flexfec,
    Text_Fwdred,
    Text_Grammar_Ref_List,
    Text_Html,
    Text_Jade,
    Text_Jcr_Cnd,
    Text_Jsx,
    Text_Less,
    Text_Markdown,
    Text_Mdx,
    Text_Mizar,
    Text_N3,
    Text_Parameters,
    Text_Parityfec,
    Text_Plain,
    Text_Provenance_Notation,
    Text_PrsFallensteinRst,
    Text_PrsLinesTag,
    Text_PrsPropLogic,
    Text_Raptorfec,
    Text_RED,
    Text_Rfc822_Headers,
    Text_Richtext,
    Text_Rtf,
    Text_Rtp_Enc_Aescm128,
    Text_Rtploopback,
    Text_Rtx,
    Text_Sgml,
    Text_Shex,
    Text_Slim,
    Text_Strings,
    Text_Stylus,
    Text_T140,
    Text_Tab_Separated_Values,
    Text_Troff,
    Text_Turtle,
    Text_Ulpfec,
    Text_Uri_List,
    Text_Vcard,
    Text_Vendor_1d_Interleaved_Parityfec,
    Text_Vendor_A,
    Text_Vendor_Abc,
    Text_Vendor_Ascii_Art,
    Text_Vendor_Curl,
    Text_Vendor_CurlDcurl,
    Text_Vendor_CurlMcurl,
    Text_Vendor_CurlScurl,
    Text_Vendor_DebianCopyright,
    Text_Vendor_DMClientScript,
    Text_Vendor_DvbSubtitle,
    Text_Vendor_EsmertecTheme_Descriptor,
    Text_Vendor_FiclabFlt,
    Text_Vendor_Fly,
    Text_Vendor_FmiFlexstor,
    Text_Vendor_Gml,
    Text_Vendor_Graphviz,
    Text_Vendor_Hgl,
    Text_Vendor_In3d3dml,
    Text_Vendor_In3dSpot,
    Text_Vendor_IPTCNewsML,
    Text_Vendor_IPTCNITF,
    Text_Vendor_Latex_Z,
    Text_Vendor_MotorolaReflex,
    Text_Vendor_Ms_Mediapackage,
    Text_Vendor_Net2phoneCommcenterCommand,
    Text_Vendor_RadisysMsml_Basic_Layout,
    Text_Vendor_SenxWarpscript,
    Text_Vendor_Sosi,
    Text_Vendor_SunJ2meApp_Descriptor,
    Text_Vendor_TrolltechLinguist,
    Text_Vendor_WapSi,
    Text_Vendor_WapSl,
    Text_Vendor_WapWml,
    Text_Vendor_WapWmlscript,
    Text_Vtt,
    Text_X_Asm,
    Text_X_C,
    Text_X_Fortran,
    Text_X_Gwt_Rpc,
    Text_X_Handlebars_Template,
    Text_X_Java_Source,
    Text_X_Jquery_Tmpl,
    Text_X_Lua,
    Text_X_Markdown,
    Text_X_Nfo,
    Text_X_Opml,
    Text_X_Org,
    Text_X_Pascal,
    Text_X_Processing,
    Text_X_Sass,
    Text_X_Scss,
    Text_X_Setext,
    Text_X_Sfv,
    Text_X_Suse_Ymp,
    Text_X_Uuencode,
    Text_X_Vcalendar,
    Text_X_Vcard,
    Text_Xml,
    Text_Xml_External_Parsed_Entity,
    Text_Yaml,
];