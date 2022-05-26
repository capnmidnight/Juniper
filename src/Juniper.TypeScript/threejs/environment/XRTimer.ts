import { arrayRemove, BaseTimerTickEvent, isDefined, ITimer } from "@juniper-lib/tslib";

export class XRTimerTickEvent extends BaseTimerTickEvent {
    frame?: XRFrame = null;

    constructor() {
        super();
        Object.seal(this);
    }

    override set(t: number, dt: number, frame?: XRFrame) {
        super.set(t, dt);
        this.frame = frame;
    }
}

type TimerTickCallback = (evt: XRTimerTickEvent) => void;

export class XRTimer implements ITimer {
    private tickHandlers = new Array<TimerTickCallback>();
    private _onTick: (t: number, frame?: XRFrame) => void;
    private lt = -1;

    constructor(private readonly renderer: THREE.WebGLRenderer) {
        const tickEvt = new XRTimerTickEvent();
        let dt = 0;
        this._onTick = (t: number, frame?: XRFrame) => {
            if (this.lt >= 0) {
                dt = t - this.lt;
                tickEvt.set(t, dt, frame);
                this.tick(tickEvt);
            }
            this.lt = t;
        };
    }

    #isRunning = false;
    get isRunning() {
        return this.#isRunning;
    }

    restart() {
        this.stop();
        this.start();
    }

    addTickHandler(onTick: (evt: XRTimerTickEvent) => void): void {
        this.tickHandlers.push(onTick);
    }

    removeTickHandler(onTick: (evt: XRTimerTickEvent) => void): void {
        arrayRemove(this.tickHandlers, onTick);
    }

    private setAnimationLoop(loop: XRFrameRequestCallback) {
        this.renderer.setAnimationLoop(loop as any);
        this.#isRunning = isDefined(loop);
    }

    start() {
        if (!this.isRunning) {
            this.setAnimationLoop(this._onTick);
        }
    }

    stop() {
        if (this.isRunning) {
            this.setAnimationLoop(null);
            this.lt = -1;
        }
    }

    private tick(evt: XRTimerTickEvent): void {
        for (const handler of this.tickHandlers) {
            handler(evt);
        }
    }
}
