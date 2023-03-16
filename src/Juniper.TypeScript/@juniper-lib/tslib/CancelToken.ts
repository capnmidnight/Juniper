import { Exception } from "./Exception";
import { IDisposable } from "./using";

export class CancelSignalException extends Exception {
    constructor() {
        super("Cancellation!");
    }
}

export class CancelToken implements IDisposable {
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

    dispose(): void {
        throw new Error("Method not implemented.");
    }
}