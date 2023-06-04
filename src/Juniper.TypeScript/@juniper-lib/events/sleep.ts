import { isDefined } from "@juniper-lib/tslib/typeChecks";
import { Task } from "./Task";

export class SleepTask extends Task {

    private _timer: number = null;

    constructor(private readonly milliseconds: number) {
        super(false);
    }

    override start(): void {
        super.start();
        this._timer = setTimeout(() => {
            this._timer = null;
            this.resolve();
        }, this.milliseconds) as unknown as number;
    }

    override reset(): void {
        super.reset();
        if (isDefined(this._timer)) {
            clearTimeout(this._timer);
            this._timer = null;
        }
    }
}

export function sleep(milliseconds: number): SleepTask {
    const task = new SleepTask(milliseconds);
    task.start();
    return task;
}
