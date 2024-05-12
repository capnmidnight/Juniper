import { hasValue, identity, toString } from "@juniper-lib/util";

export class HistoryStateChangedEvent extends Event {
    constructor(state: any) {
        super("statechanged");
        this.#state = state;
    }

    #state;
    get state() { return this.#state; }
}

export class HistoryManager extends EventTarget {

    #curState: any;
    get state() { return this.#curState; }

    #qp;
    #qpDeserializer;
    #qpSerializer;
    #here;
    #paused = false;

    constructor(queryStringParamName?: string,
        queryStringStateParser?: (value: string) => any,
        queryStringStateSerializer?: (value: any) => string) {
        super();

        this.#qp = queryStringParamName;
        this.#qpDeserializer = queryStringStateParser || identity;
        this.#qpSerializer = queryStringStateSerializer || toString;
        this.#here = new URL(location.href);

        globalThis.addEventListener("popstate", (evt) => this.#check(evt));
    }

    #check(evt?: PopStateEvent) {
        this.#here = new URL(location.href);

        this.#curState = evt && evt.state
            || !evt && this.#qp && this.#qpDeserializer(this.#here.searchParams.get(this.#qp))
            || null;

        if (!evt && this.#curState) {
            history.replaceState(this.#curState, null, this.#here);
        }

        this.dispatchEvent(new HistoryStateChangedEvent(this.#curState));

    }

    pause() {
        this.#paused = true;
    }

    resume() {
        this.#paused = false;
    }

    start() {
        this.#check();
    }

    push(state: any) {
        if (!this.#paused) {
            if (this.#qp) {
                const str = this.#qpSerializer(state);
                if (hasValue(str)) {
                    this.#here.searchParams.set(this.#qp, str);
                }
                else {
                    this.#here.searchParams.delete(this.#qp);
                }
            }
            history.pushState(state, null, this.#here);
        }
    }
}