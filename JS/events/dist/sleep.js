import { isDefined } from "@juniper-lib/util";
import { Task } from "./Task";
export class SleepTask extends Task {
    #timer = null;
    #milliseconds;
    constructor(milliseconds) {
        super(false);
        this.#milliseconds = milliseconds;
    }
    start() {
        super.start();
        this.#timer = setTimeout(() => {
            this.#timer = null;
            this.resolve();
        }, this.#milliseconds);
    }
    reset() {
        super.reset();
        if (isDefined(this.#timer)) {
            clearTimeout(this.#timer);
            this.#timer = null;
        }
    }
}
export function sleep(milliseconds) {
    const task = new SleepTask(milliseconds);
    task.start();
    return task;
}
//# sourceMappingURL=sleep.js.map