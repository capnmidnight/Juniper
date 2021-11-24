import { mapMap } from "./mapMap";
export function mapBuild(items, makeValue) {
    return mapMap(items, i => i, makeValue);
}
