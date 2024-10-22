import { arrayClear } from "@juniper-lib/util";
import { TypedEventTarget } from "@juniper-lib/events";
import { Raycaster } from "three";
import { FOREGROUND } from "../layers";
import { objGraph } from "../objects";
import { PointerHand } from "./devices/PointerHand";
import { PointerMouse } from "./devices/PointerMouse";
import { PointerNose } from "./devices/PointerNose";
import { PointerPen } from "./devices/PointerPen";
import { PointerTouch } from "./devices/PointerTouch";
import { getRayTarget } from "./RayTarget";
const defaultSortFunction = (a, b) => a.distance - b.distance;
export class EventSystem extends TypedEventTarget {
    set sortFunction(func) {
        this.customSortFunction = func;
    }
    get sortFunction() {
        return this.customSortFunction
            || defaultSortFunction;
    }
    constructor(env) {
        super();
        this.env = env;
        this.raycaster = new Raycaster();
        this.hands = new Array();
        this.hits = new Array();
        this.queue = new Array();
        this.targetsFound = new Set();
        this.targets = new Array();
        this.customSortFunction = null;
        this.raycaster.camera = this.env.camera;
        this.raycaster.layers.set(FOREGROUND);
        this.mouse = new PointerMouse(this.env);
        this.pen = new PointerPen(this.env);
        this.touches = new PointerTouch(this.env);
        this.nose = new PointerNose(this.env);
        for (let i = 0; i < 2; ++i) {
            this.hands[i] = new PointerHand(this.env, i);
        }
        this.pointers = [
            this.mouse,
            this.pen,
            this.touches,
            this.nose,
            ...this.hands
        ];
        for (const pointer of this.pointers) {
            pointer.addBubbler(this);
            if (pointer.cursor) {
                objGraph(this.env.stage, pointer.cursor);
            }
        }
        this.checkXRMouse();
        Object.seal(this);
    }
    checkXRMouse() {
        let count = 0;
        for (const hand of this.hands.values()) {
            if (hand.enabled) {
                ++count;
            }
        }
        const enableScreenPointers = count === 0;
        this.mouse.enabled = enableScreenPointers;
        this.pen.enabled = enableScreenPointers;
        this.touches.enabled = enableScreenPointers;
    }
    refreshCursors() {
        for (const pointer of this.pointers) {
            if (pointer.cursor) {
                pointer.cursor = this.env.cursor3D.clone();
            }
        }
    }
    fireRay(origin, direction) {
        arrayClear(this.hits);
        this.raycaster.ray.origin.copy(origin);
        this.raycaster.ray.direction.copy(direction);
        this.raycaster.intersectObjects(this.targets, false, this.hits);
        this.hits.sort(this.sortFunction);
        return this.hits[0] || null;
    }
    update() {
        this.targetsFound.clear();
        arrayClear(this.targets);
        this.queue.push(this.env.scene);
        while (this.queue.length > 0) {
            const here = this.queue.shift();
            if (here.children.length > 0) {
                this.queue.push(...here.children);
            }
            const target = getRayTarget(here);
            if (target && !this.targetsFound.has(target)) {
                this.targetsFound.add(target);
                this.targets.push(...target.meshes);
            }
        }
        for (const pointer of this.pointers) {
            pointer.update();
        }
    }
}
//# sourceMappingURL=EventSystem.js.map