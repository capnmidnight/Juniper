import { IDisposable } from "@juniper-lib/util";
import { IDexStore } from "./IDexStore";
export declare class IDexDatabase extends EventTarget implements IDisposable {
    #private;
    constructor(db: IDBDatabase);
    dispose(): void;
    get name(): string;
    get version(): number;
    get storeNames(): string[];
    get isOpen(): boolean;
    getStore<T>(storeName: string): IDexStore<T>;
}
//# sourceMappingURL=IDexDatabase.d.ts.map