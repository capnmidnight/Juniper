import { Quaternion, Vector3 } from "three";
import { setMatrixFromUpFwdPos } from "../../setMatrixFromUpFwdPos";
import { BaseCursor } from "./BaseCursor";
export class BaseCursor3D extends BaseCursor {
    #content3d;
    get side() {
        return this._side;
    }
    set side(v) {
        this._side = v;
    }
    get content3d() {
        return this.#content3d;
    }
    set content3d(v) {
        this.#content3d = v;
    }
    constructor(env) {
        super();
        this.env = env;
        this.#content3d = null;
        this.T = new Vector3();
        this.V = new Vector3();
        this.Q = new Quaternion();
        this._side = -1;
        this.f = new Vector3();
        this.up = new Vector3();
        this.right = new Vector3();
    }
    get position() {
        return this.content3d.position;
    }
    update(avatarHeadPos, comfortOffset, hit, target, defaultDistance, isLocal, canDragView, canTeleport, origin, direction, isPrimaryPressed) {
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
    lookAt(p, v) {
        this.f
            .copy(v)
            .sub(p)
            .normalize();
        this.up
            .set(0, 1, 0)
            .applyQuaternion(this.env.avatar.worldQuat);
        this.right.crossVectors(this.up, this.f);
        this.up.crossVectors(this.f, this.right);
        setMatrixFromUpFwdPos(this.up, this.f, p, this.content3d.matrixWorld);
        this.content3d.matrix
            .copy(this.content3d.parent.matrixWorld)
            .invert()
            .multiply(this.content3d.matrixWorld);
        this.content3d.matrix.decompose(this.content3d.position, this.content3d.quaternion, this.content3d.scale);
        this.content3d.scale.x *= this.side;
        this.content3d.matrix.compose(this.content3d.position, this.content3d.quaternion, this.content3d.scale);
    }
}
//# sourceMappingURL=BaseCursor3D.js.map