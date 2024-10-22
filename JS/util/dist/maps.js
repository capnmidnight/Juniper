import { getIndex, identity } from "./filters";
import { isDefined, isNullOrUndefined } from "./typeChecks";
export function makeLookup(items, makeKey, makeValue) {
    if (!makeKey) {
        makeKey = getIndex;
    }
    if (!makeValue) {
        makeValue = identity;
    }
    return new Map(items.map((item, i) => [
        makeKey(item, i),
        makeValue(item, i)
    ]));
}
export function mapMap(map, makeKey, makeValue) {
    const mapOut = new Map();
    for (const [key, value] of map) {
        mapOut.set(makeKey(key, value), makeValue(value, key));
    }
    return mapOut;
}
export function mapInvert(map) {
    return mapMap(map, (_, value) => value, (_, key) => key);
}
export function makeReverseLookup(items, makeValue) {
    if (isNullOrUndefined(makeValue)) {
        makeValue = getIndex;
    }
    return makeLookup(items, identity, makeValue);
}
export function mapPivot(mapMap) {
    const mapOut = new Map();
    for (const [t, map] of mapMap) {
        for (const [k, v] of map) {
            if (!mapOut.has(k)) {
                mapOut.set(k, new Map());
            }
            mapOut.get(k).set(t, v);
        }
    }
    return mapOut;
}
export function mapJoin(dest, ...sources) {
    for (const source of sources) {
        if (isDefined(source)) {
            for (const [key, value] of source) {
                dest.set(key, value);
            }
        }
    }
    return dest;
}
export function objToMap(type, obj) {
    const map = new Map();
    for (const [keyStr, value] of Object.entries(obj)) {
        const key = type === "number"
            ? parseFloat(keyStr)
            : type === "symbol"
                ? Symbol(keyStr)
                : keyStr;
        map.set(key, value);
    }
    return map;
}
export function mapReplace(destination, source) {
    destination.clear();
    for (const [key, value] of source) {
        destination.set(key, value);
    }
}
//# sourceMappingURL=maps.js.map