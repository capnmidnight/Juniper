import { hasValue } from "@juniper-lib/util";

export class PrioritySet<K, V> {
    #items = new Map<K, Set<V>>();

    constructor(init?: Iterable<[K, V]>) {
        if (hasValue(init)) {
            for (const [key, value] of init) {
                this.add(key, value);
            }
        }
    }

    add(key: K, ...values: V[]) {
        let set = this.#items.get(key);
        if (!hasValue(set)) {
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

    has(key: K) {
        return this.#items.has(key);
    }

    get(key: K) {
        let set = this.#items.get(key);
        if (!hasValue(set)) {
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

    delete(key: K) {
        return this.#items.delete(key);
    }

    remove(key: K, value: V) {
        const set = this.#items.get(key);
        if (hasValue(set)) {
            set.delete(value);
            if (set.size === 0) {
                this.#items.delete(key);
            }
        }
    }

    clear() {
        this.#items.clear();
    }
}


