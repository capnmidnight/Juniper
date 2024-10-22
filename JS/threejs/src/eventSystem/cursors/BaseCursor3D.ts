import { Intersection, Object3D, Quaternion, Vector3 } from "three";
import { BaseEnvironment } from "../../environment/BaseEnvironment";
import { ErsatzObject } from "../../objects";
import { setMatrixFromUpFwdPos } from "../../setMatrixFromUpFwdPos";
import { RayTarget } from "../RayTarget";
import { BaseCursor } from "./BaseCursor";

export abstract class BaseCursor3D
    extends BaseCursor
    implements ErsatzObject {
    private _object: Object3D = null;

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

    constructor(protected readonly env: BaseEnvironment) {
        super();
    }

    get position() {
        return this.object.position;
    }

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

    private readonly f = new Vector3();
    private readonly up = new Vector3();
    private readonly right = new Vector3();

    lookAt(p: Vector3, v: Vector3) {
        this.f
            .copy(v)
            .sub(p)
            .normalize();

        this.up
            .set(0, 1, 0)
            .applyQuaternion(this.env.avatar.worldQuat);

        this.right.crossVectors(this.up, this.f);
        this.up.crossVectors(this.f, this.right);

        setMatrixFromUpFwdPos(
            this.up,
            this.f,
            p,
            this.object.matrixWorld);

        this.object.matrix
            .copy(this.object.parent.matrixWorld)
            .invert()
            .multiply(this.object.matrixWorld);

        this.object.matrix.decompose(
            this.object.position,
            this.object.quaternion,
            this.object.scale);
        this.object.scale.x *= this.side;
        this.object.matrix.compose(
            this.object.position,
            this.object.quaternion,
            this.object.scale);
    }
}
