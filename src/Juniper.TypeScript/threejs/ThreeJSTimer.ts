import { BaseTimer } from "juniper-timers";

export class ThreeJSTimer extends BaseTimer<XRAnimationLoopCallback> {

    constructor(private readonly renderer: THREE.WebGLRenderer) {
        super(120);
    }

    private setAnimationLoop(loop: XRAnimationLoopCallback) {
        this._timer = loop;
        this.renderer.setAnimationLoop(this._timer as any);
    }

    start() {
        if (!this.isRunning) {
            this.setAnimationLoop(this._onTick);
        }
    }

    override stop() {
        if (this.isRunning) {
            this.setAnimationLoop(null);
            super.stop();
        }
    }
}
