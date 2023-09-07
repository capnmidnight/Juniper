import { deg2rad, minly, Pi, Tau } from "@juniper-lib/tslib/math";
import { Object3D, Quaternion, Vector3 } from "three";
import { getLookHeadingRadians } from "./lookAngles";
const targetPos = new Vector3();
let targetHeadingRadians = 0;
const dPos = new Vector3();
const curPos = new Vector3();
const curDir = new Vector3();
const dQuat = new Quaternion();
let curHeadingRadians = 0;
let copyCounter = 0;
function minRotAngle(to, from) {
    const a = to - from;
    const b = a + Tau;
    const c = a - Tau;
    return minly(a, b, c);
}
export class BodyFollower extends Object3D {
    constructor(name, minDistance, minHeadingDegrees, heightOffset, speed = 1) {
        super();
        this.minDistance = minDistance;
        this.heightOffset = heightOffset;
        this.speed = speed;
        this.name = name;
        this.lerp = this.minHeadingRadians > 0
            || this.minDistance > 0;
        this.maxDistance = this.minDistance * 5;
        this.minHeadingRadians = deg2rad(minHeadingDegrees);
        this.maxHeadingRadians = Pi - this.minHeadingRadians;
        Object.seal(this);
    }
    copy(source, recursive = true) {
        super.copy(source, recursive);
        this.name = source.name + (++copyCounter);
        this.lerp = source.lerp;
        this.maxDistance = source.maxDistance;
        this.minHeadingRadians = source.minHeadingRadians;
        this.maxHeadingRadians = source.maxHeadingRadians;
        return this;
    }
    update(height, position, headingRadians, dt) {
        dt *= 0.001;
        this.clampTo(this.lerp, height, position, this.minDistance, this.maxDistance, headingRadians, this.minHeadingRadians, this.maxHeadingRadians, dt);
    }
    reset(height, position, headingRadians) {
        this.clampTo(false, height, position, 0, 0, headingRadians, 0, 0, 0);
    }
    clampTo(lerp, height, position, minDistance, maxDistance, headingRadians, minHeadingRadians, maxHeadingRadians, dt) {
        targetPos.copy(position);
        targetPos.y -= this.heightOffset * height;
        targetHeadingRadians = headingRadians;
        this.getWorldPosition(curPos);
        this.getWorldDirection(curDir);
        curDir.negate();
        curHeadingRadians = getLookHeadingRadians(curDir);
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
            const dHeadingRadians = minRotAngle(targetHeadingRadians, curHeadingRadians);
            const mHeadingRadians = Math.abs(dHeadingRadians);
            if (minHeadingRadians < mHeadingRadians) {
                if (mHeadingRadians < maxHeadingRadians) {
                    dQuat.setFromAxisAngle(this.up, dHeadingRadians * this.speed * dt);
                }
                else {
                    dQuat.setFromAxisAngle(this.up, dHeadingRadians);
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
//# sourceMappingURL=BodyFollower.js.map