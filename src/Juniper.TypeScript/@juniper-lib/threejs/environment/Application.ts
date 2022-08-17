import { TypedEventBase } from "@juniper-lib/tslib/events/EventBase";
import type { IProgress } from "@juniper-lib/tslib/progress/IProgress";
import type { TimerTickEvent } from "@juniper-lib/tslib/timers/ITimer";
import type { IDisposable } from "@juniper-lib/tslib/using";
import type { ApplicationJoinRoomEvent, ApplicationQuitEvent } from "./ApplicationEvents";
import type { Environment } from "./Environment";

export interface ApplicationEvents {
    joinroom: ApplicationJoinRoomEvent;
    quit: ApplicationQuitEvent;
}

export abstract class Application<EventsT extends ApplicationEvents = ApplicationEvents>
    extends TypedEventBase<EventsT>
    implements IDisposable {

    constructor(public readonly env: Environment) {
        super();
    }

    abstract init(params: Map<string, unknown>): Promise<void>;
    abstract load(prog?: IProgress): Promise<void>;
    abstract show(prog?: IProgress): Promise<void>;
    abstract update(evt: TimerTickEvent): void;
    abstract dispose(): void;

}

export interface ApplicationConstructor extends Constructor<Application, typeof Application> { }

export interface ApplicationModule {
    default: ApplicationConstructor;
}