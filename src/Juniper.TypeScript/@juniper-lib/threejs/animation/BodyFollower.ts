import { deg2rad, Tau } from "@juniper-lib/tslib/math";
import { Object3D, Quaternion, Vector3 } from "three";
import { getLookHeading } from "./lookAngles";

const targetPos = new Vector3();
let targetAngle = 0;
const dPos = new Vector3();

const curPos = new Vector3();
const curDir = new Vector3();
const dQuat = new Quaternion();
let curAngle = 0;
let copyCounter = 0;

function minRotAngle(to: number, from: number) {
    const a = to - from;
    const b = a + Tau;
    const c = a - Tau;
    const A = Math.abs(a);
    const B = Math.abs(b);
    const C = Math.abs(c);
    if (A < B && A < C) {
        return a;
    }
    else if (B < C) {
        return b;
    }
    else {
        return c;
    }
}

export class BodyFollower extends Object3D {
    private lerp: boolean;
    private maxDistance: number;
    private minAngle: number;
    private maxAngle: number;

    constructor(name: string,
        private readonly minDistance: number,
        minAngle: number,
        private readonly heightOffset: number,
        private readonly speed = 1) {
        super();

        this.name = name;
        this.lerp = this.minAngle > 0
            || this.minDistance > 0;

        this.maxDistance = this.minDistance * 5;
        this.minAngle = deg2rad(minAngle);
        this.maxAngle = Math.PI - this.minAngle;

        Object.seal(this);
    }

    override copy(source: this, recursive = true): this {
        super.copy(source, recursive);
        this.name = source.name + (++copyCounter);
        this.lerp = source.lerp;
        this.maxDistance = source.maxDistance;
        this.minAngle = source.minAngle;
        this.maxAngle = source.maxAngle;
        return this;
    }

    update(height: number, position: Vector3, angle: number, dt: number) {
        dt *= 0.001;
        this.clampTo(this.lerp, height, position, this.minDistance, this.maxDistance, angle, this.minAngle, this.maxAngle, dt);
    }

    reset(height: number, position: Vector3, angle: number) {
        this.clampTo(false, height, position, 0, 0, angle, 0, 0, 0);
    }

    private clampTo(
        lerp: boolean,
        height: number,
        position: Vector3,
        minDistance: number,
        maxDistance: number,
        angle: number,
        minAngle: number,
        maxAngle: number,
        dt: number) {
        targetPos.copy(position);
        targetPos.y -= this.heightOffset * height;
        targetAngle = angle;

        this.getWorldPosition(curPos);
        this.getWorldDirection(curDir);
        curDir.negate();

        curAngle = getLookHeading(curDir);
        dQuat.identity();

        let setPos = !lerp;
        let setRot = !lerp;
        if (lerp) {
            const dist = dPos.copy(targetPos)
                .sub(curPos)
                .length();

            if (minDistance < dist) {
                if (dist < maxDistance) {
                    targetPos.lerpVectors(curPos, targetPos, this.speed * dt);
                }
                setPos = true;
            }

            const dAngle = minRotAngle(targetAngle, curAngle);
            const mAngle = Math.abs(dAngle);
            if (minAngle < mAngle) {
                if (mAngle < maxAngle) {
                    dQuat.setFromAxisAngle(this.up, dAngle * this.speed * dt);
                }
                else {
                    dQuat.setFromAxisAngle(this.up, dAngle);
                }
                setRot = true;
            }
        }

        if (setPos || setRot) {

            if (setPos) {
                this.position.add(targetPos
                    .sub(curPos));
            }

            if (setRot) {
                this.quaternion.multiply(dQuat);
            }
        }
    }
}