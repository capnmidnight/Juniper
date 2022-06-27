import { arrayClear, arrayRemove, Task } from "@juniper-lib/tslib";

export class Animator {
    private animations = new Array<(dt: number) => void>();

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
        const animationComplete = new Task();
        const onTick = (dt: number) => {
            time += dt / duration;
            if (time >= 1) {
                update(1);
                arrayRemove(this.animations, onTick);
                animationComplete.resolve();
            }
            else if (time >= 0) {
                update(time);
            }
        };
        this.animations.push(onTick);
        return animationComplete;
    }
}
