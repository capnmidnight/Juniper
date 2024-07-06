import { Exception } from "@juniper-lib/util";

export class CancelSignalException extends Exception {
    constructor() {
        super("Cancellation!");
    }
}

export class CancelToken {
    #cancelled = false;
    get cancelled() { return this.#cancelled; }

    check() {
        if (this.cancelled) {
            throw new CancelSignalException();
        }
    }

    cancel() {
        this.#cancelled = true;
    }
}
