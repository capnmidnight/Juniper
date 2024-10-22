import { bump } from "@juniper-lib/graphics2d/dist/animation/tween";
import { singleton } from "@juniper-lib/tslib/dist/singleton";
import { dispose } from "@juniper-lib/tslib/dist/using";
import { objectResolve } from "../objects";
import { isMesh } from "../typeChecks";
/**
 * This is a hack to make sure all Applications get the same scaleOnHover state as the Environment.
 **/
const scaledItems = singleton("Juniper:ScaledItems", () => new Map());
const start = 1;
const end = 1.1;
const timeScale = 0.005;
class ScaleState {
    constructor(target) {
        this.target = target;
        this.obj = objectResolve(this.target);
        this.base = this.obj.scale.clone();
        this.p = 0;
        this.dir = 0;
        this.running = false;
        this.wasDisabled = this.disabled;
        this.target.addScopedEventListener(this, "enter", (evt) => {
            if (evt.pointer.type !== "nose") {
                this.run(1);
            }
        });
        this.target.addScopedEventListener(this, "exit", (evt) => {
            if (evt.pointer.type !== "nose") {
                this.run(-1);
            }
        });
        this.obj.traverse(child => {
            if (isMesh(child)) {
                this.target.addMesh(child);
            }
        });
    }
    get disabled() {
        return this.target.disabled;
    }
    run(d) {
        if (!this.disabled || (d === -1 || this.p > 0)) {
            this.dir = d;
            this.running = true;
        }
    }
    updateScaling(dt) {
        if (this.disabled !== this.wasDisabled) {
            this.wasDisabled = this.disabled;
            if (this.disabled) {
                this.run(-1);
            }
        }
        if (this.running) {
            this.p += this.dir * dt;
            if (this.dir > 0 && this.p >= 1
                || this.dir < 0 && this.p < 0) {
                this.p = Math.max(0, Math.min(1, this.p));
                this.running = false;
            }
            const q = bump(this.p, 1.1);
            this.obj.scale.copy(this.base)
                .multiplyScalar(q * (end - start) + start);
        }
    }
    dispose() {
        this.target.removeScope(this);
    }
}
export function updateScalings(dt) {
    dt *= timeScale;
    for (const state of scaledItems.values()) {
        state.updateScaling(dt);
    }
}
export function removeScaledObj(obj) {
    const state = scaledItems.get(obj);
    if (state) {
        scaledItems.delete(obj);
        dispose(state);
    }
}
export function scaleOnHover(target, enabled) {
    const has = scaledItems.has(target);
    if (enabled != has) {
        if (enabled) {
            scaledItems.set(target, new ScaleState(target));
        }
        else {
            const scaler = scaledItems.get(target);
            dispose(scaler);
            scaledItems.delete(target);
        }
    }
}
//# sourceMappingURL=scaleOnHover.js.map