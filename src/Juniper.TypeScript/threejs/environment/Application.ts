import type { IDisposable, IProgress, TimerTickEvent, TypedEventBase } from "@juniper/tslib";
import type { ApplicationJoinRoomEvent, ApplicationQuitEvent } from "./ApplicationEvents";
import type { Environment } from "./Environment";

export interface ApplicationEvents {
    joinroom: ApplicationJoinRoomEvent;
    quit: ApplicationQuitEvent;
}

export interface Application extends IDisposable, TypedEventBase<ApplicationEvents> {
    init(params: Map<string, unknown>): Promise<void>;
    load(prog?: IProgress): Promise<void>;
    show(prog?: IProgress): Promise<void>;
    update(evt: TimerTickEvent): void;
}

export interface ApplicationConstructor {
    new(environment: Environment): Application;
}

export interface ApplicationModule {
    default: ApplicationConstructor;
}