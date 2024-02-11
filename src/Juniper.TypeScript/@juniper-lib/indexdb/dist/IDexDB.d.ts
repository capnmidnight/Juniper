import { IDisposable } from "@juniper-lib/tslib/dist/using";
import { StoreDef } from "./interfaces";
import { IDexStore } from "./IDexStore";
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
//# sourceMappingURL=IDexDB.d.ts.map