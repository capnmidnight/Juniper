import { ElementChild } from "@juniper-lib/dom";
export type AsyncDocFragmentReadyState = "none" | "fetching" | "complete" | "error";
export declare class AsyncDocFragmentElement extends HTMLElement {
    #private;
    get ready(): Promise<AsyncDocFragmentElement>;
    constructor();
    connectedCallback(): void;
    get readyState(): AsyncDocFragmentReadyState;
    get error(): any;
    get src(): string;
    set src(v: string);
    static install(): import("@juniper-lib/dom").ElementFactory<AsyncDocFragmentElement>;
}
export declare function AsyncDocFragment(...rest: ElementChild<AsyncDocFragmentElement>[]): AsyncDocFragmentElement;
//# sourceMappingURL=AsyncDocFragment.d.ts.map