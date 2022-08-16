import { PointerID } from "@juniper-lib/tslib";
import { Matrix4, Object3D, Vector3 } from "three";
import { AvatarRemote } from "../../AvatarRemote";
import { Cube } from "../../Cube";
import type { BaseEnvironment } from "../../environment/BaseEnvironment";
import { green, litGrey, yellow } from "../../materials";
import { ErsatzObject, obj, objGraph } from "../../objects";
import { setMatrixFromUpFwdPos } from "../../setMatrixFromUpFwdPos";
import { CursorColor } from "../cursors/CursorColor";
import { Laser } from "../Laser";
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

    constructor(
        private readonly avatar: AvatarRemote,
        env: BaseEnvironment,
        private readonly remoteID: PointerID) {

        const cursor = env.cursor3D && env.cursor3D.clone() || new CursorColor(env);

        super("remote", PointerID.RemoteUser, env, cursor);

        const hand = new Cube(0.05, 0.05, 0.05, litGrey);
        const elbow = new Cube(0.05, 0.05, 0.05, litGrey);

        this.object = obj(`remote:${this.avatar.userName}:${this.name}`,
            hand,
            elbow);

        // Fakey "inverse kinematics" arm model. Doesn't actually
        // do any IK, just make an elbow that sits behind the hand
        // which is good enough for most work.
        if (this.remoteID === PointerID.MotionController
            || this.remoteID === PointerID.MotionControllerLeft
            || this.remoteID === PointerID.MotionControllerRight) {
            elbow.position.set(0, 0, 0.2);
        }
        else {
            hand.position.set(0, 0, -0.2);
        }

        this.laser = new Laser(
            this.avatar.isInstructor ? green : yellow,
            0.002);
        this.laser.length = 30;
        objGraph(elbow, this.laser);

        this.cursor.object.name = `${this.object.name}:cursor`;
        this.cursor.visible = true;

        Object.seal(this);
    }

    setState(
        pointerPosition: Vector3,
        pointerForward: Vector3,
        pointerUp: Vector3) {

        // Target the pointer based on the remote user's perspective
        this.up.copy(pointerUp);
        this.pTarget.copy(pointerPosition).sub(this.avatar.worldPos);
        this.direction.copy(pointerForward);

        if (this.remoteID === PointerID.Mouse) {
            this.O.set(0.2, -0.6, 0)
                .applyQuaternion(this.avatar.body.quaternion);
        }
        else if (this.remoteID === PointerID.Touch) {
            this.O.set(0, -0.5, 0)
                .applyQuaternion(this.avatar.body.quaternion);
        }
        else {
            this.O.setScalar(0);
        }

        this.O.add(this.avatar.comfortOffset);
    }

    protected override onUpdate(): void {
        // do nothing
    }

    animate(dt: number) {
        this.origin.lerp(this.pTarget, dt * 0.01);

        this.origin.add(this.avatar.worldPos);

        this.P.copy(this.origin);
        this.F.copy(this.direction);        

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
            // ... but first, use the pointer length to set the laser length
            this.laser.length = 19 * this.F.length();
            this.F.normalize();
        }
        else {
            this.laser.length = 57;
        }

        // construct the LERPing targets for the pointer graphics
        setMatrixFromUpFwdPos(
            this.up,
            this.F,
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