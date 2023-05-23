import { bump } from "@juniper-lib/graphics2d/animation/tween";
import { singleton } from "@juniper-lib/tslib/singleton";
import { IDisposable, dispose } from "@juniper-lib/tslib/using";
import { Object3D, Vector3 } from "three";
import { RayTarget } from "../eventSystem/RayTarget";
import { objectResolve, Objects } from "../objects";
import { isMesh } from "../typeChecks";

/**
 * This is a hack to make sure all Applications get the same scaleOnHover state as the Environment.
 **/
const scaledItems = singleton("Juniper:ScaledItems", () => new Map<Objects, ScaleState>());
const start = 1;
const end = 1.1;
const timeScale = 0.005;

class ScaleState implements IDisposable {
    private readonly obj: Object3D;
    private readonly base: Vector3;

    private p: number;
    private dir: number;
    private running: boolean;
    private wasDisabled: boolean;

    constructor(private readonly target: RayTarget) {
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

    private get disabled() {
        return this.target.disabled;
    }

    private run(d: number) {
        if (!this.disabled || (d === -1 || this.p > 0)) {
            this.dir = d;
            this.running = true;
        }
    }

    updateScaling(dt: number) {
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

export function updateScalings(dt: number) {
    dt *= timeScale;
    for (const state of scaledItems.values()) {
        state.updateScaling(dt);
    }
}

export function removeScaledObj(obj: Object3D) {
    const state = scaledItems.get(obj);
    if (state) {
        scaledItems.delete(obj);
        dispose(state);
    }
}

export function scaleOnHover(target: RayTarget<any>, enabled: boolean) {
    const has = scaledItems.has(target);
    if (enabled != has) {
        if (enabled) {
            scaledItems.set(target, new ScaleState(target));;
        }
        else {
            const scaler = scaledItems.get(target);
            dispose(scaler);
            scaledItems.delete(target);
        }
    }
}