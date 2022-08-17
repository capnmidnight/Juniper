import { TypedEvent } from "@juniper-lib/tslib/events/EventBase";
import type { Application } from "./Application";

class ApplicationEvent<T extends string> extends TypedEvent<T> {
    constructor(type: T, public readonly app: Application) {
        super(type);
    }
}

export class ApplicationJoinRoomEvent extends ApplicationEvent<"joinroom">{
    constructor(app: Application, public readonly roomName: string) {
        super("joinroom", app);
    }
}

export class ApplicationQuitEvent extends ApplicationEvent<"quit">{
    constructor(app: Application) {
        super("quit", app);
    }
}