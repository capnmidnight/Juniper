namespace Juniper
{
    public partial class MediaType
    {
        public sealed partial class Application : MediaType
        {
            public static readonly Application A2L = new("a2l");
            public static readonly Application Activemessage = new("activemessage");
            public static readonly Application ActivityJson = new("activity+json", new string[] { "json" });
            public static readonly Application Alto_CostmapfilterJson = new("alto-costmapfilter+json", new string[] { "json" });
            public static readonly Application Alto_CostmapJson = new("alto-costmap+json", new string[] { "json" });
            public static readonly Application Alto_DirectoryJson = new("alto-directory+json", new string[] { "json" });
            public static readonly Application Alto_EndpointcostJson = new("alto-endpointcost+json", new string[] { "json" });
            public static readonly Application Alto_EndpointcostparamsJson = new("alto-endpointcostparams+json", new string[] { "json" });
            public static readonly Application Alto_EndpointpropJson = new("alto-endpointprop+json", new string[] { "json" });
            public static readonly Application Alto_EndpointpropparamsJson = new("alto-endpointpropparams+json", new string[] { "json" });
            public static readonly Application Alto_ErrorJson = new("alto-error+json", new string[] { "json" });
            public static readonly Application Alto_NetworkmapfilterJson = new("alto-networkmapfilter+json", new string[] { "json" });
            public static readonly Application Alto_NetworkmapJson = new("alto-networkmap+json", new string[] { "json" });
            public static readonly Application AML = new("aml");
            public static readonly Application Andrew_Inset = new("andrew-inset", new string[] { "ez" });
            public static readonly Application Applefile = new("applefile");
            public static readonly Application Applixware = new("applixware", new string[] { "aw" });
            public static readonly Application ATF = new("atf");
            public static readonly Application ATFX = new("atfx");
            public static readonly Application AtomcatXml = new("atomcat+xml", new string[] { "atomcat" });
            public static readonly Application AtomdeletedXml = new("atomdeleted+xml", new string[] { "xml" });
            public static readonly Application Atomicmail = new("atomicmail");
            public static readonly Application AtomsvcXml = new("atomsvc+xml", new string[] { "atomsvc" });
            public static readonly Application AtomXml = new("atom+xml", new string[] { "atom" });
            public static readonly Application Atsc_DwdXml = new("atsc-dwd+xml", new string[] { "xml" });
            public static readonly Application Atsc_HeldXml = new("atsc-held+xml", new string[] { "xml" });
            public static readonly Application Atsc_RdtJson = new("atsc-rdt+json", new string[] { "json" });
            public static readonly Application Atsc_RsatXml = new("atsc-rsat+xml", new string[] { "xml" });
            public static readonly Application ATXML = new("atxml");
            public static readonly Application Auth_PolicyXml = new("auth-policy+xml", new string[] { "xml" });
            public static readonly Application Bacnet_XddZip = new("bacnet-xdd+zip", new string[] { "zip" });
            public static readonly Application Batch_SMTP = new("batch-smtp");
            public static readonly Application BeepXml = new("beep+xml", new string[] { "xml" });
            public static readonly Application CalendarJson = new("calendar+json", new string[] { "json" });
            public static readonly Application CalendarXml = new("calendar+xml", new string[] { "xml" });
            public static readonly Application Call_Completion = new("call-completion");
            public static readonly Application CALS_1840 = new("cals-1840");
            public static readonly Application Cbor = new("cbor");
            public static readonly Application Cbor_Seq = new("cbor-seq");
            public static readonly Application Cccex = new("cccex");
            public static readonly Application CcmpXml = new("ccmp+xml", new string[] { "xml" });
            public static readonly Application CcxmlXml = new("ccxml+xml", new string[] { "ccxml" });
            public static readonly Application CDFXXML = new("cdfx+xml", new string[] { "xml" });
            public static readonly Application Cdmi_Capability = new("cdmi-capability", new string[] { "cdmia" });
            public static readonly Application Cdmi_Container = new("cdmi-container", new string[] { "cdmic" });
            public static readonly Application Cdmi_Domain = new("cdmi-domain", new string[] { "cdmid" });
            public static readonly Application Cdmi_Object = new("cdmi-object", new string[] { "cdmio" });
            public static readonly Application Cdmi_Queue = new("cdmi-queue", new string[] { "cdmiq" });
            public static readonly Application Cdni = new("cdni");
            public static readonly Application CEA = new("cea");
            public static readonly Application Cea_2018Xml = new("cea-2018+xml", new string[] { "xml" });
            public static readonly Application CellmlXml = new("cellml+xml", new string[] { "xml" });
            public static readonly Application Cfw = new("cfw");
            public static readonly Application Clue_infoXml = new("clue_info+xml", new string[] { "xml" });
            public static readonly Application ClueXml = new("clue+xml", new string[] { "xml" });
            public static readonly Application Cms = new("cms");
            public static readonly Application CnrpXml = new("cnrp+xml", new string[] { "xml" });
            public static readonly Application Coap_GroupJson = new("coap-group+json", new string[] { "json" });
            public static readonly Application Coap_Payload = new("coap-payload");
            public static readonly Application Commonground = new("commonground");
            public static readonly Application Conference_InfoXml = new("conference-info+xml", new string[] { "xml" });
            public static readonly Application Cose = new("cose");
            public static readonly Application Cose_Key = new("cose-key");
            public static readonly Application Cose_Key_Set = new("cose-key-set");
            public static readonly Application CplXml = new("cpl+xml", new string[] { "xml" });
            public static readonly Application Csrattrs = new("csrattrs");
            public static readonly Application CSTAdataXml = new("cstadata+xml", new string[] { "xml" });
            public static readonly Application CstaXml = new("csta+xml", new string[] { "xml" });
            public static readonly Application CsvmJson = new("csvm+json", new string[] { "json" });
            public static readonly Application Cu_Seeme = new("cu-seeme", new string[] { "cu" });
            public static readonly Application Cwt = new("cwt");
            public static readonly Application Cybercash = new("cybercash");
            public static readonly Application Dashdelta = new("dashdelta");
            public static readonly Application DashXml = new("dash+xml", new string[] { "xml" });
            public static readonly Application DavmountXml = new("davmount+xml", new string[] { "davmount" });
            public static readonly Application Dca_Rft = new("dca-rft");
            public static readonly Application DCD = new("dcd");
            public static readonly Application Dec_Dx = new("dec-dx");
            public static readonly Application Dialog_InfoXml = new("dialog-info+xml", new string[] { "xml" });
            public static readonly Application Dicom = new("dicom");
            public static readonly Application DicomJson = new("dicom+json", new string[] { "json" });
            public static readonly Application DicomXml = new("dicom+xml", new string[] { "xml" });
            public static readonly Application DII = new("dii");
            public static readonly Application DIT = new("dit");
            public static readonly Application Dns = new("dns");
            public static readonly Application Dns_Message = new("dns-message");
            public static readonly Application DnsJson = new("dns+json", new string[] { "json" });
            public static readonly Application DocbookXml = new("docbook+xml", new string[] { "dbk" });
            public static readonly Application DotsCbor = new("dots+cbor", new string[] { "cbor" });
            public static readonly Application DskppXml = new("dskpp+xml", new string[] { "xml" });
            public static readonly Application DsscDer = new("dssc+der", new string[] { "dssc" });
            public static readonly Application DsscXml = new("dssc+xml", new string[] { "xdssc" });
            public static readonly Application Dvcs = new("dvcs");
            public static readonly Application Ecmascript = new("ecmascript", new string[] { "ecma" });
            public static readonly Application EDI_Consent = new("edi-consent");
            public static readonly Application EDI_X12 = new("edi-x12");
            public static readonly Application EDIFACT = new("edifact");
            public static readonly Application Efi = new("efi");
            public static readonly Application EmergencyCallDataCommentXml = new("emergencycalldata.comment+xml", new string[] { "xml" });
            public static readonly Application EmergencyCallDataControlXml = new("emergencycalldata.control+xml", new string[] { "xml" });
            public static readonly Application EmergencyCallDataDeviceInfoXml = new("emergencycalldata.deviceinfo+xml", new string[] { "xml" });
            public static readonly Application EmergencyCallDataECallMSD = new("emergencycalldata.ecall.msd");
            public static readonly Application EmergencyCallDataProviderInfoXml = new("emergencycalldata.providerinfo+xml", new string[] { "xml" });
            public static readonly Application EmergencyCallDataServiceInfoXml = new("emergencycalldata.serviceinfo+xml", new string[] { "xml" });
            public static readonly Application EmergencyCallDataSubscriberInfoXml = new("emergencycalldata.subscriberinfo+xml", new string[] { "xml" });
            public static readonly Application EmergencyCallDataVEDSXml = new("emergencycalldata.veds+xml", new string[] { "xml" });
            public static readonly Application EmmaXml = new("emma+xml", new string[] { "emma" });
            public static readonly Application EmotionmlXml = new("emotionml+xml", new string[] { "xml" });
            public static readonly Application Encaprtp = new("encaprtp");
            public static readonly Application EppXml = new("epp+xml", new string[] { "xml" });
            public static readonly Application EpubZip = new("epub+zip", new string[] { "epub" });
            public static readonly Application Eshop = new("eshop");
            public static readonly Application Example = new("example");
            public static readonly Application Exi = new("exi", new string[] { "exi" });
            public static readonly Application Expect_Ct_ReportJson = new("expect-ct-report+json", new string[] { "json" });
            public static readonly Application Fastinfoset = new("fastinfoset");
            public static readonly Application Fastsoap = new("fastsoap");
            public static readonly Application FdtXml = new("fdt+xml", new string[] { "xml" });
            public static readonly Application FhirJson = new("fhir+json", new string[] { "json" });
            public static readonly Application FhirXml = new("fhir+xml", new string[] { "xml" });
            public static readonly Application Fits = new("fits");
            public static readonly Application Flexfec = new("flexfec");

            [System.Obsolete("DEPRECATED in favor of font/sfnt")]
            public static readonly Application Font_Sfnt = new("font-sfnt");

            public static readonly Application Font_Tdpfr = new("font-tdpfr", new string[] { "pfr" });

            [System.Obsolete("DEPRECATED in favor of font/woff")]
            public static readonly Application Font_Woff = new("font-woff");

            public static readonly Application Framework_AttributesXml = new("framework-attributes+xml", new string[] { "xml" });
            public static readonly Application GeoJson = new("geo+json", new string[] { "json" });
            public static readonly Application GeoJson_Seq = new("geo+json-seq", new string[] { "json-seq" });
            public static readonly Application GeopackageSqlite3 = new("geopackage+sqlite3", new string[] { "sqlite3" });
            public static readonly Application GeoxacmlXml = new("geoxacml+xml", new string[] { "xml" });
            public static readonly Application Gltf_Buffer = new("gltf-buffer");
            public static readonly Application GmlXml = new("gml+xml", new string[] { "gml" });
            public static readonly Application GpxXml = new("gpx+xml", new string[] { "gpx" });
            public static readonly Application Gxf = new("gxf", new string[] { "gxf" });
            public static readonly Application Gzip = new("gzip");
            public static readonly Application H224 = new("h224");
            public static readonly Application HeldXml = new("held+xml", new string[] { "xml" });
            public static readonly Application Http = new("http");
            public static readonly Application Hyperstudio = new("hyperstudio", new string[] { "stk" });
            public static readonly Application Ibe_Key_RequestXml = new("ibe-key-request+xml", new string[] { "xml" });
            public static readonly Application Ibe_Pkg_ReplyXml = new("ibe-pkg-reply+xml", new string[] { "xml" });
            public static readonly Application Ibe_Pp_Data = new("ibe-pp-data");
            public static readonly Application Iges = new("iges");
            public static readonly Application Im_IscomposingXml = new("im-iscomposing+xml", new string[] { "xml" });
            public static readonly Application Index = new("index");
            public static readonly Application IndexCmd = new("index.cmd");
            public static readonly Application IndexObj = new("index.obj");
            public static readonly Application IndexResponse = new("index.response");
            public static readonly Application IndexVnd = new("index.vnd");
            public static readonly Application InkmlXml = new("inkml+xml", new string[] { "ink", "inkml" });
            public static readonly Application IOTP = new("iotp");
            public static readonly Application Ipfix = new("ipfix", new string[] { "ipfix" });
            public static readonly Application Ipp = new("ipp");
            public static readonly Application ISUP = new("isup");
            public static readonly Application ItsXml = new("its+xml", new string[] { "xml" });
            public static readonly Application Java_Archive = new("java-archive", new string[] { "jar" });
            public static readonly Application Java_Serialized_Object = new("java-serialized-object", new string[] { "ser" });
            public static readonly Application Java_Vm = new("java-vm", new string[] { "class" });
            public static readonly Application Javascript = new("javascript", new string[] { "js" });
            public static readonly Application Jf2feedJson = new("jf2feed+json", new string[] { "json" });
            public static readonly Application Jose = new("jose");
            public static readonly Application JoseJson = new("jose+json", new string[] { "json" });
            public static readonly Application JrdJson = new("jrd+json", new string[] { "json" });
            public static readonly Application Json = new("json", new string[] { "json" });
            public static readonly Application Json_PatchJson = new("json-patch+json", new string[] { "json" });
            public static readonly Application Json_Seq = new("json-seq");
            public static readonly Application JsonmlJson = new("jsonml+json", new string[] { "jsonml" });
            public static readonly Application Jwk_SetJson = new("jwk-set+json", new string[] { "json" });
            public static readonly Application JwkJson = new("jwk+json", new string[] { "json" });
            public static readonly Application Jwt = new("jwt");
            public static readonly Application Kpml_RequestXml = new("kpml-request+xml", new string[] { "xml" });
            public static readonly Application Kpml_ResponseXml = new("kpml-response+xml", new string[] { "xml" });
            public static readonly Application LdJson = new("ld+json", new string[] { "json" });
            public static readonly Application LgrXml = new("lgr+xml", new string[] { "xml" });
            public static readonly Application Link_Format = new("link-format");
            public static readonly Application Load_ControlXml = new("load-control+xml", new string[] { "xml" });
            public static readonly Application LostsyncXml = new("lostsync+xml", new string[] { "xml" });
            public static readonly Application LostXml = new("lost+xml", new string[] { "lostxml" });
            public static readonly Application LXF = new("lxf");
            public static readonly Application Mac_Binhex40 = new("mac-binhex40", new string[] { "hqx" });
            public static readonly Application Mac_Compactpro = new("mac-compactpro", new string[] { "cpt" });
            public static readonly Application Macwriteii = new("macwriteii");
            public static readonly Application MadsXml = new("mads+xml", new string[] { "mads" });
            public static readonly Application Marc = new("marc", new string[] { "mrc" });
            public static readonly Application MarcxmlXml = new("marcxml+xml", new string[] { "mrcx" });
            public static readonly Application Mathematica = new("mathematica", new string[] { "ma", "nb", "mb" });
            public static readonly Application Mathml_ContentXml = new("mathml-content+xml", new string[] { "xml" });
            public static readonly Application Mathml_PresentationXml = new("mathml-presentation+xml", new string[] { "xml" });
            public static readonly Application MathmlXml = new("mathml+xml", new string[] { "mathml" });
            public static readonly Application Mbms_Associated_Procedure_DescriptionXml = new("mbms-associated-procedure-description+xml", new string[] { "xml" });
            public static readonly Application Mbms_DeregisterXml = new("mbms-deregister+xml", new string[] { "xml" });
            public static readonly Application Mbms_EnvelopeXml = new("mbms-envelope+xml", new string[] { "xml" });
            public static readonly Application Mbms_Msk_ResponseXml = new("mbms-msk-response+xml", new string[] { "xml" });
            public static readonly Application Mbms_MskXml = new("mbms-msk+xml", new string[] { "xml" });
            public static readonly Application Mbms_Protection_DescriptionXml = new("mbms-protection-description+xml", new string[] { "xml" });
            public static readonly Application Mbms_Reception_ReportXml = new("mbms-reception-report+xml", new string[] { "xml" });
            public static readonly Application Mbms_Register_ResponseXml = new("mbms-register-response+xml", new string[] { "xml" });
            public static readonly Application Mbms_RegisterXml = new("mbms-register+xml", new string[] { "xml" });
            public static readonly Application Mbms_ScheduleXml = new("mbms-schedule+xml", new string[] { "xml" });
            public static readonly Application Mbms_User_Service_DescriptionXml = new("mbms-user-service-description+xml", new string[] { "xml" });
            public static readonly Application Mbox = new("mbox", new string[] { "mbox" });
            public static readonly Application Media_controlXml = new("media_control+xml", new string[] { "xml" });
            public static readonly Application Media_Policy_DatasetXml = new("media-policy-dataset+xml", new string[] { "xml" });
            public static readonly Application MediaservercontrolXml = new("mediaservercontrol+xml", new string[] { "mscml" });
            public static readonly Application Merge_PatchJson = new("merge-patch+json", new string[] { "json" });
            public static readonly Application Metalink4Xml = new("metalink4+xml", new string[] { "meta4" });
            public static readonly Application MetalinkXml = new("metalink+xml", new string[] { "metalink" });
            public static readonly Application MetsXml = new("mets+xml", new string[] { "mets" });
            public static readonly Application MF4 = new("mf4");
            public static readonly Application Mikey = new("mikey");
            public static readonly Application Mipc = new("mipc");
            public static readonly Application Mmt_AeiXml = new("mmt-aei+xml", new string[] { "xml" });
            public static readonly Application Mmt_UsdXml = new("mmt-usd+xml", new string[] { "xml" });
            public static readonly Application ModsXml = new("mods+xml", new string[] { "mods" });
            public static readonly Application Moss_Keys = new("moss-keys");
            public static readonly Application Moss_Signature = new("moss-signature");
            public static readonly Application Mosskey_Data = new("mosskey-data");
            public static readonly Application Mosskey_Request = new("mosskey-request");
            public static readonly Application Mp21 = new("mp21", new string[] { "m21", "mp21" });
            public static readonly Application Mp4 = new("mp4", new string[] { "mp4s" });
            public static readonly Application Mpeg4_Generic = new("mpeg4-generic");
            public static readonly Application Mpeg4_Iod = new("mpeg4-iod");
            public static readonly Application Mpeg4_Iod_Xmt = new("mpeg4-iod-xmt");
            public static readonly Application Mrb_ConsumerXml = new("mrb-consumer+xml", new string[] { "xml" });
            public static readonly Application Mrb_PublishXml = new("mrb-publish+xml", new string[] { "xml" });
            public static readonly Application Msc_IvrXml = new("msc-ivr+xml", new string[] { "xml" });
            public static readonly Application Msc_MixerXml = new("msc-mixer+xml", new string[] { "xml" });
            public static readonly Application Msword = new("msword", new string[] { "doc", "dot" });
            public static readonly Application MudJson = new("mud+json", new string[] { "json" });
            public static readonly Application Multipart_Core = new("multipart-core");
            public static readonly Application Mxf = new("mxf", new string[] { "mxf" });
            public static readonly Application N_Quads = new("n-quads");
            public static readonly Application N_Triples = new("n-triples");
            public static readonly Application Nasdata = new("nasdata");
            public static readonly Application News_Checkgroups = new("news-checkgroups");
            public static readonly Application News_Groupinfo = new("news-groupinfo");
            public static readonly Application News_Transmission = new("news-transmission");
            public static readonly Application NlsmlXml = new("nlsml+xml", new string[] { "xml" });
            public static readonly Application Node = new("node");
            public static readonly Application Nss = new("nss");
            public static readonly Application Ocsp_Request = new("ocsp-request");
            public static readonly Application Ocsp_Response = new("ocsp-response");
            public static readonly Application Octet_Stream = new("octet-stream", new string[] { "bin", "dms", "lrf", "mar", "so", "dist", "distz", "pkg", "bpk", "dump", "elc", "deploy" });
            public static readonly Application ODA = new("oda", new string[] { "oda" });
            public static readonly Application OdmXml = new("odm+xml", new string[] { "xml" });
            public static readonly Application ODX = new("odx");
            public static readonly Application Oebps_PackageXml = new("oebps-package+xml", new string[] { "opf" });
            public static readonly Application Ogg = new("ogg", new string[] { "ogx" });
            public static readonly Application OmdocXml = new("omdoc+xml", new string[] { "omdoc" });
            public static readonly Application Onenote = new("onenote", new string[] { "onetoc", "onetoc2", "onetmp", "onepkg" });
            public static readonly Application Oscore = new("oscore");
            public static readonly Application Oxps = new("oxps", new string[] { "oxps" });
            public static readonly Application P2p_OverlayXml = new("p2p-overlay+xml", new string[] { "xml" });
            public static readonly Application Parityfec = new("parityfec");
            public static readonly Application Passport = new("passport");
            public static readonly Application Patch_Ops_ErrorXml = new("patch-ops-error+xml", new string[] { "xer" });
            public static readonly Application Pdf = new("pdf", new string[] { "pdf" });
            public static readonly Application PDX = new("pdx");
            public static readonly Application Pem_Certificate_Chain = new("pem-certificate-chain");
            public static readonly Application Pgp_Encrypted = new("pgp-encrypted", new string[] { "pgp" });
            public static readonly Application Pgp_Keys = new("pgp-keys");
            public static readonly Application Pgp_Signature = new("pgp-signature", new string[] { "asc", "sig" });
            public static readonly Application Pics_Rules = new("pics-rules", new string[] { "prf" });
            public static readonly Application Pidf_DiffXml = new("pidf-diff+xml", new string[] { "xml" });
            public static readonly Application PidfXml = new("pidf+xml", new string[] { "xml" });
            public static readonly Application Pkcs10 = new("pkcs10", new string[] { "p10" });
            public static readonly Application Pkcs12 = new("pkcs12");
            public static readonly Application Pkcs7_Mime = new("pkcs7-mime", new string[] { "p7m", "p7c" });
            public static readonly Application Pkcs7_Signature = new("pkcs7-signature", new string[] { "p7s" });
            public static readonly Application Pkcs8 = new("pkcs8", new string[] { "p8" });
            public static readonly Application Pkcs8_Encrypted = new("pkcs8-encrypted");
            public static readonly Application Pkix_Attr_Cert = new("pkix-attr-cert", new string[] { "ac" });
            public static readonly Application Pkix_Cert = new("pkix-cert", new string[] { "cer" });
            public static readonly Application Pkix_Crl = new("pkix-crl", new string[] { "crl" });
            public static readonly Application Pkix_Pkipath = new("pkix-pkipath", new string[] { "pkipath" });
            public static readonly Application Pkixcmp = new("pkixcmp", new string[] { "pki" });
            public static readonly Application PlsXml = new("pls+xml", new string[] { "pls" });
            public static readonly Application Poc_SettingsXml = new("poc-settings+xml", new string[] { "xml" });
            public static readonly Application Postscript = new("postscript", new string[] { "ai", "eps", "ps" });
            public static readonly Application Ppsp_TrackerJson = new("ppsp-tracker+json", new string[] { "json" });
            public static readonly Application ProblemJson = new("problem+json", new string[] { "json" });
            public static readonly Application ProblemXml = new("problem+xml", new string[] { "xml" });
            public static readonly Application ProvenanceXml = new("provenance+xml", new string[] { "xml" });
            public static readonly Application PrsAlvestrandTitrax_Sheet = new("prs.alvestrand.titrax-sheet");
            public static readonly Application PrsCww = new("prs.cww", new string[] { "cww" });
            public static readonly Application PrsHpubZip = new("prs.hpub+zip", new string[] { "zip" });
            public static readonly Application PrsNprend = new("prs.nprend");
            public static readonly Application PrsPlucker = new("prs.plucker");
            public static readonly Application PrsRdf_Xml_Crypt = new("prs.rdf-xml-crypt");
            public static readonly Application PrsXsfXml = new("prs.xsf+xml", new string[] { "xml" });
            public static readonly Application PskcXml = new("pskc+xml", new string[] { "pskcxml" });
            public static readonly Application QSIG = new("qsig");
            public static readonly Application Raptorfec = new("raptorfec");
            public static readonly Application RdapJson = new("rdap+json", new string[] { "json" });
            public static readonly Application RdfXml = new("rdf+xml", new string[] { "rdf" });
            public static readonly Application ReginfoXml = new("reginfo+xml", new string[] { "rif" });
            public static readonly Application Relax_Ng_Compact_Syntax = new("relax-ng-compact-syntax", new string[] { "rnc" });
            public static readonly Application Remote_Printing = new("remote-printing");
            public static readonly Application ReputonJson = new("reputon+json", new string[] { "json" });
            public static readonly Application Resource_Lists_DiffXml = new("resource-lists-diff+xml", new string[] { "rld" });
            public static readonly Application Resource_ListsXml = new("resource-lists+xml", new string[] { "rl" });
            public static readonly Application RfcXml = new("rfc+xml", new string[] { "xml" });
            public static readonly Application Riscos = new("riscos");
            public static readonly Application RlmiXml = new("rlmi+xml", new string[] { "xml" });
            public static readonly Application Rls_ServicesXml = new("rls-services+xml", new string[] { "rs" });
            public static readonly Application Route_ApdXml = new("route-apd+xml", new string[] { "xml" });
            public static readonly Application Route_S_TsidXml = new("route-s-tsid+xml", new string[] { "xml" });
            public static readonly Application Route_UsdXml = new("route-usd+xml", new string[] { "xml" });
            public static readonly Application Rpki_Ghostbusters = new("rpki-ghostbusters", new string[] { "gbr" });
            public static readonly Application Rpki_Manifest = new("rpki-manifest", new string[] { "mft" });
            public static readonly Application Rpki_Publication = new("rpki-publication");
            public static readonly Application Rpki_Roa = new("rpki-roa", new string[] { "roa" });
            public static readonly Application Rpki_Updown = new("rpki-updown");
            public static readonly Application RsdXml = new("rsd+xml", new string[] { "rsd" });
            public static readonly Application RssXml = new("rss+xml", new string[] { "rss" });
            public static readonly Application Rtf = new("rtf", new string[] { "rtf" });
            public static readonly Application Rtploopback = new("rtploopback");
            public static readonly Application Rtx = new("rtx");
            public static readonly Application SamlassertionXml = new("samlassertion+xml", new string[] { "xml" });
            public static readonly Application SamlmetadataXml = new("samlmetadata+xml", new string[] { "xml" });
            public static readonly Application SbmlXml = new("sbml+xml", new string[] { "sbml" });
            public static readonly Application ScaipXml = new("scaip+xml", new string[] { "xml" });
            public static readonly Application ScimJson = new("scim+json", new string[] { "json" });
            public static readonly Application Scvp_Cv_Request = new("scvp-cv-request", new string[] { "scq" });
            public static readonly Application Scvp_Cv_Response = new("scvp-cv-response", new string[] { "scs" });
            public static readonly Application Scvp_Vp_Request = new("scvp-vp-request", new string[] { "spq" });
            public static readonly Application Scvp_Vp_Response = new("scvp-vp-response", new string[] { "spp" });
            public static readonly Application Sdp = new("sdp", new string[] { "sdp" });
            public static readonly Application SeceventJwt = new("secevent+jwt", new string[] { "jwt" });
            public static readonly Application Senml_Exi = new("senml-exi");
            public static readonly Application SenmlCbor = new("senml+cbor", new string[] { "cbor" });
            public static readonly Application SenmlJson = new("senml+json", new string[] { "json" });
            public static readonly Application SenmlXml = new("senml+xml", new string[] { "xml" });
            public static readonly Application Sensml_Exi = new("sensml-exi");
            public static readonly Application SensmlCbor = new("sensml+cbor", new string[] { "cbor" });
            public static readonly Application SensmlJson = new("sensml+json", new string[] { "json" });
            public static readonly Application SensmlXml = new("sensml+xml", new string[] { "xml" });
            public static readonly Application Sep_Exi = new("sep-exi");
            public static readonly Application SepXml = new("sep+xml", new string[] { "xml" });
            public static readonly Application Session_Info = new("session-info");
            public static readonly Application Set_Payment = new("set-payment");
            public static readonly Application Set_Payment_Initiation = new("set-payment-initiation", new string[] { "setpay" });
            public static readonly Application Set_Registration = new("set-registration");
            public static readonly Application Set_Registration_Initiation = new("set-registration-initiation", new string[] { "setreg" });
            public static readonly Application SGML = new("sgml");
            public static readonly Application Sgml_Open_Catalog = new("sgml-open-catalog");
            public static readonly Application ShfXml = new("shf+xml", new string[] { "shf" });
            public static readonly Application Sieve = new("sieve");
            public static readonly Application Simple_FilterXml = new("simple-filter+xml", new string[] { "xml" });
            public static readonly Application Simple_Message_Summary = new("simple-message-summary");
            public static readonly Application SimpleSymbolContainer = new("simplesymbolcontainer");
            public static readonly Application Sipc = new("sipc");
            public static readonly Application Slate = new("slate");

            [System.Obsolete("OBSOLETED in favor of application/smil+xml")]
            public static readonly Application Smil = new("smil");

            public static readonly Application SmilXml = new("smil+xml", new string[] { "smi", "smil" });
            public static readonly Application Smpte336m = new("smpte336m");
            public static readonly Application SoapFastinfoset = new("soap+fastinfoset", new string[] { "fastinfoset" });
            public static readonly Application SoapXml = new("soap+xml", new string[] { "xml" });
            public static readonly Application Sparql_Query = new("sparql-query", new string[] { "rq" });
            public static readonly Application Sparql_ResultsXml = new("sparql-results+xml", new string[] { "srx" });
            public static readonly Application Spirits_EventXml = new("spirits-event+xml", new string[] { "xml" });
            public static readonly Application Sql = new("sql");
            public static readonly Application Srgs = new("srgs", new string[] { "gram" });
            public static readonly Application SrgsXml = new("srgs+xml", new string[] { "grxml" });
            public static readonly Application SruXml = new("sru+xml", new string[] { "sru" });
            public static readonly Application SsdlXml = new("ssdl+xml", new string[] { "ssdl" });
            public static readonly Application SsmlXml = new("ssml+xml", new string[] { "ssml" });
            public static readonly Application StixJson = new("stix+json", new string[] { "json" });
            public static readonly Application SwidXml = new("swid+xml", new string[] { "xml" });
            public static readonly Application Tamp_Apex_Update = new("tamp-apex-update");
            public static readonly Application Tamp_Apex_Update_Confirm = new("tamp-apex-update-confirm");
            public static readonly Application Tamp_Community_Update = new("tamp-community-update");
            public static readonly Application Tamp_Community_Update_Confirm = new("tamp-community-update-confirm");
            public static readonly Application Tamp_Error = new("tamp-error");
            public static readonly Application Tamp_Sequence_Adjust = new("tamp-sequence-adjust");
            public static readonly Application Tamp_Sequence_Adjust_Confirm = new("tamp-sequence-adjust-confirm");
            public static readonly Application Tamp_Status_Query = new("tamp-status-query");
            public static readonly Application Tamp_Status_Response = new("tamp-status-response");
            public static readonly Application Tamp_Update = new("tamp-update");
            public static readonly Application Tamp_Update_Confirm = new("tamp-update-confirm");
            public static readonly Application TaxiiJson = new("taxii+json", new string[] { "json" });
            public static readonly Application TeiXml = new("tei+xml", new string[] { "tei", "teicorpus" });
            public static readonly Application TETRA_ISI = new("tetra_isi");
            public static readonly Application ThraudXml = new("thraud+xml", new string[] { "tfi" });
            public static readonly Application Timestamp_Query = new("timestamp-query");
            public static readonly Application Timestamp_Reply = new("timestamp-reply");
            public static readonly Application Timestamped_Data = new("timestamped-data", new string[] { "tsd" });
            public static readonly Application TlsrptGzip = new("tlsrpt+gzip", new string[] { "gzip" });
            public static readonly Application TlsrptJson = new("tlsrpt+json", new string[] { "json" });
            public static readonly Application Tnauthlist = new("tnauthlist");
            public static readonly Application Trickle_Ice_Sdpfrag = new("trickle-ice-sdpfrag");
            public static readonly Application Trig = new("trig");
            public static readonly Application TtmlXml = new("ttml+xml", new string[] { "xml" });
            public static readonly Application Tve_Trigger = new("tve-trigger");
            public static readonly Application Tzif = new("tzif");
            public static readonly Application Tzif_Leap = new("tzif-leap");
            public static readonly Application Ulpfec = new("ulpfec");
            public static readonly Application Urc_GrpsheetXml = new("urc-grpsheet+xml", new string[] { "xml" });
            public static readonly Application Urc_RessheetXml = new("urc-ressheet+xml", new string[] { "xml" });
            public static readonly Application Urc_TargetdescXml = new("urc-targetdesc+xml", new string[] { "xml" });
            public static readonly Application Urc_UisocketdescXml = new("urc-uisocketdesc+xml", new string[] { "xml" });
            public static readonly Application VcardJson = new("vcard+json", new string[] { "json" });
            public static readonly Application VcardXml = new("vcard+xml", new string[] { "xml" });
            public static readonly Application Vemmi = new("vemmi");
            public static readonly Application Vendor1000mindsDecision_ModelXml = new("vnd.1000minds.decision-model+xml", new string[] { "xml" });
            public static readonly Application Vendor1d_Interleaved_Parityfec = new("1d-interleaved-parityfec");
            public static readonly Application Vendor3gpdash_Qoe_ReportXml = new("3gpdash-qoe-report+xml", new string[] { "xml" });
            public static readonly Application Vendor3gpp_ImsXml = new("3gpp-ims+xml", new string[] { "xml" });
            public static readonly Application Vendor3gpp_Prose_Pc3chXml = new("vnd.3gpp-prose-pc3ch+xml", new string[] { "xml" });
            public static readonly Application Vendor3gpp_ProseXml = new("vnd.3gpp-prose+xml", new string[] { "xml" });
            public static readonly Application Vendor3gpp_V2x_Local_Service_Information = new("vnd.3gpp-v2x-local-service-information");
            public static readonly Application Vendor3gpp2BcmcsinfoXml = new("vnd.3gpp2.bcmcsinfo+xml", new string[] { "xml" });
            public static readonly Application Vendor3gpp2Sms = new("vnd.3gpp2.sms");
            public static readonly Application Vendor3gpp2Tcap = new("vnd.3gpp2.tcap", new string[] { "tcap" });
            public static readonly Application Vendor3gppAccess_Transfer_EventsXml = new("vnd.3gpp.access-transfer-events+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppBsfXml = new("vnd.3gpp.bsf+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppGMOPXml = new("vnd.3gpp.gmop+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMc_Signalling_Ear = new("vnd.3gpp.mc-signalling-ear");
            public static readonly Application Vendor3gppMcdata_Affiliation_CommandXml = new("vnd.3gpp.mcdata-affiliation-command+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcdata_InfoXml = new("vnd.3gpp.mcdata-info+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcdata_Payload = new("vnd.3gpp.mcdata-payload");
            public static readonly Application Vendor3gppMcdata_Service_ConfigXml = new("vnd.3gpp.mcdata-service-config+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcdata_Signalling = new("vnd.3gpp.mcdata-signalling");
            public static readonly Application Vendor3gppMcdata_Ue_ConfigXml = new("vnd.3gpp.mcdata-ue-config+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcdata_User_ProfileXml = new("vnd.3gpp.mcdata-user-profile+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcptt_Affiliation_CommandXml = new("vnd.3gpp.mcptt-affiliation-command+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcptt_Floor_RequestXml = new("vnd.3gpp.mcptt-floor-request+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcptt_InfoXml = new("vnd.3gpp.mcptt-info+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcptt_Location_InfoXml = new("vnd.3gpp.mcptt-location-info+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcptt_Mbms_Usage_InfoXml = new("vnd.3gpp.mcptt-mbms-usage-info+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcptt_Service_ConfigXml = new("vnd.3gpp.mcptt-service-config+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcptt_SignedXml = new("vnd.3gpp.mcptt-signed+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcptt_Ue_ConfigXml = new("vnd.3gpp.mcptt-ue-config+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcptt_Ue_Init_ConfigXml = new("vnd.3gpp.mcptt-ue-init-config+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcptt_User_ProfileXml = new("vnd.3gpp.mcptt-user-profile+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcvideo_Affiliation_CommandXml = new("vnd.3gpp.mcvideo-affiliation-command+xml", new string[] { "xml" });

            [System.Obsolete("OBSOLETED in favor of application/vnd.3gpp.mcvideo-info+xml")]
            public static readonly Application Vendor3gppMcvideo_Affiliation_InfoXml = new("vnd.3gpp.mcvideo-affiliation-info+xml", new string[] { "xml" });

            public static readonly Application Vendor3gppMcvideo_InfoXml = new("vnd.3gpp.mcvideo-info+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcvideo_Location_InfoXml = new("vnd.3gpp.mcvideo-location-info+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcvideo_Mbms_Usage_InfoXml = new("vnd.3gpp.mcvideo-mbms-usage-info+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcvideo_Service_ConfigXml = new("vnd.3gpp.mcvideo-service-config+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcvideo_Transmission_RequestXml = new("vnd.3gpp.mcvideo-transmission-request+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcvideo_Ue_ConfigXml = new("vnd.3gpp.mcvideo-ue-config+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcvideo_User_ProfileXml = new("vnd.3gpp.mcvideo-user-profile+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMid_CallXml = new("vnd.3gpp.mid-call+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppPic_Bw_Large = new("vnd.3gpp.pic-bw-large", new string[] { "plb" });
            public static readonly Application Vendor3gppPic_Bw_Small = new("vnd.3gpp.pic-bw-small", new string[] { "psb" });
            public static readonly Application Vendor3gppPic_Bw_Var = new("vnd.3gpp.pic-bw-var", new string[] { "pvb" });
            public static readonly Application Vendor3gppSms = new("vnd.3gpp.sms");
            public static readonly Application Vendor3gppSmsXml = new("vnd.3gpp.sms+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppSrvcc_ExtXml = new("vnd.3gpp.srvcc-ext+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppSRVCC_InfoXml = new("vnd.3gpp.srvcc-info+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppState_And_Event_InfoXml = new("vnd.3gpp.state-and-event-info+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppUssdXml = new("vnd.3gpp.ussd+xml", new string[] { "xml" });
            public static readonly Application Vendor3lightssoftwareImagescal = new("vnd.3lightssoftware.imagescal");
            public static readonly Application Vendor3MPost_It_Notes = new("vnd.3m.post-it-notes", new string[] { "pwn" });
            public static readonly Application VendorAccpacSimplyAso = new("vnd.accpac.simply.aso", new string[] { "aso" });
            public static readonly Application VendorAccpacSimplyImp = new("vnd.accpac.simply.imp", new string[] { "imp" });
            public static readonly Application VendorAcucobol = new("vnd.acucobol", new string[] { "acu" });
            public static readonly Application VendorAcucorp = new("vnd.acucorp", new string[] { "atc", "acutc" });
            public static readonly Application VendorAdobeAir_Application_Installer_PackageZip = new("vnd.adobe.air-application-installer-package+zip", new string[] { "air" });
            public static readonly Application VendorAdobeFlashMovie = new("vnd.adobe.flash.movie");
            public static readonly Application VendorAdobeFormscentralFcdt = new("vnd.adobe.formscentral.fcdt", new string[] { "fcdt" });
            public static readonly Application VendorAdobeFxp = new("vnd.adobe.fxp", new string[] { "fxp", "fxpl" });
            public static readonly Application VendorAdobePartial_Upload = new("vnd.adobe.partial-upload");
            public static readonly Application VendorAdobeXdpXml = new("vnd.adobe.xdp+xml", new string[] { "xdp" });
            public static readonly Application VendorAdobeXfdf = new("vnd.adobe.xfdf", new string[] { "xfdf" });
            public static readonly Application VendorAetherImp = new("vnd.aether.imp");
            public static readonly Application VendorAfpcAfplinedata = new("vnd.afpc.afplinedata");
            public static readonly Application VendorAfpcAfplinedata_Pagedef = new("vnd.afpc.afplinedata-pagedef");
            public static readonly Application VendorAfpcFoca_Charset = new("vnd.afpc.foca-charset");
            public static readonly Application VendorAfpcFoca_Codedfont = new("vnd.afpc.foca-codedfont");
            public static readonly Application VendorAfpcFoca_Codepage = new("vnd.afpc.foca-codepage");
            public static readonly Application VendorAfpcModca = new("vnd.afpc.modca");
            public static readonly Application VendorAfpcModca_Formdef = new("vnd.afpc.modca-formdef");
            public static readonly Application VendorAfpcModca_Mediummap = new("vnd.afpc.modca-mediummap");
            public static readonly Application VendorAfpcModca_Objectcontainer = new("vnd.afpc.modca-objectcontainer");
            public static readonly Application VendorAfpcModca_Overlay = new("vnd.afpc.modca-overlay");
            public static readonly Application VendorAfpcModca_Pagesegment = new("vnd.afpc.modca-pagesegment");
            public static readonly Application VendorAh_Barcode = new("vnd.ah-barcode");
            public static readonly Application VendorAheadSpace = new("vnd.ahead.space", new string[] { "ahead" });
            public static readonly Application VendorAirzipFilesecureAzf = new("vnd.airzip.filesecure.azf", new string[] { "azf" });
            public static readonly Application VendorAirzipFilesecureAzs = new("vnd.airzip.filesecure.azs", new string[] { "azs" });
            public static readonly Application VendorAmadeusJson = new("vnd.amadeus+json", new string[] { "json" });
            public static readonly Application VendorAmazonEbook = new("vnd.amazon.ebook", new string[] { "azw" });
            public static readonly Application VendorAmazonMobi8_Ebook = new("vnd.amazon.mobi8-ebook");
            public static readonly Application VendorAmericandynamicsAcc = new("vnd.americandynamics.acc", new string[] { "acc" });
            public static readonly Application VendorAmigaAmi = new("vnd.amiga.ami", new string[] { "ami" });
            public static readonly Application VendorAmundsenMazeXml = new("vnd.amundsen.maze+xml", new string[] { "xml" });
            public static readonly Application VendorAndroidOta = new("vnd.android.ota");
            public static readonly Application VendorAndroidPackage_Archive = new("vnd.android.package-archive", new string[] { "apk" });
            public static readonly Application VendorAnki = new("vnd.anki");
            public static readonly Application VendorAnser_Web_Certificate_Issue_Initiation = new("vnd.anser-web-certificate-issue-initiation", new string[] { "cii" });
            public static readonly Application VendorAnser_Web_Funds_Transfer_Initiation = new("vnd.anser-web-funds-transfer-initiation", new string[] { "fti" });
            public static readonly Application VendorAntixGame_Component = new("vnd.antix.game-component", new string[] { "atx" });
            public static readonly Application VendorApacheThriftBinary = new("vnd.apache.thrift.binary");
            public static readonly Application VendorApacheThriftCompact = new("vnd.apache.thrift.compact");
            public static readonly Application VendorApacheThriftJson = new("vnd.apache.thrift.json");
            public static readonly Application VendorApiJson = new("vnd.api+json", new string[] { "json" });
            public static readonly Application VendorAplextorWarrpJson = new("vnd.aplextor.warrp+json", new string[] { "json" });
            public static readonly Application VendorApothekendeReservationJson = new("vnd.apothekende.reservation+json", new string[] { "json" });
            public static readonly Application VendorAppleInstallerXml = new("vnd.apple.installer+xml", new string[] { "mpkg" });
            public static readonly Application VendorAppleKeynote = new("vnd.apple.keynote");
            public static readonly Application VendorAppleMpegurl = new("vnd.apple.mpegurl", new string[] { "m3u8" });
            public static readonly Application VendorAppleNumbers = new("vnd.apple.numbers");
            public static readonly Application VendorApplePages = new("vnd.apple.pages");

            [System.Obsolete("OBSOLETED in favor of application/vnd.aristanetworks.swi")]
            public static readonly Application VendorArastraSwi = new("vnd.arastra.swi");

            public static readonly Application VendorAristanetworksSwi = new("vnd.aristanetworks.swi", new string[] { "swi" });
            public static readonly Application VendorArtisanJson = new("vnd.artisan+json", new string[] { "json" });
            public static readonly Application VendorArtsquare = new("vnd.artsquare");
            public static readonly Application VendorAstraea_SoftwareIota = new("vnd.astraea-software.iota", new string[] { "iota" });
            public static readonly Application VendorAudiograph = new("vnd.audiograph", new string[] { "aep" });
            public static readonly Application VendorAutopackage = new("vnd.autopackage");
            public static readonly Application VendorAvalonJson = new("vnd.avalon+json", new string[] { "json" });
            public static readonly Application VendorAvistarXml = new("vnd.avistar+xml", new string[] { "xml" });
            public static readonly Application VendorBalsamiqBmmlXml = new("vnd.balsamiq.bmml+xml", new string[] { "xml" });
            public static readonly Application VendorBalsamiqBmpr = new("vnd.balsamiq.bmpr");
            public static readonly Application VendorBanana_Accounting = new("vnd.banana-accounting");
            public static readonly Application VendorBbfUspError = new("vnd.bbf.usp.error");
            public static readonly Application VendorBbfUspMsg = new("vnd.bbf.usp.msg");
            public static readonly Application VendorBbfUspMsgJson = new("vnd.bbf.usp.msg+json", new string[] { "json" });
            public static readonly Application VendorBekitzur_StechJson = new("vnd.bekitzur-stech+json", new string[] { "json" });
            public static readonly Application VendorBintMed_Content = new("vnd.bint.med-content");
            public static readonly Application VendorBiopaxRdfXml = new("vnd.biopax.rdf+xml", new string[] { "xml" });
            public static readonly Application VendorBlink_Idb_Value_Wrapper = new("vnd.blink-idb-value-wrapper");
            public static readonly Application VendorBlueiceMultipass = new("vnd.blueice.multipass", new string[] { "mpm" });
            public static readonly Application VendorBluetoothEpOob = new("vnd.bluetooth.ep.oob");
            public static readonly Application VendorBluetoothLeOob = new("vnd.bluetooth.le.oob");
            public static readonly Application VendorBmi = new("vnd.bmi", new string[] { "bmi" });
            public static readonly Application VendorBpf = new("vnd.bpf");
            public static readonly Application VendorBpf3 = new("vnd.bpf3");
            public static readonly Application VendorBusinessobjects = new("vnd.businessobjects", new string[] { "rep" });
            public static readonly Application VendorByuUapiJson = new("vnd.byu.uapi+json", new string[] { "json" });
            public static readonly Application VendorCab_Jscript = new("vnd.cab-jscript");
            public static readonly Application VendorCanon_Cpdl = new("vnd.canon-cpdl");
            public static readonly Application VendorCanon_Lips = new("vnd.canon-lips");
            public static readonly Application VendorCapasystems_PgJson = new("vnd.capasystems-pg+json", new string[] { "json" });
            public static readonly Application VendorCendioThinlincClientconf = new("vnd.cendio.thinlinc.clientconf");
            public static readonly Application VendorCentury_SystemsTcp_stream = new("vnd.century-systems.tcp_stream");
            public static readonly Application VendorChemdrawXml = new("vnd.chemdraw+xml", new string[] { "cdxml" });
            public static readonly Application VendorChess_Pgn = new("vnd.chess-pgn");
            public static readonly Application VendorChipnutsKaraoke_Mmd = new("vnd.chipnuts.karaoke-mmd", new string[] { "mmd" });
            public static readonly Application VendorCiedi = new("vnd.ciedi");
            public static readonly Application VendorCinderella = new("vnd.cinderella", new string[] { "cdy" });
            public static readonly Application VendorCirpackIsdn_Ext = new("vnd.cirpack.isdn-ext");
            public static readonly Application VendorCitationstylesStyleXml = new("vnd.citationstyles.style+xml", new string[] { "xml" });
            public static readonly Application VendorClaymore = new("vnd.claymore", new string[] { "cla" });
            public static readonly Application VendorCloantoRp9 = new("vnd.cloanto.rp9", new string[] { "rp9" });
            public static readonly Application VendorClonkC4group = new("vnd.clonk.c4group", new string[] { "c4g", "c4d", "c4f", "c4p", "c4u" });
            public static readonly Application VendorCluetrustCartomobile_Config = new("vnd.cluetrust.cartomobile-config", new string[] { "c11amc" });
            public static readonly Application VendorCluetrustCartomobile_Config_Pkg = new("vnd.cluetrust.cartomobile-config-pkg", new string[] { "c11amz" });
            public static readonly Application VendorCoffeescript = new("vnd.coffeescript");
            public static readonly Application VendorCollabioXodocumentsDocument = new("vnd.collabio.xodocuments.document");
            public static readonly Application VendorCollabioXodocumentsDocument_Template = new("vnd.collabio.xodocuments.document-template");
            public static readonly Application VendorCollabioXodocumentsPresentation = new("vnd.collabio.xodocuments.presentation");
            public static readonly Application VendorCollabioXodocumentsPresentation_Template = new("vnd.collabio.xodocuments.presentation-template");
            public static readonly Application VendorCollabioXodocumentsSpreadsheet = new("vnd.collabio.xodocuments.spreadsheet");
            public static readonly Application VendorCollabioXodocumentsSpreadsheet_Template = new("vnd.collabio.xodocuments.spreadsheet-template");
            public static readonly Application VendorCollectionDocJson = new("vnd.collection.doc+json", new string[] { "json" });
            public static readonly Application VendorCollectionJson = new("vnd.collection+json", new string[] { "json" });
            public static readonly Application VendorCollectionNextJson = new("vnd.collection.next+json", new string[] { "json" });
            public static readonly Application VendorComicbook_Rar = new("vnd.comicbook-rar");
            public static readonly Application VendorComicbookZip = new("vnd.comicbook+zip", new string[] { "zip" });
            public static readonly Application VendorCommerce_Battelle = new("vnd.commerce-battelle");
            public static readonly Application VendorCommonspace = new("vnd.commonspace", new string[] { "csp" });
            public static readonly Application VendorContactCmsg = new("vnd.contact.cmsg", new string[] { "cdbcmsg" });
            public static readonly Application VendorCoreosIgnitionJson = new("vnd.coreos.ignition+json", new string[] { "json" });
            public static readonly Application VendorCosmocaller = new("vnd.cosmocaller", new string[] { "cmc" });
            public static readonly Application VendorCrickClicker = new("vnd.crick.clicker", new string[] { "clkx" });
            public static readonly Application VendorCrickClickerKeyboard = new("vnd.crick.clicker.keyboard", new string[] { "clkk" });
            public static readonly Application VendorCrickClickerPalette = new("vnd.crick.clicker.palette", new string[] { "clkp" });
            public static readonly Application VendorCrickClickerTemplate = new("vnd.crick.clicker.template", new string[] { "clkt" });
            public static readonly Application VendorCrickClickerWordbank = new("vnd.crick.clicker.wordbank", new string[] { "clkw" });
            public static readonly Application VendorCriticaltoolsWbsXml = new("vnd.criticaltools.wbs+xml", new string[] { "wbs" });
            public static readonly Application VendorCryptiiPipeJson = new("vnd.cryptii.pipe+json", new string[] { "json" });
            public static readonly Application VendorCrypto_Shade_File = new("vnd.crypto-shade-file");
            public static readonly Application VendorCtc_Posml = new("vnd.ctc-posml", new string[] { "pml" });
            public static readonly Application VendorCtctWsXml = new("vnd.ctct.ws+xml", new string[] { "xml" });
            public static readonly Application VendorCups_Pdf = new("vnd.cups-pdf");
            public static readonly Application VendorCups_Postscript = new("vnd.cups-postscript");
            public static readonly Application VendorCups_Ppd = new("vnd.cups-ppd", new string[] { "ppd" });
            public static readonly Application VendorCups_Raster = new("vnd.cups-raster");
            public static readonly Application VendorCups_Raw = new("vnd.cups-raw");
            public static readonly Application VendorCurl = new("vnd.curl");
            public static readonly Application VendorCurlCar = new("vnd.curl.car", new string[] { "car" });
            public static readonly Application VendorCurlPcurl = new("vnd.curl.pcurl", new string[] { "pcurl" });
            public static readonly Application VendorCyanDeanRootXml = new("vnd.cyan.dean.root+xml", new string[] { "xml" });
            public static readonly Application VendorCybank = new("vnd.cybank");
            public static readonly Application VendorD2lCoursepackage1p0Zip = new("vnd.d2l.coursepackage1p0+zip", new string[] { "zip" });
            public static readonly Application VendorDart = new("vnd.dart", new string[] { "dart" });
            public static readonly Application VendorData_VisionRdz = new("vnd.data-vision.rdz", new string[] { "rdz" });
            public static readonly Application VendorDatapackageJson = new("vnd.datapackage+json", new string[] { "json" });
            public static readonly Application VendorDataresourceJson = new("vnd.dataresource+json", new string[] { "json" });
            public static readonly Application VendorDebianBinary_Package = new("vnd.debian.binary-package");
            public static readonly Application VendorDeceData = new("vnd.dece.data", new string[] { "uvf", "uvvf", "uvd", "uvvd" });
            public static readonly Application VendorDeceTtmlXml = new("vnd.dece.ttml+xml", new string[] { "uvt", "uvvt" });
            public static readonly Application VendorDeceUnspecified = new("vnd.dece.unspecified", new string[] { "uvx", "uvvx" });
            public static readonly Application VendorDeceZip = new("vnd.dece.zip", new string[] { "uvz", "uvvz" });
            public static readonly Application VendorDenovoFcselayout_Link = new("vnd.denovo.fcselayout-link", new string[] { "fe_launch" });
            public static readonly Application VendorDesmumeMovie = new("vnd.desmume.movie");
            public static readonly Application VendorDir_BiPlate_Dl_Nosuffix = new("vnd.dir-bi.plate-dl-nosuffix");
            public static readonly Application VendorDmDelegationXml = new("vnd.dm.delegation+xml", new string[] { "xml" });
            public static readonly Application VendorDna = new("vnd.dna", new string[] { "dna" });
            public static readonly Application VendorDocumentJson = new("vnd.document+json", new string[] { "json" });
            public static readonly Application VendorDolbyMlp = new("vnd.dolby.mlp", new string[] { "mlp" });
            public static readonly Application VendorDolbyMobile1 = new("vnd.dolby.mobile.1");
            public static readonly Application VendorDolbyMobile2 = new("vnd.dolby.mobile.2");
            public static readonly Application VendorDoremirScorecloud_Binary_Document = new("vnd.doremir.scorecloud-binary-document");
            public static readonly Application VendorDpgraph = new("vnd.dpgraph", new string[] { "dpg" });
            public static readonly Application VendorDreamfactory = new("vnd.dreamfactory", new string[] { "dfac" });
            public static readonly Application VendorDriveJson = new("vnd.drive+json", new string[] { "json" });
            public static readonly Application VendorDs_Keypoint = new("vnd.ds-keypoint", new string[] { "kpxx" });
            public static readonly Application VendorDtgLocal = new("vnd.dtg.local");
            public static readonly Application VendorDtgLocalFlash = new("vnd.dtg.local.flash");
            public static readonly Application VendorDtgLocalHtml = new("vnd.dtg.local.html");
            public static readonly Application VendorDvbAit = new("vnd.dvb.ait", new string[] { "ait" });
            public static readonly Application VendorDvbDvbj = new("vnd.dvb.dvbj");
            public static readonly Application VendorDvbEsgcontainer = new("vnd.dvb.esgcontainer");
            public static readonly Application VendorDvbIpdcdftnotifaccess = new("vnd.dvb.ipdcdftnotifaccess");
            public static readonly Application VendorDvbIpdcesgaccess = new("vnd.dvb.ipdcesgaccess");
            public static readonly Application VendorDvbIpdcesgaccess2 = new("vnd.dvb.ipdcesgaccess2");
            public static readonly Application VendorDvbIpdcesgpdd = new("vnd.dvb.ipdcesgpdd");
            public static readonly Application VendorDvbIpdcroaming = new("vnd.dvb.ipdcroaming");
            public static readonly Application VendorDvbIptvAlfec_Base = new("vnd.dvb.iptv.alfec-base");
            public static readonly Application VendorDvbIptvAlfec_Enhancement = new("vnd.dvb.iptv.alfec-enhancement");
            public static readonly Application VendorDvbNotif_Aggregate_RootXml = new("vnd.dvb.notif-aggregate-root+xml", new string[] { "xml" });
            public static readonly Application VendorDvbNotif_ContainerXml = new("vnd.dvb.notif-container+xml", new string[] { "xml" });
            public static readonly Application VendorDvbNotif_GenericXml = new("vnd.dvb.notif-generic+xml", new string[] { "xml" });
            public static readonly Application VendorDvbNotif_Ia_MsglistXml = new("vnd.dvb.notif-ia-msglist+xml", new string[] { "xml" });
            public static readonly Application VendorDvbNotif_Ia_Registration_RequestXml = new("vnd.dvb.notif-ia-registration-request+xml", new string[] { "xml" });
            public static readonly Application VendorDvbNotif_Ia_Registration_ResponseXml = new("vnd.dvb.notif-ia-registration-response+xml", new string[] { "xml" });
            public static readonly Application VendorDvbNotif_InitXml = new("vnd.dvb.notif-init+xml", new string[] { "xml" });
            public static readonly Application VendorDvbPfr = new("vnd.dvb.pfr");
            public static readonly Application VendorDvbService = new("vnd.dvb.service", new string[] { "svc" });
            public static readonly Application VendorDxr = new("vnd.dxr");
            public static readonly Application VendorDynageo = new("vnd.dynageo", new string[] { "geo" });
            public static readonly Application VendorDzr = new("vnd.dzr");
            public static readonly Application VendorEasykaraokeCdgdownload = new("vnd.easykaraoke.cdgdownload");
            public static readonly Application VendorEcdis_Update = new("vnd.ecdis-update");
            public static readonly Application VendorEcipRlp = new("vnd.ecip.rlp");
            public static readonly Application VendorEcowinChart = new("vnd.ecowin.chart", new string[] { "mag" });
            public static readonly Application VendorEcowinFilerequest = new("vnd.ecowin.filerequest");
            public static readonly Application VendorEcowinFileupdate = new("vnd.ecowin.fileupdate");
            public static readonly Application VendorEcowinSeries = new("vnd.ecowin.series");
            public static readonly Application VendorEcowinSeriesrequest = new("vnd.ecowin.seriesrequest");
            public static readonly Application VendorEcowinSeriesupdate = new("vnd.ecowin.seriesupdate");
            public static readonly Application VendorEfiImg = new("vnd.efi.img");
            public static readonly Application VendorEfiIso = new("vnd.efi.iso");
            public static readonly Application VendorEmclientAccessrequestXml = new("vnd.emclient.accessrequest+xml", new string[] { "xml" });
            public static readonly Application VendorEnliven = new("vnd.enliven", new string[] { "nml" });
            public static readonly Application VendorEnphaseEnvoy = new("vnd.enphase.envoy");
            public static readonly Application VendorEprintsDataXml = new("vnd.eprints.data+xml", new string[] { "xml" });
            public static readonly Application VendorEpsonEsf = new("vnd.epson.esf", new string[] { "esf" });
            public static readonly Application VendorEpsonMsf = new("vnd.epson.msf", new string[] { "msf" });
            public static readonly Application VendorEpsonQuickanime = new("vnd.epson.quickanime", new string[] { "qam" });
            public static readonly Application VendorEpsonSalt = new("vnd.epson.salt", new string[] { "slt" });
            public static readonly Application VendorEpsonSsf = new("vnd.epson.ssf", new string[] { "ssf" });
            public static readonly Application VendorEricssonQuickcall = new("vnd.ericsson.quickcall");
            public static readonly Application VendorEspass_EspassZip = new("vnd.espass-espass+zip", new string[] { "zip" });
            public static readonly Application VendorEszigno3Xml = new("vnd.eszigno3+xml", new string[] { "es3", "et3" });
            public static readonly Application VendorEtsiAocXml = new("vnd.etsi.aoc+xml", new string[] { "xml" });
            public static readonly Application VendorEtsiAsic_EZip = new("vnd.etsi.asic-e+zip", new string[] { "zip" });
            public static readonly Application VendorEtsiAsic_SZip = new("vnd.etsi.asic-s+zip", new string[] { "zip" });
            public static readonly Application VendorEtsiCugXml = new("vnd.etsi.cug+xml", new string[] { "xml" });
            public static readonly Application VendorEtsiIptvcommandXml = new("vnd.etsi.iptvcommand+xml", new string[] { "xml" });
            public static readonly Application VendorEtsiIptvdiscoveryXml = new("vnd.etsi.iptvdiscovery+xml", new string[] { "xml" });
            public static readonly Application VendorEtsiIptvprofileXml = new("vnd.etsi.iptvprofile+xml", new string[] { "xml" });
            public static readonly Application VendorEtsiIptvsad_BcXml = new("vnd.etsi.iptvsad-bc+xml", new string[] { "xml" });
            public static readonly Application VendorEtsiIptvsad_CodXml = new("vnd.etsi.iptvsad-cod+xml", new string[] { "xml" });
            public static readonly Application VendorEtsiIptvsad_NpvrXml = new("vnd.etsi.iptvsad-npvr+xml", new string[] { "xml" });
            public static readonly Application VendorEtsiIptvserviceXml = new("vnd.etsi.iptvservice+xml", new string[] { "xml" });
            public static readonly Application VendorEtsiIptvsyncXml = new("vnd.etsi.iptvsync+xml", new string[] { "xml" });
            public static readonly Application VendorEtsiIptvueprofileXml = new("vnd.etsi.iptvueprofile+xml", new string[] { "xml" });
            public static readonly Application VendorEtsiMcidXml = new("vnd.etsi.mcid+xml", new string[] { "xml" });
            public static readonly Application VendorEtsiMheg5 = new("vnd.etsi.mheg5");
            public static readonly Application VendorEtsiOverload_Control_Policy_DatasetXml = new("vnd.etsi.overload-control-policy-dataset+xml", new string[] { "xml" });
            public static readonly Application VendorEtsiPstnXml = new("vnd.etsi.pstn+xml", new string[] { "xml" });
            public static readonly Application VendorEtsiSciXml = new("vnd.etsi.sci+xml", new string[] { "xml" });
            public static readonly Application VendorEtsiSimservsXml = new("vnd.etsi.simservs+xml", new string[] { "xml" });
            public static readonly Application VendorEtsiTimestamp_Token = new("vnd.etsi.timestamp-token");
            public static readonly Application VendorEtsiTslDer = new("vnd.etsi.tsl.der");
            public static readonly Application VendorEtsiTslXml = new("vnd.etsi.tsl+xml", new string[] { "xml" });
            public static readonly Application VendorEudoraData = new("vnd.eudora.data");
            public static readonly Application VendorEvolvEcigProfile = new("vnd.evolv.ecig.profile");
            public static readonly Application VendorEvolvEcigSettings = new("vnd.evolv.ecig.settings");
            public static readonly Application VendorEvolvEcigTheme = new("vnd.evolv.ecig.theme");
            public static readonly Application VendorExstream_EmpowerZip = new("vnd.exstream-empower+zip", new string[] { "zip" });
            public static readonly Application VendorExstream_Package = new("vnd.exstream-package");
            public static readonly Application VendorEzpix_Album = new("vnd.ezpix-album", new string[] { "ez2" });
            public static readonly Application VendorEzpix_Package = new("vnd.ezpix-package", new string[] { "ez3" });
            public static readonly Application VendorF_SecureMobile = new("vnd.f-secure.mobile");
            public static readonly Application VendorFastcopy_Disk_Image = new("vnd.fastcopy-disk-image");
            public static readonly Application VendorFdf = new("vnd.fdf", new string[] { "fdf" });
            public static readonly Application VendorFdsnMseed = new("vnd.fdsn.mseed", new string[] { "mseed" });
            public static readonly Application VendorFdsnSeed = new("vnd.fdsn.seed", new string[] { "seed", "dataless" });
            public static readonly Application VendorFfsns = new("vnd.ffsns");
            public static readonly Application VendorFiclabFlbZip = new("vnd.ficlab.flb+zip", new string[] { "zip" });
            public static readonly Application VendorFilmitZfc = new("vnd.filmit.zfc");
            public static readonly Application VendorFints = new("vnd.fints");
            public static readonly Application VendorFiremonkeysCloudcell = new("vnd.firemonkeys.cloudcell");
            public static readonly Application VendorFloGraphIt = new("vnd.flographit", new string[] { "gph" });
            public static readonly Application VendorFluxtimeClip = new("vnd.fluxtime.clip", new string[] { "ftc" });
            public static readonly Application VendorFont_Fontforge_Sfd = new("vnd.font-fontforge-sfd");
            public static readonly Application VendorFramemaker = new("vnd.framemaker", new string[] { "fm", "frame", "maker", "book" });
            public static readonly Application VendorFrogansFnc = new("vnd.frogans.fnc", new string[] { "fnc" });
            public static readonly Application VendorFrogansLtf = new("vnd.frogans.ltf", new string[] { "ltf" });
            public static readonly Application VendorFscWeblaunch = new("vnd.fsc.weblaunch", new string[] { "fsc" });
            public static readonly Application VendorFujitsuOasys = new("vnd.fujitsu.oasys", new string[] { "oas" });
            public static readonly Application VendorFujitsuOasys2 = new("vnd.fujitsu.oasys2", new string[] { "oa2" });
            public static readonly Application VendorFujitsuOasys3 = new("vnd.fujitsu.oasys3", new string[] { "oa3" });
            public static readonly Application VendorFujitsuOasysgp = new("vnd.fujitsu.oasysgp", new string[] { "fg5" });
            public static readonly Application VendorFujitsuOasysprs = new("vnd.fujitsu.oasysprs", new string[] { "bh2" });
            public static readonly Application VendorFujixeroxART_EX = new("vnd.fujixerox.art-ex");
            public static readonly Application VendorFujixeroxART4 = new("vnd.fujixerox.art4");
            public static readonly Application VendorFujixeroxDdd = new("vnd.fujixerox.ddd", new string[] { "ddd" });
            public static readonly Application VendorFujixeroxDocuworks = new("vnd.fujixerox.docuworks", new string[] { "xdw" });
            public static readonly Application VendorFujixeroxDocuworksBinder = new("vnd.fujixerox.docuworks.binder", new string[] { "xbd" });
            public static readonly Application VendorFujixeroxDocuworksContainer = new("vnd.fujixerox.docuworks.container");
            public static readonly Application VendorFujixeroxHBPL = new("vnd.fujixerox.hbpl");
            public static readonly Application VendorFut_Misnet = new("vnd.fut-misnet");
            public static readonly Application VendorFutoinCbor = new("vnd.futoin+cbor", new string[] { "cbor" });
            public static readonly Application VendorFutoinJson = new("vnd.futoin+json", new string[] { "json" });
            public static readonly Application VendorFuzzysheet = new("vnd.fuzzysheet", new string[] { "fzs" });
            public static readonly Application VendorGenomatixTuxedo = new("vnd.genomatix.tuxedo", new string[] { "txd" });
            public static readonly Application VendorGenticsGrdJson = new("vnd.gentics.grd+json", new string[] { "json" });

            [System.Obsolete("OBSOLETED by request")]
            public static readonly Application VendorGeocubeXml = new("vnd.geocube+xml", new string[] { "xml" });

            public static readonly Application VendorGeogebraFile = new("vnd.geogebra.file", new string[] { "ggb" });
            public static readonly Application VendorGeogebraTool = new("vnd.geogebra.tool", new string[] { "ggt" });

            [System.Obsolete("(OBSOLETED by  in favor of application/geo+json)")]
            public static readonly Application VendorGeoJson = new("vnd.geo+json", new string[] { "json" });

            public static readonly Application VendorGeometry_Explorer = new("vnd.geometry-explorer", new string[] { "gex", "gre" });
            public static readonly Application VendorGeonext = new("vnd.geonext", new string[] { "gxt" });
            public static readonly Application VendorGeoplan = new("vnd.geoplan", new string[] { "g2w" });
            public static readonly Application VendorGeospace = new("vnd.geospace", new string[] { "g3w" });
            public static readonly Application VendorGerber = new("vnd.gerber");
            public static readonly Application VendorGlobalplatformCard_Content_Mgt = new("vnd.globalplatform.card-content-mgt");
            public static readonly Application VendorGlobalplatformCard_Content_Mgt_Response = new("vnd.globalplatform.card-content-mgt-response");

            [System.Obsolete("DEPRECATED")]
            public static readonly Application VendorGmx = new("vnd.gmx", new string[] { "gmx" });

            public static readonly Application VendorGoogle_EarthKmlXml = new("vnd.google-earth.kml+xml", new string[] { "kml" });
            public static readonly Application VendorGoogle_EarthKmz = new("vnd.google-earth.kmz", new string[] { "kmz" });
            public static readonly Application VendorGovSkE_FormXml = new("vnd.gov.sk.e-form+xml", new string[] { "xml" });
            public static readonly Application VendorGovSkE_FormZip = new("vnd.gov.sk.e-form+zip", new string[] { "zip" });
            public static readonly Application VendorGovSkXmldatacontainerXml = new("vnd.gov.sk.xmldatacontainer+xml", new string[] { "xml" });
            public static readonly Application VendorGrafeq = new("vnd.grafeq", new string[] { "gqf", "gqs" });
            public static readonly Application VendorGridmp = new("vnd.gridmp");
            public static readonly Application VendorGroove_Account = new("vnd.groove-account", new string[] { "gac" });
            public static readonly Application VendorGroove_Help = new("vnd.groove-help", new string[] { "ghf" });
            public static readonly Application VendorGroove_Identity_Message = new("vnd.groove-identity-message", new string[] { "gim" });
            public static readonly Application VendorGroove_Injector = new("vnd.groove-injector", new string[] { "grv" });
            public static readonly Application VendorGroove_Tool_Message = new("vnd.groove-tool-message", new string[] { "gtm" });
            public static readonly Application VendorGroove_Tool_Template = new("vnd.groove-tool-template", new string[] { "tpl" });
            public static readonly Application VendorGroove_Vcard = new("vnd.groove-vcard", new string[] { "vcg" });
            public static readonly Application VendorHalJson = new("vnd.hal+json", new string[] { "json" });
            public static readonly Application VendorHalXml = new("vnd.hal+xml", new string[] { "hal" });
            public static readonly Application VendorHandHeld_EntertainmentXml = new("vnd.handheld-entertainment+xml", new string[] { "zmm" });
            public static readonly Application VendorHbci = new("vnd.hbci", new string[] { "hbci" });
            public static readonly Application VendorHcJson = new("vnd.hc+json", new string[] { "json" });
            public static readonly Application VendorHcl_Bireports = new("vnd.hcl-bireports");
            public static readonly Application VendorHdt = new("vnd.hdt");
            public static readonly Application VendorHerokuJson = new("vnd.heroku+json", new string[] { "json" });
            public static readonly Application VendorHheLesson_Player = new("vnd.hhe.lesson-player", new string[] { "les" });
            public static readonly Application VendorHp_HPGL = new("vnd.hp-hpgl", new string[] { "hpgl" });
            public static readonly Application VendorHp_Hpid = new("vnd.hp-hpid", new string[] { "hpid" });
            public static readonly Application VendorHp_Hps = new("vnd.hp-hps", new string[] { "hps" });
            public static readonly Application VendorHp_Jlyt = new("vnd.hp-jlyt", new string[] { "jlt" });
            public static readonly Application VendorHp_PCL = new("vnd.hp-pcl", new string[] { "pcl" });
            public static readonly Application VendorHp_PCLXL = new("vnd.hp-pclxl", new string[] { "pclxl" });
            public static readonly Application VendorHttphone = new("vnd.httphone");
            public static readonly Application VendorHydrostatixSof_Data = new("vnd.hydrostatix.sof-data", new string[] { "sfd-hdstx" });
            public static readonly Application VendorHyper_ItemJson = new("vnd.hyper-item+json", new string[] { "json" });
            public static readonly Application VendorHyperdriveJson = new("vnd.hyperdrive+json", new string[] { "json" });
            public static readonly Application VendorHyperJson = new("vnd.hyper+json", new string[] { "json" });
            public static readonly Application VendorHzn_3d_Crossword = new("vnd.hzn-3d-crossword");

            [System.Obsolete("OBSOLETED in favor of vnd.afpc.afplinedata")]
            public static readonly Application VendorIbmAfplinedata = new("vnd.ibm.afplinedata");

            public static readonly Application VendorIbmElectronic_Media = new("vnd.ibm.electronic-media");
            public static readonly Application VendorIbmMiniPay = new("vnd.ibm.minipay", new string[] { "mpy" });

            [System.Obsolete("OBSOLETED in favor of application/vnd.afpc.modca")]
            public static readonly Application VendorIbmModcap = new("vnd.ibm.modcap", new string[] { "afp", "listafp", "list3820" });

            public static readonly Application VendorIbmRights_Management = new("vnd.ibm.rights-management", new string[] { "irm" });
            public static readonly Application VendorIbmSecure_Container = new("vnd.ibm.secure-container", new string[] { "sc" });
            public static readonly Application VendorIccprofile = new("vnd.iccprofile", new string[] { "icc", "icm" });
            public static readonly Application VendorIeee1905 = new("vnd.ieee.1905");
            public static readonly Application VendorIgloader = new("vnd.igloader", new string[] { "igl" });
            public static readonly Application VendorImagemeterFolderZip = new("vnd.imagemeter.folder+zip", new string[] { "zip" });
            public static readonly Application VendorImagemeterImageZip = new("vnd.imagemeter.image+zip", new string[] { "zip" });
            public static readonly Application VendorImmervision_Ivp = new("vnd.immervision-ivp", new string[] { "ivp" });
            public static readonly Application VendorImmervision_Ivu = new("vnd.immervision-ivu", new string[] { "ivu" });
            public static readonly Application VendorImsImsccv1p1 = new("vnd.ims.imsccv1p1");
            public static readonly Application VendorImsImsccv1p2 = new("vnd.ims.imsccv1p2");
            public static readonly Application VendorImsImsccv1p3 = new("vnd.ims.imsccv1p3");
            public static readonly Application VendorImsLisV2ResultJson = new("vnd.ims.lis.v2.result+json", new string[] { "json" });
            public static readonly Application VendorImsLtiV2ToolconsumerprofileJson = new("vnd.ims.lti.v2.toolconsumerprofile+json", new string[] { "json" });
            public static readonly Application VendorImsLtiV2ToolproxyIdJson = new("vnd.ims.lti.v2.toolproxy.id+json", new string[] { "json" });
            public static readonly Application VendorImsLtiV2ToolproxyJson = new("vnd.ims.lti.v2.toolproxy+json", new string[] { "json" });
            public static readonly Application VendorImsLtiV2ToolsettingsJson = new("vnd.ims.lti.v2.toolsettings+json", new string[] { "json" });
            public static readonly Application VendorImsLtiV2ToolsettingsSimpleJson = new("vnd.ims.lti.v2.toolsettings.simple+json", new string[] { "json" });
            public static readonly Application VendorInformedcontrolRmsXml = new("vnd.informedcontrol.rms+xml", new string[] { "xml" });

            [System.Obsolete("OBSOLETED in favor of application/vnd.visionary")]
            public static readonly Application VendorInformix_Visionary = new("vnd.informix-visionary");

            public static readonly Application VendorInfotechProject = new("vnd.infotech.project");
            public static readonly Application VendorInfotechProjectXml = new("vnd.infotech.project+xml", new string[] { "xml" });
            public static readonly Application VendorInnopathWampNotification = new("vnd.innopath.wamp.notification");
            public static readonly Application VendorInsorsIgm = new("vnd.insors.igm", new string[] { "igm" });
            public static readonly Application VendorInterconFormnet = new("vnd.intercon.formnet", new string[] { "xpw", "xpx" });
            public static readonly Application VendorIntergeo = new("vnd.intergeo", new string[] { "i2g" });
            public static readonly Application VendorIntertrustDigibox = new("vnd.intertrust.digibox");
            public static readonly Application VendorIntertrustNncp = new("vnd.intertrust.nncp");
            public static readonly Application VendorIntuQbo = new("vnd.intu.qbo", new string[] { "qbo" });
            public static readonly Application VendorIntuQfx = new("vnd.intu.qfx", new string[] { "qfx" });
            public static readonly Application VendorIptcG2CatalogitemXml = new("vnd.iptc.g2.catalogitem+xml", new string[] { "xml" });
            public static readonly Application VendorIptcG2ConceptitemXml = new("vnd.iptc.g2.conceptitem+xml", new string[] { "xml" });
            public static readonly Application VendorIptcG2KnowledgeitemXml = new("vnd.iptc.g2.knowledgeitem+xml", new string[] { "xml" });
            public static readonly Application VendorIptcG2NewsitemXml = new("vnd.iptc.g2.newsitem+xml", new string[] { "xml" });
            public static readonly Application VendorIptcG2NewsmessageXml = new("vnd.iptc.g2.newsmessage+xml", new string[] { "xml" });
            public static readonly Application VendorIptcG2PackageitemXml = new("vnd.iptc.g2.packageitem+xml", new string[] { "xml" });
            public static readonly Application VendorIptcG2PlanningitemXml = new("vnd.iptc.g2.planningitem+xml", new string[] { "xml" });
            public static readonly Application VendorIpunpluggedRcprofile = new("vnd.ipunplugged.rcprofile", new string[] { "rcprofile" });
            public static readonly Application VendorIrepositoryPackageXml = new("vnd.irepository.package+xml", new string[] { "irp" });
            public static readonly Application VendorIs_Xpr = new("vnd.is-xpr", new string[] { "xpr" });
            public static readonly Application VendorIsacFcs = new("vnd.isac.fcs", new string[] { "fcs" });
            public static readonly Application VendorIso11783_10Zip = new("vnd.iso11783-10+zip", new string[] { "zip" });
            public static readonly Application VendorJam = new("vnd.jam", new string[] { "jam" });
            public static readonly Application VendorJapannet_Directory_Service = new("vnd.japannet-directory-service");
            public static readonly Application VendorJapannet_Jpnstore_Wakeup = new("vnd.japannet-jpnstore-wakeup");
            public static readonly Application VendorJapannet_Payment_Wakeup = new("vnd.japannet-payment-wakeup");
            public static readonly Application VendorJapannet_Registration = new("vnd.japannet-registration");
            public static readonly Application VendorJapannet_Registration_Wakeup = new("vnd.japannet-registration-wakeup");
            public static readonly Application VendorJapannet_Setstore_Wakeup = new("vnd.japannet-setstore-wakeup");
            public static readonly Application VendorJapannet_Verification = new("vnd.japannet-verification");
            public static readonly Application VendorJapannet_Verification_Wakeup = new("vnd.japannet-verification-wakeup");
            public static readonly Application VendorJcpJavameMidlet_Rms = new("vnd.jcp.javame.midlet-rms", new string[] { "rms" });
            public static readonly Application VendorJisp = new("vnd.jisp", new string[] { "jisp" });
            public static readonly Application VendorJoostJoda_Archive = new("vnd.joost.joda-archive", new string[] { "joda" });
            public static readonly Application VendorJskIsdn_Ngn = new("vnd.jsk.isdn-ngn");
            public static readonly Application VendorKahootz = new("vnd.kahootz", new string[] { "ktz", "ktr" });
            public static readonly Application VendorKdeKarbon = new("vnd.kde.karbon", new string[] { "karbon" });
            public static readonly Application VendorKdeKchart = new("vnd.kde.kchart", new string[] { "chrt" });
            public static readonly Application VendorKdeKformula = new("vnd.kde.kformula", new string[] { "kfo" });
            public static readonly Application VendorKdeKivio = new("vnd.kde.kivio", new string[] { "flw" });
            public static readonly Application VendorKdeKontour = new("vnd.kde.kontour", new string[] { "kon" });
            public static readonly Application VendorKdeKpresenter = new("vnd.kde.kpresenter", new string[] { "kpr", "kpt" });
            public static readonly Application VendorKdeKspread = new("vnd.kde.kspread", new string[] { "ksp" });
            public static readonly Application VendorKdeKword = new("vnd.kde.kword", new string[] { "kwd", "kwt" });
            public static readonly Application VendorKenameaapp = new("vnd.kenameaapp", new string[] { "htke" });
            public static readonly Application VendorKidspiration = new("vnd.kidspiration", new string[] { "kia" });
            public static readonly Application VendorKinar = new("vnd.kinar", new string[] { "kne", "knp" });
            public static readonly Application VendorKoan = new("vnd.koan", new string[] { "skp", "skd", "skt", "skm" });
            public static readonly Application VendorKodak_Descriptor = new("vnd.kodak-descriptor", new string[] { "sse" });
            public static readonly Application VendorLas = new("vnd.las");
            public static readonly Application VendorLasLasJson = new("vnd.las.las+json", new string[] { "json" });
            public static readonly Application VendorLasLasXml = new("vnd.las.las+xml", new string[] { "lasxml" });
            public static readonly Application VendorLaszip = new("vnd.laszip");
            public static readonly Application VendorLeapJson = new("vnd.leap+json", new string[] { "json" });
            public static readonly Application VendorLiberty_RequestXml = new("vnd.liberty-request+xml", new string[] { "xml" });
            public static readonly Application VendorLlamagraphicsLife_BalanceDesktop = new("vnd.llamagraphics.life-balance.desktop", new string[] { "lbd" });
            public static readonly Application VendorLlamagraphicsLife_BalanceExchangeXml = new("vnd.llamagraphics.life-balance.exchange+xml", new string[] { "lbe" });
            public static readonly Application VendorLogipipeCircuitZip = new("vnd.logipipe.circuit+zip", new string[] { "zip" });
            public static readonly Application VendorLoom = new("vnd.loom");
            public static readonly Application VendorLotus_1_2_3 = new("vnd.lotus-1-2-3", new string[] { "123" });
            public static readonly Application VendorLotus_Approach = new("vnd.lotus-approach", new string[] { "apr" });
            public static readonly Application VendorLotus_Freelance = new("vnd.lotus-freelance", new string[] { "pre" });
            public static readonly Application VendorLotus_Notes = new("vnd.lotus-notes", new string[] { "nsf" });
            public static readonly Application VendorLotus_Organizer = new("vnd.lotus-organizer", new string[] { "org" });
            public static readonly Application VendorLotus_Screencam = new("vnd.lotus-screencam", new string[] { "scm" });
            public static readonly Application VendorLotus_Wordpro = new("vnd.lotus-wordpro", new string[] { "lwp" });
            public static readonly Application VendorMacportsPortpkg = new("vnd.macports.portpkg", new string[] { "portpkg" });
            public static readonly Application VendorMapbox_Vector_Tile = new("vnd.mapbox-vector-tile");
            public static readonly Application VendorMarlinDrmActiontokenXml = new("vnd.marlin.drm.actiontoken+xml", new string[] { "xml" });
            public static readonly Application VendorMarlinDrmConftokenXml = new("vnd.marlin.drm.conftoken+xml", new string[] { "xml" });
            public static readonly Application VendorMarlinDrmLicenseXml = new("vnd.marlin.drm.license+xml", new string[] { "xml" });
            public static readonly Application VendorMarlinDrmMdcf = new("vnd.marlin.drm.mdcf");
            public static readonly Application VendorMasonJson = new("vnd.mason+json", new string[] { "json" });
            public static readonly Application VendorMaxmindMaxmind_Db = new("vnd.maxmind.maxmind-db");
            public static readonly Application VendorMcd = new("vnd.mcd", new string[] { "mcd" });
            public static readonly Application VendorMedcalcdata = new("vnd.medcalcdata", new string[] { "mc1" });
            public static readonly Application VendorMediastationCdkey = new("vnd.mediastation.cdkey", new string[] { "cdkey" });
            public static readonly Application VendorMeridian_Slingshot = new("vnd.meridian-slingshot");
            public static readonly Application VendorMFER = new("vnd.mfer", new string[] { "mwf" });
            public static readonly Application VendorMfmp = new("vnd.mfmp", new string[] { "mfm" });
            public static readonly Application VendorMicrografxFlo = new("vnd.micrografx.flo", new string[] { "flo" });
            public static readonly Application VendorMicrografxIgx = new("vnd.micrografx.igx", new string[] { "igx" });
            public static readonly Application VendorMicroJson = new("vnd.micro+json", new string[] { "json" });
            public static readonly Application VendorMicrosoftPortable_Executable = new("vnd.microsoft.portable-executable");
            public static readonly Application VendorMicrosoftWindowsThumbnail_Cache = new("vnd.microsoft.windows.thumbnail-cache");
            public static readonly Application VendorMieleJson = new("vnd.miele+json", new string[] { "json" });
            public static readonly Application VendorMif = new("vnd.mif", new string[] { "mif" });
            public static readonly Application VendorMinisoft_Hp3000_Save = new("vnd.minisoft-hp3000-save");
            public static readonly Application VendorMitsubishiMisty_GuardTrustweb = new("vnd.mitsubishi.misty-guard.trustweb");
            public static readonly Application VendorMobiusDAF = new("vnd.mobius.daf", new string[] { "daf" });
            public static readonly Application VendorMobiusDIS = new("vnd.mobius.dis", new string[] { "dis" });
            public static readonly Application VendorMobiusMBK = new("vnd.mobius.mbk", new string[] { "mbk" });
            public static readonly Application VendorMobiusMQY = new("vnd.mobius.mqy", new string[] { "mqy" });
            public static readonly Application VendorMobiusMSL = new("vnd.mobius.msl", new string[] { "msl" });
            public static readonly Application VendorMobiusPLC = new("vnd.mobius.plc", new string[] { "plc" });
            public static readonly Application VendorMobiusTXF = new("vnd.mobius.txf", new string[] { "txf" });
            public static readonly Application VendorMophunApplication = new("vnd.mophun.application", new string[] { "mpn" });
            public static readonly Application VendorMophunCertificate = new("vnd.mophun.certificate", new string[] { "mpc" });
            public static readonly Application VendorMotorolaFlexsuite = new("vnd.motorola.flexsuite");
            public static readonly Application VendorMotorolaFlexsuiteAdsi = new("vnd.motorola.flexsuite.adsi");
            public static readonly Application VendorMotorolaFlexsuiteFis = new("vnd.motorola.flexsuite.fis");
            public static readonly Application VendorMotorolaFlexsuiteGotap = new("vnd.motorola.flexsuite.gotap");
            public static readonly Application VendorMotorolaFlexsuiteKmr = new("vnd.motorola.flexsuite.kmr");
            public static readonly Application VendorMotorolaFlexsuiteTtc = new("vnd.motorola.flexsuite.ttc");
            public static readonly Application VendorMotorolaFlexsuiteWem = new("vnd.motorola.flexsuite.wem");
            public static readonly Application VendorMotorolaIprm = new("vnd.motorola.iprm");
            public static readonly Application VendorMozillaXulXml = new("vnd.mozilla.xul+xml", new string[] { "xul" });
            public static readonly Application VendorMs_3mfdocument = new("vnd.ms-3mfdocument");
            public static readonly Application VendorMs_Artgalry = new("vnd.ms-artgalry", new string[] { "cil" });
            public static readonly Application VendorMs_Asf = new("vnd.ms-asf");
            public static readonly Application VendorMs_Cab_Compressed = new("vnd.ms-cab-compressed", new string[] { "cab" });
            public static readonly Application VendorMs_ColorIccprofile = new("vnd.ms-color.iccprofile");
            public static readonly Application VendorMs_Excel = new("vnd.ms-excel", new string[] { "xls", "xlm", "xla", "xlc", "xlt", "xlw" });
            public static readonly Application VendorMs_ExcelAddinMacroEnabled12 = new("vnd.ms-excel.addin.macroenabled.12", new string[] { "xlam" });
            public static readonly Application VendorMs_ExcelSheetBinaryMacroEnabled12 = new("vnd.ms-excel.sheet.binary.macroenabled.12", new string[] { "xlsb" });
            public static readonly Application VendorMs_ExcelSheetMacroEnabled12 = new("vnd.ms-excel.sheet.macroenabled.12", new string[] { "xlsm" });
            public static readonly Application VendorMs_ExcelTemplateMacroEnabled12 = new("vnd.ms-excel.template.macroenabled.12", new string[] { "xltm" });
            public static readonly Application VendorMs_Fontobject = new("vnd.ms-fontobject", new string[] { "eot" });
            public static readonly Application VendorMs_Htmlhelp = new("vnd.ms-htmlhelp", new string[] { "chm" });
            public static readonly Application VendorMs_Ims = new("vnd.ms-ims", new string[] { "ims" });
            public static readonly Application VendorMs_Lrm = new("vnd.ms-lrm", new string[] { "lrm" });
            public static readonly Application VendorMs_OfficeActiveXXml = new("vnd.ms-office.activex+xml", new string[] { "xml" });
            public static readonly Application VendorMs_Officetheme = new("vnd.ms-officetheme", new string[] { "thmx" });
            public static readonly Application VendorMs_Opentype = new("vnd.ms-opentype");
            public static readonly Application VendorMs_PackageObfuscated_Opentype = new("vnd.ms-package.obfuscated-opentype");
            public static readonly Application VendorMs_PkiSeccat = new("vnd.ms-pki.seccat", new string[] { "cat" });
            public static readonly Application VendorMs_PkiStl = new("vnd.ms-pki.stl", new string[] { "stl" });
            public static readonly Application VendorMs_PlayreadyInitiatorXml = new("vnd.ms-playready.initiator+xml", new string[] { "xml" });
            public static readonly Application VendorMs_Powerpoint = new("vnd.ms-powerpoint", new string[] { "ppt", "pps", "pot" });
            public static readonly Application VendorMs_PowerpointAddinMacroEnabled12 = new("vnd.ms-powerpoint.addin.macroenabled.12", new string[] { "ppam" });
            public static readonly Application VendorMs_PowerpointPresentationMacroEnabled12 = new("vnd.ms-powerpoint.presentation.macroenabled.12", new string[] { "pptm" });
            public static readonly Application VendorMs_PowerpointSlideMacroEnabled12 = new("vnd.ms-powerpoint.slide.macroenabled.12", new string[] { "sldm" });
            public static readonly Application VendorMs_PowerpointSlideshowMacroEnabled12 = new("vnd.ms-powerpoint.slideshow.macroenabled.12", new string[] { "ppsm" });
            public static readonly Application VendorMs_PowerpointTemplateMacroEnabled12 = new("vnd.ms-powerpoint.template.macroenabled.12", new string[] { "potm" });
            public static readonly Application VendorMs_PrintDeviceCapabilitiesXml = new("vnd.ms-printdevicecapabilities+xml", new string[] { "xml" });
            public static readonly Application VendorMs_PrintingPrintticketXml = new("vnd.ms-printing.printticket+xml", new string[] { "xml" });
            public static readonly Application VendorMs_PrintSchemaTicketXml = new("vnd.ms-printschematicket+xml", new string[] { "xml" });
            public static readonly Application VendorMs_Project = new("vnd.ms-project", new string[] { "mpp", "mpt" });
            public static readonly Application VendorMs_Tnef = new("vnd.ms-tnef");
            public static readonly Application VendorMs_WindowsDevicepairing = new("vnd.ms-windows.devicepairing");
            public static readonly Application VendorMs_WindowsNwprintingOob = new("vnd.ms-windows.nwprinting.oob");
            public static readonly Application VendorMs_WindowsPrinterpairing = new("vnd.ms-windows.printerpairing");
            public static readonly Application VendorMs_WindowsWsdOob = new("vnd.ms-windows.wsd.oob");
            public static readonly Application VendorMs_WmdrmLic_Chlg_Req = new("vnd.ms-wmdrm.lic-chlg-req");
            public static readonly Application VendorMs_WmdrmLic_Resp = new("vnd.ms-wmdrm.lic-resp");
            public static readonly Application VendorMs_WmdrmMeter_Chlg_Req = new("vnd.ms-wmdrm.meter-chlg-req");
            public static readonly Application VendorMs_WmdrmMeter_Resp = new("vnd.ms-wmdrm.meter-resp");
            public static readonly Application VendorMs_WordDocumentMacroEnabled12 = new("vnd.ms-word.document.macroenabled.12", new string[] { "docm" });
            public static readonly Application VendorMs_WordTemplateMacroEnabled12 = new("vnd.ms-word.template.macroenabled.12", new string[] { "dotm" });
            public static readonly Application VendorMs_Works = new("vnd.ms-works", new string[] { "wps", "wks", "wcm", "wdb" });
            public static readonly Application VendorMs_Wpl = new("vnd.ms-wpl", new string[] { "wpl" });
            public static readonly Application VendorMs_Xpsdocument = new("vnd.ms-xpsdocument", new string[] { "xps" });
            public static readonly Application VendorMsa_Disk_Image = new("vnd.msa-disk-image");
            public static readonly Application VendorMseq = new("vnd.mseq", new string[] { "mseq" });
            public static readonly Application VendorMsign = new("vnd.msign");
            public static readonly Application VendorMultiadCreator = new("vnd.multiad.creator");
            public static readonly Application VendorMultiadCreatorCif = new("vnd.multiad.creator.cif");
            public static readonly Application VendorMusic_Niff = new("vnd.music-niff");
            public static readonly Application VendorMusician = new("vnd.musician", new string[] { "mus" });
            public static readonly Application VendorMuveeStyle = new("vnd.muvee.style", new string[] { "msty" });
            public static readonly Application VendorMynfc = new("vnd.mynfc", new string[] { "taglet" });
            public static readonly Application VendorNcdControl = new("vnd.ncd.control");
            public static readonly Application VendorNcdReference = new("vnd.ncd.reference");
            public static readonly Application VendorNearstInvJson = new("vnd.nearst.inv+json", new string[] { "json" });
            public static readonly Application VendorNervana = new("vnd.nervana");
            public static readonly Application VendorNetfpx = new("vnd.netfpx");
            public static readonly Application VendorNeurolanguageNlu = new("vnd.neurolanguage.nlu", new string[] { "nlu" });
            public static readonly Application VendorNimn = new("vnd.nimn");
            public static readonly Application VendorNintendoNitroRom = new("vnd.nintendo.nitro.rom");
            public static readonly Application VendorNintendoSnesRom = new("vnd.nintendo.snes.rom");
            public static readonly Application VendorNitf = new("vnd.nitf", new string[] { "ntf", "nitf" });
            public static readonly Application VendorNoblenet_Directory = new("vnd.noblenet-directory", new string[] { "nnd" });
            public static readonly Application VendorNoblenet_Sealer = new("vnd.noblenet-sealer", new string[] { "nns" });
            public static readonly Application VendorNoblenet_Web = new("vnd.noblenet-web", new string[] { "nnw" });
            public static readonly Application VendorNokiaCatalogs = new("vnd.nokia.catalogs");
            public static readonly Application VendorNokiaConmlWbxml = new("vnd.nokia.conml+wbxml", new string[] { "wbxml" });
            public static readonly Application VendorNokiaConmlXml = new("vnd.nokia.conml+xml", new string[] { "xml" });
            public static readonly Application VendorNokiaIptvConfigXml = new("vnd.nokia.iptv.config+xml", new string[] { "xml" });
            public static readonly Application VendorNokiaISDS_Radio_Presets = new("vnd.nokia.isds-radio-presets");
            public static readonly Application VendorNokiaLandmarkcollectionXml = new("vnd.nokia.landmarkcollection+xml", new string[] { "xml" });
            public static readonly Application VendorNokiaLandmarkWbxml = new("vnd.nokia.landmark+wbxml", new string[] { "wbxml" });
            public static readonly Application VendorNokiaLandmarkXml = new("vnd.nokia.landmark+xml", new string[] { "xml" });
            public static readonly Application VendorNokiaN_GageAcXml = new("vnd.nokia.n-gage.ac+xml", new string[] { "xml" });
            public static readonly Application VendorNokiaN_GageData = new("vnd.nokia.n-gage.data", new string[] { "ngdat" });

            [System.Obsolete("OBSOLETE; no replacement given")]
            public static readonly Application VendorNokiaN_GageSymbianInstall = new("vnd.nokia.n-gage.symbian.install", new string[] { "n-gage" });

            public static readonly Application VendorNokiaNcd = new("vnd.nokia.ncd");
            public static readonly Application VendorNokiaPcdWbxml = new("vnd.nokia.pcd+wbxml", new string[] { "wbxml" });
            public static readonly Application VendorNokiaPcdXml = new("vnd.nokia.pcd+xml", new string[] { "xml" });
            public static readonly Application VendorNokiaRadio_Preset = new("vnd.nokia.radio-preset", new string[] { "rpst" });
            public static readonly Application VendorNokiaRadio_Presets = new("vnd.nokia.radio-presets", new string[] { "rpss" });
            public static readonly Application VendorNovadigmEDM = new("vnd.novadigm.edm", new string[] { "edm" });
            public static readonly Application VendorNovadigmEDX = new("vnd.novadigm.edx", new string[] { "edx" });
            public static readonly Application VendorNovadigmEXT = new("vnd.novadigm.ext", new string[] { "ext" });
            public static readonly Application VendorNtt_LocalContent_Share = new("vnd.ntt-local.content-share");
            public static readonly Application VendorNtt_LocalFile_Transfer = new("vnd.ntt-local.file-transfer");
            public static readonly Application VendorNtt_LocalOgw_remote_Access = new("vnd.ntt-local.ogw_remote-access");
            public static readonly Application VendorNtt_LocalSip_Ta_remote = new("vnd.ntt-local.sip-ta_remote");
            public static readonly Application VendorNtt_LocalSip_Ta_tcp_stream = new("vnd.ntt-local.sip-ta_tcp_stream");
            public static readonly Application VendorOasisOpendocumentChart = new("vnd.oasis.opendocument.chart", new string[] { "odc" });
            public static readonly Application VendorOasisOpendocumentChart_Template = new("vnd.oasis.opendocument.chart-template", new string[] { "otc" });
            public static readonly Application VendorOasisOpendocumentDatabase = new("vnd.oasis.opendocument.database", new string[] { "odb" });
            public static readonly Application VendorOasisOpendocumentFormula = new("vnd.oasis.opendocument.formula", new string[] { "odf" });
            public static readonly Application VendorOasisOpendocumentFormula_Template = new("vnd.oasis.opendocument.formula-template", new string[] { "odft" });
            public static readonly Application VendorOasisOpendocumentGraphics = new("vnd.oasis.opendocument.graphics", new string[] { "odg" });
            public static readonly Application VendorOasisOpendocumentGraphics_Template = new("vnd.oasis.opendocument.graphics-template", new string[] { "otg" });
            public static readonly Application VendorOasisOpendocumentImage = new("vnd.oasis.opendocument.image", new string[] { "odi" });
            public static readonly Application VendorOasisOpendocumentImage_Template = new("vnd.oasis.opendocument.image-template", new string[] { "oti" });
            public static readonly Application VendorOasisOpendocumentPresentation = new("vnd.oasis.opendocument.presentation", new string[] { "odp" });
            public static readonly Application VendorOasisOpendocumentPresentation_Template = new("vnd.oasis.opendocument.presentation-template", new string[] { "otp" });
            public static readonly Application VendorOasisOpendocumentSpreadsheet = new("vnd.oasis.opendocument.spreadsheet", new string[] { "ods" });
            public static readonly Application VendorOasisOpendocumentSpreadsheet_Template = new("vnd.oasis.opendocument.spreadsheet-template", new string[] { "ots" });
            public static readonly Application VendorOasisOpendocumentText = new("vnd.oasis.opendocument.text", new string[] { "odt" });
            public static readonly Application VendorOasisOpendocumentText_Master = new("vnd.oasis.opendocument.text-master", new string[] { "odm" });
            public static readonly Application VendorOasisOpendocumentText_Template = new("vnd.oasis.opendocument.text-template", new string[] { "ott" });
            public static readonly Application VendorOasisOpendocumentText_Web = new("vnd.oasis.opendocument.text-web", new string[] { "oth" });
            public static readonly Application VendorObn = new("vnd.obn");
            public static readonly Application VendorOcfCbor = new("vnd.ocf+cbor", new string[] { "cbor" });
            public static readonly Application VendorOftnL10nJson = new("vnd.oftn.l10n+json", new string[] { "json" });
            public static readonly Application VendorOipfContentaccessdownloadXml = new("vnd.oipf.contentaccessdownload+xml", new string[] { "xml" });
            public static readonly Application VendorOipfContentaccessstreamingXml = new("vnd.oipf.contentaccessstreaming+xml", new string[] { "xml" });
            public static readonly Application VendorOipfCspg_Hexbinary = new("vnd.oipf.cspg-hexbinary");
            public static readonly Application VendorOipfDaeSvgXml = new("vnd.oipf.dae.svg+xml", new string[] { "xml" });
            public static readonly Application VendorOipfDaeXhtmlXml = new("vnd.oipf.dae.xhtml+xml", new string[] { "xml" });
            public static readonly Application VendorOipfMippvcontrolmessageXml = new("vnd.oipf.mippvcontrolmessage+xml", new string[] { "xml" });
            public static readonly Application VendorOipfPaeGem = new("vnd.oipf.pae.gem");
            public static readonly Application VendorOipfSpdiscoveryXml = new("vnd.oipf.spdiscovery+xml", new string[] { "xml" });
            public static readonly Application VendorOipfSpdlistXml = new("vnd.oipf.spdlist+xml", new string[] { "xml" });
            public static readonly Application VendorOipfUeprofileXml = new("vnd.oipf.ueprofile+xml", new string[] { "xml" });
            public static readonly Application VendorOipfUserprofileXml = new("vnd.oipf.userprofile+xml", new string[] { "xml" });
            public static readonly Application VendorOlpc_Sugar = new("vnd.olpc-sugar", new string[] { "xo" });
            public static readonly Application VendorOma_Scws_Config = new("vnd.oma-scws-config");
            public static readonly Application VendorOma_Scws_Http_Request = new("vnd.oma-scws-http-request");
            public static readonly Application VendorOma_Scws_Http_Response = new("vnd.oma-scws-http-response");
            public static readonly Application VendorOmaBcastAssociated_Procedure_ParameterXml = new("vnd.oma.bcast.associated-procedure-parameter+xml", new string[] { "xml" });
            public static readonly Application VendorOmaBcastDrm_TriggerXml = new("vnd.oma.bcast.drm-trigger+xml", new string[] { "xml" });
            public static readonly Application VendorOmaBcastImdXml = new("vnd.oma.bcast.imd+xml", new string[] { "xml" });
            public static readonly Application VendorOmaBcastLtkm = new("vnd.oma.bcast.ltkm");
            public static readonly Application VendorOmaBcastNotificationXml = new("vnd.oma.bcast.notification+xml", new string[] { "xml" });
            public static readonly Application VendorOmaBcastProvisioningtrigger = new("vnd.oma.bcast.provisioningtrigger");
            public static readonly Application VendorOmaBcastSgboot = new("vnd.oma.bcast.sgboot");
            public static readonly Application VendorOmaBcastSgddXml = new("vnd.oma.bcast.sgdd+xml", new string[] { "xml" });
            public static readonly Application VendorOmaBcastSgdu = new("vnd.oma.bcast.sgdu");
            public static readonly Application VendorOmaBcastSimple_Symbol_Container = new("vnd.oma.bcast.simple-symbol-container");
            public static readonly Application VendorOmaBcastSmartcard_TriggerXml = new("vnd.oma.bcast.smartcard-trigger+xml", new string[] { "xml" });
            public static readonly Application VendorOmaBcastSprovXml = new("vnd.oma.bcast.sprov+xml", new string[] { "xml" });
            public static readonly Application VendorOmaBcastStkm = new("vnd.oma.bcast.stkm");
            public static readonly Application VendorOmaCab_Address_BookXml = new("vnd.oma.cab-address-book+xml", new string[] { "xml" });
            public static readonly Application VendorOmaCab_Feature_HandlerXml = new("vnd.oma.cab-feature-handler+xml", new string[] { "xml" });
            public static readonly Application VendorOmaCab_PccXml = new("vnd.oma.cab-pcc+xml", new string[] { "xml" });
            public static readonly Application VendorOmaCab_Subs_InviteXml = new("vnd.oma.cab-subs-invite+xml", new string[] { "xml" });
            public static readonly Application VendorOmaCab_User_PrefsXml = new("vnd.oma.cab-user-prefs+xml", new string[] { "xml" });
            public static readonly Application VendorOmaDcd = new("vnd.oma.dcd");
            public static readonly Application VendorOmaDcdc = new("vnd.oma.dcdc");
            public static readonly Application VendorOmaDd2Xml = new("vnd.oma.dd2+xml", new string[] { "dd2" });
            public static readonly Application VendorOmaDrmRisdXml = new("vnd.oma.drm.risd+xml", new string[] { "xml" });
            public static readonly Application VendorOmads_EmailXml = new("vnd.omads-email+xml", new string[] { "xml" });
            public static readonly Application VendorOmads_FileXml = new("vnd.omads-file+xml", new string[] { "xml" });
            public static readonly Application VendorOmads_FolderXml = new("vnd.omads-folder+xml", new string[] { "xml" });
            public static readonly Application VendorOmaGroup_Usage_ListXml = new("vnd.oma.group-usage-list+xml", new string[] { "xml" });
            public static readonly Application VendorOmaloc_Supl_Init = new("vnd.omaloc-supl-init");
            public static readonly Application VendorOmaLwm2mJson = new("vnd.oma.lwm2m+json", new string[] { "json" });
            public static readonly Application VendorOmaLwm2mTlv = new("vnd.oma.lwm2m+tlv", new string[] { "tlv" });
            public static readonly Application VendorOmaPalXml = new("vnd.oma.pal+xml", new string[] { "xml" });
            public static readonly Application VendorOmaPocDetailed_Progress_ReportXml = new("vnd.oma.poc.detailed-progress-report+xml", new string[] { "xml" });
            public static readonly Application VendorOmaPocFinal_ReportXml = new("vnd.oma.poc.final-report+xml", new string[] { "xml" });
            public static readonly Application VendorOmaPocGroupsXml = new("vnd.oma.poc.groups+xml", new string[] { "xml" });
            public static readonly Application VendorOmaPocInvocation_DescriptorXml = new("vnd.oma.poc.invocation-descriptor+xml", new string[] { "xml" });
            public static readonly Application VendorOmaPocOptimized_Progress_ReportXml = new("vnd.oma.poc.optimized-progress-report+xml", new string[] { "xml" });
            public static readonly Application VendorOmaPush = new("vnd.oma.push");
            public static readonly Application VendorOmaScidmMessagesXml = new("vnd.oma.scidm.messages+xml", new string[] { "xml" });
            public static readonly Application VendorOmaXcap_DirectoryXml = new("vnd.oma.xcap-directory+xml", new string[] { "xml" });
            public static readonly Application VendorOnepager = new("vnd.onepager");
            public static readonly Application VendorOnepagertamp = new("vnd.onepagertamp");
            public static readonly Application VendorOnepagertamx = new("vnd.onepagertamx");
            public static readonly Application VendorOnepagertat = new("vnd.onepagertat");
            public static readonly Application VendorOnepagertatp = new("vnd.onepagertatp");
            public static readonly Application VendorOnepagertatx = new("vnd.onepagertatx");
            public static readonly Application VendorOpenbloxGame_Binary = new("vnd.openblox.game-binary");
            public static readonly Application VendorOpenbloxGameXml = new("vnd.openblox.game+xml", new string[] { "xml" });
            public static readonly Application VendorOpeneyeOeb = new("vnd.openeye.oeb");
            public static readonly Application VendorOpenofficeorgExtension = new("vnd.openofficeorg.extension", new string[] { "oxt" });
            public static readonly Application VendorOpenstreetmapDataXml = new("vnd.openstreetmap.data+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentCustom_PropertiesXml = new("vnd.openxmlformats-officedocument.custom-properties+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentCustomXmlPropertiesXml = new("vnd.openxmlformats-officedocument.customxmlproperties+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentDrawingmlChartshapesXml = new("vnd.openxmlformats-officedocument.drawingml.chartshapes+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentDrawingmlChartXml = new("vnd.openxmlformats-officedocument.drawingml.chart+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentDrawingmlDiagramColorsXml = new("vnd.openxmlformats-officedocument.drawingml.diagramcolors+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentDrawingmlDiagramDataXml = new("vnd.openxmlformats-officedocument.drawingml.diagramdata+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentDrawingmlDiagramLayoutXml = new("vnd.openxmlformats-officedocument.drawingml.diagramlayout+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentDrawingmlDiagramStyleXml = new("vnd.openxmlformats-officedocument.drawingml.diagramstyle+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentDrawingXml = new("vnd.openxmlformats-officedocument.drawing+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentExtended_PropertiesXml = new("vnd.openxmlformats-officedocument.extended-properties+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlCommentAuthorsXml = new("vnd.openxmlformats-officedocument.presentationml.commentauthors+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlCommentsXml = new("vnd.openxmlformats-officedocument.presentationml.comments+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlHandoutMasterXml = new("vnd.openxmlformats-officedocument.presentationml.handoutmaster+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlNotesMasterXml = new("vnd.openxmlformats-officedocument.presentationml.notesmaster+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlNotesSlideXml = new("vnd.openxmlformats-officedocument.presentationml.notesslide+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlPresentation = new("vnd.openxmlformats-officedocument.presentationml.presentation", new string[] { "pptx" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlPresentationMainXml = new("vnd.openxmlformats-officedocument.presentationml.presentation.main+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlPresPropsXml = new("vnd.openxmlformats-officedocument.presentationml.presprops+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlSlide = new("vnd.openxmlformats-officedocument.presentationml.slide", new string[] { "sldx" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlSlideLayoutXml = new("vnd.openxmlformats-officedocument.presentationml.slidelayout+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlSlideMasterXml = new("vnd.openxmlformats-officedocument.presentationml.slidemaster+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlSlideshow = new("vnd.openxmlformats-officedocument.presentationml.slideshow", new string[] { "ppsx" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlSlideshowMainXml = new("vnd.openxmlformats-officedocument.presentationml.slideshow.main+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlSlideUpdateInfoXml = new("vnd.openxmlformats-officedocument.presentationml.slideupdateinfo+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlSlideXml = new("vnd.openxmlformats-officedocument.presentationml.slide+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlTableStylesXml = new("vnd.openxmlformats-officedocument.presentationml.tablestyles+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlTagsXml = new("vnd.openxmlformats-officedocument.presentationml.tags+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlTemplate = new("vnd.openxmlformats-officedocument.presentationml.template", new string[] { "potx" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlTemplateMainXml = new("vnd.openxmlformats-officedocument.presentationml.template.main+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlViewPropsXml = new("vnd.openxmlformats-officedocument.presentationml.viewprops+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlCalcChainXml = new("vnd.openxmlformats-officedocument.spreadsheetml.calcchain+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlChartsheetXml = new("vnd.openxmlformats-officedocument.spreadsheetml.chartsheet+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlCommentsXml = new("vnd.openxmlformats-officedocument.spreadsheetml.comments+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlConnectionsXml = new("vnd.openxmlformats-officedocument.spreadsheetml.connections+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlDialogsheetXml = new("vnd.openxmlformats-officedocument.spreadsheetml.dialogsheet+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlExternalLinkXml = new("vnd.openxmlformats-officedocument.spreadsheetml.externallink+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlPivotCacheDefinitionXml = new("vnd.openxmlformats-officedocument.spreadsheetml.pivotcachedefinition+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlPivotCacheRecordsXml = new("vnd.openxmlformats-officedocument.spreadsheetml.pivotcacherecords+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlPivotTableXml = new("vnd.openxmlformats-officedocument.spreadsheetml.pivottable+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlQueryTableXml = new("vnd.openxmlformats-officedocument.spreadsheetml.querytable+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlRevisionHeadersXml = new("vnd.openxmlformats-officedocument.spreadsheetml.revisionheaders+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlRevisionLogXml = new("vnd.openxmlformats-officedocument.spreadsheetml.revisionlog+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlSharedStringsXml = new("vnd.openxmlformats-officedocument.spreadsheetml.sharedstrings+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlSheet = new("vnd.openxmlformats-officedocument.spreadsheetml.sheet", new string[] { "xlsx" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlSheetMainXml = new("vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlSheetMetadataXml = new("vnd.openxmlformats-officedocument.spreadsheetml.sheetmetadata+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlStylesXml = new("vnd.openxmlformats-officedocument.spreadsheetml.styles+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlTableSingleCellsXml = new("vnd.openxmlformats-officedocument.spreadsheetml.tablesinglecells+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlTableXml = new("vnd.openxmlformats-officedocument.spreadsheetml.table+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlTemplate = new("vnd.openxmlformats-officedocument.spreadsheetml.template", new string[] { "xltx" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlTemplateMainXml = new("vnd.openxmlformats-officedocument.spreadsheetml.template.main+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlUserNamesXml = new("vnd.openxmlformats-officedocument.spreadsheetml.usernames+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlVolatileDependenciesXml = new("vnd.openxmlformats-officedocument.spreadsheetml.volatiledependencies+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlWorksheetXml = new("vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentThemeOverrideXml = new("vnd.openxmlformats-officedocument.themeoverride+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentThemeXml = new("vnd.openxmlformats-officedocument.theme+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentVmlDrawing = new("vnd.openxmlformats-officedocument.vmldrawing");
            public static readonly Application VendorOpenxmlformats_OfficedocumentWordprocessingmlCommentsXml = new("vnd.openxmlformats-officedocument.wordprocessingml.comments+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentWordprocessingmlDocument = new("vnd.openxmlformats-officedocument.wordprocessingml.document", new string[] { "docx" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentWordprocessingmlDocumentGlossaryXml = new("vnd.openxmlformats-officedocument.wordprocessingml.document.glossary+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentWordprocessingmlDocumentMainXml = new("vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentWordprocessingmlEndnotesXml = new("vnd.openxmlformats-officedocument.wordprocessingml.endnotes+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentWordprocessingmlFontTableXml = new("vnd.openxmlformats-officedocument.wordprocessingml.fonttable+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentWordprocessingmlFooterXml = new("vnd.openxmlformats-officedocument.wordprocessingml.footer+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentWordprocessingmlFootnotesXml = new("vnd.openxmlformats-officedocument.wordprocessingml.footnotes+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentWordprocessingmlNumberingXml = new("vnd.openxmlformats-officedocument.wordprocessingml.numbering+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentWordprocessingmlSettingsXml = new("vnd.openxmlformats-officedocument.wordprocessingml.settings+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentWordprocessingmlStylesXml = new("vnd.openxmlformats-officedocument.wordprocessingml.styles+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentWordprocessingmlTemplate = new("vnd.openxmlformats-officedocument.wordprocessingml.template", new string[] { "dotx" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentWordprocessingmlTemplateMainXml = new("vnd.openxmlformats-officedocument.wordprocessingml.template.main+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentWordprocessingmlWebSettingsXml = new("vnd.openxmlformats-officedocument.wordprocessingml.websettings+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_PackageCore_PropertiesXml = new("vnd.openxmlformats-package.core-properties+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_PackageDigital_Signature_XmlsignatureXml = new("vnd.openxmlformats-package.digital-signature-xmlsignature+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_PackageRelationshipsXml = new("vnd.openxmlformats-package.relationships+xml", new string[] { "xml" });
            public static readonly Application VendorOracleResourceJson = new("vnd.oracle.resource+json", new string[] { "json" });
            public static readonly Application VendorOrangeIndata = new("vnd.orange.indata");
            public static readonly Application VendorOsaNetdeploy = new("vnd.osa.netdeploy");
            public static readonly Application VendorOsgeoMapguidePackage = new("vnd.osgeo.mapguide.package", new string[] { "mgp" });
            public static readonly Application VendorOsgiBundle = new("vnd.osgi.bundle");
            public static readonly Application VendorOsgiDp = new("vnd.osgi.dp", new string[] { "dp" });
            public static readonly Application VendorOsgiSubsystem = new("vnd.osgi.subsystem", new string[] { "esa" });
            public static readonly Application VendorOtpsCt_KipXml = new("vnd.otps.ct-kip+xml", new string[] { "xml" });
            public static readonly Application VendorOxliCountgraph = new("vnd.oxli.countgraph");
            public static readonly Application VendorPagerdutyJson = new("vnd.pagerduty+json", new string[] { "json" });
            public static readonly Application VendorPalm = new("vnd.palm", new string[] { "pdb", "pqa", "oprc" });
            public static readonly Application VendorPanoply = new("vnd.panoply");
            public static readonly Application VendorPaosXml = new("vnd.paos.xml");
            public static readonly Application VendorPatentdive = new("vnd.patentdive");
            public static readonly Application VendorPatientecommsdoc = new("vnd.patientecommsdoc");
            public static readonly Application VendorPawaafile = new("vnd.pawaafile", new string[] { "paw" });
            public static readonly Application VendorPcos = new("vnd.pcos");
            public static readonly Application VendorPgFormat = new("vnd.pg.format", new string[] { "str" });
            public static readonly Application VendorPgOsasli = new("vnd.pg.osasli", new string[] { "ei6" });
            public static readonly Application VendorPiaccessApplication_Licence = new("vnd.piaccess.application-licence");
            public static readonly Application VendorPicsel = new("vnd.picsel", new string[] { "efif" });
            public static readonly Application VendorPmiWidget = new("vnd.pmi.widget", new string[] { "wg" });
            public static readonly Application VendorPocGroup_AdvertisementXml = new("vnd.poc.group-advertisement+xml", new string[] { "xml" });
            public static readonly Application VendorPocketlearn = new("vnd.pocketlearn", new string[] { "plf" });
            public static readonly Application VendorPowerbuilder6 = new("vnd.powerbuilder6", new string[] { "pbd" });
            public static readonly Application VendorPowerbuilder6_S = new("vnd.powerbuilder6-s");
            public static readonly Application VendorPowerbuilder7 = new("vnd.powerbuilder7");
            public static readonly Application VendorPowerbuilder7_S = new("vnd.powerbuilder7-s");
            public static readonly Application VendorPowerbuilder75 = new("vnd.powerbuilder75");
            public static readonly Application VendorPowerbuilder75_S = new("vnd.powerbuilder75-s");
            public static readonly Application VendorPreminet = new("vnd.preminet");
            public static readonly Application VendorPreviewsystemsBox = new("vnd.previewsystems.box", new string[] { "box" });
            public static readonly Application VendorProteusMagazine = new("vnd.proteus.magazine", new string[] { "mgz" });
            public static readonly Application VendorPsfs = new("vnd.psfs");
            public static readonly Application VendorPublishare_Delta_Tree = new("vnd.publishare-delta-tree", new string[] { "qps" });
            public static readonly Application VendorPviPtid1 = new("vnd.pvi.ptid1", new string[] { "ptid" });
            public static readonly Application VendorPwg_Multiplexed = new("vnd.pwg-multiplexed");
            public static readonly Application VendorPwg_Xhtml_PrintXml = new("vnd.pwg-xhtml-print+xml", new string[] { "xml" });
            public static readonly Application VendorQualcommBrew_App_Res = new("vnd.qualcomm.brew-app-res");
            public static readonly Application VendorQuarantainenet = new("vnd.quarantainenet");
            public static readonly Application VendorQuarkQuarkXPress = new("vnd.quark.quarkxpress", new string[] { "qxd", "qxt", "qwd", "qwt", "qxl", "qxb" });
            public static readonly Application VendorQuobject_Quoxdocument = new("vnd.quobject-quoxdocument");
            public static readonly Application VendorRadisysMomlXml = new("vnd.radisys.moml+xml", new string[] { "xml" });
            public static readonly Application VendorRadisysMsml_Audit_ConfXml = new("vnd.radisys.msml-audit-conf+xml", new string[] { "xml" });
            public static readonly Application VendorRadisysMsml_Audit_ConnXml = new("vnd.radisys.msml-audit-conn+xml", new string[] { "xml" });
            public static readonly Application VendorRadisysMsml_Audit_DialogXml = new("vnd.radisys.msml-audit-dialog+xml", new string[] { "xml" });
            public static readonly Application VendorRadisysMsml_Audit_StreamXml = new("vnd.radisys.msml-audit-stream+xml", new string[] { "xml" });
            public static readonly Application VendorRadisysMsml_AuditXml = new("vnd.radisys.msml-audit+xml", new string[] { "xml" });
            public static readonly Application VendorRadisysMsml_ConfXml = new("vnd.radisys.msml-conf+xml", new string[] { "xml" });
            public static readonly Application VendorRadisysMsml_Dialog_BaseXml = new("vnd.radisys.msml-dialog-base+xml", new string[] { "xml" });
            public static readonly Application VendorRadisysMsml_Dialog_Fax_DetectXml = new("vnd.radisys.msml-dialog-fax-detect+xml", new string[] { "xml" });
            public static readonly Application VendorRadisysMsml_Dialog_Fax_SendrecvXml = new("vnd.radisys.msml-dialog-fax-sendrecv+xml", new string[] { "xml" });
            public static readonly Application VendorRadisysMsml_Dialog_GroupXml = new("vnd.radisys.msml-dialog-group+xml", new string[] { "xml" });
            public static readonly Application VendorRadisysMsml_Dialog_SpeechXml = new("vnd.radisys.msml-dialog-speech+xml", new string[] { "xml" });
            public static readonly Application VendorRadisysMsml_Dialog_TransformXml = new("vnd.radisys.msml-dialog-transform+xml", new string[] { "xml" });
            public static readonly Application VendorRadisysMsml_DialogXml = new("vnd.radisys.msml-dialog+xml", new string[] { "xml" });
            public static readonly Application VendorRadisysMsmlXml = new("vnd.radisys.msml+xml", new string[] { "xml" });
            public static readonly Application VendorRainstorData = new("vnd.rainstor.data");
            public static readonly Application VendorRapid = new("vnd.rapid");
            public static readonly Application VendorRar = new("vnd.rar");
            public static readonly Application VendorRealvncBed = new("vnd.realvnc.bed", new string[] { "bed" });
            public static readonly Application VendorRecordareMusicxml = new("vnd.recordare.musicxml", new string[] { "mxl" });
            public static readonly Application VendorRecordareMusicxmlXml = new("vnd.recordare.musicxml+xml", new string[] { "musicxml" });
            public static readonly Application VendorRenLearnRlprint = new("vnd.renlearn.rlprint");
            public static readonly Application VendorRestfulJson = new("vnd.restful+json", new string[] { "json" });
            public static readonly Application VendorRigCryptonote = new("vnd.rig.cryptonote", new string[] { "cryptonote" });
            public static readonly Application VendorRimCod = new("vnd.rim.cod", new string[] { "cod" });
            public static readonly Application VendorRn_Realmedia = new("vnd.rn-realmedia", new string[] { "rm" });
            public static readonly Application VendorRn_Realmedia_Vbr = new("vnd.rn-realmedia-vbr", new string[] { "rmvb" });
            public static readonly Application VendorRoute66Link66Xml = new("vnd.route66.link66+xml", new string[] { "link66" });
            public static readonly Application VendorRs_274x = new("vnd.rs-274x");
            public static readonly Application VendorRuckusDownload = new("vnd.ruckus.download");
            public static readonly Application VendorS3sms = new("vnd.s3sms");
            public static readonly Application VendorSailingtrackerTrack = new("vnd.sailingtracker.track", new string[] { "st" });
            public static readonly Application VendorSar = new("vnd.sar");
            public static readonly Application VendorSbmCid = new("vnd.sbm.cid");
            public static readonly Application VendorSbmMid2 = new("vnd.sbm.mid2");
            public static readonly Application VendorScribus = new("vnd.scribus");
            public static readonly Application VendorSealed3df = new("vnd.sealed.3df");
            public static readonly Application VendorSealedCsf = new("vnd.sealed.csf");
            public static readonly Application VendorSealedDoc = new("vnd.sealed.doc");
            public static readonly Application VendorSealedEml = new("vnd.sealed.eml");
            public static readonly Application VendorSealedmediaSoftsealHtml = new("vnd.sealedmedia.softseal.html");
            public static readonly Application VendorSealedmediaSoftsealPdf = new("vnd.sealedmedia.softseal.pdf");
            public static readonly Application VendorSealedMht = new("vnd.sealed.mht");
            public static readonly Application VendorSealedNet = new("vnd.sealed.net");
            public static readonly Application VendorSealedPpt = new("vnd.sealed.ppt");
            public static readonly Application VendorSealedTiff = new("vnd.sealed.tiff");
            public static readonly Application VendorSealedXls = new("vnd.sealed.xls");
            public static readonly Application VendorSeemail = new("vnd.seemail", new string[] { "see" });
            public static readonly Application VendorSema = new("vnd.sema", new string[] { "sema" });
            public static readonly Application VendorSemd = new("vnd.semd", new string[] { "semd" });
            public static readonly Application VendorSemf = new("vnd.semf", new string[] { "semf" });
            public static readonly Application VendorShade_Save_File = new("vnd.shade-save-file");
            public static readonly Application VendorShanaInformedFormdata = new("vnd.shana.informed.formdata", new string[] { "ifm" });
            public static readonly Application VendorShanaInformedFormtemplate = new("vnd.shana.informed.formtemplate", new string[] { "itp" });
            public static readonly Application VendorShanaInformedInterchange = new("vnd.shana.informed.interchange", new string[] { "iif" });
            public static readonly Application VendorShanaInformedPackage = new("vnd.shana.informed.package", new string[] { "ipk" });
            public static readonly Application VendorShootproofJson = new("vnd.shootproof+json", new string[] { "json" });
            public static readonly Application VendorShopkickJson = new("vnd.shopkick+json", new string[] { "json" });
            public static readonly Application VendorSigrokSession = new("vnd.sigrok.session");
            public static readonly Application VendorSimTech_MindMapper = new("vnd.simtech-mindmapper", new string[] { "twd", "twds" });
            public static readonly Application VendorSirenJson = new("vnd.siren+json", new string[] { "json" });
            public static readonly Application VendorSmaf = new("vnd.smaf", new string[] { "mmf" });
            public static readonly Application VendorSmartNotebook = new("vnd.smart.notebook");
            public static readonly Application VendorSmartTeacher = new("vnd.smart.teacher", new string[] { "teacher" });
            public static readonly Application VendorSoftware602FillerForm_Xml_Zip = new("vnd.software602.filler.form-xml-zip");
            public static readonly Application VendorSoftware602FillerFormXml = new("vnd.software602.filler.form+xml", new string[] { "xml" });
            public static readonly Application VendorSolentSdkmXml = new("vnd.solent.sdkm+xml", new string[] { "sdkm", "sdkd" });
            public static readonly Application VendorSpotfireDxp = new("vnd.spotfire.dxp", new string[] { "dxp" });
            public static readonly Application VendorSpotfireSfs = new("vnd.spotfire.sfs", new string[] { "sfs" });
            public static readonly Application VendorSqlite3 = new("vnd.sqlite3");
            public static readonly Application VendorSss_Cod = new("vnd.sss-cod");
            public static readonly Application VendorSss_Dtf = new("vnd.sss-dtf");
            public static readonly Application VendorSss_Ntf = new("vnd.sss-ntf");
            public static readonly Application VendorStardivisionCalc = new("vnd.stardivision.calc", new string[] { "sdc" });
            public static readonly Application VendorStardivisionDraw = new("vnd.stardivision.draw", new string[] { "sda" });
            public static readonly Application VendorStardivisionImpress = new("vnd.stardivision.impress", new string[] { "sdd" });
            public static readonly Application VendorStardivisionMath = new("vnd.stardivision.math", new string[] { "smf" });
            public static readonly Application VendorStardivisionWriter = new("vnd.stardivision.writer", new string[] { "sdw", "vor" });
            public static readonly Application VendorStardivisionWriter_Global = new("vnd.stardivision.writer-global", new string[] { "sgl" });
            public static readonly Application VendorStepmaniaPackage = new("vnd.stepmania.package", new string[] { "smzip" });
            public static readonly Application VendorStepmaniaStepchart = new("vnd.stepmania.stepchart", new string[] { "sm" });
            public static readonly Application VendorStreet_Stream = new("vnd.street-stream");
            public static readonly Application VendorSunWadlXml = new("vnd.sun.wadl+xml", new string[] { "xml" });
            public static readonly Application VendorSunXmlCalc = new("vnd.sun.xml.calc", new string[] { "sxc" });
            public static readonly Application VendorSunXmlCalcTemplate = new("vnd.sun.xml.calc.template", new string[] { "stc" });
            public static readonly Application VendorSunXmlDraw = new("vnd.sun.xml.draw", new string[] { "sxd" });
            public static readonly Application VendorSunXmlDrawTemplate = new("vnd.sun.xml.draw.template", new string[] { "std" });
            public static readonly Application VendorSunXmlImpress = new("vnd.sun.xml.impress", new string[] { "sxi" });
            public static readonly Application VendorSunXmlImpressTemplate = new("vnd.sun.xml.impress.template", new string[] { "sti" });
            public static readonly Application VendorSunXmlMath = new("vnd.sun.xml.math", new string[] { "sxm" });
            public static readonly Application VendorSunXmlWriter = new("vnd.sun.xml.writer", new string[] { "sxw" });
            public static readonly Application VendorSunXmlWriterGlobal = new("vnd.sun.xml.writer.global", new string[] { "sxg" });
            public static readonly Application VendorSunXmlWriterTemplate = new("vnd.sun.xml.writer.template", new string[] { "stw" });
            public static readonly Application VendorSus_Calendar = new("vnd.sus-calendar", new string[] { "sus", "susp" });
            public static readonly Application VendorSvd = new("vnd.svd", new string[] { "svd" });
            public static readonly Application VendorSwiftview_Ics = new("vnd.swiftview-ics");
            public static readonly Application VendorSymbianInstall = new("vnd.symbian.install", new string[] { "sis", "sisx" });
            public static readonly Application VendorSyncmlDmddfWbxml = new("vnd.syncml.dmddf+wbxml", new string[] { "wbxml" });
            public static readonly Application VendorSyncmlDmddfXml = new("vnd.syncml.dmddf+xml", new string[] { "xml" });
            public static readonly Application VendorSyncmlDmNotification = new("vnd.syncml.dm.notification");
            public static readonly Application VendorSyncmlDmtndsWbxml = new("vnd.syncml.dmtnds+wbxml", new string[] { "wbxml" });
            public static readonly Application VendorSyncmlDmtndsXml = new("vnd.syncml.dmtnds+xml", new string[] { "xml" });
            public static readonly Application VendorSyncmlDmWbxml = new("vnd.syncml.dm+wbxml", new string[] { "bdm" });
            public static readonly Application VendorSyncmlDmXml = new("vnd.syncml.dm+xml", new string[] { "xdm" });
            public static readonly Application VendorSyncmlDsNotification = new("vnd.syncml.ds.notification");
            public static readonly Application VendorSyncmlXml = new("vnd.syncml+xml", new string[] { "xsm" });
            public static readonly Application VendorTableschemaJson = new("vnd.tableschema+json", new string[] { "json" });
            public static readonly Application VendorTaoIntent_Module_Archive = new("vnd.tao.intent-module-archive", new string[] { "tao" });
            public static readonly Application VendorTcpdumpPcap = new("vnd.tcpdump.pcap", new string[] { "pcap", "cap", "dmp" });
            public static readonly Application VendorThink_CellPpttcJson = new("vnd.think-cell.ppttc+json", new string[] { "json" });
            public static readonly Application VendorTmdMediaflexApiXml = new("vnd.tmd.mediaflex.api+xml", new string[] { "xml" });
            public static readonly Application VendorTml = new("vnd.tml");
            public static readonly Application VendorTmobile_Livetv = new("vnd.tmobile-livetv", new string[] { "tmo" });
            public static readonly Application VendorTridTpt = new("vnd.trid.tpt", new string[] { "tpt" });
            public static readonly Application VendorTriOnesource = new("vnd.tri.onesource");
            public static readonly Application VendorTriscapeMxs = new("vnd.triscape.mxs", new string[] { "mxs" });
            public static readonly Application VendorTrueapp = new("vnd.trueapp", new string[] { "tra" });
            public static readonly Application VendorTruedoc = new("vnd.truedoc");
            public static readonly Application VendorUbisoftWebplayer = new("vnd.ubisoft.webplayer");
            public static readonly Application VendorUfdl = new("vnd.ufdl", new string[] { "ufd", "ufdl" });
            public static readonly Application VendorUiqTheme = new("vnd.uiq.theme", new string[] { "utz" });
            public static readonly Application VendorUmajin = new("vnd.umajin", new string[] { "umj" });
            public static readonly Application VendorUnity = new("vnd.unity", new string[] { "unityweb" });
            public static readonly Application VendorUomlXml = new("vnd.uoml+xml", new string[] { "uoml" });
            public static readonly Application VendorUplanetAlert = new("vnd.uplanet.alert");
            public static readonly Application VendorUplanetAlert_Wbxml = new("vnd.uplanet.alert-wbxml");
            public static readonly Application VendorUplanetBearer_Choice = new("vnd.uplanet.bearer-choice");
            public static readonly Application VendorUplanetBearer_Choice_Wbxml = new("vnd.uplanet.bearer-choice-wbxml");
            public static readonly Application VendorUplanetCacheop = new("vnd.uplanet.cacheop");
            public static readonly Application VendorUplanetCacheop_Wbxml = new("vnd.uplanet.cacheop-wbxml");
            public static readonly Application VendorUplanetChannel = new("vnd.uplanet.channel");
            public static readonly Application VendorUplanetChannel_Wbxml = new("vnd.uplanet.channel-wbxml");
            public static readonly Application VendorUplanetList = new("vnd.uplanet.list");
            public static readonly Application VendorUplanetList_Wbxml = new("vnd.uplanet.list-wbxml");
            public static readonly Application VendorUplanetListcmd = new("vnd.uplanet.listcmd");
            public static readonly Application VendorUplanetListcmd_Wbxml = new("vnd.uplanet.listcmd-wbxml");
            public static readonly Application VendorUplanetSignal = new("vnd.uplanet.signal");
            public static readonly Application VendorUri_Map = new("vnd.uri-map");
            public static readonly Application VendorValveSourceMaterial = new("vnd.valve.source.material");
            public static readonly Application VendorVcx = new("vnd.vcx", new string[] { "vcx" });
            public static readonly Application VendorVd_Study = new("vnd.vd-study");
            public static readonly Application VendorVectorworks = new("vnd.vectorworks");
            public static readonly Application VendorVelJson = new("vnd.vel+json", new string[] { "json" });
            public static readonly Application VendorVerimatrixVcas = new("vnd.verimatrix.vcas");
            public static readonly Application VendorVeryantThin = new("vnd.veryant.thin");
            public static readonly Application VendorVesEncrypted = new("vnd.ves.encrypted");
            public static readonly Application VendorVidsoftVidconference = new("vnd.vidsoft.vidconference");
            public static readonly Application VendorVisio = new("vnd.visio", new string[] { "vsd", "vst", "vss", "vsw" });
            public static readonly Application VendorVisionary = new("vnd.visionary", new string[] { "vis" });
            public static readonly Application VendorVividenceScriptfile = new("vnd.vividence.scriptfile");
            public static readonly Application VendorVsf = new("vnd.vsf", new string[] { "vsf" });
            public static readonly Application VendorWapSic = new("vnd.wap.sic");
            public static readonly Application VendorWapSlc = new("vnd.wap.slc");
            public static readonly Application VendorWapWbxml = new("vnd.wap.wbxml", new string[] { "wbxml" });
            public static readonly Application VendorWapWmlc = new("vnd.wap.wmlc", new string[] { "wmlc" });
            public static readonly Application VendorWapWmlscriptc = new("vnd.wap.wmlscriptc", new string[] { "wmlsc" });
            public static readonly Application VendorWebturbo = new("vnd.webturbo", new string[] { "wtb" });
            public static readonly Application VendorWfaP2p = new("vnd.wfa.p2p");
            public static readonly Application VendorWfaWsc = new("vnd.wfa.wsc");
            public static readonly Application VendorWindowsDevicepairing = new("vnd.windows.devicepairing");
            public static readonly Application VendorWmc = new("vnd.wmc");
            public static readonly Application VendorWmfBootstrap = new("vnd.wmf.bootstrap");
            public static readonly Application VendorWolframMathematica = new("vnd.wolfram.mathematica");
            public static readonly Application VendorWolframMathematicaPackage = new("vnd.wolfram.mathematica.package");
            public static readonly Application VendorWolframPlayer = new("vnd.wolfram.player", new string[] { "nbp" });
            public static readonly Application VendorWordperfect = new("vnd.wordperfect", new string[] { "wpd" });
            public static readonly Application VendorWqd = new("vnd.wqd", new string[] { "wqd" });
            public static readonly Application VendorWrq_Hp3000_Labelled = new("vnd.wrq-hp3000-labelled");
            public static readonly Application VendorWtStf = new("vnd.wt.stf", new string[] { "stf" });
            public static readonly Application VendorWvCspWbxml = new("vnd.wv.csp+wbxml", new string[] { "wbxml" });
            public static readonly Application VendorWvCspXml = new("vnd.wv.csp+xml", new string[] { "xml" });
            public static readonly Application VendorWvSspXml = new("vnd.wv.ssp+xml", new string[] { "xml" });
            public static readonly Application VendorXacmlJson = new("vnd.xacml+json", new string[] { "json" });
            public static readonly Application VendorXara = new("vnd.xara", new string[] { "xar" });
            public static readonly Application VendorXfdl = new("vnd.xfdl", new string[] { "xfdl" });
            public static readonly Application VendorXfdlWebform = new("vnd.xfdl.webform");
            public static readonly Application VendorXmiXml = new("vnd.xmi+xml", new string[] { "xml" });
            public static readonly Application VendorXmpieCpkg = new("vnd.xmpie.cpkg");
            public static readonly Application VendorXmpieDpkg = new("vnd.xmpie.dpkg");
            public static readonly Application VendorXmpiePlan = new("vnd.xmpie.plan");
            public static readonly Application VendorXmpiePpkg = new("vnd.xmpie.ppkg");
            public static readonly Application VendorXmpieXlim = new("vnd.xmpie.xlim");
            public static readonly Application VendorYamahaHv_Dic = new("vnd.yamaha.hv-dic", new string[] { "hvd" });
            public static readonly Application VendorYamahaHv_Script = new("vnd.yamaha.hv-script", new string[] { "hvs" });
            public static readonly Application VendorYamahaHv_Voice = new("vnd.yamaha.hv-voice", new string[] { "hvp" });
            public static readonly Application VendorYamahaOpenscoreformat = new("vnd.yamaha.openscoreformat", new string[] { "osf" });
            public static readonly Application VendorYamahaOpenscoreformatOsfpvgXml = new("vnd.yamaha.openscoreformat.osfpvg+xml", new string[] { "osfpvg" });
            public static readonly Application VendorYamahaRemote_Setup = new("vnd.yamaha.remote-setup");
            public static readonly Application VendorYamahaSmaf_Audio = new("vnd.yamaha.smaf-audio", new string[] { "saf" });
            public static readonly Application VendorYamahaSmaf_Phrase = new("vnd.yamaha.smaf-phrase", new string[] { "spf" });
            public static readonly Application VendorYamahaThrough_Ngn = new("vnd.yamaha.through-ngn");
            public static readonly Application VendorYamahaTunnel_Udpencap = new("vnd.yamaha.tunnel-udpencap");
            public static readonly Application VendorYaoweme = new("vnd.yaoweme");
            public static readonly Application VendorYellowriver_Custom_Menu = new("vnd.yellowriver-custom-menu", new string[] { "cmp" });

            [System.Obsolete("OBSOLETED in favor of video/vnd.youtube.yt")]
            public static readonly Application VendorYoutubeYt = new("vnd.youtube.yt");

            public static readonly Application VendorZul = new("vnd.zul", new string[] { "zir", "zirz" });
            public static readonly Application VendorZzazzDeckXml = new("vnd.zzazz.deck+xml", new string[] { "zaz" });
            public static readonly Application VividenceScriptfile = new("vividence.scriptfile");
            public static readonly Application VoicexmlXml = new("voicexml+xml", new string[] { "vxml" });
            public static readonly Application Voucher_CmsJson = new("voucher-cms+json", new string[] { "json" });
            public static readonly Application Vq_Rtcpxr = new("vq-rtcpxr");
            public static readonly Application WatcherinfoXml = new("watcherinfo+xml", new string[] { "xml" });
            public static readonly Application Webpush_OptionsJson = new("webpush-options+json", new string[] { "json" });
            public static readonly Application Whoispp_Query = new("whoispp-query");
            public static readonly Application Whoispp_Response = new("whoispp-response");
            public static readonly Application Widget = new("widget", new string[] { "wgt" });
            public static readonly Application Winhlp = new("winhlp", new string[] { "hlp" });
            public static readonly Application Wita = new("wita");
            public static readonly Application Wordperfect51 = new("wordperfect5.1");
            public static readonly Application WsdlXml = new("wsdl+xml", new string[] { "wsdl" });
            public static readonly Application WspolicyXml = new("wspolicy+xml", new string[] { "wspolicy" });
            public static readonly Application X_7z_Compressed = new("x-7z-compressed", new string[] { "7z" });
            public static readonly Application X_Abiword = new("x-abiword", new string[] { "abw" });
            public static readonly Application X_Ace_Compressed = new("x-ace-compressed", new string[] { "ace" });
            public static readonly Application X_Amf = new("x-amf");
            public static readonly Application X_Apple_Diskimage = new("x-apple-diskimage", new string[] { "dmg" });
            public static readonly Application X_Authorware_Bin = new("x-authorware-bin", new string[] { "aab", "x32", "u32", "vox" });
            public static readonly Application X_Authorware_Map = new("x-authorware-map", new string[] { "aam" });
            public static readonly Application X_Authorware_Seg = new("x-authorware-seg", new string[] { "aas" });
            public static readonly Application X_Bcpio = new("x-bcpio", new string[] { "bcpio" });
            public static readonly Application X_Bittorrent = new("x-bittorrent", new string[] { "torrent" });
            public static readonly Application X_Blorb = new("x-blorb", new string[] { "blb", "blorb" });
            public static readonly Application X_Bzip = new("x-bzip", new string[] { "bz" });
            public static readonly Application X_Bzip2 = new("x-bzip2", new string[] { "bz2", "boz" });
            public static readonly Application X_Cbr = new("x-cbr", new string[] { "cbr", "cba", "cbt", "cbz", "cb7" });
            public static readonly Application X_Cdlink = new("x-cdlink", new string[] { "vcd" });
            public static readonly Application X_Cfs_Compressed = new("x-cfs-compressed", new string[] { "cfs" });
            public static readonly Application X_Chat = new("x-chat", new string[] { "chat" });
            public static readonly Application X_Chess_Pgn = new("x-chess-pgn", new string[] { "pgn" });
            public static readonly Application X_Compress = new("x-compress");
            public static readonly Application X_Conference = new("x-conference", new string[] { "nsc" });
            public static readonly Application X_Cpio = new("x-cpio", new string[] { "cpio" });
            public static readonly Application X_Csh = new("x-csh", new string[] { "csh" });
            public static readonly Application X_Debian_Package = new("x-debian-package", new string[] { "deb", "udeb" });
            public static readonly Application X_Dgc_Compressed = new("x-dgc-compressed", new string[] { "dgc" });
            public static readonly Application X_Director = new("x-director", new string[] { "dir", "dcr", "dxr", "cst", "cct", "cxt", "w3d", "fgd", "swa" });
            public static readonly Application X_Doom = new("x-doom", new string[] { "wad" });
            public static readonly Application X_DtbncxXml = new("x-dtbncx+xml", new string[] { "ncx" });
            public static readonly Application X_DtbookXml = new("x-dtbook+xml", new string[] { "dtb" });
            public static readonly Application X_DtbresourceXml = new("x-dtbresource+xml", new string[] { "res" });
            public static readonly Application X_Dvi = new("x-dvi", new string[] { "dvi" });
            public static readonly Application X_Envoy = new("x-envoy", new string[] { "evy" });
            public static readonly Application X_Eva = new("x-eva", new string[] { "eva" });
            public static readonly Application X_Font_Bdf = new("x-font-bdf", new string[] { "bdf" });
            public static readonly Application X_Font_Dos = new("x-font-dos");
            public static readonly Application X_Font_Framemaker = new("x-font-framemaker");
            public static readonly Application X_Font_Ghostscript = new("x-font-ghostscript", new string[] { "gsf" });
            public static readonly Application X_Font_Libgrx = new("x-font-libgrx");
            public static readonly Application X_Font_Linux_Psf = new("x-font-linux-psf", new string[] { "psf" });
            public static readonly Application X_Font_Pcf = new("x-font-pcf", new string[] { "pcf" });
            public static readonly Application X_Font_Snf = new("x-font-snf", new string[] { "snf" });
            public static readonly Application X_Font_Speedo = new("x-font-speedo");
            public static readonly Application X_Font_Sunos_News = new("x-font-sunos-news");
            public static readonly Application X_Font_Type1 = new("x-font-type1", new string[] { "pfa", "pfb", "pfm", "afm" });
            public static readonly Application X_Font_Vfont = new("x-font-vfont");
            public static readonly Application X_Freearc = new("x-freearc", new string[] { "arc" });
            public static readonly Application X_Futuresplash = new("x-futuresplash", new string[] { "spl" });
            public static readonly Application X_Gca_Compressed = new("x-gca-compressed", new string[] { "gca" });
            public static readonly Application X_Glulx = new("x-glulx", new string[] { "ulx" });
            public static readonly Application X_Gnumeric = new("x-gnumeric", new string[] { "gnumeric" });
            public static readonly Application X_Gramps_Xml = new("x-gramps-xml", new string[] { "gramps" });
            public static readonly Application X_Gtar = new("x-gtar", new string[] { "gtar" });
            public static readonly Application X_Gzip = new("x-gzip");
            public static readonly Application X_Hdf = new("x-hdf", new string[] { "hdf" });
            public static readonly Application X_Install_Instructions = new("x-install-instructions", new string[] { "install" });
            public static readonly Application X_Iso9660_Image = new("x-iso9660-image", new string[] { "iso" });
            public static readonly Application X_Java_Jnlp_File = new("x-java-jnlp-file", new string[] { "jnlp" });
            public static readonly Application X_Latex = new("x-latex", new string[] { "latex" });
            public static readonly Application X_Lzh_Compressed = new("x-lzh-compressed", new string[] { "lzh", "lha" });
            public static readonly Application X_Mie = new("x-mie", new string[] { "mie" });
            public static readonly Application X_Mobipocket_Ebook = new("x-mobipocket-ebook", new string[] { "prc", "mobi" });
            public static readonly Application X_Ms_Application = new("x-ms-application", new string[] { "application" });
            public static readonly Application X_Ms_Shortcut = new("x-ms-shortcut", new string[] { "lnk" });
            public static readonly Application X_Ms_Wmd = new("x-ms-wmd", new string[] { "wmd" });
            public static readonly Application X_Ms_Wmz = new("x-ms-wmz", new string[] { "wmz" });
            public static readonly Application X_Ms_Xbap = new("x-ms-xbap", new string[] { "xbap" });
            public static readonly Application X_Msaccess = new("x-msaccess", new string[] { "mdb" });
            public static readonly Application X_Msbinder = new("x-msbinder", new string[] { "obd" });
            public static readonly Application X_Mscardfile = new("x-mscardfile", new string[] { "crd" });
            public static readonly Application X_Msclip = new("x-msclip", new string[] { "clp" });
            public static readonly Application X_Msdownload = new("x-msdownload", new string[] { "exe", "dll", "com", "bat", "msi" });
            public static readonly Application X_Msmediaview = new("x-msmediaview", new string[] { "mvb", "m13", "m14" });
            public static readonly Application X_Msmetafile = new("x-msmetafile", new string[] { "wmf", "wmz", "emf", "emz" });
            public static readonly Application X_Msmoney = new("x-msmoney", new string[] { "mny" });
            public static readonly Application X_Mspublisher = new("x-mspublisher", new string[] { "pub" });
            public static readonly Application X_Msschedule = new("x-msschedule", new string[] { "scd" });
            public static readonly Application X_Msterminal = new("x-msterminal", new string[] { "trm" });
            public static readonly Application X_Mswrite = new("x-mswrite", new string[] { "wri" });
            public static readonly Application X_Netcdf = new("x-netcdf", new string[] { "nc", "cdf" });
            public static readonly Application X_Nzb = new("x-nzb", new string[] { "nzb" });
            public static readonly Application X_Pkcs12 = new("x-pkcs12", new string[] { "p12", "pfx" });
            public static readonly Application X_Pkcs7_Certificates = new("x-pkcs7-certificates", new string[] { "p7b", "spc" });
            public static readonly Application X_Pkcs7_Certreqresp = new("x-pkcs7-certreqresp", new string[] { "p7r" });
            public static readonly Application X_Rar_Compressed = new("x-rar-compressed", new string[] { "rar" });
            public static readonly Application X_Research_Info_Systems = new("x-research-info-systems", new string[] { "ris" });
            public static readonly Application X_Sh = new("x-sh", new string[] { "sh" });
            public static readonly Application X_Shar = new("x-shar", new string[] { "shar" });
            public static readonly Application X_Shockwave_Flash = new("x-shockwave-flash", new string[] { "swf" });
            public static readonly Application X_Silverlight_App = new("x-silverlight-app", new string[] { "xap" });
            public static readonly Application X_Sql = new("x-sql", new string[] { "sql" });
            public static readonly Application X_Stuffit = new("x-stuffit", new string[] { "sit" });
            public static readonly Application X_Stuffitx = new("x-stuffitx", new string[] { "sitx" });
            public static readonly Application X_Subrip = new("x-subrip", new string[] { "srt" });
            public static readonly Application X_Sv4cpio = new("x-sv4cpio", new string[] { "sv4cpio" });
            public static readonly Application X_Sv4crc = new("x-sv4crc", new string[] { "sv4crc" });
            public static readonly Application X_T3vm_Image = new("x-t3vm-image", new string[] { "t3" });
            public static readonly Application X_Tads = new("x-tads", new string[] { "gam" });
            public static readonly Application X_Tar = new("x-tar", new string[] { "tar" });
            public static readonly Application X_Tcl = new("x-tcl", new string[] { "tcl" });
            public static readonly Application X_Tex = new("x-tex", new string[] { "tex" });
            public static readonly Application X_Tex_Tfm = new("x-tex-tfm", new string[] { "tfm" });
            public static readonly Application X_Texinfo = new("x-texinfo", new string[] { "texinfo", "texi" });
            public static readonly Application X_Tgif = new("x-tgif", new string[] { "obj" });
            public static readonly Application X_Ustar = new("x-ustar", new string[] { "ustar" });
            public static readonly Application X_Wais_Source = new("x-wais-source", new string[] { "src" });
            public static readonly Application X_Www_Form_Urlencoded = new("x-www-form-urlencoded");
            public static readonly Application X_X509_Ca_Cert = new("x-x509-ca-cert", new string[] { "der", "crt" });
            public static readonly Application X_Xfig = new("x-xfig", new string[] { "fig" });
            public static readonly Application X_XliffXml = new("x-xliff+xml", new string[] { "xlf" });
            public static readonly Application X_Xpinstall = new("x-xpinstall", new string[] { "xpi" });
            public static readonly Application X_Xz = new("x-xz", new string[] { "xz" });
            public static readonly Application X_Zmachine = new("x-zmachine", new string[] { "z1", "z2", "z3", "z4", "z5", "z6", "z7", "z8" });
            public static readonly Application X400_Bp = new("x400-bp");
            public static readonly Application XacmlXml = new("xacml+xml", new string[] { "xml" });
            public static readonly Application XamlXml = new("xaml+xml", new string[] { "xaml" });
            public static readonly Application Xcap_AttXml = new("xcap-att+xml", new string[] { "xml" });
            public static readonly Application Xcap_CapsXml = new("xcap-caps+xml", new string[] { "xml" });
            public static readonly Application Xcap_DiffXml = new("xcap-diff+xml", new string[] { "xdf" });
            public static readonly Application Xcap_ElXml = new("xcap-el+xml", new string[] { "xml" });
            public static readonly Application Xcap_ErrorXml = new("xcap-error+xml", new string[] { "xml" });
            public static readonly Application Xcap_NsXml = new("xcap-ns+xml", new string[] { "xml" });
            public static readonly Application Xcon_Conference_Info_DiffXml = new("xcon-conference-info-diff+xml", new string[] { "xml" });
            public static readonly Application Xcon_Conference_InfoXml = new("xcon-conference-info+xml", new string[] { "xml" });
            public static readonly Application XencXml = new("xenc+xml", new string[] { "xenc" });
            public static readonly Application Xhtml_VoiceXml = new("xhtml-voice+xml", new string[] { "xml" });
            public static readonly Application XhtmlXml = new("xhtml+xml", new string[] { "xhtml", "xht" });
            public static readonly Application XliffXml = new("xliff+xml", new string[] { "xml" });
            public static readonly Application Xml = new("xml", new string[] { "xml", "xsl" });
            public static readonly Application Xml_Dtd = new("xml-dtd", new string[] { "dtd" });
            public static readonly Application Xml_External_Parsed_Entity = new("xml-external-parsed-entity");
            public static readonly Application Xml_PatchXml = new("xml-patch+xml", new string[] { "xml" });
            public static readonly Application XmppXml = new("xmpp+xml", new string[] { "xml" });
            public static readonly Application XopXml = new("xop+xml", new string[] { "xop" });
            public static readonly Application XprocXml = new("xproc+xml", new string[] { "xpl" });
            public static readonly Application XsltXml = new("xslt+xml", new string[] { "xslt" });
            public static readonly Application XspfXml = new("xspf+xml", new string[] { "xspf" });
            public static readonly Application XvXml = new("xv+xml", new string[] { "mxml", "xhvml", "xvml", "xvm" });
            public static readonly Application Yang = new("yang", new string[] { "yang" });
            public static readonly Application Yang_DataJson = new("yang-data+json", new string[] { "json" });
            public static readonly Application Yang_DataXml = new("yang-data+xml", new string[] { "xml" });
            public static readonly Application Yang_PatchJson = new("yang-patch+json", new string[] { "json" });
            public static readonly Application Yang_PatchXml = new("yang-patch+xml", new string[] { "xml" });
            public static readonly Application YinXml = new("yin+xml", new string[] { "yin" });
            public static readonly Application Zip = new("zip", new string[] { "zip" });
            public static readonly Application Zlib = new("zlib");
            public static readonly Application Zstd = new("zstd");

            public static new readonly Application[] Values = {
                A2L,
                Activemessage,
                ActivityJson,
                Alto_CostmapfilterJson,
                Alto_CostmapJson,
                Alto_DirectoryJson,
                Alto_EndpointcostJson,
                Alto_EndpointcostparamsJson,
                Alto_EndpointpropJson,
                Alto_EndpointpropparamsJson,
                Alto_ErrorJson,
                Alto_NetworkmapfilterJson,
                Alto_NetworkmapJson,
                AML,
                Andrew_Inset,
                Applefile,
                Applixware,
                ATF,
                ATFX,
                AtomcatXml,
                AtomdeletedXml,
                Atomicmail,
                AtomsvcXml,
                AtomXml,
                Atsc_DwdXml,
                Atsc_HeldXml,
                Atsc_RdtJson,
                Atsc_RsatXml,
                ATXML,
                Auth_PolicyXml,
                Bacnet_XddZip,
                Batch_SMTP,
                BeepXml,
                CalendarJson,
                CalendarXml,
                Call_Completion,
                CALS_1840,
                Cbor,
                Cbor_Seq,
                Cccex,
                CcmpXml,
                CcxmlXml,
                CDFXXML,
                Cdmi_Capability,
                Cdmi_Container,
                Cdmi_Domain,
                Cdmi_Object,
                Cdmi_Queue,
                Cdni,
                CEA,
                Cea_2018Xml,
                CellmlXml,
                Cfw,
                Clue_infoXml,
                ClueXml,
                Cms,
                CnrpXml,
                Coap_GroupJson,
                Coap_Payload,
                Commonground,
                Conference_InfoXml,
                Cose,
                Cose_Key,
                Cose_Key_Set,
                CplXml,
                Csrattrs,
                CSTAdataXml,
                CstaXml,
                CsvmJson,
                Cu_Seeme,
                Cwt,
                Cybercash,
                Dashdelta,
                DashXml,
                DavmountXml,
                Dca_Rft,
                DCD,
                Dec_Dx,
                Dialog_InfoXml,
                Dicom,
                DicomJson,
                DicomXml,
                DII,
                DIT,
                Dns,
                Dns_Message,
                DnsJson,
                DocbookXml,
                DotsCbor,
                DskppXml,
                DsscDer,
                DsscXml,
                Dvcs,
                Ecmascript,
                EDI_Consent,
                EDI_X12,
                EDIFACT,
                Efi,
                EmergencyCallDataCommentXml,
                EmergencyCallDataControlXml,
                EmergencyCallDataDeviceInfoXml,
                EmergencyCallDataECallMSD,
                EmergencyCallDataProviderInfoXml,
                EmergencyCallDataServiceInfoXml,
                EmergencyCallDataSubscriberInfoXml,
                EmergencyCallDataVEDSXml,
                EmmaXml,
                EmotionmlXml,
                Encaprtp,
                EppXml,
                EpubZip,
                Eshop,
                Example,
                Exi,
                Expect_Ct_ReportJson,
                Fastinfoset,
                Fastsoap,
                FdtXml,
                FhirJson,
                FhirXml,
                Fits,
                Flexfec,
                Font_Tdpfr,
                Framework_AttributesXml,
                GeoJson,
                GeoJson_Seq,
                GeopackageSqlite3,
                GeoxacmlXml,
                Gltf_Buffer,
                GmlXml,
                GpxXml,
                Gxf,
                Gzip,
                H224,
                HeldXml,
                Http,
                Hyperstudio,
                Ibe_Key_RequestXml,
                Ibe_Pkg_ReplyXml,
                Ibe_Pp_Data,
                Iges,
                Im_IscomposingXml,
                Index,
                IndexCmd,
                IndexObj,
                IndexResponse,
                IndexVnd,
                InkmlXml,
                IOTP,
                Ipfix,
                Ipp,
                ISUP,
                ItsXml,
                Java_Archive,
                Java_Serialized_Object,
                Java_Vm,
                Javascript,
                Jf2feedJson,
                Jose,
                JoseJson,
                JrdJson,
                Json,
                Json_PatchJson,
                Json_Seq,
                JsonmlJson,
                Jwk_SetJson,
                JwkJson,
                Jwt,
                Kpml_RequestXml,
                Kpml_ResponseXml,
                LdJson,
                LgrXml,
                Link_Format,
                Load_ControlXml,
                LostsyncXml,
                LostXml,
                LXF,
                Mac_Binhex40,
                Mac_Compactpro,
                Macwriteii,
                MadsXml,
                Marc,
                MarcxmlXml,
                Mathematica,
                Mathml_ContentXml,
                Mathml_PresentationXml,
                MathmlXml,
                Mbms_Associated_Procedure_DescriptionXml,
                Mbms_DeregisterXml,
                Mbms_EnvelopeXml,
                Mbms_Msk_ResponseXml,
                Mbms_MskXml,
                Mbms_Protection_DescriptionXml,
                Mbms_Reception_ReportXml,
                Mbms_Register_ResponseXml,
                Mbms_RegisterXml,
                Mbms_ScheduleXml,
                Mbms_User_Service_DescriptionXml,
                Mbox,
                Media_controlXml,
                Media_Policy_DatasetXml,
                MediaservercontrolXml,
                Merge_PatchJson,
                Metalink4Xml,
                MetalinkXml,
                MetsXml,
                MF4,
                Mikey,
                Mipc,
                Mmt_AeiXml,
                Mmt_UsdXml,
                ModsXml,
                Moss_Keys,
                Moss_Signature,
                Mosskey_Data,
                Mosskey_Request,
                Mp21,
                Mp4,
                Mpeg4_Generic,
                Mpeg4_Iod,
                Mpeg4_Iod_Xmt,
                Mrb_ConsumerXml,
                Mrb_PublishXml,
                Msc_IvrXml,
                Msc_MixerXml,
                Msword,
                MudJson,
                Multipart_Core,
                Mxf,
                N_Quads,
                N_Triples,
                Nasdata,
                News_Checkgroups,
                News_Groupinfo,
                News_Transmission,
                NlsmlXml,
                Node,
                Nss,
                Ocsp_Request,
                Ocsp_Response,
                Octet_Stream,
                ODA,
                OdmXml,
                ODX,
                Oebps_PackageXml,
                Ogg,
                OmdocXml,
                Onenote,
                Oscore,
                Oxps,
                P2p_OverlayXml,
                Parityfec,
                Passport,
                Patch_Ops_ErrorXml,
                Pdf,
                PDX,
                Pem_Certificate_Chain,
                Pgp_Encrypted,
                Pgp_Keys,
                Pgp_Signature,
                Pics_Rules,
                Pidf_DiffXml,
                PidfXml,
                Pkcs10,
                Pkcs12,
                Pkcs7_Mime,
                Pkcs7_Signature,
                Pkcs8,
                Pkcs8_Encrypted,
                Pkix_Attr_Cert,
                Pkix_Cert,
                Pkix_Crl,
                Pkix_Pkipath,
                Pkixcmp,
                PlsXml,
                Poc_SettingsXml,
                Postscript,
                Ppsp_TrackerJson,
                ProblemJson,
                ProblemXml,
                ProvenanceXml,
                PrsAlvestrandTitrax_Sheet,
                PrsCww,
                PrsHpubZip,
                PrsNprend,
                PrsPlucker,
                PrsRdf_Xml_Crypt,
                PrsXsfXml,
                PskcXml,
                QSIG,
                Raptorfec,
                RdapJson,
                RdfXml,
                ReginfoXml,
                Relax_Ng_Compact_Syntax,
                Remote_Printing,
                ReputonJson,
                Resource_Lists_DiffXml,
                Resource_ListsXml,
                RfcXml,
                Riscos,
                RlmiXml,
                Rls_ServicesXml,
                Route_ApdXml,
                Route_S_TsidXml,
                Route_UsdXml,
                Rpki_Ghostbusters,
                Rpki_Manifest,
                Rpki_Publication,
                Rpki_Roa,
                Rpki_Updown,
                RsdXml,
                RssXml,
                Rtf,
                Rtploopback,
                Rtx,
                SamlassertionXml,
                SamlmetadataXml,
                SbmlXml,
                ScaipXml,
                ScimJson,
                Scvp_Cv_Request,
                Scvp_Cv_Response,
                Scvp_Vp_Request,
                Scvp_Vp_Response,
                Sdp,
                SeceventJwt,
                Senml_Exi,
                SenmlCbor,
                SenmlJson,
                SenmlXml,
                Sensml_Exi,
                SensmlCbor,
                SensmlJson,
                SensmlXml,
                Sep_Exi,
                SepXml,
                Session_Info,
                Set_Payment,
                Set_Payment_Initiation,
                Set_Registration,
                Set_Registration_Initiation,
                SGML,
                Sgml_Open_Catalog,
                ShfXml,
                Sieve,
                Simple_FilterXml,
                Simple_Message_Summary,
                SimpleSymbolContainer,
                Sipc,
                Slate,
                SmilXml,
                Smpte336m,
                SoapFastinfoset,
                SoapXml,
                Sparql_Query,
                Sparql_ResultsXml,
                Spirits_EventXml,
                Sql,
                Srgs,
                SrgsXml,
                SruXml,
                SsdlXml,
                SsmlXml,
                StixJson,
                SwidXml,
                Tamp_Apex_Update,
                Tamp_Apex_Update_Confirm,
                Tamp_Community_Update,
                Tamp_Community_Update_Confirm,
                Tamp_Error,
                Tamp_Sequence_Adjust,
                Tamp_Sequence_Adjust_Confirm,
                Tamp_Status_Query,
                Tamp_Status_Response,
                Tamp_Update,
                Tamp_Update_Confirm,
                TaxiiJson,
                TeiXml,
                TETRA_ISI,
                ThraudXml,
                Timestamp_Query,
                Timestamp_Reply,
                Timestamped_Data,
                TlsrptGzip,
                TlsrptJson,
                Tnauthlist,
                Trickle_Ice_Sdpfrag,
                Trig,
                TtmlXml,
                Tve_Trigger,
                Tzif,
                Tzif_Leap,
                Ulpfec,
                Urc_GrpsheetXml,
                Urc_RessheetXml,
                Urc_TargetdescXml,
                Urc_UisocketdescXml,
                VcardJson,
                VcardXml,
                Vemmi,
                Vendor1000mindsDecision_ModelXml,
                Vendor1d_Interleaved_Parityfec,
                Vendor3gpdash_Qoe_ReportXml,
                Vendor3gpp_ImsXml,
                Vendor3gpp_Prose_Pc3chXml,
                Vendor3gpp_ProseXml,
                Vendor3gpp_V2x_Local_Service_Information,
                Vendor3gpp2BcmcsinfoXml,
                Vendor3gpp2Sms,
                Vendor3gpp2Tcap,
                Vendor3gppAccess_Transfer_EventsXml,
                Vendor3gppBsfXml,
                Vendor3gppGMOPXml,
                Vendor3gppMc_Signalling_Ear,
                Vendor3gppMcdata_Affiliation_CommandXml,
                Vendor3gppMcdata_InfoXml,
                Vendor3gppMcdata_Payload,
                Vendor3gppMcdata_Service_ConfigXml,
                Vendor3gppMcdata_Signalling,
                Vendor3gppMcdata_Ue_ConfigXml,
                Vendor3gppMcdata_User_ProfileXml,
                Vendor3gppMcptt_Affiliation_CommandXml,
                Vendor3gppMcptt_Floor_RequestXml,
                Vendor3gppMcptt_InfoXml,
                Vendor3gppMcptt_Location_InfoXml,
                Vendor3gppMcptt_Mbms_Usage_InfoXml,
                Vendor3gppMcptt_Service_ConfigXml,
                Vendor3gppMcptt_SignedXml,
                Vendor3gppMcptt_Ue_ConfigXml,
                Vendor3gppMcptt_Ue_Init_ConfigXml,
                Vendor3gppMcptt_User_ProfileXml,
                Vendor3gppMcvideo_Affiliation_CommandXml,
                Vendor3gppMcvideo_InfoXml,
                Vendor3gppMcvideo_Location_InfoXml,
                Vendor3gppMcvideo_Mbms_Usage_InfoXml,
                Vendor3gppMcvideo_Service_ConfigXml,
                Vendor3gppMcvideo_Transmission_RequestXml,
                Vendor3gppMcvideo_Ue_ConfigXml,
                Vendor3gppMcvideo_User_ProfileXml,
                Vendor3gppMid_CallXml,
                Vendor3gppPic_Bw_Large,
                Vendor3gppPic_Bw_Small,
                Vendor3gppPic_Bw_Var,
                Vendor3gppSms,
                Vendor3gppSmsXml,
                Vendor3gppSrvcc_ExtXml,
                Vendor3gppSRVCC_InfoXml,
                Vendor3gppState_And_Event_InfoXml,
                Vendor3gppUssdXml,
                Vendor3lightssoftwareImagescal,
                Vendor3MPost_It_Notes,
                VendorAccpacSimplyAso,
                VendorAccpacSimplyImp,
                VendorAcucobol,
                VendorAcucorp,
                VendorAdobeAir_Application_Installer_PackageZip,
                VendorAdobeFlashMovie,
                VendorAdobeFormscentralFcdt,
                VendorAdobeFxp,
                VendorAdobePartial_Upload,
                VendorAdobeXdpXml,
                VendorAdobeXfdf,
                VendorAetherImp,
                VendorAfpcAfplinedata,
                VendorAfpcAfplinedata_Pagedef,
                VendorAfpcFoca_Charset,
                VendorAfpcFoca_Codedfont,
                VendorAfpcFoca_Codepage,
                VendorAfpcModca,
                VendorAfpcModca_Formdef,
                VendorAfpcModca_Mediummap,
                VendorAfpcModca_Objectcontainer,
                VendorAfpcModca_Overlay,
                VendorAfpcModca_Pagesegment,
                VendorAh_Barcode,
                VendorAheadSpace,
                VendorAirzipFilesecureAzf,
                VendorAirzipFilesecureAzs,
                VendorAmadeusJson,
                VendorAmazonEbook,
                VendorAmazonMobi8_Ebook,
                VendorAmericandynamicsAcc,
                VendorAmigaAmi,
                VendorAmundsenMazeXml,
                VendorAndroidOta,
                VendorAndroidPackage_Archive,
                VendorAnki,
                VendorAnser_Web_Certificate_Issue_Initiation,
                VendorAnser_Web_Funds_Transfer_Initiation,
                VendorAntixGame_Component,
                VendorApacheThriftBinary,
                VendorApacheThriftCompact,
                VendorApacheThriftJson,
                VendorApiJson,
                VendorAplextorWarrpJson,
                VendorApothekendeReservationJson,
                VendorAppleInstallerXml,
                VendorAppleKeynote,
                VendorAppleMpegurl,
                VendorAppleNumbers,
                VendorApplePages,
                VendorAristanetworksSwi,
                VendorArtisanJson,
                VendorArtsquare,
                VendorAstraea_SoftwareIota,
                VendorAudiograph,
                VendorAutopackage,
                VendorAvalonJson,
                VendorAvistarXml,
                VendorBalsamiqBmmlXml,
                VendorBalsamiqBmpr,
                VendorBanana_Accounting,
                VendorBbfUspError,
                VendorBbfUspMsg,
                VendorBbfUspMsgJson,
                VendorBekitzur_StechJson,
                VendorBintMed_Content,
                VendorBiopaxRdfXml,
                VendorBlink_Idb_Value_Wrapper,
                VendorBlueiceMultipass,
                VendorBluetoothEpOob,
                VendorBluetoothLeOob,
                VendorBmi,
                VendorBpf,
                VendorBpf3,
                VendorBusinessobjects,
                VendorByuUapiJson,
                VendorCab_Jscript,
                VendorCanon_Cpdl,
                VendorCanon_Lips,
                VendorCapasystems_PgJson,
                VendorCendioThinlincClientconf,
                VendorCentury_SystemsTcp_stream,
                VendorChemdrawXml,
                VendorChess_Pgn,
                VendorChipnutsKaraoke_Mmd,
                VendorCiedi,
                VendorCinderella,
                VendorCirpackIsdn_Ext,
                VendorCitationstylesStyleXml,
                VendorClaymore,
                VendorCloantoRp9,
                VendorClonkC4group,
                VendorCluetrustCartomobile_Config,
                VendorCluetrustCartomobile_Config_Pkg,
                VendorCoffeescript,
                VendorCollabioXodocumentsDocument,
                VendorCollabioXodocumentsDocument_Template,
                VendorCollabioXodocumentsPresentation,
                VendorCollabioXodocumentsPresentation_Template,
                VendorCollabioXodocumentsSpreadsheet,
                VendorCollabioXodocumentsSpreadsheet_Template,
                VendorCollectionDocJson,
                VendorCollectionJson,
                VendorCollectionNextJson,
                VendorComicbook_Rar,
                VendorComicbookZip,
                VendorCommerce_Battelle,
                VendorCommonspace,
                VendorContactCmsg,
                VendorCoreosIgnitionJson,
                VendorCosmocaller,
                VendorCrickClicker,
                VendorCrickClickerKeyboard,
                VendorCrickClickerPalette,
                VendorCrickClickerTemplate,
                VendorCrickClickerWordbank,
                VendorCriticaltoolsWbsXml,
                VendorCryptiiPipeJson,
                VendorCrypto_Shade_File,
                VendorCtc_Posml,
                VendorCtctWsXml,
                VendorCups_Pdf,
                VendorCups_Postscript,
                VendorCups_Ppd,
                VendorCups_Raster,
                VendorCups_Raw,
                VendorCurl,
                VendorCurlCar,
                VendorCurlPcurl,
                VendorCyanDeanRootXml,
                VendorCybank,
                VendorD2lCoursepackage1p0Zip,
                VendorDart,
                VendorData_VisionRdz,
                VendorDatapackageJson,
                VendorDataresourceJson,
                VendorDebianBinary_Package,
                VendorDeceData,
                VendorDeceTtmlXml,
                VendorDeceUnspecified,
                VendorDeceZip,
                VendorDenovoFcselayout_Link,
                VendorDesmumeMovie,
                VendorDir_BiPlate_Dl_Nosuffix,
                VendorDmDelegationXml,
                VendorDna,
                VendorDocumentJson,
                VendorDolbyMlp,
                VendorDolbyMobile1,
                VendorDolbyMobile2,
                VendorDoremirScorecloud_Binary_Document,
                VendorDpgraph,
                VendorDreamfactory,
                VendorDriveJson,
                VendorDs_Keypoint,
                VendorDtgLocal,
                VendorDtgLocalFlash,
                VendorDtgLocalHtml,
                VendorDvbAit,
                VendorDvbDvbj,
                VendorDvbEsgcontainer,
                VendorDvbIpdcdftnotifaccess,
                VendorDvbIpdcesgaccess,
                VendorDvbIpdcesgaccess2,
                VendorDvbIpdcesgpdd,
                VendorDvbIpdcroaming,
                VendorDvbIptvAlfec_Base,
                VendorDvbIptvAlfec_Enhancement,
                VendorDvbNotif_Aggregate_RootXml,
                VendorDvbNotif_ContainerXml,
                VendorDvbNotif_GenericXml,
                VendorDvbNotif_Ia_MsglistXml,
                VendorDvbNotif_Ia_Registration_RequestXml,
                VendorDvbNotif_Ia_Registration_ResponseXml,
                VendorDvbNotif_InitXml,
                VendorDvbPfr,
                VendorDvbService,
                VendorDxr,
                VendorDynageo,
                VendorDzr,
                VendorEasykaraokeCdgdownload,
                VendorEcdis_Update,
                VendorEcipRlp,
                VendorEcowinChart,
                VendorEcowinFilerequest,
                VendorEcowinFileupdate,
                VendorEcowinSeries,
                VendorEcowinSeriesrequest,
                VendorEcowinSeriesupdate,
                VendorEfiImg,
                VendorEfiIso,
                VendorEmclientAccessrequestXml,
                VendorEnliven,
                VendorEnphaseEnvoy,
                VendorEprintsDataXml,
                VendorEpsonEsf,
                VendorEpsonMsf,
                VendorEpsonQuickanime,
                VendorEpsonSalt,
                VendorEpsonSsf,
                VendorEricssonQuickcall,
                VendorEspass_EspassZip,
                VendorEszigno3Xml,
                VendorEtsiAocXml,
                VendorEtsiAsic_EZip,
                VendorEtsiAsic_SZip,
                VendorEtsiCugXml,
                VendorEtsiIptvcommandXml,
                VendorEtsiIptvdiscoveryXml,
                VendorEtsiIptvprofileXml,
                VendorEtsiIptvsad_BcXml,
                VendorEtsiIptvsad_CodXml,
                VendorEtsiIptvsad_NpvrXml,
                VendorEtsiIptvserviceXml,
                VendorEtsiIptvsyncXml,
                VendorEtsiIptvueprofileXml,
                VendorEtsiMcidXml,
                VendorEtsiMheg5,
                VendorEtsiOverload_Control_Policy_DatasetXml,
                VendorEtsiPstnXml,
                VendorEtsiSciXml,
                VendorEtsiSimservsXml,
                VendorEtsiTimestamp_Token,
                VendorEtsiTslDer,
                VendorEtsiTslXml,
                VendorEudoraData,
                VendorEvolvEcigProfile,
                VendorEvolvEcigSettings,
                VendorEvolvEcigTheme,
                VendorExstream_EmpowerZip,
                VendorExstream_Package,
                VendorEzpix_Album,
                VendorEzpix_Package,
                VendorF_SecureMobile,
                VendorFastcopy_Disk_Image,
                VendorFdf,
                VendorFdsnMseed,
                VendorFdsnSeed,
                VendorFfsns,
                VendorFiclabFlbZip,
                VendorFilmitZfc,
                VendorFints,
                VendorFiremonkeysCloudcell,
                VendorFloGraphIt,
                VendorFluxtimeClip,
                VendorFont_Fontforge_Sfd,
                VendorFramemaker,
                VendorFrogansFnc,
                VendorFrogansLtf,
                VendorFscWeblaunch,
                VendorFujitsuOasys,
                VendorFujitsuOasys2,
                VendorFujitsuOasys3,
                VendorFujitsuOasysgp,
                VendorFujitsuOasysprs,
                VendorFujixeroxART_EX,
                VendorFujixeroxART4,
                VendorFujixeroxDdd,
                VendorFujixeroxDocuworks,
                VendorFujixeroxDocuworksBinder,
                VendorFujixeroxDocuworksContainer,
                VendorFujixeroxHBPL,
                VendorFut_Misnet,
                VendorFutoinCbor,
                VendorFutoinJson,
                VendorFuzzysheet,
                VendorGenomatixTuxedo,
                VendorGenticsGrdJson,
                VendorGeogebraFile,
                VendorGeogebraTool,
                VendorGeometry_Explorer,
                VendorGeonext,
                VendorGeoplan,
                VendorGeospace,
                VendorGerber,
                VendorGlobalplatformCard_Content_Mgt,
                VendorGlobalplatformCard_Content_Mgt_Response,
                VendorGoogle_EarthKmlXml,
                VendorGoogle_EarthKmz,
                VendorGovSkE_FormXml,
                VendorGovSkE_FormZip,
                VendorGovSkXmldatacontainerXml,
                VendorGrafeq,
                VendorGridmp,
                VendorGroove_Account,
                VendorGroove_Help,
                VendorGroove_Identity_Message,
                VendorGroove_Injector,
                VendorGroove_Tool_Message,
                VendorGroove_Tool_Template,
                VendorGroove_Vcard,
                VendorHalJson,
                VendorHalXml,
                VendorHandHeld_EntertainmentXml,
                VendorHbci,
                VendorHcJson,
                VendorHcl_Bireports,
                VendorHdt,
                VendorHerokuJson,
                VendorHheLesson_Player,
                VendorHp_HPGL,
                VendorHp_Hpid,
                VendorHp_Hps,
                VendorHp_Jlyt,
                VendorHp_PCL,
                VendorHp_PCLXL,
                VendorHttphone,
                VendorHydrostatixSof_Data,
                VendorHyper_ItemJson,
                VendorHyperdriveJson,
                VendorHyperJson,
                VendorHzn_3d_Crossword,
                VendorIbmElectronic_Media,
                VendorIbmMiniPay,
                VendorIbmRights_Management,
                VendorIbmSecure_Container,
                VendorIccprofile,
                VendorIeee1905,
                VendorIgloader,
                VendorImagemeterFolderZip,
                VendorImagemeterImageZip,
                VendorImmervision_Ivp,
                VendorImmervision_Ivu,
                VendorImsImsccv1p1,
                VendorImsImsccv1p2,
                VendorImsImsccv1p3,
                VendorImsLisV2ResultJson,
                VendorImsLtiV2ToolconsumerprofileJson,
                VendorImsLtiV2ToolproxyIdJson,
                VendorImsLtiV2ToolproxyJson,
                VendorImsLtiV2ToolsettingsJson,
                VendorImsLtiV2ToolsettingsSimpleJson,
                VendorInformedcontrolRmsXml,
                VendorInfotechProject,
                VendorInfotechProjectXml,
                VendorInnopathWampNotification,
                VendorInsorsIgm,
                VendorInterconFormnet,
                VendorIntergeo,
                VendorIntertrustDigibox,
                VendorIntertrustNncp,
                VendorIntuQbo,
                VendorIntuQfx,
                VendorIptcG2CatalogitemXml,
                VendorIptcG2ConceptitemXml,
                VendorIptcG2KnowledgeitemXml,
                VendorIptcG2NewsitemXml,
                VendorIptcG2NewsmessageXml,
                VendorIptcG2PackageitemXml,
                VendorIptcG2PlanningitemXml,
                VendorIpunpluggedRcprofile,
                VendorIrepositoryPackageXml,
                VendorIs_Xpr,
                VendorIsacFcs,
                VendorIso11783_10Zip,
                VendorJam,
                VendorJapannet_Directory_Service,
                VendorJapannet_Jpnstore_Wakeup,
                VendorJapannet_Payment_Wakeup,
                VendorJapannet_Registration,
                VendorJapannet_Registration_Wakeup,
                VendorJapannet_Setstore_Wakeup,
                VendorJapannet_Verification,
                VendorJapannet_Verification_Wakeup,
                VendorJcpJavameMidlet_Rms,
                VendorJisp,
                VendorJoostJoda_Archive,
                VendorJskIsdn_Ngn,
                VendorKahootz,
                VendorKdeKarbon,
                VendorKdeKchart,
                VendorKdeKformula,
                VendorKdeKivio,
                VendorKdeKontour,
                VendorKdeKpresenter,
                VendorKdeKspread,
                VendorKdeKword,
                VendorKenameaapp,
                VendorKidspiration,
                VendorKinar,
                VendorKoan,
                VendorKodak_Descriptor,
                VendorLas,
                VendorLasLasJson,
                VendorLasLasXml,
                VendorLaszip,
                VendorLeapJson,
                VendorLiberty_RequestXml,
                VendorLlamagraphicsLife_BalanceDesktop,
                VendorLlamagraphicsLife_BalanceExchangeXml,
                VendorLogipipeCircuitZip,
                VendorLoom,
                VendorLotus_1_2_3,
                VendorLotus_Approach,
                VendorLotus_Freelance,
                VendorLotus_Notes,
                VendorLotus_Organizer,
                VendorLotus_Screencam,
                VendorLotus_Wordpro,
                VendorMacportsPortpkg,
                VendorMapbox_Vector_Tile,
                VendorMarlinDrmActiontokenXml,
                VendorMarlinDrmConftokenXml,
                VendorMarlinDrmLicenseXml,
                VendorMarlinDrmMdcf,
                VendorMasonJson,
                VendorMaxmindMaxmind_Db,
                VendorMcd,
                VendorMedcalcdata,
                VendorMediastationCdkey,
                VendorMeridian_Slingshot,
                VendorMFER,
                VendorMfmp,
                VendorMicrografxFlo,
                VendorMicrografxIgx,
                VendorMicroJson,
                VendorMicrosoftPortable_Executable,
                VendorMicrosoftWindowsThumbnail_Cache,
                VendorMieleJson,
                VendorMif,
                VendorMinisoft_Hp3000_Save,
                VendorMitsubishiMisty_GuardTrustweb,
                VendorMobiusDAF,
                VendorMobiusDIS,
                VendorMobiusMBK,
                VendorMobiusMQY,
                VendorMobiusMSL,
                VendorMobiusPLC,
                VendorMobiusTXF,
                VendorMophunApplication,
                VendorMophunCertificate,
                VendorMotorolaFlexsuite,
                VendorMotorolaFlexsuiteAdsi,
                VendorMotorolaFlexsuiteFis,
                VendorMotorolaFlexsuiteGotap,
                VendorMotorolaFlexsuiteKmr,
                VendorMotorolaFlexsuiteTtc,
                VendorMotorolaFlexsuiteWem,
                VendorMotorolaIprm,
                VendorMozillaXulXml,
                VendorMs_3mfdocument,
                VendorMs_Artgalry,
                VendorMs_Asf,
                VendorMs_Cab_Compressed,
                VendorMs_ColorIccprofile,
                VendorMs_Excel,
                VendorMs_ExcelAddinMacroEnabled12,
                VendorMs_ExcelSheetBinaryMacroEnabled12,
                VendorMs_ExcelSheetMacroEnabled12,
                VendorMs_ExcelTemplateMacroEnabled12,
                VendorMs_Fontobject,
                VendorMs_Htmlhelp,
                VendorMs_Ims,
                VendorMs_Lrm,
                VendorMs_OfficeActiveXXml,
                VendorMs_Officetheme,
                VendorMs_Opentype,
                VendorMs_PackageObfuscated_Opentype,
                VendorMs_PkiSeccat,
                VendorMs_PkiStl,
                VendorMs_PlayreadyInitiatorXml,
                VendorMs_Powerpoint,
                VendorMs_PowerpointAddinMacroEnabled12,
                VendorMs_PowerpointPresentationMacroEnabled12,
                VendorMs_PowerpointSlideMacroEnabled12,
                VendorMs_PowerpointSlideshowMacroEnabled12,
                VendorMs_PowerpointTemplateMacroEnabled12,
                VendorMs_PrintDeviceCapabilitiesXml,
                VendorMs_PrintingPrintticketXml,
                VendorMs_PrintSchemaTicketXml,
                VendorMs_Project,
                VendorMs_Tnef,
                VendorMs_WindowsDevicepairing,
                VendorMs_WindowsNwprintingOob,
                VendorMs_WindowsPrinterpairing,
                VendorMs_WindowsWsdOob,
                VendorMs_WmdrmLic_Chlg_Req,
                VendorMs_WmdrmLic_Resp,
                VendorMs_WmdrmMeter_Chlg_Req,
                VendorMs_WmdrmMeter_Resp,
                VendorMs_WordDocumentMacroEnabled12,
                VendorMs_WordTemplateMacroEnabled12,
                VendorMs_Works,
                VendorMs_Wpl,
                VendorMs_Xpsdocument,
                VendorMsa_Disk_Image,
                VendorMseq,
                VendorMsign,
                VendorMultiadCreator,
                VendorMultiadCreatorCif,
                VendorMusic_Niff,
                VendorMusician,
                VendorMuveeStyle,
                VendorMynfc,
                VendorNcdControl,
                VendorNcdReference,
                VendorNearstInvJson,
                VendorNervana,
                VendorNetfpx,
                VendorNeurolanguageNlu,
                VendorNimn,
                VendorNintendoNitroRom,
                VendorNintendoSnesRom,
                VendorNitf,
                VendorNoblenet_Directory,
                VendorNoblenet_Sealer,
                VendorNoblenet_Web,
                VendorNokiaCatalogs,
                VendorNokiaConmlWbxml,
                VendorNokiaConmlXml,
                VendorNokiaIptvConfigXml,
                VendorNokiaISDS_Radio_Presets,
                VendorNokiaLandmarkcollectionXml,
                VendorNokiaLandmarkWbxml,
                VendorNokiaLandmarkXml,
                VendorNokiaN_GageAcXml,
                VendorNokiaN_GageData,
                VendorNokiaNcd,
                VendorNokiaPcdWbxml,
                VendorNokiaPcdXml,
                VendorNokiaRadio_Preset,
                VendorNokiaRadio_Presets,
                VendorNovadigmEDM,
                VendorNovadigmEDX,
                VendorNovadigmEXT,
                VendorNtt_LocalContent_Share,
                VendorNtt_LocalFile_Transfer,
                VendorNtt_LocalOgw_remote_Access,
                VendorNtt_LocalSip_Ta_remote,
                VendorNtt_LocalSip_Ta_tcp_stream,
                VendorOasisOpendocumentChart,
                VendorOasisOpendocumentChart_Template,
                VendorOasisOpendocumentDatabase,
                VendorOasisOpendocumentFormula,
                VendorOasisOpendocumentFormula_Template,
                VendorOasisOpendocumentGraphics,
                VendorOasisOpendocumentGraphics_Template,
                VendorOasisOpendocumentImage,
                VendorOasisOpendocumentImage_Template,
                VendorOasisOpendocumentPresentation,
                VendorOasisOpendocumentPresentation_Template,
                VendorOasisOpendocumentSpreadsheet,
                VendorOasisOpendocumentSpreadsheet_Template,
                VendorOasisOpendocumentText,
                VendorOasisOpendocumentText_Master,
                VendorOasisOpendocumentText_Template,
                VendorOasisOpendocumentText_Web,
                VendorObn,
                VendorOcfCbor,
                VendorOftnL10nJson,
                VendorOipfContentaccessdownloadXml,
                VendorOipfContentaccessstreamingXml,
                VendorOipfCspg_Hexbinary,
                VendorOipfDaeSvgXml,
                VendorOipfDaeXhtmlXml,
                VendorOipfMippvcontrolmessageXml,
                VendorOipfPaeGem,
                VendorOipfSpdiscoveryXml,
                VendorOipfSpdlistXml,
                VendorOipfUeprofileXml,
                VendorOipfUserprofileXml,
                VendorOlpc_Sugar,
                VendorOma_Scws_Config,
                VendorOma_Scws_Http_Request,
                VendorOma_Scws_Http_Response,
                VendorOmaBcastAssociated_Procedure_ParameterXml,
                VendorOmaBcastDrm_TriggerXml,
                VendorOmaBcastImdXml,
                VendorOmaBcastLtkm,
                VendorOmaBcastNotificationXml,
                VendorOmaBcastProvisioningtrigger,
                VendorOmaBcastSgboot,
                VendorOmaBcastSgddXml,
                VendorOmaBcastSgdu,
                VendorOmaBcastSimple_Symbol_Container,
                VendorOmaBcastSmartcard_TriggerXml,
                VendorOmaBcastSprovXml,
                VendorOmaBcastStkm,
                VendorOmaCab_Address_BookXml,
                VendorOmaCab_Feature_HandlerXml,
                VendorOmaCab_PccXml,
                VendorOmaCab_Subs_InviteXml,
                VendorOmaCab_User_PrefsXml,
                VendorOmaDcd,
                VendorOmaDcdc,
                VendorOmaDd2Xml,
                VendorOmaDrmRisdXml,
                VendorOmads_EmailXml,
                VendorOmads_FileXml,
                VendorOmads_FolderXml,
                VendorOmaGroup_Usage_ListXml,
                VendorOmaloc_Supl_Init,
                VendorOmaLwm2mJson,
                VendorOmaLwm2mTlv,
                VendorOmaPalXml,
                VendorOmaPocDetailed_Progress_ReportXml,
                VendorOmaPocFinal_ReportXml,
                VendorOmaPocGroupsXml,
                VendorOmaPocInvocation_DescriptorXml,
                VendorOmaPocOptimized_Progress_ReportXml,
                VendorOmaPush,
                VendorOmaScidmMessagesXml,
                VendorOmaXcap_DirectoryXml,
                VendorOnepager,
                VendorOnepagertamp,
                VendorOnepagertamx,
                VendorOnepagertat,
                VendorOnepagertatp,
                VendorOnepagertatx,
                VendorOpenbloxGame_Binary,
                VendorOpenbloxGameXml,
                VendorOpeneyeOeb,
                VendorOpenofficeorgExtension,
                VendorOpenstreetmapDataXml,
                VendorOpenxmlformats_OfficedocumentCustom_PropertiesXml,
                VendorOpenxmlformats_OfficedocumentCustomXmlPropertiesXml,
                VendorOpenxmlformats_OfficedocumentDrawingmlChartshapesXml,
                VendorOpenxmlformats_OfficedocumentDrawingmlChartXml,
                VendorOpenxmlformats_OfficedocumentDrawingmlDiagramColorsXml,
                VendorOpenxmlformats_OfficedocumentDrawingmlDiagramDataXml,
                VendorOpenxmlformats_OfficedocumentDrawingmlDiagramLayoutXml,
                VendorOpenxmlformats_OfficedocumentDrawingmlDiagramStyleXml,
                VendorOpenxmlformats_OfficedocumentDrawingXml,
                VendorOpenxmlformats_OfficedocumentExtended_PropertiesXml,
                VendorOpenxmlformats_OfficedocumentPresentationmlCommentAuthorsXml,
                VendorOpenxmlformats_OfficedocumentPresentationmlCommentsXml,
                VendorOpenxmlformats_OfficedocumentPresentationmlHandoutMasterXml,
                VendorOpenxmlformats_OfficedocumentPresentationmlNotesMasterXml,
                VendorOpenxmlformats_OfficedocumentPresentationmlNotesSlideXml,
                VendorOpenxmlformats_OfficedocumentPresentationmlPresentation,
                VendorOpenxmlformats_OfficedocumentPresentationmlPresentationMainXml,
                VendorOpenxmlformats_OfficedocumentPresentationmlPresPropsXml,
                VendorOpenxmlformats_OfficedocumentPresentationmlSlide,
                VendorOpenxmlformats_OfficedocumentPresentationmlSlideLayoutXml,
                VendorOpenxmlformats_OfficedocumentPresentationmlSlideMasterXml,
                VendorOpenxmlformats_OfficedocumentPresentationmlSlideshow,
                VendorOpenxmlformats_OfficedocumentPresentationmlSlideshowMainXml,
                VendorOpenxmlformats_OfficedocumentPresentationmlSlideUpdateInfoXml,
                VendorOpenxmlformats_OfficedocumentPresentationmlSlideXml,
                VendorOpenxmlformats_OfficedocumentPresentationmlTableStylesXml,
                VendorOpenxmlformats_OfficedocumentPresentationmlTagsXml,
                VendorOpenxmlformats_OfficedocumentPresentationmlTemplate,
                VendorOpenxmlformats_OfficedocumentPresentationmlTemplateMainXml,
                VendorOpenxmlformats_OfficedocumentPresentationmlViewPropsXml,
                VendorOpenxmlformats_OfficedocumentSpreadsheetmlCalcChainXml,
                VendorOpenxmlformats_OfficedocumentSpreadsheetmlChartsheetXml,
                VendorOpenxmlformats_OfficedocumentSpreadsheetmlCommentsXml,
                VendorOpenxmlformats_OfficedocumentSpreadsheetmlConnectionsXml,
                VendorOpenxmlformats_OfficedocumentSpreadsheetmlDialogsheetXml,
                VendorOpenxmlformats_OfficedocumentSpreadsheetmlExternalLinkXml,
                VendorOpenxmlformats_OfficedocumentSpreadsheetmlPivotCacheDefinitionXml,
                VendorOpenxmlformats_OfficedocumentSpreadsheetmlPivotCacheRecordsXml,
                VendorOpenxmlformats_OfficedocumentSpreadsheetmlPivotTableXml,
                VendorOpenxmlformats_OfficedocumentSpreadsheetmlQueryTableXml,
                VendorOpenxmlformats_OfficedocumentSpreadsheetmlRevisionHeadersXml,
                VendorOpenxmlformats_OfficedocumentSpreadsheetmlRevisionLogXml,
                VendorOpenxmlformats_OfficedocumentSpreadsheetmlSharedStringsXml,
                VendorOpenxmlformats_OfficedocumentSpreadsheetmlSheet,
                VendorOpenxmlformats_OfficedocumentSpreadsheetmlSheetMainXml,
                VendorOpenxmlformats_OfficedocumentSpreadsheetmlSheetMetadataXml,
                VendorOpenxmlformats_OfficedocumentSpreadsheetmlStylesXml,
                VendorOpenxmlformats_OfficedocumentSpreadsheetmlTableSingleCellsXml,
                VendorOpenxmlformats_OfficedocumentSpreadsheetmlTableXml,
                VendorOpenxmlformats_OfficedocumentSpreadsheetmlTemplate,
                VendorOpenxmlformats_OfficedocumentSpreadsheetmlTemplateMainXml,
                VendorOpenxmlformats_OfficedocumentSpreadsheetmlUserNamesXml,
                VendorOpenxmlformats_OfficedocumentSpreadsheetmlVolatileDependenciesXml,
                VendorOpenxmlformats_OfficedocumentSpreadsheetmlWorksheetXml,
                VendorOpenxmlformats_OfficedocumentThemeOverrideXml,
                VendorOpenxmlformats_OfficedocumentThemeXml,
                VendorOpenxmlformats_OfficedocumentVmlDrawing,
                VendorOpenxmlformats_OfficedocumentWordprocessingmlCommentsXml,
                VendorOpenxmlformats_OfficedocumentWordprocessingmlDocument,
                VendorOpenxmlformats_OfficedocumentWordprocessingmlDocumentGlossaryXml,
                VendorOpenxmlformats_OfficedocumentWordprocessingmlDocumentMainXml,
                VendorOpenxmlformats_OfficedocumentWordprocessingmlEndnotesXml,
                VendorOpenxmlformats_OfficedocumentWordprocessingmlFontTableXml,
                VendorOpenxmlformats_OfficedocumentWordprocessingmlFooterXml,
                VendorOpenxmlformats_OfficedocumentWordprocessingmlFootnotesXml,
                VendorOpenxmlformats_OfficedocumentWordprocessingmlNumberingXml,
                VendorOpenxmlformats_OfficedocumentWordprocessingmlSettingsXml,
                VendorOpenxmlformats_OfficedocumentWordprocessingmlStylesXml,
                VendorOpenxmlformats_OfficedocumentWordprocessingmlTemplate,
                VendorOpenxmlformats_OfficedocumentWordprocessingmlTemplateMainXml,
                VendorOpenxmlformats_OfficedocumentWordprocessingmlWebSettingsXml,
                VendorOpenxmlformats_PackageCore_PropertiesXml,
                VendorOpenxmlformats_PackageDigital_Signature_XmlsignatureXml,
                VendorOpenxmlformats_PackageRelationshipsXml,
                VendorOracleResourceJson,
                VendorOrangeIndata,
                VendorOsaNetdeploy,
                VendorOsgeoMapguidePackage,
                VendorOsgiBundle,
                VendorOsgiDp,
                VendorOsgiSubsystem,
                VendorOtpsCt_KipXml,
                VendorOxliCountgraph,
                VendorPagerdutyJson,
                VendorPalm,
                VendorPanoply,
                VendorPaosXml,
                VendorPatentdive,
                VendorPatientecommsdoc,
                VendorPawaafile,
                VendorPcos,
                VendorPgFormat,
                VendorPgOsasli,
                VendorPiaccessApplication_Licence,
                VendorPicsel,
                VendorPmiWidget,
                VendorPocGroup_AdvertisementXml,
                VendorPocketlearn,
                VendorPowerbuilder6,
                VendorPowerbuilder6_S,
                VendorPowerbuilder7,
                VendorPowerbuilder7_S,
                VendorPowerbuilder75,
                VendorPowerbuilder75_S,
                VendorPreminet,
                VendorPreviewsystemsBox,
                VendorProteusMagazine,
                VendorPsfs,
                VendorPublishare_Delta_Tree,
                VendorPviPtid1,
                VendorPwg_Multiplexed,
                VendorPwg_Xhtml_PrintXml,
                VendorQualcommBrew_App_Res,
                VendorQuarantainenet,
                VendorQuarkQuarkXPress,
                VendorQuobject_Quoxdocument,
                VendorRadisysMomlXml,
                VendorRadisysMsml_Audit_ConfXml,
                VendorRadisysMsml_Audit_ConnXml,
                VendorRadisysMsml_Audit_DialogXml,
                VendorRadisysMsml_Audit_StreamXml,
                VendorRadisysMsml_AuditXml,
                VendorRadisysMsml_ConfXml,
                VendorRadisysMsml_Dialog_BaseXml,
                VendorRadisysMsml_Dialog_Fax_DetectXml,
                VendorRadisysMsml_Dialog_Fax_SendrecvXml,
                VendorRadisysMsml_Dialog_GroupXml,
                VendorRadisysMsml_Dialog_SpeechXml,
                VendorRadisysMsml_Dialog_TransformXml,
                VendorRadisysMsml_DialogXml,
                VendorRadisysMsmlXml,
                VendorRainstorData,
                VendorRapid,
                VendorRar,
                VendorRealvncBed,
                VendorRecordareMusicxml,
                VendorRecordareMusicxmlXml,
                VendorRenLearnRlprint,
                VendorRestfulJson,
                VendorRigCryptonote,
                VendorRimCod,
                VendorRn_Realmedia,
                VendorRn_Realmedia_Vbr,
                VendorRoute66Link66Xml,
                VendorRs_274x,
                VendorRuckusDownload,
                VendorS3sms,
                VendorSailingtrackerTrack,
                VendorSar,
                VendorSbmCid,
                VendorSbmMid2,
                VendorScribus,
                VendorSealed3df,
                VendorSealedCsf,
                VendorSealedDoc,
                VendorSealedEml,
                VendorSealedmediaSoftsealHtml,
                VendorSealedmediaSoftsealPdf,
                VendorSealedMht,
                VendorSealedNet,
                VendorSealedPpt,
                VendorSealedTiff,
                VendorSealedXls,
                VendorSeemail,
                VendorSema,
                VendorSemd,
                VendorSemf,
                VendorShade_Save_File,
                VendorShanaInformedFormdata,
                VendorShanaInformedFormtemplate,
                VendorShanaInformedInterchange,
                VendorShanaInformedPackage,
                VendorShootproofJson,
                VendorShopkickJson,
                VendorSigrokSession,
                VendorSimTech_MindMapper,
                VendorSirenJson,
                VendorSmaf,
                VendorSmartNotebook,
                VendorSmartTeacher,
                VendorSoftware602FillerForm_Xml_Zip,
                VendorSoftware602FillerFormXml,
                VendorSolentSdkmXml,
                VendorSpotfireDxp,
                VendorSpotfireSfs,
                VendorSqlite3,
                VendorSss_Cod,
                VendorSss_Dtf,
                VendorSss_Ntf,
                VendorStardivisionCalc,
                VendorStardivisionDraw,
                VendorStardivisionImpress,
                VendorStardivisionMath,
                VendorStardivisionWriter,
                VendorStardivisionWriter_Global,
                VendorStepmaniaPackage,
                VendorStepmaniaStepchart,
                VendorStreet_Stream,
                VendorSunWadlXml,
                VendorSunXmlCalc,
                VendorSunXmlCalcTemplate,
                VendorSunXmlDraw,
                VendorSunXmlDrawTemplate,
                VendorSunXmlImpress,
                VendorSunXmlImpressTemplate,
                VendorSunXmlMath,
                VendorSunXmlWriter,
                VendorSunXmlWriterGlobal,
                VendorSunXmlWriterTemplate,
                VendorSus_Calendar,
                VendorSvd,
                VendorSwiftview_Ics,
                VendorSymbianInstall,
                VendorSyncmlDmddfWbxml,
                VendorSyncmlDmddfXml,
                VendorSyncmlDmNotification,
                VendorSyncmlDmtndsWbxml,
                VendorSyncmlDmtndsXml,
                VendorSyncmlDmWbxml,
                VendorSyncmlDmXml,
                VendorSyncmlDsNotification,
                VendorSyncmlXml,
                VendorTableschemaJson,
                VendorTaoIntent_Module_Archive,
                VendorTcpdumpPcap,
                VendorThink_CellPpttcJson,
                VendorTmdMediaflexApiXml,
                VendorTml,
                VendorTmobile_Livetv,
                VendorTridTpt,
                VendorTriOnesource,
                VendorTriscapeMxs,
                VendorTrueapp,
                VendorTruedoc,
                VendorUbisoftWebplayer,
                VendorUfdl,
                VendorUiqTheme,
                VendorUmajin,
                VendorUnity,
                VendorUomlXml,
                VendorUplanetAlert,
                VendorUplanetAlert_Wbxml,
                VendorUplanetBearer_Choice,
                VendorUplanetBearer_Choice_Wbxml,
                VendorUplanetCacheop,
                VendorUplanetCacheop_Wbxml,
                VendorUplanetChannel,
                VendorUplanetChannel_Wbxml,
                VendorUplanetList,
                VendorUplanetList_Wbxml,
                VendorUplanetListcmd,
                VendorUplanetListcmd_Wbxml,
                VendorUplanetSignal,
                VendorUri_Map,
                VendorValveSourceMaterial,
                VendorVcx,
                VendorVd_Study,
                VendorVectorworks,
                VendorVelJson,
                VendorVerimatrixVcas,
                VendorVeryantThin,
                VendorVesEncrypted,
                VendorVidsoftVidconference,
                VendorVisio,
                VendorVisionary,
                VendorVividenceScriptfile,
                VendorVsf,
                VendorWapSic,
                VendorWapSlc,
                VendorWapWbxml,
                VendorWapWmlc,
                VendorWapWmlscriptc,
                VendorWebturbo,
                VendorWfaP2p,
                VendorWfaWsc,
                VendorWindowsDevicepairing,
                VendorWmc,
                VendorWmfBootstrap,
                VendorWolframMathematica,
                VendorWolframMathematicaPackage,
                VendorWolframPlayer,
                VendorWordperfect,
                VendorWqd,
                VendorWrq_Hp3000_Labelled,
                VendorWtStf,
                VendorWvCspWbxml,
                VendorWvCspXml,
                VendorWvSspXml,
                VendorXacmlJson,
                VendorXara,
                VendorXfdl,
                VendorXfdlWebform,
                VendorXmiXml,
                VendorXmpieCpkg,
                VendorXmpieDpkg,
                VendorXmpiePlan,
                VendorXmpiePpkg,
                VendorXmpieXlim,
                VendorYamahaHv_Dic,
                VendorYamahaHv_Script,
                VendorYamahaHv_Voice,
                VendorYamahaOpenscoreformat,
                VendorYamahaOpenscoreformatOsfpvgXml,
                VendorYamahaRemote_Setup,
                VendorYamahaSmaf_Audio,
                VendorYamahaSmaf_Phrase,
                VendorYamahaThrough_Ngn,
                VendorYamahaTunnel_Udpencap,
                VendorYaoweme,
                VendorYellowriver_Custom_Menu,
                VendorZul,
                VendorZzazzDeckXml,
                VividenceScriptfile,
                VoicexmlXml,
                Voucher_CmsJson,
                Vq_Rtcpxr,
                WatcherinfoXml,
                Webpush_OptionsJson,
                Whoispp_Query,
                Whoispp_Response,
                Widget,
                Winhlp,
                Wita,
                Wordperfect51,
                WsdlXml,
                WspolicyXml,
                X_7z_Compressed,
                X_Abiword,
                X_Ace_Compressed,
                X_Amf,
                X_Apple_Diskimage,
                X_Authorware_Bin,
                X_Authorware_Map,
                X_Authorware_Seg,
                X_Bcpio,
                X_Bittorrent,
                X_Blorb,
                X_Bzip,
                X_Bzip2,
                X_Cbr,
                X_Cdlink,
                X_Cfs_Compressed,
                X_Chat,
                X_Chess_Pgn,
                X_Compress,
                X_Conference,
                X_Cpio,
                X_Csh,
                X_Debian_Package,
                X_Dgc_Compressed,
                X_Director,
                X_Doom,
                X_DtbncxXml,
                X_DtbookXml,
                X_DtbresourceXml,
                X_Dvi,
                X_Envoy,
                X_Eva,
                X_Font_Bdf,
                X_Font_Dos,
                X_Font_Framemaker,
                X_Font_Ghostscript,
                X_Font_Libgrx,
                X_Font_Linux_Psf,
                X_Font_Pcf,
                X_Font_Snf,
                X_Font_Speedo,
                X_Font_Sunos_News,
                X_Font_Type1,
                X_Font_Vfont,
                X_Freearc,
                X_Futuresplash,
                X_Gca_Compressed,
                X_Glulx,
                X_Gnumeric,
                X_Gramps_Xml,
                X_Gtar,
                X_Gzip,
                X_Hdf,
                X_Install_Instructions,
                X_Iso9660_Image,
                X_Java_Jnlp_File,
                X_Latex,
                X_Lzh_Compressed,
                X_Mie,
                X_Mobipocket_Ebook,
                X_Ms_Application,
                X_Ms_Shortcut,
                X_Ms_Wmd,
                X_Ms_Wmz,
                X_Ms_Xbap,
                X_Msaccess,
                X_Msbinder,
                X_Mscardfile,
                X_Msclip,
                X_Msdownload,
                X_Msmediaview,
                X_Msmetafile,
                X_Msmoney,
                X_Mspublisher,
                X_Msschedule,
                X_Msterminal,
                X_Mswrite,
                X_Netcdf,
                X_Nzb,
                X_Pkcs12,
                X_Pkcs7_Certificates,
                X_Pkcs7_Certreqresp,
                X_Rar_Compressed,
                X_Research_Info_Systems,
                X_Sh,
                X_Shar,
                X_Shockwave_Flash,
                X_Silverlight_App,
                X_Sql,
                X_Stuffit,
                X_Stuffitx,
                X_Subrip,
                X_Sv4cpio,
                X_Sv4crc,
                X_T3vm_Image,
                X_Tads,
                X_Tar,
                X_Tcl,
                X_Tex,
                X_Tex_Tfm,
                X_Texinfo,
                X_Tgif,
                X_Ustar,
                X_Wais_Source,
                X_Www_Form_Urlencoded,
                X_X509_Ca_Cert,
                X_Xfig,
                X_XliffXml,
                X_Xpinstall,
                X_Xz,
                X_Zmachine,
                X400_Bp,
                XacmlXml,
                XamlXml,
                Xcap_AttXml,
                Xcap_CapsXml,
                Xcap_DiffXml,
                Xcap_ElXml,
                Xcap_ErrorXml,
                Xcap_NsXml,
                Xcon_Conference_Info_DiffXml,
                Xcon_Conference_InfoXml,
                XencXml,
                Xhtml_VoiceXml,
                XhtmlXml,
                XliffXml,
                Xml,
                Xml_Dtd,
                Xml_External_Parsed_Entity,
                Xml_PatchXml,
                XmppXml,
                XopXml,
                XprocXml,
                XsltXml,
                XspfXml,
                XvXml,
                Yang,
                Yang_DataJson,
                Yang_DataXml,
                Yang_PatchJson,
                Yang_PatchXml,
                YinXml,
                Zip,
                Zlib,
                Zstd
            };
        }
    }
}
