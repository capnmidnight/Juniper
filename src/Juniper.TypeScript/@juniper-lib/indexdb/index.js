import { arrayCompare } from "@juniper-lib/collections/arrays";
import { makeLookup } from "@juniper-lib/collections/makeLookup";
import { PriorityList } from "@juniper-lib/collections/PriorityList";
import { PriorityMap } from "@juniper-lib/collections/PriorityMap";
import { once, success } from "@juniper-lib/events/once";
import { Task } from "@juniper-lib/events/Task";
import { isArray, isDefined, isString } from "@juniper-lib/tslib/typeChecks";
import { dispose } from "@juniper-lib/tslib/using";
export class IDexDB {
    static delete(dbName) {
        const deleteRequest = indexedDB.deleteDatabase(dbName);
        const task = once(deleteRequest, "success", "error", "blocked");
        return success(task);
    }
    static async open(name, ...storeDefs) {
        const storesByName = makeLookup(storeDefs, (v) => v.name);
        const indexesByName = new PriorityMap(storeDefs
            .filter((storeDef) => isDefined(storeDef.indexes))
            .flatMap((storeDef) => storeDef.indexes.map((indexDef) => [storeDef.name, indexDef.name, indexDef])));
        const storesToAdd = new Array();
        const storesToRemove = new Array();
        const storesToChange = new Array();
        const indexesToAdd = new PriorityList();
        const indexesToRemove = new PriorityList();
        let version = null;
        const D = indexedDB.open(name);
        if (await success(once(D, "success", "error", "blocked"))) {
            const db = D.result;
            version = db.version;
            const storesToScrutinize = new Array();
            for (const storeName of db.objectStoreNames) {
                if (!storesByName.has(storeName)) {
                    storesToRemove.push(storeName);
                }
            }
            for (const storeName of storesByName.keys()) {
                if (!db.objectStoreNames.contains(storeName)) {
                    storesToAdd.push(storeName);
                }
                else {
                    storesToScrutinize.push(storeName);
                }
            }
            if (storesToScrutinize.length > 0) {
                const transaction = db.transaction(storesToScrutinize);
                const transacting = once(transaction, "complete", "error", "abort");
                const transacted = success(transacting);
                for (const storeName of storesToScrutinize) {
                    const store = transaction.objectStore(storeName);
                    const storeDef = storesByName.get(storeName);
                    if (isDefined(storeDef.options) && store.keyPath !== storeDef.options.keyPath) {
                        storesToRemove.push(storeName);
                        storesToAdd.push(storeName);
                    }
                    for (const indexName of store.indexNames) {
                        if (!indexesByName.has(storeName, indexName)) {
                            if (storesToChange.indexOf(storeName) === -1) {
                                storesToChange.push(storeName);
                            }
                            indexesToRemove.add(storeName, indexName);
                        }
                    }
                    if (indexesByName.has(storeName)) {
                        for (const indexName of indexesByName.get(storeName).keys()) {
                            if (!store.indexNames.contains(indexName)) {
                                if (storesToChange.indexOf(storeName) === -1) {
                                    storesToChange.push(storeName);
                                }
                                indexesToAdd.add(storeName, indexName);
                            }
                            else {
                                const indexDef = indexesByName.get(storeName, indexName);
                                const index = store.index(indexName);
                                if (isString(indexDef.keyPath) !== isString(index.keyPath)
                                    || isString(indexDef.keyPath) && isString(index.keyPath) && indexDef.keyPath !== index.keyPath
                                    || isArray(indexDef.keyPath) && isArray(index.keyPath) && arrayCompare(indexDef.keyPath, index.keyPath)) {
                                    if (storesToChange.indexOf(storeName) === -1) {
                                        storesToChange.push(storeName);
                                    }
                                    indexesToRemove.add(storeName, indexName);
                                    indexesToAdd.add(storeName, indexName);
                                }
                            }
                        }
                    }
                }
                transaction.commit();
                await transacted;
            }
            dispose(db);
        }
        else {
            version = 0;
            storesToAdd.push(...storesByName.keys());
            for (const storeDef of storeDefs) {
                if (isDefined(storeDef.indexes)) {
                    for (const indexDef of storeDef.indexes) {
                        indexesToAdd.add(storeDef.name, indexDef.name);
                    }
                }
            }
        }
        if (storesToAdd.length > 0
            || storesToRemove.length > 0
            || indexesToAdd.size > 0
            || indexesToRemove.size > 0) {
            ++version;
        }
        const upgrading = new Task();
        const openRequest = isDefined(version)
            ? indexedDB.open(name, version)
            : indexedDB.open(name);
        const opening = once(openRequest, "success", "error", "blocked");
        const upgraded = success(upgrading);
        const opened = success(opening);
        const noUpgrade = upgrading.resolver(false);
        openRequest.addEventListener("success", noUpgrade);
        openRequest.addEventListener("upgradeneeded", () => {
            const transacting = once(openRequest.transaction, "complete", "error", "abort");
            const db = openRequest.result;
            for (const storeName of storesToRemove) {
                db.deleteObjectStore(storeName);
            }
            const stores = new Map();
            for (const storeName of storesToAdd) {
                const storeDef = storesByName.get(storeName);
                const store = db.createObjectStore(storeName, storeDef.options);
                stores.set(storeName, store);
            }
            for (const storeName of storesToChange) {
                const store = openRequest.transaction.objectStore(storeName);
                stores.set(storeName, store);
            }
            for (const [storeName, store] of stores) {
                for (const indexName of indexesToRemove.get(storeName)) {
                    store.deleteIndex(indexName);
                }
                for (const indexName of indexesToAdd.get(storeName)) {
                    const indexDef = indexesByName.get(storeName, indexName);
                    store.createIndex(indexName, indexDef.keyPath, indexDef.options);
                }
            }
            success(transacting)
                .then(upgrading.resolve)
                .catch(upgrading.reject)
                .finally(() => openRequest.removeEventListener("success", noUpgrade));
        });
        if (!(await upgraded)) {
            throw upgrading.error;
        }
        if (!(await opened)) {
            throw opening.error;
        }
        return new IDexDB(openRequest.result);
    }
    constructor(db) {
        this.db = db;
    }
    dispose() {
        dispose(this.db);
    }
    get name() {
        return this.db.name;
    }
    get version() {
        return this.db.version;
    }
    get storeNames() {
        return Array.from(this.db.objectStoreNames);
    }
    getStore(storeName) {
        return new IDexStore(this.db, storeName);
    }
}
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
//# sourceMappingURL=index.js.map