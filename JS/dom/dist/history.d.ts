import { TypedEventTarget } from "@juniper-lib/events";
export declare class HistoryStateChangedEvent<StateT> extends Event {
    #private;
    constructor(state: StateT);
    get state(): StateT;
}
interface HistoryManagerOptions<StateT> {
    queryStringParam?: string;
    pathPattern?: RegExp;
    stateDeserializer: (value: string) => StateT;
    stateSerializer: (state: StateT) => string;
}
export declare class HistoryManager<StateT> extends TypedEventTarget<{
    "statechanged": HistoryStateChangedEvent<StateT>;
}> {
    #private;
    get state(): StateT;
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
    constructor(options: HistoryManagerOptions<StateT>);
    pause(): void;
    resume(): void;
    start(): void;
    push(state: StateT): void;
    clear(): void;
}
export {};
//# sourceMappingURL=history.d.ts.map