import { TypedEvent, TypedEventTarget } from "./TypedEventTarget";
export class WindowQuitEventer extends TypedEventTarget {
    constructor() {
        super();
        this.event = new TypedEvent("quitting");
        const onWindowClosed = () => this.dispatchEvent(this.event);
        window.addEventListener("beforeunload", onWindowClosed);
        window.addEventListener("unload", onWindowClosed);
        window.addEventListener("pagehide", onWindowClosed);
    }
}
//# sourceMappingURL=WindowQuitEventer.js.map