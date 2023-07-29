import { TypedEvent, TypedEventBase } from "./TypedEventBase";

export class DwellEvent extends TypedEvent<"dwell">{
    constructor(public readonly dwellTimeSeconds: number) {
        super("dwell");
    }
}

export class DwellEventer extends TypedEventBase<{
    dwell: DwellEvent;
}>{
    private startTime: number = null;
    private graceTimer: number = null;

    constructor(
        private readonly minTimeSeconds: number = 1,
        private readonly graceTimeSeconds: number = 0.25) {
        super();
        Object.seal(this);
    }

    start() {
        this.stopGrace();

        if (this.startTime === null) {
            this.startTime = performance.now();
        }
    }

    private stopGrace() {
        if (this.graceTimer !== null) {
            clearTimeout(this.graceTimer);
            this.graceTimer = null;
        }
    }

    stop() {
        if (this.startTime !== null) {
            this.graceTimer = setTimeout(() => {
                this.graceTimer = null;
                const delta = (performance.now() - this.startTime) / 1000;
                this.startTime = null;
                if (delta > this.minTimeSeconds) {
                    this.dispatchEvent(new DwellEvent(delta));
                }
            }, this.graceTimeSeconds * 1000) as any;
        }
    }
}
