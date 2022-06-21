import { bump } from "@juniper-lib/graphics2d/animation/tween";
import { IDisposable, singleton } from "@juniper-lib/tslib";
import { assureRayTarget, RayTarget } from "../eventSystem/RayTarget";
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
    private readonly obj: THREE.Object3D;
    private readonly target: RayTarget;
    private readonly base: THREE.Vector3;
    private readonly onEnter: () => void;
    private readonly onExit: () => void;

    private p: number;
    private dir: number;
    private running: boolean;
    private wasDisabled: boolean;

    constructor(obj: Objects) {
        this.target = assureRayTarget(obj);
        this.obj = objectResolve(obj);
        this.base = this.obj.scale.clone();
        this.p = 0;
        this.dir = 0;
        this.running = false;
        this.wasDisabled = this.disabled;
        this.onEnter = () => this.run(1);
        this.onExit = () => this.run(-1);


        this.target.addEventListener("enter", this.onEnter);
        this.target.addEventListener("exit", this.onExit);

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
                this.onExit();
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
        this.target.removeEventListener("enter", this.onEnter);
        this.target.removeEventListener("exit", this.onExit);
    }
}

export function updateScalings(dt: number) {
    dt *= timeScale;
    for (const state of scaledItems.values()) {
        state.updateScaling(dt);
    }
}

export function removeScaledObj(obj: THREE.Object3D) {
    const state = scaledItems.get(obj);
    if (state) {
        scaledItems.delete(obj);
        state.dispose();
    }
}

export function scaleOnHover(obj: Objects, enabled: boolean) {
    const has = scaledItems.has(obj);
    if (enabled != has) {
        if (enabled) {
            scaledItems.set(obj, new ScaleState(obj));;
        }
        else {
            const scaler = scaledItems.get(obj);
            scaler.dispose();
            scaledItems.delete(obj);
        }
    }
}