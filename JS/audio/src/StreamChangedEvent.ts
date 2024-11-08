import { TypedEvent } from "@juniper-lib/events";

export class StreamChangedEvent
    extends TypedEvent<"streamchanged"> {
    public constructor(
        public readonly oldStream: MediaStream,
        public readonly newStream: MediaStream) {
        super("streamchanged");
    }
}
