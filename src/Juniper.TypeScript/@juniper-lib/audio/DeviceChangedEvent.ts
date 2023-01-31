import { TypedEvent } from "@juniper-lib/tslib/events/EventBase";

export class DeviceChangedEvent
    extends TypedEvent<"devicechanged"> {
    public constructor(public readonly device: MediaDeviceInfo) {
        super("devicechanged");
    }
}
