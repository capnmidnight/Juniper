import { getIndex, identity } from "./filters";
import { isDefined, isNullOrUndefined } from "./typeChecks";

export function makeLookup<T>(items: readonly T[]): Map<number, T>;
export function makeLookup<T, K>(items: readonly T[], makeKey: (value: T, i?: number) => K): Map<K, T>;
export function makeLookup<T, K, V>(items: readonly T[], makeKey: (value: T, i?: number) => K, makeValue: (value: T, i?: number) => V): Map<K, V>;
export function makeLookup<T>(items: readonly T[], makeKey?: (value: T, i?: number) => unknown, makeValue?: (value: T, i?: number) => unknown): Map<unknown, unknown> {
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

export function mapMap<K1, V1, K2, V2>(map: Map<K1, V1>, makeKey: (key: K1, value?: V1) => K2, makeValue: (value: V1, key?: K1) => V2): Map<K2, V2> {
    const mapOut = new Map<K2, V2>();
    for (const [key, value] of map) {
        mapOut.set(
            makeKey(key, value),
            makeValue(value, key)
        );
    }

    return mapOut;
}

export function mapInvert<K, V>(map: Map<K, V>): Map<V, K> {
    return mapMap(map, (_, value) => value, (_, key) => key);
}

export function makeReverseLookup<T>(items: readonly T[]): Map<T, number>;
export function makeReverseLookup<T, V>(items: readonly T[], makeValue: (value: T, i?: number) => V): Map<T, V>;
export function makeReverseLookup<T>(items: readonly T[], makeValue?: (value: T, i?: number) => any): Map<T, any> {
    if (isNullOrUndefined(makeValue)) {
        makeValue = getIndex;
    }
    return makeLookup(items, identity, makeValue);
}

export function mapPivot<T, K, V>(mapMap: Map<T, Map<K, V>>): Map<K, Map<T, V>> {
    const mapOut = new Map<K, Map<T, V>>()

    for (const [t, map] of mapMap) {
        for (const [k, v] of map) {
            if (!mapOut.has(k)) {
                mapOut.set(k, new Map<T, V>());
            }

            mapOut.get(k).set(t, v);
        }
    }

    return mapOut;
}

export function mapJoin<KeyT, ValueT>(dest: Map<KeyT, ValueT>, ...sources: Map<KeyT, ValueT>[]): Map<KeyT, ValueT> {
    for (const source of sources) {
        if (isDefined(source)) {
            for (const [key, value] of source) {
                dest.set(key, value);
            }
        }
    }

    return dest;
}

type RecordKeyNames = "string" | "number" | "symbol";
type RecordKeyTypes = string | number | symbol;

interface RecordKeys extends Record<RecordKeyNames, RecordKeyTypes> {
    "string": string;
    "number": number;
    "symbol": symbol;
}

export function objToMap<T, K extends RecordKeyNames>(type: K, obj: Record<RecordKeys[K], T>): Map<RecordKeys[K], T> {
    const map = new Map<RecordKeys[K], T>();
    for (const [keyStr, value] of Object.entries(obj)) {
        const key: any = type === "number"
            ? parseFloat(keyStr)
            : type === "symbol"
                ? Symbol(keyStr)
                : keyStr;
        map.set(key, value as T);
    }
    return map;
}

export function mapReplace<K, T>(destination: Map<K, T>, source: Map<K, T>) {
    destination.clear();
    for (const [key, value] of source) {
        destination.set(key, value);
    }
} 
