export declare class RingBuffer<T> {
    #private;
    get length(): number;
    constructor(size: number);
    push(item: T): void;
    [Symbol.iterator](): Generator<T, void, unknown>;
    reduce<V>(callback: (accum: V, next: T) => V, start: V): V;
}
//# sourceMappingURL=RingBuffer.d.ts.map