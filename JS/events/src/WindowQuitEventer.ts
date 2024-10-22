import { TypedEvent, TypedEventTarget } from "./TypedEventTarget";

export class WindowQuitEventer extends TypedEventTarget<{
    "quitting": TypedEvent<"quitting">
}>{
    readonly #event = new TypedEvent("quitting");

    constructor() {
        super();

        const onWindowClosed = () => this.dispatchEvent(this.#event);

        globalThis.addEventListener("beforeunload", onWindowClosed);
        globalThis.addEventListener("unload", onWindowClosed);
        globalThis.addEventListener("pagehide", onWindowClosed);
    }
}