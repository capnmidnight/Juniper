import { arrayCompare } from "@juniper-lib/tslib/collections/arrays";
import { makeLookup } from "@juniper-lib/tslib/collections/makeLookup";
import { PriorityList } from "@juniper-lib/tslib/collections/PriorityList";
import { PriorityMap } from "@juniper-lib/tslib/collections/PriorityMap";
import { once, success } from "@juniper-lib/tslib/events/once";
import { Task } from "@juniper-lib/tslib/events/Task";
import { isArray, isDefined, isNullOrUndefined, isString } from "@juniper-lib/tslib/typeChecks";
import { IDisposable } from "@juniper-lib/tslib/using";

export interface IDexDBIndexDef<T = any> {
    name: string;
    keyPath: (keyof T) | (keyof T)[];
    options?: IDBIndexParameters;
}

export interface IDexDBOptionsDef<T = any> {
    autoIncrement?: boolean;
    keyPath?: (keyof T) | (keyof T)[];
}

interface StoreDef {
    name: string;
    options?: IDexDBOptionsDef;
    indexes?: IDexDBIndexDef[];
}

export class IDexDB implements IDisposable {

    static async getCurrentVersion(dbName: string) {
        if (isDefined(indexedDB.databases)) {
            const databases = await indexedDB.databases();
            for (const { name, version } of databases) {
                if (name === dbName) {
                    return version;
                }
            }
        }

        return null;
    }

    static delete(dbName: string) {
        const deleteRequest = indexedDB.deleteDatabase(dbName);
        const task = once<IDBOpenDBRequestEventMap>(deleteRequest, "success", "error", "blocked");
        return success(task);
    }

    static async open(name: string, ...storeDefs: StoreDef[]): Promise<IDexDB> {
        const storesByName = makeLookup(storeDefs, (v) => v.name);
        const indexesByName = new PriorityMap<string, string, IDexDBIndexDef>(
            storeDefs
                .filter((storeDef) => isDefined(storeDef.indexes))
                .flatMap<[string, string, IDexDBIndexDef]>((storeDef) =>
                    storeDef.indexes.map<[string, string, IDexDBIndexDef]>((indexDef) =>
                        [storeDef.name, indexDef.name, indexDef])));

        const storesToAdd = new Array<string>();
        const storesToRemove = new Array<string>();
        const storesToChange = new Array<string>();
        const indexesToAdd = new PriorityList<string, string>();
        const indexesToRemove = new PriorityList<string, string>();

        let version = await this.getCurrentVersion(name);

        if (isNullOrUndefined(version)) {
            storesToAdd.push(...storesByName.keys());
            for (const storeDef of storeDefs) {
                if (isDefined(storeDef.indexes)) {
                    for (const indexDef of storeDef.indexes) {
                        indexesToAdd.add(storeDef.name, indexDef.name);
                    }
                }
            }
        }
        else {
            const D = indexedDB.open(name);
            if (await success(once<IDBOpenDBRequestEventMap>(D, "success", "error", "blocked"))) {
                const db = D.result;

                const storesToScrutinize = new Array<string>();

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
                    const transacting = once<IDBTransactionEventMap>(transaction, "complete", "error", "abort");
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

                db.close();
            }

            if (storesToAdd.length > 0
                || storesToRemove.length > 0
                || indexesToAdd.size > 0
                || indexesToRemove.size > 0) {
                ++version;
            }
        }

        const upgrading = new Task<boolean>();
        const openRequest = isDefined(version)
            ? indexedDB.open(name, version)
            : indexedDB.open(name);
        const opening = once<IDBOpenDBRequestEventMap>(openRequest, "success", "error", "blocked");
        const upgraded = success(upgrading);
        const opened = success(opening);

        const noUpgrade = upgrading.resolver(false);
        openRequest.addEventListener("success", noUpgrade);

        openRequest.addEventListener("upgradeneeded", () => {
            const transacting = once<IDBTransactionEventMap>(openRequest.transaction, "complete", "error", "abort");
            const db = openRequest.result;
            for (const storeName of storesToRemove) {
                db.deleteObjectStore(storeName);
            }

            const stores = new Map<string, IDBObjectStore>();

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

    constructor(private readonly db: IDBDatabase) {

    }

    dispose() {
        this.db.close();
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

    getStore<T>(storeName: string): IDexStore<T> {
        return new IDexStore<T>(this.db, storeName);
    }
}

export class IDexStore<T> {
    constructor(private readonly db: IDBDatabase, private readonly storeName: string) {

    }

    private async request<T>(makeRequest: (store: IDBObjectStore) => IDBRequest<T>, mode: IDBTransactionMode): Promise<T> {
        const transaction = this.db.transaction(this.storeName, mode);
        const transacting = once<IDBTransactionEventMap>(transaction, "complete", "error");

        const store = transaction.objectStore(this.storeName);
        const request = makeRequest(store);
        const requesting = once<IDBRequestEventMap>(request, "success", "error");

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

    add<T>(value: T, key?: IDBValidKey): Promise<IDBValidKey> {
        return this.request((store) => store.add(value, key), "readwrite");
    }

    clear() {
        return this.request((store) => store.clear(), "readwrite");
    }

    getCount(query?: IDBValidKey | IDBKeyRange): Promise<number> {
        return this.request((store) => store.count(query), "readonly");
    }

    async has(query: IDBValidKey): Promise<boolean> {
        return (await this.getCount(query)) > 0;
    }

    delete(query: IDBValidKey | IDBKeyRange) {
        return this.request((store) => store.delete(query), "readwrite");
    }

    get(key: IDBValidKey): Promise<T> {
        return this.request((store) => store.get<T>(key), "readonly");
    }

    getAll(): Promise<T[]> {
        return this.request((store) => store.getAll<T>(), "readonly")
    }

    getAllKeys(): Promise<IDBValidKey[]> {
        return this.request((store) => store.getAllKeys(), "readonly")
    }

    getKey(query: IDBValidKey | IDBKeyRange): Promise<IDBValidKey | undefined> {
        return this.request((store) => store.getKey(query), "readonly");
    }

    openCursor(query?: IDBValidKey | IDBKeyRange | null, direction?: IDBCursorDirection): Promise<IDBCursorWithValue | null> {
        return this.request((store) => store.openCursor(query, direction), "readonly");
    }

    openKeyCursor(query?: IDBValidKey | IDBKeyRange | null, direction?: IDBCursorDirection): Promise<IDBCursor | null> {
        return this.request((store) => store.openKeyCursor(query, direction), "readonly");
    }

    put(value: T, key?: IDBValidKey): Promise<IDBValidKey> {
        return this.request((store) => store.put(value, key), "readwrite");
    }
}