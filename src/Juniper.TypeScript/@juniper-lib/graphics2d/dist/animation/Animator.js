import { arrayScan } from "@juniper-lib/collections/dist/arrays";
import { Task } from "@juniper-lib/events/dist/Task";
function isDead(task) {
    return !task.started || task.finished;
}
class AnimationTask extends Task {
    constructor() {
        super(false);
        this.time = 0;
        this.duration = 0;
        this.onTick = null;
    }
    begin(delay, duration, onTick) {
        this.restart();
        this.time = -delay;
        this.duration = duration;
        this.onTick = onTick;
        this.onTick(0);
    }
    update(dt) {
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
    constructor() {
        this.animations = new Array();
    }
    update(dt) {
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
    start(delay, duration, update) {
        let task = arrayScan(this.animations, isDead);
        if (!task) {
            this.animations.push(task = new AnimationTask());
        }
        task.begin(delay, duration, update);
        return task;
    }
}
//# sourceMappingURL=Animator.js.map