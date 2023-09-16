import { TypedEvent } from "@juniper-lib/events/dist/TypedEventTarget";

export class StreamChangedEvent
    extends TypedEvent<"streamchanged"> {
    public constructor(
        public readonly oldStream: MediaStream,
        public readonly newStream: MediaStream) {
        super("streamchanged");
    }
}
