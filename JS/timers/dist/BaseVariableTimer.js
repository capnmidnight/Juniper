import { BaseManagedTimer } from "./BaseManagedTimer";
export class BaseVariableTimer extends BaseManagedTimer {
    #targetFPS = null;
    get targetFPS() { return this.#targetFPS; }
    set targetFPS(v) { this.#targetFPS = v; }
    constructor(targetFrameRate) {
        super();
        this.targetFPS = targetFrameRate;
    }
    get targetFrameTime() {
        return 1000 / this.targetFPS;
    }
}
//# sourceMappingURL=BaseVariableTimer.js.map