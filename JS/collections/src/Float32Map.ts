import { isArrayBuffer, makeLookup, mapInvert } from "@juniper-lib/util";

export class Float32Map extends Float32Array {

    #namesToIndex: Map<string, number>;

    constructor(keys: string[]);
    constructor(buffer: ArrayBufferLike, byteOffset: number, keys: string[]);
    constructor(keysOrBuffer: string[] | ArrayBufferLike, byteOffset?: number, keys?: string[]) {
        let buffer: ArrayBufferLike = null;
        if (isArrayBuffer(keysOrBuffer)) {
            buffer = keysOrBuffer;
        }
        else {
            keys = keysOrBuffer;
        }


        if (buffer) {
            // @ts-ignore
            super(buffer, byteOffset, keys.length);
        }
        else {
            super(keys.length);
        }

        this.#namesToIndex = mapInvert(makeLookup(keys, (_, i) => i));
    }

    has(key: string): boolean {
        return this.#namesToIndex.has(key);
    }

    get(key: string): number {
        return this[this.#namesToIndex.get(key)];
    }

    put(key: string, value: number): void {
        this[this.#namesToIndex.get(key)] = value;
    }
}
