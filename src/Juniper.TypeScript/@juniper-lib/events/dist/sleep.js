import { isDefined } from "@juniper-lib/tslib/dist/typeChecks";
import { Task } from "./Task";
export class SleepTask extends Task {
    constructor(milliseconds) {
        super(false);
        this.milliseconds = milliseconds;
        this._timer = null;
    }
    start() {
        super.start();
        this._timer = setTimeout(() => {
            this._timer = null;
            this.resolve();
        }, this.milliseconds);
    }
    reset() {
        super.reset();
        if (isDefined(this._timer)) {
            clearTimeout(this._timer);
            this._timer = null;
        }
    }
}
export function sleep(milliseconds) {
    const task = new SleepTask(milliseconds);
    task.start();
    return task;
}
//# sourceMappingURL=sleep.js.map