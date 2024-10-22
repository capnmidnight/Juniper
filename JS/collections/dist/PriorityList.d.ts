export declare class PriorityList<KeyT, ValueT> {
    #private;
    constructor(init?: [KeyT, ValueT][]);
    add(key: KeyT, ...values: ValueT[]): this;
    entries(): IterableIterator<[KeyT, ValueT[]]>;
    [Symbol.iterator](): IterableIterator<[KeyT, ValueT[]]>;
    keys(): IterableIterator<KeyT>;
    values(): IterableIterator<ValueT>;
    has(key: KeyT): boolean;
    get(key: KeyT): ValueT[];
    count(key: KeyT): number;
    get size(): number;
    delete(key: KeyT): boolean;
    remove(key: KeyT, value: ValueT): void;
    clear(): void;
}
//# sourceMappingURL=PriorityList.d.ts.map