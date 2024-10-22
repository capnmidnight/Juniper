import { arrayClear, arrayRemove, isDefined, isNullOrUndefined } from "@juniper-lib/util";
export class PriorityList {
    #items = new Map();
    #defaultItems = new Array();
    constructor(init) {
        if (isDefined(init)) {
            for (const [key, value] of init) {
                this.add(key, value);
            }
        }
    }
    add(key, ...values) {
        for (const value of values) {
            if (isNullOrUndefined(key)) {
                this.#defaultItems.push(value);
            }
            else {
                let list = this.#items.get(key);
                if (isNullOrUndefined(list)) {
                    this.#items.set(key, list = []);
                }
                list.push(value);
            }
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
        for (const item of this.#defaultItems) {
            yield item;
        }
        for (const list of this.#items.values()) {
            for (const item of list) {
                yield item;
            }
        }
    }
    has(key) {
        if (isDefined(key)) {
            return this.#items.has(key);
        }
        else {
            return this.#defaultItems.length > 0;
        }
    }
    get(key) {
        if (isNullOrUndefined(key)) {
            return this.#defaultItems;
        }
        return this.#items.get(key) || [];
    }
    count(key) {
        if (isNullOrUndefined(key)) {
            return this.#defaultItems.length;
        }
        const list = this.get(key);
        if (isDefined(list)) {
            return list.length;
        }
        return 0;
    }
    get size() {
        let size = this.#defaultItems.length;
        for (const list of this.#items.values()) {
            size += list.length;
        }
        return size;
    }
    delete(key) {
        if (isNullOrUndefined(key)) {
            return arrayClear(this.#defaultItems).length > 0;
        }
        else {
            return this.#items.delete(key);
        }
    }
    remove(key, value) {
        if (isNullOrUndefined(key)) {
            arrayRemove(this.#defaultItems, value);
        }
        else {
            const list = this.#items.get(key);
            if (isDefined(list)) {
                arrayRemove(list, value);
                if (list.length === 0) {
                    this.#items.delete(key);
                }
            }
        }
    }
    clear() {
        this.#items.clear();
        arrayClear(this.#defaultItems);
    }
}
//# sourceMappingURL=PriorityList.js.map