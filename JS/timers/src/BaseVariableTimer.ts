import { BaseManagedTimer } from "./BaseManagedTimer";

export abstract class BaseVariableTimer<TimerT> extends BaseManagedTimer<TimerT> {

    #targetFPS: number = null;
    get targetFPS() { return this.#targetFPS; }
    set targetFPS(v: number) { this.#targetFPS = v; }

    constructor(targetFrameRate: number) {
        super();
        this.targetFPS = targetFrameRate;
    }

    protected get targetFrameTime() {
        return 1000 / this.targetFPS;
    }
}
