export declare function makeLookup<T>(items: readonly T[]): Map<number, T>;
export declare function makeLookup<T, K>(items: readonly T[], makeKey: (value: T, i?: number) => K): Map<K, T>;
export declare function makeLookup<T, K, V>(items: readonly T[], makeKey: (value: T, i?: number) => K, makeValue: (value: T, i?: number) => V): Map<K, V>;
export declare function mapMap<K1, V1, K2, V2>(map: Map<K1, V1>, makeKey: (key: K1, value?: V1) => K2, makeValue: (value: V1, key?: K1) => V2): Map<K2, V2>;
export declare function mapInvert<K, V>(map: Map<K, V>): Map<V, K>;
export declare function makeReverseLookup<T>(items: readonly T[]): Map<T, number>;
export declare function makeReverseLookup<T, V>(items: readonly T[], makeValue: (value: T, i?: number) => V): Map<T, V>;
export declare function mapPivot<T, K, V>(mapMap: Map<T, Map<K, V>>): Map<K, Map<T, V>>;
export declare function mapJoin<KeyT, ValueT>(dest: Map<KeyT, ValueT>, ...sources: Map<KeyT, ValueT>[]): Map<KeyT, ValueT>;
type RecordKeyNames = "string" | "number" | "symbol";
type RecordKeyTypes = string | number | symbol;
interface RecordKeys extends Record<RecordKeyNames, RecordKeyTypes> {
    "string": string;
    "number": number;
    "symbol": symbol;
}
export declare function objToMap<T, K extends RecordKeyNames>(type: K, obj: Record<RecordKeys[K], T>): Map<RecordKeys[K], T>;
export declare function mapReplace<K, T>(destination: Map<K, T>, source: Map<K, T>): void;
export {};
//# sourceMappingURL=maps.d.ts.map