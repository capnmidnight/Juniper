import { hasMethod, isNullOrUndefined, once } from "@juniper-lib/util";
import { CombinedRequest } from "./CombinedRequest";
import type { IDexDatabase } from "./IDexDatabase";
import { IDexIndex } from "./IDexIndex";


export function prepareObject(value: any) {
    if (value) {
        if (hasMethod(value, "toIndexDB")) {
            value = value.toIndexDB();
        }
        else if (hasMethod(value, "toJSON")) {
            value = value.toJSON();
        }
    }
    return value;
}

export interface IDexDBIndexDef<T = any> {
    name: string;
    keyPath: (keyof T & string) | (keyof T & string)[];
    options?: IDBIndexParameters;
}

export interface IDexDBOptionsDef<T = any> {
    autoIncrement?: boolean;
    keyPath?: (keyof T & string) | (keyof T & string)[];
}

export interface StoreDef {
    name: string;
    options?: IDexDBOptionsDef;
    indexes?: IDexDBIndexDef[];
}


export class IDexStore<V> {
    #parent;
    #db;
    #storeName;

    /**
     * @param {import("./IDexDatabase").IDexDatabase} parent;
     * @param {IDBDatabase} db
     * @param {string} storeName
     */
    constructor(parent: IDexDatabase, db: IDBDatabase, storeName: string) {
        this.#parent = parent;
        this.#db = db;
        this.#storeName = storeName;
    }

    async getIndexNames() {
        if (!this.#parent.isOpen) {
            throw new Error("Cannot get index names from a closed database.");
        }

        return await this.#transaction("readonly", (store) => store.indexNames);
    }

    async #transaction<T>(mode: IDBTransactionMode, makeAction: (store: IDBObjectStore, transaction?: IDBTransaction) => (T | Promise<T>)) {
        if (!this.#parent.isOpen) {
            throw new Error("Cannot open a transaction from a closed database.");
        }

        const transaction = this.#db.transaction(this.#storeName, mode);
        const store = transaction.objectStore(this.#storeName);

        try {
            const value = await makeAction(store, transaction);
            transaction.commit();
            await once(transaction, "complete", "error");
            return await value;
        }
        catch (err) {
            transaction.abort();
            throw err;
        }
    }

    async #request<T>(mode: IDBTransactionMode, makeRequest: (store: IDBObjectStore, transaction?: IDBTransaction) => IDBRequest<T>) {
        return await this.#transaction(mode, async (store, transaction) => {
            const request = makeRequest(store, transaction);
            await once(request, "success", "error");
            return request.result;
        });
    }

    /**
     * Adds or updates a record in store with the given value and key.
     *
     * If the store uses in-line keys and key is specified a "DataError" DOMException will be thrown.
     *
     * If put() is used, any existing record with the key will be replaced. If add() is used, and if a record with the key already exists the request will fail, with request's error set to a "ConstraintError" DOMException.
     *
     * If successful, request's result will be the record's key.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/IDBObjectStore/add)
     */
    add(value: V, key?: IDBValidKey) {
        return this.#request("readwrite", (store) => store.add(prepareObject(value), key));
    }

    /**
     * Adds or updates a range of records in store with the given value and key.
     *
     * If the store uses in-line keys and key is specified a "DataError" DOMException will be thrown.
     *
     * If put() is used, any existing record with the key will be replaced. If add() is used, and if a record with the key already exists the request will fail, with request's error set to a "ConstraintError" DOMException.
     *
     * If successful, request's result will be the record's key.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/IDBObjectStore/add)
     */
    addAll(values: V[], getKeyForValue?: (v: V) => IDBValidKey) {
        if (isNullOrUndefined(getKeyForValue)) {
            getKeyForValue = () => undefined;
        }

        return this.#request("readwrite", (store, transaction) =>
            new CombinedRequest<IDBValidKey>(store, transaction, values.map(value =>
                store.add(prepareObject(value), getKeyForValue(value)))));
    }

    /**
     * Deletes all records in store.
     *
     * If successful, request's result will be undefined.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/IDBObjectStore/clear)
     */
    clear() {
        return this.#request("readwrite", (store) => store.clear());
    }

    /**
     * Retrieves the number of records matching the given key or key range in query.
     *
     * If successful, request's result will be the count.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/IDBObjectStore/count)
     */
    count(query?: (IDBValidKey | IDBKeyRange)) {
        return this.#request("readonly", (store) => store.count(query));
    }

    /**
     * Creates a new index in store with the given name, keyPath and options and returns a new IDBIndex. If the keyPath and options define constraints that cannot be satisfied with the data already in store the upgrade transaction will abort with a "ConstraintError" DOMException.
     *
     * Throws an "InvalidStateError" DOMException if not called within an upgrade transaction.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/IDBObjectStore/createIndex)
     */
    createIndex(name: string, keyPath: string | string[], options?: IDBIndexParameters) {
        return this.#transaction("readonly", (store) => store.createIndex(name, keyPath, options));
    }

    async has(query: IDBValidKey) {
        return (await this.count(query)) > 0;
    }

    /**
     * Deletes records in store with the given key or in the given key range in query.
     *
     * If successful, request's result will be undefined.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/IDBObjectStore/delete)
     */
    delete(query: (IDBValidKey | IDBKeyRange)) {
        return this.#request("readwrite", (store) => store.delete(query));
    }

    /**
     * Retrieves the value of the first record matching the given key or key range in query.
     *
     * If successful, request's result will be the value, or undefined if there was no matching record.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/IDBObjectStore/get)
     */
    get(key: IDBValidKey) {
        return this.#request<V>("readonly", (store) => store.get(key));
    }

    /**
     * Retrieves the values of the records matching the given key or key range in query (up to count if given).
     *
     * If successful, request's result will be an Array of the values.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/IDBObjectStore/getAll)
     */
    getAll() {
        return this.#request<V[]>("readonly", (store) => store.getAll());
    }

    /**
     * Retrieves the keys of records matching the given key or key range in query (up to count if given).
     *
     * If successful, request's result will be an Array of the keys.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/IDBObjectStore/getAllKeys)
     */
    getAllKeys() {
        return this.#request("readonly", (store) => store.getAllKeys());
    }

    /**
     * Retrieves the key of the first record matching the given key or key range in query.
     *
     * If successful, request's result will be the key, or undefined if there was no matching record.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/IDBObjectStore/getKey)
     */
    getKey(query: (IDBValidKey | IDBKeyRange)) {
        return this.#request("readonly", (store) => store.getKey(query));
    }

    /**
     * The index() method of the IDBObjectStore interface opens a named index in the current object store, after which it can be used to, for example, return a series of records sorted by that index using a cursor.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/IDBObjectStore/index)
     */
    withIndex<T>(name: string, act: (index: IDexIndex<V>, transaction?: IDBTransaction) => Promise<T>): Promise<T> {
        return this.#transaction("readonly", async (store, transaction) => await act(new IDexIndex(this, store.index(name)), transaction));
    }

    /**
     * Opens a cursor over the records matching query, ordered by direction. If query is null, all records in store are matched.
     *
     * If successful, request's result will be an IDBCursorWithValue pointing at the first matching record, or null if there were no matching records.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/IDBObjectStore/openCursor)
     */
    openCursor(query?: (IDBValidKey | IDBKeyRange), direction?: IDBCursorDirection) {
        return this.#request("readonly", (store) => store.openCursor(query, direction));
    }

    /**
     * Opens a cursor with key only flag set over the records matching query, ordered by direction. If query is null, all records in store are matched.
     *
     * If successful, request's result will be an IDBCursor pointing at the first matching record, or null if there were no matching records.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/IDBObjectStore/openKeyCursor)
     */
    openKeyCursor(query?: IDBValidKey | IDBKeyRange, direction?: IDBCursorDirection) {
        return this.#request("readonly", (store) => store.openKeyCursor(query, direction));
    }

    /**
     * Adds or updates a record in store with the given value and key.
     *
     * If the store uses in-line keys and key is specified a "DataError" DOMException will be thrown.
     *
     * If put() is used, any existing record with the key will be replaced. If add() is used, and if a record with the key already exists the request will fail, with request's error set to a "ConstraintError" DOMException.
     *
     * If successful, request's result will be the record's key.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/IDBObjectStore/put)
     */
    put<T>(value: T, key?: IDBValidKey) {
        return this.#request("readwrite", (store) => store.put(prepareObject(value), key));
    }
}
