import { isDefined, isNullOrUndefined } from "@juniper-lib/util";
export class PrioritySet {
    #items = new Map();
    constructor(init) {
        if (isDefined(init)) {
            for (const [key, value] of init) {
                this.add(key, value);
            }
        }
    }
    add(key, firstValue, ...values) {
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
    has(key, value) {
        return this.#items.has(key)
            && isNullOrUndefined(value)
            || this.#items.get(key).has(value);
    }
    get(key) {
        let set = this.#items.get(key);
        if (isNullOrUndefined(set)) {
            this.#items.set(key, set = new Set());
        }
        return set;
    }
    count(key) {
        return this.get(key).size;
    }
    get size() {
        let size = 0;
        for (const set of this.#items.values()) {
            size += set.size;
        }
        return size;
    }
    delete(key, value) {
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
//# sourceMappingURL=PrioritySet.js.map