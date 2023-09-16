import { IDisposable } from "@juniper-lib/tslib/dist/using";
export interface IDexDBIndexDef<T = any> {
    name: string;
    keyPath: (keyof T & string) | (keyof T & string)[];
    options?: IDBIndexParameters;
}
export interface IDexDBOptionsDef<T = any> {
    autoIncrement?: boolean;
    keyPath?: (keyof T & string) | (keyof T & string)[];
}
interface StoreDef {
    name: string;
    options?: IDexDBOptionsDef;
    indexes?: IDexDBIndexDef[];
}
export declare class IDexDB implements IDisposable {
    private readonly db;
    static delete(dbName: string): Promise<boolean>;
    static open(name: string, ...storeDefs: StoreDef[]): Promise<IDexDB>;
    constructor(db: IDBDatabase);
    dispose(): void;
    get name(): string;
    get version(): number;
    get storeNames(): string[];
    getStore<T>(storeName: string): IDexStore<T>;
}
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
export {};
//# sourceMappingURL=index.d.ts.map