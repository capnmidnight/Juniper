import { isDefined, isNullOrUndefined, makeLookup, once, success, twonce } from "@juniper-lib/util";
import { IDexDatabase } from "./IDexDatabase";
/**
 * The entry point for getting access to IndexedDB with Promises.
 */
export class IDex {
    #name;
    get name() { return this.#name; }
    #db;
    get isOpen() { return isDefined(this.#db); }
    constructor(name) {
        this.#name = name;
    }
    #setDB(db) {
        this.#db = new IDexDatabase(db);
        this.#db.addEventListener("disposed", () => {
            this.#db = null;
        });
        return this.#db;
    }
    async delete() {
        if (this.isOpen) {
            throw new Error("Cannot delete an open database");
        }
        const deleteRequest = indexedDB.deleteDatabase(this.#name);
        const task = once(deleteRequest, "success", "error", "blocked");
        return await success(task);
    }
    async open() {
        if (this.isOpen) {
            throw new Error("Database is already open");
        }
        const db = await this.#openExisting();
        return this.#setDB(db);
    }
    async #openExisting() {
        const openRequest = indexedDB.open(this.#name);
        // If we get an upgrade needed event, that means the database didn't already exist.
        const evt = await twonce(openRequest, "success", "upgradeneeded", "error", "blocked");
        if (evt.type === "upgradeneeded") {
            openRequest.result.close();
            await this.delete();
            throw new Error(`Database named '${this.#name}' does not exist.`);
        }
        return openRequest.result;
    }
    async assert(...storeDefs) {
        return await this.#modify("full", ...storeDefs);
    }
    async deleteStores(...storeNames) {
        if (this.isOpen) {
            throw new Error("Cannot delete stores from an open database");
        }
        return await this.#modify("remove", ...storeNames.map(name => {
            return { name };
        }));
    }
    async addStores(...storeDefs) {
        if (this.isOpen) {
            throw new Error("Cannot add stores to an open database");
        }
        return await this.#modify("add", ...storeDefs);
    }
    async changeStores(...storeDefs) {
        const storeNames = storeDefs.map(v => v.name);
        const removed = await this.deleteStores(...storeNames);
        const added = await this.addStores(...storeDefs);
        for (const storeName of added.storeNamesToAdd) {
            removed.storeNamesToRemove.delete(storeName);
        }
        for (const storeName of removed.storeNamesToRemove) {
            added.storeNamesToRemove.add(storeName);
        }
        return added;
    }
    async #modify(mode, ...storeDefs) {
        if (this.isOpen) {
            throw new Error("Database is already open");
        }
        const diff = prepareDiff(storeDefs);
        const maybeUpgrade = indexedDB.open(this.#name);
        const evt = await twonce(maybeUpgrade, "upgradeneeded", "success", "error", "blocked");
        if (evt.type === "upgradeneeded") {
            if (mode === "remove") {
                // If we got the upgradeneeded event, that means the database didn't exist
                // before we tried to open it. So close it and clean up after ourselves,
                // because there's nothing to delete.
                maybeUpgrade.result.close();
                await this.delete();
            }
            else {
                const upgradeNeeded = await this.#inspect(null, diff, mode);
                if (upgradeNeeded) {
                    await usingAsync(maybeUpgrade.result, async (/** @type {IDBDatabase} */ db) => await this.#upgrade(db, maybeUpgrade.transaction, diff));
                }
            }
        }
        else {
            const version = maybeUpgrade.result.version;
            const upgradeNeeded = await usingAsync(maybeUpgrade.result, async (/** @type {IDBDatabase} */ db) => await this.#inspect(db, diff, mode));
            if (upgradeNeeded) {
                const upgrade = indexedDB.open(this.#name, version + 1);
                const evt = await twonce(upgrade, "upgradeneeded", "success", "error", "blocked");
                this.#assertIsUpgradeNeeded(evt, upgrade);
                await usingAsync(upgrade.result, async (/** @type {IDBDatabase} */ db) => await this.#upgrade(db, upgrade.transaction, diff));
            }
        }
        return diff;
    }
    #assertIsUpgradeNeeded(evt, upgrade) {
        if (evt.type === "success") {
            // I don't know why this would occur, but just in case, I'm capturing it so I can close the database before erroring out.
            upgrade.result.close();
            throw new Error("Unknown issue: Attempted to upgraded but opened instead");
        }
    }
    async #inspect(db, diff, mode) {
        const { storesByName, storeNamesToAdd, storeNamesToRemove, storeNamesToChange, indexesByNameByStoreName, indexNamesToAddByStoreName, indexNamesToRemoveByStoreName } = diff;
        if (isNullOrUndefined(db)) {
            if (mode !== "remove") {
                // no database, add all the stores and indexes
                for (const [storeName, storeDef] of storesByName) {
                    storeNamesToAdd.add(storeName);
                    if (isDefined(storeDef.indexes)) {
                        const indexNamesToAdd = indexNamesToAddByStoreName.get(storeName);
                        for (const indexDef of storeDef.indexes) {
                            indexNamesToAdd.add(indexDef.name);
                        }
                    }
                }
            }
        }
        else {
            const storeNamesToScrutinize = new Set();
            if (mode !== "add") {
                // find stores to remove completely
                for (const storeName of db.objectStoreNames) {
                    if ((mode === "remove") === storesByName.has(storeName)) {
                        storeNamesToRemove.add(storeName);
                    }
                }
            }
            if (mode !== "remove") {
                // find stores that need to be added or changed
                for (const storeName of storesByName.keys()) {
                    if (db.objectStoreNames.contains(storeName)) {
                        storeNamesToScrutinize.add(storeName);
                    }
                    else {
                        storeNamesToAdd.add(storeName);
                    }
                }
                // check the stores that might need to be changed
                if (storeNamesToScrutinize.size > 0) {
                    const transaction = db.transaction(storeNamesToScrutinize);
                    const transacting = once(transaction, "abort", "error");
                    try {
                        /** @type {Map<string, IDBObjectStore>} */
                        const stores = new Map();
                        // does the store itself need to be changed?
                        for (const storeName of storeNamesToScrutinize) {
                            const storeDef = storesByName.get(storeName);
                            const store = transaction.objectStore(storeName);
                            stores.set(storeName, store);
                            if (isDefined(storeDef.options)
                                && (store.keyPath !== storeDef.options.keyPath
                                    || store.autoIncrement !== !!storeDef.options.autoIncrement)) {
                                if (mode === "add") {
                                    console.info(store, storeDef);
                                    throw new Error(`Store '${storeName} differs in definition from existing version.`);
                                }
                                storeNamesToRemove.add(storeName);
                                storeNamesToAdd.add(storeName);
                            }
                        }
                        // check the indexes
                        for (const [storeName, indexesByName] of indexesByNameByStoreName) {
                            const indexNamesToAdd = indexNamesToAddByStoreName.get(storeName);
                            const indexNamesToRemove = indexNamesToRemoveByStoreName.get(storeName);
                            const store = stores.get(storeName);
                            // skip any stores that are being dropped
                            if (!storeNamesToAdd.has(storeName) && !storeNamesToRemove.has(storeName)) {
                                // find indexes that can be removed altogether
                                for (const indexName of store.indexNames) {
                                    if (!indexesByName.has(indexName)) {
                                        indexNamesToRemove.add(indexName);
                                        storeNamesToChange.add(storeName);
                                    }
                                }
                                // find indexes that need to be changed
                                for (const [indexName, indexDef] of indexesByName) {
                                    // check indexes that already exist
                                    if (store.indexNames.contains(indexName)) {
                                        const index = store.index(indexName);
                                        if (indexDef.keyPath !== index.keyPath
                                            || isDefined(indexDef.options)
                                                && (!!indexDef.options.multiEntry !== index.multiEntry
                                                    || !!indexDef.options.unique !== index.unique)) {
                                            indexNamesToRemove.add(indexName);
                                            indexNamesToAdd.add(indexName);
                                            storeNamesToChange.add(storeName);
                                        }
                                    }
                                    else {
                                        // otherwise, add new indexes to existing stores.
                                        indexNamesToAdd.add(indexName);
                                        storeNamesToChange.add(storeName);
                                    }
                                }
                            }
                        }
                    }
                    finally {
                        // don't have any changes to save.
                        transaction.abort();
                        await transacting;
                    }
                    // check the indexes
                    for (const [storeName, indexesByName] of indexesByNameByStoreName) {
                        const indexNamesToAdd = indexNamesToAddByStoreName.get(storeName);
                        // stores that are being added can just have their indexes dumped
                        if (storeNamesToAdd.has(storeName)) {
                            for (const indexName of indexesByName.keys()) {
                                indexNamesToAdd.add(indexName);
                            }
                        }
                    }
                }
            }
        }
        // cleanup some empty items.
        for (const storeName of storesByName.keys()) {
            if (indexNamesToAddByStoreName.get(storeName)?.size === 0) {
                indexNamesToAddByStoreName.delete(storeName);
            }
            if (indexNamesToRemoveByStoreName.get(storeName)?.size === 0) {
                indexNamesToRemoveByStoreName.delete(storeName);
            }
            if (indexesByNameByStoreName.get(storeName)?.size === 0) {
                indexesByNameByStoreName.delete(storeName);
            }
        }
        return storeNamesToAdd.size > 0
            || storeNamesToRemove.size > 0
            || storeNamesToChange.size > 0
            || indexNamesToAddByStoreName.size > 0
            || indexNamesToRemoveByStoreName.size > 0;
    }
    async #upgrade(db, transaction, diff) {
        const transacting = once(transaction, "complete", "error", "abort");
        const { storesByName, storeNamesToAdd, storeNamesToRemove, storeNamesToChange, indexesByNameByStoreName, indexNamesToAddByStoreName, indexNamesToRemoveByStoreName } = diff;
        for (const storeName of storeNamesToRemove) {
            db.deleteObjectStore(storeName);
        }
        const stores = new Map();
        for (const storeName of storeNamesToAdd) {
            const storeDef = storesByName.get(storeName);
            const store = db.createObjectStore(storeName, storeDef.options);
            stores.set(storeName, store);
        }
        for (const storeName of storeNamesToChange) {
            const store = transaction.objectStore(storeName);
            stores.set(storeName, store);
        }
        for (const [storeName, indexesByName] of indexesByNameByStoreName) {
            const store = stores.get(storeName);
            const indexNamesToAdd = indexNamesToAddByStoreName.get(storeName);
            const indexNamesToRemove = indexNamesToRemoveByStoreName.get(storeName);
            if (indexNamesToRemove) {
                for (const indexName of indexNamesToRemove) {
                    store.deleteIndex(indexName);
                }
            }
            if (indexNamesToAdd) {
                for (const indexName of indexNamesToAdd) {
                    const indexDef = indexesByName.get(indexName);
                    store.createIndex(indexName, indexDef.keyPath, indexDef.options);
                }
            }
        }
        await transacting;
    }
}
function prepareDiff(storeDefs) {
    const indexesByNameByStoreName = new Map();
    const storeNamesToAdd = new Set();
    const storeNamesToRemove = new Set();
    const storeNamesToChange = new Set();
    const indexNamesToAddByStoreName = new Map();
    const indexNamesToRemoveByStoreName = new Map();
    const storesByName = makeLookup(storeDefs, (v) => v.name);
    for (const storeDef of storeDefs) {
        if (storeDef.indexes) {
            const indexesByName = new Map();
            indexesByNameByStoreName.set(storeDef.name, indexesByName);
            indexNamesToAddByStoreName.set(storeDef.name, new Set());
            indexNamesToRemoveByStoreName.set(storeDef.name, new Set());
            if (storeDef.indexes) {
                for (const indexDef of storeDef.indexes) {
                    indexesByName.set(indexDef.name, indexDef);
                }
            }
        }
    }
    return {
        storesByName,
        storeNamesToAdd,
        storeNamesToRemove,
        storeNamesToChange,
        indexesByNameByStoreName,
        indexNamesToAddByStoreName,
        indexNamesToRemoveByStoreName
    };
}
async function usingAsync(db, thunk) {
    try {
        return await thunk(db);
    }
    finally {
        db.close();
    }
}
//# sourceMappingURL=IDex.js.map