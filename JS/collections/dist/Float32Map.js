import { isArrayBuffer, makeLookup, mapInvert } from "@juniper-lib/util";
export class Float32Map extends Float32Array {
    #namesToIndex;
    constructor(keysOrBuffer, byteOffset, keys) {
        let buffer = null;
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
    has(key) {
        return this.#namesToIndex.has(key);
    }
    get(key) {
        return this[this.#namesToIndex.get(key)];
    }
    put(key, value) {
        this[this.#namesToIndex.get(key)] = value;
    }
}
//# sourceMappingURL=Float32Map.js.map