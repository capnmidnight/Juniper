export declare class PrioritySet<K, V> {
    #private;
    constructor(init?: Iterable<[K, V]>);
    add(key: K, firstValue: V, ...values: V[]): this;
    entries(): MapIterator<[K, Set<V>]>;
    [Symbol.iterator](): MapIterator<[K, Set<V>]>;
    keys(): MapIterator<K>;
    values(): Generator<V, void, unknown>;
    has(key: K, value?: V): boolean;
    get(key: K): Set<V>;
    count(key: K): number;
    get size(): number;
    delete(key: K, value?: V): boolean;
    clear(): void;
}
//# sourceMappingURL=PrioritySet.d.ts.map