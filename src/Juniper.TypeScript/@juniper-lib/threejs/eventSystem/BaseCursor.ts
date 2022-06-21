import { MouseButtons } from "@juniper-lib/threejs/eventSystem/MouseButton";
import type { PointerState } from "@juniper-lib/threejs/eventSystem/PointerState";
import { ErsatzObject } from "../objects";
import { getRayTarget } from "./RayTarget";

const T = new THREE.Vector3();
const V = new THREE.Vector3();
const Q = new THREE.Quaternion();

export abstract class BaseCursor implements ErsatzObject {
    private _object: THREE.Object3D = null;
    private _visible: boolean = true;
    private _style: string = "default";

    get object() {
        return this._object;
    }

    set object(v) {
        this._object = v;
    }

    get style(): string {
        return this._style;
    }

    set style(v) {
        this._style = v;
    }

    get visible(): boolean {
        return this._visible;
    }

    set visible(v) {
        this._visible = v;
    }

    abstract get position(): THREE.Vector3;

    update(avatarHeadPos: THREE.Vector3, hit: THREE.Intersection, defaultDistance: number, canMoveView: boolean, state: PointerState, origin: THREE.Vector3, direction: THREE.Vector3) {

        if (hit && hit.face) {
            this.position.copy(hit.point);

            hit.object.getWorldQuaternion(Q);
            T.copy(hit.face.normal)
                .applyQuaternion(Q);

            V.copy(T)
                .multiplyScalar(0.02);
            this.position.add(V);

            V.copy(T)
                .multiplyScalar(10)
                .add(this.position);
        }
        else {
            this.position.copy(direction)
                .multiplyScalar(defaultDistance)
                .add(origin);
            V.copy(avatarHeadPos);
        }

        this.object.parent.worldToLocal(this.position);

        this.lookAt(V);

        const target = getRayTarget(hit);

        this.style = target
            ? !target.enabled
                ? "not-allowed"
                : target.draggable
                    ? state.dragging
                        ? "grabbing"
                        : "move"
                    : target.clickable
                        ? "pointer"
                        : "default"
            : canMoveView
                ? state.buttons === MouseButtons.Mouse0
                    ? "grabbing"
                    : "grab"
                : "default";
    }

    lookAt(_v: THREE.Vector3) {
    }
}
