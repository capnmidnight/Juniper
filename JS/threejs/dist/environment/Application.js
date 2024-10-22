import { TypedEvent, TypedEventTarget } from "@juniper-lib/events";
import { isDefined } from "@juniper-lib/util";
class ApplicationEvent extends TypedEvent {
    constructor(type, app) {
        super(type);
        this.app = app;
    }
}
export class ApplicationJoinRoomEvent extends ApplicationEvent {
    constructor(app, roomName) {
        super("joinroom", app);
        this.roomName = roomName;
    }
}
export class ApplicationQuitEvent extends ApplicationEvent {
    constructor(app) {
        super("quit", app);
    }
}
export class ApplicationShownEvent extends ApplicationEvent {
    constructor(app) {
        super("shown", app);
    }
}
export class ApplicationHiddenEvent extends ApplicationEvent {
    constructor(app) {
        super("hidden", app);
    }
}
export class Application extends TypedEventTarget {
    constructor(env) {
        super();
        this.env = env;
        this.dataLogger = null;
    }
    quit() {
        this.dispatchEvent(new ApplicationQuitEvent(this));
    }
    join(roomName) {
        this.dispatchEvent(new ApplicationJoinRoomEvent(this, roomName));
        this.env.avatar.reset();
    }
    async show(prog) {
        await this.showing(prog);
        this.dispatchEvent(new ApplicationShownEvent(this));
    }
    hide() {
        this.hiding();
        this.dispatchEvent(new ApplicationHiddenEvent(this));
    }
    init(params) {
        this.dataLogger = params.get("dataLogger");
        return Promise.resolve();
    }
    log(key, value) {
        if (isDefined(this.dataLogger)) {
            this.dataLogger.log(key, value);
        }
    }
    error(page, operation, exception) {
        if (isDefined(this.dataLogger)) {
            this.dataLogger.error(page, operation, exception);
        }
    }
    onError(page, operation) {
        return this.error.bind(this, page, operation);
    }
}
//# sourceMappingURL=Application.js.map