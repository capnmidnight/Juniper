import { TypedEventBase } from "@juniper-lib/tslib";
import type { BaseEnvironment } from "../environment/BaseEnvironment";
import { FOREGROUND } from "../layers";
import { objGraph } from "../objects";
import { Pointer3DEvents } from "./Pointer3DEvent";
import type { IPointer } from "./IPointer";
import { PointerHand } from "./PointerHand";
import { PointerMouse } from "./PointerMouse";
import { PointerTouch } from "./PointerTouch";
import { PointerPen } from "./PointerPen";

export class PointerManager extends TypedEventBase<Pointer3DEvents> {
    private readonly raycaster = new THREE.Raycaster();

    public readonly mouse: PointerMouse;
    private readonly pen: PointerPen;
    private readonly touches: PointerTouch;
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
            if (pointer instanceof TypedEventBase) {
                pointer.addBubbler(this);
            }

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

    fireRay(origin: THREE.Vector3, direction: THREE.Vector3, hits: THREE.Intersection[]): void {
        this.raycaster.ray.origin.copy(origin);
        this.raycaster.ray.direction.copy(direction);
        this.raycaster.intersectObject(this.env.scene, true, hits);
    }

    update() {
        for (const pointer of this.pointers) {
            if (pointer.needsUpdate) {
                pointer.update();
            }
        }
    }
}