import { TypedEvent } from "@juniper-lib/events";
export declare class StreamChangedEvent extends TypedEvent<"streamchanged"> {
    readonly oldStream: MediaStream;
    readonly newStream: MediaStream;
    constructor(oldStream: MediaStream, newStream: MediaStream);
}
//# sourceMappingURL=StreamChangedEvent.d.ts.map