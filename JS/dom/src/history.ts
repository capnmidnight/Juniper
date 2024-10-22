import { Exception, isDefined, isNullOrUndefined } from "@juniper-lib/util";
import { TypedEventTarget } from "@juniper-lib/events";

export class HistoryStateChangedEvent<StateT> extends Event {
    constructor(state: StateT) {
        super("statechanged");
        this.#state = state;
    }

    #state: StateT;
    get state() { return this.#state; }
}

interface HistoryManagerOptions<StateT> {
    queryStringParam?: string;
    pathPattern?: RegExp;
    stateDeserializer: (value: string) => StateT;
    stateSerializer: (state: StateT) => string;
}

export class HistoryManager<StateT> extends TypedEventTarget<{
    "statechanged": HistoryStateChangedEvent<StateT>
}> {
    #curState: StateT;
    get state() { return this.#curState; }

    readonly #options: HistoryManagerOptions<StateT>;

    #here: URL = null;
    #paused = false;

    /**
     * Creates a HistoryManager object for managing the displayed URL and
     * parsing key data for use in deep-linkable single-page-app style pages.
     * 
     * Supports both query-string parameters and path parameters.
     * 
     * When defining a path parameter, use a Regular Expression that contains
     * a capture group for the whole key parameter. This should be capture
     * group number 1.
     * 
     * @param options
     */
    constructor(options: HistoryManagerOptions<StateT>) {
        super();
        this.#options = Object.assign({}, options) as HistoryManagerOptions<StateT>;

        if (isNullOrUndefined(this.#options.pathPattern) && isNullOrUndefined(this.#options.queryStringParam)) {
            throw new Exception("One of pathPattern or queryStringParam must be included in options.");
        }

        if (isDefined(this.#options.pathPattern) && isDefined(this.#options.queryStringParam)) {
            throw new Exception("Only one of pathPattern or queryStringParam my be included in options.");
        }

        this.#here = new URL(location.href);

        globalThis.addEventListener("popstate", (evt) => this.#check(evt));
    }

    #check(evt?: PopStateEvent) {
        this.#here = new URL(location.href);

        this.#curState = evt && evt.state
            || !evt && this.#options.stateDeserializer(this.#getSlug())
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

    push(state: StateT) {
        if (!this.#paused) {
            const str = this.#options.stateSerializer(state);
            if (isDefined(str)) {
                this.#addSlug(str);
            }
            else {
                this.#removeSlug();
            }
            history.pushState(state, null, this.#here);
        }
    }

    clear() {
        this.#removeSlug();
        history.replaceState(null, null, this.#here);
    }

    #addSlug(str: string) {
        if (this.#options.queryStringParam) {
            this.#here.searchParams.set(this.#options.queryStringParam, str);
        }
        else {
            if (this.#options.pathPattern.test(this.#here.href)) {
                this.#removeSlug();
            }

            this.#here.href += "/" + str;
        }
    }

    #removeSlug() {
        if (this.#options.queryStringParam) {
            this.#here.searchParams.delete(this.#options.queryStringParam);
        }
        else if (this.#options.pathPattern.test(this.#here.href)) {
            this.#here.href = this.#here.href.substring(0, this.#here.href.lastIndexOf("/"));
        }
    }

    #getSlug() {
        if (this.#options.queryStringParam) {
            return this.#here.searchParams.get(this.#options.queryStringParam);
        }
        else {
            const match = this.#here.href.match(this.#options.pathPattern);
            if (match) {
                return match[1];
            }
        }

        return null;
    }
}