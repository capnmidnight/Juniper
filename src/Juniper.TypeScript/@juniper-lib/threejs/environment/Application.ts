import { TypedEvent, TypedEventBase } from "@juniper-lib/tslib/events/EventBase";
import type { IProgress } from "@juniper-lib/tslib/progress/IProgress";
import type { IDisposable } from "@juniper-lib/tslib/using";
import type { Environment } from "./Environment";


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

export class ApplicationShownEvent extends ApplicationEvent<"shown">{
    constructor(app: Application) {
        super("shown", app);
    }
}


export interface ApplicationEvents {
    joinroom: ApplicationJoinRoomEvent;
    quit: ApplicationQuitEvent;
    shown: ApplicationShownEvent;
}

export abstract class Application<EventsT extends ApplicationEvents = ApplicationEvents>
    extends TypedEventBase<EventsT>
    implements IDisposable {

    constructor(public readonly env: Environment) {
        super();
    }

    quit() {
        this.dispatchEvent(new ApplicationQuitEvent(this));
    }

    join(roomName: string) {
        this.dispatchEvent(new ApplicationJoinRoomEvent(this, roomName));
    }

    abstract init(params: Map<string, unknown>): Promise<void>;
    abstract load(prog?: IProgress): Promise<void>;
    protected abstract showing(prog?: IProgress): Promise<void>;
    async show(prog?: IProgress): Promise<void> {
        await this.showing(prog);
        this.dispatchEvent(new ApplicationShownEvent(this));
    }
    abstract dispose(): void;
}

export interface ApplicationConstructor extends Constructor<Application, typeof Application> { }

export interface ApplicationModule {
    default: ApplicationConstructor;
}