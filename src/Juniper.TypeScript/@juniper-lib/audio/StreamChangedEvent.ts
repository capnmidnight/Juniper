import { TypedEvent } from "@juniper-lib/tslib/events/EventBase";

export class StreamChangedEvent
    extends TypedEvent<"streamchanged"> {
    public constructor(public readonly device: MediaDeviceInfo,
        public readonly oldStream: MediaStream,
        public readonly newStream: MediaStream) {
        super("streamchanged");
    }
}
