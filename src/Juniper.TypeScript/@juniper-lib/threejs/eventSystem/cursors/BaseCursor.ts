import { Intersection, Object3D, Quaternion, Vector3 } from "three";
import { BaseEnvironment } from "../../environment/BaseEnvironment";
import type { ErsatzObject } from "../../objects";
import type { RayTarget } from "../RayTarget";

export abstract class BaseCursor implements ErsatzObject {
    private _object: Object3D = null;
    private _visible: boolean = true;
    private _style: CSSCursorValue = "default";

    private readonly T = new Vector3();
    private readonly V = new Vector3();
    private readonly Q = new Quaternion();

    private _side = -1;

    get side() {
        return this._side;
    }

    set side(v) {
        this._side = v;
    }

    get object() {
        return this._object;
    }

    set object(v) {
        this._object = v;
    }

    get style(): CSSCursorValue {
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

    abstract get position(): Vector3;

    update(
        avatarHeadPos: Vector3,
        comfortOffset: Vector3,
        hit: Intersection,
        target: RayTarget,
        defaultDistance: number,
        isLocal: boolean,
        canDragView: boolean,
        canTeleport: boolean,
        origin: Vector3,
        direction: Vector3,
        isPrimaryPressed: boolean) {

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
            if (isLocal) {
                this.position
                    .copy(direction)
                    .multiplyScalar(2)
                    .add(origin)
                    .sub(this.env.avatar.worldPos)
                    .normalize()
                    .multiplyScalar(defaultDistance)
                    .add(this.env.avatar.worldPos);
            }
            else {
                this.V.copy(origin)
                    .add(comfortOffset)
                    .sub(avatarHeadPos)
                    .multiplyScalar(2); // why 2? I don't know.

                this.position
                    .copy(direction)
                    .multiplyScalar(defaultDistance)
                    .add(this.V)
                    .add(this.env.avatar.worldPos);
            }

            this.V.copy(this.env.avatar.worldPos);
        }

        this.lookAt(this.position, this.V);

        this.style = !target || (target.navigable && !canTeleport)
            ? canDragView
                ? isPrimaryPressed
                    ? "grabbing"
                    : "grab"
                : "default"
            : !target.enabled
                ? "not-allowed"
                : target.draggable
                    ? isPrimaryPressed
                        ? "grabbing"
                        : "move"
                    : target.navigable
                        ? "cell"
                        : target.clickable
                            ? "pointer"
                            : "default";
    }

    lookAt(_p: Vector3, _v: Vector3) {
    }
}
