import { isString } from "@juniper-lib/tslib/dist/typeChecks";
export function isVideoRecord(obj) {
    return isString(obj.vcodec);
}
//# sourceMappingURL=data.js.map