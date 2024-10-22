import { TypedEvent } from "@juniper-lib/events/dist/TypedEventTarget";
export class StreamChangedEvent extends TypedEvent {
    constructor(oldStream, newStream) {
        super("streamchanged");
        this.oldStream = oldStream;
        this.newStream = newStream;
    }
}
//# sourceMappingURL=StreamChangedEvent.js.map