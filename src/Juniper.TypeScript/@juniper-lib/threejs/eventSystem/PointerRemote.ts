import { PointerID } from "@juniper-lib/tslib";
import { Cube } from "../Cube";
import type { BaseEnvironment } from "../environment/BaseEnvironment";
import { green, litGrey, yellow } from "../materials";
import { ErsatzObject, obj, objGraph } from "../objects";
import { setMatrixFromUpFwdPos } from "../setMatrixFromUpFwdPos";
import { BasePointer } from "./BasePointer";
import type { Cursor3D } from "./Cursor3D";
import { CursorColor } from "./CursorColor";
import { Laser } from "./Laser";

export class PointerRemote
    extends BasePointer
    implements ErsatzObject {

    readonly object: THREE.Object3D;

    private readonly laser: Laser = null;
    private readonly dumpS = new THREE.Vector3();
    private readonly M = new THREE.Matrix4();
    private readonly MW = new THREE.Matrix4();
    private readonly pTarget = new THREE.Vector3();
    private readonly qTarget = new THREE.Quaternion().identity();

    constructor(
        env: BaseEnvironment,
        userName: string,
        isInstructor: boolean,
        private readonly remoteID: PointerID,
        cursor: Cursor3D) {
        super("remote", PointerID.RemoteUser, env, cursor || new CursorColor(env));

        const hand = new Cube(0.05, 0.05, 0.05, litGrey);
        const elbow = new Cube(0.05, 0.05, 0.05, litGrey);

        this.object = obj(`remote:${userName}:${this.name}`,
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
            isInstructor ? green : yellow,
            0.002);
        this.laser.length = 30;
        objGraph(elbow, this.laser);

        this.cursor.object.name = `${this.object.name}:cursor`;
        this.cursor.visible = true;

        Object.seal(this);
    }

    setState(
        pointerPosition: THREE.Vector3,
        pointerForward: THREE.Vector3,
        pointerUp: THREE.Vector3,
        comfortOffset: THREE.Vector3) {

        // Target the pointer based on the remote user's perspective
        this.up.copy(pointerUp);
        this.origin.copy(pointerPosition);
        this.direction.copy(pointerForward);

        // position the pointer graphics origin
        pointerPosition.add(comfortOffset);

        this.cursor.visible = this.env.avatar.worldPos.distanceTo(pointerPosition) < 10;
        if (this.cursor.visible) {
            // See if anything was hit...
            if (!this.fireRay(this.origin, this.direction)) {
                // ... if nothing was hit, target the background from
                // the local user's perspective.
                this.origin.copy(this.env.avatar.worldPos);
            }

            // Move the cursor to wherever the pointer is pointing.
            this.updateCursor(3);

            // point the pointer at the cursor
            this.cursor.object.getWorldPosition(pointerForward);
            pointerForward.sub(pointerPosition);
            // ... but first, use the pointer length to set the laser length
            this.laser.length = 19 * pointerForward.length();
            pointerForward.normalize();
        }
        else {
            this.laser.length = 57;
        }

        // construct the LERPing targets for the pointer graphics
        setMatrixFromUpFwdPos(
            pointerUp,
            pointerForward,
            pointerPosition,
            this.MW);
        this.M
            .copy(this.object.parent.matrixWorld)
            .invert()
            .multiply(this.MW)
            .decompose(
                this.pTarget,
                this.qTarget,
                this.dumpS);
    }

    protected override onUpdate(): void {
        // do nothing
    }

    animate(dt: number) {
        this.object.position.lerp(this.pTarget, dt * 0.01);
        this.object.quaternion.slerp(this.qTarget, dt * 0.01);
    }

    updatePointerOrientation() {
        // do nothing
    }

    vibrate() {
        // do nothing
    }
}