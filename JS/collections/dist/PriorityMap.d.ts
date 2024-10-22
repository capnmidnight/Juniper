export declare class PriorityMap<Key1T, Key2T, ValueT> {
    private readonly items;
    constructor(init?: Iterable<[Key1T, Key2T, ValueT]>);
    add(key1: Key1T, key2: Key2T, value: ValueT): this;
    entries(): IterableIterator<[Key1T, Key2T, ValueT]>;
    keys(): IterableIterator<Key1T>;
    keys(key1: Key1T): IterableIterator<Key2T>;
    values(): IterableIterator<ValueT>;
    has(key1: Key1T, key2?: Key2T): boolean;
    get(key1: Key1T): Map<Key2T, ValueT>;
    get(key1: Key1T, key2: Key2T): ValueT;
    count(key1: Key1T): number;
    get size(): number;
    delete(key1: Key1T, key2?: Key2T): boolean;
    clear(): void;
}
//# sourceMappingURL=PriorityMap.d.ts.map