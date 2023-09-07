import { isDefined, isNullOrUndefined } from "@juniper-lib/tslib/typeChecks";
export class PriorityMap {
    constructor(init) {
        this.items = new Map();
        if (isDefined(init)) {
            for (const [key1, key2, value] of init) {
                this.add(key1, key2, value);
            }
        }
    }
    add(key1, key2, value) {
        let level1 = this.items.get(key1);
        if (isNullOrUndefined(level1)) {
            this.items.set(key1, level1 = new Map());
        }
        level1.set(key2, value);
        return this;
    }
    *entries() {
        for (const [key1, level1] of this.items) {
            for (const [key2, value] of level1) {
                yield [key1, key2, value];
            }
        }
    }
    keys(key1) {
        if (isNullOrUndefined(key1)) {
            return this.items.keys();
        }
        else {
            return this.items.get(key1).keys();
        }
    }
    *values() {
        for (const level1 of this.items.values()) {
            for (const value of level1.values()) {
                yield value;
            }
        }
    }
    has(key1, key2) {
        return this.items.has(key1)
            && (isNullOrUndefined(key2)
                || this.items.get(key1).has(key2));
    }
    get(key1, key2) {
        if (isNullOrUndefined(key2)) {
            return this.items.get(key1);
        }
        else if (this.items.has(key1)) {
            return this.items.get(key1).get(key2);
        }
        else {
            return null;
        }
    }
    count(key1) {
        if (this.items.has(key1)) {
            return this.items.get(key1).size;
        }
        return null;
    }
    get size() {
        let size = 0;
        for (const list of this.items.values()) {
            size += list.size;
        }
        return size;
    }
    delete(key1, key2) {
        if (isNullOrUndefined(key2)) {
            return this.items.delete(key1);
        }
        else if (this.items.has(key1)) {
            const items = this.items.get(key1);
            const deleted = items.delete(key2);
            if (items.size === 0) {
                this.items.delete(key1);
            }
            return deleted;
        }
        else {
            return false;
        }
    }
    clear() {
        this.items.clear();
    }
}
//# sourceMappingURL=PriorityMap.js.map