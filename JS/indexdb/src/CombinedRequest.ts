import { eventHandler, once } from '@juniper-lib/util';

function replaceEventHandler(evtTarget: EventTarget, evtName: string, oldValue: eventHandler<Event>, newValue: eventHandler<Event>) {
    evtTarget.removeEventListener(evtName, oldValue);
    evtTarget.addEventListener(evtName, newValue);
    return newValue;
}

export class CombinedRequest<T> extends EventTarget implements IDBRequest<T[]> {
    #source;
    get source() { return this.#source; }

    #transaction;
    get transaction() { return this.#transaction; }

    #result: T[];
    get result() { return this.#result; }

    #error: DOMException = null;
    get error() { return this.#error; }

    #onsuccess: eventHandler<Event> = null;
    get onsuccess() { return this.#onsuccess; }
    set onsuccess(v) { this.#onsuccess = replaceEventHandler(this, "success", this.#onsuccess, v); }

    #onerror: eventHandler<Event> = null;
    get onerror() { return this.#onerror; }
    set onerror(v) { this.#onerror = replaceEventHandler(this, "error", this.#onerror, v); }

    #requests;
    get readyState() {
        return this.#requests
            .filter(r => r.readyState === "pending")
            .length === 0 ? "done" : "pending";
    }

    constructor(source: IDBObjectStore, transaction: IDBTransaction, requests: IDBRequest<T>[]) {
        super();

        this.#source = source;
        this.#transaction = transaction;
        this.#requests = requests;

        Promise.all(requests.map(request =>
            once(request, "success", "error"))
        ).then(data => {
            this.#result = data.map(v => v.target.result);
            this.dispatchEvent(new Event("success"));
        }).catch(error => {
            this.#error = error.target.error;
            this.dispatchEvent(new ErrorEvent("error", { error }));
        });
    }
}
