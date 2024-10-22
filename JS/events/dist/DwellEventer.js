import { TypedEvent, TypedEventTarget } from "./TypedEventTarget";
export class DwellEvent extends TypedEvent {
    constructor(dwellTimeSeconds) {
        super("dwell");
        this.dwellTimeSeconds = dwellTimeSeconds;
    }
}
export class DwellEventer extends TypedEventTarget {
    #startTime = null;
    #graceTimer = null;
    #minTimeSeconds;
    #graceTimeSeconds;
    constructor(minTimeSeconds = 1, graceTimeSeconds = 0.25) {
        super();
        this.#minTimeSeconds = minTimeSeconds;
        this.#graceTimeSeconds = graceTimeSeconds;
        Object.seal(this);
    }
    start() {
        this.#stopGrace();
        if (this.#startTime === null) {
            this.#startTime = performance.now();
        }
    }
    #stopGrace() {
        if (this.#graceTimer !== null) {
            clearTimeout(this.#graceTimer);
            this.#graceTimer = null;
        }
    }
    stop() {
        if (this.#startTime !== null) {
            this.#graceTimer = setTimeout(() => {
                this.#graceTimer = null;
                const delta = (performance.now() - this.#startTime) / 1000;
                this.#startTime = null;
                if (delta > this.#minTimeSeconds) {
                    this.dispatchEvent(new DwellEvent(delta));
                }
            }, this.#graceTimeSeconds * 1000);
        }
    }
}
//# sourceMappingURL=DwellEventer.js.map