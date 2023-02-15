import { Matrix4, Object3D, Vector3 } from "three";
import { AvatarRemote } from "../../AvatarRemote";
import { HANDEDNESSES } from "../../BaseTele";
import { BufferReaderWriter } from "../../BufferReaderWriter";
import { cleanup } from "../../cleanup";
import { Cube } from "../../Cube";
import type { BaseEnvironment } from "../../environment/BaseEnvironment";
import { IXRHandModel } from "../../examples/webxr/XRHandModelFactory";
import { green, litGrey, yellow } from "../../materials";
import { ErsatzObject, obj, objGraph } from "../../objects";
import { setMatrixFromUpFwdPos } from "../../setMatrixFromUpFwdPos";
import { CursorColor } from "../cursors/CursorColor";
import { Laser } from "../Laser";
import { getPointerType, PointerID, PointerType } from "../Pointers";
import { BasePointer } from "./BasePointer";

export class PointerRemote
    extends BasePointer
    implements ErsatzObject {

    readonly object: Object3D;

    private readonly laser: Laser = null;
    private readonly P = new Vector3();
    private readonly F = new Vector3();
    private readonly O = new Vector3();
    private readonly S = new Vector3();
    private readonly M = new Matrix4();
    private readonly MW = new Matrix4();
    private readonly pTarget = new Vector3();
    private readonly dTarget = new Vector3();
    private readonly uTarget = new Vector3();
    private readonly handCube: Object3D;
    private readonly elbowCube: Object3D;
    public readonly remoteType: PointerType;
    private _hand: IXRHandModel = null;

    constructor(
        private readonly avatar: AvatarRemote,
        env: BaseEnvironment,
        private readonly remoteID: PointerID,
        private readonly handsParent: Object3D) {

        const cursor = env.cursor3D && env.cursor3D.clone() || new CursorColor(env);

        super("remote", PointerID.RemoteUser, env, cursor);

        this.remoteType = getPointerType(this.remoteID);

        this.object = obj(`remote:${this.avatar.userName}:${this.name}`,
            this.elbowCube = new Cube(0.05, 0.05, 0.05, litGrey),
            this.laser = new Laser(
                this.avatar.isInstructor ? green : yellow,
                this.avatar.isInstructor ? 1 : 0.5,
                0.002),
            this.handCube = new Cube(0.05, 0.05, 0.05, litGrey));

        this.cursor.object.name = `${this.object.name}:cursor`;
        this.cursor.visible = true;

        // Fakey "inverse kinematics" arm model. Doesn't actually
        // do any IK, just make an elbow that sits behind the hand
        // which is good enough for most work.
        this.handCube.position.set(0, 0, -0.2);

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
                this.elbowCube.removeFromParent();
                objGraph(this,
                    this.laser);
                objGraph(this.handsParent,
                    this.hand);
            }
            else {
                objGraph(this,
                    this.elbowCube,
                    this.laser,
                    this.handCube);
            }
        }
    }

    readState(buffer: BufferReaderWriter) {
        // base pointer
        buffer.readVector48(this.pTarget);
        buffer.readVector48(this.dTarget);
        buffer.readVector48(this.uTarget);

        this.pTarget.sub(this.avatar.worldPos);

        if (this.remoteID === PointerID.Mouse
            || this.remoteID === PointerID.Touch) {
            this.O.set(0.25, -0.55, 0.05)
                .applyQuaternion(this.avatar.bodyQuaternion);
        }
        else {
            this.O.setScalar(0);
        }

        this.O.add(this.avatar.comfortOffset);

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

    private killTimeout: number = null;
    public deferExecution(killTime: number, killAction: () => void) {
        if (this.killTimeout !== null) {
            clearTimeout(this.killTimeout);
            this.killTimeout = null;
        }

        this.killTimeout = setTimeout(() => {
            this.killTimeout = null
            killAction();
        }, killTime * 1000) as any;
    }

    override writeState(_buffer: BufferReaderWriter) {
        // do nothing
    }

    protected override onUpdate(): void {
        // do nothing
    }

    animate(dt: number) {
        this.origin.lerp(this.pTarget, dt * 0.01);
        this.direction.lerp(this.dTarget, dt * 0.01).normalize();
        this.up.lerp(this.uTarget, dt * 0.01).normalize();

        // subtract this back out at the end of the animation phase so it can be used for pointer updates correctly
        this.origin.add(this.avatar.worldPos);

        this.P.copy(this.origin);

        this.cursor.visible = this.env.avatar.worldPos.distanceTo(this.P) < 10;
        if (this.cursor.visible) {
            // See if anything was hit...
            this.fireRay(this.origin, this.direction);

            // Move the cursor to wherever the pointer is pointing.
            this.updateCursor(this.avatar.worldPos, this.avatar.comfortOffset, false, 4);

            // point the pointer at the cursor
            this.cursor.object.getWorldPosition(this.F)
                .sub(this.P)
                .sub(this.O);
            // set the laser length
            this.laser.length = 19 * this.F.length();
        }
        else {
            this.laser.length = 30;
        }

        // construct the LERPing targets for the pointer graphics
        setMatrixFromUpFwdPos(
            this.up,
            this.direction,
            this.P.add(this.O),
            this.MW);
        this.M
            .copy(this.object.parent.matrixWorld)
            .invert()
            .multiply(this.MW)
            .decompose(
                this.object.position,
                this.object.quaternion,
                this.S);

        this.origin.sub(this.avatar.worldPos);
    }

    updatePointerOrientation() {
        // do nothing
    }

    vibrate() {
        // do nothing
    }
}