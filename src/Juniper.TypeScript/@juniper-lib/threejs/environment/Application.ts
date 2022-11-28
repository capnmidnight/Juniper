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

export class ApplicationHiddenEvent extends ApplicationEvent<"hidden">{
    constructor(app: Application) {
        super("hidden", app);
    }
}


export interface ApplicationEvents {
    joinroom: ApplicationJoinRoomEvent;
    quit: ApplicationQuitEvent;
    shown: ApplicationShownEvent;
    hidden: ApplicationHiddenEvent;
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

    async show(prog?: IProgress): Promise<void> {
        await this.showing(prog);
        this.dispatchEvent(new ApplicationShownEvent(this));
    }

    hide(): void {
        this.hiding();
        this.dispatchEvent(new ApplicationHiddenEvent(this));
    }

    protected abstract showing(prog?: IProgress): Promise<void>;
    protected abstract hiding(): void;

    abstract get visible(): boolean;

    abstract init(params: Map<string, unknown>): Promise<void>;
    abstract load(prog?: IProgress): Promise<void>;
    abstract dispose(): void;
}

export interface ApplicationConstructor extends Constructor<Application, typeof Application> { }

export interface ApplicationModule {
    default: ApplicationConstructor;
}