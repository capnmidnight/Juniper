import { specialize } from "./util";

const text = /*@__PURE__*/ specialize("text");

export const Text_Cache_Manifest = /*@__PURE__*/ text("cache-manifest", "appcache");
export const Text_Calendar = /*@__PURE__*/ text("calendar", "ics", "ifb");
export const Text_Calender = /*@__PURE__*/ text("calender");
export const Text_Cmd = /*@__PURE__*/ text("cmd");
export const Text_Coffeescript = /*@__PURE__*/ text("coffeescript", "coffee", "litcoffee");
export const Text_Css = /*@__PURE__*/ text("css", "css");
export const Text_Csv = /*@__PURE__*/ text("csv", "csv");
export const Text_Csv_Schema = /*@__PURE__*/ text("csv-schema");
export const Text_Directory = /*@__PURE__*/ text("directory").deprecate("by RFC6350");
export const Text_Dns = /*@__PURE__*/ text("dns");
export const Text_Ecmascript = /*@__PURE__*/ text("ecmascript").deprecate("in favor of application/ecmascript");
export const Text_Encaprtp = /*@__PURE__*/ text("encaprtp");
export const Text_Enriched = /*@__PURE__*/ text("enriched");
export const Text_Event_Stream = /*@__PURE__*/ text("event-stream");
export const Text_Example = /*@__PURE__*/ text("example");
export const Text_Flexfec = /*@__PURE__*/ text("flexfec");
export const Text_Fwdred = /*@__PURE__*/ text("fwdred");
export const Text_Grammar_Ref_List = /*@__PURE__*/ text("grammar-ref-list");
export const Text_Html = /*@__PURE__*/ text("html", "html", "htm");
export const Text_Jade = /*@__PURE__*/ text("jade", "jade");
export const Text_Javascript = /*@__PURE__*/ text("javascript").deprecate("in favor of application/javascript");
export const Text_Jcr_Cnd = /*@__PURE__*/ text("jcr-cnd");
export const Text_Jsx = /*@__PURE__*/ text("jsx", "jsx");
export const Text_Less = /*@__PURE__*/ text("less", "less");
export const Text_Markdown = /*@__PURE__*/ text("markdown");
export const Text_Mdx = /*@__PURE__*/ text("mdx", "mdx");
export const Text_Mizar = /*@__PURE__*/ text("mizar");
export const Text_N3 = /*@__PURE__*/ text("n3", "n3");
export const Text_Parameters = /*@__PURE__*/ text("parameters");
export const Text_Parityfec = /*@__PURE__*/ text("parityfec");
export const Text_Plain = /*@__PURE__*/ text("plain", "txt", "text", "conf", "def", "list", "log", "in");
export const Text_PlainUTF8 = /*@__PURE__*/ text("plain; charset=UTF-8", "txt", "text", "conf", "def", "list", "log", "in");
export const Text_Provenance_Notation = /*@__PURE__*/ text("provenance-notation");
export const Text_PrsFallensteinRst = /*@__PURE__*/ text("prs.fallenstein.rst");
export const Text_PrsLinesTag = /*@__PURE__*/ text("prs.lines.tag", "dsc");
export const Text_PrsPropLogic = /*@__PURE__*/ text("prs.prop.logic");
export const Text_Raptorfec = /*@__PURE__*/ text("raptorfec");
export const Text_RED = /*@__PURE__*/ text("red");
export const Text_Rfc822_Headers = /*@__PURE__*/ text("rfc822-headers");
export const Text_Richtext = /*@__PURE__*/ text("richtext", "rtx");
export const Text_Rtf = /*@__PURE__*/ text("rtf");
export const Text_Rtp_Enc_Aescm128 = /*@__PURE__*/ text("rtp-enc-aescm128");
export const Text_Rtploopback = /*@__PURE__*/ text("rtploopback");
export const Text_Rtx = /*@__PURE__*/ text("rtx");
export const Text_Sgml = /*@__PURE__*/ text("sgml", "sgml", "sgm");
export const Text_Shex = /*@__PURE__*/ text("shex", "shex");
export const Text_Slim = /*@__PURE__*/ text("slim", "slim", "slm");
export const Text_Strings = /*@__PURE__*/ text("strings");
export const Text_Stylus = /*@__PURE__*/ text("stylus", "stylus", "styl");
export const Text_T140 = /*@__PURE__*/ text("t140");
export const Text_Tab_Separated_Values = /*@__PURE__*/ text("tab-separated-values", "tsv");
export const Text_Troff = /*@__PURE__*/ text("troff", "t", "tr", "roff", "man", "me", "ms");
export const Text_Turtle = /*@__PURE__*/ text("turtle", "ttl");
export const Text_Ulpfec = /*@__PURE__*/ text("ulpfec");
export const Text_Uri_List = /*@__PURE__*/ text("uri-list", "uri", "uris", "urls");
export const Text_Vcard = /*@__PURE__*/ text("vcard", "vcard");
export const Text_Vendor_1d_Interleaved_Parityfec = /*@__PURE__*/ text("1d-interleaved-parityfec");
export const Text_Vendor_A = /*@__PURE__*/ text("vnd.a");
export const Text_Vendor_Abc = /*@__PURE__*/ text("vnd.abc");
export const Text_Vendor_Ascii_Art = /*@__PURE__*/ text("vnd.ascii-art");
export const Text_Vendor_Curl = /*@__PURE__*/ text("vnd.curl", "curl");
export const Text_Vendor_CurlDcurl = /*@__PURE__*/ text("vnd.curl.dcurl", "dcurl");
export const Text_Vendor_CurlMcurl = /*@__PURE__*/ text("vnd.curl.mcurl", "mcurl");
export const Text_Vendor_CurlScurl = /*@__PURE__*/ text("vnd.curl.scurl", "scurl");
export const Text_Vendor_DebianCopyright = /*@__PURE__*/ text("vnd.debian.copyright");
export const Text_Vendor_DMClientScript = /*@__PURE__*/ text("vnd.dmclientscript");
export const Text_Vendor_DvbSubtitle = /*@__PURE__*/ text("vnd.dvb.subtitle", "sub");
export const Text_Vendor_EsmertecTheme_Descriptor = /*@__PURE__*/ text("vnd.esmertec.theme-descriptor");
export const Text_Vendor_FiclabFlt = /*@__PURE__*/ text("vnd.ficlab.flt");
export const Text_Vendor_Fly = /*@__PURE__*/ text("vnd.fly", "fly");
export const Text_Vendor_FmiFlexstor = /*@__PURE__*/ text("vnd.fmi.flexstor", "flx");
export const Text_Vendor_Gml = /*@__PURE__*/ text("vnd.gml");
export const Text_Vendor_Graphviz = /*@__PURE__*/ text("vnd.graphviz", "gv");
export const Text_Vendor_Hgl = /*@__PURE__*/ text("vnd.hgl");
export const Text_Vendor_In3d3dml = /*@__PURE__*/ text("vnd.in3d.3dml", "3dml");
export const Text_Vendor_In3dSpot = /*@__PURE__*/ text("vnd.in3d.spot", "spot");
export const Text_Vendor_IPTCNewsML = /*@__PURE__*/ text("vnd.iptc.newsml");
export const Text_Vendor_IPTCNITF = /*@__PURE__*/ text("vnd.iptc.nitf");
export const Text_Vendor_Latex_Z = /*@__PURE__*/ text("vnd.latex-z");
export const Text_Vendor_MotorolaReflex = /*@__PURE__*/ text("vnd.motorola.reflex");
export const Text_Vendor_Ms_Mediapackage = /*@__PURE__*/ text("vnd.ms-mediapackage");
export const Text_Vendor_Net2phoneCommcenterCommand = /*@__PURE__*/ text("vnd.net2phone.commcenter.command");
export const Text_Vendor_RadisysMsml_Basic_Layout = /*@__PURE__*/ text("vnd.radisys.msml-basic-layout");
export const Text_Vendor_SenxWarpscript = /*@__PURE__*/ text("vnd.senx.warpscript");
export const Text_Vendor_SiUricatalogue = /*@__PURE__*/ text("vnd.si.uricatalogue").deprecate("by request");
export const Text_Vendor_Sosi = /*@__PURE__*/ text("vnd.sosi");
export const Text_Vendor_SunJ2meApp_Descriptor = /*@__PURE__*/ text("vnd.sun.j2me.app-descriptor", "jad");
export const Text_Vendor_TrolltechLinguist = /*@__PURE__*/ text("vnd.trolltech.linguist");
export const Text_Vendor_WapSi = /*@__PURE__*/ text("vnd.wap.si");
export const Text_Vendor_WapSl = /*@__PURE__*/ text("vnd.wap.sl");
export const Text_Vendor_WapWml = /*@__PURE__*/ text("vnd.wap.wml", "wml");
export const Text_Vendor_WapWmlscript = /*@__PURE__*/ text("vnd.wap.wmlscript", "wmls");
export const Text_Vtt = /*@__PURE__*/ text("vtt");
export const Text_Wgsl = /*@__PURE__*/ text("wgsl", "wgsl");
export const Text_X_Asm = /*@__PURE__*/ text("x-asm", "s", "asm");
export const Text_X_C = /*@__PURE__*/ text("x-c", "c", "cc", "cxx", "cpp", "h", "hh", "dic");
export const Text_X_Fortran = /*@__PURE__*/ text("x-fortran", "f", "for", "f77", "f90");
export const Text_X_Gwt_Rpc = /*@__PURE__*/ text("x-gwt-rpc");
export const Text_X_Handlebars_Template = /*@__PURE__*/ text("x-handlebars-template", "hbs");
export const Text_X_Java_Source = /*@__PURE__*/ text("x-java-source", "java");
export const Text_X_Jquery_Tmpl = /*@__PURE__*/ text("x-jquery-tmpl");
export const Text_X_Lua = /*@__PURE__*/ text("x-lua", "lua");
export const Text_X_Markdown = /*@__PURE__*/ text("x-markdown", "mkd");
export const Text_X_Nfo = /*@__PURE__*/ text("x-nfo", "nfo");
export const Text_X_Opml = /*@__PURE__*/ text("x-opml", "opml");
export const Text_X_Org = /*@__PURE__*/ text("x-org", "org");
export const Text_X_Pascal = /*@__PURE__*/ text("x-pascal", "p", "pas");
export const Text_X_Processing = /*@__PURE__*/ text("x-processing", "pde");
export const Text_X_Python = /*@__PURE__*/ text("x-python", "py");
export const Text_X_Sass = /*@__PURE__*/ text("x-sass", "sass");
export const Text_X_Script_Python = /*@__PURE__*/ text("x.script-python", "py");
export const Text_X_Scss = /*@__PURE__*/ text("x-scss", "scss");
export const Text_X_Setext = /*@__PURE__*/ text("x-setext", "etx");
export const Text_X_Sfv = /*@__PURE__*/ text("x-sfv", "sfv");
export const Text_X_Suse_Ymp = /*@__PURE__*/ text("x-suse-ymp", "ymp");
export const Text_X_Uuencode = /*@__PURE__*/ text("x-uuencode", "uu");
export const Text_X_Vcalendar = /*@__PURE__*/ text("x-vcalendar", "vcs");
export const Text_X_Vcard = /*@__PURE__*/ text("x-vcard", "vcf");
export const Text_Xml = /*@__PURE__*/ text("xml");
export const Text_Xml_External_Parsed_Entity = /*@__PURE__*/ text("xml-external-parsed-entity");
export const Text_Yaml = /*@__PURE__*/ text("yaml", "yaml", "yml");