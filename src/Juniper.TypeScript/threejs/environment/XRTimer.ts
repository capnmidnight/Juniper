import { arrayRemove, ITimer, BaseTimerTickEvent } from "@juniper/tslib";

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

type TimerSource = typeof globalThis | XRSession;
type TimerTickCallback = (evt: XRTimerTickEvent) => void;

export class XRTimer implements ITimer {
    private _timerSource: TimerSource = null;
    private _timer: XRFrameRequestCallback = null;
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

    get timerSource(): TimerSource {
        return this._timerSource;
    }

    set timerSource(v: TimerSource) {
        if (v !== this.timerSource) {
            this.stop();
            this._timerSource = v;
            this.start();
        }
    }

    get isRunning() {
        return this._timer != null;
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
        this._timer = loop;
        this.renderer.setAnimationLoop(this._timer as any);
    }

    start() {
        if (!this.isRunning) {
            this.setAnimationLoop(this._onTick);
        }
    }

    stop() {
        if (this.isRunning) {
            this.setAnimationLoop(null);
            this._timer = null;
            this.lt = -1;
        }
    }

    private tick(evt: XRTimerTickEvent): void {
        for (const handler of this.tickHandlers) {
            handler(evt);
        }
    }
}
