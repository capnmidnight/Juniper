import { isDefined, isNullOrUndefined } from "@juniper-lib/util";



export class PriorityMap<Key1T, Key2T, ValueT> {
    readonly #items = new Map<Key1T, Map<Key2T, ValueT>>();

    constructor(init?: Iterable<[Key1T, Key2T, ValueT]>) {
        if (isDefined(init)) {
            for (const [key1, key2, value] of init) {
                this.add(key1, key2, value);
            }
        }
    }

    add(key1: Key1T, key2: Key2T, value: ValueT): this {
        let level1 = this.#items.get(key1);
        if (isNullOrUndefined(level1)) {
            this.#items.set(key1, level1 = new Map());
        }

        level1.set(key2, value);

        return this;
    }

    [Symbol.iterator]() {
        return this.entries();
    }

    *entries(): IterableIterator<[Key1T, Key2T, ValueT]> {
        for (const [key1, level1] of this.#items) {
            for (const [key2, value] of level1) {
                yield [key1, key2, value];
            }
        }
    }
    keys(): IterableIterator<Key1T>;
    keys(key1: Key1T): IterableIterator<Key2T>;
    keys(key1?: Key1T): IterableIterator<Key1T | Key2T> {
        if (isNullOrUndefined(key1)) {
            return this.#items.keys();
        }
        else {
            return this.#items.get(key1).keys();
        }
    }

    *values(): IterableIterator<ValueT> {
        for (const level1 of this.#items.values()) {
            for (const value of level1.values()) {
                yield value;
            }
        }
    }

    has(key1: Key1T, key2?: Key2T): boolean {
        return this.#items.has(key1)
            && (isNullOrUndefined(key2)
                || this.#items.get(key1).has(key2));
    }

    get(key1: Key1T): Map<Key2T, ValueT>;
    get(key1: Key1T, key2: Key2T): ValueT;
    get(key1: Key1T, key2?: Key2T): ValueT | Map<Key2T, ValueT> {
        if (isNullOrUndefined(key2)) {
            return this.#items.get(key1);
        }
        else if (this.#items.has(key1)) {
            return this.#items.get(key1).get(key2);
        }
        else {
            return null;
        }
    }

    count(key1: Key1T): number {
        if (this.#items.has(key1)) {
            return this.#items.get(key1).size;
        }

        return null;
    }

    get size(): number {
        let size = 0;
        for (const list of this.#items.values()) {
            size += list.size;
        }
        return size;
    }

    delete(key1: Key1T, key2?: Key2T) {
        if (isNullOrUndefined(key2)) {
            return this.#items.delete(key1);
        }
        else if (this.#items.has(key1)) {
            const items = this.#items.get(key1);
            const deleted = items.delete(key2);
            if (items.size === 0) {
                this.#items.delete(key1);
            }

            return deleted;
        }
        else {
            return false;
        }
    }

    clear(): void {
        this.#items.clear();
    }
}
