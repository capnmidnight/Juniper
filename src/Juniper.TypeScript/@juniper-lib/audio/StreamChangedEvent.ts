import { TypedEvent } from "@juniper-lib/events/TypedEventBase";

export class StreamChangedEvent
    extends TypedEvent<"streamchanged"> {
    public constructor(
        public readonly oldStream: MediaStream,
        public readonly newStream: MediaStream) {
        super("streamchanged");
    }
}
