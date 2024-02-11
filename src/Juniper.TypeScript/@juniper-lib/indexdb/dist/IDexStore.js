import { once, success } from "@juniper-lib/events/dist/once";
export class IDexStore {
    constructor(db, storeName) {
        this.db = db;
        this.storeName = storeName;
    }
    async request(makeRequest, mode) {
        const transaction = this.db.transaction(this.storeName, mode);
        const transacting = once(transaction, "complete", "error");
        const store = transaction.objectStore(this.storeName);
        const request = makeRequest(store);
        const requesting = once(request, "success", "error");
        if (!(await success(requesting))) {
            transaction.abort();
            throw requesting.error;
        }
        transaction.commit();
        if (!(await success(transacting))) {
            throw transacting.error;
        }
        return request.result;
    }
    add(value, key) {
        return this.request((store) => store.add(value, key), "readwrite");
    }
    clear() {
        return this.request((store) => store.clear(), "readwrite");
    }
    getCount(query) {
        return this.request((store) => store.count(query), "readonly");
    }
    async has(query) {
        return (await this.getCount(query)) > 0;
    }
    delete(query) {
        return this.request((store) => store.delete(query), "readwrite");
    }
    get(key) {
        return this.request((store) => store.get(key), "readonly");
    }
    getAll() {
        return this.request((store) => store.getAll(), "readonly");
    }
    getAllKeys() {
        return this.request((store) => store.getAllKeys(), "readonly");
    }
    getKey(query) {
        return this.request((store) => store.getKey(query), "readonly");
    }
    openCursor(query, direction) {
        return this.request((store) => store.openCursor(query, direction), "readonly");
    }
    openKeyCursor(query, direction) {
        return this.request((store) => store.openKeyCursor(query, direction), "readonly");
    }
    put(value, key) {
        return this.request((store) => store.put(value, key), "readwrite");
    }
}
//# sourceMappingURL=IDexStore.js.map