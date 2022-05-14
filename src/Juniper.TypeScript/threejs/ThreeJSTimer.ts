import { BaseTimer } from "@juniper/tslib";

export class ThreeJSTimer extends BaseTimer<XRFrameRequestCallback> {

    constructor(private readonly renderer: THREE.WebGLRenderer) {
        super(120);
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

    override stop() {
        if (this.isRunning) {
            this.setAnimationLoop(null);
            super.stop();
        }
    }
}
