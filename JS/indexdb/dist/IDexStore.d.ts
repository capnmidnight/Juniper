export declare class IDexStore<T> {
    private readonly db;
    private readonly storeName;
    constructor(db: IDBDatabase, storeName: string);
    private request;
    add<T>(value: T, key?: IDBValidKey): Promise<IDBValidKey>;
    clear(): Promise<undefined>;
    getCount(query?: IDBValidKey | IDBKeyRange): Promise<number>;
    has(query: IDBValidKey): Promise<boolean>;
    delete(query: IDBValidKey | IDBKeyRange): Promise<undefined>;
    get(key: IDBValidKey): Promise<T>;
    getAll(): Promise<T[]>;
    getAllKeys(): Promise<IDBValidKey[]>;
    getKey(query: IDBValidKey | IDBKeyRange): Promise<IDBValidKey | undefined>;
    openCursor(query?: IDBValidKey | IDBKeyRange | null, direction?: IDBCursorDirection): Promise<IDBCursorWithValue | null>;
    openKeyCursor(query?: IDBValidKey | IDBKeyRange | null, direction?: IDBCursorDirection): Promise<IDBCursor | null>;
    put(value: T, key?: IDBValidKey): Promise<IDBValidKey>;
}
//# sourceMappingURL=IDexStore.d.ts.map