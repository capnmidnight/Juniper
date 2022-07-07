import { BaseEnvironment } from "../environment/BaseEnvironment";
import type { ErsatzObject } from "../objects";
import type { RayTarget } from "./RayTarget";

export abstract class BaseCursor implements ErsatzObject {
    private _object: THREE.Object3D = null;
    private _visible: boolean = true;
    private _style: string = "default";

    private readonly T = new THREE.Vector3();
    private readonly V = new THREE.Vector3();
    private readonly Q = new THREE.Quaternion();

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

    constructor(protected readonly env: BaseEnvironment) {

    }

    abstract get position(): THREE.Vector3;

    update(avatarHeadPos: THREE.Vector3, hit: THREE.Intersection, target: RayTarget, defaultDistance: number, canMoveView: boolean, origin: THREE.Vector3, direction: THREE.Vector3, isPrimaryPressed: boolean) {

        if (hit && hit.face) {
            this.position.copy(hit.point);

            hit.object.getWorldQuaternion(this.Q);
            this.T.copy(hit.face.normal)
                .applyQuaternion(this.Q);

            this.V.copy(this.T)
                .multiplyScalar(0.02);
            this.position.add(this.V);

            this.V.copy(this.T)
                .multiplyScalar(10)
                .add(this.position);
        }
        else {
            this.position
                .copy(direction)
                .multiplyScalar(10000)
                .add(origin)
                .sub(avatarHeadPos)
                .normalize()
                .multiplyScalar(defaultDistance)
                .add(avatarHeadPos);

            this.V.copy(avatarHeadPos);
        }

        this.object.parent.worldToLocal(this.position);

        this.lookAt(this.V);

        this.style = target
            ? !target.enabled
                ? "not-allowed"
                : target.draggable
                    ? isPrimaryPressed
                        ? "grabbing"
                        : "move"
                    : target.clickable
                        ? "pointer"
                        : "default"
            : canMoveView
                ? isPrimaryPressed
                    ? "grabbing"
                    : "grab"
                : "default";
    }

    lookAt(_v: THREE.Vector3) {
    }
}
