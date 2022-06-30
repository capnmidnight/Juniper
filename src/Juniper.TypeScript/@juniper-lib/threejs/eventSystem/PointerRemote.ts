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
    private laser: Laser;

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
        avatarHeadPos: THREE.Vector3,
        position: THREE.Vector3,
        forward: THREE.Vector3,
        up: THREE.Vector3,
        offset: THREE.Vector3) {

        this.origin
            .copy(this.env.avatar.worldPos)
            .add(position)
            .sub(avatarHeadPos);
        this.direction.copy(forward);
        this.up.copy(up);

        this.cursor.visible = true;
        this.fireRay();

        this.updateCursor(avatarHeadPos, this.curHit, this.curTarget, 3);

        position.add(offset);

        if (this.curHit) {
            forward.copy(this.curHit.point)
                .sub(position)
                .normalize();
        }

        setMatrixFromUpFwdPos(up, forward, position, MW);
        M.copy(this.object.parent.matrixWorld)
            .invert()
            .multiply(MW)
            .decompose(
                this.pTarget,
                this.qTarget,
                dumpS);
    }

    private readonly pTarget = new THREE.Vector3();
    private readonly qTarget = new THREE.Quaternion().identity();

    protected onUpdate(): void { }

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

const dumpS = new THREE.Vector3();
const M = new THREE.Matrix4();
const MW = new THREE.Matrix4();