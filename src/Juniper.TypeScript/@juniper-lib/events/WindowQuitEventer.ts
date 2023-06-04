import { TypedEvent, TypedEventBase } from "./EventBase";

export class WindowQuitEventer extends TypedEventBase<{
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