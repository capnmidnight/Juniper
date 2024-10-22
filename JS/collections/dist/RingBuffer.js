export class RingBuffer {
    #elements;
    #size = 0;
    #start = 0;
    #end = 0;
    get length() { return this.#end - this.#start; }
    constructor(size) {
        this.#size = size;
        this.#elements = new Array(size);
    }
    push(item) {
        this.#elements[this.#end % this.#size] = item;
        ++this.#end;
        if (this.length > this.#size) {
            ++this.#start;
            if (this.#start >= this.#size) {
                this.#start -= this.#size;
                this.#end -= this.#size;
            }
        }
    }
    *[Symbol.iterator]() {
        for (let i = this.#start; i < this.#end; ++i) {
            yield this.#elements[i % this.#size];
        }
    }
    reduce(callback, start) {
        for (const next of this) {
            start = callback(start, next);
        }
        return start;
    }
}
//# sourceMappingURL=RingBuffer.js.map