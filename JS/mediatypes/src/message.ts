import { specialize } from "./util";

const message = /*@__PURE__*/ (function() { return specialize("message"); })();

export const Message_CPIM = /*@__PURE__*/ (function() { return message("cpim"); })();
export const Message_Delivery_Status = /*@__PURE__*/ (function() { return message("delivery-status"); })();
export const Message_Disposition_Notification = /*@__PURE__*/ (function() { return message("disposition-notification"); })();
export const Message_Example = /*@__PURE__*/ (function() { return message("example"); })();
export const Message_External_Body = /*@__PURE__*/ (function() { return message("external-body"); })();
export const Message_Feedback_Report = /*@__PURE__*/ (function() { return message("feedback-report"); })();
export const Message_Global = /*@__PURE__*/ (function() { return message("global"); })();
export const Message_Global_Delivery_Status = /*@__PURE__*/ (function() { return message("global-delivery-status"); })();
export const Message_Global_Disposition_Notification = /*@__PURE__*/ (function() { return message("global-disposition-notification"); })();
export const Message_Global_Headers = /*@__PURE__*/ (function() { return message("global-headers"); })();
export const Message_Http = /*@__PURE__*/ (function() { return message("http"); })();
export const Message_ImdnXml = /*@__PURE__*/ (function() { return message("imdn+xml", "xml"); })();
export const Message_News = /*@__PURE__*/ (function() { return message("news").deprecate("OBSOLETED by RFC5537"); })();
export const Message_Partial = /*@__PURE__*/ (function() { return message("partial"); })();
export const Message_Rfc822 = /*@__PURE__*/ (function() { return message("rfc822", "eml", "mime"); })();
export const Message_S_Http = /*@__PURE__*/ (function() { return message("s-http"); })();
export const Message_Sip = /*@__PURE__*/ (function() { return message("sip"); })();
export const Message_Sipfrag = /*@__PURE__*/ (function() { return message("sipfrag"); })();
export const Message_Tracking_Status = /*@__PURE__*/ (function() { return message("tracking-status"); })();
export const Message_Vendor_SiSimp = /*@__PURE__*/ (function() { return message("vnd.si.simp").deprecate("OBSOLETED by request"); })();
export const Message_Vendor_WfaWsc = /*@__PURE__*/ (function() { return message("vnd.wfa.wsc"); })();
