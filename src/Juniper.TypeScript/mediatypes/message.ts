import { deprecate, specialize } from "./util";

const message = specialize("message");

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