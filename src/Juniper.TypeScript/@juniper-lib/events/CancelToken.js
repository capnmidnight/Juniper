import { Exception } from "@juniper-lib/tslib/Exception";
export class CancelSignalException extends Exception {
    constructor() {
        super("Cancellation!");
    }
}
export class CancelToken {
    constructor() {
        this._cancelled = false;
    }
    get cancelled() {
        return this._cancelled;
    }
    check() {
        if (this.cancelled) {
            throw new CancelSignalException();
        }
    }
    cancel() {
        this._cancelled = true;
    }
}
//# sourceMappingURL=CancelToken.js.map