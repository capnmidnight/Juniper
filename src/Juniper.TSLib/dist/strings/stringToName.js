import { isDefined } from "../";
export function stringToName(...parts) {
    const goodParts = [];
    for (const part of parts) {
        if (isDefined(part)
            && part.length > 0
            && goodParts.indexOf(part) === -1) {
            goodParts.push(part);
        }
    }
    return goodParts.join("-");
}
