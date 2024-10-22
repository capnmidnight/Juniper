import { eventHandler } from '@juniper-lib/util';
export declare class CombinedRequest<T> extends EventTarget implements IDBRequest<T[]> {
    #private;
    get source(): IDBObjectStore;
    get transaction(): IDBTransaction;
    get result(): T[];
    get error(): DOMException;
    get onsuccess(): eventHandler<Event>;
    set onsuccess(v: eventHandler<Event>);
    get onerror(): eventHandler<Event>;
    set onerror(v: eventHandler<Event>);
    get readyState(): "done" | "pending";
    constructor(source: IDBObjectStore, transaction: IDBTransaction, requests: IDBRequest<T>[]);
}
//# sourceMappingURL=CombinedRequest.d.ts.map