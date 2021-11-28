import { arrayClear } from "../collections/arrayClear";
export class Animator {
    animations = new Array();
    update(dt) {
        dt = 0.001 * dt;
        for (const animation of this.animations) {
            animation(dt);
        }
    }
    clear() {
        arrayClear(this.animations);
    }
    start(delay, duration, update) {
        let time = -delay;
        update(0);
        return new Promise((resolve) => {
            this.animations.push((dt) => {
                time += dt / duration;
                if (time >= 1) {
                    update(1);
                    resolve();
                }
                else if (time >= 0) {
                    update(time);
                }
            });
        });
    }
}
