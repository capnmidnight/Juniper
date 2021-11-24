import { mapMap } from "../";
export function mapBuild(items, makeValue) {
    return mapMap(items, i => i, makeValue);
}
