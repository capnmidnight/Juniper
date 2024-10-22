import { isObject } from "@juniper-lib/util";
export function isErsatzNode(obj) {
    return isObject(obj)
        && "content" in obj
        && (obj.content instanceof Element
            || obj.content instanceof DocumentFragment);
}
export function isNodes(obj) {
    return obj instanceof Node
        || isErsatzNode(obj);
}
export function resolveNode(obj) {
    if (isErsatzNode(obj)) {
        return obj.content;
    }
    return obj;
}
//# sourceMappingURL=ErsatzNode.js.map