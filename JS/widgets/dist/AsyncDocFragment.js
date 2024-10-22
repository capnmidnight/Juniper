import { singleton } from "@juniper-lib/util";
import { Div, ImportDOM, registerFactory } from "@juniper-lib/dom";
export class AsyncDocFragmentElement extends HTMLElement {
    #ready;
    get ready() { return this.#ready; }
    #placeholder;
    #error = null;
    #readyState = "none";
    constructor() {
        super();
        this.#ready = new Promise((resolve, reject) => {
            const onLoad = () => {
                this.removeEventListener("load", onLoad);
                this.removeEventListener("error", onError);
                resolve(this);
            };
            const onError = (evt) => {
                this.removeEventListener("load", onLoad);
                this.removeEventListener("error", onError);
                reject(evt.error);
            };
            this.addEventListener("load", onLoad);
            this.addEventListener("error", onError);
        });
        this.#placeholder = Div();
    }
    connectedCallback() {
        if (this.#readyState === "none"
            && this.#placeholder
            && !this.#placeholder.parentElement) {
            this.append(this.#placeholder);
            this.#refresh();
        }
    }
    get readyState() { return this.#readyState; }
    get error() { return this.#error; }
    get src() {
        return this.getAttribute("src");
    }
    set src(v) {
        if (this.readyState !== "none") {
            throw new Error("Cannot change fragment source after initialization.");
        }
        this.setAttribute("src", v);
        this.#refresh();
    }
    async #refresh() {
        if (this.src
            && this.#readyState === "none"
            && this.#placeholder
            && this.#placeholder.parentElement) {
            this.#readyState = "fetching";
            this.dispatchEvent(new Event("loadstart"));
            try {
                const interior = await ImportDOM(this.src);
                this.#placeholder.replaceWith(...interior);
                this.#placeholder = null;
                this.#readyState = "complete";
                this.dispatchEvent(new Event("load"));
            }
            catch (error) {
                this.#error = error;
                this.dispatchEvent(new ErrorEvent("error", { error }));
            }
        }
    }
    static install() {
        return singleton("Juniper::Widgets::AsynDocFragmentElement", () => registerFactory("async-doc-fragment", AsyncDocFragmentElement));
    }
}
export function AsyncDocFragment(...rest) {
    return AsyncDocFragmentElement.install()(...rest);
}
//# sourceMappingURL=AsyncDocFragment.js.map