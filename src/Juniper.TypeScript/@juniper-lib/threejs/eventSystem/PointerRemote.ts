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
        id: PointerID,
        cursor: Cursor3D) {
        super("remote", PointerID.RemoteUser, env, cursor || new CursorColor());
        this.laser = new Laser(
            isInstructor ? green : yellow,
            0.002);
        this.laser.length = 30;

        const hand = new Cube(0.05, 0.05, 0.05, litGrey);
        objGraph(hand, this.laser);

        const elbow = new Cube(0.05, 0.05, 0.05, litGrey);

        this.object = obj(`remote:${userName}:${this.name}`, hand, elbow);

        if (id === PointerID.Mouse) {
            hand.position.set(0, 0, -0.2);
        }
        else if (id === PointerID.MotionController
            || id === PointerID.MotionControllerLeft
            || id === PointerID.MotionControllerRight) {
            elbow.position.set(0, 0, 0.2);
        }

        this.cursor.object.name = `${this.object.name}:cursor`;
    }

    setState(
        _avatarHeadPos: THREE.Vector3,
        avatarComfortOffset: THREE.Vector3,
        pointerPosition: THREE.Vector3,
        pointerForward: THREE.Vector3,
        pointerUp: THREE.Vector3,
        pointerComfortOffset: THREE.Vector3) {

        this.origin.copy(pointerPosition);
        this.direction.copy(pointerForward);
        this.up.copy(pointerUp);

        if (!this.fireRay(this.origin, this.direction)) {
            this.origin.copy(this.env.avatar.worldPos);
            this.fireRay(this.origin, this.direction);
        }

        this.cursor.visible = true;
        this.updateCursor(3);

        pointerPosition.add(pointerComfortOffset);

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