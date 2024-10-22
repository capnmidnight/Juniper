import { isDefined } from "@juniper-lib/util";
import { IDexStore } from "./IDexStore";
export class IDexDatabase extends EventTarget {
    #db;
    #name;
    #version;
    #storeNames;
    constructor(db) {
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
    getStore(storeName) {
        if (!this.isOpen) {
            throw new Error("Cannot get a store from a closed database");
        }
        return new IDexStore(this, this.#db, storeName);
    }
}
//# sourceMappingURL=IDexDatabase.js.map