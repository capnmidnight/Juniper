import { isDefined } from "@juniper-lib/util";
export function isEndpoint(obj) {
    return isDefined(obj)
        && "_resolveInput" in obj;
}
export function isIAudioNode(obj) {
    return isEndpoint(obj)
        && "_resolveOutput" in obj;
}
//# sourceMappingURL=IAudioNode.js.map