import { Exception } from "@juniper-lib/tslib/dist/Exception";

export class CancelSignalException extends Exception {
    constructor() {
        super("Cancellation!");
    }
}

export class CancelToken {
    private _cancelled = false;

    public get cancelled() {
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