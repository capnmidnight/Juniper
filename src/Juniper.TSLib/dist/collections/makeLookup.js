import { mapMap } from "../";
export function makeLookup(items, makeID) {
    return mapMap(items, makeID, i => i);
}
