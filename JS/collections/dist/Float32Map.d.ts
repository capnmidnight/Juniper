export declare class Float32Map extends Float32Array {
    #private;
    constructor(keys: string[]);
    constructor(buffer: ArrayBufferLike, byteOffset: number, keys: string[]);
    has(key: string): boolean;
    get(key: string): number;
    put(key: string, value: number): void;
}
//# sourceMappingURL=Float32Map.d.ts.map