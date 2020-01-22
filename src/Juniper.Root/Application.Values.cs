namespace Juniper
{
    public partial class MediaType
    {
        public sealed partial class Application : MediaType
        {
            public static readonly Application A2L = new Application("a2l");
            public static readonly Application Activemessage = new Application("activemessage");
            public static readonly Application ActivityJson = new Application("activity+json", new string[] { "json" });
            public static readonly Application Alto_CostmapfilterJson = new Application("alto-costmapfilter+json", new string[] { "json" });
            public static readonly Application Alto_CostmapJson = new Application("alto-costmap+json", new string[] { "json" });
            public static readonly Application Alto_DirectoryJson = new Application("alto-directory+json", new string[] { "json" });
            public static readonly Application Alto_EndpointcostJson = new Application("alto-endpointcost+json", new string[] { "json" });
            public static readonly Application Alto_EndpointcostparamsJson = new Application("alto-endpointcostparams+json", new string[] { "json" });
            public static readonly Application Alto_EndpointpropJson = new Application("alto-endpointprop+json", new string[] { "json" });
            public static readonly Application Alto_EndpointpropparamsJson = new Application("alto-endpointpropparams+json", new string[] { "json" });
            public static readonly Application Alto_ErrorJson = new Application("alto-error+json", new string[] { "json" });
            public static readonly Application Alto_NetworkmapfilterJson = new Application("alto-networkmapfilter+json", new string[] { "json" });
            public static readonly Application Alto_NetworkmapJson = new Application("alto-networkmap+json", new string[] { "json" });
            public static readonly Application AML = new Application("aml");
            public static readonly Application Andrew_Inset = new Application("andrew-inset", new string[] { "ez" });
            public static readonly Application Applefile = new Application("applefile");
            public static readonly Application Applixware = new Application("applixware", new string[] { "aw" });
            public static readonly Application ATF = new Application("atf");
            public static readonly Application ATFX = new Application("atfx");
            public static readonly Application AtomcatXml = new Application("atomcat+xml", new string[] { "atomcat" });
            public static readonly Application AtomdeletedXml = new Application("atomdeleted+xml", new string[] { "xml" });
            public static readonly Application Atomicmail = new Application("atomicmail");
            public static readonly Application AtomsvcXml = new Application("atomsvc+xml", new string[] { "atomsvc" });
            public static readonly Application AtomXml = new Application("atom+xml", new string[] { "atom" });
            public static readonly Application Atsc_DwdXml = new Application("atsc-dwd+xml", new string[] { "xml" });
            public static readonly Application Atsc_HeldXml = new Application("atsc-held+xml", new string[] { "xml" });
            public static readonly Application Atsc_RsatXml = new Application("atsc-rsat+xml", new string[] { "xml" });
            public static readonly Application ATXML = new Application("atxml");
            public static readonly Application Auth_PolicyXml = new Application("auth-policy+xml", new string[] { "xml" });
            public static readonly Application Bacnet_XddZip = new Application("bacnet-xdd+zip", new string[] { "zip" });
            public static readonly Application Batch_SMTP = new Application("batch-smtp");
            public static readonly Application BeepXml = new Application("beep+xml", new string[] { "xml" });
            public static readonly Application CalendarJson = new Application("calendar+json", new string[] { "json" });
            public static readonly Application CalendarXml = new Application("calendar+xml", new string[] { "xml" });
            public static readonly Application Call_Completion = new Application("call-completion");
            public static readonly Application CALS_1840 = new Application("cals-1840");
            public static readonly Application Cbor = new Application("cbor");
            public static readonly Application Cbor_Seq = new Application("cbor-seq");
            public static readonly Application Cccex = new Application("cccex");
            public static readonly Application CcmpXml = new Application("ccmp+xml", new string[] { "xml" });
            public static readonly Application CcxmlXml = new Application("ccxml+xml", new string[] { "ccxml" });
            public static readonly Application CDFXXML = new Application("cdfx+xml", new string[] { "xml" });
            public static readonly Application Cdmi_Capability = new Application("cdmi-capability", new string[] { "cdmia" });
            public static readonly Application Cdmi_Container = new Application("cdmi-container", new string[] { "cdmic" });
            public static readonly Application Cdmi_Domain = new Application("cdmi-domain", new string[] { "cdmid" });
            public static readonly Application Cdmi_Object = new Application("cdmi-object", new string[] { "cdmio" });
            public static readonly Application Cdmi_Queue = new Application("cdmi-queue", new string[] { "cdmiq" });
            public static readonly Application Cdni = new Application("cdni");
            public static readonly Application CEA = new Application("cea");
            public static readonly Application Cea_2018Xml = new Application("cea-2018+xml", new string[] { "xml" });
            public static readonly Application CellmlXml = new Application("cellml+xml", new string[] { "xml" });
            public static readonly Application Cfw = new Application("cfw");
            public static readonly Application Clue_infoXml = new Application("clue_info+xml", new string[] { "xml" });
            public static readonly Application Cms = new Application("cms");
            public static readonly Application CnrpXml = new Application("cnrp+xml", new string[] { "xml" });
            public static readonly Application Coap_GroupJson = new Application("coap-group+json", new string[] { "json" });
            public static readonly Application Coap_Payload = new Application("coap-payload");
            public static readonly Application Commonground = new Application("commonground");
            public static readonly Application Conference_InfoXml = new Application("conference-info+xml", new string[] { "xml" });
            public static readonly Application Cose = new Application("cose");
            public static readonly Application Cose_Key = new Application("cose-key");
            public static readonly Application Cose_Key_Set = new Application("cose-key-set");
            public static readonly Application CplXml = new Application("cpl+xml", new string[] { "xml" });
            public static readonly Application Csrattrs = new Application("csrattrs");
            public static readonly Application CSTAdataXml = new Application("cstadata+xml", new string[] { "xml" });
            public static readonly Application CstaXml = new Application("csta+xml", new string[] { "xml" });
            public static readonly Application CsvmJson = new Application("csvm+json", new string[] { "json" });
            public static readonly Application Cu_Seeme = new Application("cu-seeme", new string[] { "cu" });
            public static readonly Application Cwt = new Application("cwt");
            public static readonly Application Cybercash = new Application("cybercash");
            public static readonly Application Dashdelta = new Application("dashdelta");
            public static readonly Application DashXml = new Application("dash+xml", new string[] { "xml" });
            public static readonly Application DavmountXml = new Application("davmount+xml", new string[] { "davmount" });
            public static readonly Application Dca_Rft = new Application("dca-rft");
            public static readonly Application DCD = new Application("dcd");
            public static readonly Application Dec_Dx = new Application("dec-dx");
            public static readonly Application Dialog_InfoXml = new Application("dialog-info+xml", new string[] { "xml" });
            public static readonly Application Dicom = new Application("dicom");
            public static readonly Application DicomJson = new Application("dicom+json", new string[] { "json" });
            public static readonly Application DicomXml = new Application("dicom+xml", new string[] { "xml" });
            public static readonly Application DII = new Application("dii");
            public static readonly Application DIT = new Application("dit");
            public static readonly Application Dns = new Application("dns");
            public static readonly Application Dns_Message = new Application("dns-message");
            public static readonly Application DnsJson = new Application("dns+json", new string[] { "json" });
            public static readonly Application DocbookXml = new Application("docbook+xml", new string[] { "dbk" });
            public static readonly Application DskppXml = new Application("dskpp+xml", new string[] { "xml" });
            public static readonly Application DsscDer = new Application("dssc+der", new string[] { "dssc" });
            public static readonly Application DsscXml = new Application("dssc+xml", new string[] { "xdssc" });
            public static readonly Application Dvcs = new Application("dvcs");
            public static readonly Application Ecmascript = new Application("ecmascript", new string[] { "ecma" });
            public static readonly Application EDI_Consent = new Application("edi-consent");
            public static readonly Application EDI_X12 = new Application("edi-x12");
            public static readonly Application EDIFACT = new Application("edifact");
            public static readonly Application Efi = new Application("efi");
            public static readonly Application EmergencyCallDataCommentXml = new Application("emergencycalldata.comment+xml", new string[] { "xml" });
            public static readonly Application EmergencyCallDataControlXml = new Application("emergencycalldata.control+xml", new string[] { "xml" });
            public static readonly Application EmergencyCallDataDeviceInfoXml = new Application("emergencycalldata.deviceinfo+xml", new string[] { "xml" });
            public static readonly Application EmergencyCallDataECallMSD = new Application("emergencycalldata.ecall.msd");
            public static readonly Application EmergencyCallDataProviderInfoXml = new Application("emergencycalldata.providerinfo+xml", new string[] { "xml" });
            public static readonly Application EmergencyCallDataServiceInfoXml = new Application("emergencycalldata.serviceinfo+xml", new string[] { "xml" });
            public static readonly Application EmergencyCallDataSubscriberInfoXml = new Application("emergencycalldata.subscriberinfo+xml", new string[] { "xml" });
            public static readonly Application EmergencyCallDataVEDSXml = new Application("emergencycalldata.veds+xml", new string[] { "xml" });
            public static readonly Application EmmaXml = new Application("emma+xml", new string[] { "emma" });
            public static readonly Application EmotionmlXml = new Application("emotionml+xml", new string[] { "xml" });
            public static readonly Application Encaprtp = new Application("encaprtp");
            public static readonly Application EppXml = new Application("epp+xml", new string[] { "xml" });
            public static readonly Application EpubZip = new Application("epub+zip", new string[] { "epub" });
            public static readonly Application Eshop = new Application("eshop");
            public static readonly Application Example = new Application("example");
            public static readonly Application Exi = new Application("exi", new string[] { "exi" });
            public static readonly Application Expect_Ct_ReportJson = new Application("expect-ct-report+json", new string[] { "json" });
            public static readonly Application Fastinfoset = new Application("fastinfoset");
            public static readonly Application Fastsoap = new Application("fastsoap");
            public static readonly Application FdtXml = new Application("fdt+xml", new string[] { "xml" });
            public static readonly Application FhirJson = new Application("fhir+json", new string[] { "json" });
            public static readonly Application FhirXml = new Application("fhir+xml", new string[] { "xml" });
            public static readonly Application Fits = new Application("fits");
            public static readonly Application Flexfec = new Application("flexfec");

            [System.Obsolete("DEPRECATED in favor of font/sfnt")]
            public static readonly Application Font_Sfnt = new Application("font-sfnt");

            public static readonly Application Font_Tdpfr = new Application("font-tdpfr", new string[] { "pfr" });

            [System.Obsolete("DEPRECATED in favor of font/woff")]
            public static readonly Application Font_Woff = new Application("font-woff");

            public static readonly Application Framework_AttributesXml = new Application("framework-attributes+xml", new string[] { "xml" });
            public static readonly Application GeoJson = new Application("geo+json", new string[] { "json" });
            public static readonly Application GeoJson_Seq = new Application("geo+json-seq", new string[] { "json-seq" });
            public static readonly Application GeopackageSqlite3 = new Application("geopackage+sqlite3", new string[] { "sqlite3" });
            public static readonly Application GeoxacmlXml = new Application("geoxacml+xml", new string[] { "xml" });
            public static readonly Application Gltf_Buffer = new Application("gltf-buffer");
            public static readonly Application GmlXml = new Application("gml+xml", new string[] { "gml" });
            public static readonly Application GpxXml = new Application("gpx+xml", new string[] { "gpx" });
            public static readonly Application Gxf = new Application("gxf", new string[] { "gxf" });
            public static readonly Application Gzip = new Application("gzip");
            public static readonly Application H224 = new Application("h224");
            public static readonly Application HeldXml = new Application("held+xml", new string[] { "xml" });
            public static readonly Application Http = new Application("http");
            public static readonly Application Hyperstudio = new Application("hyperstudio", new string[] { "stk" });
            public static readonly Application Ibe_Key_RequestXml = new Application("ibe-key-request+xml", new string[] { "xml" });
            public static readonly Application Ibe_Pkg_ReplyXml = new Application("ibe-pkg-reply+xml", new string[] { "xml" });
            public static readonly Application Ibe_Pp_Data = new Application("ibe-pp-data");
            public static readonly Application Iges = new Application("iges");
            public static readonly Application Im_IscomposingXml = new Application("im-iscomposing+xml", new string[] { "xml" });
            public static readonly Application Index = new Application("index");
            public static readonly Application IndexCmd = new Application("index.cmd");
            public static readonly Application IndexObj = new Application("index.obj");
            public static readonly Application IndexResponse = new Application("index.response");
            public static readonly Application IndexVnd = new Application("index.vnd");
            public static readonly Application InkmlXml = new Application("inkml+xml", new string[] { "ink", "inkml" });
            public static readonly Application IOTP = new Application("iotp");
            public static readonly Application Ipfix = new Application("ipfix", new string[] { "ipfix" });
            public static readonly Application Ipp = new Application("ipp");
            public static readonly Application Isup = new Application("isup");
            public static readonly Application ItsXml = new Application("its+xml", new string[] { "xml" });
            public static readonly Application Java_Archive = new Application("java-archive", new string[] { "jar" });
            public static readonly Application Java_Serialized_Object = new Application("java-serialized-object", new string[] { "ser" });
            public static readonly Application Java_Vm = new Application("java-vm", new string[] { "class" });
            public static readonly Application Javascript = new Application("javascript", new string[] { "js" });
            public static readonly Application Jf2feedJson = new Application("jf2feed+json", new string[] { "json" });
            public static readonly Application Jose = new Application("jose");
            public static readonly Application JoseJson = new Application("jose+json", new string[] { "json" });
            public static readonly Application JrdJson = new Application("jrd+json", new string[] { "json" });
            public static readonly Application Json = new Application("json", new string[] { "json" });
            public static readonly Application Json_PatchJson = new Application("json-patch+json", new string[] { "json" });
            public static readonly Application Json_Seq = new Application("json-seq");
            public static readonly Application JsonmlJson = new Application("jsonml+json", new string[] { "jsonml" });
            public static readonly Application Jwk_SetJson = new Application("jwk-set+json", new string[] { "json" });
            public static readonly Application JwkJson = new Application("jwk+json", new string[] { "json" });
            public static readonly Application Jwt = new Application("jwt");
            public static readonly Application Kpml_RequestXml = new Application("kpml-request+xml", new string[] { "xml" });
            public static readonly Application Kpml_ResponseXml = new Application("kpml-response+xml", new string[] { "xml" });
            public static readonly Application LdJson = new Application("ld+json", new string[] { "json" });
            public static readonly Application LgrXml = new Application("lgr+xml", new string[] { "xml" });
            public static readonly Application Link_Format = new Application("link-format");
            public static readonly Application Load_ControlXml = new Application("load-control+xml", new string[] { "xml" });
            public static readonly Application LostsyncXml = new Application("lostsync+xml", new string[] { "xml" });
            public static readonly Application LostXml = new Application("lost+xml", new string[] { "lostxml" });
            public static readonly Application LXF = new Application("lxf");
            public static readonly Application Mac_Binhex40 = new Application("mac-binhex40", new string[] { "hqx" });
            public static readonly Application Mac_Compactpro = new Application("mac-compactpro", new string[] { "cpt" });
            public static readonly Application Macwriteii = new Application("macwriteii");
            public static readonly Application MadsXml = new Application("mads+xml", new string[] { "mads" });
            public static readonly Application Marc = new Application("marc", new string[] { "mrc" });
            public static readonly Application MarcxmlXml = new Application("marcxml+xml", new string[] { "mrcx" });
            public static readonly Application Mathematica = new Application("mathematica", new string[] { "ma", "nb", "mb" });
            public static readonly Application Mathml_ContentXml = new Application("mathml-content+xml", new string[] { "xml" });
            public static readonly Application Mathml_PresentationXml = new Application("mathml-presentation+xml", new string[] { "xml" });
            public static readonly Application MathmlXml = new Application("mathml+xml", new string[] { "mathml" });
            public static readonly Application Mbms_Associated_Procedure_DescriptionXml = new Application("mbms-associated-procedure-description+xml", new string[] { "xml" });
            public static readonly Application Mbms_DeregisterXml = new Application("mbms-deregister+xml", new string[] { "xml" });
            public static readonly Application Mbms_EnvelopeXml = new Application("mbms-envelope+xml", new string[] { "xml" });
            public static readonly Application Mbms_Msk_ResponseXml = new Application("mbms-msk-response+xml", new string[] { "xml" });
            public static readonly Application Mbms_MskXml = new Application("mbms-msk+xml", new string[] { "xml" });
            public static readonly Application Mbms_Protection_DescriptionXml = new Application("mbms-protection-description+xml", new string[] { "xml" });
            public static readonly Application Mbms_Reception_ReportXml = new Application("mbms-reception-report+xml", new string[] { "xml" });
            public static readonly Application Mbms_Register_ResponseXml = new Application("mbms-register-response+xml", new string[] { "xml" });
            public static readonly Application Mbms_RegisterXml = new Application("mbms-register+xml", new string[] { "xml" });
            public static readonly Application Mbms_ScheduleXml = new Application("mbms-schedule+xml", new string[] { "xml" });
            public static readonly Application Mbms_User_Service_DescriptionXml = new Application("mbms-user-service-description+xml", new string[] { "xml" });
            public static readonly Application Mbox = new Application("mbox", new string[] { "mbox" });
            public static readonly Application Media_controlXml = new Application("media_control+xml", new string[] { "xml" });
            public static readonly Application Media_Policy_DatasetXml = new Application("media-policy-dataset+xml", new string[] { "xml" });
            public static readonly Application MediaservercontrolXml = new Application("mediaservercontrol+xml", new string[] { "mscml" });
            public static readonly Application Merge_PatchJson = new Application("merge-patch+json", new string[] { "json" });
            public static readonly Application Metalink4Xml = new Application("metalink4+xml", new string[] { "meta4" });
            public static readonly Application MetalinkXml = new Application("metalink+xml", new string[] { "metalink" });
            public static readonly Application MetsXml = new Application("mets+xml", new string[] { "mets" });
            public static readonly Application MF4 = new Application("mf4");
            public static readonly Application Mikey = new Application("mikey");
            public static readonly Application Mipc = new Application("mipc");
            public static readonly Application Mmt_AeiXml = new Application("mmt-aei+xml", new string[] { "xml" });
            public static readonly Application Mmt_UsdXml = new Application("mmt-usd+xml", new string[] { "xml" });
            public static readonly Application ModsXml = new Application("mods+xml", new string[] { "mods" });
            public static readonly Application Moss_Keys = new Application("moss-keys");
            public static readonly Application Moss_Signature = new Application("moss-signature");
            public static readonly Application Mosskey_Data = new Application("mosskey-data");
            public static readonly Application Mosskey_Request = new Application("mosskey-request");
            public static readonly Application Mp21 = new Application("mp21", new string[] { "m21", "mp21" });
            public static readonly Application Mp4 = new Application("mp4", new string[] { "mp4s" });
            public static readonly Application Mpeg4_Generic = new Application("mpeg4-generic");
            public static readonly Application Mpeg4_Iod = new Application("mpeg4-iod");
            public static readonly Application Mpeg4_Iod_Xmt = new Application("mpeg4-iod-xmt");
            public static readonly Application Mrb_ConsumerXml = new Application("mrb-consumer+xml", new string[] { "xml" });
            public static readonly Application Mrb_PublishXml = new Application("mrb-publish+xml", new string[] { "xml" });
            public static readonly Application Msc_IvrXml = new Application("msc-ivr+xml", new string[] { "xml" });
            public static readonly Application Msc_MixerXml = new Application("msc-mixer+xml", new string[] { "xml" });
            public static readonly Application Msword = new Application("msword", new string[] { "doc", "dot" });
            public static readonly Application MudJson = new Application("mud+json", new string[] { "json" });
            public static readonly Application Multipart_Core = new Application("multipart-core");
            public static readonly Application Mxf = new Application("mxf", new string[] { "mxf" });
            public static readonly Application N_Quads = new Application("n-quads");
            public static readonly Application N_Triples = new Application("n-triples");
            public static readonly Application Nasdata = new Application("nasdata");
            public static readonly Application News_Checkgroups = new Application("news-checkgroups");
            public static readonly Application News_Groupinfo = new Application("news-groupinfo");
            public static readonly Application News_Transmission = new Application("news-transmission");
            public static readonly Application NlsmlXml = new Application("nlsml+xml", new string[] { "xml" });
            public static readonly Application Node = new Application("node");
            public static readonly Application Nss = new Application("nss");
            public static readonly Application Ocsp_Request = new Application("ocsp-request");
            public static readonly Application Ocsp_Response = new Application("ocsp-response");
            public static readonly Application Octet_Stream = new Application("octet-stream", new string[] { "bin", "dms", "lrf", "mar", "so", "dist", "distz", "pkg", "bpk", "dump", "elc", "deploy" });
            public static readonly Application ODA = new Application("oda", new string[] { "oda" });
            public static readonly Application OdmXml = new Application("odm+xml", new string[] { "xml" });
            public static readonly Application ODX = new Application("odx");
            public static readonly Application Oebps_PackageXml = new Application("oebps-package+xml", new string[] { "opf" });
            public static readonly Application Ogg = new Application("ogg", new string[] { "ogx" });
            public static readonly Application OmdocXml = new Application("omdoc+xml", new string[] { "omdoc" });
            public static readonly Application Onenote = new Application("onenote", new string[] { "onetoc", "onetoc2", "onetmp", "onepkg" });
            public static readonly Application Oscore = new Application("oscore");
            public static readonly Application Oxps = new Application("oxps", new string[] { "oxps" });
            public static readonly Application P2p_OverlayXml = new Application("p2p-overlay+xml", new string[] { "xml" });
            public static readonly Application Parityfec = new Application("parityfec");
            public static readonly Application Passport = new Application("passport");
            public static readonly Application Patch_Ops_ErrorXml = new Application("patch-ops-error+xml", new string[] { "xer" });
            public static readonly Application Pdf = new Application("pdf", new string[] { "pdf" });
            public static readonly Application PDX = new Application("pdx");
            public static readonly Application Pem_Certificate_Chain = new Application("pem-certificate-chain");
            public static readonly Application Pgp_Encrypted = new Application("pgp-encrypted", new string[] { "pgp" });
            public static readonly Application Pgp_Keys = new Application("pgp-keys");
            public static readonly Application Pgp_Signature = new Application("pgp-signature", new string[] { "asc", "sig" });
            public static readonly Application Pics_Rules = new Application("pics-rules", new string[] { "prf" });
            public static readonly Application Pidf_DiffXml = new Application("pidf-diff+xml", new string[] { "xml" });
            public static readonly Application PidfXml = new Application("pidf+xml", new string[] { "xml" });
            public static readonly Application Pkcs10 = new Application("pkcs10", new string[] { "p10" });
            public static readonly Application Pkcs12 = new Application("pkcs12");
            public static readonly Application Pkcs7_Mime = new Application("pkcs7-mime", new string[] { "p7m", "p7c" });
            public static readonly Application Pkcs7_Signature = new Application("pkcs7-signature", new string[] { "p7s" });
            public static readonly Application Pkcs8 = new Application("pkcs8", new string[] { "p8" });
            public static readonly Application Pkcs8_Encrypted = new Application("pkcs8-encrypted");
            public static readonly Application Pkix_Attr_Cert = new Application("pkix-attr-cert", new string[] { "ac" });
            public static readonly Application Pkix_Cert = new Application("pkix-cert", new string[] { "cer" });
            public static readonly Application Pkix_Crl = new Application("pkix-crl", new string[] { "crl" });
            public static readonly Application Pkix_Pkipath = new Application("pkix-pkipath", new string[] { "pkipath" });
            public static readonly Application Pkixcmp = new Application("pkixcmp", new string[] { "pki" });
            public static readonly Application PlsXml = new Application("pls+xml", new string[] { "pls" });
            public static readonly Application Poc_SettingsXml = new Application("poc-settings+xml", new string[] { "xml" });
            public static readonly Application Postscript = new Application("postscript", new string[] { "ai", "eps", "ps" });
            public static readonly Application Ppsp_TrackerJson = new Application("ppsp-tracker+json", new string[] { "json" });
            public static readonly Application ProblemJson = new Application("problem+json", new string[] { "json" });
            public static readonly Application ProblemXml = new Application("problem+xml", new string[] { "xml" });
            public static readonly Application ProvenanceXml = new Application("provenance+xml", new string[] { "xml" });
            public static readonly Application PrsAlvestrandTitrax_Sheet = new Application("prs.alvestrand.titrax-sheet");
            public static readonly Application PrsCww = new Application("prs.cww", new string[] { "cww" });
            public static readonly Application PrsHpubZip = new Application("prs.hpub+zip", new string[] { "zip" });
            public static readonly Application PrsNprend = new Application("prs.nprend");
            public static readonly Application PrsPlucker = new Application("prs.plucker");
            public static readonly Application PrsRdf_Xml_Crypt = new Application("prs.rdf-xml-crypt");
            public static readonly Application PrsXsfXml = new Application("prs.xsf+xml", new string[] { "xml" });
            public static readonly Application PskcXml = new Application("pskc+xml", new string[] { "pskcxml" });
            public static readonly Application QSIG = new Application("qsig");
            public static readonly Application Raptorfec = new Application("raptorfec");
            public static readonly Application RdapJson = new Application("rdap+json", new string[] { "json" });
            public static readonly Application RdfXml = new Application("rdf+xml", new string[] { "rdf" });
            public static readonly Application ReginfoXml = new Application("reginfo+xml", new string[] { "rif" });
            public static readonly Application Relax_Ng_Compact_Syntax = new Application("relax-ng-compact-syntax", new string[] { "rnc" });
            public static readonly Application Remote_Printing = new Application("remote-printing");
            public static readonly Application ReputonJson = new Application("reputon+json", new string[] { "json" });
            public static readonly Application Resource_Lists_DiffXml = new Application("resource-lists-diff+xml", new string[] { "rld" });
            public static readonly Application Resource_ListsXml = new Application("resource-lists+xml", new string[] { "rl" });
            public static readonly Application RfcXml = new Application("rfc+xml", new string[] { "xml" });
            public static readonly Application Riscos = new Application("riscos");
            public static readonly Application RlmiXml = new Application("rlmi+xml", new string[] { "xml" });
            public static readonly Application Rls_ServicesXml = new Application("rls-services+xml", new string[] { "rs" });
            public static readonly Application Route_ApdXml = new Application("route-apd+xml", new string[] { "xml" });
            public static readonly Application Route_S_TsidXml = new Application("route-s-tsid+xml", new string[] { "xml" });
            public static readonly Application Route_UsdXml = new Application("route-usd+xml", new string[] { "xml" });
            public static readonly Application Rpki_Ghostbusters = new Application("rpki-ghostbusters", new string[] { "gbr" });
            public static readonly Application Rpki_Manifest = new Application("rpki-manifest", new string[] { "mft" });
            public static readonly Application Rpki_Publication = new Application("rpki-publication");
            public static readonly Application Rpki_Roa = new Application("rpki-roa", new string[] { "roa" });
            public static readonly Application Rpki_Updown = new Application("rpki-updown");
            public static readonly Application RsdXml = new Application("rsd+xml", new string[] { "rsd" });
            public static readonly Application RssXml = new Application("rss+xml", new string[] { "rss" });
            public static readonly Application Rtf = new Application("rtf", new string[] { "rtf" });
            public static readonly Application Rtploopback = new Application("rtploopback");
            public static readonly Application Rtx = new Application("rtx");
            public static readonly Application SamlassertionXml = new Application("samlassertion+xml", new string[] { "xml" });
            public static readonly Application SamlmetadataXml = new Application("samlmetadata+xml", new string[] { "xml" });
            public static readonly Application SbmlXml = new Application("sbml+xml", new string[] { "sbml" });
            public static readonly Application ScaipXml = new Application("scaip+xml", new string[] { "xml" });
            public static readonly Application ScimJson = new Application("scim+json", new string[] { "json" });
            public static readonly Application Scvp_Cv_Request = new Application("scvp-cv-request", new string[] { "scq" });
            public static readonly Application Scvp_Cv_Response = new Application("scvp-cv-response", new string[] { "scs" });
            public static readonly Application Scvp_Vp_Request = new Application("scvp-vp-request", new string[] { "spq" });
            public static readonly Application Scvp_Vp_Response = new Application("scvp-vp-response", new string[] { "spp" });
            public static readonly Application Sdp = new Application("sdp", new string[] { "sdp" });
            public static readonly Application SeceventJwt = new Application("secevent+jwt", new string[] { "jwt" });
            public static readonly Application Senml_Exi = new Application("senml-exi");
            public static readonly Application SenmlCbor = new Application("senml+cbor", new string[] { "cbor" });
            public static readonly Application SenmlJson = new Application("senml+json", new string[] { "json" });
            public static readonly Application SenmlXml = new Application("senml+xml", new string[] { "xml" });
            public static readonly Application Sensml_Exi = new Application("sensml-exi");
            public static readonly Application SensmlCbor = new Application("sensml+cbor", new string[] { "cbor" });
            public static readonly Application SensmlJson = new Application("sensml+json", new string[] { "json" });
            public static readonly Application SensmlXml = new Application("sensml+xml", new string[] { "xml" });
            public static readonly Application Sep_Exi = new Application("sep-exi");
            public static readonly Application SepXml = new Application("sep+xml", new string[] { "xml" });
            public static readonly Application Session_Info = new Application("session-info");
            public static readonly Application Set_Payment = new Application("set-payment");
            public static readonly Application Set_Payment_Initiation = new Application("set-payment-initiation", new string[] { "setpay" });
            public static readonly Application Set_Registration = new Application("set-registration");
            public static readonly Application Set_Registration_Initiation = new Application("set-registration-initiation", new string[] { "setreg" });
            public static readonly Application Sgml = new Application("sgml");
            public static readonly Application Sgml_Open_Catalog = new Application("sgml-open-catalog");
            public static readonly Application ShfXml = new Application("shf+xml", new string[] { "shf" });
            public static readonly Application Sieve = new Application("sieve");
            public static readonly Application Simple_FilterXml = new Application("simple-filter+xml", new string[] { "xml" });
            public static readonly Application Simple_Message_Summary = new Application("simple-message-summary");
            public static readonly Application SimpleSymbolContainer = new Application("simplesymbolcontainer");
            public static readonly Application Sipc = new Application("sipc");
            public static readonly Application Slate = new Application("slate");

            [System.Obsolete("OBSOLETED in favor of application/smil+xml")]
            public static readonly Application Smil = new Application("smil");

            public static readonly Application SmilXml = new Application("smil+xml", new string[] { "smi", "smil" });
            public static readonly Application Smpte336m = new Application("smpte336m");
            public static readonly Application SoapFastinfoset = new Application("soap+fastinfoset", new string[] { "fastinfoset" });
            public static readonly Application SoapXml = new Application("soap+xml", new string[] { "xml" });
            public static readonly Application Sparql_Query = new Application("sparql-query", new string[] { "rq" });
            public static readonly Application Sparql_ResultsXml = new Application("sparql-results+xml", new string[] { "srx" });
            public static readonly Application Spirits_EventXml = new Application("spirits-event+xml", new string[] { "xml" });
            public static readonly Application Sql = new Application("sql");
            public static readonly Application Srgs = new Application("srgs", new string[] { "gram" });
            public static readonly Application SrgsXml = new Application("srgs+xml", new string[] { "grxml" });
            public static readonly Application SruXml = new Application("sru+xml", new string[] { "sru" });
            public static readonly Application SsdlXml = new Application("ssdl+xml", new string[] { "ssdl" });
            public static readonly Application SsmlXml = new Application("ssml+xml", new string[] { "ssml" });
            public static readonly Application StixJson = new Application("stix+json", new string[] { "json" });
            public static readonly Application SwidXml = new Application("swid+xml", new string[] { "xml" });
            public static readonly Application Tamp_Apex_Update = new Application("tamp-apex-update");
            public static readonly Application Tamp_Apex_Update_Confirm = new Application("tamp-apex-update-confirm");
            public static readonly Application Tamp_Community_Update = new Application("tamp-community-update");
            public static readonly Application Tamp_Community_Update_Confirm = new Application("tamp-community-update-confirm");
            public static readonly Application Tamp_Error = new Application("tamp-error");
            public static readonly Application Tamp_Sequence_Adjust = new Application("tamp-sequence-adjust");
            public static readonly Application Tamp_Sequence_Adjust_Confirm = new Application("tamp-sequence-adjust-confirm");
            public static readonly Application Tamp_Status_Query = new Application("tamp-status-query");
            public static readonly Application Tamp_Status_Response = new Application("tamp-status-response");
            public static readonly Application Tamp_Update = new Application("tamp-update");
            public static readonly Application Tamp_Update_Confirm = new Application("tamp-update-confirm");
            public static readonly Application TaxiiJson = new Application("taxii+json", new string[] { "json" });
            public static readonly Application TeiXml = new Application("tei+xml", new string[] { "tei", "teicorpus" });
            public static readonly Application TETRA_ISI = new Application("tetra_isi");
            public static readonly Application ThraudXml = new Application("thraud+xml", new string[] { "tfi" });
            public static readonly Application Timestamp_Query = new Application("timestamp-query");
            public static readonly Application Timestamp_Reply = new Application("timestamp-reply");
            public static readonly Application Timestamped_Data = new Application("timestamped-data", new string[] { "tsd" });
            public static readonly Application TlsrptGzip = new Application("tlsrpt+gzip", new string[] { "gzip" });
            public static readonly Application TlsrptJson = new Application("tlsrpt+json", new string[] { "json" });
            public static readonly Application Tnauthlist = new Application("tnauthlist");
            public static readonly Application Trickle_Ice_Sdpfrag = new Application("trickle-ice-sdpfrag");
            public static readonly Application Trig = new Application("trig");
            public static readonly Application TtmlXml = new Application("ttml+xml", new string[] { "xml" });
            public static readonly Application Tve_Trigger = new Application("tve-trigger");
            public static readonly Application Tzif = new Application("tzif");
            public static readonly Application Tzif_Leap = new Application("tzif-leap");
            public static readonly Application Ulpfec = new Application("ulpfec");
            public static readonly Application Urc_GrpsheetXml = new Application("urc-grpsheet+xml", new string[] { "xml" });
            public static readonly Application Urc_RessheetXml = new Application("urc-ressheet+xml", new string[] { "xml" });
            public static readonly Application Urc_TargetdescXml = new Application("urc-targetdesc+xml", new string[] { "xml" });
            public static readonly Application Urc_UisocketdescXml = new Application("urc-uisocketdesc+xml", new string[] { "xml" });
            public static readonly Application VcardJson = new Application("vcard+json", new string[] { "json" });
            public static readonly Application VcardXml = new Application("vcard+xml", new string[] { "xml" });
            public static readonly Application Vemmi = new Application("vemmi");
            public static readonly Application Vendor1000mindsDecision_ModelXml = new Application("vnd.1000minds.decision-model+xml", new string[] { "xml" });
            public static readonly Application Vendor1d_Interleaved_Parityfec = new Application("1d-interleaved-parityfec");
            public static readonly Application Vendor3gpdash_Qoe_ReportXml = new Application("3gpdash-qoe-report+xml", new string[] { "xml" });
            public static readonly Application Vendor3gpp_ImsXml = new Application("3gpp-ims+xml", new string[] { "xml" });
            public static readonly Application Vendor3gpp_Prose_Pc3chXml = new Application("vnd.3gpp-prose-pc3ch+xml", new string[] { "xml" });
            public static readonly Application Vendor3gpp_ProseXml = new Application("vnd.3gpp-prose+xml", new string[] { "xml" });
            public static readonly Application Vendor3gpp_V2x_Local_Service_Information = new Application("vnd.3gpp-v2x-local-service-information");
            public static readonly Application Vendor3gpp2BcmcsinfoXml = new Application("vnd.3gpp2.bcmcsinfo+xml", new string[] { "xml" });
            public static readonly Application Vendor3gpp2Sms = new Application("vnd.3gpp2.sms");
            public static readonly Application Vendor3gpp2Tcap = new Application("vnd.3gpp2.tcap", new string[] { "tcap" });
            public static readonly Application Vendor3gppAccess_Transfer_EventsXml = new Application("vnd.3gpp.access-transfer-events+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppBsfXml = new Application("vnd.3gpp.bsf+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppGMOPXml = new Application("vnd.3gpp.gmop+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMc_Signalling_Ear = new Application("vnd.3gpp.mc-signalling-ear");
            public static readonly Application Vendor3gppMcdata_Affiliation_CommandXml = new Application("vnd.3gpp.mcdata-affiliation-command+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcdata_InfoXml = new Application("vnd.3gpp.mcdata-info+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcdata_Payload = new Application("vnd.3gpp.mcdata-payload");
            public static readonly Application Vendor3gppMcdata_Service_ConfigXml = new Application("vnd.3gpp.mcdata-service-config+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcdata_Signalling = new Application("vnd.3gpp.mcdata-signalling");
            public static readonly Application Vendor3gppMcdata_Ue_ConfigXml = new Application("vnd.3gpp.mcdata-ue-config+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcdata_User_ProfileXml = new Application("vnd.3gpp.mcdata-user-profile+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcptt_Affiliation_CommandXml = new Application("vnd.3gpp.mcptt-affiliation-command+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcptt_Floor_RequestXml = new Application("vnd.3gpp.mcptt-floor-request+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcptt_InfoXml = new Application("vnd.3gpp.mcptt-info+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcptt_Location_InfoXml = new Application("vnd.3gpp.mcptt-location-info+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcptt_Mbms_Usage_InfoXml = new Application("vnd.3gpp.mcptt-mbms-usage-info+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcptt_Service_ConfigXml = new Application("vnd.3gpp.mcptt-service-config+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcptt_SignedXml = new Application("vnd.3gpp.mcptt-signed+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcptt_Ue_ConfigXml = new Application("vnd.3gpp.mcptt-ue-config+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcptt_Ue_Init_ConfigXml = new Application("vnd.3gpp.mcptt-ue-init-config+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcptt_User_ProfileXml = new Application("vnd.3gpp.mcptt-user-profile+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcvideo_Affiliation_CommandXml = new Application("vnd.3gpp.mcvideo-affiliation-command+xml", new string[] { "xml" });

            [System.Obsolete("OBSOLETED in favor of application/vnd.3gpp.mcvideo-info+xml")]
            public static readonly Application Vendor3gppMcvideo_Affiliation_InfoXml = new Application("vnd.3gpp.mcvideo-affiliation-info+xml", new string[] { "xml" });

            public static readonly Application Vendor3gppMcvideo_InfoXml = new Application("vnd.3gpp.mcvideo-info+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcvideo_Location_InfoXml = new Application("vnd.3gpp.mcvideo-location-info+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcvideo_Mbms_Usage_InfoXml = new Application("vnd.3gpp.mcvideo-mbms-usage-info+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcvideo_Service_ConfigXml = new Application("vnd.3gpp.mcvideo-service-config+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcvideo_Transmission_RequestXml = new Application("vnd.3gpp.mcvideo-transmission-request+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcvideo_Ue_ConfigXml = new Application("vnd.3gpp.mcvideo-ue-config+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMcvideo_User_ProfileXml = new Application("vnd.3gpp.mcvideo-user-profile+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppMid_CallXml = new Application("vnd.3gpp.mid-call+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppPic_Bw_Large = new Application("vnd.3gpp.pic-bw-large", new string[] { "plb" });
            public static readonly Application Vendor3gppPic_Bw_Small = new Application("vnd.3gpp.pic-bw-small", new string[] { "psb" });
            public static readonly Application Vendor3gppPic_Bw_Var = new Application("vnd.3gpp.pic-bw-var", new string[] { "pvb" });
            public static readonly Application Vendor3gppSms = new Application("vnd.3gpp.sms");
            public static readonly Application Vendor3gppSmsXml = new Application("vnd.3gpp.sms+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppSrvcc_ExtXml = new Application("vnd.3gpp.srvcc-ext+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppSRVCC_InfoXml = new Application("vnd.3gpp.srvcc-info+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppState_And_Event_InfoXml = new Application("vnd.3gpp.state-and-event-info+xml", new string[] { "xml" });
            public static readonly Application Vendor3gppUssdXml = new Application("vnd.3gpp.ussd+xml", new string[] { "xml" });
            public static readonly Application Vendor3lightssoftwareImagescal = new Application("vnd.3lightssoftware.imagescal");
            public static readonly Application Vendor3MPost_It_Notes = new Application("vnd.3m.post-it-notes", new string[] { "pwn" });
            public static readonly Application VendorAccpacSimplyAso = new Application("vnd.accpac.simply.aso", new string[] { "aso" });
            public static readonly Application VendorAccpacSimplyImp = new Application("vnd.accpac.simply.imp", new string[] { "imp" });
            public static readonly Application VendorAcucobol = new Application("vnd.acucobol", new string[] { "acu" });
            public static readonly Application VendorAcucorp = new Application("vnd.acucorp", new string[] { "atc", "acutc" });
            public static readonly Application VendorAdobeAir_Application_Installer_PackageZip = new Application("vnd.adobe.air-application-installer-package+zip", new string[] { "air" });
            public static readonly Application VendorAdobeFlashMovie = new Application("vnd.adobe.flash.movie");
            public static readonly Application VendorAdobeFormscentralFcdt = new Application("vnd.adobe.formscentral.fcdt", new string[] { "fcdt" });
            public static readonly Application VendorAdobeFxp = new Application("vnd.adobe.fxp", new string[] { "fxp", "fxpl" });
            public static readonly Application VendorAdobePartial_Upload = new Application("vnd.adobe.partial-upload");
            public static readonly Application VendorAdobeXdpXml = new Application("vnd.adobe.xdp+xml", new string[] { "xdp" });
            public static readonly Application VendorAdobeXfdf = new Application("vnd.adobe.xfdf", new string[] { "xfdf" });
            public static readonly Application VendorAetherImp = new Application("vnd.aether.imp");
            public static readonly Application VendorAfpcAfplinedata = new Application("vnd.afpc.afplinedata");
            public static readonly Application VendorAfpcModca = new Application("vnd.afpc.modca");
            public static readonly Application VendorAh_Barcode = new Application("vnd.ah-barcode");
            public static readonly Application VendorAheadSpace = new Application("vnd.ahead.space", new string[] { "ahead" });
            public static readonly Application VendorAirzipFilesecureAzf = new Application("vnd.airzip.filesecure.azf", new string[] { "azf" });
            public static readonly Application VendorAirzipFilesecureAzs = new Application("vnd.airzip.filesecure.azs", new string[] { "azs" });
            public static readonly Application VendorAmadeusJson = new Application("vnd.amadeus+json", new string[] { "json" });
            public static readonly Application VendorAmazonEbook = new Application("vnd.amazon.ebook", new string[] { "azw" });
            public static readonly Application VendorAmazonMobi8_Ebook = new Application("vnd.amazon.mobi8-ebook");
            public static readonly Application VendorAmericandynamicsAcc = new Application("vnd.americandynamics.acc", new string[] { "acc" });
            public static readonly Application VendorAmigaAmi = new Application("vnd.amiga.ami", new string[] { "ami" });
            public static readonly Application VendorAmundsenMazeXml = new Application("vnd.amundsen.maze+xml", new string[] { "xml" });
            public static readonly Application VendorAndroidOta = new Application("vnd.android.ota");
            public static readonly Application VendorAndroidPackage_Archive = new Application("vnd.android.package-archive", new string[] { "apk" });
            public static readonly Application VendorAnki = new Application("vnd.anki");
            public static readonly Application VendorAnser_Web_Certificate_Issue_Initiation = new Application("vnd.anser-web-certificate-issue-initiation", new string[] { "cii" });
            public static readonly Application VendorAnser_Web_Funds_Transfer_Initiation = new Application("vnd.anser-web-funds-transfer-initiation", new string[] { "fti" });
            public static readonly Application VendorAntixGame_Component = new Application("vnd.antix.game-component", new string[] { "atx" });
            public static readonly Application VendorApacheThriftBinary = new Application("vnd.apache.thrift.binary");
            public static readonly Application VendorApacheThriftCompact = new Application("vnd.apache.thrift.compact");
            public static readonly Application VendorApacheThriftJson = new Application("vnd.apache.thrift.json");
            public static readonly Application VendorApiJson = new Application("vnd.api+json", new string[] { "json" });
            public static readonly Application VendorApothekendeReservationJson = new Application("vnd.apothekende.reservation+json", new string[] { "json" });
            public static readonly Application VendorAppleInstallerXml = new Application("vnd.apple.installer+xml", new string[] { "mpkg" });
            public static readonly Application VendorAppleKeynote = new Application("vnd.apple.keynote");
            public static readonly Application VendorAppleMpegurl = new Application("vnd.apple.mpegurl", new string[] { "m3u8" });
            public static readonly Application VendorAppleNumbers = new Application("vnd.apple.numbers");
            public static readonly Application VendorApplePages = new Application("vnd.apple.pages");

            [System.Obsolete("OBSOLETED in favor of application/vnd.aristanetworks.swi")]
            public static readonly Application VendorArastraSwi = new Application("vnd.arastra.swi");

            public static readonly Application VendorAristanetworksSwi = new Application("vnd.aristanetworks.swi", new string[] { "swi" });
            public static readonly Application VendorArtisanJson = new Application("vnd.artisan+json", new string[] { "json" });
            public static readonly Application VendorArtsquare = new Application("vnd.artsquare");
            public static readonly Application VendorAstraea_SoftwareIota = new Application("vnd.astraea-software.iota", new string[] { "iota" });
            public static readonly Application VendorAudiograph = new Application("vnd.audiograph", new string[] { "aep" });
            public static readonly Application VendorAutopackage = new Application("vnd.autopackage");
            public static readonly Application VendorAvalonJson = new Application("vnd.avalon+json", new string[] { "json" });
            public static readonly Application VendorAvistarXml = new Application("vnd.avistar+xml", new string[] { "xml" });
            public static readonly Application VendorBalsamiqBmmlXml = new Application("vnd.balsamiq.bmml+xml", new string[] { "xml" });
            public static readonly Application VendorBalsamiqBmpr = new Application("vnd.balsamiq.bmpr");
            public static readonly Application VendorBanana_Accounting = new Application("vnd.banana-accounting");
            public static readonly Application VendorBbfUspError = new Application("vnd.bbf.usp.error");
            public static readonly Application VendorBbfUspMsg = new Application("vnd.bbf.usp.msg");
            public static readonly Application VendorBbfUspMsgJson = new Application("vnd.bbf.usp.msg+json", new string[] { "json" });
            public static readonly Application VendorBekitzur_StechJson = new Application("vnd.bekitzur-stech+json", new string[] { "json" });
            public static readonly Application VendorBintMed_Content = new Application("vnd.bint.med-content");
            public static readonly Application VendorBiopaxRdfXml = new Application("vnd.biopax.rdf+xml", new string[] { "xml" });
            public static readonly Application VendorBlink_Idb_Value_Wrapper = new Application("vnd.blink-idb-value-wrapper");
            public static readonly Application VendorBlueiceMultipass = new Application("vnd.blueice.multipass", new string[] { "mpm" });
            public static readonly Application VendorBluetoothEpOob = new Application("vnd.bluetooth.ep.oob");
            public static readonly Application VendorBluetoothLeOob = new Application("vnd.bluetooth.le.oob");
            public static readonly Application VendorBmi = new Application("vnd.bmi", new string[] { "bmi" });
            public static readonly Application VendorBpf = new Application("vnd.bpf");
            public static readonly Application VendorBpf3 = new Application("vnd.bpf3");
            public static readonly Application VendorBusinessobjects = new Application("vnd.businessobjects", new string[] { "rep" });
            public static readonly Application VendorByuUapiJson = new Application("vnd.byu.uapi+json", new string[] { "json" });
            public static readonly Application VendorCab_Jscript = new Application("vnd.cab-jscript");
            public static readonly Application VendorCanon_Cpdl = new Application("vnd.canon-cpdl");
            public static readonly Application VendorCanon_Lips = new Application("vnd.canon-lips");
            public static readonly Application VendorCapasystems_PgJson = new Application("vnd.capasystems-pg+json", new string[] { "json" });
            public static readonly Application VendorCendioThinlincClientconf = new Application("vnd.cendio.thinlinc.clientconf");
            public static readonly Application VendorCentury_SystemsTcp_stream = new Application("vnd.century-systems.tcp_stream");
            public static readonly Application VendorChemdrawXml = new Application("vnd.chemdraw+xml", new string[] { "cdxml" });
            public static readonly Application VendorChess_Pgn = new Application("vnd.chess-pgn");
            public static readonly Application VendorChipnutsKaraoke_Mmd = new Application("vnd.chipnuts.karaoke-mmd", new string[] { "mmd" });
            public static readonly Application VendorCiedi = new Application("vnd.ciedi");
            public static readonly Application VendorCinderella = new Application("vnd.cinderella", new string[] { "cdy" });
            public static readonly Application VendorCirpackIsdn_Ext = new Application("vnd.cirpack.isdn-ext");
            public static readonly Application VendorCitationstylesStyleXml = new Application("vnd.citationstyles.style+xml", new string[] { "xml" });
            public static readonly Application VendorClaymore = new Application("vnd.claymore", new string[] { "cla" });
            public static readonly Application VendorCloantoRp9 = new Application("vnd.cloanto.rp9", new string[] { "rp9" });
            public static readonly Application VendorClonkC4group = new Application("vnd.clonk.c4group", new string[] { "c4g", "c4d", "c4f", "c4p", "c4u" });
            public static readonly Application VendorCluetrustCartomobile_Config = new Application("vnd.cluetrust.cartomobile-config", new string[] { "c11amc" });
            public static readonly Application VendorCluetrustCartomobile_Config_Pkg = new Application("vnd.cluetrust.cartomobile-config-pkg", new string[] { "c11amz" });
            public static readonly Application VendorCoffeescript = new Application("vnd.coffeescript");
            public static readonly Application VendorCollabioXodocumentsDocument = new Application("vnd.collabio.xodocuments.document");
            public static readonly Application VendorCollabioXodocumentsDocument_Template = new Application("vnd.collabio.xodocuments.document-template");
            public static readonly Application VendorCollabioXodocumentsPresentation = new Application("vnd.collabio.xodocuments.presentation");
            public static readonly Application VendorCollabioXodocumentsPresentation_Template = new Application("vnd.collabio.xodocuments.presentation-template");
            public static readonly Application VendorCollabioXodocumentsSpreadsheet = new Application("vnd.collabio.xodocuments.spreadsheet");
            public static readonly Application VendorCollabioXodocumentsSpreadsheet_Template = new Application("vnd.collabio.xodocuments.spreadsheet-template");
            public static readonly Application VendorCollectionDocJson = new Application("vnd.collection.doc+json", new string[] { "json" });
            public static readonly Application VendorCollectionJson = new Application("vnd.collection+json", new string[] { "json" });
            public static readonly Application VendorCollectionNextJson = new Application("vnd.collection.next+json", new string[] { "json" });
            public static readonly Application VendorComicbook_Rar = new Application("vnd.comicbook-rar");
            public static readonly Application VendorComicbookZip = new Application("vnd.comicbook+zip", new string[] { "zip" });
            public static readonly Application VendorCommerce_Battelle = new Application("vnd.commerce-battelle");
            public static readonly Application VendorCommonspace = new Application("vnd.commonspace", new string[] { "csp" });
            public static readonly Application VendorContactCmsg = new Application("vnd.contact.cmsg", new string[] { "cdbcmsg" });
            public static readonly Application VendorCoreosIgnitionJson = new Application("vnd.coreos.ignition+json", new string[] { "json" });
            public static readonly Application VendorCosmocaller = new Application("vnd.cosmocaller", new string[] { "cmc" });
            public static readonly Application VendorCrickClicker = new Application("vnd.crick.clicker", new string[] { "clkx" });
            public static readonly Application VendorCrickClickerKeyboard = new Application("vnd.crick.clicker.keyboard", new string[] { "clkk" });
            public static readonly Application VendorCrickClickerPalette = new Application("vnd.crick.clicker.palette", new string[] { "clkp" });
            public static readonly Application VendorCrickClickerTemplate = new Application("vnd.crick.clicker.template", new string[] { "clkt" });
            public static readonly Application VendorCrickClickerWordbank = new Application("vnd.crick.clicker.wordbank", new string[] { "clkw" });
            public static readonly Application VendorCriticaltoolsWbsXml = new Application("vnd.criticaltools.wbs+xml", new string[] { "wbs" });
            public static readonly Application VendorCryptiiPipeJson = new Application("vnd.cryptii.pipe+json", new string[] { "json" });
            public static readonly Application VendorCrypto_Shade_File = new Application("vnd.crypto-shade-file");
            public static readonly Application VendorCtc_Posml = new Application("vnd.ctc-posml", new string[] { "pml" });
            public static readonly Application VendorCtctWsXml = new Application("vnd.ctct.ws+xml", new string[] { "xml" });
            public static readonly Application VendorCups_Pdf = new Application("vnd.cups-pdf");
            public static readonly Application VendorCups_Postscript = new Application("vnd.cups-postscript");
            public static readonly Application VendorCups_Ppd = new Application("vnd.cups-ppd", new string[] { "ppd" });
            public static readonly Application VendorCups_Raster = new Application("vnd.cups-raster");
            public static readonly Application VendorCups_Raw = new Application("vnd.cups-raw");
            public static readonly Application VendorCurl = new Application("vnd.curl");
            public static readonly Application VendorCurlCar = new Application("vnd.curl.car", new string[] { "car" });
            public static readonly Application VendorCurlPcurl = new Application("vnd.curl.pcurl", new string[] { "pcurl" });
            public static readonly Application VendorCyanDeanRootXml = new Application("vnd.cyan.dean.root+xml", new string[] { "xml" });
            public static readonly Application VendorCybank = new Application("vnd.cybank");
            public static readonly Application VendorD2lCoursepackage1p0Zip = new Application("vnd.d2l.coursepackage1p0+zip", new string[] { "zip" });
            public static readonly Application VendorDart = new Application("vnd.dart", new string[] { "dart" });
            public static readonly Application VendorData_VisionRdz = new Application("vnd.data-vision.rdz", new string[] { "rdz" });
            public static readonly Application VendorDatapackageJson = new Application("vnd.datapackage+json", new string[] { "json" });
            public static readonly Application VendorDataresourceJson = new Application("vnd.dataresource+json", new string[] { "json" });
            public static readonly Application VendorDebianBinary_Package = new Application("vnd.debian.binary-package");
            public static readonly Application VendorDeceData = new Application("vnd.dece.data", new string[] { "uvf", "uvvf", "uvd", "uvvd" });
            public static readonly Application VendorDeceTtmlXml = new Application("vnd.dece.ttml+xml", new string[] { "uvt", "uvvt" });
            public static readonly Application VendorDeceUnspecified = new Application("vnd.dece.unspecified", new string[] { "uvx", "uvvx" });
            public static readonly Application VendorDeceZip = new Application("vnd.dece.zip", new string[] { "uvz", "uvvz" });
            public static readonly Application VendorDenovoFcselayout_Link = new Application("vnd.denovo.fcselayout-link", new string[] { "fe_launch" });
            public static readonly Application VendorDesmumeMovie = new Application("vnd.desmume.movie");
            public static readonly Application VendorDir_BiPlate_Dl_Nosuffix = new Application("vnd.dir-bi.plate-dl-nosuffix");
            public static readonly Application VendorDmDelegationXml = new Application("vnd.dm.delegation+xml", new string[] { "xml" });
            public static readonly Application VendorDna = new Application("vnd.dna", new string[] { "dna" });
            public static readonly Application VendorDocumentJson = new Application("vnd.document+json", new string[] { "json" });
            public static readonly Application VendorDolbyMlp = new Application("vnd.dolby.mlp", new string[] { "mlp" });
            public static readonly Application VendorDolbyMobile1 = new Application("vnd.dolby.mobile.1");
            public static readonly Application VendorDolbyMobile2 = new Application("vnd.dolby.mobile.2");
            public static readonly Application VendorDoremirScorecloud_Binary_Document = new Application("vnd.doremir.scorecloud-binary-document");
            public static readonly Application VendorDpgraph = new Application("vnd.dpgraph", new string[] { "dpg" });
            public static readonly Application VendorDreamfactory = new Application("vnd.dreamfactory", new string[] { "dfac" });
            public static readonly Application VendorDriveJson = new Application("vnd.drive+json", new string[] { "json" });
            public static readonly Application VendorDs_Keypoint = new Application("vnd.ds-keypoint", new string[] { "kpxx" });
            public static readonly Application VendorDtgLocal = new Application("vnd.dtg.local");
            public static readonly Application VendorDtgLocalFlash = new Application("vnd.dtg.local.flash");
            public static readonly Application VendorDtgLocalHtml = new Application("vnd.dtg.local.html");
            public static readonly Application VendorDvbAit = new Application("vnd.dvb.ait", new string[] { "ait" });
            public static readonly Application VendorDvbDvbj = new Application("vnd.dvb.dvbj");
            public static readonly Application VendorDvbEsgcontainer = new Application("vnd.dvb.esgcontainer");
            public static readonly Application VendorDvbIpdcdftnotifaccess = new Application("vnd.dvb.ipdcdftnotifaccess");
            public static readonly Application VendorDvbIpdcesgaccess = new Application("vnd.dvb.ipdcesgaccess");
            public static readonly Application VendorDvbIpdcesgaccess2 = new Application("vnd.dvb.ipdcesgaccess2");
            public static readonly Application VendorDvbIpdcesgpdd = new Application("vnd.dvb.ipdcesgpdd");
            public static readonly Application VendorDvbIpdcroaming = new Application("vnd.dvb.ipdcroaming");
            public static readonly Application VendorDvbIptvAlfec_Base = new Application("vnd.dvb.iptv.alfec-base");
            public static readonly Application VendorDvbIptvAlfec_Enhancement = new Application("vnd.dvb.iptv.alfec-enhancement");
            public static readonly Application VendorDvbNotif_Aggregate_RootXml = new Application("vnd.dvb.notif-aggregate-root+xml", new string[] { "xml" });
            public static readonly Application VendorDvbNotif_ContainerXml = new Application("vnd.dvb.notif-container+xml", new string[] { "xml" });
            public static readonly Application VendorDvbNotif_GenericXml = new Application("vnd.dvb.notif-generic+xml", new string[] { "xml" });
            public static readonly Application VendorDvbNotif_Ia_MsglistXml = new Application("vnd.dvb.notif-ia-msglist+xml", new string[] { "xml" });
            public static readonly Application VendorDvbNotif_Ia_Registration_RequestXml = new Application("vnd.dvb.notif-ia-registration-request+xml", new string[] { "xml" });
            public static readonly Application VendorDvbNotif_Ia_Registration_ResponseXml = new Application("vnd.dvb.notif-ia-registration-response+xml", new string[] { "xml" });
            public static readonly Application VendorDvbNotif_InitXml = new Application("vnd.dvb.notif-init+xml", new string[] { "xml" });
            public static readonly Application VendorDvbPfr = new Application("vnd.dvb.pfr");
            public static readonly Application VendorDvbService = new Application("vnd.dvb.service", new string[] { "svc" });
            public static readonly Application VendorDxr = new Application("vnd.dxr");
            public static readonly Application VendorDynageo = new Application("vnd.dynageo", new string[] { "geo" });
            public static readonly Application VendorDzr = new Application("vnd.dzr");
            public static readonly Application VendorEasykaraokeCdgdownload = new Application("vnd.easykaraoke.cdgdownload");
            public static readonly Application VendorEcdis_Update = new Application("vnd.ecdis-update");
            public static readonly Application VendorEcipRlp = new Application("vnd.ecip.rlp");
            public static readonly Application VendorEcowinChart = new Application("vnd.ecowin.chart", new string[] { "mag" });
            public static readonly Application VendorEcowinFilerequest = new Application("vnd.ecowin.filerequest");
            public static readonly Application VendorEcowinFileupdate = new Application("vnd.ecowin.fileupdate");
            public static readonly Application VendorEcowinSeries = new Application("vnd.ecowin.series");
            public static readonly Application VendorEcowinSeriesrequest = new Application("vnd.ecowin.seriesrequest");
            public static readonly Application VendorEcowinSeriesupdate = new Application("vnd.ecowin.seriesupdate");
            public static readonly Application VendorEfiImg = new Application("vnd.efi.img");
            public static readonly Application VendorEfiIso = new Application("vnd.efi.iso");
            public static readonly Application VendorEmclientAccessrequestXml = new Application("vnd.emclient.accessrequest+xml", new string[] { "xml" });
            public static readonly Application VendorEnliven = new Application("vnd.enliven", new string[] { "nml" });
            public static readonly Application VendorEnphaseEnvoy = new Application("vnd.enphase.envoy");
            public static readonly Application VendorEprintsDataXml = new Application("vnd.eprints.data+xml", new string[] { "xml" });
            public static readonly Application VendorEpsonEsf = new Application("vnd.epson.esf", new string[] { "esf" });
            public static readonly Application VendorEpsonMsf = new Application("vnd.epson.msf", new string[] { "msf" });
            public static readonly Application VendorEpsonQuickanime = new Application("vnd.epson.quickanime", new string[] { "qam" });
            public static readonly Application VendorEpsonSalt = new Application("vnd.epson.salt", new string[] { "slt" });
            public static readonly Application VendorEpsonSsf = new Application("vnd.epson.ssf", new string[] { "ssf" });
            public static readonly Application VendorEricssonQuickcall = new Application("vnd.ericsson.quickcall");
            public static readonly Application VendorEspass_EspassZip = new Application("vnd.espass-espass+zip", new string[] { "zip" });
            public static readonly Application VendorEszigno3Xml = new Application("vnd.eszigno3+xml", new string[] { "es3", "et3" });
            public static readonly Application VendorEtsiAocXml = new Application("vnd.etsi.aoc+xml", new string[] { "xml" });
            public static readonly Application VendorEtsiAsic_EZip = new Application("vnd.etsi.asic-e+zip", new string[] { "zip" });
            public static readonly Application VendorEtsiAsic_SZip = new Application("vnd.etsi.asic-s+zip", new string[] { "zip" });
            public static readonly Application VendorEtsiCugXml = new Application("vnd.etsi.cug+xml", new string[] { "xml" });
            public static readonly Application VendorEtsiIptvcommandXml = new Application("vnd.etsi.iptvcommand+xml", new string[] { "xml" });
            public static readonly Application VendorEtsiIptvdiscoveryXml = new Application("vnd.etsi.iptvdiscovery+xml", new string[] { "xml" });
            public static readonly Application VendorEtsiIptvprofileXml = new Application("vnd.etsi.iptvprofile+xml", new string[] { "xml" });
            public static readonly Application VendorEtsiIptvsad_BcXml = new Application("vnd.etsi.iptvsad-bc+xml", new string[] { "xml" });
            public static readonly Application VendorEtsiIptvsad_CodXml = new Application("vnd.etsi.iptvsad-cod+xml", new string[] { "xml" });
            public static readonly Application VendorEtsiIptvsad_NpvrXml = new Application("vnd.etsi.iptvsad-npvr+xml", new string[] { "xml" });
            public static readonly Application VendorEtsiIptvserviceXml = new Application("vnd.etsi.iptvservice+xml", new string[] { "xml" });
            public static readonly Application VendorEtsiIptvsyncXml = new Application("vnd.etsi.iptvsync+xml", new string[] { "xml" });
            public static readonly Application VendorEtsiIptvueprofileXml = new Application("vnd.etsi.iptvueprofile+xml", new string[] { "xml" });
            public static readonly Application VendorEtsiMcidXml = new Application("vnd.etsi.mcid+xml", new string[] { "xml" });
            public static readonly Application VendorEtsiMheg5 = new Application("vnd.etsi.mheg5");
            public static readonly Application VendorEtsiOverload_Control_Policy_DatasetXml = new Application("vnd.etsi.overload-control-policy-dataset+xml", new string[] { "xml" });
            public static readonly Application VendorEtsiPstnXml = new Application("vnd.etsi.pstn+xml", new string[] { "xml" });
            public static readonly Application VendorEtsiSciXml = new Application("vnd.etsi.sci+xml", new string[] { "xml" });
            public static readonly Application VendorEtsiSimservsXml = new Application("vnd.etsi.simservs+xml", new string[] { "xml" });
            public static readonly Application VendorEtsiTimestamp_Token = new Application("vnd.etsi.timestamp-token");
            public static readonly Application VendorEtsiTslDer = new Application("vnd.etsi.tsl.der");
            public static readonly Application VendorEtsiTslXml = new Application("vnd.etsi.tsl+xml", new string[] { "xml" });
            public static readonly Application VendorEudoraData = new Application("vnd.eudora.data");
            public static readonly Application VendorEvolvEcigProfile = new Application("vnd.evolv.ecig.profile");
            public static readonly Application VendorEvolvEcigSettings = new Application("vnd.evolv.ecig.settings");
            public static readonly Application VendorEvolvEcigTheme = new Application("vnd.evolv.ecig.theme");
            public static readonly Application VendorExstream_EmpowerZip = new Application("vnd.exstream-empower+zip", new string[] { "zip" });
            public static readonly Application VendorExstream_Package = new Application("vnd.exstream-package");
            public static readonly Application VendorEzpix_Album = new Application("vnd.ezpix-album", new string[] { "ez2" });
            public static readonly Application VendorEzpix_Package = new Application("vnd.ezpix-package", new string[] { "ez3" });
            public static readonly Application VendorF_SecureMobile = new Application("vnd.f-secure.mobile");
            public static readonly Application VendorFastcopy_Disk_Image = new Application("vnd.fastcopy-disk-image");
            public static readonly Application VendorFdf = new Application("vnd.fdf", new string[] { "fdf" });
            public static readonly Application VendorFdsnMseed = new Application("vnd.fdsn.mseed", new string[] { "mseed" });
            public static readonly Application VendorFdsnSeed = new Application("vnd.fdsn.seed", new string[] { "seed", "dataless" });
            public static readonly Application VendorFfsns = new Application("vnd.ffsns");
            public static readonly Application VendorFiclabFlbZip = new Application("vnd.ficlab.flb+zip", new string[] { "zip" });
            public static readonly Application VendorFilmitZfc = new Application("vnd.filmit.zfc");
            public static readonly Application VendorFints = new Application("vnd.fints");
            public static readonly Application VendorFiremonkeysCloudcell = new Application("vnd.firemonkeys.cloudcell");
            public static readonly Application VendorFloGraphIt = new Application("vnd.flographit", new string[] { "gph" });
            public static readonly Application VendorFluxtimeClip = new Application("vnd.fluxtime.clip", new string[] { "ftc" });
            public static readonly Application VendorFont_Fontforge_Sfd = new Application("vnd.font-fontforge-sfd");
            public static readonly Application VendorFramemaker = new Application("vnd.framemaker", new string[] { "fm", "frame", "maker", "book" });
            public static readonly Application VendorFrogansFnc = new Application("vnd.frogans.fnc", new string[] { "fnc" });
            public static readonly Application VendorFrogansLtf = new Application("vnd.frogans.ltf", new string[] { "ltf" });
            public static readonly Application VendorFscWeblaunch = new Application("vnd.fsc.weblaunch", new string[] { "fsc" });
            public static readonly Application VendorFujitsuOasys = new Application("vnd.fujitsu.oasys", new string[] { "oas" });
            public static readonly Application VendorFujitsuOasys2 = new Application("vnd.fujitsu.oasys2", new string[] { "oa2" });
            public static readonly Application VendorFujitsuOasys3 = new Application("vnd.fujitsu.oasys3", new string[] { "oa3" });
            public static readonly Application VendorFujitsuOasysgp = new Application("vnd.fujitsu.oasysgp", new string[] { "fg5" });
            public static readonly Application VendorFujitsuOasysprs = new Application("vnd.fujitsu.oasysprs", new string[] { "bh2" });
            public static readonly Application VendorFujixeroxART_EX = new Application("vnd.fujixerox.art-ex");
            public static readonly Application VendorFujixeroxART4 = new Application("vnd.fujixerox.art4");
            public static readonly Application VendorFujixeroxDdd = new Application("vnd.fujixerox.ddd", new string[] { "ddd" });
            public static readonly Application VendorFujixeroxDocuworks = new Application("vnd.fujixerox.docuworks", new string[] { "xdw" });
            public static readonly Application VendorFujixeroxDocuworksBinder = new Application("vnd.fujixerox.docuworks.binder", new string[] { "xbd" });
            public static readonly Application VendorFujixeroxDocuworksContainer = new Application("vnd.fujixerox.docuworks.container");
            public static readonly Application VendorFujixeroxHBPL = new Application("vnd.fujixerox.hbpl");
            public static readonly Application VendorFut_Misnet = new Application("vnd.fut-misnet");
            public static readonly Application VendorFutoinCbor = new Application("vnd.futoin+cbor", new string[] { "cbor" });
            public static readonly Application VendorFutoinJson = new Application("vnd.futoin+json", new string[] { "json" });
            public static readonly Application VendorFuzzysheet = new Application("vnd.fuzzysheet", new string[] { "fzs" });
            public static readonly Application VendorGenomatixTuxedo = new Application("vnd.genomatix.tuxedo", new string[] { "txd" });

            [System.Obsolete("OBSOLETED by request")]
            public static readonly Application VendorGeocubeXml = new Application("vnd.geocube+xml", new string[] { "xml" });

            public static readonly Application VendorGeogebraFile = new Application("vnd.geogebra.file", new string[] { "ggb" });
            public static readonly Application VendorGeogebraTool = new Application("vnd.geogebra.tool", new string[] { "ggt" });

            [System.Obsolete("(OBSOLETED by  in favor of application/geo+json)")]
            public static readonly Application VendorGeoJson = new Application("vnd.geo+json", new string[] { "json" });

            public static readonly Application VendorGeometry_Explorer = new Application("vnd.geometry-explorer", new string[] { "gex", "gre" });
            public static readonly Application VendorGeonext = new Application("vnd.geonext", new string[] { "gxt" });
            public static readonly Application VendorGeoplan = new Application("vnd.geoplan", new string[] { "g2w" });
            public static readonly Application VendorGeospace = new Application("vnd.geospace", new string[] { "g3w" });
            public static readonly Application VendorGerber = new Application("vnd.gerber");
            public static readonly Application VendorGlobalplatformCard_Content_Mgt = new Application("vnd.globalplatform.card-content-mgt");
            public static readonly Application VendorGlobalplatformCard_Content_Mgt_Response = new Application("vnd.globalplatform.card-content-mgt-response");

            [System.Obsolete("DEPRECATED")]
            public static readonly Application VendorGmx = new Application("vnd.gmx", new string[] { "gmx" });

            public static readonly Application VendorGoogle_EarthKmlXml = new Application("vnd.google-earth.kml+xml", new string[] { "kml" });
            public static readonly Application VendorGoogle_EarthKmz = new Application("vnd.google-earth.kmz", new string[] { "kmz" });
            public static readonly Application VendorGovSkE_FormXml = new Application("vnd.gov.sk.e-form+xml", new string[] { "xml" });
            public static readonly Application VendorGovSkE_FormZip = new Application("vnd.gov.sk.e-form+zip", new string[] { "zip" });
            public static readonly Application VendorGovSkXmldatacontainerXml = new Application("vnd.gov.sk.xmldatacontainer+xml", new string[] { "xml" });
            public static readonly Application VendorGrafeq = new Application("vnd.grafeq", new string[] { "gqf", "gqs" });
            public static readonly Application VendorGridmp = new Application("vnd.gridmp");
            public static readonly Application VendorGroove_Account = new Application("vnd.groove-account", new string[] { "gac" });
            public static readonly Application VendorGroove_Help = new Application("vnd.groove-help", new string[] { "ghf" });
            public static readonly Application VendorGroove_Identity_Message = new Application("vnd.groove-identity-message", new string[] { "gim" });
            public static readonly Application VendorGroove_Injector = new Application("vnd.groove-injector", new string[] { "grv" });
            public static readonly Application VendorGroove_Tool_Message = new Application("vnd.groove-tool-message", new string[] { "gtm" });
            public static readonly Application VendorGroove_Tool_Template = new Application("vnd.groove-tool-template", new string[] { "tpl" });
            public static readonly Application VendorGroove_Vcard = new Application("vnd.groove-vcard", new string[] { "vcg" });
            public static readonly Application VendorHalJson = new Application("vnd.hal+json", new string[] { "json" });
            public static readonly Application VendorHalXml = new Application("vnd.hal+xml", new string[] { "hal" });
            public static readonly Application VendorHandHeld_EntertainmentXml = new Application("vnd.handheld-entertainment+xml", new string[] { "zmm" });
            public static readonly Application VendorHbci = new Application("vnd.hbci", new string[] { "hbci" });
            public static readonly Application VendorHcJson = new Application("vnd.hc+json", new string[] { "json" });
            public static readonly Application VendorHcl_Bireports = new Application("vnd.hcl-bireports");
            public static readonly Application VendorHdt = new Application("vnd.hdt");
            public static readonly Application VendorHerokuJson = new Application("vnd.heroku+json", new string[] { "json" });
            public static readonly Application VendorHheLesson_Player = new Application("vnd.hhe.lesson-player", new string[] { "les" });
            public static readonly Application VendorHp_HPGL = new Application("vnd.hp-hpgl", new string[] { "hpgl" });
            public static readonly Application VendorHp_Hpid = new Application("vnd.hp-hpid", new string[] { "hpid" });
            public static readonly Application VendorHp_Hps = new Application("vnd.hp-hps", new string[] { "hps" });
            public static readonly Application VendorHp_Jlyt = new Application("vnd.hp-jlyt", new string[] { "jlt" });
            public static readonly Application VendorHp_PCL = new Application("vnd.hp-pcl", new string[] { "pcl" });
            public static readonly Application VendorHp_PCLXL = new Application("vnd.hp-pclxl", new string[] { "pclxl" });
            public static readonly Application VendorHttphone = new Application("vnd.httphone");
            public static readonly Application VendorHydrostatixSof_Data = new Application("vnd.hydrostatix.sof-data", new string[] { "sfd-hdstx" });
            public static readonly Application VendorHyper_ItemJson = new Application("vnd.hyper-item+json", new string[] { "json" });
            public static readonly Application VendorHyperdriveJson = new Application("vnd.hyperdrive+json", new string[] { "json" });
            public static readonly Application VendorHyperJson = new Application("vnd.hyper+json", new string[] { "json" });
            public static readonly Application VendorHzn_3d_Crossword = new Application("vnd.hzn-3d-crossword");

            [System.Obsolete("OBSOLETED in favor of vnd.afpc.afplinedata")]
            public static readonly Application VendorIbmAfplinedata = new Application("vnd.ibm.afplinedata");

            public static readonly Application VendorIbmElectronic_Media = new Application("vnd.ibm.electronic-media");
            public static readonly Application VendorIbmMiniPay = new Application("vnd.ibm.minipay", new string[] { "mpy" });

            [System.Obsolete("OBSOLETED in favor of application/vnd.afpc.modca")]
            public static readonly Application VendorIbmModcap = new Application("vnd.ibm.modcap", new string[] { "afp", "listafp", "list3820" });

            public static readonly Application VendorIbmRights_Management = new Application("vnd.ibm.rights-management", new string[] { "irm" });
            public static readonly Application VendorIbmSecure_Container = new Application("vnd.ibm.secure-container", new string[] { "sc" });
            public static readonly Application VendorIccprofile = new Application("vnd.iccprofile", new string[] { "icc", "icm" });
            public static readonly Application VendorIeee1905 = new Application("vnd.ieee.1905");
            public static readonly Application VendorIgloader = new Application("vnd.igloader", new string[] { "igl" });
            public static readonly Application VendorImagemeterFolderZip = new Application("vnd.imagemeter.folder+zip", new string[] { "zip" });
            public static readonly Application VendorImagemeterImageZip = new Application("vnd.imagemeter.image+zip", new string[] { "zip" });
            public static readonly Application VendorImmervision_Ivp = new Application("vnd.immervision-ivp", new string[] { "ivp" });
            public static readonly Application VendorImmervision_Ivu = new Application("vnd.immervision-ivu", new string[] { "ivu" });
            public static readonly Application VendorImsImsccv1p1 = new Application("vnd.ims.imsccv1p1");
            public static readonly Application VendorImsImsccv1p2 = new Application("vnd.ims.imsccv1p2");
            public static readonly Application VendorImsImsccv1p3 = new Application("vnd.ims.imsccv1p3");
            public static readonly Application VendorImsLisV2ResultJson = new Application("vnd.ims.lis.v2.result+json", new string[] { "json" });
            public static readonly Application VendorImsLtiV2ToolconsumerprofileJson = new Application("vnd.ims.lti.v2.toolconsumerprofile+json", new string[] { "json" });
            public static readonly Application VendorImsLtiV2ToolproxyIdJson = new Application("vnd.ims.lti.v2.toolproxy.id+json", new string[] { "json" });
            public static readonly Application VendorImsLtiV2ToolproxyJson = new Application("vnd.ims.lti.v2.toolproxy+json", new string[] { "json" });
            public static readonly Application VendorImsLtiV2ToolsettingsJson = new Application("vnd.ims.lti.v2.toolsettings+json", new string[] { "json" });
            public static readonly Application VendorImsLtiV2ToolsettingsSimpleJson = new Application("vnd.ims.lti.v2.toolsettings.simple+json", new string[] { "json" });
            public static readonly Application VendorInformedcontrolRmsXml = new Application("vnd.informedcontrol.rms+xml", new string[] { "xml" });

            [System.Obsolete("OBSOLETED in favor of application/vnd.visionary")]
            public static readonly Application VendorInformix_Visionary = new Application("vnd.informix-visionary");

            public static readonly Application VendorInfotechProject = new Application("vnd.infotech.project");
            public static readonly Application VendorInfotechProjectXml = new Application("vnd.infotech.project+xml", new string[] { "xml" });
            public static readonly Application VendorInnopathWampNotification = new Application("vnd.innopath.wamp.notification");
            public static readonly Application VendorInsorsIgm = new Application("vnd.insors.igm", new string[] { "igm" });
            public static readonly Application VendorInterconFormnet = new Application("vnd.intercon.formnet", new string[] { "xpw", "xpx" });
            public static readonly Application VendorIntergeo = new Application("vnd.intergeo", new string[] { "i2g" });
            public static readonly Application VendorIntertrustDigibox = new Application("vnd.intertrust.digibox");
            public static readonly Application VendorIntertrustNncp = new Application("vnd.intertrust.nncp");
            public static readonly Application VendorIntuQbo = new Application("vnd.intu.qbo", new string[] { "qbo" });
            public static readonly Application VendorIntuQfx = new Application("vnd.intu.qfx", new string[] { "qfx" });
            public static readonly Application VendorIptcG2CatalogitemXml = new Application("vnd.iptc.g2.catalogitem+xml", new string[] { "xml" });
            public static readonly Application VendorIptcG2ConceptitemXml = new Application("vnd.iptc.g2.conceptitem+xml", new string[] { "xml" });
            public static readonly Application VendorIptcG2KnowledgeitemXml = new Application("vnd.iptc.g2.knowledgeitem+xml", new string[] { "xml" });
            public static readonly Application VendorIptcG2NewsitemXml = new Application("vnd.iptc.g2.newsitem+xml", new string[] { "xml" });
            public static readonly Application VendorIptcG2NewsmessageXml = new Application("vnd.iptc.g2.newsmessage+xml", new string[] { "xml" });
            public static readonly Application VendorIptcG2PackageitemXml = new Application("vnd.iptc.g2.packageitem+xml", new string[] { "xml" });
            public static readonly Application VendorIptcG2PlanningitemXml = new Application("vnd.iptc.g2.planningitem+xml", new string[] { "xml" });
            public static readonly Application VendorIpunpluggedRcprofile = new Application("vnd.ipunplugged.rcprofile", new string[] { "rcprofile" });
            public static readonly Application VendorIrepositoryPackageXml = new Application("vnd.irepository.package+xml", new string[] { "irp" });
            public static readonly Application VendorIs_Xpr = new Application("vnd.is-xpr", new string[] { "xpr" });
            public static readonly Application VendorIsacFcs = new Application("vnd.isac.fcs", new string[] { "fcs" });
            public static readonly Application VendorIso11783_10Zip = new Application("vnd.iso11783-10+zip", new string[] { "zip" });
            public static readonly Application VendorJam = new Application("vnd.jam", new string[] { "jam" });
            public static readonly Application VendorJapannet_Directory_Service = new Application("vnd.japannet-directory-service");
            public static readonly Application VendorJapannet_Jpnstore_Wakeup = new Application("vnd.japannet-jpnstore-wakeup");
            public static readonly Application VendorJapannet_Payment_Wakeup = new Application("vnd.japannet-payment-wakeup");
            public static readonly Application VendorJapannet_Registration = new Application("vnd.japannet-registration");
            public static readonly Application VendorJapannet_Registration_Wakeup = new Application("vnd.japannet-registration-wakeup");
            public static readonly Application VendorJapannet_Setstore_Wakeup = new Application("vnd.japannet-setstore-wakeup");
            public static readonly Application VendorJapannet_Verification = new Application("vnd.japannet-verification");
            public static readonly Application VendorJapannet_Verification_Wakeup = new Application("vnd.japannet-verification-wakeup");
            public static readonly Application VendorJcpJavameMidlet_Rms = new Application("vnd.jcp.javame.midlet-rms", new string[] { "rms" });
            public static readonly Application VendorJisp = new Application("vnd.jisp", new string[] { "jisp" });
            public static readonly Application VendorJoostJoda_Archive = new Application("vnd.joost.joda-archive", new string[] { "joda" });
            public static readonly Application VendorJskIsdn_Ngn = new Application("vnd.jsk.isdn-ngn");
            public static readonly Application VendorKahootz = new Application("vnd.kahootz", new string[] { "ktz", "ktr" });
            public static readonly Application VendorKdeKarbon = new Application("vnd.kde.karbon", new string[] { "karbon" });
            public static readonly Application VendorKdeKchart = new Application("vnd.kde.kchart", new string[] { "chrt" });
            public static readonly Application VendorKdeKformula = new Application("vnd.kde.kformula", new string[] { "kfo" });
            public static readonly Application VendorKdeKivio = new Application("vnd.kde.kivio", new string[] { "flw" });
            public static readonly Application VendorKdeKontour = new Application("vnd.kde.kontour", new string[] { "kon" });
            public static readonly Application VendorKdeKpresenter = new Application("vnd.kde.kpresenter", new string[] { "kpr", "kpt" });
            public static readonly Application VendorKdeKspread = new Application("vnd.kde.kspread", new string[] { "ksp" });
            public static readonly Application VendorKdeKword = new Application("vnd.kde.kword", new string[] { "kwd", "kwt" });
            public static readonly Application VendorKenameaapp = new Application("vnd.kenameaapp", new string[] { "htke" });
            public static readonly Application VendorKidspiration = new Application("vnd.kidspiration", new string[] { "kia" });
            public static readonly Application VendorKinar = new Application("vnd.kinar", new string[] { "kne", "knp" });
            public static readonly Application VendorKoan = new Application("vnd.koan", new string[] { "skp", "skd", "skt", "skm" });
            public static readonly Application VendorKodak_Descriptor = new Application("vnd.kodak-descriptor", new string[] { "sse" });
            public static readonly Application VendorLas = new Application("vnd.las");
            public static readonly Application VendorLasLasJson = new Application("vnd.las.las+json", new string[] { "json" });
            public static readonly Application VendorLasLasXml = new Application("vnd.las.las+xml", new string[] { "lasxml" });
            public static readonly Application VendorLaszip = new Application("vnd.laszip");
            public static readonly Application VendorLeapJson = new Application("vnd.leap+json", new string[] { "json" });
            public static readonly Application VendorLiberty_RequestXml = new Application("vnd.liberty-request+xml", new string[] { "xml" });
            public static readonly Application VendorLlamagraphicsLife_BalanceDesktop = new Application("vnd.llamagraphics.life-balance.desktop", new string[] { "lbd" });
            public static readonly Application VendorLlamagraphicsLife_BalanceExchangeXml = new Application("vnd.llamagraphics.life-balance.exchange+xml", new string[] { "lbe" });
            public static readonly Application VendorLogipipeCircuitZip = new Application("vnd.logipipe.circuit+zip", new string[] { "zip" });
            public static readonly Application VendorLoom = new Application("vnd.loom");
            public static readonly Application VendorLotus_1_2_3 = new Application("vnd.lotus-1-2-3", new string[] { "123" });
            public static readonly Application VendorLotus_Approach = new Application("vnd.lotus-approach", new string[] { "apr" });
            public static readonly Application VendorLotus_Freelance = new Application("vnd.lotus-freelance", new string[] { "pre" });
            public static readonly Application VendorLotus_Notes = new Application("vnd.lotus-notes", new string[] { "nsf" });
            public static readonly Application VendorLotus_Organizer = new Application("vnd.lotus-organizer", new string[] { "org" });
            public static readonly Application VendorLotus_Screencam = new Application("vnd.lotus-screencam", new string[] { "scm" });
            public static readonly Application VendorLotus_Wordpro = new Application("vnd.lotus-wordpro", new string[] { "lwp" });
            public static readonly Application VendorMacportsPortpkg = new Application("vnd.macports.portpkg", new string[] { "portpkg" });
            public static readonly Application VendorMapbox_Vector_Tile = new Application("vnd.mapbox-vector-tile");
            public static readonly Application VendorMarlinDrmActiontokenXml = new Application("vnd.marlin.drm.actiontoken+xml", new string[] { "xml" });
            public static readonly Application VendorMarlinDrmConftokenXml = new Application("vnd.marlin.drm.conftoken+xml", new string[] { "xml" });
            public static readonly Application VendorMarlinDrmLicenseXml = new Application("vnd.marlin.drm.license+xml", new string[] { "xml" });
            public static readonly Application VendorMarlinDrmMdcf = new Application("vnd.marlin.drm.mdcf");
            public static readonly Application VendorMasonJson = new Application("vnd.mason+json", new string[] { "json" });
            public static readonly Application VendorMaxmindMaxmind_Db = new Application("vnd.maxmind.maxmind-db");
            public static readonly Application VendorMcd = new Application("vnd.mcd", new string[] { "mcd" });
            public static readonly Application VendorMedcalcdata = new Application("vnd.medcalcdata", new string[] { "mc1" });
            public static readonly Application VendorMediastationCdkey = new Application("vnd.mediastation.cdkey", new string[] { "cdkey" });
            public static readonly Application VendorMeridian_Slingshot = new Application("vnd.meridian-slingshot");
            public static readonly Application VendorMFER = new Application("vnd.mfer", new string[] { "mwf" });
            public static readonly Application VendorMfmp = new Application("vnd.mfmp", new string[] { "mfm" });
            public static readonly Application VendorMicrografxFlo = new Application("vnd.micrografx.flo", new string[] { "flo" });
            public static readonly Application VendorMicrografxIgx = new Application("vnd.micrografx.igx", new string[] { "igx" });
            public static readonly Application VendorMicroJson = new Application("vnd.micro+json", new string[] { "json" });
            public static readonly Application VendorMicrosoftPortable_Executable = new Application("vnd.microsoft.portable-executable");
            public static readonly Application VendorMicrosoftWindowsThumbnail_Cache = new Application("vnd.microsoft.windows.thumbnail-cache");
            public static readonly Application VendorMieleJson = new Application("vnd.miele+json", new string[] { "json" });
            public static readonly Application VendorMif = new Application("vnd.mif", new string[] { "mif" });
            public static readonly Application VendorMinisoft_Hp3000_Save = new Application("vnd.minisoft-hp3000-save");
            public static readonly Application VendorMitsubishiMisty_GuardTrustweb = new Application("vnd.mitsubishi.misty-guard.trustweb");
            public static readonly Application VendorMobiusDAF = new Application("vnd.mobius.daf", new string[] { "daf" });
            public static readonly Application VendorMobiusDIS = new Application("vnd.mobius.dis", new string[] { "dis" });
            public static readonly Application VendorMobiusMBK = new Application("vnd.mobius.mbk", new string[] { "mbk" });
            public static readonly Application VendorMobiusMQY = new Application("vnd.mobius.mqy", new string[] { "mqy" });
            public static readonly Application VendorMobiusMSL = new Application("vnd.mobius.msl", new string[] { "msl" });
            public static readonly Application VendorMobiusPLC = new Application("vnd.mobius.plc", new string[] { "plc" });
            public static readonly Application VendorMobiusTXF = new Application("vnd.mobius.txf", new string[] { "txf" });
            public static readonly Application VendorMophunApplication = new Application("vnd.mophun.application", new string[] { "mpn" });
            public static readonly Application VendorMophunCertificate = new Application("vnd.mophun.certificate", new string[] { "mpc" });
            public static readonly Application VendorMotorolaFlexsuite = new Application("vnd.motorola.flexsuite");
            public static readonly Application VendorMotorolaFlexsuiteAdsi = new Application("vnd.motorola.flexsuite.adsi");
            public static readonly Application VendorMotorolaFlexsuiteFis = new Application("vnd.motorola.flexsuite.fis");
            public static readonly Application VendorMotorolaFlexsuiteGotap = new Application("vnd.motorola.flexsuite.gotap");
            public static readonly Application VendorMotorolaFlexsuiteKmr = new Application("vnd.motorola.flexsuite.kmr");
            public static readonly Application VendorMotorolaFlexsuiteTtc = new Application("vnd.motorola.flexsuite.ttc");
            public static readonly Application VendorMotorolaFlexsuiteWem = new Application("vnd.motorola.flexsuite.wem");
            public static readonly Application VendorMotorolaIprm = new Application("vnd.motorola.iprm");
            public static readonly Application VendorMozillaXulXml = new Application("vnd.mozilla.xul+xml", new string[] { "xul" });
            public static readonly Application VendorMs_3mfdocument = new Application("vnd.ms-3mfdocument");
            public static readonly Application VendorMs_Artgalry = new Application("vnd.ms-artgalry", new string[] { "cil" });
            public static readonly Application VendorMs_Asf = new Application("vnd.ms-asf");
            public static readonly Application VendorMs_Cab_Compressed = new Application("vnd.ms-cab-compressed", new string[] { "cab" });
            public static readonly Application VendorMs_ColorIccprofile = new Application("vnd.ms-color.iccprofile");
            public static readonly Application VendorMs_Excel = new Application("vnd.ms-excel", new string[] { "xls", "xlm", "xla", "xlc", "xlt", "xlw" });
            public static readonly Application VendorMs_ExcelAddinMacroEnabled12 = new Application("vnd.ms-excel.addin.macroenabled.12", new string[] { "xlam" });
            public static readonly Application VendorMs_ExcelSheetBinaryMacroEnabled12 = new Application("vnd.ms-excel.sheet.binary.macroenabled.12", new string[] { "xlsb" });
            public static readonly Application VendorMs_ExcelSheetMacroEnabled12 = new Application("vnd.ms-excel.sheet.macroenabled.12", new string[] { "xlsm" });
            public static readonly Application VendorMs_ExcelTemplateMacroEnabled12 = new Application("vnd.ms-excel.template.macroenabled.12", new string[] { "xltm" });
            public static readonly Application VendorMs_Fontobject = new Application("vnd.ms-fontobject", new string[] { "eot" });
            public static readonly Application VendorMs_Htmlhelp = new Application("vnd.ms-htmlhelp", new string[] { "chm" });
            public static readonly Application VendorMs_Ims = new Application("vnd.ms-ims", new string[] { "ims" });
            public static readonly Application VendorMs_Lrm = new Application("vnd.ms-lrm", new string[] { "lrm" });
            public static readonly Application VendorMs_OfficeActiveXXml = new Application("vnd.ms-office.activex+xml", new string[] { "xml" });
            public static readonly Application VendorMs_Officetheme = new Application("vnd.ms-officetheme", new string[] { "thmx" });
            public static readonly Application VendorMs_Opentype = new Application("vnd.ms-opentype");
            public static readonly Application VendorMs_PackageObfuscated_Opentype = new Application("vnd.ms-package.obfuscated-opentype");
            public static readonly Application VendorMs_PkiSeccat = new Application("vnd.ms-pki.seccat", new string[] { "cat" });
            public static readonly Application VendorMs_PkiStl = new Application("vnd.ms-pki.stl", new string[] { "stl" });
            public static readonly Application VendorMs_PlayreadyInitiatorXml = new Application("vnd.ms-playready.initiator+xml", new string[] { "xml" });
            public static readonly Application VendorMs_Powerpoint = new Application("vnd.ms-powerpoint", new string[] { "ppt", "pps", "pot" });
            public static readonly Application VendorMs_PowerpointAddinMacroEnabled12 = new Application("vnd.ms-powerpoint.addin.macroenabled.12", new string[] { "ppam" });
            public static readonly Application VendorMs_PowerpointPresentationMacroEnabled12 = new Application("vnd.ms-powerpoint.presentation.macroenabled.12", new string[] { "pptm" });
            public static readonly Application VendorMs_PowerpointSlideMacroEnabled12 = new Application("vnd.ms-powerpoint.slide.macroenabled.12", new string[] { "sldm" });
            public static readonly Application VendorMs_PowerpointSlideshowMacroEnabled12 = new Application("vnd.ms-powerpoint.slideshow.macroenabled.12", new string[] { "ppsm" });
            public static readonly Application VendorMs_PowerpointTemplateMacroEnabled12 = new Application("vnd.ms-powerpoint.template.macroenabled.12", new string[] { "potm" });
            public static readonly Application VendorMs_PrintDeviceCapabilitiesXml = new Application("vnd.ms-printdevicecapabilities+xml", new string[] { "xml" });
            public static readonly Application VendorMs_PrintingPrintticketXml = new Application("vnd.ms-printing.printticket+xml", new string[] { "xml" });
            public static readonly Application VendorMs_PrintSchemaTicketXml = new Application("vnd.ms-printschematicket+xml", new string[] { "xml" });
            public static readonly Application VendorMs_Project = new Application("vnd.ms-project", new string[] { "mpp", "mpt" });
            public static readonly Application VendorMs_Tnef = new Application("vnd.ms-tnef");
            public static readonly Application VendorMs_WindowsDevicepairing = new Application("vnd.ms-windows.devicepairing");
            public static readonly Application VendorMs_WindowsNwprintingOob = new Application("vnd.ms-windows.nwprinting.oob");
            public static readonly Application VendorMs_WindowsPrinterpairing = new Application("vnd.ms-windows.printerpairing");
            public static readonly Application VendorMs_WindowsWsdOob = new Application("vnd.ms-windows.wsd.oob");
            public static readonly Application VendorMs_WmdrmLic_Chlg_Req = new Application("vnd.ms-wmdrm.lic-chlg-req");
            public static readonly Application VendorMs_WmdrmLic_Resp = new Application("vnd.ms-wmdrm.lic-resp");
            public static readonly Application VendorMs_WmdrmMeter_Chlg_Req = new Application("vnd.ms-wmdrm.meter-chlg-req");
            public static readonly Application VendorMs_WmdrmMeter_Resp = new Application("vnd.ms-wmdrm.meter-resp");
            public static readonly Application VendorMs_WordDocumentMacroEnabled12 = new Application("vnd.ms-word.document.macroenabled.12", new string[] { "docm" });
            public static readonly Application VendorMs_WordTemplateMacroEnabled12 = new Application("vnd.ms-word.template.macroenabled.12", new string[] { "dotm" });
            public static readonly Application VendorMs_Works = new Application("vnd.ms-works", new string[] { "wps", "wks", "wcm", "wdb" });
            public static readonly Application VendorMs_Wpl = new Application("vnd.ms-wpl", new string[] { "wpl" });
            public static readonly Application VendorMs_Xpsdocument = new Application("vnd.ms-xpsdocument", new string[] { "xps" });
            public static readonly Application VendorMsa_Disk_Image = new Application("vnd.msa-disk-image");
            public static readonly Application VendorMseq = new Application("vnd.mseq", new string[] { "mseq" });
            public static readonly Application VendorMsign = new Application("vnd.msign");
            public static readonly Application VendorMultiadCreator = new Application("vnd.multiad.creator");
            public static readonly Application VendorMultiadCreatorCif = new Application("vnd.multiad.creator.cif");
            public static readonly Application VendorMusic_Niff = new Application("vnd.music-niff");
            public static readonly Application VendorMusician = new Application("vnd.musician", new string[] { "mus" });
            public static readonly Application VendorMuveeStyle = new Application("vnd.muvee.style", new string[] { "msty" });
            public static readonly Application VendorMynfc = new Application("vnd.mynfc", new string[] { "taglet" });
            public static readonly Application VendorNcdControl = new Application("vnd.ncd.control");
            public static readonly Application VendorNcdReference = new Application("vnd.ncd.reference");
            public static readonly Application VendorNearstInvJson = new Application("vnd.nearst.inv+json", new string[] { "json" });
            public static readonly Application VendorNervana = new Application("vnd.nervana");
            public static readonly Application VendorNetfpx = new Application("vnd.netfpx");
            public static readonly Application VendorNeurolanguageNlu = new Application("vnd.neurolanguage.nlu", new string[] { "nlu" });
            public static readonly Application VendorNimn = new Application("vnd.nimn");
            public static readonly Application VendorNintendoNitroRom = new Application("vnd.nintendo.nitro.rom");
            public static readonly Application VendorNintendoSnesRom = new Application("vnd.nintendo.snes.rom");
            public static readonly Application VendorNitf = new Application("vnd.nitf", new string[] { "ntf", "nitf" });
            public static readonly Application VendorNoblenet_Directory = new Application("vnd.noblenet-directory", new string[] { "nnd" });
            public static readonly Application VendorNoblenet_Sealer = new Application("vnd.noblenet-sealer", new string[] { "nns" });
            public static readonly Application VendorNoblenet_Web = new Application("vnd.noblenet-web", new string[] { "nnw" });
            public static readonly Application VendorNokiaCatalogs = new Application("vnd.nokia.catalogs");
            public static readonly Application VendorNokiaConmlWbxml = new Application("vnd.nokia.conml+wbxml", new string[] { "wbxml" });
            public static readonly Application VendorNokiaConmlXml = new Application("vnd.nokia.conml+xml", new string[] { "xml" });
            public static readonly Application VendorNokiaIptvConfigXml = new Application("vnd.nokia.iptv.config+xml", new string[] { "xml" });
            public static readonly Application VendorNokiaISDS_Radio_Presets = new Application("vnd.nokia.isds-radio-presets");
            public static readonly Application VendorNokiaLandmarkcollectionXml = new Application("vnd.nokia.landmarkcollection+xml", new string[] { "xml" });
            public static readonly Application VendorNokiaLandmarkWbxml = new Application("vnd.nokia.landmark+wbxml", new string[] { "wbxml" });
            public static readonly Application VendorNokiaLandmarkXml = new Application("vnd.nokia.landmark+xml", new string[] { "xml" });
            public static readonly Application VendorNokiaN_GageAcXml = new Application("vnd.nokia.n-gage.ac+xml", new string[] { "xml" });
            public static readonly Application VendorNokiaN_GageData = new Application("vnd.nokia.n-gage.data", new string[] { "ngdat" });

            [System.Obsolete("OBSOLETE; no replacement given")]
            public static readonly Application VendorNokiaN_GageSymbianInstall = new Application("vnd.nokia.n-gage.symbian.install", new string[] { "n-gage" });

            public static readonly Application VendorNokiaNcd = new Application("vnd.nokia.ncd");
            public static readonly Application VendorNokiaPcdWbxml = new Application("vnd.nokia.pcd+wbxml", new string[] { "wbxml" });
            public static readonly Application VendorNokiaPcdXml = new Application("vnd.nokia.pcd+xml", new string[] { "xml" });
            public static readonly Application VendorNokiaRadio_Preset = new Application("vnd.nokia.radio-preset", new string[] { "rpst" });
            public static readonly Application VendorNokiaRadio_Presets = new Application("vnd.nokia.radio-presets", new string[] { "rpss" });
            public static readonly Application VendorNovadigmEDM = new Application("vnd.novadigm.edm", new string[] { "edm" });
            public static readonly Application VendorNovadigmEDX = new Application("vnd.novadigm.edx", new string[] { "edx" });
            public static readonly Application VendorNovadigmEXT = new Application("vnd.novadigm.ext", new string[] { "ext" });
            public static readonly Application VendorNtt_LocalContent_Share = new Application("vnd.ntt-local.content-share");
            public static readonly Application VendorNtt_LocalFile_Transfer = new Application("vnd.ntt-local.file-transfer");
            public static readonly Application VendorNtt_LocalOgw_remote_Access = new Application("vnd.ntt-local.ogw_remote-access");
            public static readonly Application VendorNtt_LocalSip_Ta_remote = new Application("vnd.ntt-local.sip-ta_remote");
            public static readonly Application VendorNtt_LocalSip_Ta_tcp_stream = new Application("vnd.ntt-local.sip-ta_tcp_stream");
            public static readonly Application VendorOasisOpendocumentChart = new Application("vnd.oasis.opendocument.chart", new string[] { "odc" });
            public static readonly Application VendorOasisOpendocumentChart_Template = new Application("vnd.oasis.opendocument.chart-template", new string[] { "otc" });
            public static readonly Application VendorOasisOpendocumentDatabase = new Application("vnd.oasis.opendocument.database", new string[] { "odb" });
            public static readonly Application VendorOasisOpendocumentFormula = new Application("vnd.oasis.opendocument.formula", new string[] { "odf" });
            public static readonly Application VendorOasisOpendocumentFormula_Template = new Application("vnd.oasis.opendocument.formula-template", new string[] { "odft" });
            public static readonly Application VendorOasisOpendocumentGraphics = new Application("vnd.oasis.opendocument.graphics", new string[] { "odg" });
            public static readonly Application VendorOasisOpendocumentGraphics_Template = new Application("vnd.oasis.opendocument.graphics-template", new string[] { "otg" });
            public static readonly Application VendorOasisOpendocumentImage = new Application("vnd.oasis.opendocument.image", new string[] { "odi" });
            public static readonly Application VendorOasisOpendocumentImage_Template = new Application("vnd.oasis.opendocument.image-template", new string[] { "oti" });
            public static readonly Application VendorOasisOpendocumentPresentation = new Application("vnd.oasis.opendocument.presentation", new string[] { "odp" });
            public static readonly Application VendorOasisOpendocumentPresentation_Template = new Application("vnd.oasis.opendocument.presentation-template", new string[] { "otp" });
            public static readonly Application VendorOasisOpendocumentSpreadsheet = new Application("vnd.oasis.opendocument.spreadsheet", new string[] { "ods" });
            public static readonly Application VendorOasisOpendocumentSpreadsheet_Template = new Application("vnd.oasis.opendocument.spreadsheet-template", new string[] { "ots" });
            public static readonly Application VendorOasisOpendocumentText = new Application("vnd.oasis.opendocument.text", new string[] { "odt" });
            public static readonly Application VendorOasisOpendocumentText_Master = new Application("vnd.oasis.opendocument.text-master", new string[] { "odm" });
            public static readonly Application VendorOasisOpendocumentText_Template = new Application("vnd.oasis.opendocument.text-template", new string[] { "ott" });
            public static readonly Application VendorOasisOpendocumentText_Web = new Application("vnd.oasis.opendocument.text-web", new string[] { "oth" });
            public static readonly Application VendorObn = new Application("vnd.obn");
            public static readonly Application VendorOcfCbor = new Application("vnd.ocf+cbor", new string[] { "cbor" });
            public static readonly Application VendorOftnL10nJson = new Application("vnd.oftn.l10n+json", new string[] { "json" });
            public static readonly Application VendorOipfContentaccessdownloadXml = new Application("vnd.oipf.contentaccessdownload+xml", new string[] { "xml" });
            public static readonly Application VendorOipfContentaccessstreamingXml = new Application("vnd.oipf.contentaccessstreaming+xml", new string[] { "xml" });
            public static readonly Application VendorOipfCspg_Hexbinary = new Application("vnd.oipf.cspg-hexbinary");
            public static readonly Application VendorOipfDaeSvgXml = new Application("vnd.oipf.dae.svg+xml", new string[] { "xml" });
            public static readonly Application VendorOipfDaeXhtmlXml = new Application("vnd.oipf.dae.xhtml+xml", new string[] { "xml" });
            public static readonly Application VendorOipfMippvcontrolmessageXml = new Application("vnd.oipf.mippvcontrolmessage+xml", new string[] { "xml" });
            public static readonly Application VendorOipfPaeGem = new Application("vnd.oipf.pae.gem");
            public static readonly Application VendorOipfSpdiscoveryXml = new Application("vnd.oipf.spdiscovery+xml", new string[] { "xml" });
            public static readonly Application VendorOipfSpdlistXml = new Application("vnd.oipf.spdlist+xml", new string[] { "xml" });
            public static readonly Application VendorOipfUeprofileXml = new Application("vnd.oipf.ueprofile+xml", new string[] { "xml" });
            public static readonly Application VendorOipfUserprofileXml = new Application("vnd.oipf.userprofile+xml", new string[] { "xml" });
            public static readonly Application VendorOlpc_Sugar = new Application("vnd.olpc-sugar", new string[] { "xo" });
            public static readonly Application VendorOma_Scws_Config = new Application("vnd.oma-scws-config");
            public static readonly Application VendorOma_Scws_Http_Request = new Application("vnd.oma-scws-http-request");
            public static readonly Application VendorOma_Scws_Http_Response = new Application("vnd.oma-scws-http-response");
            public static readonly Application VendorOmaBcastAssociated_Procedure_ParameterXml = new Application("vnd.oma.bcast.associated-procedure-parameter+xml", new string[] { "xml" });
            public static readonly Application VendorOmaBcastDrm_TriggerXml = new Application("vnd.oma.bcast.drm-trigger+xml", new string[] { "xml" });
            public static readonly Application VendorOmaBcastImdXml = new Application("vnd.oma.bcast.imd+xml", new string[] { "xml" });
            public static readonly Application VendorOmaBcastLtkm = new Application("vnd.oma.bcast.ltkm");
            public static readonly Application VendorOmaBcastNotificationXml = new Application("vnd.oma.bcast.notification+xml", new string[] { "xml" });
            public static readonly Application VendorOmaBcastProvisioningtrigger = new Application("vnd.oma.bcast.provisioningtrigger");
            public static readonly Application VendorOmaBcastSgboot = new Application("vnd.oma.bcast.sgboot");
            public static readonly Application VendorOmaBcastSgddXml = new Application("vnd.oma.bcast.sgdd+xml", new string[] { "xml" });
            public static readonly Application VendorOmaBcastSgdu = new Application("vnd.oma.bcast.sgdu");
            public static readonly Application VendorOmaBcastSimple_Symbol_Container = new Application("vnd.oma.bcast.simple-symbol-container");
            public static readonly Application VendorOmaBcastSmartcard_TriggerXml = new Application("vnd.oma.bcast.smartcard-trigger+xml", new string[] { "xml" });
            public static readonly Application VendorOmaBcastSprovXml = new Application("vnd.oma.bcast.sprov+xml", new string[] { "xml" });
            public static readonly Application VendorOmaBcastStkm = new Application("vnd.oma.bcast.stkm");
            public static readonly Application VendorOmaCab_Address_BookXml = new Application("vnd.oma.cab-address-book+xml", new string[] { "xml" });
            public static readonly Application VendorOmaCab_Feature_HandlerXml = new Application("vnd.oma.cab-feature-handler+xml", new string[] { "xml" });
            public static readonly Application VendorOmaCab_PccXml = new Application("vnd.oma.cab-pcc+xml", new string[] { "xml" });
            public static readonly Application VendorOmaCab_Subs_InviteXml = new Application("vnd.oma.cab-subs-invite+xml", new string[] { "xml" });
            public static readonly Application VendorOmaCab_User_PrefsXml = new Application("vnd.oma.cab-user-prefs+xml", new string[] { "xml" });
            public static readonly Application VendorOmaDcd = new Application("vnd.oma.dcd");
            public static readonly Application VendorOmaDcdc = new Application("vnd.oma.dcdc");
            public static readonly Application VendorOmaDd2Xml = new Application("vnd.oma.dd2+xml", new string[] { "dd2" });
            public static readonly Application VendorOmaDrmRisdXml = new Application("vnd.oma.drm.risd+xml", new string[] { "xml" });
            public static readonly Application VendorOmads_EmailXml = new Application("vnd.omads-email+xml", new string[] { "xml" });
            public static readonly Application VendorOmads_FileXml = new Application("vnd.omads-file+xml", new string[] { "xml" });
            public static readonly Application VendorOmads_FolderXml = new Application("vnd.omads-folder+xml", new string[] { "xml" });
            public static readonly Application VendorOmaGroup_Usage_ListXml = new Application("vnd.oma.group-usage-list+xml", new string[] { "xml" });
            public static readonly Application VendorOmaloc_Supl_Init = new Application("vnd.omaloc-supl-init");
            public static readonly Application VendorOmaLwm2mJson = new Application("vnd.oma.lwm2m+json", new string[] { "json" });
            public static readonly Application VendorOmaLwm2mTlv = new Application("vnd.oma.lwm2m+tlv", new string[] { "tlv" });
            public static readonly Application VendorOmaPalXml = new Application("vnd.oma.pal+xml", new string[] { "xml" });
            public static readonly Application VendorOmaPocDetailed_Progress_ReportXml = new Application("vnd.oma.poc.detailed-progress-report+xml", new string[] { "xml" });
            public static readonly Application VendorOmaPocFinal_ReportXml = new Application("vnd.oma.poc.final-report+xml", new string[] { "xml" });
            public static readonly Application VendorOmaPocGroupsXml = new Application("vnd.oma.poc.groups+xml", new string[] { "xml" });
            public static readonly Application VendorOmaPocInvocation_DescriptorXml = new Application("vnd.oma.poc.invocation-descriptor+xml", new string[] { "xml" });
            public static readonly Application VendorOmaPocOptimized_Progress_ReportXml = new Application("vnd.oma.poc.optimized-progress-report+xml", new string[] { "xml" });
            public static readonly Application VendorOmaPush = new Application("vnd.oma.push");
            public static readonly Application VendorOmaScidmMessagesXml = new Application("vnd.oma.scidm.messages+xml", new string[] { "xml" });
            public static readonly Application VendorOmaXcap_DirectoryXml = new Application("vnd.oma.xcap-directory+xml", new string[] { "xml" });
            public static readonly Application VendorOnepager = new Application("vnd.onepager");
            public static readonly Application VendorOnepagertamp = new Application("vnd.onepagertamp");
            public static readonly Application VendorOnepagertamx = new Application("vnd.onepagertamx");
            public static readonly Application VendorOnepagertat = new Application("vnd.onepagertat");
            public static readonly Application VendorOnepagertatp = new Application("vnd.onepagertatp");
            public static readonly Application VendorOnepagertatx = new Application("vnd.onepagertatx");
            public static readonly Application VendorOpenbloxGame_Binary = new Application("vnd.openblox.game-binary");
            public static readonly Application VendorOpenbloxGameXml = new Application("vnd.openblox.game+xml", new string[] { "xml" });
            public static readonly Application VendorOpeneyeOeb = new Application("vnd.openeye.oeb");
            public static readonly Application VendorOpenofficeorgExtension = new Application("vnd.openofficeorg.extension", new string[] { "oxt" });
            public static readonly Application VendorOpenstreetmapDataXml = new Application("vnd.openstreetmap.data+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentCustom_PropertiesXml = new Application("vnd.openxmlformats-officedocument.custom-properties+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentCustomXmlPropertiesXml = new Application("vnd.openxmlformats-officedocument.customxmlproperties+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentDrawingmlChartshapesXml = new Application("vnd.openxmlformats-officedocument.drawingml.chartshapes+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentDrawingmlChartXml = new Application("vnd.openxmlformats-officedocument.drawingml.chart+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentDrawingmlDiagramColorsXml = new Application("vnd.openxmlformats-officedocument.drawingml.diagramcolors+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentDrawingmlDiagramDataXml = new Application("vnd.openxmlformats-officedocument.drawingml.diagramdata+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentDrawingmlDiagramLayoutXml = new Application("vnd.openxmlformats-officedocument.drawingml.diagramlayout+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentDrawingmlDiagramStyleXml = new Application("vnd.openxmlformats-officedocument.drawingml.diagramstyle+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentDrawingXml = new Application("vnd.openxmlformats-officedocument.drawing+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentExtended_PropertiesXml = new Application("vnd.openxmlformats-officedocument.extended-properties+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlCommentAuthorsXml = new Application("vnd.openxmlformats-officedocument.presentationml.commentauthors+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlCommentsXml = new Application("vnd.openxmlformats-officedocument.presentationml.comments+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlHandoutMasterXml = new Application("vnd.openxmlformats-officedocument.presentationml.handoutmaster+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlNotesMasterXml = new Application("vnd.openxmlformats-officedocument.presentationml.notesmaster+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlNotesSlideXml = new Application("vnd.openxmlformats-officedocument.presentationml.notesslide+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlPresentation = new Application("vnd.openxmlformats-officedocument.presentationml.presentation", new string[] { "pptx" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlPresentationMainXml = new Application("vnd.openxmlformats-officedocument.presentationml.presentation.main+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlPresPropsXml = new Application("vnd.openxmlformats-officedocument.presentationml.presprops+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlSlide = new Application("vnd.openxmlformats-officedocument.presentationml.slide", new string[] { "sldx" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlSlideLayoutXml = new Application("vnd.openxmlformats-officedocument.presentationml.slidelayout+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlSlideMasterXml = new Application("vnd.openxmlformats-officedocument.presentationml.slidemaster+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlSlideshow = new Application("vnd.openxmlformats-officedocument.presentationml.slideshow", new string[] { "ppsx" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlSlideshowMainXml = new Application("vnd.openxmlformats-officedocument.presentationml.slideshow.main+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlSlideUpdateInfoXml = new Application("vnd.openxmlformats-officedocument.presentationml.slideupdateinfo+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlSlideXml = new Application("vnd.openxmlformats-officedocument.presentationml.slide+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlTableStylesXml = new Application("vnd.openxmlformats-officedocument.presentationml.tablestyles+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlTagsXml = new Application("vnd.openxmlformats-officedocument.presentationml.tags+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlTemplate = new Application("vnd.openxmlformats-officedocument.presentationml.template", new string[] { "potx" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlTemplateMainXml = new Application("vnd.openxmlformats-officedocument.presentationml.template.main+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentPresentationmlViewPropsXml = new Application("vnd.openxmlformats-officedocument.presentationml.viewprops+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlCalcChainXml = new Application("vnd.openxmlformats-officedocument.spreadsheetml.calcchain+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlChartsheetXml = new Application("vnd.openxmlformats-officedocument.spreadsheetml.chartsheet+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlCommentsXml = new Application("vnd.openxmlformats-officedocument.spreadsheetml.comments+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlConnectionsXml = new Application("vnd.openxmlformats-officedocument.spreadsheetml.connections+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlDialogsheetXml = new Application("vnd.openxmlformats-officedocument.spreadsheetml.dialogsheet+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlExternalLinkXml = new Application("vnd.openxmlformats-officedocument.spreadsheetml.externallink+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlPivotCacheDefinitionXml = new Application("vnd.openxmlformats-officedocument.spreadsheetml.pivotcachedefinition+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlPivotCacheRecordsXml = new Application("vnd.openxmlformats-officedocument.spreadsheetml.pivotcacherecords+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlPivotTableXml = new Application("vnd.openxmlformats-officedocument.spreadsheetml.pivottable+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlQueryTableXml = new Application("vnd.openxmlformats-officedocument.spreadsheetml.querytable+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlRevisionHeadersXml = new Application("vnd.openxmlformats-officedocument.spreadsheetml.revisionheaders+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlRevisionLogXml = new Application("vnd.openxmlformats-officedocument.spreadsheetml.revisionlog+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlSharedStringsXml = new Application("vnd.openxmlformats-officedocument.spreadsheetml.sharedstrings+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlSheet = new Application("vnd.openxmlformats-officedocument.spreadsheetml.sheet", new string[] { "xlsx" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlSheetMainXml = new Application("vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlSheetMetadataXml = new Application("vnd.openxmlformats-officedocument.spreadsheetml.sheetmetadata+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlStylesXml = new Application("vnd.openxmlformats-officedocument.spreadsheetml.styles+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlTableSingleCellsXml = new Application("vnd.openxmlformats-officedocument.spreadsheetml.tablesinglecells+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlTableXml = new Application("vnd.openxmlformats-officedocument.spreadsheetml.table+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlTemplate = new Application("vnd.openxmlformats-officedocument.spreadsheetml.template", new string[] { "xltx" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlTemplateMainXml = new Application("vnd.openxmlformats-officedocument.spreadsheetml.template.main+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlUserNamesXml = new Application("vnd.openxmlformats-officedocument.spreadsheetml.usernames+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlVolatileDependenciesXml = new Application("vnd.openxmlformats-officedocument.spreadsheetml.volatiledependencies+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentSpreadsheetmlWorksheetXml = new Application("vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentThemeOverrideXml = new Application("vnd.openxmlformats-officedocument.themeoverride+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentThemeXml = new Application("vnd.openxmlformats-officedocument.theme+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentVmlDrawing = new Application("vnd.openxmlformats-officedocument.vmldrawing");
            public static readonly Application VendorOpenxmlformats_OfficedocumentWordprocessingmlCommentsXml = new Application("vnd.openxmlformats-officedocument.wordprocessingml.comments+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentWordprocessingmlDocument = new Application("vnd.openxmlformats-officedocument.wordprocessingml.document", new string[] { "docx" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentWordprocessingmlDocumentGlossaryXml = new Application("vnd.openxmlformats-officedocument.wordprocessingml.document.glossary+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentWordprocessingmlDocumentMainXml = new Application("vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentWordprocessingmlEndnotesXml = new Application("vnd.openxmlformats-officedocument.wordprocessingml.endnotes+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentWordprocessingmlFontTableXml = new Application("vnd.openxmlformats-officedocument.wordprocessingml.fonttable+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentWordprocessingmlFooterXml = new Application("vnd.openxmlformats-officedocument.wordprocessingml.footer+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentWordprocessingmlFootnotesXml = new Application("vnd.openxmlformats-officedocument.wordprocessingml.footnotes+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentWordprocessingmlNumberingXml = new Application("vnd.openxmlformats-officedocument.wordprocessingml.numbering+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentWordprocessingmlSettingsXml = new Application("vnd.openxmlformats-officedocument.wordprocessingml.settings+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentWordprocessingmlStylesXml = new Application("vnd.openxmlformats-officedocument.wordprocessingml.styles+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentWordprocessingmlTemplate = new Application("vnd.openxmlformats-officedocument.wordprocessingml.template", new string[] { "dotx" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentWordprocessingmlTemplateMainXml = new Application("vnd.openxmlformats-officedocument.wordprocessingml.template.main+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_OfficedocumentWordprocessingmlWebSettingsXml = new Application("vnd.openxmlformats-officedocument.wordprocessingml.websettings+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_PackageCore_PropertiesXml = new Application("vnd.openxmlformats-package.core-properties+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_PackageDigital_Signature_XmlsignatureXml = new Application("vnd.openxmlformats-package.digital-signature-xmlsignature+xml", new string[] { "xml" });
            public static readonly Application VendorOpenxmlformats_PackageRelationshipsXml = new Application("vnd.openxmlformats-package.relationships+xml", new string[] { "xml" });
            public static readonly Application VendorOracleResourceJson = new Application("vnd.oracle.resource+json", new string[] { "json" });
            public static readonly Application VendorOrangeIndata = new Application("vnd.orange.indata");
            public static readonly Application VendorOsaNetdeploy = new Application("vnd.osa.netdeploy");
            public static readonly Application VendorOsgeoMapguidePackage = new Application("vnd.osgeo.mapguide.package", new string[] { "mgp" });
            public static readonly Application VendorOsgiBundle = new Application("vnd.osgi.bundle");
            public static readonly Application VendorOsgiDp = new Application("vnd.osgi.dp", new string[] { "dp" });
            public static readonly Application VendorOsgiSubsystem = new Application("vnd.osgi.subsystem", new string[] { "esa" });
            public static readonly Application VendorOtpsCt_KipXml = new Application("vnd.otps.ct-kip+xml", new string[] { "xml" });
            public static readonly Application VendorOxliCountgraph = new Application("vnd.oxli.countgraph");
            public static readonly Application VendorPagerdutyJson = new Application("vnd.pagerduty+json", new string[] { "json" });
            public static readonly Application VendorPalm = new Application("vnd.palm", new string[] { "pdb", "pqa", "oprc" });
            public static readonly Application VendorPanoply = new Application("vnd.panoply");
            public static readonly Application VendorPaosXml = new Application("vnd.paos.xml");
            public static readonly Application VendorPatentdive = new Application("vnd.patentdive");
            public static readonly Application VendorPatientecommsdoc = new Application("vnd.patientecommsdoc");
            public static readonly Application VendorPawaafile = new Application("vnd.pawaafile", new string[] { "paw" });
            public static readonly Application VendorPcos = new Application("vnd.pcos");
            public static readonly Application VendorPgFormat = new Application("vnd.pg.format", new string[] { "str" });
            public static readonly Application VendorPgOsasli = new Application("vnd.pg.osasli", new string[] { "ei6" });
            public static readonly Application VendorPiaccessApplication_Licence = new Application("vnd.piaccess.application-licence");
            public static readonly Application VendorPicsel = new Application("vnd.picsel", new string[] { "efif" });
            public static readonly Application VendorPmiWidget = new Application("vnd.pmi.widget", new string[] { "wg" });
            public static readonly Application VendorPocGroup_AdvertisementXml = new Application("vnd.poc.group-advertisement+xml", new string[] { "xml" });
            public static readonly Application VendorPocketlearn = new Application("vnd.pocketlearn", new string[] { "plf" });
            public static readonly Application VendorPowerbuilder6 = new Application("vnd.powerbuilder6", new string[] { "pbd" });
            public static readonly Application VendorPowerbuilder6_S = new Application("vnd.powerbuilder6-s");
            public static readonly Application VendorPowerbuilder7 = new Application("vnd.powerbuilder7");
            public static readonly Application VendorPowerbuilder7_S = new Application("vnd.powerbuilder7-s");
            public static readonly Application VendorPowerbuilder75 = new Application("vnd.powerbuilder75");
            public static readonly Application VendorPowerbuilder75_S = new Application("vnd.powerbuilder75-s");
            public static readonly Application VendorPreminet = new Application("vnd.preminet");
            public static readonly Application VendorPreviewsystemsBox = new Application("vnd.previewsystems.box", new string[] { "box" });
            public static readonly Application VendorProteusMagazine = new Application("vnd.proteus.magazine", new string[] { "mgz" });
            public static readonly Application VendorPsfs = new Application("vnd.psfs");
            public static readonly Application VendorPublishare_Delta_Tree = new Application("vnd.publishare-delta-tree", new string[] { "qps" });
            public static readonly Application VendorPviPtid1 = new Application("vnd.pvi.ptid1", new string[] { "ptid" });
            public static readonly Application VendorPwg_Multiplexed = new Application("vnd.pwg-multiplexed");
            public static readonly Application VendorPwg_Xhtml_PrintXml = new Application("vnd.pwg-xhtml-print+xml", new string[] { "xml" });
            public static readonly Application VendorQualcommBrew_App_Res = new Application("vnd.qualcomm.brew-app-res");
            public static readonly Application VendorQuarantainenet = new Application("vnd.quarantainenet");
            public static readonly Application VendorQuarkQuarkXPress = new Application("vnd.quark.quarkxpress", new string[] { "qxd", "qxt", "qwd", "qwt", "qxl", "qxb" });
            public static readonly Application VendorQuobject_Quoxdocument = new Application("vnd.quobject-quoxdocument");
            public static readonly Application VendorRadisysMomlXml = new Application("vnd.radisys.moml+xml", new string[] { "xml" });
            public static readonly Application VendorRadisysMsml_Audit_ConfXml = new Application("vnd.radisys.msml-audit-conf+xml", new string[] { "xml" });
            public static readonly Application VendorRadisysMsml_Audit_ConnXml = new Application("vnd.radisys.msml-audit-conn+xml", new string[] { "xml" });
            public static readonly Application VendorRadisysMsml_Audit_DialogXml = new Application("vnd.radisys.msml-audit-dialog+xml", new string[] { "xml" });
            public static readonly Application VendorRadisysMsml_Audit_StreamXml = new Application("vnd.radisys.msml-audit-stream+xml", new string[] { "xml" });
            public static readonly Application VendorRadisysMsml_AuditXml = new Application("vnd.radisys.msml-audit+xml", new string[] { "xml" });
            public static readonly Application VendorRadisysMsml_ConfXml = new Application("vnd.radisys.msml-conf+xml", new string[] { "xml" });
            public static readonly Application VendorRadisysMsml_Dialog_BaseXml = new Application("vnd.radisys.msml-dialog-base+xml", new string[] { "xml" });
            public static readonly Application VendorRadisysMsml_Dialog_Fax_DetectXml = new Application("vnd.radisys.msml-dialog-fax-detect+xml", new string[] { "xml" });
            public static readonly Application VendorRadisysMsml_Dialog_Fax_SendrecvXml = new Application("vnd.radisys.msml-dialog-fax-sendrecv+xml", new string[] { "xml" });
            public static readonly Application VendorRadisysMsml_Dialog_GroupXml = new Application("vnd.radisys.msml-dialog-group+xml", new string[] { "xml" });
            public static readonly Application VendorRadisysMsml_Dialog_SpeechXml = new Application("vnd.radisys.msml-dialog-speech+xml", new string[] { "xml" });
            public static readonly Application VendorRadisysMsml_Dialog_TransformXml = new Application("vnd.radisys.msml-dialog-transform+xml", new string[] { "xml" });
            public static readonly Application VendorRadisysMsml_DialogXml = new Application("vnd.radisys.msml-dialog+xml", new string[] { "xml" });
            public static readonly Application VendorRadisysMsmlXml = new Application("vnd.radisys.msml+xml", new string[] { "xml" });
            public static readonly Application VendorRainstorData = new Application("vnd.rainstor.data");
            public static readonly Application VendorRapid = new Application("vnd.rapid");
            public static readonly Application VendorRar = new Application("vnd.rar");
            public static readonly Application VendorRealvncBed = new Application("vnd.realvnc.bed", new string[] { "bed" });
            public static readonly Application VendorRecordareMusicxml = new Application("vnd.recordare.musicxml", new string[] { "mxl" });
            public static readonly Application VendorRecordareMusicxmlXml = new Application("vnd.recordare.musicxml+xml", new string[] { "musicxml" });
            public static readonly Application VendorRenLearnRlprint = new Application("vnd.renlearn.rlprint");
            public static readonly Application VendorRestfulJson = new Application("vnd.restful+json", new string[] { "json" });
            public static readonly Application VendorRigCryptonote = new Application("vnd.rig.cryptonote", new string[] { "cryptonote" });
            public static readonly Application VendorRimCod = new Application("vnd.rim.cod", new string[] { "cod" });
            public static readonly Application VendorRn_Realmedia = new Application("vnd.rn-realmedia", new string[] { "rm" });
            public static readonly Application VendorRn_Realmedia_Vbr = new Application("vnd.rn-realmedia-vbr", new string[] { "rmvb" });
            public static readonly Application VendorRoute66Link66Xml = new Application("vnd.route66.link66+xml", new string[] { "link66" });
            public static readonly Application VendorRs_274x = new Application("vnd.rs-274x");
            public static readonly Application VendorRuckusDownload = new Application("vnd.ruckus.download");
            public static readonly Application VendorS3sms = new Application("vnd.s3sms");
            public static readonly Application VendorSailingtrackerTrack = new Application("vnd.sailingtracker.track", new string[] { "st" });
            public static readonly Application VendorSbmCid = new Application("vnd.sbm.cid");
            public static readonly Application VendorSbmMid2 = new Application("vnd.sbm.mid2");
            public static readonly Application VendorScribus = new Application("vnd.scribus");
            public static readonly Application VendorSealed3df = new Application("vnd.sealed.3df");
            public static readonly Application VendorSealedCsf = new Application("vnd.sealed.csf");
            public static readonly Application VendorSealedDoc = new Application("vnd.sealed.doc");
            public static readonly Application VendorSealedEml = new Application("vnd.sealed.eml");
            public static readonly Application VendorSealedmediaSoftsealHtml = new Application("vnd.sealedmedia.softseal.html");
            public static readonly Application VendorSealedmediaSoftsealPdf = new Application("vnd.sealedmedia.softseal.pdf");
            public static readonly Application VendorSealedMht = new Application("vnd.sealed.mht");
            public static readonly Application VendorSealedNet = new Application("vnd.sealed.net");
            public static readonly Application VendorSealedPpt = new Application("vnd.sealed.ppt");
            public static readonly Application VendorSealedTiff = new Application("vnd.sealed.tiff");
            public static readonly Application VendorSealedXls = new Application("vnd.sealed.xls");
            public static readonly Application VendorSeemail = new Application("vnd.seemail", new string[] { "see" });
            public static readonly Application VendorSema = new Application("vnd.sema", new string[] { "sema" });
            public static readonly Application VendorSemd = new Application("vnd.semd", new string[] { "semd" });
            public static readonly Application VendorSemf = new Application("vnd.semf", new string[] { "semf" });
            public static readonly Application VendorShade_Save_File = new Application("vnd.shade-save-file");
            public static readonly Application VendorShanaInformedFormdata = new Application("vnd.shana.informed.formdata", new string[] { "ifm" });
            public static readonly Application VendorShanaInformedFormtemplate = new Application("vnd.shana.informed.formtemplate", new string[] { "itp" });
            public static readonly Application VendorShanaInformedInterchange = new Application("vnd.shana.informed.interchange", new string[] { "iif" });
            public static readonly Application VendorShanaInformedPackage = new Application("vnd.shana.informed.package", new string[] { "ipk" });
            public static readonly Application VendorShootproofJson = new Application("vnd.shootproof+json", new string[] { "json" });
            public static readonly Application VendorShopkickJson = new Application("vnd.shopkick+json", new string[] { "json" });
            public static readonly Application VendorSigrokSession = new Application("vnd.sigrok.session");
            public static readonly Application VendorSimTech_MindMapper = new Application("vnd.simtech-mindmapper", new string[] { "twd", "twds" });
            public static readonly Application VendorSirenJson = new Application("vnd.siren+json", new string[] { "json" });
            public static readonly Application VendorSmaf = new Application("vnd.smaf", new string[] { "mmf" });
            public static readonly Application VendorSmartNotebook = new Application("vnd.smart.notebook");
            public static readonly Application VendorSmartTeacher = new Application("vnd.smart.teacher", new string[] { "teacher" });
            public static readonly Application VendorSoftware602FillerForm_Xml_Zip = new Application("vnd.software602.filler.form-xml-zip");
            public static readonly Application VendorSoftware602FillerFormXml = new Application("vnd.software602.filler.form+xml", new string[] { "xml" });
            public static readonly Application VendorSolentSdkmXml = new Application("vnd.solent.sdkm+xml", new string[] { "sdkm", "sdkd" });
            public static readonly Application VendorSpotfireDxp = new Application("vnd.spotfire.dxp", new string[] { "dxp" });
            public static readonly Application VendorSpotfireSfs = new Application("vnd.spotfire.sfs", new string[] { "sfs" });
            public static readonly Application VendorSqlite3 = new Application("vnd.sqlite3");
            public static readonly Application VendorSss_Cod = new Application("vnd.sss-cod");
            public static readonly Application VendorSss_Dtf = new Application("vnd.sss-dtf");
            public static readonly Application VendorSss_Ntf = new Application("vnd.sss-ntf");
            public static readonly Application VendorStardivisionCalc = new Application("vnd.stardivision.calc", new string[] { "sdc" });
            public static readonly Application VendorStardivisionDraw = new Application("vnd.stardivision.draw", new string[] { "sda" });
            public static readonly Application VendorStardivisionImpress = new Application("vnd.stardivision.impress", new string[] { "sdd" });
            public static readonly Application VendorStardivisionMath = new Application("vnd.stardivision.math", new string[] { "smf" });
            public static readonly Application VendorStardivisionWriter = new Application("vnd.stardivision.writer", new string[] { "sdw", "vor" });
            public static readonly Application VendorStardivisionWriter_Global = new Application("vnd.stardivision.writer-global", new string[] { "sgl" });
            public static readonly Application VendorStepmaniaPackage = new Application("vnd.stepmania.package", new string[] { "smzip" });
            public static readonly Application VendorStepmaniaStepchart = new Application("vnd.stepmania.stepchart", new string[] { "sm" });
            public static readonly Application VendorStreet_Stream = new Application("vnd.street-stream");
            public static readonly Application VendorSunWadlXml = new Application("vnd.sun.wadl+xml", new string[] { "xml" });
            public static readonly Application VendorSunXmlCalc = new Application("vnd.sun.xml.calc", new string[] { "sxc" });
            public static readonly Application VendorSunXmlCalcTemplate = new Application("vnd.sun.xml.calc.template", new string[] { "stc" });
            public static readonly Application VendorSunXmlDraw = new Application("vnd.sun.xml.draw", new string[] { "sxd" });
            public static readonly Application VendorSunXmlDrawTemplate = new Application("vnd.sun.xml.draw.template", new string[] { "std" });
            public static readonly Application VendorSunXmlImpress = new Application("vnd.sun.xml.impress", new string[] { "sxi" });
            public static readonly Application VendorSunXmlImpressTemplate = new Application("vnd.sun.xml.impress.template", new string[] { "sti" });
            public static readonly Application VendorSunXmlMath = new Application("vnd.sun.xml.math", new string[] { "sxm" });
            public static readonly Application VendorSunXmlWriter = new Application("vnd.sun.xml.writer", new string[] { "sxw" });
            public static readonly Application VendorSunXmlWriterGlobal = new Application("vnd.sun.xml.writer.global", new string[] { "sxg" });
            public static readonly Application VendorSunXmlWriterTemplate = new Application("vnd.sun.xml.writer.template", new string[] { "stw" });
            public static readonly Application VendorSus_Calendar = new Application("vnd.sus-calendar", new string[] { "sus", "susp" });
            public static readonly Application VendorSvd = new Application("vnd.svd", new string[] { "svd" });
            public static readonly Application VendorSwiftview_Ics = new Application("vnd.swiftview-ics");
            public static readonly Application VendorSymbianInstall = new Application("vnd.symbian.install", new string[] { "sis", "sisx" });
            public static readonly Application VendorSyncmlDmddfWbxml = new Application("vnd.syncml.dmddf+wbxml", new string[] { "wbxml" });
            public static readonly Application VendorSyncmlDmddfXml = new Application("vnd.syncml.dmddf+xml", new string[] { "xml" });
            public static readonly Application VendorSyncmlDmNotification = new Application("vnd.syncml.dm.notification");
            public static readonly Application VendorSyncmlDmtndsWbxml = new Application("vnd.syncml.dmtnds+wbxml", new string[] { "wbxml" });
            public static readonly Application VendorSyncmlDmtndsXml = new Application("vnd.syncml.dmtnds+xml", new string[] { "xml" });
            public static readonly Application VendorSyncmlDmWbxml = new Application("vnd.syncml.dm+wbxml", new string[] { "bdm" });
            public static readonly Application VendorSyncmlDmXml = new Application("vnd.syncml.dm+xml", new string[] { "xdm" });
            public static readonly Application VendorSyncmlDsNotification = new Application("vnd.syncml.ds.notification");
            public static readonly Application VendorSyncmlXml = new Application("vnd.syncml+xml", new string[] { "xsm" });
            public static readonly Application VendorTableschemaJson = new Application("vnd.tableschema+json", new string[] { "json" });
            public static readonly Application VendorTaoIntent_Module_Archive = new Application("vnd.tao.intent-module-archive", new string[] { "tao" });
            public static readonly Application VendorTcpdumpPcap = new Application("vnd.tcpdump.pcap", new string[] { "pcap", "cap", "dmp" });
            public static readonly Application VendorThink_CellPpttcJson = new Application("vnd.think-cell.ppttc+json", new string[] { "json" });
            public static readonly Application VendorTmdMediaflexApiXml = new Application("vnd.tmd.mediaflex.api+xml", new string[] { "xml" });
            public static readonly Application VendorTml = new Application("vnd.tml");
            public static readonly Application VendorTmobile_Livetv = new Application("vnd.tmobile-livetv", new string[] { "tmo" });
            public static readonly Application VendorTridTpt = new Application("vnd.trid.tpt", new string[] { "tpt" });
            public static readonly Application VendorTriOnesource = new Application("vnd.tri.onesource");
            public static readonly Application VendorTriscapeMxs = new Application("vnd.triscape.mxs", new string[] { "mxs" });
            public static readonly Application VendorTrueapp = new Application("vnd.trueapp", new string[] { "tra" });
            public static readonly Application VendorTruedoc = new Application("vnd.truedoc");
            public static readonly Application VendorUbisoftWebplayer = new Application("vnd.ubisoft.webplayer");
            public static readonly Application VendorUfdl = new Application("vnd.ufdl", new string[] { "ufd", "ufdl" });
            public static readonly Application VendorUiqTheme = new Application("vnd.uiq.theme", new string[] { "utz" });
            public static readonly Application VendorUmajin = new Application("vnd.umajin", new string[] { "umj" });
            public static readonly Application VendorUnity = new Application("vnd.unity", new string[] { "unityweb" });
            public static readonly Application VendorUomlXml = new Application("vnd.uoml+xml", new string[] { "uoml" });
            public static readonly Application VendorUplanetAlert = new Application("vnd.uplanet.alert");
            public static readonly Application VendorUplanetAlert_Wbxml = new Application("vnd.uplanet.alert-wbxml");
            public static readonly Application VendorUplanetBearer_Choice = new Application("vnd.uplanet.bearer-choice");
            public static readonly Application VendorUplanetBearer_Choice_Wbxml = new Application("vnd.uplanet.bearer-choice-wbxml");
            public static readonly Application VendorUplanetCacheop = new Application("vnd.uplanet.cacheop");
            public static readonly Application VendorUplanetCacheop_Wbxml = new Application("vnd.uplanet.cacheop-wbxml");
            public static readonly Application VendorUplanetChannel = new Application("vnd.uplanet.channel");
            public static readonly Application VendorUplanetChannel_Wbxml = new Application("vnd.uplanet.channel-wbxml");
            public static readonly Application VendorUplanetList = new Application("vnd.uplanet.list");
            public static readonly Application VendorUplanetList_Wbxml = new Application("vnd.uplanet.list-wbxml");
            public static readonly Application VendorUplanetListcmd = new Application("vnd.uplanet.listcmd");
            public static readonly Application VendorUplanetListcmd_Wbxml = new Application("vnd.uplanet.listcmd-wbxml");
            public static readonly Application VendorUplanetSignal = new Application("vnd.uplanet.signal");
            public static readonly Application VendorUri_Map = new Application("vnd.uri-map");
            public static readonly Application VendorValveSourceMaterial = new Application("vnd.valve.source.material");
            public static readonly Application VendorVcx = new Application("vnd.vcx", new string[] { "vcx" });
            public static readonly Application VendorVd_Study = new Application("vnd.vd-study");
            public static readonly Application VendorVectorworks = new Application("vnd.vectorworks");
            public static readonly Application VendorVelJson = new Application("vnd.vel+json", new string[] { "json" });
            public static readonly Application VendorVerimatrixVcas = new Application("vnd.verimatrix.vcas");
            public static readonly Application VendorVeryantThin = new Application("vnd.veryant.thin");
            public static readonly Application VendorVesEncrypted = new Application("vnd.ves.encrypted");
            public static readonly Application VendorVidsoftVidconference = new Application("vnd.vidsoft.vidconference");
            public static readonly Application VendorVisio = new Application("vnd.visio", new string[] { "vsd", "vst", "vss", "vsw" });
            public static readonly Application VendorVisionary = new Application("vnd.visionary", new string[] { "vis" });
            public static readonly Application VendorVividenceScriptfile = new Application("vnd.vividence.scriptfile");
            public static readonly Application VendorVsf = new Application("vnd.vsf", new string[] { "vsf" });
            public static readonly Application VendorWapSic = new Application("vnd.wap.sic");
            public static readonly Application VendorWapSlc = new Application("vnd.wap.slc");
            public static readonly Application VendorWapWbxml = new Application("vnd.wap.wbxml", new string[] { "wbxml" });
            public static readonly Application VendorWapWmlc = new Application("vnd.wap.wmlc", new string[] { "wmlc" });
            public static readonly Application VendorWapWmlscriptc = new Application("vnd.wap.wmlscriptc", new string[] { "wmlsc" });
            public static readonly Application VendorWebturbo = new Application("vnd.webturbo", new string[] { "wtb" });
            public static readonly Application VendorWfaP2p = new Application("vnd.wfa.p2p");
            public static readonly Application VendorWfaWsc = new Application("vnd.wfa.wsc");
            public static readonly Application VendorWindowsDevicepairing = new Application("vnd.windows.devicepairing");
            public static readonly Application VendorWmc = new Application("vnd.wmc");
            public static readonly Application VendorWmfBootstrap = new Application("vnd.wmf.bootstrap");
            public static readonly Application VendorWolframMathematica = new Application("vnd.wolfram.mathematica");
            public static readonly Application VendorWolframMathematicaPackage = new Application("vnd.wolfram.mathematica.package");
            public static readonly Application VendorWolframPlayer = new Application("vnd.wolfram.player", new string[] { "nbp" });
            public static readonly Application VendorWordperfect = new Application("vnd.wordperfect", new string[] { "wpd" });
            public static readonly Application VendorWqd = new Application("vnd.wqd", new string[] { "wqd" });
            public static readonly Application VendorWrq_Hp3000_Labelled = new Application("vnd.wrq-hp3000-labelled");
            public static readonly Application VendorWtStf = new Application("vnd.wt.stf", new string[] { "stf" });
            public static readonly Application VendorWvCspWbxml = new Application("vnd.wv.csp+wbxml", new string[] { "wbxml" });
            public static readonly Application VendorWvCspXml = new Application("vnd.wv.csp+xml", new string[] { "xml" });
            public static readonly Application VendorWvSspXml = new Application("vnd.wv.ssp+xml", new string[] { "xml" });
            public static readonly Application VendorXacmlJson = new Application("vnd.xacml+json", new string[] { "json" });
            public static readonly Application VendorXara = new Application("vnd.xara", new string[] { "xar" });
            public static readonly Application VendorXfdl = new Application("vnd.xfdl", new string[] { "xfdl" });
            public static readonly Application VendorXfdlWebform = new Application("vnd.xfdl.webform");
            public static readonly Application VendorXmiXml = new Application("vnd.xmi+xml", new string[] { "xml" });
            public static readonly Application VendorXmpieCpkg = new Application("vnd.xmpie.cpkg");
            public static readonly Application VendorXmpieDpkg = new Application("vnd.xmpie.dpkg");
            public static readonly Application VendorXmpiePlan = new Application("vnd.xmpie.plan");
            public static readonly Application VendorXmpiePpkg = new Application("vnd.xmpie.ppkg");
            public static readonly Application VendorXmpieXlim = new Application("vnd.xmpie.xlim");
            public static readonly Application VendorYamahaHv_Dic = new Application("vnd.yamaha.hv-dic", new string[] { "hvd" });
            public static readonly Application VendorYamahaHv_Script = new Application("vnd.yamaha.hv-script", new string[] { "hvs" });
            public static readonly Application VendorYamahaHv_Voice = new Application("vnd.yamaha.hv-voice", new string[] { "hvp" });
            public static readonly Application VendorYamahaOpenscoreformat = new Application("vnd.yamaha.openscoreformat", new string[] { "osf" });
            public static readonly Application VendorYamahaOpenscoreformatOsfpvgXml = new Application("vnd.yamaha.openscoreformat.osfpvg+xml", new string[] { "osfpvg" });
            public static readonly Application VendorYamahaRemote_Setup = new Application("vnd.yamaha.remote-setup");
            public static readonly Application VendorYamahaSmaf_Audio = new Application("vnd.yamaha.smaf-audio", new string[] { "saf" });
            public static readonly Application VendorYamahaSmaf_Phrase = new Application("vnd.yamaha.smaf-phrase", new string[] { "spf" });
            public static readonly Application VendorYamahaThrough_Ngn = new Application("vnd.yamaha.through-ngn");
            public static readonly Application VendorYamahaTunnel_Udpencap = new Application("vnd.yamaha.tunnel-udpencap");
            public static readonly Application VendorYaoweme = new Application("vnd.yaoweme");
            public static readonly Application VendorYellowriver_Custom_Menu = new Application("vnd.yellowriver-custom-menu", new string[] { "cmp" });

            [System.Obsolete("OBSOLETED in favor of video/vnd.youtube.yt")]
            public static readonly Application VendorYoutubeYt = new Application("vnd.youtube.yt");

            public static readonly Application VendorZul = new Application("vnd.zul", new string[] { "zir", "zirz" });
            public static readonly Application VendorZzazzDeckXml = new Application("vnd.zzazz.deck+xml", new string[] { "zaz" });
            public static readonly Application VividenceScriptfile = new Application("vividence.scriptfile");
            public static readonly Application VoicexmlXml = new Application("voicexml+xml", new string[] { "vxml" });
            public static readonly Application Voucher_CmsJson = new Application("voucher-cms+json", new string[] { "json" });
            public static readonly Application Vq_Rtcpxr = new Application("vq-rtcpxr");
            public static readonly Application WatcherinfoXml = new Application("watcherinfo+xml", new string[] { "xml" });
            public static readonly Application Webpush_OptionsJson = new Application("webpush-options+json", new string[] { "json" });
            public static readonly Application Whoispp_Query = new Application("whoispp-query");
            public static readonly Application Whoispp_Response = new Application("whoispp-response");
            public static readonly Application Widget = new Application("widget", new string[] { "wgt" });
            public static readonly Application Winhlp = new Application("winhlp", new string[] { "hlp" });
            public static readonly Application Wita = new Application("wita");
            public static readonly Application Wordperfect51 = new Application("wordperfect5.1");
            public static readonly Application WsdlXml = new Application("wsdl+xml", new string[] { "wsdl" });
            public static readonly Application WspolicyXml = new Application("wspolicy+xml", new string[] { "wspolicy" });
            public static readonly Application X_7z_Compressed = new Application("x-7z-compressed", new string[] { "7z" });
            public static readonly Application X_Abiword = new Application("x-abiword", new string[] { "abw" });
            public static readonly Application X_Ace_Compressed = new Application("x-ace-compressed", new string[] { "ace" });
            public static readonly Application X_Amf = new Application("x-amf");
            public static readonly Application X_Apple_Diskimage = new Application("x-apple-diskimage", new string[] { "dmg" });
            public static readonly Application X_Authorware_Bin = new Application("x-authorware-bin", new string[] { "aab", "x32", "u32", "vox" });
            public static readonly Application X_Authorware_Map = new Application("x-authorware-map", new string[] { "aam" });
            public static readonly Application X_Authorware_Seg = new Application("x-authorware-seg", new string[] { "aas" });
            public static readonly Application X_Bcpio = new Application("x-bcpio", new string[] { "bcpio" });
            public static readonly Application X_Bittorrent = new Application("x-bittorrent", new string[] { "torrent" });
            public static readonly Application X_Blorb = new Application("x-blorb", new string[] { "blb", "blorb" });
            public static readonly Application X_Bzip = new Application("x-bzip", new string[] { "bz" });
            public static readonly Application X_Bzip2 = new Application("x-bzip2", new string[] { "bz2", "boz" });
            public static readonly Application X_Cbr = new Application("x-cbr", new string[] { "cbr", "cba", "cbt", "cbz", "cb7" });
            public static readonly Application X_Cdlink = new Application("x-cdlink", new string[] { "vcd" });
            public static readonly Application X_Cfs_Compressed = new Application("x-cfs-compressed", new string[] { "cfs" });
            public static readonly Application X_Chat = new Application("x-chat", new string[] { "chat" });
            public static readonly Application X_Chess_Pgn = new Application("x-chess-pgn", new string[] { "pgn" });
            public static readonly Application X_Compress = new Application("x-compress");
            public static readonly Application X_Conference = new Application("x-conference", new string[] { "nsc" });
            public static readonly Application X_Cpio = new Application("x-cpio", new string[] { "cpio" });
            public static readonly Application X_Csh = new Application("x-csh", new string[] { "csh" });
            public static readonly Application X_Debian_Package = new Application("x-debian-package", new string[] { "deb", "udeb" });
            public static readonly Application X_Dgc_Compressed = new Application("x-dgc-compressed", new string[] { "dgc" });
            public static readonly Application X_Director = new Application("x-director", new string[] { "dir", "dcr", "dxr", "cst", "cct", "cxt", "w3d", "fgd", "swa" });
            public static readonly Application X_Doom = new Application("x-doom", new string[] { "wad" });
            public static readonly Application X_DtbncxXml = new Application("x-dtbncx+xml", new string[] { "ncx" });
            public static readonly Application X_DtbookXml = new Application("x-dtbook+xml", new string[] { "dtb" });
            public static readonly Application X_DtbresourceXml = new Application("x-dtbresource+xml", new string[] { "res" });
            public static readonly Application X_Dvi = new Application("x-dvi", new string[] { "dvi" });
            public static readonly Application X_Envoy = new Application("x-envoy", new string[] { "evy" });
            public static readonly Application X_Eva = new Application("x-eva", new string[] { "eva" });
            public static readonly Application X_Font_Bdf = new Application("x-font-bdf", new string[] { "bdf" });
            public static readonly Application X_Font_Dos = new Application("x-font-dos");
            public static readonly Application X_Font_Framemaker = new Application("x-font-framemaker");
            public static readonly Application X_Font_Ghostscript = new Application("x-font-ghostscript", new string[] { "gsf" });
            public static readonly Application X_Font_Libgrx = new Application("x-font-libgrx");
            public static readonly Application X_Font_Linux_Psf = new Application("x-font-linux-psf", new string[] { "psf" });
            public static readonly Application X_Font_Pcf = new Application("x-font-pcf", new string[] { "pcf" });
            public static readonly Application X_Font_Snf = new Application("x-font-snf", new string[] { "snf" });
            public static readonly Application X_Font_Speedo = new Application("x-font-speedo");
            public static readonly Application X_Font_Sunos_News = new Application("x-font-sunos-news");
            public static readonly Application X_Font_Type1 = new Application("x-font-type1", new string[] { "pfa", "pfb", "pfm", "afm" });
            public static readonly Application X_Font_Vfont = new Application("x-font-vfont");
            public static readonly Application X_Freearc = new Application("x-freearc", new string[] { "arc" });
            public static readonly Application X_Futuresplash = new Application("x-futuresplash", new string[] { "spl" });
            public static readonly Application X_Gca_Compressed = new Application("x-gca-compressed", new string[] { "gca" });
            public static readonly Application X_Glulx = new Application("x-glulx", new string[] { "ulx" });
            public static readonly Application X_Gnumeric = new Application("x-gnumeric", new string[] { "gnumeric" });
            public static readonly Application X_Gramps_Xml = new Application("x-gramps-xml", new string[] { "gramps" });
            public static readonly Application X_Gtar = new Application("x-gtar", new string[] { "gtar" });
            public static readonly Application X_Gzip = new Application("x-gzip");
            public static readonly Application X_Hdf = new Application("x-hdf", new string[] { "hdf" });
            public static readonly Application X_Install_Instructions = new Application("x-install-instructions", new string[] { "install" });
            public static readonly Application X_Iso9660_Image = new Application("x-iso9660-image", new string[] { "iso" });
            public static readonly Application X_Java_Jnlp_File = new Application("x-java-jnlp-file", new string[] { "jnlp" });
            public static readonly Application X_Latex = new Application("x-latex", new string[] { "latex" });
            public static readonly Application X_Lzh_Compressed = new Application("x-lzh-compressed", new string[] { "lzh", "lha" });
            public static readonly Application X_Mie = new Application("x-mie", new string[] { "mie" });
            public static readonly Application X_Mobipocket_Ebook = new Application("x-mobipocket-ebook", new string[] { "prc", "mobi" });
            public static readonly Application X_Ms_Application = new Application("x-ms-application", new string[] { "application" });
            public static readonly Application X_Ms_Shortcut = new Application("x-ms-shortcut", new string[] { "lnk" });
            public static readonly Application X_Ms_Wmd = new Application("x-ms-wmd", new string[] { "wmd" });
            public static readonly Application X_Ms_Wmz = new Application("x-ms-wmz", new string[] { "wmz" });
            public static readonly Application X_Ms_Xbap = new Application("x-ms-xbap", new string[] { "xbap" });
            public static readonly Application X_Msaccess = new Application("x-msaccess", new string[] { "mdb" });
            public static readonly Application X_Msbinder = new Application("x-msbinder", new string[] { "obd" });
            public static readonly Application X_Mscardfile = new Application("x-mscardfile", new string[] { "crd" });
            public static readonly Application X_Msclip = new Application("x-msclip", new string[] { "clp" });
            public static readonly Application X_Msdownload = new Application("x-msdownload", new string[] { "exe", "dll", "com", "bat", "msi" });
            public static readonly Application X_Msmediaview = new Application("x-msmediaview", new string[] { "mvb", "m13", "m14" });
            public static readonly Application X_Msmetafile = new Application("x-msmetafile", new string[] { "wmf", "wmz", "emf", "emz" });
            public static readonly Application X_Msmoney = new Application("x-msmoney", new string[] { "mny" });
            public static readonly Application X_Mspublisher = new Application("x-mspublisher", new string[] { "pub" });
            public static readonly Application X_Msschedule = new Application("x-msschedule", new string[] { "scd" });
            public static readonly Application X_Msterminal = new Application("x-msterminal", new string[] { "trm" });
            public static readonly Application X_Mswrite = new Application("x-mswrite", new string[] { "wri" });
            public static readonly Application X_Netcdf = new Application("x-netcdf", new string[] { "nc", "cdf" });
            public static readonly Application X_Nzb = new Application("x-nzb", new string[] { "nzb" });
            public static readonly Application X_Pkcs12 = new Application("x-pkcs12", new string[] { "p12", "pfx" });
            public static readonly Application X_Pkcs7_Certificates = new Application("x-pkcs7-certificates", new string[] { "p7b", "spc" });
            public static readonly Application X_Pkcs7_Certreqresp = new Application("x-pkcs7-certreqresp", new string[] { "p7r" });
            public static readonly Application X_Rar_Compressed = new Application("x-rar-compressed", new string[] { "rar" });
            public static readonly Application X_Research_Info_Systems = new Application("x-research-info-systems", new string[] { "ris" });
            public static readonly Application X_Sh = new Application("x-sh", new string[] { "sh" });
            public static readonly Application X_Shar = new Application("x-shar", new string[] { "shar" });
            public static readonly Application X_Shockwave_Flash = new Application("x-shockwave-flash", new string[] { "swf" });
            public static readonly Application X_Silverlight_App = new Application("x-silverlight-app", new string[] { "xap" });
            public static readonly Application X_Sql = new Application("x-sql", new string[] { "sql" });
            public static readonly Application X_Stuffit = new Application("x-stuffit", new string[] { "sit" });
            public static readonly Application X_Stuffitx = new Application("x-stuffitx", new string[] { "sitx" });
            public static readonly Application X_Subrip = new Application("x-subrip", new string[] { "srt" });
            public static readonly Application X_Sv4cpio = new Application("x-sv4cpio", new string[] { "sv4cpio" });
            public static readonly Application X_Sv4crc = new Application("x-sv4crc", new string[] { "sv4crc" });
            public static readonly Application X_T3vm_Image = new Application("x-t3vm-image", new string[] { "t3" });
            public static readonly Application X_Tads = new Application("x-tads", new string[] { "gam" });
            public static readonly Application X_Tar = new Application("x-tar", new string[] { "tar" });
            public static readonly Application X_Tcl = new Application("x-tcl", new string[] { "tcl" });
            public static readonly Application X_Tex = new Application("x-tex", new string[] { "tex" });
            public static readonly Application X_Tex_Tfm = new Application("x-tex-tfm", new string[] { "tfm" });
            public static readonly Application X_Texinfo = new Application("x-texinfo", new string[] { "texinfo", "texi" });
            public static readonly Application X_Tgif = new Application("x-tgif", new string[] { "obj" });
            public static readonly Application X_Ustar = new Application("x-ustar", new string[] { "ustar" });
            public static readonly Application X_Wais_Source = new Application("x-wais-source", new string[] { "src" });
            public static readonly Application X_Www_Form_Urlencoded = new Application("x-www-form-urlencoded");
            public static readonly Application X_X509_Ca_Cert = new Application("x-x509-ca-cert", new string[] { "der", "crt" });
            public static readonly Application X_Xfig = new Application("x-xfig", new string[] { "fig" });
            public static readonly Application X_XliffXml = new Application("x-xliff+xml", new string[] { "xlf" });
            public static readonly Application X_Xpinstall = new Application("x-xpinstall", new string[] { "xpi" });
            public static readonly Application X_Xz = new Application("x-xz", new string[] { "xz" });
            public static readonly Application X_Zmachine = new Application("x-zmachine", new string[] { "z1", "z2", "z3", "z4", "z5", "z6", "z7", "z8" });
            public static readonly Application X400_Bp = new Application("x400-bp");
            public static readonly Application XacmlXml = new Application("xacml+xml", new string[] { "xml" });
            public static readonly Application XamlXml = new Application("xaml+xml", new string[] { "xaml" });
            public static readonly Application Xcap_AttXml = new Application("xcap-att+xml", new string[] { "xml" });
            public static readonly Application Xcap_CapsXml = new Application("xcap-caps+xml", new string[] { "xml" });
            public static readonly Application Xcap_DiffXml = new Application("xcap-diff+xml", new string[] { "xdf" });
            public static readonly Application Xcap_ElXml = new Application("xcap-el+xml", new string[] { "xml" });
            public static readonly Application Xcap_ErrorXml = new Application("xcap-error+xml", new string[] { "xml" });
            public static readonly Application Xcap_NsXml = new Application("xcap-ns+xml", new string[] { "xml" });
            public static readonly Application Xcon_Conference_Info_DiffXml = new Application("xcon-conference-info-diff+xml", new string[] { "xml" });
            public static readonly Application Xcon_Conference_InfoXml = new Application("xcon-conference-info+xml", new string[] { "xml" });
            public static readonly Application XencXml = new Application("xenc+xml", new string[] { "xenc" });
            public static readonly Application Xhtml_VoiceXml = new Application("xhtml-voice+xml", new string[] { "xml" });
            public static readonly Application XhtmlXml = new Application("xhtml+xml", new string[] { "xhtml", "xht" });
            public static readonly Application XliffXml = new Application("xliff+xml", new string[] { "xml" });
            public static readonly Application Xml = new Application("xml", new string[] { "xml", "xsl" });
            public static readonly Application Xml_Dtd = new Application("xml-dtd", new string[] { "dtd" });
            public static readonly Application Xml_External_Parsed_Entity = new Application("xml-external-parsed-entity");
            public static readonly Application Xml_PatchXml = new Application("xml-patch+xml", new string[] { "xml" });
            public static readonly Application XmppXml = new Application("xmpp+xml", new string[] { "xml" });
            public static readonly Application XopXml = new Application("xop+xml", new string[] { "xop" });
            public static readonly Application XprocXml = new Application("xproc+xml", new string[] { "xpl" });
            public static readonly Application XsltXml = new Application("xslt+xml", new string[] { "xslt" });
            public static readonly Application XspfXml = new Application("xspf+xml", new string[] { "xspf" });
            public static readonly Application XvXml = new Application("xv+xml", new string[] { "mxml", "xhvml", "xvml", "xvm" });
            public static readonly Application Yang = new Application("yang", new string[] { "yang" });
            public static readonly Application Yang_DataJson = new Application("yang-data+json", new string[] { "json" });
            public static readonly Application Yang_DataXml = new Application("yang-data+xml", new string[] { "xml" });
            public static readonly Application Yang_PatchJson = new Application("yang-patch+json", new string[] { "json" });
            public static readonly Application Yang_PatchXml = new Application("yang-patch+xml", new string[] { "xml" });
            public static readonly Application YinXml = new Application("yin+xml", new string[] { "yin" });
            public static readonly Application Zip = new Application("zip", new string[] { "zip" });
            public static readonly Application Zlib = new Application("zlib");
            public static readonly Application Zstd = new Application("zstd");

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
                Isup,
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
                Sgml,
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
                VendorAfpcModca,
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
