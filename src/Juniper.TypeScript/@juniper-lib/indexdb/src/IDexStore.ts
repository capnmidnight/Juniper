import { once, success } from "@juniper-lib/events/dist/once";


export class IDexStore<T> {
    constructor(private readonly db: IDBDatabase, private readonly storeName: string) {
    }

    private async request<T>(makeRequest: (store: IDBObjectStore) => IDBRequest<T>, mode: IDBTransactionMode): Promise<T> {
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
        return this.request((store) => store.getAll<T>(), "readonly");
    }

    getAllKeys(): Promise<IDBValidKey[]> {
        return this.request((store) => store.getAllKeys(), "readonly");
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
