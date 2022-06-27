import { arrayClear, Task } from "@juniper-lib/tslib";

export class Animator {
    private animations = new Array<(dt: number) => void>();
    private readonly complete = new Task();

    update(dt: number) {
        dt = 0.001 * dt;
        for (const animation of this.animations) {
            animation(dt);
        }
    }

    clear() {
        arrayClear(this.animations);
    }

    start(delay: number, duration: number, update: (t: number) => void): Promise<void> {
        let time = -delay;
        update(0);
        this.complete.reset();
        this.animations.push((dt: number) => {
            time += dt / duration;
            if (time >= 1) {
                update(1);
                this.complete.resolve();
            }
            else if (time >= 0) {
                update(time);
            }
        });
        return this.complete;
    }
}
