import { TypedEvent, TypedEventTarget } from "./TypedEventTarget";

export class WindowQuitEventer extends TypedEventTarget<{
    "quitting": TypedEvent<"quitting">
}>{
    private readonly event = new TypedEvent("quitting");

    constructor() {
        super();

        const onWindowClosed = () => this.dispatchEvent(this.event);

        window.addEventListener("beforeunload", onWindowClosed);
        window.addEventListener("unload", onWindowClosed);
        window.addEventListener("pagehide", onWindowClosed);
    }
}