import { bump } from "@juniper-lib/graphics2d/animation/tween";
import { IDisposable, singleton } from "@juniper-lib/tslib";
import { getObjectTargets, makeRayTarget } from "../eventSystem/RayTarget";
import { isMesh } from "../typeChecks";

/**
 * This is a hack to make sure all Applications get the same scaleOnHover state as the Environment.
 **/
const scaledItems = singleton("Juniper:ScaledItems", () => new Map<THREE.Object3D, ScaleState>());
const start = 1;
const end = 1.1;
const timeScale = 0.005;

class ScaleState implements IDisposable {
    private readonly base: THREE.Vector3;
    private readonly onEnter: () => void;
    private readonly onExit: () => void;

    private p: number;
    private dir: number;
    private running: boolean;
    private wasDisabled: boolean;

    constructor(private readonly obj: THREE.Object3D) {
        this.base = obj.scale.clone();
        this.p = 0;
        this.dir = 0;
        this.running = false;
        this.wasDisabled = this.disabled;
        this.onEnter = () => this.run(1);
        this.onExit = () => this.run(-1);

        this.obj.traverse(child => {
            if (isMesh(child)) {
                const target = makeRayTarget(child, this.obj);
                target.addEventListener("enter", this.onEnter);
                target.addEventListener("exit", this.onExit);
            }
        });
    }

    private get enabled() {
        const targets = getObjectTargets(this.obj);
        if (!targets || targets.length === 0) {
            return false;
        }

        for (const target of targets) {
            if (!target.enabled) {
                return false;
            }
        }

        return true;
    }

    private get disabled() {
        return !this.enabled;
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
        this.obj.removeEventListener("enter", this.onEnter);
        this.obj.removeEventListener("exit", this.onExit);
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

export function scaleOnHover(obj: THREE.Object3D) {
    scaledItems.set(obj, new ScaleState(obj));
}