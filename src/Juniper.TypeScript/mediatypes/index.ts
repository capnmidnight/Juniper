import { isDefined, isString, singleton } from "juniper-tslib";

export interface MediaType {
    typeName: string;
    subTypeName: string;
    value: string;
    fullValue: string;
    extensions: ReadonlyArray<string>;
    primaryExtension: string;

    withParameter(key: string, value: string): MediaType;

    toString(): string;
    addExtension(fileName: string): string;
    matches(value: MediaType | string): boolean;
    matchesFileName(fileName: string): boolean;
}


const byValue = singleton("Juniper:MediaTypes:byValue", () => new Map<string, MediaType>());
const byExtension = singleton("Juniper:MediaTypes:byExtension", () => new Map<string, MediaType[]>());
const depMessages = singleton("Juniper:MediaTypes:depMessages", () => new WeakMap<MediaType, string>());

function register(type: MediaType): MediaType {
    let isNew = false;
    type = singleton("Juniper.MediaTypes:" + type.value, () => {
        isNew = true;
        return type;
    });

    if (isNew) {
        byValue.set(type.value, type);

        for (const ext of type.extensions) {
            if (!byExtension.has(ext)) {
                byExtension.set(ext, new Array<MediaType>());
            }

            const byExts = byExtension.get(ext);
            if (byExts.indexOf(type) < 0) {
                byExts.push(type);
            }
        }
    }

    return type;
}

function deprecate(type: MediaType, msg: string): MediaType {
    depMessages.set(type, msg);
    return type;
}

const subTypePattern = /(?:([^\.]+)\.)?([^\+;]+)(?:\+([^;]+))?((?:; *([^=]+)=([^;]+))*)/;
class InternalMediaType implements MediaType {
    private readonly _tree: string;
    private readonly _subType: string;
    private readonly _suffix: string;
    private readonly _parameters: ReadonlyMap<string, string>;

    private readonly _value: string;
    private readonly _fullValue: string;

    private readonly _extensions: ReadonlyArray<string>;
    private readonly _primaryExtension: string = null;


    constructor(
        private readonly _type: string,
        private readonly _fullSubType: string,
        extensions?: ReadonlyArray<string>) {

        const parameters = new Map<string, string>();
        this._parameters = parameters;

        const subTypeParts = this._fullSubType.match(subTypePattern);
        this._tree = subTypeParts[1];
        this._subType = subTypeParts[2];
        this._suffix = subTypeParts[3];
        const paramStr = subTypeParts[4];

        this._value = this._fullValue = this._type + "/";

        if (isDefined(this._tree)) {
            this._value = this._fullValue += this._tree + ".";
        }

        this._value = this._fullValue += this._subType;

        if (isDefined(this._suffix)) {
            this._value = this._fullValue += "+" + this._suffix;
        }

        if (isDefined(paramStr)) {
            const pairs = paramStr.split(';')
                .map(p => p.trim())
                .filter(p => p.length > 0)
                .map(p => p.split('='));
            for (const [key, ...values] of pairs) {
                const value = values.join("=");
                parameters.set(key, value);
                const slug = `; ${key}=${value}`;
                this._fullValue += slug;
                if (key !== "q") {
                    this._value += slug;
                }
            }
        }

        this._extensions = extensions || [];
        this._primaryExtension = this._extensions[0] || null;
    }

    private check() {
        const msg = depMessages.get(this);
        if (msg) {
            console.warn(`${this._value} is deprecated ${msg}`);
        }
    }

    withParameter(key: string, value: string): MediaType {
        const newSubType = `${this._fullSubType}; ${key}=${value}`;
        const type = new InternalMediaType(this.typeName, newSubType, this.extensions);
        const msg = depMessages.get(this);
        if (msg) {
            depMessages.set(type, msg);
        }
        return type;
    }

    get typeName(): string {
        this.check();
        return this._type;
    }

    get tree(): string {
        this.check();
        return this._tree;
    }

    get suffix(): string {
        return this._suffix;
    }

    get subTypeName(): string {
        this.check();
        return this._subType;
    }

    get value(): string {
        this.check();
        return this._value;
    }

    get fullValue(): string {
        this.check();
        return this._fullValue;
    }

    get parameters(): ReadonlyMap<string, string> {
        this.check();
        return this._parameters;
    }

    get extensions(): ReadonlyArray<string> {
        this.check();
        return this._extensions;
    }

    get primaryExtension(): string {
        this.check();
        return this._primaryExtension;
    }

    toString() {
        if (this.parameters.get("q") === "1") {
            return this.value;
        }
        else {
            return this.fullValue;
        }
    }

    addExtension(fileName: string): string {
        if (!fileName) {
            throw new Error("File name is not defined");
        }

        if (this.primaryExtension) {
            const idx = fileName.lastIndexOf(".");
            if (idx > -1) {
                const currentExtension = fileName.substring(idx + 1);;
                if (this.extensions.indexOf(currentExtension) > -1) {
                    fileName = fileName.substring(0, idx);
                }
            }

            fileName = `${fileName}.${this.primaryExtension}`;
        }

        return fileName;
    }

    matches(value: MediaType | string): boolean {
        if (isString(value)) {
            value = mediaTypeParse(value);
        }

        return (this.typeName === "*" && this.subTypeName === "*")
            || (this.typeName === value.typeName && (this.subTypeName === "*" || this.subTypeName === value.subTypeName));
    }

    matchesFileName(fileName: string): boolean {
        if (!fileName) {
            return false;
        }

        const types = mediaTypeGuessByFileName(fileName);
        for (const type of types) {
            if (mediaTypesMatch(type, this)) {
                return true;
            }
        }

        return false;
    }
}

const typePattern = /([^\/]+)\/(.+)/;
export function mediaTypeParse(value: string): MediaType {
    if (!value) {
        return null;
    }

    const match = value.match(typePattern);
    if (!match) {
        return null;
    }

    const type = match[1];
    const subType = match[2];
    const parsedType = new InternalMediaType(type, subType);
    const weight = parsedType.parameters.get("q");
    const basicType = byValue.get(parsedType.value) || new InternalMediaType(type, subType, []);

    if (isDefined(weight)) {
        return basicType.withParameter("q", weight);
    }
    else {
        return basicType;
    }
}

export function mediaTypesMatch(value: string | MediaType, pattern: string | MediaType): boolean {
    if (isString(value)) {
        value = mediaTypeParse(value);
    }

    if (isString(pattern)) {
        pattern = mediaTypeParse(pattern);
    }

    return pattern.matches(value);
}

export function mediaTypeGuessByFileName(fileName: string): MediaType[] {
    if (!fileName) {
        console.warn("Couldn't guess media type. Must provide a valid fileName.");
        return [];
    }

    const idx = fileName.lastIndexOf(".");
    if (idx === -1) {
        console.warn("Couldn't guess media type. FileName has no extension.");
        return [];
    }

    const ext = fileName.substring(idx);
    return mediaTypeGuessByExtension(ext);
}

export function mediaTypeGuessByExtension(ext: string): MediaType[] {
    if (!ext) {
        ext = "unknown";
    }
    else if (ext[0] == '.') {
        ext = ext.substring(1);
    }

    if (byExtension.has(ext)) {
        return byExtension.get(ext);
    }
    else {
        return [new InternalMediaType("unknown", ext, [ext])];
    }
}

export function mediaTypeNormalizeFileName(fileName: string, fileType: string): string {
    if (!fileType && fileName.indexOf(".") > -1) {
        const guesses = mediaTypeGuessByFileName(fileName);
        if (guesses.length > 0) {
            fileType = guesses[0].value;
        }
    }

    return fileType;
}

const specialize = (group: string) =>
    (value: string, ...extensions: string[]) =>
        register(new InternalMediaType(group, value, extensions));

const application = specialize("application");
const audio = specialize("audio");
const chemical = specialize("chemical");
const font = specialize("font");
const image = specialize("image");
const message = specialize("message");
const model = specialize("model");
const multipart = specialize("multipart");
const text = specialize("text");
const video = specialize("video");
const xConference = specialize("xconference");
const xShader = specialize("x-shader");

export const anyApplication = application("*");
export const Application_A2L = application("a2l");
export const Application_Activemessage = application("activemessage");
export const Application_ScenarioJson = application("activity+json", "json");
export const Application_Alto_CostmapfilterJson = application("alto-costmapfilter+json", "json");
export const Application_Alto_CostmapJson = application("alto-costmap+json", "json");
export const Application_Alto_DirectoryJson = application("alto-directory+json", "json");
export const Application_Alto_EndpointcostJson = application("alto-endpointcost+json", "json");
export const Application_Alto_EndpointcostparamsJson = application("alto-endpointcostparams+json", "json");
export const Application_Alto_EndpointpropJson = application("alto-endpointprop+json", "json");
export const Application_Alto_EndpointpropparamsJson = application("alto-endpointpropparams+json", "json");
export const Application_Alto_ErrorJson = application("alto-error+json", "json");
export const Application_Alto_NetworkmapfilterJson = application("alto-networkmapfilter+json", "json");
export const Application_Alto_NetworkmapJson = application("alto-networkmap+json", "json");
export const Application_AML = application("aml");
export const Application_Andrew_Inset = application("andrew-inset", "ez");
export const Application_Applefile = application("applefile");
export const Application_Applixware = application("applixware", "aw");
export const Application_ATF = application("atf");
export const Application_ATFX = application("atfx");
export const Application_AtomcatXml = application("atomcat+xml", "atomcat");
export const Application_AtomdeletedXml = application("atomdeleted+xml", "xml");
export const Application_Atomicmail = application("atomicmail");
export const Application_AtomsvcXml = application("atomsvc+xml", "atomsvc");
export const Application_AtomXml = application("atom+xml", "atom");
export const Application_Atsc_DwdXml = application("atsc-dwd+xml", "xml");
export const Application_Atsc_HeldXml = application("atsc-held+xml", "xml");
export const Application_Atsc_RdtJson = application("atsc-rdt+json", "json");
export const Application_Atsc_RsatXml = application("atsc-rsat+xml", "xml");
export const Application_ATXML = application("atxml");
export const Application_Auth_PolicyXml = application("auth-policy+xml", "xml");
export const Application_Bacnet_XddZip = application("bacnet-xdd+zip", "zip");
export const Application_Batch_SMTP = application("batch-smtp");
export const Application_Bdoc = application("bdoc", "bdoc"); // Digital signature container
export const Application_BeepXml = application("beep+xml", "xml");
export const Application_CalendarJson = application("calendar+json", "json");
export const Application_CalendarXml = application("calendar+xml", "xml");
export const Application_Call_Completion = application("call-completion");
export const Application_CALS_1840 = application("cals-1840");
export const Application_Cbor = application("cbor");
export const Application_Cbor_Seq = application("cbor-seq");
export const Application_Cccex = application("cccex");
export const Application_CcmpXml = application("ccmp+xml", "xml");
export const Application_CcxmlXml = application("ccxml+xml", "ccxml");
export const Application_CDFXXML = application("cdfx+xml", "xml");
export const Application_Cdmi_Capability = application("cdmi-capability", "cdmia");
export const Application_Cdmi_Container = application("cdmi-container", "cdmic");
export const Application_Cdmi_Domain = application("cdmi-domain", "cdmid");
export const Application_Cdmi_Object = application("cdmi-object", "cdmio");
export const Application_Cdmi_Queue = application("cdmi-queue", "cdmiq");
export const Application_Cdni = application("cdni");
export const Application_CEA = application("cea");
export const Application_Cea_2018Xml = application("cea-2018+xml", "xml");
export const Application_CellmlXml = application("cellml+xml", "xml");
export const Application_Cfw = application("cfw");
export const Application_Clue_infoXml = application("clue_info+xml", "xml");
export const Application_ClueXml = application("clue+xml", "xml");
export const Application_Cms = application("cms");
export const Application_CnrpXml = application("cnrp+xml", "xml");
export const Application_Coap_GroupJson = application("coap-group+json", "json");
export const Application_Coap_Payload = application("coap-payload");
export const Application_Commonground = application("commonground");
export const Application_Conference_InfoXml = application("conference-info+xml", "xml");
export const Application_Cose = application("cose");
export const Application_Cose_Key = application("cose-key");
export const Application_Cose_Key_Set = application("cose-key-set");
export const Application_CplXml = application("cpl+xml", "xml");
export const Application_Csrattrs = application("csrattrs");
export const Application_CSTAdataXml = application("cstadata+xml", "xml");
export const Application_CstaXml = application("csta+xml", "xml");
export const Application_CsvmJson = application("csvm+json", "json");
export const Application_Cu_Seeme = application("cu-seeme", "cu");
export const Application_Cwt = application("cwt");
export const Application_Cybercash = application("cybercash");
export const Application_Dart = application("dart");
export const Application_Dashdelta = application("dashdelta");
export const Application_DashXml = application("dash+xml", "xml");
export const Application_DavmountXml = application("davmount+xml", "davmount");
export const Application_Dca_Rft = application("dca-rft");
export const Application_DCD = application("dcd");
export const Application_Dec_Dx = application("dec-dx");
export const Application_Dialog_InfoXml = application("dialog-info+xml", "xml");
export const Application_Dicom = application("dicom");
export const Application_DicomJson = application("dicom+json", "json");
export const Application_DicomXml = application("dicom+xml", "xml");
export const Application_DII = application("dii");
export const Application_DIT = application("dit");
export const Application_Dns = application("dns");
export const Application_Dns_Message = application("dns-message");
export const Application_DnsJson = application("dns+json", "json");
export const Application_DocbookXml = application("docbook+xml", "dbk");
export const Application_DotsCbor = application("dots+cbor", "cbor");
export const Application_DskppXml = application("dskpp+xml", "xml");
export const Application_DsscDer = application("dssc+der", "dssc");
export const Application_DsscXml = application("dssc+xml", "xdssc");
export const Application_Dvcs = application("dvcs");
export const Application_Ecmascript = application("ecmascript", "ecma");
export const Application_EDI_Consent = application("edi-consent");
export const Application_EDI_X12 = application("edi-x12");
export const Application_EDIFACT = application("edifact");
export const Application_Efi = application("efi");
export const Application_EmergencyCallDataCommentXml = application("emergencycalldata.comment+xml", "xml");
export const Application_EmergencyCallDataControlXml = application("emergencycalldata.control+xml", "xml");
export const Application_EmergencyCallDataDeviceInfoXml = application("emergencycalldata.deviceinfo+xml", "xml");
export const Application_EmergencyCallDataECallMSD = application("emergencycalldata.ecall.msd");
export const Application_EmergencyCallDataProviderInfoXml = application("emergencycalldata.providerinfo+xml", "xml");
export const Application_EmergencyCallDataServiceInfoXml = application("emergencycalldata.serviceinfo+xml", "xml");
export const Application_EmergencyCallDataSubscriberInfoXml = application("emergencycalldata.subscriberinfo+xml", "xml");
export const Application_EmergencyCallDataVEDSXml = application("emergencycalldata.veds+xml", "xml");
export const Application_EmmaXml = application("emma+xml", "emma");
export const Application_EmotionmlXml = application("emotionml+xml", "xml");
export const Application_Encaprtp = application("encaprtp");
export const Application_EppXml = application("epp+xml", "xml");
export const Application_EpubZip = application("epub+zip", "epub");
export const Application_Eshop = application("eshop");
export const Application_Example = application("example");
export const Application_Exi = application("exi", "exi");
export const Application_Expect_Ct_ReportJson = application("expect-ct-report+json", "json");
export const Application_Fastinfoset = application("fastinfoset");
export const Application_Fastsoap = application("fastsoap");
export const Application_FdtXml = application("fdt+xml", "xml");
export const Application_FhirJson = application("fhir+json", "json");
export const Application_FhirXml = application("fhir+xml", "xml");
export const Application_Fido_TrustedAppsJson = application("fido.trusted-apps+json");
export const Application_Fits = application("fits");
export const Application_Flexfec = application("flexfec");
export const Application_Font_Sfnt = deprecate(application("font-sfnt"), "in favor of font/sfnt");
export const Application_Font_Tdpfr = application("font-tdpfr", "pfr");
export const Application_Font_Woff = deprecate(application("font-woff"), "in favor of font/woff");
export const Application_Framework_AttributesXml = application("framework-attributes+xml", "xml");
export const Application_GeoJson = application("geo+json", "json");
export const Application_GeoJson_Seq = application("geo+json-seq", "json-seq");
export const Application_GeopackageSqlite3 = application("geopackage+sqlite3", "sqlite3");
export const Application_GeoxacmlXml = application("geoxacml+xml", "xml");
export const Application_Gltf_Buffer = application("gltf-buffer");
export const Application_GmlXml = application("gml+xml", "gml");
export const Application_GpxXml = application("gpx+xml", "gpx");
export const Application_Gxf = application("gxf", "gxf");
export const Application_Gzip = application("gzip");
export const Application_H224 = application("h224");
export const Application_HeldXml = application("held+xml", "xml");
export const Application_Hjson = application("hjson", "hjson");
export const Application_Http = application("http");
export const Application_Hyperstudio = application("hyperstudio", "stk");
export const Application_Ibe_Key_RequestXml = application("ibe-key-request+xml", "xml");
export const Application_Ibe_Pkg_ReplyXml = application("ibe-pkg-reply+xml", "xml");
export const Application_Ibe_Pp_Data = application("ibe-pp-data");
export const Application_Iges = application("iges");
export const Application_Im_IscomposingXml = application("im-iscomposing+xml", "xml");
export const Application_Index = application("index");
export const Application_IndexCmd = application("index.cmd");
export const Application_IndexObj = application("index.obj");
export const Application_IndexResponse = application("index.response");
export const Application_IndexVnd = application("index.vnd");
export const Application_InkmlXml = application("inkml+xml", "ink", "inkml");
export const Application_IOTP = application("iotp");
export const Application_Ipfix = application("ipfix", "ipfix");
export const Application_Ipp = application("ipp");
export const Application_ISUP = application("isup");
export const Application_ItsXml = application("its+xml", "xml");
export const Application_Java_Archive = application("java-archive", "jar");
export const Application_Java_Serialized_Object = application("java-serialized-object", "ser");
export const Application_Java_Vm = application("java-vm", "class");
export const Application_Javascript = application("javascript", "js");
export const Application_Jf2feedJson = application("jf2feed+json", "json");
export const Application_Jose = application("jose");
export const Application_JoseJson = application("jose+json", "json");
export const Application_JrdJson = application("jrd+json", "json");
export const Application_Json = application("json", "json");
export const Application_Json5 = application("json5", "json5");
export const Application_JsonUTF8 = application("json; charset=UTF-8", "json");
export const Application_Json_PatchJson = application("json-patch+json", "json");
export const Application_Json_Seq = application("json-seq");
export const Application_JsonmlJson = application("jsonml+json", "jsonml");
export const Application_Jwk_SetJson = application("jwk-set+json", "json");
export const Application_JwkJson = application("jwk+json", "json");
export const Application_Jwt = application("jwt");
export const Application_Kpml_RequestXml = application("kpml-request+xml", "xml");
export const Application_Kpml_ResponseXml = application("kpml-response+xml", "xml");
export const Application_LdJson = application("ld+json", "json");
export const Application_LgrXml = application("lgr+xml", "xml");
export const Application_Link_Format = application("link-format");
export const Application_Load_ControlXml = application("load-control+xml", "xml");
export const Application_LostsyncXml = application("lostsync+xml", "xml");
export const Application_LostXml = application("lost+xml", "lostxml");
export const Application_LXF = application("lxf");
export const Application_Mac_Binhex40 = application("mac-binhex40", "hqx");
export const Application_Mac_Compactpro = application("mac-compactpro", "cpt");
export const Application_Macwriteii = application("macwriteii");
export const Application_MadsXml = application("mads+xml", "mads");
export const Application_ManifestJson = application("manifest+json");
export const Application_Marc = application("marc", "mrc");
export const Application_MarcxmlXml = application("marcxml+xml", "mrcx");
export const Application_Mathematica = application("mathematica", "ma", "nb", "mb");
export const Application_Mathml_ContentXml = application("mathml-content+xml", "xml");
export const Application_Mathml_PresentationXml = application("mathml-presentation+xml", "xml");
export const Application_MathmlXml = application("mathml+xml", "mathml");
export const Application_Mbms_Associated_Procedure_DescriptionXml = application("mbms-associated-procedure-description+xml", "xml");
export const Application_Mbms_DeregisterXml = application("mbms-deregister+xml", "xml");
export const Application_Mbms_EnvelopeXml = application("mbms-envelope+xml", "xml");
export const Application_Mbms_Msk_ResponseXml = application("mbms-msk-response+xml", "xml");
export const Application_Mbms_MskXml = application("mbms-msk+xml", "xml");
export const Application_Mbms_Protection_DescriptionXml = application("mbms-protection-description+xml", "xml");
export const Application_Mbms_Reception_ReportXml = application("mbms-reception-report+xml", "xml");
export const Application_Mbms_Register_ResponseXml = application("mbms-register-response+xml", "xml");
export const Application_Mbms_RegisterXml = application("mbms-register+xml", "xml");
export const Application_Mbms_ScheduleXml = application("mbms-schedule+xml", "xml");
export const Application_Mbms_User_Service_DescriptionXml = application("mbms-user-service-description+xml", "xml");
export const Application_Mbox = application("mbox", "mbox");
export const Application_Media_controlXml = application("media_control+xml", "xml");
export const Application_Media_Policy_DatasetXml = application("media-policy-dataset+xml", "xml");
export const Application_MediaservercontrolXml = application("mediaservercontrol+xml", "mscml");
export const Application_Merge_PatchJson = application("merge-patch+json", "json");
export const Application_Metalink4Xml = application("metalink4+xml", "meta4");
export const Application_MetalinkXml = application("metalink+xml", "metalink");
export const Application_MetsXml = application("mets+xml", "mets");
export const Application_MF4 = application("mf4");
export const Application_Mikey = application("mikey");
export const Application_Mipc = application("mipc");
export const Application_Mmt_AeiXml = application("mmt-aei+xml", "xml");
export const Application_Mmt_UsdXml = application("mmt-usd+xml", "xml");
export const Application_ModsXml = application("mods+xml", "mods");
export const Application_Moss_Keys = application("moss-keys");
export const Application_Moss_Signature = application("moss-signature");
export const Application_Mosskey_Data = application("mosskey-data");
export const Application_Mosskey_Request = application("mosskey-request");
export const Application_Mp21 = application("mp21", "m21", "mp21");
export const Application_Mp4 = application("mp4", "mp4s");
export const Application_Mpeg4_Generic = application("mpeg4-generic");
export const Application_Mpeg4_Iod = application("mpeg4-iod");
export const Application_Mpeg4_Iod_Xmt = application("mpeg4-iod-xmt");
export const Application_Mrb_ConsumerXml = application("mrb-consumer+xml", "xml");
export const Application_Mrb_PublishXml = application("mrb-publish+xml", "xml");
export const Application_Msc_IvrXml = application("msc-ivr+xml", "xml");
export const Application_Msc_MixerXml = application("msc-mixer+xml", "xml");
export const Application_Msword = application("msword", "doc", "dot");
export const Application_MudJson = application("mud+json", "json");
export const Application_Multipart_Core = application("multipart-core");
export const Application_Mxf = application("mxf", "mxf");
export const Application_N_Quads = application("n-quads");
export const Application_N_Triples = application("n-triples");
export const Application_Nasdata = application("nasdata");
export const Application_News_Checkgroups = application("news-checkgroups");
export const Application_News_Groupinfo = application("news-groupinfo");
export const Application_News_Transmission = application("news-transmission");
export const Application_NlsmlXml = application("nlsml+xml", "xml");
export const Application_Node = application("node");
export const Application_Nss = application("nss");
export const Application_Ocsp_Request = application("ocsp-request");
export const Application_Ocsp_Response = application("ocsp-response");
export const Application_Octet_Stream = application("octet-stream", "bin", "dms", "lrf", "mar", "so", "dist", "distz", "pkg", "bpk", "dump", "elc", "deploy");
export const Application_ODA = application("oda", "oda");
export const Application_OdmXml = application("odm+xml", "xml");
export const Application_ODX = application("odx");
export const Application_Oebps_PackageXml = application("oebps-package+xml", "opf");
export const Application_Ogg = application("ogg", "ogx");
export const Application_OmdocXml = application("omdoc+xml", "omdoc");
export const Application_Onenote = application("onenote", "onetoc", "onetoc2", "onetmp", "onepkg");
export const Application_Oscore = application("oscore");
export const Application_Oxps = application("oxps", "oxps");
export const Application_P2p_OverlayXml = application("p2p-overlay+xml", "xml");
export const Application_Parityfec = application("parityfec");
export const Application_Passport = application("passport");
export const Application_Patch_Ops_ErrorXml = application("patch-ops-error+xml", "xer");
export const Application_Pdf = application("pdf", "pdf");
export const Application_PDX = application("pdx");
export const Application_Pem_Certificate_Chain = application("pem-certificate-chain");
export const Application_Pgp_Encrypted = application("pgp-encrypted", "pgp");
export const Application_Pgp_Keys = application("pgp-keys");
export const Application_Pgp_Signature = application("pgp-signature", "asc", "sig");
export const Application_Pics_Rules = application("pics-rules", "prf");
export const Application_Pidf_DiffXml = application("pidf-diff+xml", "xml");
export const Application_PidfXml = application("pidf+xml", "xml");
export const Application_Pkcs10 = application("pkcs10", "p10");
export const Application_Pkcs12 = application("pkcs12");
export const Application_Pkcs7_Mime = application("pkcs7-mime", "p7m", "p7c");
export const Application_Pkcs7_Signature = application("pkcs7-signature", "p7s");
export const Application_Pkcs8 = application("pkcs8", "p8");
export const Application_Pkcs8_Encrypted = application("pkcs8-encrypted");
export const Application_Pkix_Attr_Cert = application("pkix-attr-cert", "ac");
export const Application_Pkix_Cert = application("pkix-cert", "cer");
export const Application_Pkix_Crl = application("pkix-crl", "crl");
export const Application_Pkix_Pkipath = application("pkix-pkipath", "pkipath");
export const Application_Pkixcmp = application("pkixcmp", "pki");
export const Application_PlsXml = application("pls+xml", "pls");
export const Application_Poc_SettingsXml = application("poc-settings+xml", "xml");
export const Application_Postscript = application("postscript", "ai", "eps", "ps");
export const Application_Ppsp_TrackerJson = application("ppsp-tracker+json", "json");
export const Application_ProblemJson = application("problem+json", "json");
export const Application_ProblemXml = application("problem+xml", "xml");
export const Application_ProvenanceXml = application("provenance+xml", "xml");
export const Application_PrsAlvestrandTitrax_Sheet = application("prs.alvestrand.titrax-sheet");
export const Application_PrsCww = application("prs.cww", "cww");
export const Application_PrsHpubZip = application("prs.hpub+zip", "zip");
export const Application_PrsNprend = application("prs.nprend");
export const Application_PrsPlucker = application("prs.plucker");
export const Application_PrsRdf_Xml_Crypt = application("prs.rdf-xml-crypt");
export const Application_PrsXsfXml = application("prs.xsf+xml", "xml");
export const Application_PskcXml = application("pskc+xml", "pskcxml");
export const Application_QSIG = application("qsig");
export const Application_RamlYaml = application("raml+yaml", "raml");
export const Application_Raptorfec = application("raptorfec");
export const Application_RdapJson = application("rdap+json", "json");
export const Application_RdfXml = application("rdf+xml", "rdf");
export const Application_ReginfoXml = application("reginfo+xml", "rif");
export const Application_Relax_Ng_Compact_Syntax = application("relax-ng-compact-syntax", "rnc");
export const Application_Remote_Printing = application("remote-printing");
export const Application_ReputonJson = application("reputon+json", "json");
export const Application_Resource_Lists_DiffXml = application("resource-lists-diff+xml", "rld");
export const Application_Resource_ListsXml = application("resource-lists+xml", "rl");
export const Application_RfcXml = application("rfc+xml", "xml");
export const Application_Riscos = application("riscos");
export const Application_RlmiXml = application("rlmi+xml", "xml");
export const Application_Rls_ServicesXml = application("rls-services+xml", "rs");
export const Application_Route_ApdXml = application("route-apd+xml", "xml");
export const Application_Route_S_TsidXml = application("route-s-tsid+xml", "xml");
export const Application_Route_UsdXml = application("route-usd+xml", "xml");
export const Application_Rpki_Ghostbusters = application("rpki-ghostbusters", "gbr");
export const Application_Rpki_Manifest = application("rpki-manifest", "mft");
export const Application_Rpki_Publication = application("rpki-publication");
export const Application_Rpki_Roa = application("rpki-roa", "roa");
export const Application_Rpki_Updown = application("rpki-updown");
export const Application_RsdXml = application("rsd+xml", "rsd");
export const Application_RssXml = application("rss+xml", "rss");
export const Application_Rtf = application("rtf", "rtf");
export const Application_Rtploopback = application("rtploopback");
export const Application_Rtx = application("rtx");
export const Application_SamlassertionXml = application("samlassertion+xml", "xml");
export const Application_SamlmetadataXml = application("samlmetadata+xml", "xml");
export const Application_SbmlXml = application("sbml+xml", "sbml");
export const Application_ScaipXml = application("scaip+xml", "xml");
export const Application_ScimJson = application("scim+json", "json");
export const Application_Scvp_Cv_Request = application("scvp-cv-request", "scq");
export const Application_Scvp_Cv_Response = application("scvp-cv-response", "scs");
export const Application_Scvp_Vp_Request = application("scvp-vp-request", "spq");
export const Application_Scvp_Vp_Response = application("scvp-vp-response", "spp");
export const Application_Sdp = application("sdp", "sdp");
export const Application_SeceventJwt = application("secevent+jwt", "jwt");
export const Application_Senml_Exi = application("senml-exi");
export const Application_SenmlCbor = application("senml+cbor", "cbor");
export const Application_SenmlJson = application("senml+json", "json");
export const Application_SenmlXml = application("senml+xml", "xml");
export const Application_Sensml_Exi = application("sensml-exi");
export const Application_SensmlCbor = application("sensml+cbor", "cbor");
export const Application_SensmlJson = application("sensml+json", "json");
export const Application_SensmlXml = application("sensml+xml", "xml");
export const Application_Sep_Exi = application("sep-exi");
export const Application_SepXml = application("sep+xml", "xml");
export const Application_Session_Info = application("session-info");
export const Application_Set_Payment = application("set-payment");
export const Application_Set_Payment_Initiation = application("set-payment-initiation", "setpay");
export const Application_Set_Registration = application("set-registration");
export const Application_Set_Registration_Initiation = application("set-registration-initiation", "setreg");
export const Application_SGML = application("sgml");
export const Application_Sgml_Open_Catalog = application("sgml-open-catalog");
export const Application_ShfXml = application("shf+xml", "shf");
export const Application_Sieve = application("sieve");
export const Application_Simple_FilterXml = application("simple-filter+xml", "xml");
export const Application_Simple_Message_Summary = application("simple-message-summary");
export const Application_SimpleSymbolContainer = application("simplesymbolcontainer");
export const Application_Sipc = application("sipc");
export const Application_Slate = application("slate");
export const Application_Smil = deprecate(application("smil"), "in favor of application/smil+xml");
export const Application_SmilXml = application("smil+xml", "smi", "smil");
export const Application_Smpte336m = application("smpte336m");
export const Application_SoapFastinfoset = application("soap+fastinfoset", "fastinfoset");
export const Application_SoapXml = application("soap+xml", "xml");
export const Application_Sparql_Query = application("sparql-query", "rq");
export const Application_Sparql_ResultsXml = application("sparql-results+xml", "srx");
export const Application_Spirits_EventXml = application("spirits-event+xml", "xml");
export const Application_Sql = application("sql");
export const Application_Srgs = application("srgs", "gram");
export const Application_SrgsXml = application("srgs+xml", "grxml");
export const Application_SruXml = application("sru+xml", "sru");
export const Application_SsdlXml = application("ssdl+xml", "ssdl");
export const Application_SsmlXml = application("ssml+xml", "ssml");
export const Application_StixJson = application("stix+json", "json");
export const Application_SwidXml = application("swid+xml", "xml");
export const Application_Tamp_Apex_Update = application("tamp-apex-update");
export const Application_Tamp_Apex_Update_Confirm = application("tamp-apex-update-confirm");
export const Application_Tamp_Community_Update = application("tamp-community-update");
export const Application_Tamp_Community_Update_Confirm = application("tamp-community-update-confirm");
export const Application_Tamp_Error = application("tamp-error");
export const Application_Tamp_Sequence_Adjust = application("tamp-sequence-adjust");
export const Application_Tamp_Sequence_Adjust_Confirm = application("tamp-sequence-adjust-confirm");
export const Application_Tamp_Status_Query = application("tamp-status-query");
export const Application_Tamp_Status_Response = application("tamp-status-response");
export const Application_Tamp_Update = application("tamp-update");
export const Application_Tamp_Update_Confirm = application("tamp-update-confirm");
export const Application_Tar = application("tar");
export const Application_TaxiiJson = application("taxii+json", "json");
export const Application_TeiXml = application("tei+xml", "tei", "teicorpus");
export const Application_TETRA_ISI = application("tetra_isi");
export const Application_ThraudXml = application("thraud+xml", "tfi");
export const Application_Timestamp_Query = application("timestamp-query");
export const Application_Timestamp_Reply = application("timestamp-reply");
export const Application_Timestamped_Data = application("timestamped-data", "tsd");
export const Application_TlsrptGzip = application("tlsrpt+gzip", "gzip");
export const Application_TlsrptJson = application("tlsrpt+json", "json");
export const Application_Tnauthlist = application("tnauthlist");
export const Application_Toml = application("toml", "toml");
export const Application_Trickle_Ice_Sdpfrag = application("trickle-ice-sdpfrag");
export const Application_Trig = application("trig");
export const Application_TtmlXml = application("ttml+xml", "xml");
export const Application_Tve_Trigger = application("tve-trigger");
export const Application_Tzif = application("tzif");
export const Application_Tzif_Leap = application("tzif-leap");
export const Application_Ubjson = application("ubjson", "ubj");
export const Application_Ulpfec = application("ulpfec");
export const Application_Urc_GrpsheetXml = application("urc-grpsheet+xml", "xml");
export const Application_Urc_RessheetXml = application("urc-ressheet+xml", "xml");
export const Application_Urc_TargetdescXml = application("urc-targetdesc+xml", "xml");
export const Application_Urc_UisocketdescXml = application("urc-uisocketdesc+xml", "xml");
export const Application_VcardJson = application("vcard+json", "json");
export const Application_VcardXml = application("vcard+xml", "xml");
export const Application_Vemmi = application("vemmi");
export const Application_Vendor_1000mindsDecision_ModelXml = application("vnd.1000minds.decision-model+xml", "xml");
export const Application_Vendor_1d_Interleaved_Parityfec = application("1d-interleaved-parityfec");
export const Application_Vendor_3gpdash_Qoe_ReportXml = application("3gpdash-qoe-report+xml", "xml");
export const Application_Vendor_3gpp_ImsXml = application("3gpp-ims+xml", "xml");
export const Application_Vendor_3gpp_Prose_Pc3chXml = application("vnd.3gpp-prose-pc3ch+xml", "xml");
export const Application_Vendor_3gpp_ProseXml = application("vnd.3gpp-prose+xml", "xml");
export const Application_Vendor_3gpp_V2x_Local_Service_Information = application("vnd.3gpp-v2x-local-service-information");
export const Application_Vendor_3gpp2BcmcsinfoXml = application("vnd.3gpp2.bcmcsinfo+xml", "xml");
export const Application_Vendor_3gpp2Sms = application("vnd.3gpp2.sms");
export const Application_Vendor_3gpp2Tcap = application("vnd.3gpp2.tcap", "tcap");
export const Application_Vendor_3gppAccess_Transfer_EventsXml = application("vnd.3gpp.access-transfer-events+xml", "xml");
export const Application_Vendor_3gppBsfXml = application("vnd.3gpp.bsf+xml", "xml");
export const Application_Vendor_3gppGMOPXml = application("vnd.3gpp.gmop+xml", "xml");
export const Application_Vendor_3gppMc_Signalling_Ear = application("vnd.3gpp.mc-signalling-ear");
export const Application_Vendor_3gppMcdata_Affiliation_CommandXml = application("vnd.3gpp.mcdata-affiliation-command+xml", "xml");
export const Application_Vendor_3gppMcdata_InfoXml = application("vnd.3gpp.mcdata-info+xml", "xml");
export const Application_Vendor_3gppMcdata_Payload = application("vnd.3gpp.mcdata-payload");
export const Application_Vendor_3gppMcdata_Service_ConfigXml = application("vnd.3gpp.mcdata-service-config+xml", "xml");
export const Application_Vendor_3gppMcdata_Signalling = application("vnd.3gpp.mcdata-signalling");
export const Application_Vendor_3gppMcdata_Ue_ConfigXml = application("vnd.3gpp.mcdata-ue-config+xml", "xml");
export const Application_Vendor_3gppMcdata_User_ProfileXml = application("vnd.3gpp.mcdata-user-profile+xml", "xml");
export const Application_Vendor_3gppMcptt_Affiliation_CommandXml = application("vnd.3gpp.mcptt-affiliation-command+xml", "xml");
export const Application_Vendor_3gppMcptt_Floor_RequestXml = application("vnd.3gpp.mcptt-floor-request+xml", "xml");
export const Application_Vendor_3gppMcptt_InfoXml = application("vnd.3gpp.mcptt-info+xml", "xml");
export const Application_Vendor_3gppMcptt_Location_InfoXml = application("vnd.3gpp.mcptt-location-info+xml", "xml");
export const Application_Vendor_3gppMcptt_Mbms_Usage_InfoXml = application("vnd.3gpp.mcptt-mbms-usage-info+xml", "xml");
export const Application_Vendor_3gppMcptt_Service_ConfigXml = application("vnd.3gpp.mcptt-service-config+xml", "xml");
export const Application_Vendor_3gppMcptt_SignedXml = application("vnd.3gpp.mcptt-signed+xml", "xml");
export const Application_Vendor_3gppMcptt_Ue_ConfigXml = application("vnd.3gpp.mcptt-ue-config+xml", "xml");
export const Application_Vendor_3gppMcptt_Ue_Init_ConfigXml = application("vnd.3gpp.mcptt-ue-init-config+xml", "xml");
export const Application_Vendor_3gppMcptt_User_ProfileXml = application("vnd.3gpp.mcptt-user-profile+xml", "xml");
export const Application_Vendor_3gppMcvideo_Affiliation_CommandXml = application("vnd.3gpp.mcvideo-affiliation-command+xml", "xml");
export const Application_Vendor_3gppMcvideo_Affiliation_InfoXml = deprecate(application("vnd.3gpp.mcvideo-affiliation-info+xml", "xml"), "in favor of application/vnd.3gpp.mcvideo-info+xml");
export const Application_Vendor_3gppMcvideo_InfoXml = application("vnd.3gpp.mcvideo-info+xml", "xml");
export const Application_Vendor_3gppMcvideo_Location_InfoXml = application("vnd.3gpp.mcvideo-location-info+xml", "xml");
export const Application_Vendor_3gppMcvideo_Mbms_Usage_InfoXml = application("vnd.3gpp.mcvideo-mbms-usage-info+xml", "xml");
export const Application_Vendor_3gppMcvideo_Service_ConfigXml = application("vnd.3gpp.mcvideo-service-config+xml", "xml");
export const Application_Vendor_3gppMcvideo_Transmission_RequestXml = application("vnd.3gpp.mcvideo-transmission-request+xml", "xml");
export const Application_Vendor_3gppMcvideo_Ue_ConfigXml = application("vnd.3gpp.mcvideo-ue-config+xml", "xml");
export const Application_Vendor_3gppMcvideo_User_ProfileXml = application("vnd.3gpp.mcvideo-user-profile+xml", "xml");
export const Application_Vendor_3gppMid_CallXml = application("vnd.3gpp.mid-call+xml", "xml");
export const Application_Vendor_3gppPic_Bw_Large = application("vnd.3gpp.pic-bw-large", "plb");
export const Application_Vendor_3gppPic_Bw_Small = application("vnd.3gpp.pic-bw-small", "psb");
export const Application_Vendor_3gppPic_Bw_Var = application("vnd.3gpp.pic-bw-var", "pvb");
export const Application_Vendor_3gppSms = application("vnd.3gpp.sms");
export const Application_Vendor_3gppSmsXml = application("vnd.3gpp.sms+xml", "xml");
export const Application_Vendor_3gppSrvcc_ExtXml = application("vnd.3gpp.srvcc-ext+xml", "xml");
export const Application_Vendor_3gppSRVCC_InfoXml = application("vnd.3gpp.srvcc-info+xml", "xml");
export const Application_Vendor_3gppState_And_Event_InfoXml = application("vnd.3gpp.state-and-event-info+xml", "xml");
export const Application_Vendor_3gppUssdXml = application("vnd.3gpp.ussd+xml", "xml");
export const Application_Vendor_3lightssoftwareImagescal = application("vnd.3lightssoftware.imagescal");
export const Application_Vendor_3MPost_It_Notes = application("vnd.3m.post-it-notes", "pwn");
export const Application_Vendor_AccpacSimplyAso = application("vnd.accpac.simply.aso", "aso");
export const Application_Vendor_AccpacSimplyImp = application("vnd.accpac.simply.imp", "imp");
export const Application_Vendor_Acucobol = application("vnd.acucobol", "acu");
export const Application_Vendor_Acucorp = application("vnd.acucorp", "atc", "acutc");
export const Application_Vendor_AdobeAir_Application_Installer_PackageZip = application("vnd.adobe.air-application-installer-package+zip", "air");
export const Application_Vendor_AdobeFlashMovie = application("vnd.adobe.flash.movie");
export const Application_Vendor_AdobeFormscentralFcdt = application("vnd.adobe.formscentral.fcdt", "fcdt");
export const Application_Vendor_AdobeFxp = application("vnd.adobe.fxp", "fxp", "fxpl");
export const Application_Vendor_AdobePartial_Upload = application("vnd.adobe.partial-upload");
export const Application_Vendor_AdobeXdpXml = application("vnd.adobe.xdp+xml", "xdp");
export const Application_Vendor_AdobeXfdf = application("vnd.adobe.xfdf", "xfdf");
export const Application_Vendor_AetherImp = application("vnd.aether.imp");
export const Application_Vendor_AfpcAfplinedata = application("vnd.afpc.afplinedata");
export const Application_Vendor_AfpcAfplinedata_Pagedef = application("vnd.afpc.afplinedata-pagedef");
export const Application_Vendor_AfpcFoca_Charset = application("vnd.afpc.foca-charset");
export const Application_Vendor_AfpcFoca_Codedfont = application("vnd.afpc.foca-codedfont");
export const Application_Vendor_AfpcFoca_Codepage = application("vnd.afpc.foca-codepage");
export const Application_Vendor_AfpcModca = application("vnd.afpc.modca");
export const Application_Vendor_AfpcModca_Formdef = application("vnd.afpc.modca-formdef");
export const Application_Vendor_AfpcModca_Mediummap = application("vnd.afpc.modca-mediummap");
export const Application_Vendor_AfpcModca_Objectcontainer = application("vnd.afpc.modca-objectcontainer");
export const Application_Vendor_AfpcModca_Overlay = application("vnd.afpc.modca-overlay");
export const Application_Vendor_AfpcModca_Pagesegment = application("vnd.afpc.modca-pagesegment");
export const Application_Vendor_Ah_Barcode = application("vnd.ah-barcode");
export const Application_Vendor_AheadSpace = application("vnd.ahead.space", "ahead");
export const Application_Vendor_AirzipFilesecureAzf = application("vnd.airzip.filesecure.azf", "azf");
export const Application_Vendor_AirzipFilesecureAzs = application("vnd.airzip.filesecure.azs", "azs");
export const Application_Vendor_AmadeusJson = application("vnd.amadeus+json", "json");
export const Application_Vendor_AmazonEbook = application("vnd.amazon.ebook", "azw");
export const Application_Vendor_AmazonMobi8_Ebook = application("vnd.amazon.mobi8-ebook");
export const Application_Vendor_AmericandynamicsAcc = application("vnd.americandynamics.acc", "acc");
export const Application_Vendor_AmigaAmi = application("vnd.amiga.ami", "ami");
export const Application_Vendor_AmundsenMazeXml = application("vnd.amundsen.maze+xml", "xml");
export const Application_Vendor_AndroidOta = application("vnd.android.ota");
export const Application_Vendor_AndroidPackage_Archive = application("vnd.android.package-archive", "apk");
export const Application_Vendor_Anki = application("vnd.anki");
export const Application_Vendor_Anser_Web_Certificate_Issue_Initiation = application("vnd.anser-web-certificate-issue-initiation", "cii");
export const Application_Vendor_Anser_Web_Funds_Transfer_Initiation = application("vnd.anser-web-funds-transfer-initiation", "fti");
export const Application_Vendor_AntixGame_Component = application("vnd.antix.game-component", "atx");
export const Application_Vendor_ApacheThriftBinary = application("vnd.apache.thrift.binary");
export const Application_Vendor_ApacheThriftCompact = application("vnd.apache.thrift.compact");
export const Application_Vendor_ApacheThriftJson = application("vnd.apache.thrift.json");
export const Application_Vendor_ApiJson = application("vnd.api+json", "json");
export const Application_Vendor_AplextorWarrpJson = application("vnd.aplextor.warrp+json", "json");
export const Application_Vendor_ApothekendeReservationJson = application("vnd.apothekende.reservation+json", "json");
export const Application_Vendor_AppleInstallerXml = application("vnd.apple.installer+xml", "mpkg");
export const Application_Vendor_AppleKeynote = application("vnd.apple.keynote");
export const Application_Vendor_AppleMpegurl = application("vnd.apple.mpegurl", "m3u8");
export const Application_Vendor_AppleNumbers = application("vnd.apple.numbers");
export const Application_Vendor_ApplePages = application("vnd.apple.pages");
export const Application_Vendor_ApplePkpass = application("vnd.apple.pkpass", "pkpass");
export const Application_Vendor_ArastraSwi = deprecate(application("vnd.arastra.swi"), "in favor of application/vnd.aristanetworks.swi");
export const Application_Vendor_AristanetworksSwi = application("vnd.aristanetworks.swi", "swi");
export const Application_Vendor_ArtisanJson = application("vnd.artisan+json", "json");
export const Application_Vendor_Artsquare = application("vnd.artsquare");
export const Application_Vendor_Astraea_SoftwareIota = application("vnd.astraea-software.iota", "iota");
export const Application_Vendor_Audiograph = application("vnd.audiograph", "aep");
export const Application_Vendor_Autopackage = application("vnd.autopackage");
export const Application_Vendor_AvalonJson = application("vnd.avalon+json", "json");
export const Application_Vendor_AvistarXml = application("vnd.avistar+xml", "xml");
export const Application_Vendor_BalsamiqBmmlXml = application("vnd.balsamiq.bmml+xml", "xml");
export const Application_Vendor_BalsamiqBmpr = application("vnd.balsamiq.bmpr");
export const Application_Vendor_Banana_Accounting = application("vnd.banana-accounting");
export const Application_Vendor_BbfUspError = application("vnd.bbf.usp.error");
export const Application_Vendor_BbfUspMsg = application("vnd.bbf.usp.msg");
export const Application_Vendor_BbfUspMsgJson = application("vnd.bbf.usp.msg+json", "json");
export const Application_Vendor_Bekitzur_StechJson = application("vnd.bekitzur-stech+json", "json");
export const Application_Vendor_BintMed_Content = application("vnd.bint.med-content");
export const Application_Vendor_BiopaxRdfXml = application("vnd.biopax.rdf+xml", "xml");
export const Application_Vendor_Blink_Idb_Value_Wrapper = application("vnd.blink-idb-value-wrapper");
export const Application_Vendor_BlueiceMultipass = application("vnd.blueice.multipass", "mpm");
export const Application_Vendor_BluetoothEpOob = application("vnd.bluetooth.ep.oob");
export const Application_Vendor_BluetoothLeOob = application("vnd.bluetooth.le.oob");
export const Application_Vendor_Bmi = application("vnd.bmi", "bmi");
export const Application_Vendor_Bpf = application("vnd.bpf");
export const Application_Vendor_Bpf3 = application("vnd.bpf3");
export const Application_Vendor_Businessobjects = application("vnd.businessobjects", "rep");
export const Application_Vendor_ByuUapiJson = application("vnd.byu.uapi+json", "json");
export const Application_Vendor_Cab_Jscript = application("vnd.cab-jscript");
export const Application_Vendor_Canon_Cpdl = application("vnd.canon-cpdl");
export const Application_Vendor_Canon_Lips = application("vnd.canon-lips");
export const Application_Vendor_Capasystems_PgJson = application("vnd.capasystems-pg+json", "json");
export const Application_Vendor_CendioThinlincClientconf = application("vnd.cendio.thinlinc.clientconf");
export const Application_Vendor_Century_SystemsTcp_stream = application("vnd.century-systems.tcp_stream");
export const Application_Vendor_ChemdrawXml = application("vnd.chemdraw+xml", "cdxml");
export const Application_Vendor_Chess_Pgn = application("vnd.chess-pgn");
export const Application_Vendor_ChipnutsKaraoke_Mmd = application("vnd.chipnuts.karaoke-mmd", "mmd");
export const Application_Vendor_Ciedi = application("vnd.ciedi");
export const Application_Vendor_Cinderella = application("vnd.cinderella", "cdy");
export const Application_Vendor_CirpackIsdn_Ext = application("vnd.cirpack.isdn-ext");
export const Application_Vendor_CitationstylesStyleXml = application("vnd.citationstyles.style+xml", "xml");
export const Application_Vendor_Claymore = application("vnd.claymore", "cla");
export const Application_Vendor_CloantoRp9 = application("vnd.cloanto.rp9", "rp9");
export const Application_Vendor_ClonkC4group = application("vnd.clonk.c4group", "c4g", "c4d", "c4f", "c4p", "c4u");
export const Application_Vendor_CluetrustCartomobile_Config = application("vnd.cluetrust.cartomobile-config", "c11amc");
export const Application_Vendor_CluetrustCartomobile_Config_Pkg = application("vnd.cluetrust.cartomobile-config-pkg", "c11amz");
export const Application_Vendor_Coffeescript = application("vnd.coffeescript");
export const Application_Vendor_CollabioXodocumentsDocument = application("vnd.collabio.xodocuments.document");
export const Application_Vendor_CollabioXodocumentsDocument_Template = application("vnd.collabio.xodocuments.document-template");
export const Application_Vendor_CollabioXodocumentsPresentation = application("vnd.collabio.xodocuments.presentation");
export const Application_Vendor_CollabioXodocumentsPresentation_Template = application("vnd.collabio.xodocuments.presentation-template");
export const Application_Vendor_CollabioXodocumentsSpreadsheet = application("vnd.collabio.xodocuments.spreadsheet");
export const Application_Vendor_CollabioXodocumentsSpreadsheet_Template = application("vnd.collabio.xodocuments.spreadsheet-template");
export const Application_Vendor_CollectionDocJson = application("vnd.collection.doc+json", "json");
export const Application_Vendor_CollectionJson = application("vnd.collection+json", "json");
export const Application_Vendor_CollectionNextJson = application("vnd.collection.next+json", "json");
export const Application_Vendor_Comicbook_Rar = application("vnd.comicbook-rar");
export const Application_Vendor_ComicbookZip = application("vnd.comicbook+zip", "zip");
export const Application_Vendor_Commerce_Battelle = application("vnd.commerce-battelle");
export const Application_Vendor_Commonspace = application("vnd.commonspace", "csp");
export const Application_Vendor_ContactCmsg = application("vnd.contact.cmsg", "cdbcmsg");
export const Application_Vendor_CoreosIgnitionJson = application("vnd.coreos.ignition+json", "json");
export const Application_Vendor_Cosmocaller = application("vnd.cosmocaller", "cmc");
export const Application_Vendor_CrickClicker = application("vnd.crick.clicker", "clkx");
export const Application_Vendor_CrickClickerKeyboard = application("vnd.crick.clicker.keyboard", "clkk");
export const Application_Vendor_CrickClickerPalette = application("vnd.crick.clicker.palette", "clkp");
export const Application_Vendor_CrickClickerTemplate = application("vnd.crick.clicker.template", "clkt");
export const Application_Vendor_CrickClickerWordbank = application("vnd.crick.clicker.wordbank", "clkw");
export const Application_Vendor_CriticaltoolsWbsXml = application("vnd.criticaltools.wbs+xml", "wbs");
export const Application_Vendor_CryptiiPipeJson = application("vnd.cryptii.pipe+json", "json");
export const Application_Vendor_Crypto_Shade_File = application("vnd.crypto-shade-file");
export const Application_Vendor_Ctc_Posml = application("vnd.ctc-posml", "pml");
export const Application_Vendor_CtctWsXml = application("vnd.ctct.ws+xml", "xml");
export const Application_Vendor_Cups_Pdf = application("vnd.cups-pdf");
export const Application_Vendor_Cups_Postscript = application("vnd.cups-postscript");
export const Application_Vendor_Cups_Ppd = application("vnd.cups-ppd", "ppd");
export const Application_Vendor_Cups_Raster = application("vnd.cups-raster");
export const Application_Vendor_Cups_Raw = application("vnd.cups-raw");
export const Application_Vendor_Curl = application("vnd.curl");
export const Application_Vendor_CurlCar = application("vnd.curl.car", "car");
export const Application_Vendor_CurlPcurl = application("vnd.curl.pcurl", "pcurl");
export const Application_Vendor_CyanDeanRootXml = application("vnd.cyan.dean.root+xml", "xml");
export const Application_Vendor_Cybank = application("vnd.cybank");
export const Application_Vendor_D2lCoursepackage1p0Zip = application("vnd.d2l.coursepackage1p0+zip", "zip");
export const Application_Vendor_Dart = application("vnd.dart", "dart");
export const Application_Vendor_Data_VisionRdz = application("vnd.data-vision.rdz", "rdz");
export const Application_Vendor_DatapackageJson = application("vnd.datapackage+json", "json");
export const Application_Vendor_DataresourceJson = application("vnd.dataresource+json", "json");
export const Application_Vendor_DebianBinary_Package = application("vnd.debian.binary-package");
export const Application_Vendor_DeceData = application("vnd.dece.data", "uvf", "uvvf", "uvd", "uvvd");
export const Application_Vendor_DeceTtmlXml = application("vnd.dece.ttml+xml", "uvt", "uvvt");
export const Application_Vendor_DeceUnspecified = application("vnd.dece.unspecified", "uvx", "uvvx");
export const Application_Vendor_DeceZip = application("vnd.dece.zip", "uvz", "uvvz");
export const Application_Vendor_DenovoFcselayout_Link = application("vnd.denovo.fcselayout-link", "fe_launch");
export const Application_Vendor_DesmumeMovie = application("vnd.desmume.movie");
export const Application_Vendor_Dir_BiPlate_Dl_Nosuffix = application("vnd.dir-bi.plate-dl-nosuffix");
export const Application_Vendor_DmDelegationXml = application("vnd.dm.delegation+xml", "xml");
export const Application_Vendor_Dna = application("vnd.dna", "dna");
export const Application_Vendor_DocumentJson = application("vnd.document+json", "json");
export const Application_Vendor_DolbyMlp = application("vnd.dolby.mlp", "mlp");
export const Application_Vendor_DolbyMobile1 = application("vnd.dolby.mobile.1");
export const Application_Vendor_DolbyMobile2 = application("vnd.dolby.mobile.2");
export const Application_Vendor_DoremirScorecloud_Binary_Document = application("vnd.doremir.scorecloud-binary-document");
export const Application_Vendor_Dpgraph = application("vnd.dpgraph", "dpg");
export const Application_Vendor_Dreamfactory = application("vnd.dreamfactory", "dfac");
export const Application_Vendor_DriveJson = application("vnd.drive+json", "json");
export const Application_Vendor_Ds_Keypoint = application("vnd.ds-keypoint", "kpxx");
export const Application_Vendor_DtgLocal = application("vnd.dtg.local");
export const Application_Vendor_DtgLocalFlash = application("vnd.dtg.local.flash");
export const Application_Vendor_DtgLocalHtml = application("vnd.dtg.local.html");
export const Application_Vendor_DvbAit = application("vnd.dvb.ait", "ait");
export const Application_Vendor_DvbDvbj = application("vnd.dvb.dvbj");
export const Application_Vendor_DvbEsgcontainer = application("vnd.dvb.esgcontainer");
export const Application_Vendor_DvbIpdcdftnotifaccess = application("vnd.dvb.ipdcdftnotifaccess");
export const Application_Vendor_DvbIpdcesgaccess = application("vnd.dvb.ipdcesgaccess");
export const Application_Vendor_DvbIpdcesgaccess2 = application("vnd.dvb.ipdcesgaccess2");
export const Application_Vendor_DvbIpdcesgpdd = application("vnd.dvb.ipdcesgpdd");
export const Application_Vendor_DvbIpdcroaming = application("vnd.dvb.ipdcroaming");
export const Application_Vendor_DvbIptvAlfec_Base = application("vnd.dvb.iptv.alfec-base");
export const Application_Vendor_DvbIptvAlfec_Enhancement = application("vnd.dvb.iptv.alfec-enhancement");
export const Application_Vendor_DvbNotif_Aggregate_RootXml = application("vnd.dvb.notif-aggregate-root+xml", "xml");
export const Application_Vendor_DvbNotif_ContainerXml = application("vnd.dvb.notif-container+xml", "xml");
export const Application_Vendor_DvbNotif_GenericXml = application("vnd.dvb.notif-generic+xml", "xml");
export const Application_Vendor_DvbNotif_Ia_MsglistXml = application("vnd.dvb.notif-ia-msglist+xml", "xml");
export const Application_Vendor_DvbNotif_Ia_Registration_RequestXml = application("vnd.dvb.notif-ia-registration-request+xml", "xml");
export const Application_Vendor_DvbNotif_Ia_Registration_ResponseXml = application("vnd.dvb.notif-ia-registration-response+xml", "xml");
export const Application_Vendor_DvbNotif_InitXml = application("vnd.dvb.notif-init+xml", "xml");
export const Application_Vendor_DvbPfr = application("vnd.dvb.pfr");
export const Application_Vendor_DvbService = application("vnd.dvb.service", "svc");
export const Application_Vendor_Dxr = application("vnd.dxr");
export const Application_Vendor_Dynageo = application("vnd.dynageo", "geo");
export const Application_Vendor_Dzr = application("vnd.dzr");
export const Application_Vendor_EasykaraokeCdgdownload = application("vnd.easykaraoke.cdgdownload");
export const Application_Vendor_Ecdis_Update = application("vnd.ecdis-update");
export const Application_Vendor_EcipRlp = application("vnd.ecip.rlp");
export const Application_Vendor_EcowinChart = application("vnd.ecowin.chart", "mag");
export const Application_Vendor_EcowinFilerequest = application("vnd.ecowin.filerequest");
export const Application_Vendor_EcowinFileupdate = application("vnd.ecowin.fileupdate");
export const Application_Vendor_EcowinSeries = application("vnd.ecowin.series");
export const Application_Vendor_EcowinSeriesrequest = application("vnd.ecowin.seriesrequest");
export const Application_Vendor_EcowinSeriesupdate = application("vnd.ecowin.seriesupdate");
export const Application_Vendor_EfiImg = application("vnd.efi.img");
export const Application_Vendor_EfiIso = application("vnd.efi.iso");
export const Application_Vendor_EmclientAccessrequestXml = application("vnd.emclient.accessrequest+xml", "xml");
export const Application_Vendor_Enliven = application("vnd.enliven", "nml");
export const Application_Vendor_EnphaseEnvoy = application("vnd.enphase.envoy");
export const Application_Vendor_EprintsDataXml = application("vnd.eprints.data+xml", "xml");
export const Application_Vendor_EpsonEsf = application("vnd.epson.esf", "esf");
export const Application_Vendor_EpsonMsf = application("vnd.epson.msf", "msf");
export const Application_Vendor_EpsonQuickanime = application("vnd.epson.quickanime", "qam");
export const Application_Vendor_EpsonSalt = application("vnd.epson.salt", "slt");
export const Application_Vendor_EpsonSsf = application("vnd.epson.ssf", "ssf");
export const Application_Vendor_EricssonQuickcall = application("vnd.ericsson.quickcall");
export const Application_Vendor_Espass_EspassZip = application("vnd.espass-espass+zip", "zip");
export const Application_Vendor_Eszigno3Xml = application("vnd.eszigno3+xml", "es3", "et3");
export const Application_Vendor_EtsiAocXml = application("vnd.etsi.aoc+xml", "xml");
export const Application_Vendor_EtsiAsic_EZip = application("vnd.etsi.asic-e+zip", "zip");
export const Application_Vendor_EtsiAsic_SZip = application("vnd.etsi.asic-s+zip", "zip");
export const Application_Vendor_EtsiCugXml = application("vnd.etsi.cug+xml", "xml");
export const Application_Vendor_EtsiIptvcommandXml = application("vnd.etsi.iptvcommand+xml", "xml");
export const Application_Vendor_EtsiIptvdiscoveryXml = application("vnd.etsi.iptvdiscovery+xml", "xml");
export const Application_Vendor_EtsiIptvprofileXml = application("vnd.etsi.iptvprofile+xml", "xml");
export const Application_Vendor_EtsiIptvsad_BcXml = application("vnd.etsi.iptvsad-bc+xml", "xml");
export const Application_Vendor_EtsiIptvsad_CodXml = application("vnd.etsi.iptvsad-cod+xml", "xml");
export const Application_Vendor_EtsiIptvsad_NpvrXml = application("vnd.etsi.iptvsad-npvr+xml", "xml");
export const Application_Vendor_EtsiIptvserviceXml = application("vnd.etsi.iptvservice+xml", "xml");
export const Application_Vendor_EtsiIptvsyncXml = application("vnd.etsi.iptvsync+xml", "xml");
export const Application_Vendor_EtsiIptvueprofileXml = application("vnd.etsi.iptvueprofile+xml", "xml");
export const Application_Vendor_EtsiMcidXml = application("vnd.etsi.mcid+xml", "xml");
export const Application_Vendor_EtsiMheg5 = application("vnd.etsi.mheg5");
export const Application_Vendor_EtsiOverload_Control_Policy_DatasetXml = application("vnd.etsi.overload-control-policy-dataset+xml", "xml");
export const Application_Vendor_EtsiPstnXml = application("vnd.etsi.pstn+xml", "xml");
export const Application_Vendor_EtsiSciXml = application("vnd.etsi.sci+xml", "xml");
export const Application_Vendor_EtsiSimservsXml = application("vnd.etsi.simservs+xml", "xml");
export const Application_Vendor_EtsiTimestamp_Token = application("vnd.etsi.timestamp-token");
export const Application_Vendor_EtsiTslDer = application("vnd.etsi.tsl.der");
export const Application_Vendor_EtsiTslXml = application("vnd.etsi.tsl+xml", "xml");
export const Application_Vendor_EudoraData = application("vnd.eudora.data");
export const Application_Vendor_EvolvEcigProfile = application("vnd.evolv.ecig.profile");
export const Application_Vendor_EvolvEcigSettings = application("vnd.evolv.ecig.settings");
export const Application_Vendor_EvolvEcigTheme = application("vnd.evolv.ecig.theme");
export const Application_Vendor_Exstream_EmpowerZip = application("vnd.exstream-empower+zip", "zip");
export const Application_Vendor_Exstream_Package = application("vnd.exstream-package");
export const Application_Vendor_Ezpix_Album = application("vnd.ezpix-album", "ez2");
export const Application_Vendor_Ezpix_Package = application("vnd.ezpix-package", "ez3");
export const Application_Vendor_F_SecureMobile = application("vnd.f-secure.mobile");
export const Application_Vendor_Fastcopy_Disk_Image = application("vnd.fastcopy-disk-image");
export const Application_Vendor_Fdf = application("vnd.fdf", "fdf");
export const Application_Vendor_FdsnMseed = application("vnd.fdsn.mseed", "mseed");
export const Application_Vendor_FdsnSeed = application("vnd.fdsn.seed", "seed", "dataless");
export const Application_Vendor_Ffsns = application("vnd.ffsns");
export const Application_Vendor_FiclabFlbZip = application("vnd.ficlab.flb+zip", "zip");
export const Application_Vendor_FilmitZfc = application("vnd.filmit.zfc");
export const Application_Vendor_Fints = application("vnd.fints");
export const Application_Vendor_FiremonkeysCloudcell = application("vnd.firemonkeys.cloudcell");
export const Application_Vendor_FloGraphIt = application("vnd.flographit", "gph");
export const Application_Vendor_FluxtimeClip = application("vnd.fluxtime.clip", "ftc");
export const Application_Vendor_Font_Fontforge_Sfd = application("vnd.font-fontforge-sfd");
export const Application_Vendor_Framemaker = application("vnd.framemaker", "fm", "frame", "maker", "book");
export const Application_Vendor_FrogansFnc = application("vnd.frogans.fnc", "fnc");
export const Application_Vendor_FrogansLtf = application("vnd.frogans.ltf", "ltf");
export const Application_Vendor_FscWeblaunch = application("vnd.fsc.weblaunch", "fsc");
export const Application_Vendor_FujitsuOasys = application("vnd.fujitsu.oasys", "oas");
export const Application_Vendor_FujitsuOasys2 = application("vnd.fujitsu.oasys2", "oa2");
export const Application_Vendor_FujitsuOasys3 = application("vnd.fujitsu.oasys3", "oa3");
export const Application_Vendor_FujitsuOasysgp = application("vnd.fujitsu.oasysgp", "fg5");
export const Application_Vendor_FujitsuOasysprs = application("vnd.fujitsu.oasysprs", "bh2");
export const Application_Vendor_FujixeroxART_EX = application("vnd.fujixerox.art-ex");
export const Application_Vendor_FujixeroxART4 = application("vnd.fujixerox.art4");
export const Application_Vendor_FujixeroxDdd = application("vnd.fujixerox.ddd", "ddd");
export const Application_Vendor_FujixeroxDocuworks = application("vnd.fujixerox.docuworks", "xdw");
export const Application_Vendor_FujixeroxDocuworksBinder = application("vnd.fujixerox.docuworks.binder", "xbd");
export const Application_Vendor_FujixeroxDocuworksContainer = application("vnd.fujixerox.docuworks.container");
export const Application_Vendor_FujixeroxHBPL = application("vnd.fujixerox.hbpl");
export const Application_Vendor_Fut_Misnet = application("vnd.fut-misnet");
export const Application_Vendor_FutoinCbor = application("vnd.futoin+cbor", "cbor");
export const Application_Vendor_FutoinJson = application("vnd.futoin+json", "json");
export const Application_Vendor_Fuzzysheet = application("vnd.fuzzysheet", "fzs");
export const Application_Vendor_GenomatixTuxedo = application("vnd.genomatix.tuxedo", "txd");
export const Application_Vendor_GenticsGrdJson = application("vnd.gentics.grd+json", "json");
export const Application_Vendor_GeocubeXml = deprecate(application("vnd.geocube+xml", "xml"), "by request");
export const Application_Vendor_GeogebraFile = application("vnd.geogebra.file", "ggb");
export const Application_Vendor_GeogebraTool = application("vnd.geogebra.tool", "ggt");
export const Application_Vendor_GeoJson = deprecate(application("vnd.geo+json", "json"), "in favor of application/geo+json");
export const Application_Vendor_Geometry_Explorer = application("vnd.geometry-explorer", "gex", "gre");
export const Application_Vendor_Geonext = application("vnd.geonext", "gxt");
export const Application_Vendor_Geoplan = application("vnd.geoplan", "g2w");
export const Application_Vendor_Geospace = application("vnd.geospace", "g3w");
export const Application_Vendor_Gerber = application("vnd.gerber");
export const Application_Vendor_GlobalplatformCard_Content_Mgt = application("vnd.globalplatform.card-content-mgt");
export const Application_Vendor_GlobalplatformCard_Content_Mgt_Response = application("vnd.globalplatform.card-content-mgt-response");
export const Application_Vendor_Gmx = deprecate(application("vnd.gmx", "gmx"), "with no reason given");
export const Application_Vendor_Google_Apps_Document = application("vnd.google-apps.document", "gdoc");
export const Application_Vendor_Google_Apps_Presentation = application("vnd.google-apps.presentation", "gslides");
export const Application_Vendor_Google_Apps_Spreadsheet = application("vnd.google-apps.spreadsheet", "gsheet");
export const Application_Vendor_Google_EarthKmlXml = application("vnd.google-earth.kml+xml", "kml");
export const Application_Vendor_Google_EarthKmz = application("vnd.google-earth.kmz", "kmz");
export const Application_Vendor_GovSkE_FormXml = application("vnd.gov.sk.e-form+xml", "xml");
export const Application_Vendor_GovSkE_FormZip = application("vnd.gov.sk.e-form+zip", "zip");
export const Application_Vendor_GovSkXmldatacontainerXml = application("vnd.gov.sk.xmldatacontainer+xml", "xml");
export const Application_Vendor_Grafeq = application("vnd.grafeq", "gqf", "gqs");
export const Application_Vendor_Gridmp = application("vnd.gridmp");
export const Application_Vendor_Groove_Account = application("vnd.groove-account", "gac");
export const Application_Vendor_Groove_Help = application("vnd.groove-help", "ghf");
export const Application_Vendor_Groove_Identity_Message = application("vnd.groove-identity-message", "gim");
export const Application_Vendor_Groove_Injector = application("vnd.groove-injector", "grv");
export const Application_Vendor_Groove_Tool_Message = application("vnd.groove-tool-message", "gtm");
export const Application_Vendor_Groove_Tool_Template = application("vnd.groove-tool-template", "tpl");
export const Application_Vendor_Groove_Vcard = application("vnd.groove-vcard", "vcg");
export const Application_Vendor_HalJson = application("vnd.hal+json", "json");
export const Application_Vendor_HalXml = application("vnd.hal+xml", "hal");
export const Application_Vendor_HandHeld_EntertainmentXml = application("vnd.handheld-entertainment+xml", "zmm");
export const Application_Vendor_Hbci = application("vnd.hbci", "hbci");
export const Application_Vendor_HcJson = application("vnd.hc+json", "json");
export const Application_Vendor_Hcl_Bireports = application("vnd.hcl-bireports");
export const Application_Vendor_Hdt = application("vnd.hdt");
export const Application_Vendor_HerokuJson = application("vnd.heroku+json", "json");
export const Application_Vendor_HheLesson_Player = application("vnd.hhe.lesson-player", "les");
export const Application_Vendor_Hp_HPGL = application("vnd.hp-hpgl", "hpgl");
export const Application_Vendor_Hp_Hpid = application("vnd.hp-hpid", "hpid");
export const Application_Vendor_Hp_Hps = application("vnd.hp-hps", "hps");
export const Application_Vendor_Hp_Jlyt = application("vnd.hp-jlyt", "jlt");
export const Application_Vendor_Hp_PCL = application("vnd.hp-pcl", "pcl");
export const Application_Vendor_Hp_PCLXL = application("vnd.hp-pclxl", "pclxl");
export const Application_Vendor_Httphone = application("vnd.httphone");
export const Application_Vendor_HydrostatixSof_Data = application("vnd.hydrostatix.sof-data", "sfd-hdstx");
export const Application_Vendor_Hyper_ItemJson = application("vnd.hyper-item+json", "json");
export const Application_Vendor_HyperdriveJson = application("vnd.hyperdrive+json", "json");
export const Application_Vendor_HyperJson = application("vnd.hyper+json", "json");
export const Application_Vendor_Hzn_3d_Crossword = application("vnd.hzn-3d-crossword");
export const Application_Vendor_IbmAfplinedata = deprecate(application("vnd.ibm.afplinedata"), "in favor of vnd.afpc.afplinedata");
export const Application_Vendor_IbmElectronic_Media = application("vnd.ibm.electronic-media");
export const Application_Vendor_IbmMiniPay = application("vnd.ibm.minipay", "mpy");
export const Application_Vendor_IbmModcap = deprecate(application("vnd.ibm.modcap", "afp", "listafp", "list3820"), "in favor of application/vnd.afpc.modca");
export const Application_Vendor_IbmRights_Management = application("vnd.ibm.rights-management", "irm");
export const Application_Vendor_IbmSecure_Container = application("vnd.ibm.secure-container", "sc");
export const Application_Vendor_Iccprofile = application("vnd.iccprofile", "icc", "icm");
export const Application_Vendor_Ieee1905 = application("vnd.ieee.1905");
export const Application_Vendor_Igloader = application("vnd.igloader", "igl");
export const Application_Vendor_ImagemeterFolderZip = application("vnd.imagemeter.folder+zip", "zip");
export const Application_Vendor_ImagemeterImageZip = application("vnd.imagemeter.image+zip", "zip");
export const Application_Vendor_Immervision_Ivp = application("vnd.immervision-ivp", "ivp");
export const Application_Vendor_Immervision_Ivu = application("vnd.immervision-ivu", "ivu");
export const Application_Vendor_ImsImsccv1p1 = application("vnd.ims.imsccv1p1");
export const Application_Vendor_ImsImsccv1p2 = application("vnd.ims.imsccv1p2");
export const Application_Vendor_ImsImsccv1p3 = application("vnd.ims.imsccv1p3");
export const Application_Vendor_ImsLisV2ResultJson = application("vnd.ims.lis.v2.result+json", "json");
export const Application_Vendor_ImsLtiV2ToolconsumerprofileJson = application("vnd.ims.lti.v2.toolconsumerprofile+json", "json");
export const Application_Vendor_ImsLtiV2ToolproxyIdJson = application("vnd.ims.lti.v2.toolproxy.id+json", "json");
export const Application_Vendor_ImsLtiV2ToolproxyJson = application("vnd.ims.lti.v2.toolproxy+json", "json");
export const Application_Vendor_ImsLtiV2ToolsettingsJson = application("vnd.ims.lti.v2.toolsettings+json", "json");
export const Application_Vendor_ImsLtiV2ToolsettingsSimpleJson = application("vnd.ims.lti.v2.toolsettings.simple+json", "json");
export const Application_Vendor_InformedcontrolRmsXml = application("vnd.informedcontrol.rms+xml", "xml");
export const Application_Vendor_Informix_Visionary = deprecate(application("vnd.informix-visionary"), "in favor of application/vnd.visionary");
export const Application_Vendor_InfotechProject = application("vnd.infotech.project");
export const Application_Vendor_InfotechProjectXml = application("vnd.infotech.project+xml", "xml");
export const Application_Vendor_InnopathWampNotification = application("vnd.innopath.wamp.notification");
export const Application_Vendor_InsorsIgm = application("vnd.insors.igm", "igm");
export const Application_Vendor_InterconFormnet = application("vnd.intercon.formnet", "xpw", "xpx");
export const Application_Vendor_Intergeo = application("vnd.intergeo", "i2g");
export const Application_Vendor_IntertrustDigibox = application("vnd.intertrust.digibox");
export const Application_Vendor_IntertrustNncp = application("vnd.intertrust.nncp");
export const Application_Vendor_IntuQbo = application("vnd.intu.qbo", "qbo");
export const Application_Vendor_IntuQfx = application("vnd.intu.qfx", "qfx");
export const Application_Vendor_IptcG2CatalogitemXml = application("vnd.iptc.g2.catalogitem+xml", "xml");
export const Application_Vendor_IptcG2ConceptitemXml = application("vnd.iptc.g2.conceptitem+xml", "xml");
export const Application_Vendor_IptcG2KnowledgeitemXml = application("vnd.iptc.g2.knowledgeitem+xml", "xml");
export const Application_Vendor_IptcG2NewsitemXml = application("vnd.iptc.g2.newsitem+xml", "xml");
export const Application_Vendor_IptcG2NewsmessageXml = application("vnd.iptc.g2.newsmessage+xml", "xml");
export const Application_Vendor_IptcG2PackageitemXml = application("vnd.iptc.g2.packageitem+xml", "xml");
export const Application_Vendor_IptcG2PlanningitemXml = application("vnd.iptc.g2.planningitem+xml", "xml");
export const Application_Vendor_IpunpluggedRcprofile = application("vnd.ipunplugged.rcprofile", "rcprofile");
export const Application_Vendor_IrepositoryPackageXml = application("vnd.irepository.package+xml", "irp");
export const Application_Vendor_Is_Xpr = application("vnd.is-xpr", "xpr");
export const Application_Vendor_IsacFcs = application("vnd.isac.fcs", "fcs");
export const Application_Vendor_Iso11783_10Zip = application("vnd.iso11783-10+zip", "zip");
export const Application_Vendor_Jam = application("vnd.jam", "jam");
export const Application_Vendor_Japannet_Directory_Service = application("vnd.japannet-directory-service");
export const Application_Vendor_Japannet_Jpnstore_Wakeup = application("vnd.japannet-jpnstore-wakeup");
export const Application_Vendor_Japannet_Payment_Wakeup = application("vnd.japannet-payment-wakeup");
export const Application_Vendor_Japannet_Registration = application("vnd.japannet-registration");
export const Application_Vendor_Japannet_Registration_Wakeup = application("vnd.japannet-registration-wakeup");
export const Application_Vendor_Japannet_Setstore_Wakeup = application("vnd.japannet-setstore-wakeup");
export const Application_Vendor_Japannet_Verification = application("vnd.japannet-verification");
export const Application_Vendor_Japannet_Verification_Wakeup = application("vnd.japannet-verification-wakeup");
export const Application_Vendor_JcpJavameMidlet_Rms = application("vnd.jcp.javame.midlet-rms", "rms");
export const Application_Vendor_Jisp = application("vnd.jisp", "jisp");
export const Application_Vendor_JoostJoda_Archive = application("vnd.joost.joda-archive", "joda");
export const Application_Vendor_JskIsdn_Ngn = application("vnd.jsk.isdn-ngn");
export const Application_Vendor_Kahootz = application("vnd.kahootz", "ktz", "ktr");
export const Application_Vendor_KdeKarbon = application("vnd.kde.karbon", "karbon");
export const Application_Vendor_KdeKchart = application("vnd.kde.kchart", "chrt");
export const Application_Vendor_KdeKformula = application("vnd.kde.kformula", "kfo");
export const Application_Vendor_KdeKivio = application("vnd.kde.kivio", "flw");
export const Application_Vendor_KdeKontour = application("vnd.kde.kontour", "kon");
export const Application_Vendor_KdeKpresenter = application("vnd.kde.kpresenter", "kpr", "kpt");
export const Application_Vendor_KdeKspread = application("vnd.kde.kspread", "ksp");
export const Application_Vendor_KdeKword = application("vnd.kde.kword", "kwd", "kwt");
export const Application_Vendor_Kenameaapp = application("vnd.kenameaapp", "htke");
export const Application_Vendor_Kidspiration = application("vnd.kidspiration", "kia");
export const Application_Vendor_Kinar = application("vnd.kinar", "kne", "knp");
export const Application_Vendor_Koan = application("vnd.koan", "skp", "skd", "skt", "skm");
export const Application_Vendor_Kodak_Descriptor = application("vnd.kodak-descriptor", "sse");
export const Application_Vendor_Las = application("vnd.las");
export const Application_Vendor_LasLasJson = application("vnd.las.las+json", "json");
export const Application_Vendor_LasLasXml = application("vnd.las.las+xml", "lasxml");
export const Application_Vendor_Laszip = application("vnd.laszip");
export const Application_Vendor_LeapJson = application("vnd.leap+json", "json");
export const Application_Vendor_Liberty_RequestXml = application("vnd.liberty-request+xml", "xml");
export const Application_Vendor_LlamagraphicsLife_BalanceDesktop = application("vnd.llamagraphics.life-balance.desktop", "lbd");
export const Application_Vendor_LlamagraphicsLife_BalanceExchangeXml = application("vnd.llamagraphics.life-balance.exchange+xml", "lbe");
export const Application_Vendor_LogipipeCircuitZip = application("vnd.logipipe.circuit+zip", "zip");
export const Application_Vendor_Loom = application("vnd.loom");
export const Application_Vendor_Lotus_1_2_3 = application("vnd.lotus-1-2-3", "123");
export const Application_Vendor_Lotus_Approach = application("vnd.lotus-approach", "apr");
export const Application_Vendor_Lotus_Freelance = application("vnd.lotus-freelance", "pre");
export const Application_Vendor_Lotus_Notes = application("vnd.lotus-notes", "nsf");
export const Application_Vendor_Lotus_Organizer = application("vnd.lotus-organizer", "org");
export const Application_Vendor_Lotus_Screencam = application("vnd.lotus-screencam", "scm");
export const Application_Vendor_Lotus_Wordpro = application("vnd.lotus-wordpro", "lwp");
export const Application_Vendor_MacportsPortpkg = application("vnd.macports.portpkg", "portpkg");
export const Application_Vendor_Mapbox_Vector_Tile = application("vnd.mapbox-vector-tile");
export const Application_Vendor_MarlinDrmActiontokenXml = application("vnd.marlin.drm.actiontoken+xml", "xml");
export const Application_Vendor_MarlinDrmConftokenXml = application("vnd.marlin.drm.conftoken+xml", "xml");
export const Application_Vendor_MarlinDrmLicenseXml = application("vnd.marlin.drm.license+xml", "xml");
export const Application_Vendor_MarlinDrmMdcf = application("vnd.marlin.drm.mdcf");
export const Application_Vendor_MasonJson = application("vnd.mason+json", "json");
export const Application_Vendor_MaxmindMaxmind_Db = application("vnd.maxmind.maxmind-db");
export const Application_Vendor_Mcd = application("vnd.mcd", "mcd");
export const Application_Vendor_Medcalcdata = application("vnd.medcalcdata", "mc1");
export const Application_Vendor_MediastationCdkey = application("vnd.mediastation.cdkey", "cdkey");
export const Application_Vendor_Meridian_Slingshot = application("vnd.meridian-slingshot");
export const Application_Vendor_MFER = application("vnd.mfer", "mwf");
export const Application_Vendor_Mfmp = application("vnd.mfmp", "mfm");
export const Application_Vendor_MicrografxFlo = application("vnd.micrografx.flo", "flo");
export const Application_Vendor_MicrografxIgx = application("vnd.micrografx.igx", "igx");
export const Application_Vendor_MicroJson = application("vnd.micro+json", "json");
export const Application_Vendor_MicrosoftPortable_Executable = application("vnd.microsoft.portable-executable");
export const Application_Vendor_MicrosoftWindowsThumbnail_Cache = application("vnd.microsoft.windows.thumbnail-cache");
export const Application_Vendor_MieleJson = application("vnd.miele+json", "json");
export const Application_Vendor_Mif = application("vnd.mif", "mif");
export const Application_Vendor_Minisoft_Hp3000_Save = application("vnd.minisoft-hp3000-save");
export const Application_Vendor_MitsubishiMisty_GuardTrustweb = application("vnd.mitsubishi.misty-guard.trustweb");
export const Application_Vendor_MobiusDAF = application("vnd.mobius.daf", "daf");
export const Application_Vendor_MobiusDIS = application("vnd.mobius.dis", "dis");
export const Application_Vendor_MobiusMBK = application("vnd.mobius.mbk", "mbk");
export const Application_Vendor_MobiusMQY = application("vnd.mobius.mqy", "mqy");
export const Application_Vendor_MobiusMSL = application("vnd.mobius.msl", "msl");
export const Application_Vendor_MobiusPLC = application("vnd.mobius.plc", "plc");
export const Application_Vendor_MobiusTXF = application("vnd.mobius.txf", "txf");
export const Application_Vendor_MophunApplication = application("vnd.mophun.application", "mpn");
export const Application_Vendor_MophunCertificate = application("vnd.mophun.certificate", "mpc");
export const Application_Vendor_MotorolaFlexsuite = application("vnd.motorola.flexsuite");
export const Application_Vendor_MotorolaFlexsuiteAdsi = application("vnd.motorola.flexsuite.adsi");
export const Application_Vendor_MotorolaFlexsuiteFis = application("vnd.motorola.flexsuite.fis");
export const Application_Vendor_MotorolaFlexsuiteGotap = application("vnd.motorola.flexsuite.gotap");
export const Application_Vendor_MotorolaFlexsuiteKmr = application("vnd.motorola.flexsuite.kmr");
export const Application_Vendor_MotorolaFlexsuiteTtc = application("vnd.motorola.flexsuite.ttc");
export const Application_Vendor_MotorolaFlexsuiteWem = application("vnd.motorola.flexsuite.wem");
export const Application_Vendor_MotorolaIprm = application("vnd.motorola.iprm");
export const Application_Vendor_MozillaXulXml = application("vnd.mozilla.xul+xml", "xul");
export const Application_Vendor_Ms_3mfdocument = application("vnd.ms-3mfdocument");
export const Application_Vendor_Ms_Artgalry = application("vnd.ms-artgalry", "cil");
export const Application_Vendor_Ms_Asf = application("vnd.ms-asf");
export const Application_Vendor_Ms_Cab_Compressed = application("vnd.ms-cab-compressed", "cab");
export const Application_Vendor_Ms_ColorIccprofile = application("vnd.ms-color.iccprofile");
export const Application_Vendor_Ms_Excel = application("vnd.ms-excel", "xls", "xlm", "xla", "xlc", "xlt", "xlw");
export const Application_Vendor_Ms_ExcelAddinMacroEnabled12 = application("vnd.ms-excel.addin.macroenabled.12", "xlam");
export const Application_Vendor_Ms_ExcelSheetBinaryMacroEnabled12 = application("vnd.ms-excel.sheet.binary.macroenabled.12", "xlsb");
export const Application_Vendor_Ms_ExcelSheetMacroEnabled12 = application("vnd.ms-excel.sheet.macroenabled.12", "xlsm");
export const Application_Vendor_Ms_ExcelTemplateMacroEnabled12 = application("vnd.ms-excel.template.macroenabled.12", "xltm");
export const Application_Vendor_Ms_Fontobject = application("vnd.ms-fontobject", "eot");
export const Application_Vendor_Ms_Htmlhelp = application("vnd.ms-htmlhelp", "chm");
export const Application_Vendor_Ms_Ims = application("vnd.ms-ims", "ims");
export const Application_Vendor_Ms_Lrm = application("vnd.ms-lrm", "lrm");
export const Application_Vendor_Ms_OfficeActiveXXml = application("vnd.ms-office.activex+xml", "xml");
export const Application_Vendor_Ms_Officetheme = application("vnd.ms-officetheme", "thmx");
export const Application_Vendor_Ms_Opentype = application("vnd.ms-opentype");
export const Application_Vendor_Ms_Outlook = application("vnd.ms-outlook", "msg");
export const Application_Vendor_Ms_PackageObfuscated_Opentype = application("vnd.ms-package.obfuscated-opentype");
export const Application_Vendor_Ms_PkiSeccat = application("vnd.ms-pki.seccat", "cat");
export const Application_Vendor_Ms_PkiStl = application("vnd.ms-pki.stl", "stl");
export const Application_Vendor_Ms_PlayreadyInitiatorXml = application("vnd.ms-playready.initiator+xml", "xml");
export const Application_Vendor_Ms_Powerpoint = application("vnd.ms-powerpoint", "ppt", "pps", "pot");
export const Application_Vendor_Ms_PowerpointAddinMacroEnabled12 = application("vnd.ms-powerpoint.addin.macroenabled.12", "ppam");
export const Application_Vendor_Ms_PowerpointPresentationMacroEnabled12 = application("vnd.ms-powerpoint.presentation.macroenabled.12", "pptm");
export const Application_Vendor_Ms_PowerpointSlideMacroEnabled12 = application("vnd.ms-powerpoint.slide.macroenabled.12", "sldm");
export const Application_Vendor_Ms_PowerpointSlideshowMacroEnabled12 = application("vnd.ms-powerpoint.slideshow.macroenabled.12", "ppsm");
export const Application_Vendor_Ms_PowerpointTemplateMacroEnabled12 = application("vnd.ms-powerpoint.template.macroenabled.12", "potm");
export const Application_Vendor_Ms_PrintDeviceCapabilitiesXml = application("vnd.ms-printdevicecapabilities+xml", "xml");
export const Application_Vendor_Ms_PrintingPrintticketXml = application("vnd.ms-printing.printticket+xml", "xml");
export const Application_Vendor_Ms_PrintSchemaTicketXml = application("vnd.ms-printschematicket+xml", "xml");
export const Application_Vendor_Ms_Project = application("vnd.ms-project", "mpp", "mpt");
export const Application_Vendor_Ms_Tnef = application("vnd.ms-tnef");
export const Application_Vendor_Ms_WindowsDevicepairing = application("vnd.ms-windows.devicepairing");
export const Application_Vendor_Ms_WindowsNwprintingOob = application("vnd.ms-windows.nwprinting.oob");
export const Application_Vendor_Ms_WindowsPrinterpairing = application("vnd.ms-windows.printerpairing");
export const Application_Vendor_Ms_WindowsWsdOob = application("vnd.ms-windows.wsd.oob");
export const Application_Vendor_Ms_WmdrmLic_Chlg_Req = application("vnd.ms-wmdrm.lic-chlg-req");
export const Application_Vendor_Ms_WmdrmLic_Resp = application("vnd.ms-wmdrm.lic-resp");
export const Application_Vendor_Ms_WmdrmMeter_Chlg_Req = application("vnd.ms-wmdrm.meter-chlg-req");
export const Application_Vendor_Ms_WmdrmMeter_Resp = application("vnd.ms-wmdrm.meter-resp");
export const Application_Vendor_Ms_WordDocumentMacroEnabled12 = application("vnd.ms-word.document.macroenabled.12", "docm");
export const Application_Vendor_Ms_WordTemplateMacroEnabled12 = application("vnd.ms-word.template.macroenabled.12", "dotm");
export const Application_Vendor_Ms_Works = application("vnd.ms-works", "wps", "wks", "wcm", "wdb");
export const Application_Vendor_Ms_Wpl = application("vnd.ms-wpl", "wpl");
export const Application_Vendor_Ms_Xpsdocument = application("vnd.ms-xpsdocument", "xps");
export const Application_Vendor_Msa_Disk_Image = application("vnd.msa-disk-image");
export const Application_Vendor_Mseq = application("vnd.mseq", "mseq");
export const Application_Vendor_Msign = application("vnd.msign");
export const Application_Vendor_MultiadCreator = application("vnd.multiad.creator");
export const Application_Vendor_MultiadCreatorCif = application("vnd.multiad.creator.cif");
export const Application_Vendor_Music_Niff = application("vnd.music-niff");
export const Application_Vendor_Musician = application("vnd.musician", "mus");
export const Application_Vendor_MuveeStyle = application("vnd.muvee.style", "msty");
export const Application_Vendor_Mynfc = application("vnd.mynfc", "taglet");
export const Application_Vendor_NcdControl = application("vnd.ncd.control");
export const Application_Vendor_NcdReference = application("vnd.ncd.reference");
export const Application_Vendor_NearstInvJson = application("vnd.nearst.inv+json", "json");
export const Application_Vendor_Nervana = application("vnd.nervana");
export const Application_Vendor_Netfpx = application("vnd.netfpx");
export const Application_Vendor_NeurolanguageNlu = application("vnd.neurolanguage.nlu", "nlu");
export const Application_Vendor_Nimn = application("vnd.nimn");
export const Application_Vendor_NintendoNitroRom = application("vnd.nintendo.nitro.rom");
export const Application_Vendor_NintendoSnesRom = application("vnd.nintendo.snes.rom");
export const Application_Vendor_Nitf = application("vnd.nitf", "ntf", "nitf");
export const Application_Vendor_Noblenet_Directory = application("vnd.noblenet-directory", "nnd");
export const Application_Vendor_Noblenet_Sealer = application("vnd.noblenet-sealer", "nns");
export const Application_Vendor_Noblenet_Web = application("vnd.noblenet-web", "nnw");
export const Application_Vendor_NokiaCatalogs = application("vnd.nokia.catalogs");
export const Application_Vendor_NokiaConmlWbxml = application("vnd.nokia.conml+wbxml", "wbxml");
export const Application_Vendor_NokiaConmlXml = application("vnd.nokia.conml+xml", "xml");
export const Application_Vendor_NokiaIptvConfigXml = application("vnd.nokia.iptv.config+xml", "xml");
export const Application_Vendor_NokiaISDS_Radio_Presets = application("vnd.nokia.isds-radio-presets");
export const Application_Vendor_NokiaLandmarkcollectionXml = application("vnd.nokia.landmarkcollection+xml", "xml");
export const Application_Vendor_NokiaLandmarkWbxml = application("vnd.nokia.landmark+wbxml", "wbxml");
export const Application_Vendor_NokiaLandmarkXml = application("vnd.nokia.landmark+xml", "xml");
export const Application_Vendor_NokiaN_GageAcXml = application("vnd.nokia.n-gage.ac+xml", "xml");
export const Application_Vendor_NokiaN_GageData = application("vnd.nokia.n-gage.data", "ngdat");
export const Application_Vendor_NokiaN_GageSymbianInstall = deprecate(application("vnd.nokia.n-gage.symbian.install", "n-gage"), "as obsolete with no replacement given");
export const Application_Vendor_NokiaNcd = application("vnd.nokia.ncd");
export const Application_Vendor_NokiaPcdWbxml = application("vnd.nokia.pcd+wbxml", "wbxml");
export const Application_Vendor_NokiaPcdXml = application("vnd.nokia.pcd+xml", "xml");
export const Application_Vendor_NokiaRadio_Preset = application("vnd.nokia.radio-preset", "rpst");
export const Application_Vendor_NokiaRadio_Presets = application("vnd.nokia.radio-presets", "rpss");
export const Application_Vendor_NovadigmEDM = application("vnd.novadigm.edm", "edm");
export const Application_Vendor_NovadigmEDX = application("vnd.novadigm.edx", "edx");
export const Application_Vendor_NovadigmEXT = application("vnd.novadigm.ext", "ext");
export const Application_Vendor_Ntt_LocalContent_Share = application("vnd.ntt-local.content-share");
export const Application_Vendor_Ntt_LocalFile_Transfer = application("vnd.ntt-local.file-transfer");
export const Application_Vendor_Ntt_LocalOgw_remote_Access = application("vnd.ntt-local.ogw_remote-access");
export const Application_Vendor_Ntt_LocalSip_Ta_remote = application("vnd.ntt-local.sip-ta_remote");
export const Application_Vendor_Ntt_LocalSip_Ta_tcp_stream = application("vnd.ntt-local.sip-ta_tcp_stream");
export const Application_Vendor_OasisOpendocumentChart = application("vnd.oasis.opendocument.chart", "odc");
export const Application_Vendor_OasisOpendocumentChart_Template = application("vnd.oasis.opendocument.chart-template", "otc");
export const Application_Vendor_OasisOpendocumentDatabase = application("vnd.oasis.opendocument.database", "odb");
export const Application_Vendor_OasisOpendocumentFormula = application("vnd.oasis.opendocument.formula", "odf");
export const Application_Vendor_OasisOpendocumentFormula_Template = application("vnd.oasis.opendocument.formula-template", "odft");
export const Application_Vendor_OasisOpendocumentGraphics = application("vnd.oasis.opendocument.graphics", "odg");
export const Application_Vendor_OasisOpendocumentGraphics_Template = application("vnd.oasis.opendocument.graphics-template", "otg");
export const Application_Vendor_OasisOpendocumentImage = application("vnd.oasis.opendocument.image", "odi");
export const Application_Vendor_OasisOpendocumentImage_Template = application("vnd.oasis.opendocument.image-template", "oti");
export const Application_Vendor_OasisOpendocumentPresentation = application("vnd.oasis.opendocument.presentation", "odp");
export const Application_Vendor_OasisOpendocumentPresentation_Template = application("vnd.oasis.opendocument.presentation-template", "otp");
export const Application_Vendor_OasisOpendocumentSpreadsheet = application("vnd.oasis.opendocument.spreadsheet", "ods");
export const Application_Vendor_OasisOpendocumentSpreadsheet_Template = application("vnd.oasis.opendocument.spreadsheet-template", "ots");
export const Application_Vendor_OasisOpendocumentText = application("vnd.oasis.opendocument.text", "odt");
export const Application_Vendor_OasisOpendocumentText_Master = application("vnd.oasis.opendocument.text-master", "odm");
export const Application_Vendor_OasisOpendocumentText_Template = application("vnd.oasis.opendocument.text-template", "ott");
export const Application_Vendor_OasisOpendocumentText_Web = application("vnd.oasis.opendocument.text-web", "oth");
export const Application_Vendor_Obn = application("vnd.obn");
export const Application_Vendor_OcfCbor = application("vnd.ocf+cbor", "cbor");
export const Application_Vendor_OftnL10nJson = application("vnd.oftn.l10n+json", "json");
export const Application_Vendor_OipfContentaccessdownloadXml = application("vnd.oipf.contentaccessdownload+xml", "xml");
export const Application_Vendor_OipfContentaccessstreamingXml = application("vnd.oipf.contentaccessstreaming+xml", "xml");
export const Application_Vendor_OipfCspg_Hexbinary = application("vnd.oipf.cspg-hexbinary");
export const Application_Vendor_OipfDaeSvgXml = application("vnd.oipf.dae.svg+xml", "xml");
export const Application_Vendor_OipfDaeXhtmlXml = application("vnd.oipf.dae.xhtml+xml", "xml");
export const Application_Vendor_OipfMippvcontrolmessageXml = application("vnd.oipf.mippvcontrolmessage+xml", "xml");
export const Application_Vendor_OipfPaeGem = application("vnd.oipf.pae.gem");
export const Application_Vendor_OipfSpdiscoveryXml = application("vnd.oipf.spdiscovery+xml", "xml");
export const Application_Vendor_OipfSpdlistXml = application("vnd.oipf.spdlist+xml", "xml");
export const Application_Vendor_OipfUeprofileXml = application("vnd.oipf.ueprofile+xml", "xml");
export const Application_Vendor_OipfUserprofileXml = application("vnd.oipf.userprofile+xml", "xml");
export const Application_Vendor_Olpc_Sugar = application("vnd.olpc-sugar", "xo");
export const Application_Vendor_Oma_Scws_Config = application("vnd.oma-scws-config");
export const Application_Vendor_Oma_Scws_Http_Request = application("vnd.oma-scws-http-request");
export const Application_Vendor_Oma_Scws_Http_Response = application("vnd.oma-scws-http-response");
export const Application_Vendor_OmaBcastAssociated_Procedure_ParameterXml = application("vnd.oma.bcast.associated-procedure-parameter+xml", "xml");
export const Application_Vendor_OmaBcastDrm_TriggerXml = application("vnd.oma.bcast.drm-trigger+xml", "xml");
export const Application_Vendor_OmaBcastImdXml = application("vnd.oma.bcast.imd+xml", "xml");
export const Application_Vendor_OmaBcastLtkm = application("vnd.oma.bcast.ltkm");
export const Application_Vendor_OmaBcastNotificationXml = application("vnd.oma.bcast.notification+xml", "xml");
export const Application_Vendor_OmaBcastProvisioningtrigger = application("vnd.oma.bcast.provisioningtrigger");
export const Application_Vendor_OmaBcastSgboot = application("vnd.oma.bcast.sgboot");
export const Application_Vendor_OmaBcastSgddXml = application("vnd.oma.bcast.sgdd+xml", "xml");
export const Application_Vendor_OmaBcastSgdu = application("vnd.oma.bcast.sgdu");
export const Application_Vendor_OmaBcastSimple_Symbol_Container = application("vnd.oma.bcast.simple-symbol-container");
export const Application_Vendor_OmaBcastSmartcard_TriggerXml = application("vnd.oma.bcast.smartcard-trigger+xml", "xml");
export const Application_Vendor_OmaBcastSprovXml = application("vnd.oma.bcast.sprov+xml", "xml");
export const Application_Vendor_OmaBcastStkm = application("vnd.oma.bcast.stkm");
export const Application_Vendor_OmaCab_Address_BookXml = application("vnd.oma.cab-address-book+xml", "xml");
export const Application_Vendor_OmaCab_Feature_HandlerXml = application("vnd.oma.cab-feature-handler+xml", "xml");
export const Application_Vendor_OmaCab_PccXml = application("vnd.oma.cab-pcc+xml", "xml");
export const Application_Vendor_OmaCab_Subs_InviteXml = application("vnd.oma.cab-subs-invite+xml", "xml");
export const Application_Vendor_OmaCab_User_PrefsXml = application("vnd.oma.cab-user-prefs+xml", "xml");
export const Application_Vendor_OmaDcd = application("vnd.oma.dcd");
export const Application_Vendor_OmaDcdc = application("vnd.oma.dcdc");
export const Application_Vendor_OmaDd2Xml = application("vnd.oma.dd2+xml", "dd2");
export const Application_Vendor_OmaDrmRisdXml = application("vnd.oma.drm.risd+xml", "xml");
export const Application_Vendor_Omads_EmailXml = application("vnd.omads-email+xml", "xml");
export const Application_Vendor_Omads_FileXml = application("vnd.omads-file+xml", "xml");
export const Application_Vendor_Omads_FolderXml = application("vnd.omads-folder+xml", "xml");
export const Application_Vendor_OmaGroup_Usage_ListXml = application("vnd.oma.group-usage-list+xml", "xml");
export const Application_Vendor_Omaloc_Supl_Init = application("vnd.omaloc-supl-init");
export const Application_Vendor_OmaLwm2mJson = application("vnd.oma.lwm2m+json", "json");
export const Application_Vendor_OmaLwm2mTlv = application("vnd.oma.lwm2m+tlv", "tlv");
export const Application_Vendor_OmaPalXml = application("vnd.oma.pal+xml", "xml");
export const Application_Vendor_OmaPocDetailed_Progress_ReportXml = application("vnd.oma.poc.detailed-progress-report+xml", "xml");
export const Application_Vendor_OmaPocFinal_ReportXml = application("vnd.oma.poc.final-report+xml", "xml");
export const Application_Vendor_OmaPocGroupsXml = application("vnd.oma.poc.groups+xml", "xml");
export const Application_Vendor_OmaPocInvocation_DescriptorXml = application("vnd.oma.poc.invocation-descriptor+xml", "xml");
export const Application_Vendor_OmaPocOptimized_Progress_ReportXml = application("vnd.oma.poc.optimized-progress-report+xml", "xml");
export const Application_Vendor_OmaPush = application("vnd.oma.push");
export const Application_Vendor_OmaScidmMessagesXml = application("vnd.oma.scidm.messages+xml", "xml");
export const Application_Vendor_OmaXcap_DirectoryXml = application("vnd.oma.xcap-directory+xml", "xml");
export const Application_Vendor_Onepager = application("vnd.onepager");
export const Application_Vendor_Onepagertamp = application("vnd.onepagertamp");
export const Application_Vendor_Onepagertamx = application("vnd.onepagertamx");
export const Application_Vendor_Onepagertat = application("vnd.onepagertat");
export const Application_Vendor_Onepagertatp = application("vnd.onepagertatp");
export const Application_Vendor_Onepagertatx = application("vnd.onepagertatx");
export const Application_Vendor_OpenbloxGame_Binary = application("vnd.openblox.game-binary");
export const Application_Vendor_OpenbloxGameXml = application("vnd.openblox.game+xml", "xml");
export const Application_Vendor_OpeneyeOeb = application("vnd.openeye.oeb");
export const Application_Vendor_OpenofficeorgExtension = application("vnd.openofficeorg.extension", "oxt");
export const Application_Vendor_OpenstreetmapDataXml = application("vnd.openstreetmap.data+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentCustom_PropertiesXml = application("vnd.openxmlformats-officedocument.custom-properties+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentCustomXmlPropertiesXml = application("vnd.openxmlformats-officedocument.customxmlproperties+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentDrawingmlChartshapesXml = application("vnd.openxmlformats-officedocument.drawingml.chartshapes+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentDrawingmlChartXml = application("vnd.openxmlformats-officedocument.drawingml.chart+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentDrawingmlDiagramColorsXml = application("vnd.openxmlformats-officedocument.drawingml.diagramcolors+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentDrawingmlDiagramDataXml = application("vnd.openxmlformats-officedocument.drawingml.diagramdata+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentDrawingmlDiagramLayoutXml = application("vnd.openxmlformats-officedocument.drawingml.diagramlayout+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentDrawingmlDiagramStyleXml = application("vnd.openxmlformats-officedocument.drawingml.diagramstyle+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentDrawingXml = application("vnd.openxmlformats-officedocument.drawing+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentExtended_PropertiesXml = application("vnd.openxmlformats-officedocument.extended-properties+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentPresentationmlCommentAuthorsXml = application("vnd.openxmlformats-officedocument.presentationml.commentauthors+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentPresentationmlCommentsXml = application("vnd.openxmlformats-officedocument.presentationml.comments+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentPresentationmlHandoutMasterXml = application("vnd.openxmlformats-officedocument.presentationml.handoutmaster+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentPresentationmlNotesMasterXml = application("vnd.openxmlformats-officedocument.presentationml.notesmaster+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentPresentationmlNotesSlideXml = application("vnd.openxmlformats-officedocument.presentationml.notesslide+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentPresentationmlPresentation = application("vnd.openxmlformats-officedocument.presentationml.presentation", "pptx");
export const Application_Vendor_Openxmlformats_OfficedocumentPresentationmlPresentationMainXml = application("vnd.openxmlformats-officedocument.presentationml.presentation.main+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentPresentationmlPresPropsXml = application("vnd.openxmlformats-officedocument.presentationml.presprops+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentPresentationmlSlide = application("vnd.openxmlformats-officedocument.presentationml.slide", "sldx");
export const Application_Vendor_Openxmlformats_OfficedocumentPresentationmlSlideLayoutXml = application("vnd.openxmlformats-officedocument.presentationml.slidelayout+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentPresentationmlSlideMasterXml = application("vnd.openxmlformats-officedocument.presentationml.slidemaster+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentPresentationmlSlideshow = application("vnd.openxmlformats-officedocument.presentationml.slideshow", "ppsx");
export const Application_Vendor_Openxmlformats_OfficedocumentPresentationmlSlideshowMainXml = application("vnd.openxmlformats-officedocument.presentationml.slideshow.main+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentPresentationmlSlideUpdateInfoXml = application("vnd.openxmlformats-officedocument.presentationml.slideupdateinfo+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentPresentationmlSlideXml = application("vnd.openxmlformats-officedocument.presentationml.slide+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentPresentationmlTableStylesXml = application("vnd.openxmlformats-officedocument.presentationml.tablestyles+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentPresentationmlTagsXml = application("vnd.openxmlformats-officedocument.presentationml.tags+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentPresentationmlTemplate = application("vnd.openxmlformats-officedocument.presentationml.template", "potx");
export const Application_Vendor_Openxmlformats_OfficedocumentPresentationmlTemplateMainXml = application("vnd.openxmlformats-officedocument.presentationml.template.main+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentPresentationmlViewPropsXml = application("vnd.openxmlformats-officedocument.presentationml.viewprops+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlCalcChainXml = application("vnd.openxmlformats-officedocument.spreadsheetml.calcchain+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlChartsheetXml = application("vnd.openxmlformats-officedocument.spreadsheetml.chartsheet+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlCommentsXml = application("vnd.openxmlformats-officedocument.spreadsheetml.comments+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlConnectionsXml = application("vnd.openxmlformats-officedocument.spreadsheetml.connections+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlDialogsheetXml = application("vnd.openxmlformats-officedocument.spreadsheetml.dialogsheet+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlExternalLinkXml = application("vnd.openxmlformats-officedocument.spreadsheetml.externallink+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlPivotCacheDefinitionXml = application("vnd.openxmlformats-officedocument.spreadsheetml.pivotcachedefinition+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlPivotCacheRecordsXml = application("vnd.openxmlformats-officedocument.spreadsheetml.pivotcacherecords+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlPivotTableXml = application("vnd.openxmlformats-officedocument.spreadsheetml.pivottable+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlQueryTableXml = application("vnd.openxmlformats-officedocument.spreadsheetml.querytable+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlRevisionHeadersXml = application("vnd.openxmlformats-officedocument.spreadsheetml.revisionheaders+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlRevisionLogXml = application("vnd.openxmlformats-officedocument.spreadsheetml.revisionlog+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlSharedStringsXml = application("vnd.openxmlformats-officedocument.spreadsheetml.sharedstrings+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlSheet = application("vnd.openxmlformats-officedocument.spreadsheetml.sheet", "xlsx");
export const Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlSheetMainXml = application("vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlSheetMetadataXml = application("vnd.openxmlformats-officedocument.spreadsheetml.sheetmetadata+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlStylesXml = application("vnd.openxmlformats-officedocument.spreadsheetml.styles+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlTableSingleCellsXml = application("vnd.openxmlformats-officedocument.spreadsheetml.tablesinglecells+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlTableXml = application("vnd.openxmlformats-officedocument.spreadsheetml.table+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlTemplate = application("vnd.openxmlformats-officedocument.spreadsheetml.template", "xltx");
export const Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlTemplateMainXml = application("vnd.openxmlformats-officedocument.spreadsheetml.template.main+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlUserNamesXml = application("vnd.openxmlformats-officedocument.spreadsheetml.usernames+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlVolatileDependenciesXml = application("vnd.openxmlformats-officedocument.spreadsheetml.volatiledependencies+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlWorksheetXml = application("vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentThemeOverrideXml = application("vnd.openxmlformats-officedocument.themeoverride+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentThemeXml = application("vnd.openxmlformats-officedocument.theme+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentVmlDrawing = application("vnd.openxmlformats-officedocument.vmldrawing");
export const Application_Vendor_Openxmlformats_OfficedocumentWordprocessingmlCommentsXml = application("vnd.openxmlformats-officedocument.wordprocessingml.comments+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentWordprocessingmlDocument = application("vnd.openxmlformats-officedocument.wordprocessingml.document", "docx");
export const Application_Vendor_Openxmlformats_OfficedocumentWordprocessingmlDocumentGlossaryXml = application("vnd.openxmlformats-officedocument.wordprocessingml.document.glossary+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentWordprocessingmlDocumentMainXml = application("vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentWordprocessingmlEndnotesXml = application("vnd.openxmlformats-officedocument.wordprocessingml.endnotes+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentWordprocessingmlFontTableXml = application("vnd.openxmlformats-officedocument.wordprocessingml.fonttable+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentWordprocessingmlFooterXml = application("vnd.openxmlformats-officedocument.wordprocessingml.footer+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentWordprocessingmlFootnotesXml = application("vnd.openxmlformats-officedocument.wordprocessingml.footnotes+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentWordprocessingmlNumberingXml = application("vnd.openxmlformats-officedocument.wordprocessingml.numbering+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentWordprocessingmlSettingsXml = application("vnd.openxmlformats-officedocument.wordprocessingml.settings+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentWordprocessingmlStylesXml = application("vnd.openxmlformats-officedocument.wordprocessingml.styles+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentWordprocessingmlTemplate = application("vnd.openxmlformats-officedocument.wordprocessingml.template", "dotx");
export const Application_Vendor_Openxmlformats_OfficedocumentWordprocessingmlTemplateMainXml = application("vnd.openxmlformats-officedocument.wordprocessingml.template.main+xml", "xml");
export const Application_Vendor_Openxmlformats_OfficedocumentWordprocessingmlWebSettingsXml = application("vnd.openxmlformats-officedocument.wordprocessingml.websettings+xml", "xml");
export const Application_Vendor_Openxmlformats_PackageCore_PropertiesXml = application("vnd.openxmlformats-package.core-properties+xml", "xml");
export const Application_Vendor_Openxmlformats_PackageDigital_Signature_XmlsignatureXml = application("vnd.openxmlformats-package.digital-signature-xmlsignature+xml", "xml");
export const Application_Vendor_Openxmlformats_PackageRelationshipsXml = application("vnd.openxmlformats-package.relationships+xml", "xml");
export const Application_Vendor_OracleResourceJson = application("vnd.oracle.resource+json", "json");
export const Application_Vendor_OrangeIndata = application("vnd.orange.indata");
export const Application_Vendor_OsaNetdeploy = application("vnd.osa.netdeploy");
export const Application_Vendor_OsgeoMapguidePackage = application("vnd.osgeo.mapguide.package", "mgp");
export const Application_Vendor_OsgiBundle = application("vnd.osgi.bundle");
export const Application_Vendor_OsgiDp = application("vnd.osgi.dp", "dp");
export const Application_Vendor_OsgiSubsystem = application("vnd.osgi.subsystem", "esa");
export const Application_Vendor_OtpsCt_KipXml = application("vnd.otps.ct-kip+xml", "xml");
export const Application_Vendor_OxliCountgraph = application("vnd.oxli.countgraph");
export const Application_Vendor_PagerdutyJson = application("vnd.pagerduty+json", "json");
export const Application_Vendor_Palm = application("vnd.palm", "pdb", "pqa", "oprc");
export const Application_Vendor_Panoply = application("vnd.panoply");
export const Application_Vendor_PaosXml = application("vnd.paos.xml");
export const Application_Vendor_Patentdive = application("vnd.patentdive");
export const Application_Vendor_Patientecommsdoc = application("vnd.patientecommsdoc");
export const Application_Vendor_Pawaafile = application("vnd.pawaafile", "paw");
export const Application_Vendor_Pcos = application("vnd.pcos");
export const Application_Vendor_PgFormat = application("vnd.pg.format", "str");
export const Application_Vendor_PgOsasli = application("vnd.pg.osasli", "ei6");
export const Application_Vendor_PiaccessApplication_Licence = application("vnd.piaccess.application-licence");
export const Application_Vendor_Picsel = application("vnd.picsel", "efif");
export const Application_Vendor_PmiWidget = application("vnd.pmi.widget", "wg");
export const Application_Vendor_PocGroup_AdvertisementXml = application("vnd.poc.group-advertisement+xml", "xml");
export const Application_Vendor_Pocketlearn = application("vnd.pocketlearn", "plf");
export const Application_Vendor_Powerbuilder6 = application("vnd.powerbuilder6", "pbd");
export const Application_Vendor_Powerbuilder6_S = application("vnd.powerbuilder6-s");
export const Application_Vendor_Powerbuilder7 = application("vnd.powerbuilder7");
export const Application_Vendor_Powerbuilder7_S = application("vnd.powerbuilder7-s");
export const Application_Vendor_Powerbuilder75 = application("vnd.powerbuilder75");
export const Application_Vendor_Powerbuilder75_S = application("vnd.powerbuilder75-s");
export const Application_Vendor_Preminet = application("vnd.preminet");
export const Application_Vendor_PreviewsystemsBox = application("vnd.previewsystems.box", "box");
export const Application_Vendor_ProteusMagazine = application("vnd.proteus.magazine", "mgz");
export const Application_Vendor_Psfs = application("vnd.psfs");
export const Application_Vendor_Publishare_Delta_Tree = application("vnd.publishare-delta-tree", "qps");
export const Application_Vendor_PviPtid1 = application("vnd.pvi.ptid1", "ptid");
export const Application_Vendor_Pwg_Multiplexed = application("vnd.pwg-multiplexed");
export const Application_Vendor_Pwg_Xhtml_PrintXml = application("vnd.pwg-xhtml-print+xml", "xml");
export const Application_Vendor_QualcommBrew_App_Res = application("vnd.qualcomm.brew-app-res");
export const Application_Vendor_Quarantainenet = application("vnd.quarantainenet");
export const Application_Vendor_QuarkQuarkXPress = application("vnd.quark.quarkxpress", "qxd", "qxt", "qwd", "qwt", "qxl", "qxb");
export const Application_Vendor_Quobject_Quoxdocument = application("vnd.quobject-quoxdocument");
export const Application_Vendor_RadisysMomlXml = application("vnd.radisys.moml+xml", "xml");
export const Application_Vendor_RadisysMsml_Audit_ConfXml = application("vnd.radisys.msml-audit-conf+xml", "xml");
export const Application_Vendor_RadisysMsml_Audit_ConnXml = application("vnd.radisys.msml-audit-conn+xml", "xml");
export const Application_Vendor_RadisysMsml_Audit_DialogXml = application("vnd.radisys.msml-audit-dialog+xml", "xml");
export const Application_Vendor_RadisysMsml_Audit_StreamXml = application("vnd.radisys.msml-audit-stream+xml", "xml");
export const Application_Vendor_RadisysMsml_AuditXml = application("vnd.radisys.msml-audit+xml", "xml");
export const Application_Vendor_RadisysMsml_ConfXml = application("vnd.radisys.msml-conf+xml", "xml");
export const Application_Vendor_RadisysMsml_Dialog_BaseXml = application("vnd.radisys.msml-dialog-base+xml", "xml");
export const Application_Vendor_RadisysMsml_Dialog_Fax_DetectXml = application("vnd.radisys.msml-dialog-fax-detect+xml", "xml");
export const Application_Vendor_RadisysMsml_Dialog_Fax_SendrecvXml = application("vnd.radisys.msml-dialog-fax-sendrecv+xml", "xml");
export const Application_Vendor_RadisysMsml_Dialog_GroupXml = application("vnd.radisys.msml-dialog-group+xml", "xml");
export const Application_Vendor_RadisysMsml_Dialog_SpeechXml = application("vnd.radisys.msml-dialog-speech+xml", "xml");
export const Application_Vendor_RadisysMsml_Dialog_TransformXml = application("vnd.radisys.msml-dialog-transform+xml", "xml");
export const Application_Vendor_RadisysMsml_DialogXml = application("vnd.radisys.msml-dialog+xml", "xml");
export const Application_Vendor_RadisysMsmlXml = application("vnd.radisys.msml+xml", "xml");
export const Application_Vendor_RainstorData = application("vnd.rainstor.data");
export const Application_Vendor_Rapid = application("vnd.rapid");
export const Application_Vendor_Rar = application("vnd.rar");
export const Application_Vendor_RealvncBed = application("vnd.realvnc.bed", "bed");
export const Application_Vendor_RecordareMusicxml = application("vnd.recordare.musicxml", "mxl");
export const Application_Vendor_RecordareMusicxmlXml = application("vnd.recordare.musicxml+xml", "musicxml");
export const Application_Vendor_RenLearnRlprint = application("vnd.renlearn.rlprint");
export const Application_Vendor_RestfulJson = application("vnd.restful+json", "json");
export const Application_Vendor_RigCryptonote = application("vnd.rig.cryptonote", "cryptonote");
export const Application_Vendor_RimCod = application("vnd.rim.cod", "cod");
export const Application_Vendor_Rn_Realmedia = application("vnd.rn-realmedia", "rm");
export const Application_Vendor_Rn_Realmedia_Vbr = application("vnd.rn-realmedia-vbr", "rmvb");
export const Application_Vendor_Route66Link66Xml = application("vnd.route66.link66+xml", "link66");
export const Application_Vendor_Rs_274x = application("vnd.rs-274x");
export const Application_Vendor_RuckusDownload = application("vnd.ruckus.download");
export const Application_Vendor_S3sms = application("vnd.s3sms");
export const Application_Vendor_SailingtrackerTrack = application("vnd.sailingtracker.track", "st");
export const Application_Vendor_Sar = application("vnd.sar");
export const Application_Vendor_SbmCid = application("vnd.sbm.cid");
export const Application_Vendor_SbmMid2 = application("vnd.sbm.mid2");
export const Application_Vendor_Scribus = application("vnd.scribus");
export const Application_Vendor_Sealed3df = application("vnd.sealed.3df");
export const Application_Vendor_SealedCsf = application("vnd.sealed.csf");
export const Application_Vendor_SealedDoc = application("vnd.sealed.doc");
export const Application_Vendor_SealedEml = application("vnd.sealed.eml");
export const Application_Vendor_SealedmediaSoftsealHtml = application("vnd.sealedmedia.softseal.html");
export const Application_Vendor_SealedmediaSoftsealPdf = application("vnd.sealedmedia.softseal.pdf");
export const Application_Vendor_SealedMht = application("vnd.sealed.mht");
export const Application_Vendor_SealedNet = application("vnd.sealed.net");
export const Application_Vendor_SealedPpt = application("vnd.sealed.ppt");
export const Application_Vendor_SealedTiff = application("vnd.sealed.tiff");
export const Application_Vendor_SealedXls = application("vnd.sealed.xls");
export const Application_Vendor_Seemail = application("vnd.seemail", "see");
export const Application_Vendor_Sema = application("vnd.sema", "sema");
export const Application_Vendor_Semd = application("vnd.semd", "semd");
export const Application_Vendor_Semf = application("vnd.semf", "semf");
export const Application_Vendor_Shade_Save_File = application("vnd.shade-save-file");
export const Application_Vendor_ShanaInformedFormdata = application("vnd.shana.informed.formdata", "ifm");
export const Application_Vendor_ShanaInformedFormtemplate = application("vnd.shana.informed.formtemplate", "itp");
export const Application_Vendor_ShanaInformedInterchange = application("vnd.shana.informed.interchange", "iif");
export const Application_Vendor_ShanaInformedPackage = application("vnd.shana.informed.package", "ipk");
export const Application_Vendor_ShootproofJson = application("vnd.shootproof+json", "json");
export const Application_Vendor_ShopkickJson = application("vnd.shopkick+json", "json");
export const Application_Vendor_SigrokSession = application("vnd.sigrok.session");
export const Application_Vendor_SimTech_MindMapper = application("vnd.simtech-mindmapper", "twd", "twds");
export const Application_Vendor_SirenJson = application("vnd.siren+json", "json");
export const Application_Vendor_Smaf = application("vnd.smaf", "mmf");
export const Application_Vendor_SmartNotebook = application("vnd.smart.notebook");
export const Application_Vendor_SmartTeacher = application("vnd.smart.teacher", "teacher");
export const Application_Vendor_Software602FillerForm_Xml_Zip = application("vnd.software602.filler.form-xml-zip");
export const Application_Vendor_Software602FillerFormXml = application("vnd.software602.filler.form+xml", "xml");
export const Application_Vendor_SolentSdkmXml = application("vnd.solent.sdkm+xml", "sdkm", "sdkd");
export const Application_Vendor_SpotfireDxp = application("vnd.spotfire.dxp", "dxp");
export const Application_Vendor_SpotfireSfs = application("vnd.spotfire.sfs", "sfs");
export const Application_Vendor_Sqlite3 = application("vnd.sqlite3");
export const Application_Vendor_Sss_Cod = application("vnd.sss-cod");
export const Application_Vendor_Sss_Dtf = application("vnd.sss-dtf");
export const Application_Vendor_Sss_Ntf = application("vnd.sss-ntf");
export const Application_Vendor_StardivisionCalc = application("vnd.stardivision.calc", "sdc");
export const Application_Vendor_StardivisionDraw = application("vnd.stardivision.draw", "sda");
export const Application_Vendor_StardivisionImpress = application("vnd.stardivision.impress", "sdd");
export const Application_Vendor_StardivisionMath = application("vnd.stardivision.math", "smf");
export const Application_Vendor_StardivisionWriter = application("vnd.stardivision.writer", "sdw", "vor");
export const Application_Vendor_StardivisionWriter_Global = application("vnd.stardivision.writer-global", "sgl");
export const Application_Vendor_StepmaniaPackage = application("vnd.stepmania.package", "smzip");
export const Application_Vendor_StepmaniaStepchart = application("vnd.stepmania.stepchart", "sm");
export const Application_Vendor_Street_Stream = application("vnd.street-stream");
export const Application_Vendor_SunWadlXml = application("vnd.sun.wadl+xml", "xml");
export const Application_Vendor_SunXmlCalc = application("vnd.sun.xml.calc", "sxc");
export const Application_Vendor_SunXmlCalcTemplate = application("vnd.sun.xml.calc.template", "stc");
export const Application_Vendor_SunXmlDraw = application("vnd.sun.xml.draw", "sxd");
export const Application_Vendor_SunXmlDrawTemplate = application("vnd.sun.xml.draw.template", "std");
export const Application_Vendor_SunXmlImpress = application("vnd.sun.xml.impress", "sxi");
export const Application_Vendor_SunXmlImpressTemplate = application("vnd.sun.xml.impress.template", "sti");
export const Application_Vendor_SunXmlMath = application("vnd.sun.xml.math", "sxm");
export const Application_Vendor_SunXmlWriter = application("vnd.sun.xml.writer", "sxw");
export const Application_Vendor_SunXmlWriterGlobal = application("vnd.sun.xml.writer.global", "sxg");
export const Application_Vendor_SunXmlWriterTemplate = application("vnd.sun.xml.writer.template", "stw");
export const Application_Vendor_Sus_Calendar = application("vnd.sus-calendar", "sus", "susp");
export const Application_Vendor_Svd = application("vnd.svd", "svd");
export const Application_Vendor_Swiftview_Ics = application("vnd.swiftview-ics");
export const Application_Vendor_SymbianInstall = application("vnd.symbian.install", "sis", "sisx");
export const Application_Vendor_SyncmlDmddfWbxml = application("vnd.syncml.dmddf+wbxml", "wbxml");
export const Application_Vendor_SyncmlDmddfXml = application("vnd.syncml.dmddf+xml", "xml");
export const Application_Vendor_SyncmlDmNotification = application("vnd.syncml.dm.notification");
export const Application_Vendor_SyncmlDmtndsWbxml = application("vnd.syncml.dmtnds+wbxml", "wbxml");
export const Application_Vendor_SyncmlDmtndsXml = application("vnd.syncml.dmtnds+xml", "xml");
export const Application_Vendor_SyncmlDmWbxml = application("vnd.syncml.dm+wbxml", "bdm");
export const Application_Vendor_SyncmlDmXml = application("vnd.syncml.dm+xml", "xdm");
export const Application_Vendor_SyncmlDsNotification = application("vnd.syncml.ds.notification");
export const Application_Vendor_SyncmlXml = application("vnd.syncml+xml", "xsm");
export const Application_Vendor_TableschemaJson = application("vnd.tableschema+json", "json");
export const Application_Vendor_TaoIntent_Module_Archive = application("vnd.tao.intent-module-archive", "tao");
export const Application_Vendor_TcpdumpPcap = application("vnd.tcpdump.pcap", "pcap", "cap", "dmp");
export const Application_Vendor_Think_CellPpttcJson = application("vnd.think-cell.ppttc+json", "json");
export const Application_Vendor_TmdMediaflexApiXml = application("vnd.tmd.mediaflex.api+xml", "xml");
export const Application_Vendor_Tml = application("vnd.tml");
export const Application_Vendor_Tmobile_Livetv = application("vnd.tmobile-livetv", "tmo");
export const Application_Vendor_TridTpt = application("vnd.trid.tpt", "tpt");
export const Application_Vendor_TriOnesource = application("vnd.tri.onesource");
export const Application_Vendor_TriscapeMxs = application("vnd.triscape.mxs", "mxs");
export const Application_Vendor_Trueapp = application("vnd.trueapp", "tra");
export const Application_Vendor_Truedoc = application("vnd.truedoc");
export const Application_Vendor_UbisoftWebplayer = application("vnd.ubisoft.webplayer");
export const Application_Vendor_Ufdl = application("vnd.ufdl", "ufd", "ufdl");
export const Application_Vendor_UiqTheme = application("vnd.uiq.theme", "utz");
export const Application_Vendor_Umajin = application("vnd.umajin", "umj");
export const Application_Vendor_Unity = application("vnd.unity", "unityweb");
export const Application_Vendor_UomlXml = application("vnd.uoml+xml", "uoml");
export const Application_Vendor_UplanetAlert = application("vnd.uplanet.alert");
export const Application_Vendor_UplanetAlert_Wbxml = application("vnd.uplanet.alert-wbxml");
export const Application_Vendor_UplanetBearer_Choice = application("vnd.uplanet.bearer-choice");
export const Application_Vendor_UplanetBearer_Choice_Wbxml = application("vnd.uplanet.bearer-choice-wbxml");
export const Application_Vendor_UplanetCacheop = application("vnd.uplanet.cacheop");
export const Application_Vendor_UplanetCacheop_Wbxml = application("vnd.uplanet.cacheop-wbxml");
export const Application_Vendor_UplanetChannel = application("vnd.uplanet.channel");
export const Application_Vendor_UplanetChannel_Wbxml = application("vnd.uplanet.channel-wbxml");
export const Application_Vendor_UplanetList = application("vnd.uplanet.list");
export const Application_Vendor_UplanetList_Wbxml = application("vnd.uplanet.list-wbxml");
export const Application_Vendor_UplanetListcmd = application("vnd.uplanet.listcmd");
export const Application_Vendor_UplanetListcmd_Wbxml = application("vnd.uplanet.listcmd-wbxml");
export const Application_Vendor_UplanetSignal = application("vnd.uplanet.signal");
export const Application_Vendor_Uri_Map = application("vnd.uri-map");
export const Application_Vendor_ValveSourceMaterial = application("vnd.valve.source.material");
export const Application_Vendor_Vcx = application("vnd.vcx", "vcx");
export const Application_Vendor_Vd_Study = application("vnd.vd-study");
export const Application_Vendor_Vectorworks = application("vnd.vectorworks");
export const Application_Vendor_VelJson = application("vnd.vel+json", "json");
export const Application_Vendor_VerimatrixVcas = application("vnd.verimatrix.vcas");
export const Application_Vendor_VeryantThin = application("vnd.veryant.thin");
export const Application_Vendor_VesEncrypted = application("vnd.ves.encrypted");
export const Application_Vendor_VidsoftVidconference = application("vnd.vidsoft.vidconference");
export const Application_Vendor_Visio = application("vnd.visio", "vsd", "vst", "vss", "vsw");
export const Application_Vendor_Visionary = application("vnd.visionary", "vis");
export const Application_Vendor_VividenceScriptfile = application("vnd.vividence.scriptfile");
export const Application_Vendor_Vsf = application("vnd.vsf", "vsf");
export const Application_Vendor_WapSic = application("vnd.wap.sic");
export const Application_Vendor_WapSlc = application("vnd.wap.slc");
export const Application_Vendor_WapWbxml = application("vnd.wap.wbxml", "wbxml");
export const Application_Vendor_WapWmlc = application("vnd.wap.wmlc", "wmlc");
export const Application_Vendor_WapWmlscriptc = application("vnd.wap.wmlscriptc", "wmlsc");
export const Application_Vendor_Webturbo = application("vnd.webturbo", "wtb");
export const Application_Vendor_WfaP2p = application("vnd.wfa.p2p");
export const Application_Vendor_WfaWsc = application("vnd.wfa.wsc");
export const Application_Vendor_WindowsDevicepairing = application("vnd.windows.devicepairing");
export const Application_Vendor_Wmc = application("vnd.wmc");
export const Application_Vendor_WmfBootstrap = application("vnd.wmf.bootstrap");
export const Application_Vendor_WolframMathematica = application("vnd.wolfram.mathematica");
export const Application_Vendor_WolframMathematicaPackage = application("vnd.wolfram.mathematica.package");
export const Application_Vendor_WolframPlayer = application("vnd.wolfram.player", "nbp");
export const Application_Vendor_Wordperfect = application("vnd.wordperfect", "wpd");
export const Application_Vendor_Wqd = application("vnd.wqd", "wqd");
export const Application_Vendor_Wrq_Hp3000_Labelled = application("vnd.wrq-hp3000-labelled");
export const Application_Vendor_WtStf = application("vnd.wt.stf", "stf");
export const Application_Vendor_WvCspWbxml = application("vnd.wv.csp+wbxml", "wbxml");
export const Application_Vendor_WvCspXml = application("vnd.wv.csp+xml", "xml");
export const Application_Vendor_WvSspXml = application("vnd.wv.ssp+xml", "xml");
export const Application_Vendor_XacmlJson = application("vnd.xacml+json", "json");
export const Application_Vendor_Xara = application("vnd.xara", "xar");
export const Application_Vendor_Xfdl = application("vnd.xfdl", "xfdl");
export const Application_Vendor_XfdlWebform = application("vnd.xfdl.webform");
export const Application_Vendor_XmiXml = application("vnd.xmi+xml", "xml");
export const Application_Vendor_XmpieCpkg = application("vnd.xmpie.cpkg");
export const Application_Vendor_XmpieDpkg = application("vnd.xmpie.dpkg");
export const Application_Vendor_XmpiePlan = application("vnd.xmpie.plan");
export const Application_Vendor_XmpiePpkg = application("vnd.xmpie.ppkg");
export const Application_Vendor_XmpieXlim = application("vnd.xmpie.xlim");
export const Application_Vendor_YamahaHv_Dic = application("vnd.yamaha.hv-dic", "hvd");
export const Application_Vendor_YamahaHv_Script = application("vnd.yamaha.hv-script", "hvs");
export const Application_Vendor_YamahaHv_Voice = application("vnd.yamaha.hv-voice", "hvp");
export const Application_Vendor_YamahaOpenscoreformat = application("vnd.yamaha.openscoreformat", "osf");
export const Application_Vendor_YamahaOpenscoreformatOsfpvgXml = application("vnd.yamaha.openscoreformat.osfpvg+xml", "osfpvg");
export const Application_Vendor_YamahaRemote_Setup = application("vnd.yamaha.remote-setup");
export const Application_Vendor_YamahaSmaf_Audio = application("vnd.yamaha.smaf-audio", "saf");
export const Application_Vendor_YamahaSmaf_Phrase = application("vnd.yamaha.smaf-phrase", "spf");
export const Application_Vendor_YamahaThrough_Ngn = application("vnd.yamaha.through-ngn");
export const Application_Vendor_YamahaTunnel_Udpencap = application("vnd.yamaha.tunnel-udpencap");
export const Application_Vendor_Yaoweme = application("vnd.yaoweme");
export const Application_Vendor_Yellowriver_Custom_Menu = application("vnd.yellowriver-custom-menu", "cmp");
export const Application_Vendor_YoutubeYt = deprecate(application("vnd.youtube.yt"), "in favor of video/vnd.youtube.yt");
export const Application_Vendor_Zul = application("vnd.zul", "zir", "zirz");
export const Application_Vendor_ZzazzDeckXml = application("vnd.zzazz.deck+xml", "zaz");
export const Application_VividenceScriptfile = application("vividence.scriptfile");
export const Application_VoicexmlXml = application("voicexml+xml", "vxml");
export const Application_Voucher_CmsJson = application("voucher-cms+json", "json");
export const Application_Vq_Rtcpxr = application("vq-rtcpxr");
export const Application_Wasm = application("wasm", "wasm");
export const Application_WatcherinfoXml = application("watcherinfo+xml", "xml");
export const Application_Webpush_OptionsJson = application("webpush-options+json", "json");
export const Application_Whoispp_Query = application("whoispp-query");
export const Application_Whoispp_Response = application("whoispp-response");
export const Application_Widget = application("widget", "wgt");
export const Application_Winhlp = application("winhlp", "hlp");
export const Application_Wita = application("wita");
export const Application_Wordperfect51 = application("wordperfect5.1");
export const Application_WsdlXml = application("wsdl+xml", "wsdl");
export const Application_WspolicyXml = application("wspolicy+xml", "wspolicy");
export const Application_X_7z_Compressed = application("x-7z-compressed", "7z");
export const Application_X_Abiword = application("x-abiword", "abw");
export const Application_X_Ace_Compressed = application("x-ace-compressed", "ace");
export const Application_X_Amf = application("x-amf");
export const Application_X_Apple_Diskimage = application("x-apple-diskimage", "dmg");
export const Application_X_Arj = application("x-arj", "arj");
export const Application_X_Authorware_Bin = application("x-authorware-bin", "aab", "x32", "u32", "vox");
export const Application_X_Authorware_Map = application("x-authorware-map", "aam");
export const Application_X_Authorware_Seg = application("x-authorware-seg", "aas");
export const Application_X_Bcpio = application("x-bcpio", "bcpio");
export const Application_X_Bdoc = application("x-bdoc", "bdoc");
export const Application_X_Bittorrent = application("x-bittorrent", "torrent");
export const Application_X_Blorb = application("x-blorb", "blb", "blorb");
export const Application_X_Bzip = application("x-bzip", "bz");
export const Application_X_Bzip2 = application("x-bzip2", "bz2", "boz");
export const Application_X_Cbr = application("x-cbr", "cbr", "cba", "cbt", "cbz", "cb7");
export const Application_X_Cdlink = application("x-cdlink", "vcd");
export const Application_X_Cfs_Compressed = application("x-cfs-compressed", "cfs");
export const Application_X_Chat = application("x-chat", "chat");
export const Application_X_Chess_Pgn = application("x-chess-pgn", "pgn");
export const Application_X_Chrome_Extension = application("x-chrome-extension", "crx");
export const Application_X_Deb = application("x-deb");
export const Application_X_Compress = application("x-compress");
export const Application_X_Conference = application("x-conference", "nsc");
export const Application_X_Cpio = application("x-cpio", "cpio");
export const Application_X_Csh = application("x-csh", "csh");
export const Application_X_Debian_Package = application("x-debian-package", "deb", "udeb");
export const Application_X_Dgc_Compressed = application("x-dgc-compressed", "dgc");
export const Application_X_Director = application("x-director", "dir", "dcr", "dxr", "cst", "cct", "cxt", "w3d", "fgd", "swa");
export const Application_X_Doom = application("x-doom", "wad");
export const Application_X_DtbncxXml = application("x-dtbncx+xml", "ncx");
export const Application_X_DtbookXml = application("x-dtbook+xml", "dtb");
export const Application_X_DtbresourceXml = application("x-dtbresource+xml", "res");
export const Application_X_Dvi = application("x-dvi", "dvi");
export const Application_X_Envoy = application("x-envoy", "evy");
export const Application_X_Eva = application("x-eva", "eva");
export const Application_X_Font_Bdf = application("x-font-bdf", "bdf");
export const Application_X_Font_Dos = application("x-font-dos");
export const Application_X_Font_Framemaker = application("x-font-framemaker");
export const Application_X_Font_Ghostscript = application("x-font-ghostscript", "gsf");
export const Application_X_Font_Libgrx = application("x-font-libgrx");
export const Application_X_Font_Linux_Psf = application("x-font-linux-psf", "psf");
export const Application_X_Font_Pcf = application("x-font-pcf", "pcf");
export const Application_X_Font_Snf = application("x-font-snf", "snf");
export const Application_X_Font_Speedo = application("x-font-speedo");
export const Application_X_Font_Sunos_News = application("x-font-sunos-news");
export const Application_X_Font_Type1 = application("x-font-type1", "pfa", "pfb", "pfm", "afm");
export const Application_X_Font_Vfont = application("x-font-vfont");
export const Application_X_Freearc = application("x-freearc", "arc");
export const Application_X_Futuresplash = application("x-futuresplash", "spl");
export const Application_X_Gca_Compressed = application("x-gca-compressed", "gca");
export const Application_X_Glulx = application("x-glulx", "ulx");
export const Application_X_Gnumeric = application("x-gnumeric", "gnumeric");
export const Application_X_Gramps_Xml = application("x-gramps-xml", "gramps");
export const Application_X_Gtar = application("x-gtar", "gtar");
export const Application_X_Gzip = application("x-gzip");
export const Application_X_Hdf = application("x-hdf", "hdf");
export const Application_X_Httpd_Php = application("x-httpd-php", "php");
export const Application_X_Install_Instructions = application("x-install-instructions", "install");
export const Application_X_Iso9660_Image = application("x-iso9660-image", "iso");
export const Application_X_Iwork_Keynote_Sffkey = deprecate(application("x-iwork-keynote-sffkey", "key"), "alias for iWorks Keynote file");
export const Application_X_Iwork_Numbers_Sffnumbers = deprecate(application("x-iwork-numbers-sffnumbers", "numbers"), "alias for iWorks Numbers file");
export const Application_X_Iwork_Pages_Sffpages = deprecate(application("x-iwork-pages-sffpages", "pages"), "alias for iWorks Pages file");
export const Application_X_Java_Jnlp_File = application("x-java-jnlp-file", "jnlp");
export const Application_X_Javascript = application("x-javascript");
export const Application_X_Keepass2 = application("x-keepass2", "kdbx");
export const Application_X_Latex = application("x-latex", "latex");
export const Application_X_Lua_Bytecode = application("x-lua-bytecode", "luac");
export const Application_X_Lzh_Compressed = application("x-lzh-compressed", "lzh", "lha");
export const Application_X_Mie = application("x-mie", "mie");
export const Application_X_Mobipocket_Ebook = application("x-mobipocket-ebook", "prc", "mobi");
export const Application_X_Mpegurl = application("x-mpegurl");
export const Application_X_Ms_Application = application("x-ms-application", "application");
export const Application_X_Ms_Shortcut = application("x-ms-shortcut", "lnk");
export const Application_X_Ms_Wmd = application("x-ms-wmd", "wmd");
export const Application_X_Ms_Wmz = application("x-ms-wmz", "wmz");
export const Application_X_Ms_Xbap = application("x-ms-xbap", "xbap");
export const Application_X_Msdos_Program = application("x-msdos-program", "exe");
export const Application_X_Msaccess = application("x-msaccess", "mdb");
export const Application_X_Msbinder = application("x-msbinder", "obd");
export const Application_X_Mscardfile = application("x-mscardfile", "crd");
export const Application_X_Msclip = application("x-msclip", "clp");
export const Application_X_Msdownload = application("x-msdownload", "exe", "dll", "com", "bat", "msi");
export const Application_X_Msmediaview = application("x-msmediaview", "mvb", "m13", "m14");
export const Application_X_Msmetafile = application("x-msmetafile", "wmf", "wmz", "emf", "emz");
export const Application_X_Msmoney = application("x-msmoney", "mny");
export const Application_X_Mspublisher = application("x-mspublisher", "pub");
export const Application_X_Msschedule = application("x-msschedule", "scd");
export const Application_X_Msterminal = application("x-msterminal", "trm");
export const Application_X_Mswrite = application("x-mswrite", "wri");
export const Application_X_Netcdf = application("x-netcdf", "nc", "cdf");
export const Application_X_Ns_Proxy_Autoconfig = application("x-ns-proxy-autoconfig", "pac");
export const Application_X_Nzb = application("x-nzb", "nzb");
export const Application_X_Pkcs12 = application("x-pkcs12", "p12", "pfx");
export const Application_X_Pkcs7_Certificates = application("x-pkcs7-certificates", "p7b", "spc");
export const Application_X_Pkcs7_Certreqresp = application("x-pkcs7-certreqresp", "p7r");
export const Application_X_Rar_Compressed = application("x-rar-compressed", "rar");
export const Application_X_Research_Info_Systems = application("x-research-info-systems", "ris");
export const Application_X_Sh = application("x-sh", "sh");
export const Application_X_Shar = application("x-shar", "shar");
export const Application_X_Shockwave_Flash = application("x-shockwave-flash", "swf");
export const Application_X_Silverlight_App = application("x-silverlight-app", "xap");
export const Application_X_Sql = application("x-sql", "sql");
export const Application_X_Stuffit = application("x-stuffit", "sit");
export const Application_X_Stuffitx = application("x-stuffitx", "sitx");
export const Application_X_Subrip = application("x-subrip", "srt");
export const Application_X_Sv4cpio = application("x-sv4cpio", "sv4cpio");
export const Application_X_Sv4crc = application("x-sv4crc", "sv4crc");
export const Application_X_T3vm_Image = application("x-t3vm-image", "t3");
export const Application_X_Tads = application("x-tads", "gam");
export const Application_X_Tar = application("x-tar", "tar");
export const Application_X_Tcl = application("x-tcl", "tcl");
export const Application_X_Tex = application("x-tex", "tex");
export const Application_X_Tex_Tfm = application("x-tex-tfm", "tfm");
export const Application_X_Texinfo = application("x-texinfo", "texinfo", "texi");
export const Application_X_Tgif = application("x-tgif", "obj");
export const Application_X_Ustar = application("x-ustar", "ustar");
export const Application_X_Virtualbox_Hdd = application("x-virtualbox-hdd", "hdd");
export const Application_X_Virtualbox_Ova = application("x-virtualbox-ova", "ova");
export const Application_X_Virtualbox_Ovf = application("x-virtualbox-ovf", "ovf");
export const Application_X_Virtualbox_Vbox = application("x-virtualbox-vbox", "vbox");
export const Application_X_Virtualbox_Vbox_Extpack = application("x-virtualbox-vbox-extpack", "vbox-extpack");
export const Application_X_Virtualbox_Vdi = application("x-virtualbox-vdi", "vdi");
export const Application_X_Virtualbox_Vhd = application("x-virtualbox-vhd", "vhd");
export const Application_X_Virtualbox_Vmdk = application("x-virtualbox-vmdk", "vmdk");
export const Application_X_Wais_Source = application("x-wais-source", "src");
export const Application_X_Web_App_ManifestJson = application("x-web-app-manifest+json", "webapp");
export const Application_X_Www_Form_Urlencoded = application("x-www-form-urlencoded");
export const Application_X_X509_Ca_Cert = application("x-x509-ca-cert", "der", "crt");
export const Application_X_Xfig = application("x-xfig", "fig");
export const Application_X_XliffXml = application("x-xliff+xml", "xlf");
export const Application_X_Xpinstall = application("x-xpinstall", "xpi");
export const Application_X_Xz = application("x-xz", "xz");
export const Application_X_Zmachine = application("x-zmachine", "z1", "z2", "z3", "z4", "z5", "z6", "z7", "z8");
export const Application_X400_Bp = application("x400-bp");
export const Application_XacmlXml = application("xacml+xml", "xml");
export const Application_XamlXml = application("xaml+xml", "xaml");
export const Application_Xcap_AttXml = application("xcap-att+xml", "xml");
export const Application_Xcap_CapsXml = application("xcap-caps+xml", "xml");
export const Application_Xcap_DiffXml = application("xcap-diff+xml", "xdf");
export const Application_Xcap_ElXml = application("xcap-el+xml", "xml");
export const Application_Xcap_ErrorXml = application("xcap-error+xml", "xml");
export const Application_Xcap_NsXml = application("xcap-ns+xml", "xml");
export const Application_Xcon_Conference_Info_DiffXml = application("xcon-conference-info-diff+xml", "xml");
export const Application_Xcon_Conference_InfoXml = application("xcon-conference-info+xml", "xml");
export const Application_XencXml = application("xenc+xml", "xenc");
export const Application_Xhtml_VoiceXml = application("xhtml-voice+xml", "xml");
export const Application_XhtmlXml = application("xhtml+xml", "xhtml", "xht");
export const Application_XliffXml = application("xliff+xml", "xml");
export const Application_Xml = application("xml", "xml", "xsl");
export const Application_Xml_Dtd = application("xml-dtd", "dtd");
export const Application_Xml_External_Parsed_Entity = application("xml-external-parsed-entity");
export const Application_Xml_PatchXml = application("xml-patch+xml", "xml");
export const Application_XmppXml = application("xmpp+xml", "xml");
export const Application_XopXml = application("xop+xml", "xop");
export const Application_XprocXml = application("xproc+xml", "xpl");
export const Application_XsltXml = application("xslt+xml", "xslt");
export const Application_XspfXml = application("xspf+xml", "xspf");
export const Application_XvXml = application("xv+xml", "mxml", "xhvml", "xvml", "xvm");
export const Application_Yang = application("yang", "yang");
export const Application_Yang_DataJson = application("yang-data+json", "json");
export const Application_Yang_DataXml = application("yang-data+xml", "xml");
export const Application_Yang_PatchJson = application("yang-patch+json", "json");
export const Application_Yang_PatchXml = application("yang-patch+xml", "xml");
export const Application_YinXml = application("yin+xml", "yin");
export const Application_Zip = application("zip", "zip");
export const Application_Zlib = application("zlib");
export const Application_Zstd = application("zstd");
export const allApplication = [
    Application_A2L,
    Application_Activemessage,
    Application_ScenarioJson,
    Application_Alto_CostmapfilterJson,
    Application_Alto_CostmapJson,
    Application_Alto_DirectoryJson,
    Application_Alto_EndpointcostJson,
    Application_Alto_EndpointcostparamsJson,
    Application_Alto_EndpointpropJson,
    Application_Alto_EndpointpropparamsJson,
    Application_Alto_ErrorJson,
    Application_Alto_NetworkmapfilterJson,
    Application_Alto_NetworkmapJson,
    Application_AML,
    Application_Andrew_Inset,
    Application_Applefile,
    Application_Applixware,
    Application_ATF,
    Application_ATFX,
    Application_AtomcatXml,
    Application_AtomdeletedXml,
    Application_Atomicmail,
    Application_AtomsvcXml,
    Application_AtomXml,
    Application_Atsc_DwdXml,
    Application_Atsc_HeldXml,
    Application_Atsc_RdtJson,
    Application_Atsc_RsatXml,
    Application_ATXML,
    Application_Auth_PolicyXml,
    Application_Bacnet_XddZip,
    Application_Batch_SMTP,
    Application_Bdoc,
    Application_BeepXml,
    Application_CalendarJson,
    Application_CalendarXml,
    Application_Call_Completion,
    Application_CALS_1840,
    Application_Cbor,
    Application_Cbor_Seq,
    Application_Cccex,
    Application_CcmpXml,
    Application_CcxmlXml,
    Application_CDFXXML,
    Application_Cdmi_Capability,
    Application_Cdmi_Container,
    Application_Cdmi_Domain,
    Application_Cdmi_Object,
    Application_Cdmi_Queue,
    Application_Cdni,
    Application_CEA,
    Application_Cea_2018Xml,
    Application_CellmlXml,
    Application_Cfw,
    Application_Clue_infoXml,
    Application_ClueXml,
    Application_Cms,
    Application_CnrpXml,
    Application_Coap_GroupJson,
    Application_Coap_Payload,
    Application_Commonground,
    Application_Conference_InfoXml,
    Application_Cose,
    Application_Cose_Key,
    Application_Cose_Key_Set,
    Application_CplXml,
    Application_Csrattrs,
    Application_CSTAdataXml,
    Application_CstaXml,
    Application_CsvmJson,
    Application_Cu_Seeme,
    Application_Cwt,
    Application_Cybercash,
    Application_Dart,
    Application_Dashdelta,
    Application_DashXml,
    Application_DavmountXml,
    Application_Dca_Rft,
    Application_DCD,
    Application_Dec_Dx,
    Application_Dialog_InfoXml,
    Application_Dicom,
    Application_DicomJson,
    Application_DicomXml,
    Application_DII,
    Application_DIT,
    Application_Dns,
    Application_Dns_Message,
    Application_DnsJson,
    Application_DocbookXml,
    Application_DotsCbor,
    Application_DskppXml,
    Application_DsscDer,
    Application_DsscXml,
    Application_Dvcs,
    Application_Ecmascript,
    Application_EDI_Consent,
    Application_EDI_X12,
    Application_EDIFACT,
    Application_Efi,
    Application_EmergencyCallDataCommentXml,
    Application_EmergencyCallDataControlXml,
    Application_EmergencyCallDataDeviceInfoXml,
    Application_EmergencyCallDataECallMSD,
    Application_EmergencyCallDataProviderInfoXml,
    Application_EmergencyCallDataServiceInfoXml,
    Application_EmergencyCallDataSubscriberInfoXml,
    Application_EmergencyCallDataVEDSXml,
    Application_EmmaXml,
    Application_EmotionmlXml,
    Application_Encaprtp,
    Application_EppXml,
    Application_EpubZip,
    Application_Eshop,
    Application_Example,
    Application_Exi,
    Application_Expect_Ct_ReportJson,
    Application_Fastinfoset,
    Application_Fastsoap,
    Application_FdtXml,
    Application_FhirJson,
    Application_FhirXml,
    Application_Fido_TrustedAppsJson,
    Application_Fits,
    Application_Flexfec,
    Application_Font_Tdpfr,
    Application_Framework_AttributesXml,
    Application_GeoJson,
    Application_GeoJson_Seq,
    Application_GeopackageSqlite3,
    Application_GeoxacmlXml,
    Application_Gltf_Buffer,
    Application_GmlXml,
    Application_GpxXml,
    Application_Gxf,
    Application_Gzip,
    Application_H224,
    Application_HeldXml,
    Application_Hjson,
    Application_Http,
    Application_Hyperstudio,
    Application_Ibe_Key_RequestXml,
    Application_Ibe_Pkg_ReplyXml,
    Application_Ibe_Pp_Data,
    Application_Iges,
    Application_Im_IscomposingXml,
    Application_Index,
    Application_IndexCmd,
    Application_IndexObj,
    Application_IndexResponse,
    Application_IndexVnd,
    Application_InkmlXml,
    Application_IOTP,
    Application_Ipfix,
    Application_Ipp,
    Application_ISUP,
    Application_ItsXml,
    Application_Java_Archive,
    Application_Java_Serialized_Object,
    Application_Java_Vm,
    Application_Javascript,
    Application_Jf2feedJson,
    Application_Jose,
    Application_JoseJson,
    Application_JrdJson,
    Application_Json,
    Application_Json5,
    Application_JsonUTF8,
    Application_Json_PatchJson,
    Application_Json_Seq,
    Application_JsonmlJson,
    Application_Jwk_SetJson,
    Application_JwkJson,
    Application_Jwt,
    Application_Kpml_RequestXml,
    Application_Kpml_ResponseXml,
    Application_LdJson,
    Application_LgrXml,
    Application_Link_Format,
    Application_Load_ControlXml,
    Application_LostsyncXml,
    Application_LostXml,
    Application_LXF,
    Application_Mac_Binhex40,
    Application_Mac_Compactpro,
    Application_Macwriteii,
    Application_MadsXml,
    Application_ManifestJson,
    Application_Marc,
    Application_MarcxmlXml,
    Application_Mathematica,
    Application_Mathml_ContentXml,
    Application_Mathml_PresentationXml,
    Application_MathmlXml,
    Application_Mbms_Associated_Procedure_DescriptionXml,
    Application_Mbms_DeregisterXml,
    Application_Mbms_EnvelopeXml,
    Application_Mbms_Msk_ResponseXml,
    Application_Mbms_MskXml,
    Application_Mbms_Protection_DescriptionXml,
    Application_Mbms_Reception_ReportXml,
    Application_Mbms_Register_ResponseXml,
    Application_Mbms_RegisterXml,
    Application_Mbms_ScheduleXml,
    Application_Mbms_User_Service_DescriptionXml,
    Application_Mbox,
    Application_Media_controlXml,
    Application_Media_Policy_DatasetXml,
    Application_MediaservercontrolXml,
    Application_Merge_PatchJson,
    Application_Metalink4Xml,
    Application_MetalinkXml,
    Application_MetsXml,
    Application_MF4,
    Application_Mikey,
    Application_Mipc,
    Application_Mmt_AeiXml,
    Application_Mmt_UsdXml,
    Application_ModsXml,
    Application_Moss_Keys,
    Application_Moss_Signature,
    Application_Mosskey_Data,
    Application_Mosskey_Request,
    Application_Mp21,
    Application_Mp4,
    Application_Mpeg4_Generic,
    Application_Mpeg4_Iod,
    Application_Mpeg4_Iod_Xmt,
    Application_Mrb_ConsumerXml,
    Application_Mrb_PublishXml,
    Application_Msc_IvrXml,
    Application_Msc_MixerXml,
    Application_Msword,
    Application_MudJson,
    Application_Multipart_Core,
    Application_Mxf,
    Application_N_Quads,
    Application_N_Triples,
    Application_Nasdata,
    Application_News_Checkgroups,
    Application_News_Groupinfo,
    Application_News_Transmission,
    Application_NlsmlXml,
    Application_Node,
    Application_Nss,
    Application_Ocsp_Request,
    Application_Ocsp_Response,
    Application_Octet_Stream,
    Application_ODA,
    Application_OdmXml,
    Application_ODX,
    Application_Oebps_PackageXml,
    Application_Ogg,
    Application_OmdocXml,
    Application_Onenote,
    Application_Oscore,
    Application_Oxps,
    Application_P2p_OverlayXml,
    Application_Parityfec,
    Application_Passport,
    Application_Patch_Ops_ErrorXml,
    Application_Pdf,
    Application_PDX,
    Application_Pem_Certificate_Chain,
    Application_Pgp_Encrypted,
    Application_Pgp_Keys,
    Application_Pgp_Signature,
    Application_Pics_Rules,
    Application_Pidf_DiffXml,
    Application_PidfXml,
    Application_Pkcs10,
    Application_Pkcs12,
    Application_Pkcs7_Mime,
    Application_Pkcs7_Signature,
    Application_Pkcs8,
    Application_Pkcs8_Encrypted,
    Application_Pkix_Attr_Cert,
    Application_Pkix_Cert,
    Application_Pkix_Crl,
    Application_Pkix_Pkipath,
    Application_Pkixcmp,
    Application_PlsXml,
    Application_Poc_SettingsXml,
    Application_Postscript,
    Application_Ppsp_TrackerJson,
    Application_ProblemJson,
    Application_ProblemXml,
    Application_ProvenanceXml,
    Application_PrsAlvestrandTitrax_Sheet,
    Application_PrsCww,
    Application_PrsHpubZip,
    Application_PrsNprend,
    Application_PrsPlucker,
    Application_PrsRdf_Xml_Crypt,
    Application_PrsXsfXml,
    Application_PskcXml,
    Application_QSIG,
    Application_RamlYaml,
    Application_Raptorfec,
    Application_RdapJson,
    Application_RdfXml,
    Application_ReginfoXml,
    Application_Relax_Ng_Compact_Syntax,
    Application_Remote_Printing,
    Application_ReputonJson,
    Application_Resource_Lists_DiffXml,
    Application_Resource_ListsXml,
    Application_RfcXml,
    Application_Riscos,
    Application_RlmiXml,
    Application_Rls_ServicesXml,
    Application_Route_ApdXml,
    Application_Route_S_TsidXml,
    Application_Route_UsdXml,
    Application_Rpki_Ghostbusters,
    Application_Rpki_Manifest,
    Application_Rpki_Publication,
    Application_Rpki_Roa,
    Application_Rpki_Updown,
    Application_RsdXml,
    Application_RssXml,
    Application_Rtf,
    Application_Rtploopback,
    Application_Rtx,
    Application_SamlassertionXml,
    Application_SamlmetadataXml,
    Application_SbmlXml,
    Application_ScaipXml,
    Application_ScimJson,
    Application_Scvp_Cv_Request,
    Application_Scvp_Cv_Response,
    Application_Scvp_Vp_Request,
    Application_Scvp_Vp_Response,
    Application_Sdp,
    Application_SeceventJwt,
    Application_Senml_Exi,
    Application_SenmlCbor,
    Application_SenmlJson,
    Application_SenmlXml,
    Application_Sensml_Exi,
    Application_SensmlCbor,
    Application_SensmlJson,
    Application_SensmlXml,
    Application_Sep_Exi,
    Application_SepXml,
    Application_Session_Info,
    Application_Set_Payment,
    Application_Set_Payment_Initiation,
    Application_Set_Registration,
    Application_Set_Registration_Initiation,
    Application_SGML,
    Application_Sgml_Open_Catalog,
    Application_ShfXml,
    Application_Sieve,
    Application_Simple_FilterXml,
    Application_Simple_Message_Summary,
    Application_SimpleSymbolContainer,
    Application_Sipc,
    Application_Slate,
    Application_SmilXml,
    Application_Smpte336m,
    Application_SoapFastinfoset,
    Application_SoapXml,
    Application_Sparql_Query,
    Application_Sparql_ResultsXml,
    Application_Spirits_EventXml,
    Application_Sql,
    Application_Srgs,
    Application_SrgsXml,
    Application_SruXml,
    Application_SsdlXml,
    Application_SsmlXml,
    Application_StixJson,
    Application_SwidXml,
    Application_Tamp_Apex_Update,
    Application_Tamp_Apex_Update_Confirm,
    Application_Tamp_Community_Update,
    Application_Tamp_Community_Update_Confirm,
    Application_Tamp_Error,
    Application_Tamp_Sequence_Adjust,
    Application_Tamp_Sequence_Adjust_Confirm,
    Application_Tamp_Status_Query,
    Application_Tamp_Status_Response,
    Application_Tamp_Update,
    Application_Tamp_Update_Confirm,
    Application_Tar,
    Application_TaxiiJson,
    Application_TeiXml,
    Application_TETRA_ISI,
    Application_ThraudXml,
    Application_Timestamp_Query,
    Application_Timestamp_Reply,
    Application_Timestamped_Data,
    Application_TlsrptGzip,
    Application_TlsrptJson,
    Application_Tnauthlist,
    Application_Toml,
    Application_Trickle_Ice_Sdpfrag,
    Application_Trig,
    Application_TtmlXml,
    Application_Tve_Trigger,
    Application_Tzif,
    Application_Tzif_Leap,
    Application_Ubjson,
    Application_Ulpfec,
    Application_Urc_GrpsheetXml,
    Application_Urc_RessheetXml,
    Application_Urc_TargetdescXml,
    Application_Urc_UisocketdescXml,
    Application_VcardJson,
    Application_VcardXml,
    Application_Vemmi,
    Application_Vendor_1000mindsDecision_ModelXml,
    Application_Vendor_1d_Interleaved_Parityfec,
    Application_Vendor_3gpdash_Qoe_ReportXml,
    Application_Vendor_3gpp_ImsXml,
    Application_Vendor_3gpp_Prose_Pc3chXml,
    Application_Vendor_3gpp_ProseXml,
    Application_Vendor_3gpp_V2x_Local_Service_Information,
    Application_Vendor_3gpp2BcmcsinfoXml,
    Application_Vendor_3gpp2Sms,
    Application_Vendor_3gpp2Tcap,
    Application_Vendor_3gppAccess_Transfer_EventsXml,
    Application_Vendor_3gppBsfXml,
    Application_Vendor_3gppGMOPXml,
    Application_Vendor_3gppMc_Signalling_Ear,
    Application_Vendor_3gppMcdata_Affiliation_CommandXml,
    Application_Vendor_3gppMcdata_InfoXml,
    Application_Vendor_3gppMcdata_Payload,
    Application_Vendor_3gppMcdata_Service_ConfigXml,
    Application_Vendor_3gppMcdata_Signalling,
    Application_Vendor_3gppMcdata_Ue_ConfigXml,
    Application_Vendor_3gppMcdata_User_ProfileXml,
    Application_Vendor_3gppMcptt_Affiliation_CommandXml,
    Application_Vendor_3gppMcptt_Floor_RequestXml,
    Application_Vendor_3gppMcptt_InfoXml,
    Application_Vendor_3gppMcptt_Location_InfoXml,
    Application_Vendor_3gppMcptt_Mbms_Usage_InfoXml,
    Application_Vendor_3gppMcptt_Service_ConfigXml,
    Application_Vendor_3gppMcptt_SignedXml,
    Application_Vendor_3gppMcptt_Ue_ConfigXml,
    Application_Vendor_3gppMcptt_Ue_Init_ConfigXml,
    Application_Vendor_3gppMcptt_User_ProfileXml,
    Application_Vendor_3gppMcvideo_Affiliation_CommandXml,
    Application_Vendor_3gppMcvideo_InfoXml,
    Application_Vendor_3gppMcvideo_Location_InfoXml,
    Application_Vendor_3gppMcvideo_Mbms_Usage_InfoXml,
    Application_Vendor_3gppMcvideo_Service_ConfigXml,
    Application_Vendor_3gppMcvideo_Transmission_RequestXml,
    Application_Vendor_3gppMcvideo_Ue_ConfigXml,
    Application_Vendor_3gppMcvideo_User_ProfileXml,
    Application_Vendor_3gppMid_CallXml,
    Application_Vendor_3gppPic_Bw_Large,
    Application_Vendor_3gppPic_Bw_Small,
    Application_Vendor_3gppPic_Bw_Var,
    Application_Vendor_3gppSms,
    Application_Vendor_3gppSmsXml,
    Application_Vendor_3gppSrvcc_ExtXml,
    Application_Vendor_3gppSRVCC_InfoXml,
    Application_Vendor_3gppState_And_Event_InfoXml,
    Application_Vendor_3gppUssdXml,
    Application_Vendor_3lightssoftwareImagescal,
    Application_Vendor_3MPost_It_Notes,
    Application_Vendor_AccpacSimplyAso,
    Application_Vendor_AccpacSimplyImp,
    Application_Vendor_Acucobol,
    Application_Vendor_Acucorp,
    Application_Vendor_AdobeAir_Application_Installer_PackageZip,
    Application_Vendor_AdobeFlashMovie,
    Application_Vendor_AdobeFormscentralFcdt,
    Application_Vendor_AdobeFxp,
    Application_Vendor_AdobePartial_Upload,
    Application_Vendor_AdobeXdpXml,
    Application_Vendor_AdobeXfdf,
    Application_Vendor_AetherImp,
    Application_Vendor_AfpcAfplinedata,
    Application_Vendor_AfpcAfplinedata_Pagedef,
    Application_Vendor_AfpcFoca_Charset,
    Application_Vendor_AfpcFoca_Codedfont,
    Application_Vendor_AfpcFoca_Codepage,
    Application_Vendor_AfpcModca,
    Application_Vendor_AfpcModca_Formdef,
    Application_Vendor_AfpcModca_Mediummap,
    Application_Vendor_AfpcModca_Objectcontainer,
    Application_Vendor_AfpcModca_Overlay,
    Application_Vendor_AfpcModca_Pagesegment,
    Application_Vendor_Ah_Barcode,
    Application_Vendor_AheadSpace,
    Application_Vendor_AirzipFilesecureAzf,
    Application_Vendor_AirzipFilesecureAzs,
    Application_Vendor_AmadeusJson,
    Application_Vendor_AmazonEbook,
    Application_Vendor_AmazonMobi8_Ebook,
    Application_Vendor_AmericandynamicsAcc,
    Application_Vendor_AmigaAmi,
    Application_Vendor_AmundsenMazeXml,
    Application_Vendor_AndroidOta,
    Application_Vendor_AndroidPackage_Archive,
    Application_Vendor_Anki,
    Application_Vendor_Anser_Web_Certificate_Issue_Initiation,
    Application_Vendor_Anser_Web_Funds_Transfer_Initiation,
    Application_Vendor_AntixGame_Component,
    Application_Vendor_ApacheThriftBinary,
    Application_Vendor_ApacheThriftCompact,
    Application_Vendor_ApacheThriftJson,
    Application_Vendor_ApiJson,
    Application_Vendor_AplextorWarrpJson,
    Application_Vendor_ApothekendeReservationJson,
    Application_Vendor_AppleInstallerXml,
    Application_Vendor_AppleKeynote,
    Application_Vendor_AppleMpegurl,
    Application_Vendor_AppleNumbers,
    Application_Vendor_ApplePages,
    Application_Vendor_ApplePkpass,
    Application_Vendor_AristanetworksSwi,
    Application_Vendor_ArtisanJson,
    Application_Vendor_Artsquare,
    Application_Vendor_Astraea_SoftwareIota,
    Application_Vendor_Audiograph,
    Application_Vendor_Autopackage,
    Application_Vendor_AvalonJson,
    Application_Vendor_AvistarXml,
    Application_Vendor_BalsamiqBmmlXml,
    Application_Vendor_BalsamiqBmpr,
    Application_Vendor_Banana_Accounting,
    Application_Vendor_BbfUspError,
    Application_Vendor_BbfUspMsg,
    Application_Vendor_BbfUspMsgJson,
    Application_Vendor_Bekitzur_StechJson,
    Application_Vendor_BintMed_Content,
    Application_Vendor_BiopaxRdfXml,
    Application_Vendor_Blink_Idb_Value_Wrapper,
    Application_Vendor_BlueiceMultipass,
    Application_Vendor_BluetoothEpOob,
    Application_Vendor_BluetoothLeOob,
    Application_Vendor_Bmi,
    Application_Vendor_Bpf,
    Application_Vendor_Bpf3,
    Application_Vendor_Businessobjects,
    Application_Vendor_ByuUapiJson,
    Application_Vendor_Cab_Jscript,
    Application_Vendor_Canon_Cpdl,
    Application_Vendor_Canon_Lips,
    Application_Vendor_Capasystems_PgJson,
    Application_Vendor_CendioThinlincClientconf,
    Application_Vendor_Century_SystemsTcp_stream,
    Application_Vendor_ChemdrawXml,
    Application_Vendor_Chess_Pgn,
    Application_Vendor_ChipnutsKaraoke_Mmd,
    Application_Vendor_Ciedi,
    Application_Vendor_Cinderella,
    Application_Vendor_CirpackIsdn_Ext,
    Application_Vendor_CitationstylesStyleXml,
    Application_Vendor_Claymore,
    Application_Vendor_CloantoRp9,
    Application_Vendor_ClonkC4group,
    Application_Vendor_CluetrustCartomobile_Config,
    Application_Vendor_CluetrustCartomobile_Config_Pkg,
    Application_Vendor_Coffeescript,
    Application_Vendor_CollabioXodocumentsDocument,
    Application_Vendor_CollabioXodocumentsDocument_Template,
    Application_Vendor_CollabioXodocumentsPresentation,
    Application_Vendor_CollabioXodocumentsPresentation_Template,
    Application_Vendor_CollabioXodocumentsSpreadsheet,
    Application_Vendor_CollabioXodocumentsSpreadsheet_Template,
    Application_Vendor_CollectionDocJson,
    Application_Vendor_CollectionJson,
    Application_Vendor_CollectionNextJson,
    Application_Vendor_Comicbook_Rar,
    Application_Vendor_ComicbookZip,
    Application_Vendor_Commerce_Battelle,
    Application_Vendor_Commonspace,
    Application_Vendor_ContactCmsg,
    Application_Vendor_CoreosIgnitionJson,
    Application_Vendor_Cosmocaller,
    Application_Vendor_CrickClicker,
    Application_Vendor_CrickClickerKeyboard,
    Application_Vendor_CrickClickerPalette,
    Application_Vendor_CrickClickerTemplate,
    Application_Vendor_CrickClickerWordbank,
    Application_Vendor_CriticaltoolsWbsXml,
    Application_Vendor_CryptiiPipeJson,
    Application_Vendor_Crypto_Shade_File,
    Application_Vendor_Ctc_Posml,
    Application_Vendor_CtctWsXml,
    Application_Vendor_Cups_Pdf,
    Application_Vendor_Cups_Postscript,
    Application_Vendor_Cups_Ppd,
    Application_Vendor_Cups_Raster,
    Application_Vendor_Cups_Raw,
    Application_Vendor_Curl,
    Application_Vendor_CurlCar,
    Application_Vendor_CurlPcurl,
    Application_Vendor_CyanDeanRootXml,
    Application_Vendor_Cybank,
    Application_Vendor_D2lCoursepackage1p0Zip,
    Application_Vendor_Dart,
    Application_Vendor_Data_VisionRdz,
    Application_Vendor_DatapackageJson,
    Application_Vendor_DataresourceJson,
    Application_Vendor_DebianBinary_Package,
    Application_Vendor_DeceData,
    Application_Vendor_DeceTtmlXml,
    Application_Vendor_DeceUnspecified,
    Application_Vendor_DeceZip,
    Application_Vendor_DenovoFcselayout_Link,
    Application_Vendor_DesmumeMovie,
    Application_Vendor_Dir_BiPlate_Dl_Nosuffix,
    Application_Vendor_DmDelegationXml,
    Application_Vendor_Dna,
    Application_Vendor_DocumentJson,
    Application_Vendor_DolbyMlp,
    Application_Vendor_DolbyMobile1,
    Application_Vendor_DolbyMobile2,
    Application_Vendor_DoremirScorecloud_Binary_Document,
    Application_Vendor_Dpgraph,
    Application_Vendor_Dreamfactory,
    Application_Vendor_DriveJson,
    Application_Vendor_Ds_Keypoint,
    Application_Vendor_DtgLocal,
    Application_Vendor_DtgLocalFlash,
    Application_Vendor_DtgLocalHtml,
    Application_Vendor_DvbAit,
    Application_Vendor_DvbDvbj,
    Application_Vendor_DvbEsgcontainer,
    Application_Vendor_DvbIpdcdftnotifaccess,
    Application_Vendor_DvbIpdcesgaccess,
    Application_Vendor_DvbIpdcesgaccess2,
    Application_Vendor_DvbIpdcesgpdd,
    Application_Vendor_DvbIpdcroaming,
    Application_Vendor_DvbIptvAlfec_Base,
    Application_Vendor_DvbIptvAlfec_Enhancement,
    Application_Vendor_DvbNotif_Aggregate_RootXml,
    Application_Vendor_DvbNotif_ContainerXml,
    Application_Vendor_DvbNotif_GenericXml,
    Application_Vendor_DvbNotif_Ia_MsglistXml,
    Application_Vendor_DvbNotif_Ia_Registration_RequestXml,
    Application_Vendor_DvbNotif_Ia_Registration_ResponseXml,
    Application_Vendor_DvbNotif_InitXml,
    Application_Vendor_DvbPfr,
    Application_Vendor_DvbService,
    Application_Vendor_Dxr,
    Application_Vendor_Dynageo,
    Application_Vendor_Dzr,
    Application_Vendor_EasykaraokeCdgdownload,
    Application_Vendor_Ecdis_Update,
    Application_Vendor_EcipRlp,
    Application_Vendor_EcowinChart,
    Application_Vendor_EcowinFilerequest,
    Application_Vendor_EcowinFileupdate,
    Application_Vendor_EcowinSeries,
    Application_Vendor_EcowinSeriesrequest,
    Application_Vendor_EcowinSeriesupdate,
    Application_Vendor_EfiImg,
    Application_Vendor_EfiIso,
    Application_Vendor_EmclientAccessrequestXml,
    Application_Vendor_Enliven,
    Application_Vendor_EnphaseEnvoy,
    Application_Vendor_EprintsDataXml,
    Application_Vendor_EpsonEsf,
    Application_Vendor_EpsonMsf,
    Application_Vendor_EpsonQuickanime,
    Application_Vendor_EpsonSalt,
    Application_Vendor_EpsonSsf,
    Application_Vendor_EricssonQuickcall,
    Application_Vendor_Espass_EspassZip,
    Application_Vendor_Eszigno3Xml,
    Application_Vendor_EtsiAocXml,
    Application_Vendor_EtsiAsic_EZip,
    Application_Vendor_EtsiAsic_SZip,
    Application_Vendor_EtsiCugXml,
    Application_Vendor_EtsiIptvcommandXml,
    Application_Vendor_EtsiIptvdiscoveryXml,
    Application_Vendor_EtsiIptvprofileXml,
    Application_Vendor_EtsiIptvsad_BcXml,
    Application_Vendor_EtsiIptvsad_CodXml,
    Application_Vendor_EtsiIptvsad_NpvrXml,
    Application_Vendor_EtsiIptvserviceXml,
    Application_Vendor_EtsiIptvsyncXml,
    Application_Vendor_EtsiIptvueprofileXml,
    Application_Vendor_EtsiMcidXml,
    Application_Vendor_EtsiMheg5,
    Application_Vendor_EtsiOverload_Control_Policy_DatasetXml,
    Application_Vendor_EtsiPstnXml,
    Application_Vendor_EtsiSciXml,
    Application_Vendor_EtsiSimservsXml,
    Application_Vendor_EtsiTimestamp_Token,
    Application_Vendor_EtsiTslDer,
    Application_Vendor_EtsiTslXml,
    Application_Vendor_EudoraData,
    Application_Vendor_EvolvEcigProfile,
    Application_Vendor_EvolvEcigSettings,
    Application_Vendor_EvolvEcigTheme,
    Application_Vendor_Exstream_EmpowerZip,
    Application_Vendor_Exstream_Package,
    Application_Vendor_Ezpix_Album,
    Application_Vendor_Ezpix_Package,
    Application_Vendor_F_SecureMobile,
    Application_Vendor_Fastcopy_Disk_Image,
    Application_Vendor_Fdf,
    Application_Vendor_FdsnMseed,
    Application_Vendor_FdsnSeed,
    Application_Vendor_Ffsns,
    Application_Vendor_FiclabFlbZip,
    Application_Vendor_FilmitZfc,
    Application_Vendor_Fints,
    Application_Vendor_FiremonkeysCloudcell,
    Application_Vendor_FloGraphIt,
    Application_Vendor_FluxtimeClip,
    Application_Vendor_Font_Fontforge_Sfd,
    Application_Vendor_Framemaker,
    Application_Vendor_FrogansFnc,
    Application_Vendor_FrogansLtf,
    Application_Vendor_FscWeblaunch,
    Application_Vendor_FujitsuOasys,
    Application_Vendor_FujitsuOasys2,
    Application_Vendor_FujitsuOasys3,
    Application_Vendor_FujitsuOasysgp,
    Application_Vendor_FujitsuOasysprs,
    Application_Vendor_FujixeroxART_EX,
    Application_Vendor_FujixeroxART4,
    Application_Vendor_FujixeroxDdd,
    Application_Vendor_FujixeroxDocuworks,
    Application_Vendor_FujixeroxDocuworksBinder,
    Application_Vendor_FujixeroxDocuworksContainer,
    Application_Vendor_FujixeroxHBPL,
    Application_Vendor_Fut_Misnet,
    Application_Vendor_FutoinCbor,
    Application_Vendor_FutoinJson,
    Application_Vendor_Fuzzysheet,
    Application_Vendor_GenomatixTuxedo,
    Application_Vendor_GenticsGrdJson,
    Application_Vendor_GeogebraFile,
    Application_Vendor_GeogebraTool,
    Application_Vendor_Geometry_Explorer,
    Application_Vendor_Geonext,
    Application_Vendor_Geoplan,
    Application_Vendor_Geospace,
    Application_Vendor_Gerber,
    Application_Vendor_GlobalplatformCard_Content_Mgt,
    Application_Vendor_GlobalplatformCard_Content_Mgt_Response,
    Application_Vendor_Google_Apps_Document,
    Application_Vendor_Google_Apps_Presentation,
    Application_Vendor_Google_Apps_Spreadsheet,
    Application_Vendor_Google_EarthKmlXml,
    Application_Vendor_Google_EarthKmz,
    Application_Vendor_GovSkE_FormXml,
    Application_Vendor_GovSkE_FormZip,
    Application_Vendor_GovSkXmldatacontainerXml,
    Application_Vendor_Grafeq,
    Application_Vendor_Gridmp,
    Application_Vendor_Groove_Account,
    Application_Vendor_Groove_Help,
    Application_Vendor_Groove_Identity_Message,
    Application_Vendor_Groove_Injector,
    Application_Vendor_Groove_Tool_Message,
    Application_Vendor_Groove_Tool_Template,
    Application_Vendor_Groove_Vcard,
    Application_Vendor_HalJson,
    Application_Vendor_HalXml,
    Application_Vendor_HandHeld_EntertainmentXml,
    Application_Vendor_Hbci,
    Application_Vendor_HcJson,
    Application_Vendor_Hcl_Bireports,
    Application_Vendor_Hdt,
    Application_Vendor_HerokuJson,
    Application_Vendor_HheLesson_Player,
    Application_Vendor_Hp_HPGL,
    Application_Vendor_Hp_Hpid,
    Application_Vendor_Hp_Hps,
    Application_Vendor_Hp_Jlyt,
    Application_Vendor_Hp_PCL,
    Application_Vendor_Hp_PCLXL,
    Application_Vendor_Httphone,
    Application_Vendor_HydrostatixSof_Data,
    Application_Vendor_Hyper_ItemJson,
    Application_Vendor_HyperdriveJson,
    Application_Vendor_HyperJson,
    Application_Vendor_Hzn_3d_Crossword,
    Application_Vendor_IbmElectronic_Media,
    Application_Vendor_IbmMiniPay,
    Application_Vendor_IbmRights_Management,
    Application_Vendor_IbmSecure_Container,
    Application_Vendor_Iccprofile,
    Application_Vendor_Ieee1905,
    Application_Vendor_Igloader,
    Application_Vendor_ImagemeterFolderZip,
    Application_Vendor_ImagemeterImageZip,
    Application_Vendor_Immervision_Ivp,
    Application_Vendor_Immervision_Ivu,
    Application_Vendor_ImsImsccv1p1,
    Application_Vendor_ImsImsccv1p2,
    Application_Vendor_ImsImsccv1p3,
    Application_Vendor_ImsLisV2ResultJson,
    Application_Vendor_ImsLtiV2ToolconsumerprofileJson,
    Application_Vendor_ImsLtiV2ToolproxyIdJson,
    Application_Vendor_ImsLtiV2ToolproxyJson,
    Application_Vendor_ImsLtiV2ToolsettingsJson,
    Application_Vendor_ImsLtiV2ToolsettingsSimpleJson,
    Application_Vendor_InformedcontrolRmsXml,
    Application_Vendor_InfotechProject,
    Application_Vendor_InfotechProjectXml,
    Application_Vendor_InnopathWampNotification,
    Application_Vendor_InsorsIgm,
    Application_Vendor_InterconFormnet,
    Application_Vendor_Intergeo,
    Application_Vendor_IntertrustDigibox,
    Application_Vendor_IntertrustNncp,
    Application_Vendor_IntuQbo,
    Application_Vendor_IntuQfx,
    Application_Vendor_IptcG2CatalogitemXml,
    Application_Vendor_IptcG2ConceptitemXml,
    Application_Vendor_IptcG2KnowledgeitemXml,
    Application_Vendor_IptcG2NewsitemXml,
    Application_Vendor_IptcG2NewsmessageXml,
    Application_Vendor_IptcG2PackageitemXml,
    Application_Vendor_IptcG2PlanningitemXml,
    Application_Vendor_IpunpluggedRcprofile,
    Application_Vendor_IrepositoryPackageXml,
    Application_Vendor_Is_Xpr,
    Application_Vendor_IsacFcs,
    Application_Vendor_Iso11783_10Zip,
    Application_Vendor_Jam,
    Application_Vendor_Japannet_Directory_Service,
    Application_Vendor_Japannet_Jpnstore_Wakeup,
    Application_Vendor_Japannet_Payment_Wakeup,
    Application_Vendor_Japannet_Registration,
    Application_Vendor_Japannet_Registration_Wakeup,
    Application_Vendor_Japannet_Setstore_Wakeup,
    Application_Vendor_Japannet_Verification,
    Application_Vendor_Japannet_Verification_Wakeup,
    Application_Vendor_JcpJavameMidlet_Rms,
    Application_Vendor_Jisp,
    Application_Vendor_JoostJoda_Archive,
    Application_Vendor_JskIsdn_Ngn,
    Application_Vendor_Kahootz,
    Application_Vendor_KdeKarbon,
    Application_Vendor_KdeKchart,
    Application_Vendor_KdeKformula,
    Application_Vendor_KdeKivio,
    Application_Vendor_KdeKontour,
    Application_Vendor_KdeKpresenter,
    Application_Vendor_KdeKspread,
    Application_Vendor_KdeKword,
    Application_Vendor_Kenameaapp,
    Application_Vendor_Kidspiration,
    Application_Vendor_Kinar,
    Application_Vendor_Koan,
    Application_Vendor_Kodak_Descriptor,
    Application_Vendor_Las,
    Application_Vendor_LasLasJson,
    Application_Vendor_LasLasXml,
    Application_Vendor_Laszip,
    Application_Vendor_LeapJson,
    Application_Vendor_Liberty_RequestXml,
    Application_Vendor_LlamagraphicsLife_BalanceDesktop,
    Application_Vendor_LlamagraphicsLife_BalanceExchangeXml,
    Application_Vendor_LogipipeCircuitZip,
    Application_Vendor_Loom,
    Application_Vendor_Lotus_1_2_3,
    Application_Vendor_Lotus_Approach,
    Application_Vendor_Lotus_Freelance,
    Application_Vendor_Lotus_Notes,
    Application_Vendor_Lotus_Organizer,
    Application_Vendor_Lotus_Screencam,
    Application_Vendor_Lotus_Wordpro,
    Application_Vendor_MacportsPortpkg,
    Application_Vendor_Mapbox_Vector_Tile,
    Application_Vendor_MarlinDrmActiontokenXml,
    Application_Vendor_MarlinDrmConftokenXml,
    Application_Vendor_MarlinDrmLicenseXml,
    Application_Vendor_MarlinDrmMdcf,
    Application_Vendor_MasonJson,
    Application_Vendor_MaxmindMaxmind_Db,
    Application_Vendor_Mcd,
    Application_Vendor_Medcalcdata,
    Application_Vendor_MediastationCdkey,
    Application_Vendor_Meridian_Slingshot,
    Application_Vendor_MFER,
    Application_Vendor_Mfmp,
    Application_Vendor_MicrografxFlo,
    Application_Vendor_MicrografxIgx,
    Application_Vendor_MicroJson,
    Application_Vendor_MicrosoftPortable_Executable,
    Application_Vendor_MicrosoftWindowsThumbnail_Cache,
    Application_Vendor_MieleJson,
    Application_Vendor_Mif,
    Application_Vendor_Minisoft_Hp3000_Save,
    Application_Vendor_MitsubishiMisty_GuardTrustweb,
    Application_Vendor_MobiusDAF,
    Application_Vendor_MobiusDIS,
    Application_Vendor_MobiusMBK,
    Application_Vendor_MobiusMQY,
    Application_Vendor_MobiusMSL,
    Application_Vendor_MobiusPLC,
    Application_Vendor_MobiusTXF,
    Application_Vendor_MophunApplication,
    Application_Vendor_MophunCertificate,
    Application_Vendor_MotorolaFlexsuite,
    Application_Vendor_MotorolaFlexsuiteAdsi,
    Application_Vendor_MotorolaFlexsuiteFis,
    Application_Vendor_MotorolaFlexsuiteGotap,
    Application_Vendor_MotorolaFlexsuiteKmr,
    Application_Vendor_MotorolaFlexsuiteTtc,
    Application_Vendor_MotorolaFlexsuiteWem,
    Application_Vendor_MotorolaIprm,
    Application_Vendor_MozillaXulXml,
    Application_Vendor_Ms_3mfdocument,
    Application_Vendor_Ms_Artgalry,
    Application_Vendor_Ms_Asf,
    Application_Vendor_Ms_Cab_Compressed,
    Application_Vendor_Ms_ColorIccprofile,
    Application_Vendor_Ms_Excel,
    Application_Vendor_Ms_ExcelAddinMacroEnabled12,
    Application_Vendor_Ms_ExcelSheetBinaryMacroEnabled12,
    Application_Vendor_Ms_ExcelSheetMacroEnabled12,
    Application_Vendor_Ms_ExcelTemplateMacroEnabled12,
    Application_Vendor_Ms_Fontobject,
    Application_Vendor_Ms_Htmlhelp,
    Application_Vendor_Ms_Ims,
    Application_Vendor_Ms_Lrm,
    Application_Vendor_Ms_OfficeActiveXXml,
    Application_Vendor_Ms_Officetheme,
    Application_Vendor_Ms_Opentype,
    Application_Vendor_Ms_Outlook,
    Application_Vendor_Ms_PackageObfuscated_Opentype,
    Application_Vendor_Ms_PkiSeccat,
    Application_Vendor_Ms_PkiStl,
    Application_Vendor_Ms_PlayreadyInitiatorXml,
    Application_Vendor_Ms_Powerpoint,
    Application_Vendor_Ms_PowerpointAddinMacroEnabled12,
    Application_Vendor_Ms_PowerpointPresentationMacroEnabled12,
    Application_Vendor_Ms_PowerpointSlideMacroEnabled12,
    Application_Vendor_Ms_PowerpointSlideshowMacroEnabled12,
    Application_Vendor_Ms_PowerpointTemplateMacroEnabled12,
    Application_Vendor_Ms_PrintDeviceCapabilitiesXml,
    Application_Vendor_Ms_PrintingPrintticketXml,
    Application_Vendor_Ms_PrintSchemaTicketXml,
    Application_Vendor_Ms_Project,
    Application_Vendor_Ms_Tnef,
    Application_Vendor_Ms_WindowsDevicepairing,
    Application_Vendor_Ms_WindowsNwprintingOob,
    Application_Vendor_Ms_WindowsPrinterpairing,
    Application_Vendor_Ms_WindowsWsdOob,
    Application_Vendor_Ms_WmdrmLic_Chlg_Req,
    Application_Vendor_Ms_WmdrmLic_Resp,
    Application_Vendor_Ms_WmdrmMeter_Chlg_Req,
    Application_Vendor_Ms_WmdrmMeter_Resp,
    Application_Vendor_Ms_WordDocumentMacroEnabled12,
    Application_Vendor_Ms_WordTemplateMacroEnabled12,
    Application_Vendor_Ms_Works,
    Application_Vendor_Ms_Wpl,
    Application_Vendor_Ms_Xpsdocument,
    Application_Vendor_Msa_Disk_Image,
    Application_Vendor_Mseq,
    Application_Vendor_Msign,
    Application_Vendor_MultiadCreator,
    Application_Vendor_MultiadCreatorCif,
    Application_Vendor_Music_Niff,
    Application_Vendor_Musician,
    Application_Vendor_MuveeStyle,
    Application_Vendor_Mynfc,
    Application_Vendor_NcdControl,
    Application_Vendor_NcdReference,
    Application_Vendor_NearstInvJson,
    Application_Vendor_Nervana,
    Application_Vendor_Netfpx,
    Application_Vendor_NeurolanguageNlu,
    Application_Vendor_Nimn,
    Application_Vendor_NintendoNitroRom,
    Application_Vendor_NintendoSnesRom,
    Application_Vendor_Nitf,
    Application_Vendor_Noblenet_Directory,
    Application_Vendor_Noblenet_Sealer,
    Application_Vendor_Noblenet_Web,
    Application_Vendor_NokiaCatalogs,
    Application_Vendor_NokiaConmlWbxml,
    Application_Vendor_NokiaConmlXml,
    Application_Vendor_NokiaIptvConfigXml,
    Application_Vendor_NokiaISDS_Radio_Presets,
    Application_Vendor_NokiaLandmarkcollectionXml,
    Application_Vendor_NokiaLandmarkWbxml,
    Application_Vendor_NokiaLandmarkXml,
    Application_Vendor_NokiaN_GageAcXml,
    Application_Vendor_NokiaN_GageData,
    Application_Vendor_NokiaNcd,
    Application_Vendor_NokiaPcdWbxml,
    Application_Vendor_NokiaPcdXml,
    Application_Vendor_NokiaRadio_Preset,
    Application_Vendor_NokiaRadio_Presets,
    Application_Vendor_NovadigmEDM,
    Application_Vendor_NovadigmEDX,
    Application_Vendor_NovadigmEXT,
    Application_Vendor_Ntt_LocalContent_Share,
    Application_Vendor_Ntt_LocalFile_Transfer,
    Application_Vendor_Ntt_LocalOgw_remote_Access,
    Application_Vendor_Ntt_LocalSip_Ta_remote,
    Application_Vendor_Ntt_LocalSip_Ta_tcp_stream,
    Application_Vendor_OasisOpendocumentChart,
    Application_Vendor_OasisOpendocumentChart_Template,
    Application_Vendor_OasisOpendocumentDatabase,
    Application_Vendor_OasisOpendocumentFormula,
    Application_Vendor_OasisOpendocumentFormula_Template,
    Application_Vendor_OasisOpendocumentGraphics,
    Application_Vendor_OasisOpendocumentGraphics_Template,
    Application_Vendor_OasisOpendocumentImage,
    Application_Vendor_OasisOpendocumentImage_Template,
    Application_Vendor_OasisOpendocumentPresentation,
    Application_Vendor_OasisOpendocumentPresentation_Template,
    Application_Vendor_OasisOpendocumentSpreadsheet,
    Application_Vendor_OasisOpendocumentSpreadsheet_Template,
    Application_Vendor_OasisOpendocumentText,
    Application_Vendor_OasisOpendocumentText_Master,
    Application_Vendor_OasisOpendocumentText_Template,
    Application_Vendor_OasisOpendocumentText_Web,
    Application_Vendor_Obn,
    Application_Vendor_OcfCbor,
    Application_Vendor_OftnL10nJson,
    Application_Vendor_OipfContentaccessdownloadXml,
    Application_Vendor_OipfContentaccessstreamingXml,
    Application_Vendor_OipfCspg_Hexbinary,
    Application_Vendor_OipfDaeSvgXml,
    Application_Vendor_OipfDaeXhtmlXml,
    Application_Vendor_OipfMippvcontrolmessageXml,
    Application_Vendor_OipfPaeGem,
    Application_Vendor_OipfSpdiscoveryXml,
    Application_Vendor_OipfSpdlistXml,
    Application_Vendor_OipfUeprofileXml,
    Application_Vendor_OipfUserprofileXml,
    Application_Vendor_Olpc_Sugar,
    Application_Vendor_Oma_Scws_Config,
    Application_Vendor_Oma_Scws_Http_Request,
    Application_Vendor_Oma_Scws_Http_Response,
    Application_Vendor_OmaBcastAssociated_Procedure_ParameterXml,
    Application_Vendor_OmaBcastDrm_TriggerXml,
    Application_Vendor_OmaBcastImdXml,
    Application_Vendor_OmaBcastLtkm,
    Application_Vendor_OmaBcastNotificationXml,
    Application_Vendor_OmaBcastProvisioningtrigger,
    Application_Vendor_OmaBcastSgboot,
    Application_Vendor_OmaBcastSgddXml,
    Application_Vendor_OmaBcastSgdu,
    Application_Vendor_OmaBcastSimple_Symbol_Container,
    Application_Vendor_OmaBcastSmartcard_TriggerXml,
    Application_Vendor_OmaBcastSprovXml,
    Application_Vendor_OmaBcastStkm,
    Application_Vendor_OmaCab_Address_BookXml,
    Application_Vendor_OmaCab_Feature_HandlerXml,
    Application_Vendor_OmaCab_PccXml,
    Application_Vendor_OmaCab_Subs_InviteXml,
    Application_Vendor_OmaCab_User_PrefsXml,
    Application_Vendor_OmaDcd,
    Application_Vendor_OmaDcdc,
    Application_Vendor_OmaDd2Xml,
    Application_Vendor_OmaDrmRisdXml,
    Application_Vendor_Omads_EmailXml,
    Application_Vendor_Omads_FileXml,
    Application_Vendor_Omads_FolderXml,
    Application_Vendor_OmaGroup_Usage_ListXml,
    Application_Vendor_Omaloc_Supl_Init,
    Application_Vendor_OmaLwm2mJson,
    Application_Vendor_OmaLwm2mTlv,
    Application_Vendor_OmaPalXml,
    Application_Vendor_OmaPocDetailed_Progress_ReportXml,
    Application_Vendor_OmaPocFinal_ReportXml,
    Application_Vendor_OmaPocGroupsXml,
    Application_Vendor_OmaPocInvocation_DescriptorXml,
    Application_Vendor_OmaPocOptimized_Progress_ReportXml,
    Application_Vendor_OmaPush,
    Application_Vendor_OmaScidmMessagesXml,
    Application_Vendor_OmaXcap_DirectoryXml,
    Application_Vendor_Onepager,
    Application_Vendor_Onepagertamp,
    Application_Vendor_Onepagertamx,
    Application_Vendor_Onepagertat,
    Application_Vendor_Onepagertatp,
    Application_Vendor_Onepagertatx,
    Application_Vendor_OpenbloxGame_Binary,
    Application_Vendor_OpenbloxGameXml,
    Application_Vendor_OpeneyeOeb,
    Application_Vendor_OpenofficeorgExtension,
    Application_Vendor_OpenstreetmapDataXml,
    Application_Vendor_Openxmlformats_OfficedocumentCustom_PropertiesXml,
    Application_Vendor_Openxmlformats_OfficedocumentCustomXmlPropertiesXml,
    Application_Vendor_Openxmlformats_OfficedocumentDrawingmlChartshapesXml,
    Application_Vendor_Openxmlformats_OfficedocumentDrawingmlChartXml,
    Application_Vendor_Openxmlformats_OfficedocumentDrawingmlDiagramColorsXml,
    Application_Vendor_Openxmlformats_OfficedocumentDrawingmlDiagramDataXml,
    Application_Vendor_Openxmlformats_OfficedocumentDrawingmlDiagramLayoutXml,
    Application_Vendor_Openxmlformats_OfficedocumentDrawingmlDiagramStyleXml,
    Application_Vendor_Openxmlformats_OfficedocumentDrawingXml,
    Application_Vendor_Openxmlformats_OfficedocumentExtended_PropertiesXml,
    Application_Vendor_Openxmlformats_OfficedocumentPresentationmlCommentAuthorsXml,
    Application_Vendor_Openxmlformats_OfficedocumentPresentationmlCommentsXml,
    Application_Vendor_Openxmlformats_OfficedocumentPresentationmlHandoutMasterXml,
    Application_Vendor_Openxmlformats_OfficedocumentPresentationmlNotesMasterXml,
    Application_Vendor_Openxmlformats_OfficedocumentPresentationmlNotesSlideXml,
    Application_Vendor_Openxmlformats_OfficedocumentPresentationmlPresentation,
    Application_Vendor_Openxmlformats_OfficedocumentPresentationmlPresentationMainXml,
    Application_Vendor_Openxmlformats_OfficedocumentPresentationmlPresPropsXml,
    Application_Vendor_Openxmlformats_OfficedocumentPresentationmlSlide,
    Application_Vendor_Openxmlformats_OfficedocumentPresentationmlSlideLayoutXml,
    Application_Vendor_Openxmlformats_OfficedocumentPresentationmlSlideMasterXml,
    Application_Vendor_Openxmlformats_OfficedocumentPresentationmlSlideshow,
    Application_Vendor_Openxmlformats_OfficedocumentPresentationmlSlideshowMainXml,
    Application_Vendor_Openxmlformats_OfficedocumentPresentationmlSlideUpdateInfoXml,
    Application_Vendor_Openxmlformats_OfficedocumentPresentationmlSlideXml,
    Application_Vendor_Openxmlformats_OfficedocumentPresentationmlTableStylesXml,
    Application_Vendor_Openxmlformats_OfficedocumentPresentationmlTagsXml,
    Application_Vendor_Openxmlformats_OfficedocumentPresentationmlTemplate,
    Application_Vendor_Openxmlformats_OfficedocumentPresentationmlTemplateMainXml,
    Application_Vendor_Openxmlformats_OfficedocumentPresentationmlViewPropsXml,
    Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlCalcChainXml,
    Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlChartsheetXml,
    Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlCommentsXml,
    Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlConnectionsXml,
    Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlDialogsheetXml,
    Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlExternalLinkXml,
    Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlPivotCacheDefinitionXml,
    Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlPivotCacheRecordsXml,
    Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlPivotTableXml,
    Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlQueryTableXml,
    Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlRevisionHeadersXml,
    Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlRevisionLogXml,
    Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlSharedStringsXml,
    Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlSheet,
    Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlSheetMainXml,
    Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlSheetMetadataXml,
    Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlStylesXml,
    Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlTableSingleCellsXml,
    Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlTableXml,
    Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlTemplate,
    Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlTemplateMainXml,
    Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlUserNamesXml,
    Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlVolatileDependenciesXml,
    Application_Vendor_Openxmlformats_OfficedocumentSpreadsheetmlWorksheetXml,
    Application_Vendor_Openxmlformats_OfficedocumentThemeOverrideXml,
    Application_Vendor_Openxmlformats_OfficedocumentThemeXml,
    Application_Vendor_Openxmlformats_OfficedocumentVmlDrawing,
    Application_Vendor_Openxmlformats_OfficedocumentWordprocessingmlCommentsXml,
    Application_Vendor_Openxmlformats_OfficedocumentWordprocessingmlDocument,
    Application_Vendor_Openxmlformats_OfficedocumentWordprocessingmlDocumentGlossaryXml,
    Application_Vendor_Openxmlformats_OfficedocumentWordprocessingmlDocumentMainXml,
    Application_Vendor_Openxmlformats_OfficedocumentWordprocessingmlEndnotesXml,
    Application_Vendor_Openxmlformats_OfficedocumentWordprocessingmlFontTableXml,
    Application_Vendor_Openxmlformats_OfficedocumentWordprocessingmlFooterXml,
    Application_Vendor_Openxmlformats_OfficedocumentWordprocessingmlFootnotesXml,
    Application_Vendor_Openxmlformats_OfficedocumentWordprocessingmlNumberingXml,
    Application_Vendor_Openxmlformats_OfficedocumentWordprocessingmlSettingsXml,
    Application_Vendor_Openxmlformats_OfficedocumentWordprocessingmlStylesXml,
    Application_Vendor_Openxmlformats_OfficedocumentWordprocessingmlTemplate,
    Application_Vendor_Openxmlformats_OfficedocumentWordprocessingmlTemplateMainXml,
    Application_Vendor_Openxmlformats_OfficedocumentWordprocessingmlWebSettingsXml,
    Application_Vendor_Openxmlformats_PackageCore_PropertiesXml,
    Application_Vendor_Openxmlformats_PackageDigital_Signature_XmlsignatureXml,
    Application_Vendor_Openxmlformats_PackageRelationshipsXml,
    Application_Vendor_OracleResourceJson,
    Application_Vendor_OrangeIndata,
    Application_Vendor_OsaNetdeploy,
    Application_Vendor_OsgeoMapguidePackage,
    Application_Vendor_OsgiBundle,
    Application_Vendor_OsgiDp,
    Application_Vendor_OsgiSubsystem,
    Application_Vendor_OtpsCt_KipXml,
    Application_Vendor_OxliCountgraph,
    Application_Vendor_PagerdutyJson,
    Application_Vendor_Palm,
    Application_Vendor_Panoply,
    Application_Vendor_PaosXml,
    Application_Vendor_Patentdive,
    Application_Vendor_Patientecommsdoc,
    Application_Vendor_Pawaafile,
    Application_Vendor_Pcos,
    Application_Vendor_PgFormat,
    Application_Vendor_PgOsasli,
    Application_Vendor_PiaccessApplication_Licence,
    Application_Vendor_Picsel,
    Application_Vendor_PmiWidget,
    Application_Vendor_PocGroup_AdvertisementXml,
    Application_Vendor_Pocketlearn,
    Application_Vendor_Powerbuilder6,
    Application_Vendor_Powerbuilder6_S,
    Application_Vendor_Powerbuilder7,
    Application_Vendor_Powerbuilder7_S,
    Application_Vendor_Powerbuilder75,
    Application_Vendor_Powerbuilder75_S,
    Application_Vendor_Preminet,
    Application_Vendor_PreviewsystemsBox,
    Application_Vendor_ProteusMagazine,
    Application_Vendor_Psfs,
    Application_Vendor_Publishare_Delta_Tree,
    Application_Vendor_PviPtid1,
    Application_Vendor_Pwg_Multiplexed,
    Application_Vendor_Pwg_Xhtml_PrintXml,
    Application_Vendor_QualcommBrew_App_Res,
    Application_Vendor_Quarantainenet,
    Application_Vendor_QuarkQuarkXPress,
    Application_Vendor_Quobject_Quoxdocument,
    Application_Vendor_RadisysMomlXml,
    Application_Vendor_RadisysMsml_Audit_ConfXml,
    Application_Vendor_RadisysMsml_Audit_ConnXml,
    Application_Vendor_RadisysMsml_Audit_DialogXml,
    Application_Vendor_RadisysMsml_Audit_StreamXml,
    Application_Vendor_RadisysMsml_AuditXml,
    Application_Vendor_RadisysMsml_ConfXml,
    Application_Vendor_RadisysMsml_Dialog_BaseXml,
    Application_Vendor_RadisysMsml_Dialog_Fax_DetectXml,
    Application_Vendor_RadisysMsml_Dialog_Fax_SendrecvXml,
    Application_Vendor_RadisysMsml_Dialog_GroupXml,
    Application_Vendor_RadisysMsml_Dialog_SpeechXml,
    Application_Vendor_RadisysMsml_Dialog_TransformXml,
    Application_Vendor_RadisysMsml_DialogXml,
    Application_Vendor_RadisysMsmlXml,
    Application_Vendor_RainstorData,
    Application_Vendor_Rapid,
    Application_Vendor_Rar,
    Application_Vendor_RealvncBed,
    Application_Vendor_RecordareMusicxml,
    Application_Vendor_RecordareMusicxmlXml,
    Application_Vendor_RenLearnRlprint,
    Application_Vendor_RestfulJson,
    Application_Vendor_RigCryptonote,
    Application_Vendor_RimCod,
    Application_Vendor_Rn_Realmedia,
    Application_Vendor_Rn_Realmedia_Vbr,
    Application_Vendor_Route66Link66Xml,
    Application_Vendor_Rs_274x,
    Application_Vendor_RuckusDownload,
    Application_Vendor_S3sms,
    Application_Vendor_SailingtrackerTrack,
    Application_Vendor_Sar,
    Application_Vendor_SbmCid,
    Application_Vendor_SbmMid2,
    Application_Vendor_Scribus,
    Application_Vendor_Sealed3df,
    Application_Vendor_SealedCsf,
    Application_Vendor_SealedDoc,
    Application_Vendor_SealedEml,
    Application_Vendor_SealedmediaSoftsealHtml,
    Application_Vendor_SealedmediaSoftsealPdf,
    Application_Vendor_SealedMht,
    Application_Vendor_SealedNet,
    Application_Vendor_SealedPpt,
    Application_Vendor_SealedTiff,
    Application_Vendor_SealedXls,
    Application_Vendor_Seemail,
    Application_Vendor_Sema,
    Application_Vendor_Semd,
    Application_Vendor_Semf,
    Application_Vendor_Shade_Save_File,
    Application_Vendor_ShanaInformedFormdata,
    Application_Vendor_ShanaInformedFormtemplate,
    Application_Vendor_ShanaInformedInterchange,
    Application_Vendor_ShanaInformedPackage,
    Application_Vendor_ShootproofJson,
    Application_Vendor_ShopkickJson,
    Application_Vendor_SigrokSession,
    Application_Vendor_SimTech_MindMapper,
    Application_Vendor_SirenJson,
    Application_Vendor_Smaf,
    Application_Vendor_SmartNotebook,
    Application_Vendor_SmartTeacher,
    Application_Vendor_Software602FillerForm_Xml_Zip,
    Application_Vendor_Software602FillerFormXml,
    Application_Vendor_SolentSdkmXml,
    Application_Vendor_SpotfireDxp,
    Application_Vendor_SpotfireSfs,
    Application_Vendor_Sqlite3,
    Application_Vendor_Sss_Cod,
    Application_Vendor_Sss_Dtf,
    Application_Vendor_Sss_Ntf,
    Application_Vendor_StardivisionCalc,
    Application_Vendor_StardivisionDraw,
    Application_Vendor_StardivisionImpress,
    Application_Vendor_StardivisionMath,
    Application_Vendor_StardivisionWriter,
    Application_Vendor_StardivisionWriter_Global,
    Application_Vendor_StepmaniaPackage,
    Application_Vendor_StepmaniaStepchart,
    Application_Vendor_Street_Stream,
    Application_Vendor_SunWadlXml,
    Application_Vendor_SunXmlCalc,
    Application_Vendor_SunXmlCalcTemplate,
    Application_Vendor_SunXmlDraw,
    Application_Vendor_SunXmlDrawTemplate,
    Application_Vendor_SunXmlImpress,
    Application_Vendor_SunXmlImpressTemplate,
    Application_Vendor_SunXmlMath,
    Application_Vendor_SunXmlWriter,
    Application_Vendor_SunXmlWriterGlobal,
    Application_Vendor_SunXmlWriterTemplate,
    Application_Vendor_Sus_Calendar,
    Application_Vendor_Svd,
    Application_Vendor_Swiftview_Ics,
    Application_Vendor_SymbianInstall,
    Application_Vendor_SyncmlDmddfWbxml,
    Application_Vendor_SyncmlDmddfXml,
    Application_Vendor_SyncmlDmNotification,
    Application_Vendor_SyncmlDmtndsWbxml,
    Application_Vendor_SyncmlDmtndsXml,
    Application_Vendor_SyncmlDmWbxml,
    Application_Vendor_SyncmlDmXml,
    Application_Vendor_SyncmlDsNotification,
    Application_Vendor_SyncmlXml,
    Application_Vendor_TableschemaJson,
    Application_Vendor_TaoIntent_Module_Archive,
    Application_Vendor_TcpdumpPcap,
    Application_Vendor_Think_CellPpttcJson,
    Application_Vendor_TmdMediaflexApiXml,
    Application_Vendor_Tml,
    Application_Vendor_Tmobile_Livetv,
    Application_Vendor_TridTpt,
    Application_Vendor_TriOnesource,
    Application_Vendor_TriscapeMxs,
    Application_Vendor_Trueapp,
    Application_Vendor_Truedoc,
    Application_Vendor_UbisoftWebplayer,
    Application_Vendor_Ufdl,
    Application_Vendor_UiqTheme,
    Application_Vendor_Umajin,
    Application_Vendor_Unity,
    Application_Vendor_UomlXml,
    Application_Vendor_UplanetAlert,
    Application_Vendor_UplanetAlert_Wbxml,
    Application_Vendor_UplanetBearer_Choice,
    Application_Vendor_UplanetBearer_Choice_Wbxml,
    Application_Vendor_UplanetCacheop,
    Application_Vendor_UplanetCacheop_Wbxml,
    Application_Vendor_UplanetChannel,
    Application_Vendor_UplanetChannel_Wbxml,
    Application_Vendor_UplanetList,
    Application_Vendor_UplanetList_Wbxml,
    Application_Vendor_UplanetListcmd,
    Application_Vendor_UplanetListcmd_Wbxml,
    Application_Vendor_UplanetSignal,
    Application_Vendor_Uri_Map,
    Application_Vendor_ValveSourceMaterial,
    Application_Vendor_Vcx,
    Application_Vendor_Vd_Study,
    Application_Vendor_Vectorworks,
    Application_Vendor_VelJson,
    Application_Vendor_VerimatrixVcas,
    Application_Vendor_VeryantThin,
    Application_Vendor_VesEncrypted,
    Application_Vendor_VidsoftVidconference,
    Application_Vendor_Visio,
    Application_Vendor_Visionary,
    Application_Vendor_VividenceScriptfile,
    Application_Vendor_Vsf,
    Application_Vendor_WapSic,
    Application_Vendor_WapSlc,
    Application_Vendor_WapWbxml,
    Application_Vendor_WapWmlc,
    Application_Vendor_WapWmlscriptc,
    Application_Vendor_Webturbo,
    Application_Vendor_WfaP2p,
    Application_Vendor_WfaWsc,
    Application_Vendor_WindowsDevicepairing,
    Application_Vendor_Wmc,
    Application_Vendor_WmfBootstrap,
    Application_Vendor_WolframMathematica,
    Application_Vendor_WolframMathematicaPackage,
    Application_Vendor_WolframPlayer,
    Application_Vendor_Wordperfect,
    Application_Vendor_Wqd,
    Application_Vendor_Wrq_Hp3000_Labelled,
    Application_Vendor_WtStf,
    Application_Vendor_WvCspWbxml,
    Application_Vendor_WvCspXml,
    Application_Vendor_WvSspXml,
    Application_Vendor_XacmlJson,
    Application_Vendor_Xara,
    Application_Vendor_Xfdl,
    Application_Vendor_XfdlWebform,
    Application_Vendor_XmiXml,
    Application_Vendor_XmpieCpkg,
    Application_Vendor_XmpieDpkg,
    Application_Vendor_XmpiePlan,
    Application_Vendor_XmpiePpkg,
    Application_Vendor_XmpieXlim,
    Application_Vendor_YamahaHv_Dic,
    Application_Vendor_YamahaHv_Script,
    Application_Vendor_YamahaHv_Voice,
    Application_Vendor_YamahaOpenscoreformat,
    Application_Vendor_YamahaOpenscoreformatOsfpvgXml,
    Application_Vendor_YamahaRemote_Setup,
    Application_Vendor_YamahaSmaf_Audio,
    Application_Vendor_YamahaSmaf_Phrase,
    Application_Vendor_YamahaThrough_Ngn,
    Application_Vendor_YamahaTunnel_Udpencap,
    Application_Vendor_Yaoweme,
    Application_Vendor_Yellowriver_Custom_Menu,
    Application_Vendor_Zul,
    Application_Vendor_ZzazzDeckXml,
    Application_VividenceScriptfile,
    Application_VoicexmlXml,
    Application_Voucher_CmsJson,
    Application_Vq_Rtcpxr,
    Application_Wasm,
    Application_WatcherinfoXml,
    Application_Webpush_OptionsJson,
    Application_Whoispp_Query,
    Application_Whoispp_Response,
    Application_Widget,
    Application_Winhlp,
    Application_Wita,
    Application_Wordperfect51,
    Application_WsdlXml,
    Application_WspolicyXml,
    Application_X_7z_Compressed,
    Application_X_Abiword,
    Application_X_Ace_Compressed,
    Application_X_Amf,
    Application_X_Apple_Diskimage,
    Application_X_Arj,
    Application_X_Authorware_Bin,
    Application_X_Authorware_Map,
    Application_X_Authorware_Seg,
    Application_X_Bcpio,
    Application_X_Bdoc,
    Application_X_Bittorrent,
    Application_X_Blorb,
    Application_X_Bzip,
    Application_X_Bzip2,
    Application_X_Cbr,
    Application_X_Cdlink,
    Application_X_Cfs_Compressed,
    Application_X_Chat,
    Application_X_Chess_Pgn,
    Application_X_Chrome_Extension,
    Application_X_Deb,
    Application_X_Compress,
    Application_X_Conference,
    Application_X_Cpio,
    Application_X_Csh,
    Application_X_Debian_Package,
    Application_X_Dgc_Compressed,
    Application_X_Director,
    Application_X_Doom,
    Application_X_DtbncxXml,
    Application_X_DtbookXml,
    Application_X_DtbresourceXml,
    Application_X_Dvi,
    Application_X_Envoy,
    Application_X_Eva,
    Application_X_Font_Bdf,
    Application_X_Font_Dos,
    Application_X_Font_Framemaker,
    Application_X_Font_Ghostscript,
    Application_X_Font_Libgrx,
    Application_X_Font_Linux_Psf,
    Application_X_Font_Pcf,
    Application_X_Font_Snf,
    Application_X_Font_Speedo,
    Application_X_Font_Sunos_News,
    Application_X_Font_Type1,
    Application_X_Font_Vfont,
    Application_X_Freearc,
    Application_X_Futuresplash,
    Application_X_Gca_Compressed,
    Application_X_Glulx,
    Application_X_Gnumeric,
    Application_X_Gramps_Xml,
    Application_X_Gtar,
    Application_X_Gzip,
    Application_X_Hdf,
    Application_X_Httpd_Php,
    Application_X_Install_Instructions,
    Application_X_Iso9660_Image,
    Application_X_Java_Jnlp_File,
    Application_X_Javascript,
    Application_X_Keepass2,
    Application_X_Latex,
    Application_X_Lua_Bytecode,
    Application_X_Lzh_Compressed,
    Application_X_Mie,
    Application_X_Mobipocket_Ebook,
    Application_X_Mpegurl,
    Application_X_Ms_Application,
    Application_X_Ms_Shortcut,
    Application_X_Ms_Wmd,
    Application_X_Ms_Wmz,
    Application_X_Ms_Xbap,
    Application_X_Msdos_Program,
    Application_X_Msaccess,
    Application_X_Msbinder,
    Application_X_Mscardfile,
    Application_X_Msclip,
    Application_X_Msdownload,
    Application_X_Msmediaview,
    Application_X_Msmetafile,
    Application_X_Msmoney,
    Application_X_Mspublisher,
    Application_X_Msschedule,
    Application_X_Msterminal,
    Application_X_Mswrite,
    Application_X_Netcdf,
    Application_X_Ns_Proxy_Autoconfig,
    Application_X_Nzb,
    Application_X_Pkcs12,
    Application_X_Pkcs7_Certificates,
    Application_X_Pkcs7_Certreqresp,
    Application_X_Rar_Compressed,
    Application_X_Research_Info_Systems,
    Application_X_Sh,
    Application_X_Shar,
    Application_X_Shockwave_Flash,
    Application_X_Silverlight_App,
    Application_X_Sql,
    Application_X_Stuffit,
    Application_X_Stuffitx,
    Application_X_Subrip,
    Application_X_Sv4cpio,
    Application_X_Sv4crc,
    Application_X_T3vm_Image,
    Application_X_Tads,
    Application_X_Tar,
    Application_X_Tcl,
    Application_X_Tex,
    Application_X_Tex_Tfm,
    Application_X_Texinfo,
    Application_X_Tgif,
    Application_X_Ustar,
    Application_X_Virtualbox_Hdd,
    Application_X_Virtualbox_Ova,
    Application_X_Virtualbox_Ovf,
    Application_X_Virtualbox_Vbox,
    Application_X_Virtualbox_Vbox_Extpack,
    Application_X_Virtualbox_Vdi,
    Application_X_Virtualbox_Vhd,
    Application_X_Virtualbox_Vmdk,
    Application_X_Wais_Source,
    Application_X_Web_App_ManifestJson,
    Application_X_Www_Form_Urlencoded,
    Application_X_X509_Ca_Cert,
    Application_X_Xfig,
    Application_X_XliffXml,
    Application_X_Xpinstall,
    Application_X_Xz,
    Application_X_Zmachine,
    Application_X400_Bp,
    Application_XacmlXml,
    Application_XamlXml,
    Application_Xcap_AttXml,
    Application_Xcap_CapsXml,
    Application_Xcap_DiffXml,
    Application_Xcap_ElXml,
    Application_Xcap_ErrorXml,
    Application_Xcap_NsXml,
    Application_Xcon_Conference_Info_DiffXml,
    Application_Xcon_Conference_InfoXml,
    Application_XencXml,
    Application_Xhtml_VoiceXml,
    Application_XhtmlXml,
    Application_XliffXml,
    Application_Xml,
    Application_Xml_Dtd,
    Application_Xml_External_Parsed_Entity,
    Application_Xml_PatchXml,
    Application_XmppXml,
    Application_XopXml,
    Application_XprocXml,
    Application_XsltXml,
    Application_XspfXml,
    Application_XvXml,
    Application_Yang,
    Application_Yang_DataJson,
    Application_Yang_DataXml,
    Application_Yang_PatchJson,
    Application_Yang_PatchXml,
    Application_YinXml,
    Application_Zip,
    Application_Zlib,
    Application_Zstd
];

export const anyAudio = audio("*");
export const Audio_Aac = audio("aac", "aac");
export const Audio_Ac3 = audio("ac3", "ac3");
export const Audio_Adpcm = audio("adpcm", "adp");
export const Audio_AMR = audio("amr", "amr");
export const Audio_AMR_WB = audio("amr-wb");
export const Audio_Amr_WbPlus = audio("amr-wb+");
export const Audio_Aptx = audio("aptx");
export const Audio_Asc = audio("asc");
export const Audio_ATRAC_ADVANCED_LOSSLESS = audio("atrac-advanced-lossless");
export const Audio_ATRAC_X = audio("atrac-x");
export const Audio_ATRAC3 = audio("atrac3");
export const Audio_Basic = audio("basic", "au", "snd");
export const Audio_BV16 = audio("bv16");
export const Audio_BV32 = audio("bv32");
export const Audio_Clearmode = audio("clearmode");
export const Audio_CN = audio("cn");
export const Audio_DAT12 = audio("dat12");
export const Audio_Dls = audio("dls");
export const Audio_Dsr_Es201108 = audio("dsr-es201108");
export const Audio_Dsr_Es202050 = audio("dsr-es202050");
export const Audio_Dsr_Es202211 = audio("dsr-es202211");
export const Audio_Dsr_Es202212 = audio("dsr-es202212");
export const Audio_DV = audio("dv");
export const Audio_DVI4 = audio("dvi4");
export const Audio_Eac3 = audio("eac3");
export const Audio_Encaprtp = audio("encaprtp");
export const Audio_EVRC = audio("evrc");
export const Audio_EVRC_QCP = audio("evrc-qcp");
export const Audio_EVRC0 = audio("evrc0");
export const Audio_EVRC1 = audio("evrc1");
export const Audio_EVRCB = audio("evrcb");
export const Audio_EVRCB0 = audio("evrcb0");
export const Audio_EVRCB1 = audio("evrcb1");
export const Audio_EVRCNW = audio("evrcnw");
export const Audio_EVRCNW0 = audio("evrcnw0");
export const Audio_EVRCNW1 = audio("evrcnw1");
export const Audio_EVRCWB = audio("evrcwb");
export const Audio_EVRCWB0 = audio("evrcwb0");
export const Audio_EVRCWB1 = audio("evrcwb1");
export const Audio_EVS = audio("evs");
export const Audio_Example = audio("example");
export const Audio_Flexfec = audio("flexfec");
export const Audio_Fwdred = audio("fwdred");
export const Audio_G711_0 = audio("g711-0");
export const Audio_G719 = audio("g719");
export const Audio_G722 = audio("g722");
export const Audio_G7221 = audio("g7221");
export const Audio_G723 = audio("g723");
export const Audio_G726_16 = audio("g726-16");
export const Audio_G726_24 = audio("g726-24");
export const Audio_G726_32 = audio("g726-32");
export const Audio_G726_40 = audio("g726-40");
export const Audio_G728 = audio("g728");
export const Audio_G729 = audio("g729");
export const Audio_G7291 = audio("g7291");
export const Audio_G729D = audio("g729d");
export const Audio_G729E = audio("g729e");
export const Audio_GSM = audio("gsm", "gsm");
export const Audio_GSM_EFR = audio("gsm-efr");
export const Audio_GSM_HR_08 = audio("gsm-hr-08");
export const Audio_ILBC = audio("ilbc");
export const Audio_Ip_Mr_v25 = audio("ip-mr_v2.5");
export const Audio_Isac = audio("isac");
export const Audio_L16 = audio("l16");
export const Audio_L20 = audio("l20");
export const Audio_L24 = audio("l24");
export const Audio_L8 = audio("l8");
export const Audio_LPC = audio("lpc");
export const Audio_MELP = audio("melp");
export const Audio_MELP1200 = audio("melp1200");
export const Audio_MELP2400 = audio("melp2400");
export const Audio_MELP600 = audio("melp600");
export const Audio_Midi = audio("midi", "mid", "midi", "kar", "rmi");
export const Audio_Mobile_Xmf = audio("mobile-xmf");
export const Audio_Mp4 = audio("mp4", "m4a", "mp4a");
export const Audio_MP4A_LATM = audio("mp4a-latm");
export const Audio_MPA = audio("mpa");
export const Audio_Mpa_Robust = audio("mpa-robust");
export const Audio_Mpeg = audio("mpeg", "mp3", "mp2", "mp2a", "mpga", "m2a", "m3a");
export const Audio_Mpeg4_Generic = audio("mpeg4-generic");
export const Audio_Musepack = audio("musepack");
export const Audio_Ogg = audio("ogg", "ogg", "oga", "spx");
export const Audio_Opus = audio("opus");
export const Audio_Parityfec = audio("parityfec");
export const Audio_PCMA = audio("pcma");
export const Audio_PCMA_WB = audio("pcma-wb");
export const Audio_PCMU = audio("pcmu");
export const Audio_PCMU_WB = audio("pcmu-wb");
export const Audio_PrsSid = audio("prs.sid");
export const Audio_Qcelp = audio("qcelp");
export const Audio_Raptorfec = audio("raptorfec");
export const Audio_RED = audio("red");
export const Audio_Rtp_Enc_Aescm128 = audio("rtp-enc-aescm128");
export const Audio_Rtp_Midi = audio("rtp-midi");
export const Audio_Rtploopback = audio("rtploopback");
export const Audio_Rtx = audio("rtx");
export const Audio_S3m = audio("s3m", "s3m");
export const Audio_Silk = audio("silk", "sil");
export const Audio_SMV = audio("smv");
export const Audio_SMV_QCP = audio("smv-qcp");
export const Audio_SMV0 = audio("smv0");
export const Audio_Sp_Midi = audio("sp-midi");
export const Audio_Speex = audio("speex");
export const Audio_T140c = audio("t140c");
export const Audio_T38 = audio("t38");
export const Audio_Telephone_Event = audio("telephone-event");
export const Audio_TETRA_ACELP = audio("tetra_acelp");
export const Audio_TETRA_ACELP_BB = audio("tetra_acelp_bb");
export const Audio_Tone = audio("tone");
export const Audio_UEMCLIP = audio("uemclip");
export const Audio_Ulpfec = audio("ulpfec");
export const Audio_Usac = audio("usac");
export const Audio_VDVI = audio("vdvi");
export const Audio_Vendor_1d_Interleaved_Parityfec = audio("1d-interleaved-parityfec");
export const Audio_Vendor_32kadpcm = audio("32kadpcm");
export const Audio_Vendor_3gpp = audio("3gpp");
export const Audio_Vendor_3gpp2 = audio("3gpp2");
export const Audio_Vendor_3gppIufp = audio("vnd.3gpp.iufp");
export const Audio_Vendor_4SB = audio("vnd.4sb");
export const Audio_Vendor_Audiokoz = audio("vnd.audiokoz");
export const Audio_Vendor_CELP = audio("vnd.celp");
export const Audio_Vendor_CiscoNse = audio("vnd.cisco.nse");
export const Audio_Vendor_CmlesRadio_Events = audio("vnd.cmles.radio-events");
export const Audio_Vendor_CnsAnp1 = audio("vnd.cns.anp1");
export const Audio_Vendor_CnsInf1 = audio("vnd.cns.inf1");
export const Audio_Vendor_DeceAudio = audio("vnd.dece.audio", "uva", "uvva");
export const Audio_Vendor_Digital_Winds = audio("vnd.digital-winds", "eol");
export const Audio_Vendor_DlnaAdts = audio("vnd.dlna.adts");
export const Audio_Vendor_DolbyHeaac1 = audio("vnd.dolby.heaac.1");
export const Audio_Vendor_DolbyHeaac2 = audio("vnd.dolby.heaac.2");
export const Audio_Vendor_DolbyMlp = audio("vnd.dolby.mlp");
export const Audio_Vendor_DolbyMps = audio("vnd.dolby.mps");
export const Audio_Vendor_DolbyPl2 = audio("vnd.dolby.pl2");
export const Audio_Vendor_DolbyPl2x = audio("vnd.dolby.pl2x");
export const Audio_Vendor_DolbyPl2z = audio("vnd.dolby.pl2z");
export const Audio_Vendor_DolbyPulse1 = audio("vnd.dolby.pulse.1");
export const Audio_Vendor_Dra = audio("vnd.dra", "dra");
export const Audio_Vendor_Dts = audio("vnd.dts", "dts");
export const Audio_Vendor_DtsHd = audio("vnd.dts.hd", "dtshd");
export const Audio_Vendor_DtsUhd = audio("vnd.dts.uhd");
export const Audio_Vendor_DvbFile = audio("vnd.dvb.file");
export const Audio_Vendor_EveradPlj = audio("vnd.everad.plj");
export const Audio_Vendor_HnsAudio = audio("vnd.hns.audio");
export const Audio_Vendor_LucentVoice = audio("vnd.lucent.voice", "lvp");
export const Audio_Vendor_Ms_PlayreadyMediaPya = audio("vnd.ms-playready.media.pya", "pya");
export const Audio_Vendor_NokiaMobile_Xmf = audio("vnd.nokia.mobile-xmf");
export const Audio_Vendor_NortelVbk = audio("vnd.nortel.vbk");
export const Audio_Vendor_NueraEcelp4800 = audio("vnd.nuera.ecelp4800", "ecelp4800");
export const Audio_Vendor_NueraEcelp7470 = audio("vnd.nuera.ecelp7470", "ecelp7470");
export const Audio_Vendor_NueraEcelp9600 = audio("vnd.nuera.ecelp9600", "ecelp9600");
export const Audio_Vendor_OctelSbc = audio("vnd.octel.sbc");
export const Audio_Vendor_PresonusMultitrack = audio("vnd.presonus.multitrack");
export const Audio_Vendor_Qcelp = deprecate(audio("vnd.qcelp"), "in favor of audio/qcelp");
export const Audio_Vendor_Rhetorex32kadpcm = audio("vnd.rhetorex.32kadpcm");
export const Audio_Vendor_Rip = audio("vnd.rip", "rip");
export const Audio_Vendor_Rn_Realaudio = audio("vnd.rn-realaudio");
export const Audio_Vendor_SealedmediaSoftsealMpeg = audio("vnd.sealedmedia.softseal.mpeg");
export const Audio_Vendor_VmxCvsd = audio("vnd.vmx.cvsd");
export const Audio_Vendor_Wave = audio("vnd.wave", "wav");
export const Audio_VMR_WB = audio("vmr-wb");
export const Audio_Vorbis = audio("vorbis");
export const Audio_Vorbis_Config = audio("vorbis-config");
export const Audio_Wav = audio("wav", "wav");
export const Audio_Wave = audio("wave", "wav");
export const Audio_Webm = audio("webm", "weba");
export const Audio_X_Aac = audio("x-aac", "aac");
export const Audio_X_Aiff = audio("x-aiff", "aif", "aiff", "aifc");
export const Audio_X_Caf = audio("x-caf", "caf");
export const Audio_X_Flac = audio("x-flac", "flac");
export const Audio_X_Matroska = audio("x-matroska", "mka");
export const Audio_X_Mpegurl = audio("x-mpegurl", "m3u");
export const Audio_X_Ms_Wax = audio("x-ms-wax", "wax");
export const Audio_X_Ms_Wma = audio("x-ms-wma", "wma");
export const Audio_X_Pn_Realaudio = audio("x-pn-realaudio", "ram", "ra");
export const Audio_X_Pn_Realaudio_Plugin = audio("x-pn-realaudio-plugin", "rmp");
export const Audio_X_Tta = audio("x-tta");
export const Audio_X_Wav = audio("x-wav", "wav");
export const Audio_Xm = audio("xm", "xm");
export const allAudio = [
    Audio_Aac,
    Audio_Ac3,
    Audio_Adpcm,
    Audio_AMR,
    Audio_AMR_WB,
    Audio_Amr_WbPlus,
    Audio_Aptx,
    Audio_Asc,
    Audio_ATRAC_ADVANCED_LOSSLESS,
    Audio_ATRAC_X,
    Audio_ATRAC3,
    Audio_Basic,
    Audio_BV16,
    Audio_BV32,
    Audio_Clearmode,
    Audio_CN,
    Audio_DAT12,
    Audio_Dls,
    Audio_Dsr_Es201108,
    Audio_Dsr_Es202050,
    Audio_Dsr_Es202211,
    Audio_Dsr_Es202212,
    Audio_DV,
    Audio_DVI4,
    Audio_Eac3,
    Audio_Encaprtp,
    Audio_EVRC,
    Audio_EVRC_QCP,
    Audio_EVRC0,
    Audio_EVRC1,
    Audio_EVRCB,
    Audio_EVRCB0,
    Audio_EVRCB1,
    Audio_EVRCNW,
    Audio_EVRCNW0,
    Audio_EVRCNW1,
    Audio_EVRCWB,
    Audio_EVRCWB0,
    Audio_EVRCWB1,
    Audio_EVS,
    Audio_Example,
    Audio_Flexfec,
    Audio_Fwdred,
    Audio_G711_0,
    Audio_G719,
    Audio_G722,
    Audio_G7221,
    Audio_G723,
    Audio_G726_16,
    Audio_G726_24,
    Audio_G726_32,
    Audio_G726_40,
    Audio_G728,
    Audio_G729,
    Audio_G7291,
    Audio_G729D,
    Audio_G729E,
    Audio_GSM,
    Audio_GSM_EFR,
    Audio_GSM_HR_08,
    Audio_ILBC,
    Audio_Ip_Mr_v25,
    Audio_Isac,
    Audio_L16,
    Audio_L20,
    Audio_L24,
    Audio_L8,
    Audio_LPC,
    Audio_MELP,
    Audio_MELP1200,
    Audio_MELP2400,
    Audio_MELP600,
    Audio_Midi,
    Audio_Mobile_Xmf,
    Audio_Mp4,
    Audio_MP4A_LATM,
    Audio_MPA,
    Audio_Mpa_Robust,
    Audio_Mpeg,
    Audio_Mpeg4_Generic,
    Audio_Musepack,
    Audio_Ogg,
    Audio_Opus,
    Audio_Parityfec,
    Audio_PCMA,
    Audio_PCMA_WB,
    Audio_PCMU,
    Audio_PCMU_WB,
    Audio_PrsSid,
    Audio_Qcelp,
    Audio_Raptorfec,
    Audio_RED,
    Audio_Rtp_Enc_Aescm128,
    Audio_Rtp_Midi,
    Audio_Rtploopback,
    Audio_Rtx,
    Audio_S3m,
    Audio_Silk,
    Audio_SMV,
    Audio_SMV_QCP,
    Audio_SMV0,
    Audio_Sp_Midi,
    Audio_Speex,
    Audio_T140c,
    Audio_T38,
    Audio_Telephone_Event,
    Audio_TETRA_ACELP,
    Audio_TETRA_ACELP_BB,
    Audio_Tone,
    Audio_UEMCLIP,
    Audio_Ulpfec,
    Audio_Usac,
    Audio_VDVI,
    Audio_Vendor_1d_Interleaved_Parityfec,
    Audio_Vendor_32kadpcm,
    Audio_Vendor_3gpp,
    Audio_Vendor_3gpp2,
    Audio_Vendor_3gppIufp,
    Audio_Vendor_4SB,
    Audio_Vendor_Audiokoz,
    Audio_Vendor_CELP,
    Audio_Vendor_CiscoNse,
    Audio_Vendor_CmlesRadio_Events,
    Audio_Vendor_CnsAnp1,
    Audio_Vendor_CnsInf1,
    Audio_Vendor_DeceAudio,
    Audio_Vendor_Digital_Winds,
    Audio_Vendor_DlnaAdts,
    Audio_Vendor_DolbyHeaac1,
    Audio_Vendor_DolbyHeaac2,
    Audio_Vendor_DolbyMlp,
    Audio_Vendor_DolbyMps,
    Audio_Vendor_DolbyPl2,
    Audio_Vendor_DolbyPl2x,
    Audio_Vendor_DolbyPl2z,
    Audio_Vendor_DolbyPulse1,
    Audio_Vendor_Dra,
    Audio_Vendor_Dts,
    Audio_Vendor_DtsHd,
    Audio_Vendor_DtsUhd,
    Audio_Vendor_DvbFile,
    Audio_Vendor_EveradPlj,
    Audio_Vendor_HnsAudio,
    Audio_Vendor_LucentVoice,
    Audio_Vendor_Ms_PlayreadyMediaPya,
    Audio_Vendor_NokiaMobile_Xmf,
    Audio_Vendor_NortelVbk,
    Audio_Vendor_NueraEcelp4800,
    Audio_Vendor_NueraEcelp7470,
    Audio_Vendor_NueraEcelp9600,
    Audio_Vendor_OctelSbc,
    Audio_Vendor_PresonusMultitrack,
    Audio_Vendor_Rhetorex32kadpcm,
    Audio_Vendor_Rip,
    Audio_Vendor_Rn_Realaudio,
    Audio_Vendor_SealedmediaSoftsealMpeg,
    Audio_Vendor_VmxCvsd,
    Audio_Vendor_Wave,
    Audio_VMR_WB,
    Audio_Vorbis,
    Audio_Vorbis_Config,
    Audio_Wav,
    Audio_Wave,
    Audio_Webm,
    Audio_X_Aac,
    Audio_X_Aiff,
    Audio_X_Caf,
    Audio_X_Flac,
    Audio_X_Matroska,
    Audio_X_Mpegurl,
    Audio_X_Ms_Wax,
    Audio_X_Ms_Wma,
    Audio_X_Pn_Realaudio,
    Audio_X_Pn_Realaudio_Plugin,
    Audio_X_Tta,
    Audio_X_Wav,
    Audio_Xm
];

export const anyChemical = chemical("*");
export const Chemical_X_Cdx = chemical("x-cdx", "cdx");
export const Chemical_X_Cif = chemical("x-cif", "cif");
export const Chemical_X_Cmdf = chemical("x-cmdf", "cmdf");
export const Chemical_X_Cml = chemical("x-cml", "cml");
export const Chemical_X_Csml = chemical("x-csml", "csml");
export const Chemical_X_Pdb = chemical("x-pdb");
export const Chemical_X_Xyz = chemical("x-xyz", "xyz");
export const allChemical = [
    Chemical_X_Cdx,
    Chemical_X_Cif,
    Chemical_X_Cmdf,
    Chemical_X_Cml,
    Chemical_X_Csml,
    Chemical_X_Pdb,
    Chemical_X_Xyz
];


export const anyFont = font("*");
export const Font_Collection = font("collection", "ttc");
export const Font_Otf = font("otf", "otf");
export const Font_Sfnt = font("sfnt");
export const Font_Ttf = font("ttf", "ttf");
export const Font_Woff = font("woff", "woff");
export const Font_Woff2 = font("woff2", "woff2");
export const allFont = [
    Font_Collection,
    Font_Otf,
    Font_Sfnt,
    Font_Ttf,
    Font_Woff,
    Font_Woff2
];


export const anyImage = image("*");
export const Image_Aces = image("aces");
export const Image_Apng = image("apng", "apng");
export const Image_Avci = image("avci");
export const Image_Avcs = image("avcs");
export const Image_Avif = image("avif", "avif");
export const Image_Bmp = image("bmp", "bmp");
export const Image_Cgm = image("cgm", "cgm");
export const Image_Dicom_Rle = image("dicom-rle");
export const Image_Emf = image("emf");
export const Image_Example = image("example");
export const Image_EXR = image("x-exr", "exr");
export const Image_Fits = image("fits");
export const Image_G3fax = image("g3fax", "g3");
export const Image_Gif = image("gif", "gif");
export const Image_Heic = image("heic");
export const Image_Heic_Sequence = image("heic-sequence");
export const Image_Heif = image("heif");
export const Image_Heif_Sequence = image("heif-sequence");
export const Image_Hej2k = image("hej2k");
export const Image_Hsj2 = image("hsj2");
export const Image_Ief = image("ief", "ief");
export const Image_Jls = image("jls");
export const Image_Jp2 = image("jp2");
export const Image_Jpeg = image("jpeg", "jpeg", "jpg", "jpe");
export const Image_Jph = image("jph");
export const Image_Jphc = image("jphc");
export const Image_Jpm = image("jpm");
export const Image_Jpx = image("jpx");
export const Image_Jxr = image("jxr");
export const Image_JxrA = image("jxra");
export const Image_JxrS = image("jxrs");
export const Image_Jxs = image("jxs");
export const Image_Jxsc = image("jxsc");
export const Image_Jxsi = image("jxsi");
export const Image_Jxss = image("jxss");
export const Image_Ktx = image("ktx", "ktx");
export const Image_Naplps = image("naplps");
export const Image_Pjpeg = image("pjpeg");
export const Image_Png = image("png", "png");
export const Image_PrsBtif = image("prs.btif", "btif");
export const Image_PrsPti = image("prs.pti");
export const Image_Pwg_Raster = image("pwg-raster");
export const Image_Raw = image("x-raw", "raw");
export const Image_Sgi = image("sgi", "sgi");
export const Image_SvgXml = image("svg+xml", "svg", "svgz");
export const Image_T38 = image("t38");
export const Image_Tiff = image("tiff", "tiff", "tif");
export const Image_Tiff_Fx = image("tiff-fx");
export const Image_Vendor_AdobePhotoshop = image("vnd.adobe.photoshop", "psd");
export const Image_Vendor_AirzipAcceleratorAzv = image("vnd.airzip.accelerator.azv");
export const Image_Vendor_CnsInf2 = image("vnd.cns.inf2");
export const Image_Vendor_DeceGraphic = image("vnd.dece.graphic", "uvi", "uvvi", "uvg", "uvvg");
export const Image_Vendor_Djvu = image("vnd.djvu", "djvu", "djv");
export const Image_Vendor_DvbSubtitle = image("vnd.dvb.subtitle", "sub");
export const Image_Vendor_Dwg = image("vnd.dwg", "dwg");
export const Image_Vendor_Dxf = image("vnd.dxf", "dxf");
export const Image_Vendor_Fastbidsheet = image("vnd.fastbidsheet", "fbs");
export const Image_Vendor_Fpx = image("vnd.fpx", "fpx");
export const Image_Vendor_Fst = image("vnd.fst", "fst");
export const Image_Vendor_FujixeroxEdmics_Mmr = image("vnd.fujixerox.edmics-mmr", "mmr");
export const Image_Vendor_FujixeroxEdmics_Rlc = image("vnd.fujixerox.edmics-rlc", "rlc");
export const Image_Vendor_GlobalgraphicsPgb = image("vnd.globalgraphics.pgb");
export const Image_Vendor_MicrosoftIcon = image("vnd.microsoft.icon");
export const Image_Vendor_Mix = image("vnd.mix");
export const Image_Vendor_MozillaApng = image("vnd.mozilla.apng");
export const Image_Vendor_Ms_Dds = image("vnd.ms-dds", "dds");
export const Image_Vendor_Ms_Modi = image("vnd.ms-modi", "mdi");
export const Image_Vendor_Ms_Photo = image("vnd.ms-photo", "wdp");
export const Image_Vendor_Net_Fpx = image("vnd.net-fpx", "npx");
export const Image_Vendor_Radiance = image("vnd.radiance");
export const Image_Vendor_SealedmediaSoftsealGif = image("vnd.sealedmedia.softseal.gif");
export const Image_Vendor_SealedmediaSoftsealJpg = image("vnd.sealedmedia.softseal.jpg");
export const Image_Vendor_SealedPng = image("vnd.sealed.png");
export const Image_Vendor_Svf = image("vnd.svf");
export const Image_Vendor_TencentTap = image("vnd.tencent.tap");
export const Image_Vendor_ValveSourceTexture = image("vnd.valve.source.texture");
export const Image_Vendor_WapWbmp = image("vnd.wap.wbmp", "wbmp");
export const Image_Vendor_Xiff = image("vnd.xiff", "xif");
export const Image_Vendor_ZbrushPcx = image("vnd.zbrush.pcx");
export const Image_Webp = image("webp", "webp");
export const Image_Wmf = image("wmf");
export const Image_X_3ds = image("x-3ds", "3ds");
export const Image_X_Cmu_Raster = image("x-cmu-raster", "ras");
export const Image_X_Cmx = image("x-cmx", "cmx");
export const Image_X_Emf = deprecate(image("x-emf"), "in favor of image/emf");
export const Image_X_Freehand = image("x-freehand", "fh", "fhc", "fh4", "fh5", "fh7");
export const Image_X_Icon = image("x-icon", "ico");
export const Image_X_Mrsid_Image = image("x-mrsid-image", "sid");
export const Image_X_Ms_Bmp = image("x-ms-bmp");
export const Image_X_Pcx = image("x-pcx", "pcx");
export const Image_X_Pict = image("x-pict", "pic", "pct");
export const Image_X_Portable_Anymap = image("x-portable-anymap", "pnm");
export const Image_X_Portable_Bitmap = image("x-portable-bitmap", "pbm");
export const Image_X_Portable_Graymap = image("x-portable-graymap", "pgm");
export const Image_X_Portable_Pixmap = image("x-portable-pixmap", "ppm");
export const Image_X_Rgb = image("x-rgb", "rgb");
export const Image_X_Tga = image("x-tga", "tga");
export const Image_X_Wmf = deprecate(image("x-wmf"), "in favor of image/wmf");
export const Image_X_Xbitmap = image("x-xbitmap", "xbm");
export const Image_X_Xcf = image("x-xcf");
export const Image_X_Xpixmap = image("x-xpixmap", "xpm");
export const Image_X_Xwindowdump = image("x-xwindowdump", "xwd");
export const allImage = [
    Image_Aces,
    Image_Apng,
    Image_Avci,
    Image_Avcs,
    Image_Avif,
    Image_Bmp,
    Image_Cgm,
    Image_Dicom_Rle,
    Image_Emf,
    Image_Example,
    Image_EXR,
    Image_Fits,
    Image_G3fax,
    Image_Gif,
    Image_Heic,
    Image_Heic_Sequence,
    Image_Heif,
    Image_Heif_Sequence,
    Image_Hej2k,
    Image_Hsj2,
    Image_Ief,
    Image_Jls,
    Image_Jp2,
    Image_Jpeg,
    Image_Jph,
    Image_Jphc,
    Image_Jpm,
    Image_Jpx,
    Image_Jxr,
    Image_JxrA,
    Image_JxrS,
    Image_Jxs,
    Image_Jxsc,
    Image_Jxsi,
    Image_Jxss,
    Image_Ktx,
    Image_Naplps,
    Image_Pjpeg,
    Image_Png,
    Image_PrsBtif,
    Image_PrsPti,
    Image_Pwg_Raster,
    Image_Raw,
    Image_Sgi,
    Image_SvgXml,
    Image_T38,
    Image_Tiff,
    Image_Tiff_Fx,
    Image_Vendor_AdobePhotoshop,
    Image_Vendor_AirzipAcceleratorAzv,
    Image_Vendor_CnsInf2,
    Image_Vendor_DeceGraphic,
    Image_Vendor_Djvu,
    Image_Vendor_DvbSubtitle,
    Image_Vendor_Dwg,
    Image_Vendor_Dxf,
    Image_Vendor_Fastbidsheet,
    Image_Vendor_Fpx,
    Image_Vendor_Fst,
    Image_Vendor_FujixeroxEdmics_Mmr,
    Image_Vendor_FujixeroxEdmics_Rlc,
    Image_Vendor_GlobalgraphicsPgb,
    Image_Vendor_MicrosoftIcon,
    Image_Vendor_Mix,
    Image_Vendor_MozillaApng,
    Image_Vendor_Ms_Dds,
    Image_Vendor_Ms_Modi,
    Image_Vendor_Ms_Photo,
    Image_Vendor_Net_Fpx,
    Image_Vendor_Radiance,
    Image_Vendor_SealedmediaSoftsealGif,
    Image_Vendor_SealedmediaSoftsealJpg,
    Image_Vendor_SealedPng,
    Image_Vendor_Svf,
    Image_Vendor_TencentTap,
    Image_Vendor_ValveSourceTexture,
    Image_Vendor_WapWbmp,
    Image_Vendor_Xiff,
    Image_Vendor_ZbrushPcx,
    Image_Webp,
    Image_Wmf,
    Image_X_3ds,
    Image_X_Cmu_Raster,
    Image_X_Cmx,
    Image_X_Freehand,
    Image_X_Icon,
    Image_X_Mrsid_Image,
    Image_X_Ms_Bmp,
    Image_X_Pcx,
    Image_X_Pict,
    Image_X_Portable_Anymap,
    Image_X_Portable_Bitmap,
    Image_X_Portable_Graymap,
    Image_X_Portable_Pixmap,
    Image_X_Rgb,
    Image_X_Tga,
    Image_X_Xbitmap,
    Image_X_Xcf,
    Image_X_Xpixmap,
    Image_X_Xwindowdump
];


export const anyMessage = message("*");
export const Message_CPIM = message("cpim");
export const Message_Delivery_Status = message("delivery-status");
export const Message_Disposition_Notification = message("disposition-notification");
export const Message_Example = message("example");
export const Message_External_Body = message("external-body");
export const Message_Feedback_Report = message("feedback-report");
export const Message_Global = message("global");
export const Message_Global_Delivery_Status = message("global-delivery-status");
export const Message_Global_Disposition_Notification = message("global-disposition-notification");
export const Message_Global_Headers = message("global-headers");
export const Message_Http = message("http");
export const Message_ImdnXml = message("imdn+xml", "xml");
export const Message_News = deprecate(message("news"), "by RFC5537");
export const Message_Partial = message("partial");
export const Message_Rfc822 = message("rfc822", "eml", "mime");
export const Message_S_Http = message("s-http");
export const Message_Sip = message("sip");
export const Message_Sipfrag = message("sipfrag");
export const Message_Tracking_Status = message("tracking-status");
export const Message_Vendor_SiSimp = deprecate(message("vnd.si.simp"), "by request");
export const Message_Vendor_WfaWsc = message("vnd.wfa.wsc");
export const allMessage = [
    Message_CPIM,
    Message_Delivery_Status,
    Message_Disposition_Notification,
    Message_Example,
    Message_External_Body,
    Message_Feedback_Report,
    Message_Global,
    Message_Global_Delivery_Status,
    Message_Global_Disposition_Notification,
    Message_Global_Headers,
    Message_Http,
    Message_ImdnXml,
    Message_Partial,
    Message_Rfc822,
    Message_S_Http,
    Message_Sip,
    Message_Sipfrag,
    Message_Tracking_Status,
    Message_Vendor_WfaWsc
];


export const anyModel = model("*");
export const Model_Example = model("example");
export const Model_Gltf_Binary = model("gltf-binary", "glb");
export const Model_Gltf_Json = model("gltf+json", "gltf");
export const Model_Iges = model("iges", "igs", "iges");
export const Model_Mesh = model("mesh", "msh", "mesh", "silo");
export const Model_Stl = model("stl");
export const Model_Vendor_3mf = model("3mf");
export const Model_Vendor_ColladaXml = model("vnd.collada+xml", "dae");
export const Model_Vendor_Dwf = model("vnd.dwf", "dwf");
export const Model_Vendor_Flatland3dml = model("vnd.flatland.3dml");
export const Model_Vendor_Gdl = model("vnd.gdl", "gdl");
export const Model_Vendor_Gs_Gdl = model("vnd.gs-gdl");
export const Model_Vendor_GsGdl = model("vnd.gs.gdl");
export const Model_Vendor_Gtw = model("vnd.gtw", "gtw");
export const Model_Vendor_MomlXml = model("vnd.moml+xml", "xml");
export const Model_Vendor_Mts = model("vnd.mts", "mts");
export const Model_Vendor_Opengex = model("vnd.opengex");
export const Model_Vendor_ParasolidTransmitBinary = model("vnd.parasolid.transmit.binary");
export const Model_Vendor_ParasolidTransmitText = model("vnd.parasolid.transmit.text");
export const Model_Vendor_RosetteAnnotated_Data_Model = model("vnd.rosette.annotated-data-model");
export const Model_Vendor_UsdzZip = model("vnd.usdz+zip", "zip");
export const Model_Vendor_ValveSourceCompiled_Map = model("vnd.valve.source.compiled-map");
export const Model_Vendor_Vtu = model("vnd.vtu", "vtu");
export const Model_Vrml = model("vrml", "wrl", "vrml");
export const Model_X3d_Vrml = model("x3d-vrml");
export const Model_X3dBinary = model("x3d+binary", "x3db", "x3dbz");
export const Model_X3dFastinfoset = model("x3d+fastinfoset", "fastinfoset");
export const Model_X3dVrml = model("x3d+vrml", "x3dv", "x3dvz");
export const Model_X3dXml = model("x3d+xml", "x3d", "x3dz");
export const allModel = [
    Model_Example,
    Model_Gltf_Binary,
    Model_Gltf_Json,
    Model_Iges,
    Model_Mesh,
    Model_Stl,
    Model_Vendor_3mf,
    Model_Vendor_ColladaXml,
    Model_Vendor_Dwf,
    Model_Vendor_Flatland3dml,
    Model_Vendor_Gdl,
    Model_Vendor_Gs_Gdl,
    Model_Vendor_GsGdl,
    Model_Vendor_Gtw,
    Model_Vendor_MomlXml,
    Model_Vendor_Mts,
    Model_Vendor_Opengex,
    Model_Vendor_ParasolidTransmitBinary,
    Model_Vendor_ParasolidTransmitText,
    Model_Vendor_RosetteAnnotated_Data_Model,
    Model_Vendor_UsdzZip,
    Model_Vendor_ValveSourceCompiled_Map,
    Model_Vendor_Vtu,
    Model_Vrml,
    Model_X3d_Vrml,
    Model_X3dBinary,
    Model_X3dFastinfoset,
    Model_X3dVrml,
    Model_X3dXml
];


export const anyMultipart = multipart("*");
export const MultipartAlternative = multipart("alternative");
export const MultipartAppledouble = multipart("appledouble");
export const MultipartByteranges = multipart("byteranges");
export const MultipartDigest = multipart("digest");
export const MultipartEncrypted = multipart("encrypted");
export const MultipartExample = multipart("example");
export const MultipartForm_Data = multipart("form-data");
export const MultipartHeader_Set = multipart("header-set");
export const MultipartMixed = multipart("mixed");
export const MultipartMultilingual = multipart("multilingual");
export const MultipartParallel = multipart("parallel");
export const MultipartRelated = multipart("related");
export const MultipartReport = multipart("report");
export const MultipartSigned = multipart("signed");
export const MultipartVendorBintMed_Plus = multipart("vnd.bint.med-plus");
export const MultipartVoice_Message = multipart("voice-message");
export const MultipartX_Mixed_Replace = multipart("x-mixed-replace");
export const allMultipart = [
    MultipartAlternative,
    MultipartAppledouble,
    MultipartByteranges,
    MultipartDigest,
    MultipartEncrypted,
    MultipartExample,
    MultipartForm_Data,
    MultipartHeader_Set,
    MultipartMixed,
    MultipartMultilingual,
    MultipartParallel,
    MultipartRelated,
    MultipartReport,
    MultipartSigned,
    MultipartVendorBintMed_Plus,
    MultipartVoice_Message,
    MultipartX_Mixed_Replace
];


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



export const anyVideo = video("*");
export const Video_BMPEG = video("bmpeg");
export const Video_BT656 = video("bt656");
export const Video_CelB = video("celb");
export const Video_DV = video("dv");
export const Video_Encaprtp = video("encaprtp");
export const Video_Example = video("example");
export const Video_Flexfec = video("flexfec");
export const Video_H261 = video("h261", "h261");
export const Video_H263 = video("h263", "h263");
export const Video_H263_1998 = video("h263-1998");
export const Video_H263_2000 = video("h263-2000");
export const Video_H264 = video("h264", "h264");
export const Video_H264_RCDO = video("h264-rcdo");
export const Video_H264_SVC = video("h264-svc");
export const Video_H265 = video("h265");
export const Video_IsoSegment = video("iso.segment");
export const Video_JPEG = video("jpeg", "jpgv");
export const Video_Jpeg2000 = video("jpeg2000");
export const Video_Jpm = video("jpm", "jpm", "jpgm");
export const Video_Mj2 = video("mj2", "mj2", "mjp2");
export const Video_MP1S = video("mp1s");
export const Video_MP2P = video("mp2p");
export const Video_MP2T = video("mp2t");
export const Video_Mp4 = video("mp4", "mp4", "mp4v", "mpg4");
export const Video_MP4V_ES = video("mp4v-es");
export const Video_Mpeg = video("mpeg", "mpeg", "mpg", "mpe", "m1v", "m2v");
export const Video_Mpeg4_Generic = video("mpeg4-generic");
export const Video_MPV = video("mpv");
export const Video_Nv = video("nv");
export const Video_Ogg = video("ogg", "ogv");
export const Video_Parityfec = video("parityfec");
export const Video_Pointer = video("pointer");
export const Video_Quicktime = video("quicktime", "qt", "mov");
export const Video_Raptorfec = video("raptorfec");
export const Video_Raw = video("raw");
export const Video_Rtp_Enc_Aescm128 = video("rtp-enc-aescm128");
export const Video_Rtploopback = video("rtploopback");
export const Video_Rtx = video("rtx");
export const Video_Smpte291 = video("smpte291");
export const Video_SMPTE292M = video("smpte292m");
export const Video_Ulpfec = video("ulpfec");
export const Video_Vc1 = video("vc1");
export const Video_Vc2 = video("vc2");
export const Video_Vendor_1d_Interleaved_Parityfec = video("1d-interleaved-parityfec");
export const Video_Vendor_3gpp = video("3gpp", "3gp");
export const Video_Vendor_3gpp_Tt = video("3gpp-tt");
export const Video_Vendor_3gpp2 = video("3gpp2", "3g2");
export const Video_Vendor_CCTV = video("vnd.cctv");
export const Video_Vendor_DeceHd = video("vnd.dece.hd", "uvh", "uvvh");
export const Video_Vendor_DeceMobile = video("vnd.dece.mobile", "uvm", "uvvm");
export const Video_Vendor_DeceMp4 = video("vnd.dece.mp4");
export const Video_Vendor_DecePd = video("vnd.dece.pd", "uvp", "uvvp");
export const Video_Vendor_DeceSd = video("vnd.dece.sd", "uvs", "uvvs");
export const Video_Vendor_DeceVideo = video("vnd.dece.video", "uvv", "uvvv");
export const Video_Vendor_DirectvMpeg = video("vnd.directv.mpeg");
export const Video_Vendor_DirectvMpeg_Tts = video("vnd.directv.mpeg-tts");
export const Video_Vendor_DlnaMpeg_Tts = video("vnd.dlna.mpeg-tts");
export const Video_Vendor_DvbFile = video("vnd.dvb.file", "dvb");
export const Video_Vendor_Fvt = video("vnd.fvt", "fvt");
export const Video_Vendor_HnsVideo = video("vnd.hns.video");
export const Video_Vendor_Iptvforum1dparityfec_1010 = video("vnd.iptvforum.1dparityfec-1010");
export const Video_Vendor_Iptvforum1dparityfec_2005 = video("vnd.iptvforum.1dparityfec-2005");
export const Video_Vendor_Iptvforum2dparityfec_1010 = video("vnd.iptvforum.2dparityfec-1010");
export const Video_Vendor_Iptvforum2dparityfec_2005 = video("vnd.iptvforum.2dparityfec-2005");
export const Video_Vendor_IptvforumTtsavc = video("vnd.iptvforum.ttsavc");
export const Video_Vendor_IptvforumTtsmpeg2 = video("vnd.iptvforum.ttsmpeg2");
export const Video_Vendor_MotorolaVideo = video("vnd.motorola.video");
export const Video_Vendor_MotorolaVideop = video("vnd.motorola.videop");
export const Video_Vendor_Mpegurl = video("vnd.mpegurl", "mxu", "m4u");
export const Video_Vendor_Ms_PlayreadyMediaPyv = video("vnd.ms-playready.media.pyv", "pyv");
export const Video_Vendor_NokiaInterleaved_Multimedia = video("vnd.nokia.interleaved-multimedia");
export const Video_Vendor_NokiaMp4vr = video("vnd.nokia.mp4vr");
export const Video_Vendor_NokiaVideovoip = video("vnd.nokia.videovoip");
export const Video_Vendor_Objectvideo = video("vnd.objectvideo");
export const Video_Vendor_RadgamettoolsBink = video("vnd.radgamettools.bink");
export const Video_Vendor_RadgamettoolsSmacker = video("vnd.radgamettools.smacker");
export const Video_Vendor_SealedmediaSoftsealMov = video("vnd.sealedmedia.softseal.mov");
export const Video_Vendor_SealedMpeg1 = video("vnd.sealed.mpeg1");
export const Video_Vendor_SealedMpeg4 = video("vnd.sealed.mpeg4");
export const Video_Vendor_SealedSwf = video("vnd.sealed.swf");
export const Video_Vendor_UvvuMp4 = video("vnd.uvvu.mp4", "uvu", "uvvu");
export const Video_Vendor_Vivo = video("vnd.vivo", "viv");
export const Video_Vendor_YoutubeYt = video("vnd.youtube.yt");
export const Video_VP8 = video("vp8");
export const Video_Webm = video("webm", "webm");
export const Video_X_F4v = video("x-f4v", "f4v");
export const Video_X_Fli = video("x-fli", "fli");
export const Video_X_Flv = video("x-flv", "flv");
export const Video_X_M4v = video("x-m4v", "m4v");
export const Video_X_Matroska = video("x-matroska", "mkv", "mk3d", "mks");
export const Video_X_Mng = video("x-mng", "mng");
export const Video_X_Ms_Asf = video("x-ms-asf", "asf", "asx");
export const Video_X_Ms_Vob = video("x-ms-vob", "vob");
export const Video_X_Ms_Wm = video("x-ms-wm", "wm");
export const Video_X_Ms_Wmv = video("x-ms-wmv", "wmv");
export const Video_X_Ms_Wmx = video("x-ms-wmx", "wmx");
export const Video_X_Ms_Wvx = video("x-ms-wvx", "wvx");
export const Video_X_Msvideo = video("x-msvideo", "avi");
export const Video_X_Sgi_Movie = video("x-sgi-movie", "movie");
export const Video_X_Smv = video("x-smv", "smv");
export const allVideo = [
    Video_BMPEG,
    Video_BT656,
    Video_CelB,
    Video_DV,
    Video_Encaprtp,
    Video_Example,
    Video_Flexfec,
    Video_H261,
    Video_H263,
    Video_H263_1998,
    Video_H263_2000,
    Video_H264,
    Video_H264_RCDO,
    Video_H264_SVC,
    Video_H265,
    Video_IsoSegment,
    Video_JPEG,
    Video_Jpeg2000,
    Video_Jpm,
    Video_Mj2,
    Video_MP1S,
    Video_MP2P,
    Video_MP2T,
    Video_Mp4,
    Video_MP4V_ES,
    Video_Mpeg,
    Video_Mpeg4_Generic,
    Video_MPV,
    Video_Nv,
    Video_Ogg,
    Video_Parityfec,
    Video_Pointer,
    Video_Quicktime,
    Video_Raptorfec,
    Video_Raw,
    Video_Rtp_Enc_Aescm128,
    Video_Rtploopback,
    Video_Rtx,
    Video_Smpte291,
    Video_SMPTE292M,
    Video_Ulpfec,
    Video_Vc1,
    Video_Vc2,
    Video_Vendor_1d_Interleaved_Parityfec,
    Video_Vendor_3gpp,
    Video_Vendor_3gpp_Tt,
    Video_Vendor_3gpp2,
    Video_Vendor_CCTV,
    Video_Vendor_DeceHd,
    Video_Vendor_DeceMobile,
    Video_Vendor_DeceMp4,
    Video_Vendor_DecePd,
    Video_Vendor_DeceSd,
    Video_Vendor_DeceVideo,
    Video_Vendor_DirectvMpeg,
    Video_Vendor_DirectvMpeg_Tts,
    Video_Vendor_DlnaMpeg_Tts,
    Video_Vendor_DvbFile,
    Video_Vendor_Fvt,
    Video_Vendor_HnsVideo,
    Video_Vendor_Iptvforum1dparityfec_1010,
    Video_Vendor_Iptvforum1dparityfec_2005,
    Video_Vendor_Iptvforum2dparityfec_1010,
    Video_Vendor_Iptvforum2dparityfec_2005,
    Video_Vendor_IptvforumTtsavc,
    Video_Vendor_IptvforumTtsmpeg2,
    Video_Vendor_MotorolaVideo,
    Video_Vendor_MotorolaVideop,
    Video_Vendor_Mpegurl,
    Video_Vendor_Ms_PlayreadyMediaPyv,
    Video_Vendor_NokiaInterleaved_Multimedia,
    Video_Vendor_NokiaMp4vr,
    Video_Vendor_NokiaVideovoip,
    Video_Vendor_Objectvideo,
    Video_Vendor_RadgamettoolsBink,
    Video_Vendor_RadgamettoolsSmacker,
    Video_Vendor_SealedmediaSoftsealMov,
    Video_Vendor_SealedMpeg1,
    Video_Vendor_SealedMpeg4,
    Video_Vendor_SealedSwf,
    Video_Vendor_UvvuMp4,
    Video_Vendor_Vivo,
    Video_Vendor_YoutubeYt,
    Video_VP8,
    Video_Webm,
    Video_X_F4v,
    Video_X_Fli,
    Video_X_Flv,
    Video_X_M4v,
    Video_X_Matroska,
    Video_X_Mng,
    Video_X_Ms_Asf,
    Video_X_Ms_Vob,
    Video_X_Ms_Wm,
    Video_X_Ms_Wmv,
    Video_X_Ms_Wmx,
    Video_X_Ms_Wvx,
    Video_X_Msvideo,
    Video_X_Sgi_Movie,
    Video_X_Smv
];


export const anyXConference = xConference("*");
export const XConference_XCooltalk = xConference("x-cooltalk", "ice");


export const anyXShader = xShader("*");
export const XShader_XVertex = xShader("x-vertex", "vert", "vs", "glsl");
export const XShader_XFragment = xShader("x-fragment", "frag", "fs", "glsl");
export const allShader = [
    XShader_XVertex,
    XShader_XFragment
];