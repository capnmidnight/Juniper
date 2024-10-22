import { once } from "@juniper-lib/util";
import type { IDexStore } from "./IDexStore";

export class IDexIndex<V> {
    #objectStore;
    #index;

    constructor(objectStore: IDexStore<V>, index: IDBIndex) {
        this.#objectStore = objectStore;
        this.#index = index;
    }

    /** [MDN Reference](https://developer.mozilla.org/docs/Web/API/IDBIndex/keyPath) */
    get keyPath() { return this.#index.keyPath; }

    /** [MDN Reference](https://developer.mozilla.org/docs/Web/API/IDBIndex/multiEntry) */
    get multiEntry() { return this.#index.multiEntry; }

    /**
     * Returns the name of the index.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/IDBIndex/name)
     */
    get name() { return this.#index.name; }

    /**
     * Returns the IDBObjectStore the index belongs to.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/IDBIndex/objectStore)
     */
    get objectStore() { return this.#objectStore; }


    /** [MDN Reference](https://developer.mozilla.org/docs/Web/API/IDBIndex/unique) */
    get unique() { return this.#index.unique; }


    async #request<T>(makeRequest: () => IDBRequest<T>) {
        const request = makeRequest();
        await once(request, "success", "error");
        return request.result;
    }

    /**
     * Retrieves the number of records matching the given key or key range in query.
     *
     * If successful, request's result will be the count.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/IDBIndex/count)
     */
    async count(query?: (IDBValidKey | IDBKeyRange)) {
        return await this.#request(() => this.#index.count(query));
    }

    /**
     * Retrieves the value of the first record matching the given key or key range in query.
     *
     * If successful, request's result will be the value, or undefined if there was no matching record.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/IDBIndex/get)
     */
    async get(query?: (IDBValidKey | IDBKeyRange)): Promise<V> {
        return await this.#request<V>(() => this.#index.get(query));
    }

    /**
     * Retrieves the values of the records matching the given key or key range in query (up to count if given).
     *
     * If successful, request's result will be an Array of the values.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/IDBIndex/getAll)
     */
    async getAll(query?: (IDBValidKey | IDBKeyRange), count?: number): Promise<V[]> {
        return await this.#request<V[]>(() => this.#index.getAll(query, count));
    }

    /**
     * Retrieves the keys of records matching the given key or key range in query (up to count if given).
     *
     * If successful, request's result will be an Array of the keys.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/IDBIndex/getAllKeys)
     */
    async getAllKeys(query?: (IDBValidKey | IDBKeyRange), count?: number) {
        return await this.#request(() => this.#index.getAllKeys(query, count));
    }

    /**
     * Retrieves the key of the first record matching the given key or key range in query.
     *
     * If successful, request's result will be the key, or undefined if there was no matching record.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/IDBIndex/getKey)
     */
    async getKey(query?: (IDBValidKey | IDBKeyRange)) {
        return await this.#request(() => this.#index.getKey(query));
    }

    /**
     * Opens a cursor over the records matching query, ordered by direction. If query is null, all records in index are matched.
     *
     * If successful, request's result will be an IDBCursorWithValue, or null if there were no matching records.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/IDBIndex/openCursor)
     */
    async openCursor(query?: (IDBValidKey | IDBKeyRange), direction?: IDBCursorDirection) {
        return await this.#request(() => this.#index.openCursor(query, direction));
    }

    /**
     * Opens a cursor with key only flag set over the records matching query, ordered by direction. If query is null, all records in index are matched.
     *
     * If successful, request's result will be an IDBCursor, or null if there were no matching records.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/IDBIndex/openKeyCursor)
     * @param {IDBValidKey | IDBKeyRange | null} [query]
     * @param {IDBCursorDirection} [direction]
     */
    async openKeyCursor(query?: (IDBValidKey | IDBKeyRange), direction?: IDBCursorDirection) {
        return await this.#request(() => this.#index.openKeyCursor(query, direction));
    }
}
