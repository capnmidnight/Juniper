import { Matrix4, Vector3 } from "three";
import { HANDEDNESSES } from "../../BaseTele";
import { cleanup } from "../../cleanup";
import { Cube } from "../../Cube";
import { green, litGrey, yellow } from "../../materials";
import { obj, objGraph } from "../../objects";
import { setMatrixFromUpFwdPos } from "../../setMatrixFromUpFwdPos";
import { CursorColor } from "../cursors/CursorColor";
import { Laser } from "../Laser";
import { getPointerType, PointerID } from "../Pointers";
import { BasePointer } from "./BasePointer";
const ARM_LENGTH = 0.2;
const ARM_DIST = 0.5 * ARM_LENGTH - 0.025;
const ARM_WIDTH = 0.05;
export class PointerRemote extends BasePointer {
    constructor(avatar, env, remoteID) {
        const cursor = env.cursor3D && env.cursor3D.clone() || new CursorColor(env);
        super("remote", PointerID.RemoteUser, env, cursor);
        this.avatar = avatar;
        this.remoteID = remoteID;
        this.laser = null;
        this.D = new Vector3();
        this.P = new Vector3();
        this.visualOffset = new Vector3();
        this.M = new Matrix4();
        this.MW = new Matrix4();
        this.originTarget = new Vector3();
        this.directionTarget = new Vector3();
        this.upTarget = new Vector3();
        this.visualOffsetTarget = new Vector3();
        this._hand = null;
        this.killTimeout = null;
        this.remoteType = getPointerType(this.remoteID);
        this.content3d = obj(`remote:${this.avatar.userName}:${this.name}`, this.laser = new Laser(this.avatar.isInstructor ? green : yellow, this.avatar.isInstructor ? 1 : 0.5, 0.002), this.handCube = new Cube(ARM_WIDTH, ARM_WIDTH, ARM_LENGTH, litGrey));
        this.cursor.content3d.name = `${this.content3d.name}:cursor`;
        this.cursor.visible = true;
        // Fakey "inverse kinematics" arm model. Doesn't actually
        // do any IK, just make an elbow that sits behind the hand
        // which is good enough for most work.
        this.handCube.position.set(0, 0, -ARM_DIST);
        this.laser.length = 30;
        Object.seal(this);
    }
    get hand() {
        return this._hand;
    }
    set hand(v) {
        if (v !== this.hand) {
            if (this.hand) {
                this.hand.removeFromParent();
                cleanup(this.hand);
            }
            this._hand = v;
            if (this.hand) {
                this.handCube.removeFromParent();
                objGraph(this.avatar.stage, this.hand);
            }
            else {
                objGraph(this.avatar.stage, this.handCube);
            }
        }
    }
    readState(buffer) {
        // base pointer
        buffer.readVector48(this.originTarget);
        buffer.readVector48(this.directionTarget);
        buffer.readVector48(this.upTarget);
        if (this.remoteID === PointerID.Mouse
            || this.remoteID === PointerID.Touch) {
            this.visualOffsetTarget.set(0.25, -0.55, 0.05)
                .applyQuaternion(this.avatar.bodyQuaternion);
        }
        else {
            this.visualOffsetTarget.setScalar(0);
        }
        this.visualOffsetTarget.add(this.avatar.comfortOffset);
        // PointerHand
        if (PointerID.MotionController <= this.remoteID && this.remoteID <= PointerID.MotionControllerRight) {
            const handedness = buffer.readEnum8(HANDEDNESSES);
            const numFingerJoints = buffer.readUint8();
            if (numFingerJoints === 0) {
                if (this.hand) {
                    this.hand = null;
                }
            }
            else {
                if (!this.hand) {
                    this.hand = this.env.handModelFactory.createHandModel(handedness);
                }
                this.hand.count = numFingerJoints;
                for (let n = 0; n < numFingerJoints; ++n) {
                    buffer.readMatrix512(this.M);
                    this.hand.setMatrixAt(n, this.M);
                }
                this.hand.updateMesh();
                this.deferExecution(1, () => this.hand = null);
            }
        }
    }
    deferExecution(killTime, killAction) {
        if (this.killTimeout !== null) {
            clearTimeout(this.killTimeout);
            this.killTimeout = null;
        }
        this.killTimeout = setTimeout(() => {
            this.killTimeout = null;
            killAction();
        }, killTime * 1000);
    }
    writeState(_buffer) {
        // do nothing
    }
    onUpdate() {
        // do nothing
    }
    animate(dt) {
        // LERP the targets for the pointer graphics
        const lt = dt * 0.01;
        this.origin.lerp(this.originTarget, lt);
        this.direction.lerp(this.directionTarget, lt).normalize();
        this.up.lerp(this.upTarget, lt).normalize();
        this.visualOffset.lerp(this.visualOffsetTarget, lt);
        this.cursor.visible = this.env.avatar.worldPos.distanceTo(this.origin) < 10;
        if (this.cursor.visible) {
            // See if anything was hit...
            this.fireRay(this.origin, this.direction);
            // Move the cursor to wherever the pointer is pointing.
            this.updateCursor(this.avatar.worldPos, this.avatar.comfortOffset, false, 4);
        }
        this.P.copy(this.origin).add(this.visualOffset);
        if (this.cursor.visible) {
            // point the pointer at the cursor
            this.cursor.content3d
                .getWorldPosition(this.D)
                .sub(this.P);
            this.laser.length = this.D.length() - 0.1;
            this.D.normalize();
        }
        else {
            this.D.copy(this.direction);
            this.laser.length = 10;
        }
        // Orient the graphics
        setMatrixFromUpFwdPos(this.up, this.D, this.P, this.MW);
        this.M
            .copy(this.content3d.parent.matrixWorld)
            .invert()
            .multiply(this.MW)
            .decompose(this.content3d.position, this.content3d.quaternion, this.content3d.scale);
    }
    updatePointerOrientation() {
        // do nothing
    }
    vibrate() {
        // do nothing
    }
}
//# sourceMappingURL=PointerRemote.js.map