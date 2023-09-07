import { TypedEvent, TypedEventTarget } from "@juniper-lib/events/TypedEventTarget";
import { IDataLogger } from "@juniper-lib/tslib/IDataLogger";
import type { IProgress } from "@juniper-lib/progress/IProgress";
import type { IDisposable } from "@juniper-lib/tslib/using";
import type { Environment } from "./Environment";
declare class ApplicationEvent<T extends string> extends TypedEvent<T> {
    readonly app: Application;
    constructor(type: T, app: Application);
}
export declare class ApplicationJoinRoomEvent extends ApplicationEvent<"joinroom"> {
    readonly roomName: string;
    constructor(app: Application, roomName: string);
}
export declare class ApplicationQuitEvent extends ApplicationEvent<"quit"> {
    constructor(app: Application);
}
export declare class ApplicationShownEvent extends ApplicationEvent<"shown"> {
    constructor(app: Application);
}
export declare class ApplicationHiddenEvent extends ApplicationEvent<"hidden"> {
    constructor(app: Application);
}
export type ApplicationEvents = {
    joinroom: ApplicationJoinRoomEvent;
    quit: ApplicationQuitEvent;
    shown: ApplicationShownEvent;
    hidden: ApplicationHiddenEvent;
};
export declare abstract class Application<EventsT extends ApplicationEvents = ApplicationEvents> extends TypedEventTarget<EventsT> implements IDisposable, IDataLogger {
    readonly env: Environment;
    private dataLogger;
    constructor(env: Environment);
    quit(): void;
    join(roomName: string): void;
    show(prog?: IProgress): Promise<void>;
    hide(): void;
    init(params: Map<string, unknown>): Promise<void>;
    log(key: string, value?: object): void;
    error(page: string, operation: string, exception: any): void;
    onError(page: string, operation: string): (exception: any) => void;
    protected abstract showing(prog?: IProgress): Promise<void>;
    protected abstract hiding(): void;
    abstract get visible(): boolean;
    abstract load(prog?: IProgress): Promise<void>;
    abstract dispose(): void;
}
export type ApplicationConstructor = Constructor<Application, typeof Application>;
export interface ApplicationModule {
    default: ApplicationConstructor;
}
export {};
//# sourceMappingURL=Application.d.ts.map