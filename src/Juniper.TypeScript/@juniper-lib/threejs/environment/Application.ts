import { TypedEvent, TypedEventTarget } from "@juniper-lib/events/TypedEventTarget";
import { IDataLogger } from "@juniper-lib/tslib/IDataLogger";
import type { IProgress } from "@juniper-lib/progress/IProgress";
import { isDefined } from "@juniper-lib/tslib/typeChecks";
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


export type ApplicationEvents = {
    joinroom: ApplicationJoinRoomEvent;
    quit: ApplicationQuitEvent;
    shown: ApplicationShownEvent;
    hidden: ApplicationHiddenEvent;
}

export abstract class Application<EventsT extends ApplicationEvents = ApplicationEvents>
    extends TypedEventTarget<EventsT>
    implements IDisposable, IDataLogger {

    private dataLogger: IDataLogger = null;

    constructor(public readonly env: Environment) {
        super();
    }

    quit() {
        this.dispatchEvent(new ApplicationQuitEvent(this));
    }

    join(roomName: string) {
        this.dispatchEvent(new ApplicationJoinRoomEvent(this, roomName));
        this.env.avatar.reset();
    }

    async show(prog?: IProgress): Promise<void> {
        await this.showing(prog);
        this.dispatchEvent(new ApplicationShownEvent(this));
    }

    hide(): void {
        this.hiding();
        this.dispatchEvent(new ApplicationHiddenEvent(this));
    }

    init(params: Map<string, unknown>): Promise<void> {
        this.dataLogger = params.get("dataLogger") as IDataLogger;
        return Promise.resolve();
    }

    log(key: string, value?: object): void {
        if (isDefined(this.dataLogger)) {
            this.dataLogger.log(key, value);
        }
    }

    error(page: string, operation: string, exception: any): void {
        if (isDefined(this.dataLogger)) {
            this.dataLogger.error(page, operation, exception);
        }
    }

    onError(page: string, operation: string): (exception: any) => void {
        return this.error.bind(this, page, operation);
    }

    protected abstract showing(prog?: IProgress): Promise<void>;
    protected abstract hiding(): void;

    abstract get visible(): boolean;

    abstract load(prog?: IProgress): Promise<void>;
    abstract dispose(): void;
}

export type ApplicationConstructor = Constructor<Application, typeof Application>

export interface ApplicationModule {
    default: ApplicationConstructor;
}