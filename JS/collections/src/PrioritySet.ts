import { isDefined, isNullOrUndefined } from "@juniper-lib/util";

export class PrioritySet<K, V> {
    #items = new Map<K, Set<V>>();

    constructor(init?: Iterable<[K, V]>) {
        if (isDefined(init)) {
            for (const [key, value] of init) {
                this.add(key, value);
            }
        }
    }

    add(key: K, firstValue: V, ...values: V[]) {
        values.unshift(firstValue);

        let set = this.#items.get(key);
        if (isNullOrUndefined(set)) {
            this.#items.set(key, set = new Set());
        }

        for (const value of values) {
            set.add(value);
        }

        return this;
    }

    entries() {
        return this.#items.entries();
    }

    [Symbol.iterator]() {
        return this.entries();
    }

    keys() {
        return this.#items.keys();
    }

    *values() {
        for (const set of this.#items.values()) {
            for (const item of set) {
                yield item;
            }
        }
    }

    has(key: K, value?: V) {
        return this.#items.has(key)
            && isNullOrUndefined(value)
            || this.#items.get(key).has(value);
    }

    get(key: K) {
        let set = this.#items.get(key);
        if (isNullOrUndefined(set)) {
            this.#items.set(key, set = new Set());
        }
        return set;
    }

    count(key: K) {
        return this.get(key).size;
    }

    get size() {
        let size = 0;
        for (const set of this.#items.values()) {
            size += set.size;
        }
        return size;
    }

    delete(key: K, value?: V) {
        let removed = isDefined(value)
            && this.#items.get(key)?.delete(value)
            || this.#items.delete(key);

        if (removed
            && this.#items.has(key)
            && this.#items.get(key).size === 0) {
            this.#items.delete(key);
        }

        return removed;
    }

    clear() {
        this.#items.clear();
    }
}


