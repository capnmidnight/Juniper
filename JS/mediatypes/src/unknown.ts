import { specialize } from "./util";

const unknown = /*@__PURE__*/ (function() { return specialize("unknown"); })();

export const Unknown_Unknown = /*@__PURE__*/ (function() { return unknown("unknown", "unknown"); })();
