export class RingBuffer<T> {

    readonly #elements: T[];

    #size: number = 0;
    #start: number = 0;
    #end: number = 0;

    get length() { return this.#end - this.#start; }

    constructor(size: number) {
        this.#size = size;
        this.#elements = new Array(size);
    }

    push(item: T) {
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

    reduce<V>(callback: (accum: V, next: T) => V, start: V): V {
        for (const next of this) {
            start = callback(start, next);
        }

        return start;
    }
}