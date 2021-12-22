import { PointerName } from "juniper-dom/eventSystem/PointerName";
import type { VirtualButtons } from "juniper-dom/eventSystem/VirtualButtons";
import { Cube } from "../Cube";
import { green, litGrey, yellow } from "../materials";
import { ErsatzObject, obj } from "../objects";
import { setMatrixFromUpFwdPos } from "../setMatrixFromUpFwdPos";
import { BasePointer } from "./BasePointer";
import type { Cursor3D } from "./Cursor3D";
import { CursorColor } from "./CursorColor";
import type { EventSystem } from "./EventSystem";
import { Laser } from "./Laser";

export class PointerRemote
    extends BasePointer
    implements ErsatzObject {

    readonly object: THREE.Object3D;
    private laser: Laser;

    constructor(
        evtSys: EventSystem,
        userName: string,
        isInstructor: boolean,
        pointerName: PointerName,
        cursor: Cursor3D) {
        super("remote", PointerName.RemoteUser, evtSys, cursor || new CursorColor());
        this.laser = new Laser(
            isInstructor ? green : yellow,
            0.002);
        this.laser.length = 30;

        const hand = new Cube(0.05, 0.05, 0.05, litGrey);
        hand.add(this.laser);

        const elbow = new Cube(0.05, 0.05, 0.05, litGrey);

        this.object = obj(`${userName}-arm`, hand, elbow);

        if (pointerName === PointerName.Mouse) {
            hand.position.set(0, 0, -0.2);
        }
        else if (pointerName === PointerName.MotionController
            || pointerName === PointerName.MotionControllerLeft
            || pointerName === PointerName.MotionControllerRight) {
            elbow.position.set(0, 0, 0.2);
        }

        this.object.name = `remote:${userName}:${PointerName[pointerName]}`;
        this.cursor.object.name = `${this.object.name}:cursor`;
    }

    setState(
        avatarHeadPos: THREE.Vector3,
        position: THREE.Vector3,
        forward: THREE.Vector3,
        up: THREE.Vector3,
        offset: THREE.Vector3) {

        this.origin.copy(position);
        this.direction.copy(forward);
        this.cursor.visible = true;
        this.evtSys.fireRay(this);

        this.updateCursor(avatarHeadPos, this.curHit, 3);

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

    animate(dt: number) {
        this.object.position.lerp(this.pTarget, dt * 0.01);
        this.object.quaternion.slerp(this.qTarget, dt * 0.01);
    }

    update() {
        // do nothing
    }

    isPressed(_button: VirtualButtons): boolean {
        return false;
    }
}

const dumpS = new THREE.Vector3();
const M = new THREE.Matrix4();
const MW = new THREE.Matrix4();