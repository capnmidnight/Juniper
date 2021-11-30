import { mapMap } from "./mapMap";
export function makeLookup(items, makeID) {
    return mapMap(items, makeID, i => i);
}
