import { specialize } from "./util";

const multipart = specialize("multipart");

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