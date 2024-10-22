import type { IDexStore } from "./IDexStore";
export declare class IDexIndex<V> {
    #private;
    constructor(objectStore: IDexStore<V>, index: IDBIndex);
    /** [MDN Reference](https://developer.mozilla.org/docs/Web/API/IDBIndex/keyPath) */
    get keyPath(): string | string[];
    /** [MDN Reference](https://developer.mozilla.org/docs/Web/API/IDBIndex/multiEntry) */
    get multiEntry(): boolean;
    /**
     * Returns the name of the index.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/IDBIndex/name)
     */
    get name(): string;
    /**
     * Returns the IDBObjectStore the index belongs to.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/IDBIndex/objectStore)
     */
    get objectStore(): IDexStore<V>;
    /** [MDN Reference](https://developer.mozilla.org/docs/Web/API/IDBIndex/unique) */
    get unique(): boolean;
    /**
     * Retrieves the number of records matching the given key or key range in query.
     *
     * If successful, request's result will be the count.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/IDBIndex/count)
     */
    count(query?: (IDBValidKey | IDBKeyRange)): Promise<number>;
    /**
     * Retrieves the value of the first record matching the given key or key range in query.
     *
     * If successful, request's result will be the value, or undefined if there was no matching record.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/IDBIndex/get)
     */
    get(query?: (IDBValidKey | IDBKeyRange)): Promise<V>;
    /**
     * Retrieves the values of the records matching the given key or key range in query (up to count if given).
     *
     * If successful, request's result will be an Array of the values.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/IDBIndex/getAll)
     */
    getAll(query?: (IDBValidKey | IDBKeyRange), count?: number): Promise<V[]>;
    /**
     * Retrieves the keys of records matching the given key or key range in query (up to count if given).
     *
     * If successful, request's result will be an Array of the keys.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/IDBIndex/getAllKeys)
     */
    getAllKeys(query?: (IDBValidKey | IDBKeyRange), count?: number): Promise<IDBValidKey[]>;
    /**
     * Retrieves the key of the first record matching the given key or key range in query.
     *
     * If successful, request's result will be the key, or undefined if there was no matching record.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/IDBIndex/getKey)
     */
    getKey(query?: (IDBValidKey | IDBKeyRange)): Promise<IDBValidKey>;
    /**
     * Opens a cursor over the records matching query, ordered by direction. If query is null, all records in index are matched.
     *
     * If successful, request's result will be an IDBCursorWithValue, or null if there were no matching records.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/IDBIndex/openCursor)
     */
    openCursor(query?: (IDBValidKey | IDBKeyRange), direction?: IDBCursorDirection): Promise<IDBCursorWithValue>;
    /**
     * Opens a cursor with key only flag set over the records matching query, ordered by direction. If query is null, all records in index are matched.
     *
     * If successful, request's result will be an IDBCursor, or null if there were no matching records.
     *
     * [MDN Reference](https://developer.mozilla.org/docs/Web/API/IDBIndex/openKeyCursor)
     * @param {IDBValidKey | IDBKeyRange | null} [query]
     * @param {IDBCursorDirection} [direction]
     */
    openKeyCursor(query?: (IDBValidKey | IDBKeyRange), direction?: IDBCursorDirection): Promise<IDBCursor>;
}
//# sourceMappingURL=IDexIndex.d.ts.map