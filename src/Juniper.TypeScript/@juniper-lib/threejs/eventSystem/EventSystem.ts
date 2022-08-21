import { arrayClear } from "@juniper-lib/tslib/collections/arrays";
import { TypedEventBase } from "@juniper-lib/tslib/events/EventBase";
import { Intersection, Mesh, Object3D, Raycaster, Vector3 } from "three";
import type { BaseEnvironment } from "../environment/BaseEnvironment";
import { FOREGROUND } from "../layers";
import { objGraph } from "../objects";
import type { IPointer } from "./devices/IPointer";
import { Pointer3DEvents } from "./devices/Pointer3DEvent";
import { PointerHand } from "./devices/PointerHand";
import { PointerMouse } from "./devices/PointerMouse";
import { PointerPen } from "./devices/PointerPen";
import { PointerTouch } from "./devices/PointerTouch";
import { getRayTarget, RayTarget } from "./RayTarget";

export class EventSystem extends TypedEventBase<Pointer3DEvents> {
    private readonly raycaster = new Raycaster();

    public readonly mouse: PointerMouse;
    private readonly pen: PointerPen;
    private readonly touches: PointerTouch;
    private readonly queue = new Array<Object3D>();
    private readonly targetsFound = new Set<RayTarget>();
    private readonly targets = new Array<Mesh>();
    readonly hands = new Array<PointerHand>();

    private readonly pointers: IPointer[];

    constructor(
        private readonly env: BaseEnvironment<unknown>) {
        super();

        this.raycaster.camera = this.env.camera;
        this.raycaster.layers.set(FOREGROUND);

        this.mouse = new PointerMouse(this.env);
        this.pen = new PointerPen(this.env);
        this.touches = new PointerTouch(this.env);

        for (let i = 0; i < 2; ++i) {
            this.hands[i] = new PointerHand(this.env, i);
        }

        this.pointers = [
            this.mouse,
            this.pen,
            this.touches,
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

    fireRay(origin: Vector3, direction: Vector3, hits: Intersection[]): void {
        this.raycaster.ray.origin.copy(origin);
        this.raycaster.ray.direction.copy(direction);
        this.raycaster.intersectObjects(this.targets, false, hits);
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
            if (pointer.needsUpdate) {
                pointer.update();
            }
        }
    }
}