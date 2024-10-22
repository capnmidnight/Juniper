import { arrayScan } from "@juniper-lib/collections/dist/arrays";
import { Task } from "@juniper-lib/events/dist/Task";

function isDead(task: AnimationTask): boolean {
    return !task.started || task.finished;
}

class AnimationTask extends Task {
    private time = 0;
    private duration = 0;
    private onTick: (t: number) => void = null;

    constructor() {
        super(false);
    }

    begin(delay: number, duration: number, onTick: (t: number) => void) {
        this.restart();
        this.time = -delay;
        this.duration = duration;
        this.onTick = onTick;
        this.onTick(0);
    }

    update(dt: number) {
        if (!isDead(this)) {
            this.time += dt / this.duration;
            if (this.time >= 1) {
                this.onTick(1);
                this.resolve();
            }
            else if (this.time >= 0) {
                this.onTick(this.time);
            }
        }
    }
}

export class Animator {
    private animations = new Array<AnimationTask>();

    update(dt: number) {
        dt = 0.001 * dt;
        for (const animation of this.animations) {
            animation.update(dt);
        }
    }

    clear() {
        for (const animation of this.animations) {
            animation.resolve();
        }
    }

    start(delay: number, duration: number, update: (t: number) => void): Promise<void> {
        let task = arrayScan(this.animations, isDead);
        if (!task) {
            this.animations.push(task = new AnimationTask());
        }
        task.begin(delay, duration, update);
        return task;
    }
}
