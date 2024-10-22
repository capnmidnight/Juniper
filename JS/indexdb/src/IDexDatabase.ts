import { IDisposable, isDefined } from "@juniper-lib/util";
import { IDexStore } from "./IDexStore";


export class IDexDatabase extends EventTarget implements IDisposable {
    #db;
    #name;
    #version;
    #storeNames;

    constructor(db: IDBDatabase) {
        super();
        this.#db = db;
        this.#name = db.name;
        this.#version = db.version;
        this.#storeNames = Array.from(this.#db.objectStoreNames);
    }

    dispose() {
        this.#db.close();
        this.#db = null;
        this.dispatchEvent(new Event("disposed"));
    }

    get name() { return this.#name; }
    get version() { return this.#version; }
    get storeNames() { return this.#storeNames; }
    get isOpen() { return isDefined(this.#db); }


    getStore<T>(storeName: string) {
        if (!this.isOpen) {
            throw new Error("Cannot get a store from a closed database");
        }

        return new IDexStore<T>(this, this.#db, storeName);
    }
}
