import { isDefined } from "@juniper-lib/util";
import { Task } from "./Task";

export class SleepTask extends Task {

    #timer: number = null;
    readonly #milliseconds: number;

    constructor(milliseconds: number) {
        super(false);
        this.#milliseconds = milliseconds;
    }

    override start(): void {
        super.start();
        this.#timer = setTimeout(() => {
            this.#timer = null;
            this.resolve();
        }, this.#milliseconds) as unknown as number;
    }

    override reset(): void {
        super.reset();
        if (isDefined(this.#timer)) {
            clearTimeout(this.#timer);
            this.#timer = null;
        }
    }
}

export function sleep(milliseconds: number): SleepTask {
    const task = new SleepTask(milliseconds);
    task.start();
    return task;
}
