import { isDefined } from "../typeChecks";

export function stringToName(...parts: string[]): string {
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
